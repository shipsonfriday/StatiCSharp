# Incremental output

__StatiC#__ updates the output directory instead of rebuilding it. A file is written only
when its content actually changed, new files are created as needed, and files that no
longer belong to the website are deleted — so deleting a markdown file deletes the
article it produced, and deleting a file from `Resources` removes it from the output too.

That counts for the files in `Resources` as well, not only for the generated html. They are
compared byte by byte, so an image stays untouched until you actually replace it.

That is the default and needs no configuration:

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .MakeAsync();
```

## Why it is the default

Mostly for websites under source control. Rewriting a file that did not change makes git
report a modification where there is no new content, and a deploy that compares files
uploads it again. Leaving it untouched keeps the diff to what you actually wrote.

Note that it is the file's *content* git looks at, not its timestamps — git stores no
modification time, so a rewritten but identical file would show up as unchanged anyway.
The reason to skip the write is the write itself: tools that watch the output directory,
rsync without checksums, and build caches all go by the modification time.

## Building from scratch

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .NoIncrementalOutput()
    .MakeAsync();
```

This empties the output directory first and writes every file anew. You rarely want it,
but it is the way to a guaranteed clean result — for instance after changing a theme,
since the generator cannot tell that the html it would produce now differs from the html
in the output. Everything in the output directory is deleted, except the names listed
below.

## Where the resources may live

Everything below your `Resources` directory, and below your theme's resources directory, is
copied into the output. Neither may therefore be the output directory or hold it - the output
would be copied into itself, one level deeper on every run. StatiC# refuses such a run before
it writes anything. A resources directory *inside* the output is fine; those files are simply
copied up into it.

## Files StatiC# never deletes

Both modes remove what does not belong to the website, so anything you put into the output
directory by hand would be gone on the next run. Three names are always kept:

| Name | Why |
| --- | --- |
| `.git` | The output directory is often the repository it is deployed from. |
| `.nojekyll` | GitHub Pages reads it and nothing generates it. |
| `CNAME` | Your custom domain, same story. |

Add your own:

```C#
await WebsiteManager.For(myAwesomeWebsite, source: @"/path/to/your/project")
    .WithPreservedOutput("robots.txt", ".well-known")
    .MakeAsync();
```

A name matches a file or a directory anywhere in the output, whatever its case, and a
preserved directory is kept whole. The call adds to the list rather than replacing it, so
the three names above cannot be lost.

Most of the time you do not need this: put the file into your `Resources` directory
instead and StatiC# copies it into the output for you. Reach for `WithPreservedOutput` when
something else writes into the output directory — your deploy, your editor, or you.
