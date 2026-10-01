using Markdig;
using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp;

/// <summary>
/// Turns the markdown content of a site into html by running it through the parser pipeline.
/// <para>
/// The parsers a caller adds run first, in the order they were added, and the integrated
/// Markdig parser runs last unless it is switched off.
/// </para>
/// </summary>
internal sealed class HtmlBuilder
{
    private readonly List<IPipelineParser> _parsers = new();

    /// <summary>
    /// Whether the integrated Markdig parser runs at the end of the pipeline.
    /// </summary>
    internal bool UseDefaultMarkdownParser { get; set; }

    /// <summary>
    /// What the parsers need in the head of every site for their output to work, all of them
    /// one after another.
    /// </summary>
    internal string AdditionalHeaderContent
    {
        get
        {
            var headerBuilder = new StringBuilder();
            foreach (var parser in _parsers)
            {
                headerBuilder.Append(parser.HeaderContent);
            }
            return headerBuilder.ToString();
        }
    }

    /// <summary>
    /// Starts a pipeline.
    /// </summary>
    /// <param name="useDefaultMarkdownParser">Whether the integrated Markdig parser runs at the end.</param>
    internal HtmlBuilder(bool useDefaultMarkdownParser)
    {
        UseDefaultMarkdownParser = useDefaultMarkdownParser;
    }

    /// <summary>
    /// Adds a parser to the end of the pipeline.
    /// </summary>
    /// <param name="parser">The parser to add.</param>
    internal void AddToPipeline(IPipelineParser parser)
    {
        _parsers.Add(parser);
    }

    /// <summary>
    /// Runs the content through every parser in the pipeline.
    /// </summary>
    /// <param name="content">The content below the front matter of a markdown file.</param>
    /// <returns>The content as html.</returns>
    internal string ToHtml(string content)
    {
        var currentHtml = content;

        foreach (var parser in _parsers)
        {
            currentHtml = parser.Parse(currentHtml);
        }

        if (UseDefaultMarkdownParser)
        {
            var pipeline = new MarkdownPipelineBuilder()
               .UseAdvancedExtensions()
               .Build();

            currentHtml = Markdig.Markdown.ToHtml(currentHtml, pipeline);
        }

        return currentHtml;
    }
}
