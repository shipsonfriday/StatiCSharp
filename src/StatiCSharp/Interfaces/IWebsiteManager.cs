using System.Collections.Generic;
using System.Threading.Tasks;

namespace StatiCSharp.Interfaces;

/// <summary>
/// The read and run contract of a website manager.
/// <para>
/// Configuration is not part of this interface: the fluent <c>For</c> and <c>With…</c>
/// methods live on <see cref="WebsiteManager"/> and return the concrete type, so a
/// chain does not lose access to them halfway through.
/// </para>
/// </summary>
public interface IWebsiteManager
{
    /// <summary>
    /// The absolute path to the directory that contains the directories `Content`, `Output` and `Resources`.
    /// </summary>
    string SourceDir { get; }

    /// <summary>
    /// The abolute path to the content (markdown-files) for the website.
    /// </summary>
    string Content { get; }

    /// <summary>
    /// The path to the resources (static files) of the website.
    /// </summary>
    string Resources { get; }

    /// <summary>
    /// The absolute path to the output directory.
    /// </summary>
    string Output { get; }

    /// <summary>
    /// If true, the generator updates the output directory: it writes a file only when its
    /// content changed and removes what no longer belongs to the website. If false, the
    /// output directory is emptied first and every file is written anew. Default is true.
    /// </summary>
    bool IncrementalOutput { get; }

    /// <summary>
    /// Names of files and directories in the output that are kept although the generator did
    /// not produce them. Always contains `.git`, `.nojekyll` and `CNAME`.
    /// </summary>
    IReadOnlyCollection<string> PreservedOutput { get; }

    /// <summary>
    /// The website of the current context.
    /// </summary>
    IWebsite Website { get; }

    /// <summary>
    /// The theme to build the website.
    /// </summary>
    IHtmlFactory HtmlFactory { get; }

    /// <summary>
    /// If the integrated default markdown parser runs at the end of the parser pipeline.
    /// Default is true.
    /// </summary>
    bool UseDefaultMarkdownParser { get; }

    /// <summary>
    /// Starts the generating process of the website with the given theme.<br/>
    /// Files are saved in the `Output` directory.<br/><br/>
    /// If the `Output` directory is not explicity set, it is inside the `Source` directory.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous generating operation.</returns>
    Task MakeAsync();
}
