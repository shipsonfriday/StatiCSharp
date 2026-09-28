<p align="center">
    <img src=".github/Images/Logo.svg" width="400" max-width="90%" alt="StatiC#" />
</p>

<p align="center">
    <a href="https://docs.microsoft.com/en-us/dotnet/csharp/">
        <img src="https://img.shields.io/badge/C%23-14.0-blue?style=flat" alt="C# 14.0" />
    </a>
    <a href="https://dotnet.microsoft.com">
        <img src="https://img.shields.io/badge/.NET-10.0-blueviolet?style=flat" />
    </a>
    <img src="https://img.shields.io/badge/Platforms-Win+Mac+Linux-green?style=flat" />
    <img src="https://img.shields.io/badge/Version-1.0.0-green?style=flat" />
    <a href="https://www.nuget.org/packages/StatiCSharp">
        <img src="https://img.shields.io/nuget/v/StatiCSharp?color=orange" />
    </a>
</p>

Welcome to **StatiC#**, a static website generator for C# developers. It enables entire websites to be built using C#. Custom themes can be used by editing the integrated default theme or importing a theme.

---

StatiC# provides everything you need to create a website with all the files needed to upload onto a web server.  

If you want to quickstart with your new website, you can start with the [default configuration](Documentation/ProjectTemplate) and build up from there. Here is an example:

```C#
using StatiCSharp;

var myAwesomeWebsite = Website.Create(
        url: "https://yourdomain.com",
        name: "My Awesome Website")
    .WithDescription("Description of your website")
    .WithLanguage("en-US")
    .WithSections("posts", "about");     // Folders that should be treated as sections.

await WebsiteManager
    .For(myAwesomeWebsite, source: @"C:\path\to\your\project")   // Holds Content, Resources and Output.
    .MakeAsync();
```


## Add StatiC# to your project

To get started, create a new console application at a path of your choice. Let's say that your new website is called *myWebsite*:

```
$ dotnet new console -n myWebsite
```
After .NET has created the project files open `myWebsite.csproj` and add StatiC# as a package reference. The file should then look something like this:

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="StatiCSharp" Version="1.0.0" />
  </ItemGroup>

</Project>
```

Then you can get started using StatiC# by importing it at the beginning of your `Program.cs`:

```C#
using StatiCSharp;
```

## Quick start

You can use StatiC#'s [project template](Documentation/ProjectTemplate) to quick start or follow the following steps to set up your project manually.  
Nevertheless it's recommended to read this readme to get a understanding how StatiC# works.  

StatiC# expects three folders to work with at the path given during the initialization of the WebsiteManager.  
  
`Content`: This folder contains the markdown files that our website depents on.  
`Output`: Here the final website with all the necessary files will be saved.  
`Resources`: Put all your static files in here. All files will be copied, without any manipulation, to the output. Folders are migrated.  

It's recommented to put those folders within your project folder of *myWebsite*. Your folder should look something like this:

```
├── myWebsite
│   ├── Content
│   ├── Output
│   ├── Resources
│   ├── myWebsite.csproj
│   ├── Program.cs
```
StatiC# renders four different types of sites:  

*index*: The homepage of your website  
*pages*: Normal sites e.g. your about page.  
*sections*: Sites that contain items e.g. articles in a specific field.  
*items*: The sites that are part of a section.  
  
Add some content to your website by adding your markdown files to the `Content` folder. Check out the [documentation](Documentation/) for a [template file](Documentation/HowTo/content-template.md):

```
├── myWebsite
│   ├── Content
│   │   ├── index.md                    // This is your homepage.
│   │   ├── posts                       // Contains all items for the posts section.
│   │   │   ├── index.md                // Content of the section site.
│   │   │   ├── your-first-post.md      // Item within the posts section.
│   │   │   ├── your-second-post.md     // Another item within the posts section.
│   │   ├── about                       // Contains a page.
│   │   │   ├── index.md                // Content of the about page.
│   │   │   ├── another-page.md         // Content of another page.
│   ├── Output
│   ├── Resources
│   ├── myWebsite.csproj
│   ├── Program.cs
```

Store your content in folders and StatiC# cares about the rest. All folders are treated as pages unless their name is used to build a section.  

Finally set up the parameters in `Program.cs` in your *myWebsite* project:

```C#
using StatiCSharp;

