# Incremental output

__StatiC#__ updates the output directory instead of rebuilding it. A file is written only
when its content actually changed, new files are created as needed, and files that no
longer belong to the website are deleted — so deleting a markdown file deletes the
article it produced.

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
in the output. Everything in the output directory is deleted, including files StatiC# did
not put there.
