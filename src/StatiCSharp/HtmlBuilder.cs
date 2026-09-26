using Markdig;
using StatiCSharp.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace StatiCSharp;

internal class HtmlBuilder : IHtmlBuilder
{
    private List<IPipelineParser> _parsers = new();
    public bool UseDefaultMarkdownParser { get; set; } = false;

    public string AdditionalHeaderContent
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

    public HtmlBuilder()
    {
        
    }

    public HtmlBuilder(bool useDefaultMarkdownParser)
    {
        UseDefaultMarkdownParser= useDefaultMarkdownParser;
    }

    public void AddToPipeline(IPipelineParser parser)
    {
        _parsers.Add(parser);
    }

    public string ToHtml(string content)
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
