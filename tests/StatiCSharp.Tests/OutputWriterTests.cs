using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace StatiCSharp.Tests;

public class OutputWriterTests
{
    [Fact]
    public async Task AFileCanOnlyBeWrittenOncePerRun()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        Assert.True(await output.WriteAsync(directory.Path, "index.html", "first"));
        Assert.False(await output.WriteAsync(directory.Path, "index.html", "second"));

        // The later site still wins, which is why the caller reports the collision.
        Assert.Equal("second", await File.ReadAllTextAsync(
            Path.Combine(directory.Path, "index.html"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task TwoWritersDoNotShareTheirBookkeeping()
    {
        // This is the point of creating one per run. As a field on the manager the records
        // survived, so a second MakeAsync considered every path from the first run taken and
        // warned about all of them.
        using var directory = new TempDirectory();

        var first = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);
        var second = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        Assert.True(await first.WriteAsync(directory.Path, "index.html", "x"));
        Assert.True(await second.WriteAsync(directory.Path, "index.html", "x"));
    }

    [Fact]
    public async Task WriteAsyncWritesTheFile()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        await output.WriteAsync(directory.Path, "index.html", "<p>x</p>");

        Assert.Equal("<p>x</p>", await File.ReadAllTextAsync(
            Path.Combine(directory.Path, "index.html"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WithoutIncrementalWritingAnUnchangedFileIsStillTouched()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);
        string file = Path.Combine(directory.Path, "index.html");

        await output.WriteAsync(directory.Path, "index.html", "same");
        DateTime first = File.GetLastWriteTimeUtc(file);

        File.SetLastWriteTimeUtc(file, first.AddDays(-1));
        await output.WriteAsync(directory.Path, "index.html", "same");

        Assert.NotEqual(first.AddDays(-1), File.GetLastWriteTimeUtc(file));
    }

    [Fact]
    public async Task WithIncrementalWritingAnUnchangedFileLeavesItAlone()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        string file = Path.Combine(directory.Path, "index.html");

        await output.WriteAsync(directory.Path, "index.html", "same");

        DateTime marker = File.GetLastWriteTimeUtc(file).AddDays(-1);
        File.SetLastWriteTimeUtc(file, marker);
        await output.WriteAsync(directory.Path, "index.html", "same");

        Assert.Equal(marker, File.GetLastWriteTimeUtc(file));
    }

    [Fact]
    public async Task AnUnchangedFileStillCountsAsProduced()
    {
        // The skipped write must not make the clean up think the file is an orphan.
        using var directory = new TempDirectory();
        directory.WriteFile("Output/index.html", "same");

        string outputPath = Path.Combine(directory.Path, "Output");
        var output = new OutputWriter(outputPath, onlyWriteWhatChanged: true);

        await output.WriteAsync(outputPath, "index.html", "same");
        await output.CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "index.html")));
    }

    [Fact]
    public async Task WithIncrementalAChangedFileIsWritten()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);

        await output.WriteAsync(directory.Path, "index.html", "before");
        await output.WriteAsync(directory.Path, "index.html", "after");

        Assert.Equal("after", await File.ReadAllTextAsync(
            Path.Combine(directory.Path, "index.html"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public void ClearEmptiesTheOutputButKeepsTheDirectory()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/index.html", "x");
        directory.WriteFile("Output/posts/index.html", "y");

        string outputPath = Path.Combine(directory.Path, "Output");
        new OutputWriter(outputPath, onlyWriteWhatChanged: false).Clear();

        Assert.True(Directory.Exists(outputPath));
        Assert.Empty(Directory.GetFileSystemEntries(outputPath));
    }

    [Fact]
    public void ClearKeepsThePreservedNames()
    {
        // An output directory often is the repository it is deployed from. Clearing it used
        // to delete .git with it.
        using var directory = new TempDirectory();
        directory.WriteFile("Output/.git/HEAD", "ref: refs/heads/main");
        directory.WriteFile("Output/CNAME", "example.com");
        directory.WriteFile("Output/.nojekyll", "");
        directory.WriteFile("Output/index.html", "x");

        string outputPath = Path.Combine(directory.Path, "Output");
        new OutputWriter(outputPath, onlyWriteWhatChanged: false).Clear();

        Assert.True(File.Exists(Path.Combine(outputPath, ".git", "HEAD")));
        Assert.True(File.Exists(Path.Combine(outputPath, "CNAME")));
        Assert.True(File.Exists(Path.Combine(outputPath, ".nojekyll")));
        Assert.False(File.Exists(Path.Combine(outputPath, "index.html")));
    }

    [Fact]
    public async Task CopyIntoOutputCopiesFilesAndSubdirectories()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Resources/favicon.png", "icon");
        directory.WriteFile("Resources/theme/styles.css", "css");

        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);

        await new OutputWriter(outputPath, onlyWriteWhatChanged: false)
            .CopyIntoOutputAsync(Path.Combine(directory.Path, "Resources"));

        Assert.True(File.Exists(Path.Combine(outputPath, "favicon.png")));
        Assert.True(File.Exists(Path.Combine(outputPath, "theme", "styles.css")));
    }

    [Fact]
    public async Task CopyIntoOutputRejectsAMissingSource()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        await Assert.ThrowsAsync<DirectoryNotFoundException>(
            () => output.CopyIntoOutputAsync(Path.Combine(directory.Path, "no-such-directory")));
    }

    [Fact]
    public async Task CleanUpRemovesAnOrphanedSiteAndTheEmptyDirectory()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/stale/index.html", "stale");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CleanUpAsync();

        Assert.False(Directory.Exists(Path.Combine(outputPath, "stale")));
    }

    [Fact]
    public async Task CleanUpKeepsWhatTheRunWrote()
    {
        using var directory = new TempDirectory();
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(Path.Combine(outputPath, "posts"));

        var output = new OutputWriter(outputPath, onlyWriteWhatChanged: true);
        await output.WriteAsync(Path.Combine(outputPath, "posts"), "index.html", "written this run");

        await output.CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "posts", "index.html")));
    }

    [Fact]
    public async Task CleanUpRemovesAnyOrphanedFileNotJustAnIndex()
    {
        // The bookkeeping used to be per directory and only index.html was removed, because
        // any other file might have been a resource. A resource deleted from the Resources
        // directory therefore stayed in the output forever.
        using var directory = new TempDirectory();
        directory.WriteFile("Output/old-photo.png", "image");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CleanUpAsync();

        Assert.False(File.Exists(Path.Combine(outputPath, "old-photo.png")));
    }

    [Fact]
    public async Task CleanUpKeepsACopiedResource()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Resources/favicon.png", "icon");
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);

        var output = new OutputWriter(outputPath, onlyWriteWhatChanged: true);
        await output.CopyIntoOutputAsync(Path.Combine(directory.Path, "Resources"));
        await output.CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "favicon.png")));
    }

    [Fact]
    public async Task CleanUpKeepsThePreservedNamesAndDoesNotEnterThem()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/.git/objects/ab/cdef", "an object");
        directory.WriteFile("Output/CNAME", "example.com");
        directory.WriteFile("Output/.nojekyll", "");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, ".git", "objects", "ab", "cdef")));
        Assert.True(File.Exists(Path.Combine(outputPath, "CNAME")));
        Assert.True(File.Exists(Path.Combine(outputPath, ".nojekyll")));
    }

    [Fact]
    public async Task CleanUpKeepsAnAdditionallyPreservedNameAnywhereInTheOutput()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/robots.txt", "User-agent: *");
        directory.WriteFile("Output/posts/.DS_Store", "noise");
        directory.WriteFile("Output/posts/orphan.html", "orphan");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(
                outputPath,
                onlyWriteWhatChanged: true,
                alsoPreserve: ["robots.txt", ".DS_Store"])
            .CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "robots.txt")));
        Assert.True(File.Exists(Path.Combine(outputPath, "posts", ".DS_Store")));
        Assert.False(File.Exists(Path.Combine(outputPath, "posts", "orphan.html")));
    }

    [Fact]
    public async Task APreservedNameIsMatchedWithoutRegardToCase()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/cname", "example.com");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "cname")));
    }

    [Fact]
    public async Task CleanUpKeepsADirectoryThatOnlyHoldsAPreservedFile()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/.well-known/security.txt", "Contact: mailto:me@example.com");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true, alsoPreserve: ["security.txt"])
            .CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, ".well-known", "security.txt")));
    }

    [Fact]
    public void RejectsAnUnusableOutputPath()
    {
        Assert.Throws<ArgumentNullException>(() => new OutputWriter(null!, false));
        Assert.Throws<ArgumentException>(() => new OutputWriter("   ", false));
    }
}
