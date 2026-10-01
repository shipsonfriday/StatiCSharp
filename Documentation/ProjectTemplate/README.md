# Project Template

A minimal StatiC# website to start from. Copy the `myWebsite` folder to wherever you keep
your projects, rename it, and run it:

```bash
cd myWebsite
dotnet run
```

The generated website ends up in `Output`.

```
myWebsite
├── myWebsite.csproj     References StatiCSharp from NuGet.
├── Program.cs           Configures the website and starts the generator.
├── Content              Your markdown files.
│   ├── index.md         The homepage.
│   ├── posts            A section - listed in Program.cs under WithSections.
│   │   ├── index.md     The section's own page; the theme lists the articles below it.
│   │   └── your-first-article.md
│   └── about            A folder that is not a section, so it becomes a page at /about.
│       └── index.md
├── Resources            Static files. Copied to the root of the output unchanged.
└── Output               Generated. Safe to delete at any time.
```

Add a section by creating a folder under `Content` and naming it in `WithSections(...)`.
Any other folder becomes a page.

See the [how-to's](../) for metadata, themes, incremental output and custom parsers, and the
[readme](../../README.md) for the full quick start.