var myAwesomeWebsite = Website.Create(
        url: "https://yourdomain.com",
        name: "My Awesome Website")
    .WithDescription("Description of your website")
    .WithLanguage("en-US")
    .WithSections("posts", "about");     // Folders that should be treated as sections.

await WebsiteManager
    .For(myAwesomeWebsite, source: @"C:\path\to\your\project")   // Holds Content, Resources and Output.
    .MakeAsync();
```

Run the project and your new awesome website will be generated in the `Output` directory:
```
$ dotnet run
```

Check out the [documentation](Documentation/) for further information.

## Upgrading from 0.5

Version 1.0 changes the public API. The old constructors and settable properties are gone;
required values go into the entry point and everything optional follows fluently.

| 0.5 | 1.0 |
| --- | --- |
| `new Website(url, name, description, language, sections)` | `Website.Create(url, name).WithDescription(…).WithLanguage(…).WithSections(…)` |
| `new WebsiteManager(website, source)` | `WebsiteManager.For(website, source)` |
| `new WebsiteManager(website, theme, source)` | `WebsiteManager.For(website, source).WithTheme(theme)` |
| `manager.Make()` | `manager.MakeAsync()` |
| `manager.GitMode = true` | `manager.WithGitMode()` |
| `manager.Content = path` | `manager.WithContentDirectory(path)` |
| `manager.Output = path` | `manager.WithOutputDirectory(path)` |
| `manager.Resources = path` | `manager.WithResourcesDirectory(path)` |
| `manager.UseDefaultMarkdownParser = false` | `manager.WithoutDefaultMarkdownParser()` |
| `manager.AddParser(parser)` | unchanged, still chainable |

Three changes affect content rather than code:

- **Tag urls are normalized.** A tag is lowercased and spaces become hyphens, so `CSharp`
  is served from `/tag/csharp` and `Web Dev` from `/tag/web-dev`. External links to the old
  spelling break. Themes should build tag links with `StatiCSharp.Tools.UrlSlug.From(tag)`
  instead of interpolating the tag name.
- **Meta data is plain text.** `Title` and `Description` are no longer run through the
  markdown parser, so `title: My *great* post` now shows the asterisks. In exchange, a
  quote or an ampersand in any meta data field no longer breaks the page.
- **Dates must be ISO 8601**, e.g. `2026-09-27`. That was always what the documentation
  said, but the parser used to accept whatever the build machine's locale happened to
  allow, which meant the same file could yield different dates on different machines.

Requires .NET 10.

### If you wrote a theme

Themes have to be rebuilt. `IHtmlFactory` is unchanged, but the html components are not:

| 0.5 | 1.0 |
| --- | --- |
| `new Div().Add(a).Add(b)` | `new Div(a, b)` or `new Div { a, b }` |
| `Content` in a custom element | `Children` |
| `Href($"/tag/{tag}")` | `Href($"/tag/{UrlSlug.From(tag)}")` |
| `new Text(item.Title)` | encode it - meta data is plain text now |

In exchange the element set grew from 20 to 67, every element takes any attribute through
`Attribute()`, custom elements are actually possible, and a chain no longer depends on the
order of its calls. See [making a custom theme](Documentation/HowTo/making_a_custom_theme.md).

## Dependencies

- [Microsoft .NET](https://dotnet.microsoft.com/)
- [Markdig](https://github.com/xoofx/markdig)



## Contributions and support

StatiC# is developed completely open, and your contributions are more than welcome.

Before you start using StatiC# in any of your projects, please have in mind that it’s a hobby project and there is no guarantee for technical correctness or future releases.  

Since this is a very young project, it’s likely to have many limitations and missing features, which is something that can really only be discovered and addressed as you use it. While StatiC# is used in production on my personal website, it’s recommended that you first try it out for your specific use case, to make sure it supports the features that you need.  

If you wish to make a change, [open a Pull Request](https://github.com/RolandBraunDev/StatiCSharp/pull/new) — even if it just contains a draft of the changes you’re planning, or a test that reproduces an issue — and we can discuss it further from there.

I hope you’ll enjoy using StatiC#!
