using System.Collections;

namespace StatiCSharp.Interfaces;

/// <summary>
/// Gives a method to parse a string, depending on the implementation.
/// </summary>
public interface IPipelineParser
{
    /// <summary>
    /// Optional HTML-Header content that the result of the parser needs.
    /// </summary>
    public string HeaderContent { get; }

    /// <summary>
    /// Parses a string, depending on the implementation.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    string Parse(string input);
}
