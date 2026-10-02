using System;
using System.Collections.Generic;
using System.Linq;

namespace StatiCSharp.Tests;

/// <summary>
/// Collects what the generator reports, so a test can assert on it. Before there was a log
/// hook, every message went straight to the console and no test could see one.
/// </summary>
internal sealed class RecordedLog
{
    private readonly List<string> _messages = [];
    private readonly object _lock = new();

    /// <summary>
    /// Pass this where an <c>Action&lt;string&gt;</c> is wanted. Locked, because the sites are
    /// written in parallel and a List is not safe for that.
    /// </summary>
    public Action<string> Write => message =>
    {
        lock (_lock)
        {
            _messages.Add(message);
        }
    };

    public IReadOnlyList<string> Messages
    {
        get
        {
            lock (_lock)
            {
                return [.. _messages];
            }
        }
    }

    public IReadOnlyList<string> Warnings
        => [.. Messages.Where(message => message.StartsWith("WARNING: ", StringComparison.Ordinal))];

    /// <summary>
    /// Whether any message contains all of the given parts.
    /// </summary>
    public bool Reported(params string[] parts)
        => Messages.Any(message => parts.All(part => message.Contains(part, StringComparison.Ordinal)));
}
