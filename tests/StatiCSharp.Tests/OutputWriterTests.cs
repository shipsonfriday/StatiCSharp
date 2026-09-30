using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace StatiCSharp.Tests;

public class OutputWriterTests
{
    [Fact]
    public void APathCanOnlyBeClaimedOnce()
    {
        using var directory = new TempDirectory();
        var output = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        Assert.True(output.ClaimPath("/somewhere"));
        Assert.False(output.ClaimPath("/somewhere"));
        Assert.True(output.WasWrittenTo("/somewhere"));
        Assert.False(output.WasWrittenTo("/elsewhere"));
    }

    [Fact]
    public void TwoWritersDoNotShareTheirClaims()
    {
        // This is the point of creating one per run. As a field on the manager the claims
        // survived, so a second MakeAsync considered every path from the first run taken and
        // warned about all of them.
        using var directory = new TempDirectory();

        var first = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);
        var second = new OutputWriter(directory.Path, onlyWriteWhatChanged: false);

        Assert.True(first.ClaimPath("/somewhere"));
        Assert.True(second.ClaimPath("/somewhere"));
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
    public async Task CleanUpRemovesAnUnclaimedIndexAndTheEmptyDirectory()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/stale/index.html", "stale");

        string outputPath = Path.Combine(directory.Path, "Output");
        var output = new OutputWriter(outputPath, onlyWriteWhatChanged: true);

        await output.CleanUpAsync();

        Assert.False(Directory.Exists(Path.Combine(outputPath, "stale")));
    }

    [Fact]
    public async Task CleanUpKeepsWhatWasClaimed()
    {
        using var directory = new TempDirectory();
        directory.WriteFile("Output/posts/index.html", "written this run");

        string outputPath = Path.Combine(directory.Path, "Output");
        var output = new OutputWriter(outputPath, onlyWriteWhatChanged: true);
        output.ClaimPath(Path.Combine(outputPath, "posts"));

        await output.CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "posts", "index.html")));
    }

    [Fact]
    public async Task CleanUpLeavesFilesThatAreNotAnIndex()
    {
        // Other files could be resources, so cleanup only removes index.html. That is the
        // reason a deleted resource lingers in the output, which is a separate matter.
        using var directory = new TempDirectory();
        directory.WriteFile("Output/stale/photo.png", "image");

        string outputPath = Path.Combine(directory.Path, "Output");

        await new OutputWriter(outputPath, onlyWriteWhatChanged: true).CleanUpAsync();

        Assert.True(File.Exists(Path.Combine(outputPath, "stale", "photo.png")));
    }

    [Fact]
    public void RejectsAnUnusableOutputPath()
    {
        Assert.Throws<ArgumentNullException>(() => new OutputWriter(null!, false));
        Assert.Throws<ArgumentException>(() => new OutputWriter("   ", false));
    }
}
