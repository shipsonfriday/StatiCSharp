# Use Themes

**StatiC#** makes it easy to use different themes for your website. This article shows how to use [Foundation](https://www.nuget.org/packages/StatiCSharp.Theme.Foundation).  

## Add a template to your website project

Add the template of your choice to your website project as a project or package reference. Check out the documentation of your template for more details. Here, we implement Foundation as a package reference in the project file:

```
<ItemGroup>
    <PackageReference Include="StatiCSharp.Themes.Foundation" Version="0.1.1" />
</ItemGroup>
```
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

var theme = new FoundationHtmlFactory(myAwesomeWebsite);

await WebsiteManager
    .For(myAwesomeWebsite, source: @"/path/to/your/project")   // Folder of your website project.
    .WithTheme(theme)
    .MakeAsync();
```

A theme built against StatiC# 0.5 still works: `IHtmlFactory` is unchanged. Only the way
the theme is handed to the manager moved from a constructor argument to `WithTheme()`.

Build and run your project. Your website is created with the new theme in your `Output` directory.

```bash
$ dotnet run
```
