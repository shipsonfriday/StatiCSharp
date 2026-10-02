using StatiCSharp.Exceptions;

namespace StatiCSharp;

/// <summary>
/// Makes sure a run can do its work before it starts: the directories it needs exist and can
/// be written to.
/// <para>
/// Holds nothing, so it is a static class rather than a third object created per run. It does
/// more than its name says - a directory that is missing is created, and only a directory
/// that cannot be created is an error.
/// </para>
/// </summary>
internal static class EnvironmentCheck
{
    /// <summary>
    /// Checks the directories a run reads from and writes to, creating the ones that are
    /// missing.
    /// </summary>
    /// <param name="content">The directory holding the markdown files.</param>
    /// <param name="resources">The directory holding the static files.</param>
    /// <param name="output">The directory the website is written to.</param>
    /// <param name="themeResources">The theme's resources directory, or null to skip that check.</param>
    /// <exception cref="ArgumentNullException">A directory is null.</exception>
    /// <exception cref="ArgumentException">A directory is empty or only whitespace.</exception>
    /// <exception cref="InvalidOperationException">A directory that is copied into the output contains the output.</exception>
    /// <exception cref="DirectoryNotFoundException">The theme's resources directory does not exist.</exception>
    /// <exception cref="CannotCreateDirectoryException">A needed directory is missing and cannot be created.</exception>
    /// <exception cref="DirectoryNotWriteableException">A needed directory exists but cannot be written to.</exception>
    internal static void Verify(string content, string resources, string output, string? themeResources = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(resources);
        ArgumentException.ThrowIfNullOrWhiteSpace(output);

        // Paths first, before anything is created: a configuration that cannot work should be
        // refused without leaving directories behind.
        RejectIfItHoldsTheOutput(resources, output, "The resources directory");

        if (themeResources is not null)
        {
            RejectIfItHoldsTheOutput(themeResources, output, "The theme's resources directory");
        }

        // The output comes first: a run that cannot write anywhere is not worth starting.
        string[] assumedDirectories = [output, content, resources];

        foreach (string assumedDirectory in assumedDirectories)
        {
            CheckIfDirectoryExists(assumedDirectory);
            CheckIfDirectoryIsWritable(assumedDirectory);
        }

        if (themeResources is not null)
        {
            if (!Directory.Exists(themeResources))
            {
                throw new DirectoryNotFoundException($"Your template resources directory does not exist. Do you have read and write access to {themeResources} ?");
            }
        }


        static void CheckIfDirectoryExists(string assumedDirectory)
        {
            if (!Directory.Exists(assumedDirectory))
            {
                try
                {
                    Directory.CreateDirectory(assumedDirectory);
                }
                catch (Exception ex)
                {
                    throw new CannotCreateDirectoryException($"The directory {assumedDirectory} does not exist and StatiC# could not create it. Do you have read and write access to it?", ex);
                }
            }
        }


        static void RejectIfItHoldsTheOutput(string directory, string output, string what)
        {
            if (!Holds(directory, output))
            {
                return;
            }

            throw new InvalidOperationException(
                $"{what} ({directory}) is the output directory ({output}) or contains it. " +
                "Everything below it is copied into the output, so the output would be copied " +
                "into itself, one level deeper on every run. Point it at a directory beside " +
                "the output instead.");
        }

        // Whether the second path is the first one or lies below it. Case is ignored: a run
        // refused by mistake says so, while one that slips through quietly nests the output
        // inside itself.
        static bool Holds(string directory, string candidate)
        {
            string outer = Path.TrimEndingDirectorySeparator(Path.GetFullPath(directory));
            string inner = Path.TrimEndingDirectorySeparator(Path.GetFullPath(candidate));

            return inner.Equals(outer, StringComparison.OrdinalIgnoreCase)
                || inner.StartsWith(outer + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        static void CheckIfDirectoryIsWritable(string path)
        {
            try
            {
                // Writing a file that deletes itself on close is the only reliable check:
                // permissions alone do not tell whether the volume is read-only or full.
                using FileStream probe = File.Create(Path.Combine(path, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
            }
            catch (Exception ex)
            {
                throw new DirectoryNotWriteableException($"StatiC# cannot write to the directory {path}. Do you have write access to it?", ex);
            }
        }
    }
}
