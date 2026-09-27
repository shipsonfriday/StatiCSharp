using StatiCSharp.Interfaces;
using StatiCSharp.Exceptions;
using System.IO;
using System;

namespace StatiCSharp;

public partial class WebsiteManager : IWebsiteManager
{
    /// <summary>
    /// Checks if all nessessary directories exist and if it can read and write in this folders.<br/>
    /// If a directory does not exist, it tries to create it.
    /// </summary>
    /// <param name="templateResources">The theme's resources directory, or null to skip that check.</param>
    /// <exception cref="DirectoryNotFoundException">The theme's resources directory does not exist.</exception>
    /// <exception cref="CannotCreateDirectoryException">A needed directory is missing and cannot be created.</exception>
    /// <exception cref="DirectoryNotWriteableException">A needed directory exists but cannot be written to.</exception>
    internal void CheckEnvironment(string? templateResources = null)
    {
        string[] assumedDirectories = [Output, Content, Resources];

        foreach (string assumedDirectory in assumedDirectories)
        {
            CheckIfDirectoryExists(assumedDirectory);
            CheckIfDirectoryIsWritable(assumedDirectory);
        }

        if (templateResources is not null)
        {
            if (!Directory.Exists(templateResources))
            {
                throw new DirectoryNotFoundException($"Your template resources directory does not exist. Do you have read and write access to {templateResources} ?");
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
