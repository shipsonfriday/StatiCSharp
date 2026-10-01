using System;
using System.IO;
using StatiCSharp.Exceptions;
using Xunit;

namespace StatiCSharp.Tests;

public class EnvironmentCheckTests
{
    /// <summary>
    /// The three directories a run needs, below the given directory.
    /// </summary>
    private static (string Content, string Resources, string Output) DirectoriesIn(TempDirectory directory) => (
        Path.Combine(directory.Path, "Content"),
        Path.Combine(directory.Path, "Resources"),
        Path.Combine(directory.Path, "Output"));

    [Fact]
    public void BothExceptionTypesArePublic()
    {
        // They are thrown out of MakeAsync, so a caller has to be able to name them in a
        // catch clause. Both used to be internal.
        Assert.True(typeof(CannotCreateDirectoryException).IsPublic);
        Assert.True(typeof(DirectoryNotWriteableException).IsPublic);
    }

    [Fact]
    public void TheMissingDirectoriesAreCreated()
    {
        using var directory = new TempDirectory();

        var (content, resources, output) = DirectoriesIn(directory);
        EnvironmentCheck.Verify(content, resources, output);

        Assert.True(Directory.Exists(content));
        Assert.True(Directory.Exists(resources));
        Assert.True(Directory.Exists(output));
    }

    [Fact]
    public void ADirectoryThatCannotBeCreatedRaisesCannotCreateDirectory()
    {
        using var directory = new TempDirectory();

        // A regular file cannot hold a subdirectory, so creating one below it fails on
        // every platform.
        string blocker = directory.WriteFile("blocker", "not a directory");

        var (content, resources, _) = DirectoriesIn(directory);

        var thrown = Assert.Throws<CannotCreateDirectoryException>(
            () => EnvironmentCheck.Verify(content, resources, Path.Combine(blocker, "Output")));

        Assert.NotNull(thrown.InnerException);
        Assert.Contains("Output", thrown.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void AnUnwritableDirectoryRaisesDirectoryNotWriteable()
    {
        // Permissions are set through the unix file mode, which Windows does not honour.
        // Written as a guarded early return rather than Assert.SkipWhen, because that is
        // the shape the platform-compatibility analyzer recognises.
        if (OperatingSystem.IsWindows())
        {
            Assert.Skip("Needs POSIX permissions.");
            return;
        }

        using var directory = new TempDirectory();
        string output = Path.Combine(directory.Path, "Output");
        Directory.CreateDirectory(output);
        File.SetUnixFileMode(output, UnixFileMode.UserRead | UnixFileMode.UserExecute);

        // Root ignores the permission bits, so there would be nothing to observe.
        Assert.SkipWhen(CanStillWriteTo(output), "Running with privileges that ignore the permission bits.");

        try
        {
            var (content, resources, _) = DirectoriesIn(directory);

            var thrown = Assert.Throws<DirectoryNotWriteableException>(
                () => EnvironmentCheck.Verify(content, resources, output));

            Assert.NotNull(thrown.InnerException);
            Assert.Contains("Output", thrown.Message, StringComparison.Ordinal);
        }
        finally
        {
            // Give it back, or the temp directory cannot be removed.
            File.SetUnixFileMode(
                output,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }
    }

    [Fact]
    public void AMissingThemeResourcesDirectoryRaisesDirectoryNotFound()
    {
        using var directory = new TempDirectory();

        var (content, resources, output) = DirectoriesIn(directory);

        Assert.Throws<DirectoryNotFoundException>(
            () => EnvironmentCheck.Verify(content, resources, output, Path.Combine(directory.Path, "no-such-theme")));
    }

    [Fact]
    public void RejectsAnUnusableDirectory()
    {
        Assert.Throws<ArgumentNullException>(() => EnvironmentCheck.Verify(null!, "/resources", "/output"));
        Assert.Throws<ArgumentNullException>(() => EnvironmentCheck.Verify("/content", null!, "/output"));
        Assert.Throws<ArgumentNullException>(() => EnvironmentCheck.Verify("/content", "/resources", null!));
        Assert.Throws<ArgumentException>(() => EnvironmentCheck.Verify("   ", "/resources", "/output"));
    }

    private static bool CanStillWriteTo(string path)
    {
        try
        {
            using FileStream probe = File.Create(
                Path.Combine(path, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
