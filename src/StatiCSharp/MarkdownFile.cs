namespace StatiCSharp;

/// <summary>
/// A markdown file as the generator sees it: the entries of its front matter and the content
/// below it.
/// <para>
/// Read in one pass. Parsing the meta data and parsing the content used to be two calls, each
/// opening the file and scanning for the <c>---</c> markers again, although no caller ever
/// wanted only one of the two.
/// </para>
/// </summary>
/// <param name="MetaData">The entries of the front matter, keyed by their lowercased name.</param>
/// <param name="Content">Everything below the front matter, with line feeds as line breaks.</param>
internal sealed record MarkdownFile(Dictionary<string, string> MetaData, string Content);
