# Messages and warnings

While generating, StatiC# reports what it is doing and warns about anything in your content it
could not use. By default that goes to the console. A warning never stops a run: the generator
does the best it can with what it found and tells you what it had to leave out.

## Taking the messages somewhere else

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .WithLog(message => myLogger.LogInformation(message))
    .MakeAsync();
```

Progress lines and warnings both go there; a warning starts with `WARNING: `. Pass
`_ => { }` to stay quiet.

This is what a build pipeline wants, because a warning means a page came out differently than
you wrote it. Collecting them and failing the build is a few lines:

```C#
List<string> problems = [];

await WebsiteManager.For(myAwesomeWebsite, source: path)
    .WithLog(message =>
    {
        Console.WriteLine(message);
        if (message.StartsWith("WARNING: ", StringComparison.Ordinal)) { problems.Add(message); }
    })
    .MakeAsync();

if (problems.Count > 0) { return 1; }
```

The log may be called from several threads, because the pages are written in parallel.
`Console.WriteLine` copes with that; a collection of your own needs a lock.

## What the warnings mean

| Warning | What happened | What to do |
| --- | --- | --- |
| Could not read the date "…" | A `Date` entry is not ISO 8601. The file's modification date is used instead. | Write it as `2026-09-27`. See [meta data](meta_data_for_sites.md). |
| The key "…" appears more than once | Two lines of the front matter set the same entry. The last one wins. | Delete one. |
| Ignoring the line "…", because it has no colon | A line between the `---` markers is not an entry. The rest of the front matter is read normally. | Give it a colon, or move the text below the front matter. |
| The tags "…" all lead to /tag/… | Two tags differ only in spelling that a url drops, e.g. `Web Dev` and `web-dev`. One tag page is written. | Settle on one spelling. |
| The tag "…" has no characters that can be used in a url | A tag of nothing but symbols, e.g. `+++`. No tag page is written for it. | Rename or remove it. |
| … has no characters that can be used in a url, neither in its filename nor in its path entry | A markdown file whose name yields no url, e.g. `+++.md`. The file is skipped. | Rename it, or give it a `Path` in the front matter. |
| The section folder "…" has no characters that can be used in a url | Same for a folder named as a section. The section is skipped. | Rename the folder. |
| Two sites are written to … | Two markdown files end up at one url, usually through the same `Path` entry. Only the last one is kept. | Change the `Path` of one of them. |

## What stops a run

These are thrown out of `MakeAsync`, so a `try`/`catch` around it sees them. All of them are
about the setup rather than the content:

| Exception | When |
| --- | --- |
| `CannotCreateDirectoryException` | A directory the run needs is missing and cannot be created. |
| `DirectoryNotWriteableException` | A directory exists but cannot be written to. |
| `DirectoryNotFoundException` | The theme's resources directory does not exist. |
| `InvalidOperationException` | Your resources directory, or the theme's, holds the output directory - the output would be copied into itself. See [incremental output](incremental_output.md#where-the-resources-may-live). |

Both of the first two live in `StatiCSharp.Exceptions`.

Configuration is checked when you set it, not when the run starts, so a bad value throws from
the call that takes it: `Website.Create` rejects a url that is not an http address, and
`WithLanguage` rejects a language tag this machine does not know.
