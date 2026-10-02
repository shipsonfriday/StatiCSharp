# Use Themes

**StatiC#** makes it easy to use different themes for your website. This article shows how to use [Foundation](https://www.nuget.org/packages/StatiCSharp.Theme.Foundation).  

## Add a template to your website project

Add the template of your choice to your website project as a project or package reference. Check out the documentation of your template for more details. Here, we implement Foundation as a package reference in the project file:

```
<ItemGroup>
    <PackageReference Include="StatiCSharp.Theme.Foundation" Version="<the version built for StatiC# 1.0>" />
</ItemGroup>
```

The package id is `StatiCSharp.Theme.Foundation`, singular. Check which version was built
against the StatiC# release you are using - a theme compiled against 0.5 does not run on
1.0, see below.
You can use the NuGet package manager as well.  
Build your project to restore packages.  

Now we can import the theme in the `Program.cs` of the website project, initiate a new member, and inject it to the StatiC# website generating process:

```C#
using StatiCSharp;
using Foundation;

var myAwesomeWebsite = Website.Create(
        url: "https://yourdomain.com",
        name: "My Awesome Website")
    .WithDescription("Description of your website")
    .WithLanguage("en-US")
    .WithSections("posts", "about");    // Folders that should be treated as sections.

await WebsiteManager
    .For(myAwesomeWebsite, source: @"/path/to/your/project")   // Folder of your website project.
    .WithTheme(new FoundationHtmlFactory())
    .MakeAsync();
```

A theme no longer takes the website. It is given everything it needs when a site is rendered,
so one instance works for any website and its constructor is free for the theme's own
options, e.g. `new FoundationHtmlFactory(accentColor: "#c0392b")`.

A theme built against StatiC# 0.5 has to be rebuilt for 1.0, and its source has to be
adjusted: `IHtmlFactory` changed, and so did the html components it renders with.

What a theme author has to do:

- Add a `RenderContext context` parameter to every `Make…Html` method, and `ISite site` plus
  `RenderContext context` to `MakeHeadHtml`. Whatever the theme took the website for is on
  `context.Website`; the content it could not reach before is on `context.Sections`,
  `context.Pages` and `context.Index`.
- Drop the constructor that took an `IWebsite`.
- Replace chained `.Add(...)` calls with constructor arguments or a collection initializer,
  as described in [making a custom theme](making_a_custom_theme.md#writing-a-tree).
- Rename `Content` to `Children` in any custom element.
- Build tag links with `StatiCSharp.Tools.UrlSlug.From(tag)` rather than interpolating the
  tag name, since tag directories are normalized now.
- Encode meta data - `Title`, `Description`, `Author`, tags - before rendering it. Those are
  plain text in 1.0 and are no longer escaped on the way in.

Build and run your project. Your website is created with the new theme in your `Output` directory.

```bash
$ dotnet run
```
