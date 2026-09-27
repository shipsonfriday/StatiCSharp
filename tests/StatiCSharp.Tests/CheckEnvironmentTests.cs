using System;
using System.IO;
using StatiCSharp.Exceptions;
using Xunit;

namespace StatiCSharp.Tests;

public class CheckEnvironmentTests
{
    private static Website AWebsite() =>
        Website.Create(url: "https://example.com", name: "My Website");

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

        WebsiteManager manager = WebsiteManager.For(AWebsite(), directory.Path);
        manager.CheckEnvironment();

        Assert.True(Directory.Exists(manager.Content));
        Assert.True(Directory.Exists(manager.Resources));
        Assert.True(Directory.Exists(manager.Output));
    }

    [Fact]
    public void ADirectoryThatCannotBeCreatedRaisesCannotCreateDirectory()
    {
        using var directory = new TempDirectory();

        // A regular file cannot hold a subdirectory, so creating one below it fails on
        // every platform.
        string blocker = directory.WriteFile("blocker", "not a directory");

        WebsiteManager manager = WebsiteManager.For(AWebsite(), directory.Path)
            .WithOutputDirectory(Path.Combine(blocker, "Output"));

        var thrown = Assert.Throws<CannotCreateDirectoryException>(() => manager.CheckEnvironment());

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
            WebsiteManager manager = WebsiteManager.For(AWebsite(), directory.Path);

            var thrown = Assert.Throws<DirectoryNotWriteableException>(() => manager.CheckEnvironment());

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

        WebsiteManager manager = WebsiteManager.For(AWebsite(), directory.Path);

        Assert.Throws<DirectoryNotFoundException>(
            () => manager.CheckEnvironment(Path.Combine(directory.Path, "no-such-theme")));
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
