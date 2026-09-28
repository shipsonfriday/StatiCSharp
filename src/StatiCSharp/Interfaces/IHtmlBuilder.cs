namespace StatiCSharp.Interfaces;

/// <summary>
/// Responsible for creating valid HTML from markdown strings.
/// </summary>
public interface IHtmlBuilder
{
    /// <summary>
    /// Content that is added to the header of every site. This content is needed to get the resulting HTML work.
    /// </summary>
    string AdditionalHeaderContent { get; }

    /// <summary>
    /// Add a parser to the end of the pipeline.
    /// </summary>
    /// <param name="parser"></param>
    void AddToPipeline(IPipelineParser parser);

    /// <summary>
    /// Generates HTML from the input parameter. The predefined parser pipeline is used.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    string ToHtml(string content);
}
