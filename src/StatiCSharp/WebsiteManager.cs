using StatiCSharp.Exceptions;
using StatiCSharp.Interfaces;

namespace StatiCSharp;

/// <summary>
/// Manager that handles the website generating process with the given website and theme.
/// <para>
/// Start with <see cref="For(IWebsite, string)"/> and add the optional parts fluently:
/// <code>
/// await WebsiteManager.For(website, source: "/path/to/your/project")
///     .WithTheme(myTheme)
///     .MakeAsync();
/// </code>
/// </para>
/// </summary>
public sealed class WebsiteManager
{
    private readonly HtmlBuilder _htmlBuilder = new(useDefaultMarkdownParser: true);
    private Action<string> _log = Console.WriteLine;
    private readonly List<string> _preservedOutput = [.. OutputWriter.AlwaysPreserved];

    /// <summary>
    /// Whether the integrated Markdig parser runs at the end of the parser pipeline.
    /// Default is true, see <see cref="NoDefaultMarkdownParser"/>.
    /// </summary>
    public bool UseDefaultMarkdownParser => _htmlBuilder.UseDefaultMarkdownParser;

    /// <summary>
    /// The absolute path to the directory that contains the directories `Content`, `Output`
    /// and `Resources`.
    /// </summary>
    public string SourceDir { get; }

    /// <summary>
    /// The absolute path to the content (markdown files) of the website.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// The absolute path to the resources (static files) of the website.
    /// </summary>
    public string Resources { get; private set; }

    /// <summary>
    /// The absolute path to the output directory.
    /// </summary>
    public string Output { get; private set; }

    /// <summary>
    /// If true, the generator updates the output directory: it writes a file only when its
    /// content changed and removes what no longer belongs to the website. If false, the
    /// output directory is emptied first and every file is written anew. Default is true,
    /// see <see cref="NoIncrementalOutput"/>.
    /// </summary>
    public bool IncrementalOutput { get; private set; } = true;

    /// <summary>
    /// Names of files and directories in the output that are kept although the generator did
    /// not produce them. Always contains `.git`, `.nojekyll` and `CNAME`, see
    /// <see cref="WithPreservedOutput"/>.
    /// </summary>
    public IReadOnlyCollection<string> PreservedOutput => _preservedOutput;

    /// <summary>
    /// The website this manager generates.
    /// </summary>
    public IWebsite Website { get; }

    /// <summary>
    /// The theme the website is rendered with. Defaults to the built-in theme, see
    /// <see cref="WithTheme"/>.
    /// </summary>
    public IHtmlFactory HtmlFactory { get; private set; }

