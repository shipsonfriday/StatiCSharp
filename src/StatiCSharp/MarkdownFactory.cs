using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static StatiCSharp.StatiCSharpConsole;

namespace StatiCSharp;

/// <summary>
/// Provides usefull methods to work with markdown files and data.
/// </summary>
internal static class MarkdownFactory
{
    /// <summary>
    /// Parses the meta data (yaml) of a markdown file.
    /// <para>
    /// A malformed line is skipped on its own. Blank lines and comments inside the front
    /// matter are ignored, anything else without a colon is reported. A key that appears
    /// twice keeps the last value and is reported as well.
    /// </para>
    /// </summary>
    /// <param name="path">Path to the markdown file.</param>
    /// <returns>A Dictionary&lt;string, string&gt; with the parsed meta data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    public static Dictionary<string, string> ParseMetaData(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        Dictionary<string, string> metaData = [];
        string[] lines = File.ReadAllLines(path);
        List<int> yamlMarker = YamlMarkers(lines);

        if (yamlMarker.Count != 2)
        {
            return metaData;
        }

        for (int i = yamlMarker[0] + 1; i < yamlMarker[1]; i++)
        {
            string line = lines[i].Trim();

            // Blank lines and yaml comments carry no meta data and are not a mistake.
            if (line.Length == 0 || line[0] == '#')
            {
                continue;
            }

            int indexOfColon = line.IndexOf(':', StringComparison.Ordinal);

            if (indexOfColon < 0)
            {
                WriteLine($"WARNING: Ignoring the line \"{lines[i]}\" in the meta data of {path}, because it has no colon.");
                continue;
            }

            // Invariant, so that a machine set to Turkish does not turn "Title" into
            // "tıtle", which would never match the key the generator looks for.
            string key = line[..indexOfColon].Trim().ToLowerInvariant();
            string value = line[(indexOfColon + 1)..].Trim();

            if (!metaData.TryAdd(key, value))
            {
                WriteLine($"WARNING: The key \"{key}\" appears more than once in the meta data of {path}. Using the last value.");
                metaData[key] = value;
            }
        }

        return metaData;
    }

    /// <summary>
    /// Parses the content of the markdownfile, while slicing the meta data.
    /// </summary>
    /// <param name="path">Path to the markdown file.</param>
    /// <returns>A string with the parsed content.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    public static string ParseContent(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        string[] lines = File.ReadAllLines(path);
        List<int> yamlMarker = YamlMarkers(lines);

        if (yamlMarker.Count == 0) // No meta data available
        {
            return string.Join("\n", lines);
        }

        string[] linesWithoutMetaData = new ArraySegment<string>(lines, yamlMarker[1] + 1, lines.Length - yamlMarker[1] - 1).ToArray();
        return string.Join("\n", linesWithoutMetaData);
    }

    /// <summary>
    /// Finds the line number of the so called yaml marker "---".<br/>
    /// The meta data is then between those lines.<br/>
    /// Only looking for the first two markers. If there is no marker in the first line, it is assumed that there is no meta data given.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns>A List&lt;int&gt; with ether zero entries if no meta data was found, or two entries identifying the marker positions. (0 is first line in the document)</returns>
    private static List<int> YamlMarkers(string[] lines)
    {
        List<int> marker = [];

        // An empty file has no first line to look at.
        if (lines.Length == 0 || lines[0] != "---")
        {
            return marker;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "---")
            {
                marker.Add(i);
            }

            if (marker.Count == 2)
            {
                break;
            }
        }

        if (marker.Count == 1)
        {
            marker.Clear();
        }

        return marker;
    }
}
