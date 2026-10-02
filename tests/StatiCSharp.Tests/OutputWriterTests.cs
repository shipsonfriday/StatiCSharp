using Xunit;

namespace StatiCSharp.Tests;

public class OutputWriterTests
{
    [Fact]
    public async Task AFileCanOnlyBeWrittenOncePerRun()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        Assert.True(await output.WriteAsync([], "index.html", "first"));
        Assert.False(await output.WriteAsync([], "index.html", "second"));

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

        Assert.True(await first.WriteAsync([], "index.html", "x"));
        Assert.True(await second.WriteAsync([], "index.html", "x"));
    }

    [Fact]
    public async Task WriteAsyncWritesTheFile()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        await output.WriteAsync([], "index.html", "<p>x</p>");

        Assert.Equal("<p>x</p>", await File.ReadAllTextAsync(
            Path.Combine(directory.Path, "index.html"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task WithoutIncrementalWritingAnUnchangedFileIsStillTouched()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);
        string file = Path.Combine(directory.Path, "index.html");

        await output.WriteAsync([], "index.html", "same");
        DateTime first = File.GetLastWriteTimeUtc(file);

        File.SetLastWriteTimeUtc(file, first.AddDays(-1));
        await output.WriteAsync([], "index.html", "same");

        Assert.NotEqual(first.AddDays(-1), File.GetLastWriteTimeUtc(file));
    }

    [Fact]
    public async Task WithIncrementalWritingAnUnchangedFileLeavesItAlone()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);
        string file = Path.Combine(directory.Path, "index.html");

        await output.WriteAsync([], "index.html", "same");

        DateTime marker = File.GetLastWriteTimeUtc(file).AddDays(-1);
        File.SetLastWriteTimeUtc(file, marker);
        await output.WriteAsync([], "index.html", "same");

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

        await output.WriteAsync([], "index.html", "same");
        await output.CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "index.html")));
    }

    [Fact]
    public async Task WithIncrementalAChangedFileIsWritten()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: true);

        await output.WriteAsync([], "index.html", "before");
        await output.WriteAsync([], "index.html", "after");

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
    public async Task AnUnchangedResourceIsNotCopiedAgain()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Resources/styles.css", "body { color: red; }");
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);
        string source = Path.Combine(directory.Path, "Resources");

        var output = new OutputWriter(outputPath, onlyWriteWhatChanged: true);
        await output.CopyIntoOutputAsync(source);

        string copy = Path.Combine(outputPath, "styles.css");
        DateTime marker = File.GetLastWriteTimeUtc(copy).AddDays(-1);
        File.SetLastWriteTimeUtc(copy, marker);

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        Assert.Equal(marker, File.GetLastWriteTimeUtc(copy));
    }

    [Fact]
    public async Task AChangedResourceIsCopiedAgain()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Resources/styles.css", "body { color: red; }");
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);
        string source = Path.Combine(directory.Path, "Resources");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        directory.WriteFile("Resources/styles.css", "body { color: blue; }");
        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        Assert.Equal("body { color: blue; }", await File.ReadAllTextAsync(
            Path.Combine(outputPath, "styles.css"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task AResourceChangedInOneByteOfTheSameLengthIsCopiedAgain()
    {
        // The length settles almost every case, which is why the byte comparison behind it
        // is the part that needs a test. These two files are not text at all.
        using var directory = new TempDirectory();
        byte[] before = [0x89, 0x50, 0x4E, 0x47, 0xFF, 0x00, 0x80];
        byte[] after = [0x89, 0x50, 0x4E, 0x47, 0xFE, 0x00, 0x80];

        directory.WriteBytes("Resources/logo.png", before);
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);
        string source = Path.Combine(directory.Path, "Resources");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        directory.WriteBytes("Resources/logo.png", after);
        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        Assert.Equal(after, await File.ReadAllBytesAsync(
            Path.Combine(outputPath, "logo.png"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ADifferenceBeyondTheFirstBufferIsFound()
    {
        // The comparison reads in 64 KiB steps. A file that differs only after the first
        // step would look unchanged to a comparison that stops there.
        using var directory = new TempDirectory();
        byte[] before = new byte[200 * 1024];
        byte[] after = new byte[200 * 1024];
        after[^1] = 0x01;

        directory.WriteBytes("Resources/big.bin", before);
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);
        string source = Path.Combine(directory.Path, "Resources");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        directory.WriteBytes("Resources/big.bin", after);
        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        Assert.Equal(after, await File.ReadAllBytesAsync(
            Path.Combine(outputPath, "big.bin"), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ALargeUnchangedResourceIsStillLeftAlone()
    {
        // The other side of the loop: reading to the end must report equality, not fall out
        // of the loop as a difference.
        using var directory = new TempDirectory();
        directory.WriteBytes("Resources/big.bin", new byte[200 * 1024]);
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);
        string source = Path.Combine(directory.Path, "Resources");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        string copy = Path.Combine(outputPath, "big.bin");
        DateTime marker = File.GetLastWriteTimeUtc(copy).AddDays(-1);
        File.SetLastWriteTimeUtc(copy, marker);

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CopyIntoOutputAsync(source);

        Assert.Equal(marker, File.GetLastWriteTimeUtc(copy));
    }

    [Fact]
    public async Task WithoutIncrementalEveryResourceIsCopiedAgain()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Resources/styles.css", "body { color: red; }");
        string outputPath = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(outputPath);
        string source = Path.Combine(directory.Path, "Resources");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: false).CopyIntoOutputAsync(source);

        string copy = Path.Combine(outputPath, "styles.css");
        DateTime marker = File.GetLastWriteTimeUtc(copy).AddDays(-1);
        File.SetLastWriteTimeUtc(copy, marker);

        await new OutputWriter(outputPath, onlyWriteWhatChanged: false).CopyIntoOutputAsync(source);

        Assert.NotEqual(marker, File.GetLastWriteTimeUtc(copy));
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
        await output.WriteAsync(["posts"], "index.html", "written this run");

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