    private WebsiteManager(IWebsite website, string source)
    {
        Website = website;
        HtmlFactory = new DefaultHtmlFactory();

        SourceDir = source;
        Content = Path.Combine(source, "Content");
        Resources = Path.Combine(source, "Resources");
        Output = Path.Combine(source, "Output");
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
    /// Writes the whole website from scratch instead of updating what is already there:
    /// the output directory is emptied first and every file is written anew.
    /// <para>
    /// Incremental output is the default and is what you want in almost every case. Reach
    /// for this when you want a guaranteed clean result, for instance after changing a
    /// theme in a way the generator cannot see.
    /// </para>
    /// </summary>
    /// <returns>this - the manager itself.</returns>
    public WebsiteManager NoIncrementalOutput()
    {
        IncrementalOutput = false;
        return this;
    }

    /// <summary>
    /// Sends the generator's messages somewhere other than the console.
    /// <para>
    /// Progress lines and warnings both go here; a warning starts with <c>"WARNING: "</c>. The
    /// warnings are the interesting part - colliding tag urls, a date that cannot be read, two
    /// sites claiming one url - and a build script that wants to fail on them needs to see
    /// them rather than watch them scroll past.
    /// </para>
    /// <para>
    /// May be called from several threads at once, because the sites are written in parallel.
    /// <see cref="Console.WriteLine(string)"/>, the default, handles that; a collection of your
    /// own has to.
    /// </para>
    /// </summary>
    /// <param name="log">Where a message goes. Pass <c>_ => { }</c> to stay quiet.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="log"/> is null.</exception>
    public WebsiteManager WithLog(Action<string> log)
    {
        ArgumentNullException.ThrowIfNull(log);

        _log = log;
        return this;
    }

    /// <summary>
    /// Keeps files and directories of these names in the output, although the generator did
    /// not produce them. Everything else in the output directory is removed, so that a
    /// deleted markdown file or a renamed resource disappears from the website.
    /// <para>
    /// A name matches a file or a directory anywhere in the output, and a preserved directory
    /// is kept whole. Case does not matter.
    /// </para>
    /// <para>
    /// Adds to the list instead of replacing it: <c>.git</c>, <c>.nojekyll</c> and
    /// <c>CNAME</c> are always kept, and losing one of those to a careless call is worse than
    /// having no way to drop them.
    /// </para>
    /// </summary>
    /// <param name="names">The names to keep. Entries are trimmed; empty ones are ignored.</param>
    /// <returns>this - the manager itself.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="names"/> is null.</exception>
    public WebsiteManager WithPreservedOutput(params string[] names)
    {
        ArgumentNullException.ThrowIfNull(names);

        _preservedOutput.AddRange(names
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim()));

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
    public WebsiteManager NoDefaultMarkdownParser()
    {
        _htmlBuilder.UseDefaultMarkdownParser = false;
        return this;
    }

    /// <summary>
    /// Generates the website and writes it into the output directory.
    /// <para>
    /// Reads the markdown files, renders every site with the theme, copies the resources and
    /// removes what no longer belongs to the website.
    /// </para>
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous generating operation.</returns>
    /// <exception cref="CannotCreateDirectoryException">A needed directory is missing and cannot be created.</exception>
    /// <exception cref="DirectoryNotWriteableException">A needed directory exists but cannot be written to.</exception>
    /// <exception cref="DirectoryNotFoundException">The theme's resources directory does not exist.</exception>
    /// <exception cref="InvalidOperationException">A directory that is copied into the output contains the output.</exception>
    public async Task MakeAsync()
    {
        _log("Website generating process startet...");

        _log("Checking environment...");
        await Task.Run(() => EnvironmentCheck.Verify(Content, Resources, Output, HtmlFactory.ResourcesPath));

        _log("Collecting markdown data...");
        RenderContext context = new ContentReader(Content, _htmlBuilder, _log).Read(Website);

        // One writer per run: the paths it records are only meaningful for this run.
        OutputWriter output = new(Output, onlyWriteWhatChanged: IncrementalOutput, alsoPreserve: _preservedOutput);

        if (!IncrementalOutput)
        {
            _log("Deleting old output files...");
            await Task.Run(output.Clear);
        }

        _log("Copying theme resources...");
        await output.CopyIntoOutputAsync(HtmlFactory.ResourcesPath);

        WebsiteRenderer renderer = new(context, HtmlFactory, _htmlBuilder, output, _log);

        _log("Writing index...");
        await renderer.RenderIndexAsync();

        _log("Writing pages...");
        await renderer.RenderPagesAsync();

        _log("Writing sections...");
        await renderer.RenderSectionsAsync();

        _log("Writing items...");
        await renderer.RenderItemsAsync();

        _log("Writing tag lists...");
        await renderer.RenderTagListsAsync();

        _log("Copying user resources...");
        await output.CopyIntoOutputAsync(Resources);

        // After everything else, because it lists what was written rather than what was meant.
        _log("Writing the sitemap...");
        await output.WriteAsync([], Sitemap.FileName, Sitemap.For(Website, output.ProducedFiles()));

        // Last, because it removes everything the steps above did not produce. User
        // resources are copied before it for that reason - and after the theme resources,
        // so a user file still wins over a theme file of the same name.
        _log("Cleaning up...");
        await output.CleanUpAsync();

        _log($"Success! Your website has been generated at {Output}");
    }
}
