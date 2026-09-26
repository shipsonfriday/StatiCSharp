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
    /// <param name="templateResources"></param>
    /// <returns></returns>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="CannotCreateDirectoryException"></exception>
    private void CheckEnvironment(string? templateResources = null)
    {
        string[] assumedDirectories = new string[] { Output, Content, Resources };

        foreach(string assumedDirectory in assumedDirectories)
        {
            CheckIfDirectoryExists(assumedDirectory);
            CheckIfDirectoryIsWritable(assumedDirectory);
        }

        if (templateResources is not null)
        {
            if (!Directory.Exists(templateResources!))
            {
                throw new DirectoryNotFoundException($"Your template resources directory does not exist. Do you have read and write access to {templateResources} ?");
            }
        }


        void CheckIfDirectoryExists(string assumedDirectory)
        {
            if (!Directory.Exists(assumedDirectory))
            {
                try
                {
                    Directory.CreateDirectory(assumedDirectory);
                }
                catch (Exception ex)
                {
                    throw new CannotCreateDirectoryException(message: $"Your {nameof(assumedDirectory).ToLower()} directory does not exist. Trying to create it failed. Do you have read and write access to {assumedDirectory} ?", ex);
                }
            }
        }


        void CheckIfDirectoryIsWritable(string path)
        {
            using (FileStream fs = File.Create(Path.Combine(path, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
            {
            }
        }
    }
}
