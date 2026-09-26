using StatiCSharp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

/// <summary>
/// Manager that handles the website generating process with the given website and theme.
/// <para>
/// Start with <see cref="For(IWebsite, string)"/> and add the optional parts fluently:
/// <code>
/// await WebsiteManager.For(website, source: "/path/to/your/project")
///     .WithTheme(myTheme)
///     .WithGitMode()
///     .MakeAsync();
/// </code>
/// </para>
/// </summary>
public partial class WebsiteManager : IWebsiteManager
{
    private readonly HtmlBuilder _htmlBuilder = new(useDefaultMarkdownParser: true);

    /// <inheritdoc/>
    public bool UseDefaultMarkdownParser => _htmlBuilder.UseDefaultMarkdownParser;

    /// <inheritdoc/>
    public string SourceDir { get; }

    /// <inheritdoc/>
    public string Content { get; private set; }

    /// <inheritdoc/>
    public string Resources { get; private set; }

    /// <inheritdoc/>
    public string Output { get; private set; }

    /// <inheritdoc/>
    public bool GitMode { get; private set; }

    /// <summary>
    /// List of all used paths while creating the sites.<br/>
    /// Used to find identical paths from meta data and to find files that have no markdown equivalent (got deleted) in GitMode.
    ///  </summary>
    private List<string> PathDirectory { get; set; }

    /// <inheritdoc/>
    public IWebsite Website { get; }

    /// <inheritdoc/>
    public IHtmlFactory HtmlFactory { get; private set; }

    private WebsiteManager(IWebsite website, string source)
    {
        Website = website;
        HtmlFactory = new DefaultHtmlFactory(website);

        SourceDir = source;
        Content = Path.Combine(source, "Content");
        Resources = Path.Combine(source, "Resources");
        Output = Path.Combine(source, "Output");

        PathDirectory = [];
    }

    /// <summary>
    /// Starts a new manager for the given website. These two values are required,
    /// everything else is optional and added with the <c>With…</c> methods.
    /// <para>
    /// Unless overridden, <c>Content</c>, <c>Resources</c> and <c>Output</c> are
    /// subdirectories of <paramref name="source"/>, and the built-in default theme is used.
    /// </para>
    /// </summary>
    /// <param name="website">The website that contains the content.</param>
    /// <param name="source">The absolute path to the directory that contains the folders `Content`, `Output` and `Resources`.</param>
    /// <returns>The new manager, ready for further configuration.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="website"/> or <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="source"/> is empty or only whitespace.</exception>
    public static WebsiteManager For(IWebsite website, string source)
    {
        ArgumentNullException.ThrowIfNull(website);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        return new WebsiteManager(website, source.Trim());
    }

    /// <summary>
    /// Sets the theme used to render the website. Without this, the built-in default theme is used.
    /// </summary>
    /// <param name="htmlFactory">The theme for the website.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="htmlFactory"/> is null.</exception>
    public WebsiteManager WithTheme(IHtmlFactory htmlFactory)
    {
        ArgumentNullException.ThrowIfNull(htmlFactory);

        HtmlFactory = htmlFactory;
        return this;
    }

    /// <summary>
    /// Turns on GitMode: output files are only rewritten when their content changed, and
    /// files without a corresponding markdown file are deleted. Off by default.
    /// </summary>
    /// <param name="enabled">Whether GitMode is on.</param>
    /// <returns>this - the manager itself.</returns>
    public WebsiteManager WithGitMode(bool enabled = true)
    {
        GitMode = enabled;
        return this;
    }

    /// <summary>
    /// Overrides where the markdown files are read from.
    /// Defaults to the `Content` folder inside the source directory.
    /// </summary>
    /// <param name="path">The path to the content directory.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or only whitespace.</exception>
    public WebsiteManager WithContentDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Content = path.Trim();
        return this;
    }

    /// <summary>
    /// Overrides where the static files are read from.
    /// Defaults to the `Resources` folder inside the source directory.
    /// </summary>
    /// <param name="path">The path to the resources directory.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or only whitespace.</exception>
    public WebsiteManager WithResourcesDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Resources = path.Trim();
        return this;
    }

    /// <summary>
    /// Overrides where the generated website is written to.
    /// Defaults to the `Output` folder inside the source directory.
    /// </summary>
    /// <param name="path">The path to the output directory.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty or only whitespace.</exception>
    public WebsiteManager WithOutputDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Output = path.Trim();
        return this;
    }

    /// <summary>
    /// Adds a parser to the end of the HTML building pipeline.
    /// Parsers run in the order they were added.
    /// </summary>
    /// <param name="parser">The parser to add.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="parser"/> is null.</exception>
    public WebsiteManager AddParser(IPipelineParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        _htmlBuilder.AddToPipeline(parser);
        return this;
    }

    /// <summary>
    /// Stops the integrated Markdig parser from running at the end of the pipeline.
    /// It is on by default.
    /// </summary>
    /// <returns>this - the manager itself.</returns>
    public WebsiteManager WithoutDefaultMarkdownParser()
    {
        _htmlBuilder.UseDefaultMarkdownParser = false;
        return this;
    }

    /// <inheritdoc/>
    public async Task MakeAsync()
    {
        WriteLine("Website generating process startet...");

        WriteLine("Checking environment...");
        var checkEnvTask = Task.Run(() => CheckEnvironment(HtmlFactory.ResourcesPath)).ConfigureAwait(false);
        await checkEnvTask;

        WriteLine("Collecting markdown data...");
        await GenerateSitesFromMarkdownAsync();

        if (!GitMode)
        {
            WriteLine("Deleting old output files...");
            var deleteAllTask = Task.Run(() => DeleteAll(Output));
            await deleteAllTask;
        }

        WriteLine("Copying theme resources...");
        await CopyAllAsync(HtmlFactory.ResourcesPath, Output);

        WriteLine("Writing index...");
        await MakeIndexAsync();

        WriteLine("Writing pages...");
        await MakePagesAsync();

        WriteLine("Writing sections...");
        await MakeSectionsAsync();

        WriteLine("Writing items...");
        await MakeItemsAsync();

        WriteLine("Writing tag lists...");
        await MakeTagListsAsync();

        WriteLine("Cleaning up...");
        await CleanUpAsync();

        WriteLine("Copying user resources...");
        await CopyAllAsync(Resources, Output);

        WriteLine($"Success! Your website has been generated at {Output}");
    }
}
