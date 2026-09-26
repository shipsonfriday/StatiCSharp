using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

/// <summary>
/// Manager that handles the website generating process with the given website and theme.
/// </summary>
public partial class WebsiteManager : IWebsiteManager
{
    private HtmlBuilder _htmlBuilder = new HtmlBuilder(useDefaultMarkdownParser: true);
    /// <inheritdoc/>
    public bool UseDefaultMarkdownParser
    {
        get => _htmlBuilder.UseDefaultMarkdownParser;
        set
        {
            _htmlBuilder.UseDefaultMarkdownParser = value;
        }
    }
    /// <inheritdoc/>
    public string SourceDir { get; set; }

    /// <inheritdoc/>
    public string Content { get; set; }

    /// <inheritdoc/>
    public string Resources { get; set; }

    /// <inheritdoc/>
    public string Output { get; set; }

    /// <inheritdoc/>
    public bool GitMode { get; set; } = false;

    /// <summary>
    /// List of all used paths while creating the sites.<br/>
    /// Used to find identical paths from meta data and to find files that have no markdown equivalent (got deleted) in GitMode.
    ///  </summary>
    private List<string> PathDirectory { get; set; }

    /// <inheritdoc/>
    public IWebsite Website { get; set; }

    /// <inheritdoc/>
    public IHtmlFactory HtmlFactory { get; set; }

    /// <summary>
    /// Initialize a new manager that generates the output from a given website and theme.
    /// </summary>
    /// <param name="website">The website that contains the content.</param>
    /// <param name="htmlFactory">The theme for the website.</param>
    /// <param name="source">The absolute path to the directory that contains the folders `Content`, `Output` and `Resources`.</param>
    public WebsiteManager(IWebsite website, IHtmlFactory? htmlFactory, string source)
    {
        Website = website;

        if (htmlFactory is null)
        {
            HtmlFactory = new DefaultHtmlFactory(Website);
        }
        else
        {
            HtmlFactory = htmlFactory;
        }

        SourceDir       = Path.Combine(source);
        Content         = Path.Combine(source, "Content");
        Resources       = Path.Combine(source, "Resources");
        Output          = Path.Combine(source, "Output");
        PathDirectory   = new List<string>();
    }

    /// <summary>
    /// Initialize a new manager that generates the output from a given website and theme.
    /// </summary>
    /// <param name="website">The website that contains the content.</param>
    /// <param name="source">The absolute path to the directory that contains the folders `Content`, `Output` and `Resources`.</param>
    public WebsiteManager(IWebsite website, string source) : this(website, null!, source)
    {
    }

    /// <inheritdoc/>
    public IWebsiteManager AddParser(IPipelineParser parser)
    {
        _htmlBuilder.AddToPipeline(parser);
        return this;
    }

    /// <inheritdoc/>
    public async Task Make()
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
