# Making a custom theme

StatiC# is built to provide third-party templates. It's more than welcome that users create their own templates, to make their website even greater. Moreover, it would be fantastic if those templates were shared with other users over GitHub or NuGet as a standalone package.  

This article describes everything you need to start building your very own template. The only thing you must know is to code C#. StatiC# will deliver the tools you'll need.  

You can code your template purely in C# if you want. Additionally, you can build reusable components.

## Supported HTML components

StatiC# ships 67 elements. Every type is named after the tag it produces, so the type name
lowercased is the tag - with two exceptions that carry a short form, `Paragraph` (`P`) for
`<p>` and `Image` (`Img`) for `<img>`.

| | |
| --- | --- |
| **Sectioning** | `body` `header` `nav` `main` `section` `article` `aside` `footer` `div` `figure` `figcaption` |
| **Headings** | `h1` `h2` `h3` `h4` `h5` `h6` |
| **Text** | `p` `span` `strong` `em` `small` `mark` `code` `pre` `blockquote` `cite` `abbr` `sub` `sup` `time` `br` `hr` |
| **Lists** | `ul` `ol` `li` `dl` `dt` `dd` |
| **Tables** | `table` `caption` `thead` `tbody` `tfoot` `tr` `th` `td` |
| **Links and media** | `a` `img` `picture` `source` `video` `audio` `iframe` |
| **Forms** | `form` `label` `input` `button` `select` `option` `textarea` |
| **Disclosure** | `details` `summary` |
| **Head** | `link` `meta` `script` `style` |

There is no `html`, `head` or `title` element: the document shell around your body is
written by StatiC# itself. Use `MakeHeadHtml(…)` to add to the `<head>`.

Elements with attributes specific to them carry typed methods for those - `A.Href()`,
`Link.Rel()`, `Time.DateTime()`, `Th.Scope()`, `Td.ColSpan()`, `Img.Alt()` and so on. For
anything not covered, every element has `Attribute()`:

```C#
new Div().Attribute("data-section", "posts").Attribute("role", "region")
new Details().Attribute("open")        // an attribute without a value
new Td().Attribute("colspan", 3)       // numbers are formatted invariantly
```

Attribute values are encoded when the element renders, and attribute *names* are rejected
if they contain characters that would break out of the tag.

## Writing a tree

There are two ways to nest elements, and they produce the same html. Pass the children as
arguments:

```C#
new Body(
    new Div(
        new Article(
            new H1("Title"),
            new Div(page.Content).Class("content"))).Class("wrapper"))
```

Or write them as a collection initializer:

```C#
new Body
{
    new Div
    {
        new Article
        {
            new H1("Title"),
            new Div(page.Content).Class("content"),
        },
    }.Class("wrapper"),
}
```

Attributes are written in the order you set them, and every method hands back your own element
type, so `new A("x").Class("nav").Href("/")` and `new A("x").Href("/").Class("nav")` both
compile.

If the `new` keyword on every line bothers you, import the factories and it disappears:

```C#
using static StatiCSharp.HtmlComponents.Tags;

Body(
    Div(
        Article(
            H1("Title"),
            Div(page.Content).Class("content"))).Class("wrapper"))
```

## Getting started

To get started, create a new class library project in [.NET](https://dotnet.microsoft.com/en-us/) 10 or higher and add [StatiC#](https://github.com/RolandBraunDev/StatiCSharp) as a package reference to the project. Feel free to check out the [integrated template](https://github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/DefaultHtmlFactory.cs) while following this documentation.  
On the top of your class-file import `StatiCSharp.HtmlComponents` and `StatiCSharp.Interfaces`:

```C#
using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;
```

Create a new class that will handle your template. This class needs to implement `IHtmlFactory`. You can call it whatever you want, but it is StatiC# convention to call it `NameOfYourTemplateHtmlFactory`.

```C#
namespace YourTemplate
{
    public class YourTemplateHtmlFactory: IHtmlFactory
    {
        ...
    }
}
```

When adding `IHtmlFactory` your IDE will prompt you to add the following property and methods to your class:  
- `public string ResourcesPath` The absolute path to the resources your template uses, like css files or images. How to work with those files is explained later.
- `public string ResourcesPath` The absolute path to the resources your template uses.
- `public string MakeHeadHtml(ISite site, RenderContext context)` Creates HTML-code for inside the \<head>-tag. Called for every site, with that site, so you can add something that differs per site.
- `public string MakeIndexHtml(IIndex index, RenderContext context)` Method that returns the \<body> HTML-code for the index site.
- `public string MakePageHtml(IPage page, RenderContext context)` Method that returns the \<body> HTML-code for a page (not section or item).
- `public string MakeSectionHtml(ISection section, RenderContext context)` Method that returns the \<body> HTML -code for a section site.
- `public string MakeItemHtml(IItem item, RenderContext context)` Method that returns the \<body> HTML -code for an item site.
- `public string MakeTagListHtml(List<IItem> items, string tag, RenderContext context)` Method that returns the \<body> HTML -code for the taglist site.

### Writing a date

`StatiCSharp.Tools.DateText.For(date, context.Website.Language)` writes a date the way the
website's language writes it, in wording and in arrangement:

| Language | Result |
| --- | --- |
| `en-US` | March 4, 2020 |
| `de-DE` | 4. März 2020 |
| `fr-FR` | 4 mars 2020 |
| `ja-JP` | 2020年3月4日 |

Reach for it rather than a fixed pattern. `date.ToString("MMMM dd, yyyy", culture)` looks
right and is not: it takes the month name from the language but keeps an English order, so a
German site reads `März 04, 2020`.

### The render context

Every method is given a `RenderContext`. It holds the website's configuration and everything
that was read, so you never have to keep anything in your theme:

| | |
| --- | --- |
| `context.Website` | the url, name, description and language you configured |
| `context.Index` | the homepage |
| `context.Pages` | every page |
| `context.Sections` | every section with its items |

That is what you reach for when a site needs more than itself - a navigation, a sitemap, a
list of the newest articles across all sections, a tag cloud. Your theme itself stays
stateless, which means one instance can render any website and you can build a context by
hand in your own tests:

```C#
var context = new RenderContext { Website = Website.Create("https://example.com", "Test") };
```

## Build your first site

First, let's look at the method called `Make...Html()`. As their names suggest, these methods deliver the HTML-Code for the sites their name stands for. StatiC# calls these methods during the website generating process and injects the parameters depending on the item that is processed. The return type of that methods is always a string containing the HTML-Code for this site. Note: Only the HTML-Code within the \<body>-tag.  The \<head> is generated by StatiC#, although you can add elements to the \<head> with the `MakeHeadHtml()` method.   
You can access the content through the object given with the parameters. Those objects do always implement the corresponding interface.  

By using the StatiC#-HTML-Components, you can write HTML-Code in C#. Let's check this out with an example with the `MakePageHtml()` method from the integrated default theme.

```C#
public string MakePageHtml(IPage page, RenderContext context)
{
    return new Body(
            new SiteHeader(context.Website),
            new Div(
                new Article(
                    new Div(page.Content).Class("content"))).Class("wrapper"),
            new Footer())
        .Render();
}
```

In this case, inspect the [IPage interface](github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/Interfaces/IPage.cs) for information about the content you can access via `page`. Pay attention to the fact that all parameter interfaces inherit from [ISite](github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/Interfaces/ISite.cs), so you always have access to those properties, too.  
Initiate a new `Body` object, which is a representation of your current body of the HTML site. Then follows the elements you want to add to the body of the page. You see that you can use chaining, and you are able to nest the elements. This makes your code more readable. Imagine: The code above is everything you need to display a page.  
`SiteHeader` and `Footer` are not basic HTML elements. They are custom components that can be used across all your sites. You can create those components with the use of other components or whatever you want. But you need to implement [IHtmlComponent](github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/Interfaces/IHtmlComponent.cs) to work with StatiC#. To ensure chaining, you have to return the element itself after every method you implement to customize the element.  
`SiteHeader` is given `context.Website` because it builds the navigation from the configured section names. Whatever a component needs, hand it over from the context - a theme has no `Website` of its own.  

Here is an example from the [integrated default theme](https://github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/DefaultHtmlFactory.cs) for a custom component called SiteFooter:

```C#
private class SiteFooter : IHtmlComponent
{
    public string Render()
    {
        return new Footer(
                new Paragraph(
                    new Text("Generated with ❤️ using "),
                    new A("StatiC#").Href("https://github.com/RolandBraunDev/StatiCSharp")))
            .Render();
    }
}
```

Give your components names that do not collide with the elements - `SiteFooter` rather than
`Footer`. The integrated theme calls its own component `Footer` and has to write
`StatiCSharp.HtmlComponents.Footer` everywhere as a result.

By the way, it would be nice if you implement this reference to StatiC# in your templates.

## Text and markup

`Text` renders its string unchanged, and so does `Add(string)`, which wraps its argument in
a `Text`. That is deliberate: a site's `Content` is already HTML by the time your theme sees
it, so encoding it would print the page as visible source.

Everything else coming off a site is plain text and has to be encoded before it goes into
the output. That includes `Title`, `Description`, `Author` and the tags - a quote in a title
or an ampersand in an author name breaks the page otherwise. The integrated default theme
uses a small helper for this:

```C#
private static Text Plain(string text) => new(System.Net.WebUtility.HtmlEncode(text));

// ...
new A(Plain(item.Title)).Href(item.Url)
```

Attribute values set through methods like `Class()`, `Id()` or `Href()` are encoded by
StatiC# itself, so those need no extra care.

## Linking to tag pages

StatiC# writes a tag page to a normalized path: lowercased, with spaces turned into
hyphens. Build the link with the same rule instead of interpolating the tag name, or the
link points at a directory that is not there:

```C#
using StatiCSharp.Tools;

new A(Plain(tag)).Href($"/tag/{UrlSlug.From(tag)}")
```

## Making your own element

If an element is missing, derive from `HtmlElement<T>` with your own type as the argument
and name the tag. That is the whole requirement:

```C#
using StatiCSharp.HtmlComponents;
using StatiCSharp.Interfaces;

public class Dialog : HtmlElement<Dialog>
{
    protected override string TagName => "dialog";

    public Dialog() { }

    public Dialog(params IHtmlComponent[] content) => Children = [.. content];

    public Dialog(string text) => Children = [new Text(text)];

    public Dialog Open() => Attribute("open");
}
```

Your element gets everything the built-in ones have: both nesting syntaxes, the fluent
attribute methods typed to `Dialog`, and the attribute name checking.

```C#
new Dialog(new H1("Title"), new Paragraph("Text")).Open().Class("modal")
```

`Children` is the list of components inside the element; assign it in your constructors.
For an element that takes no content, override `VoidElement`:

```C#
public class Wbr : HtmlElement<Wbr>
{
    protected override string TagName => "wbr";

    protected override bool VoidElement => true;
}
```

Deriving from the non-generic `HtmlElement` also works if you do not need the fluent chain.

One thing to expect: the analyzer reports **CA1010** on your element, because `HtmlElement`
implements `IEnumerable` so that the collection initializer works. Your element is not a
collection, so the warning does not apply - switch it off for your components folder:

```ini
[YourTheme/Components/*.cs]
dotnet_diagnostic.CA1010.severity = none
```

Contributions of new elements to StatiC# itself are welcome, too.

## Managing resources

With all the steps above, you can build the HTML code for all sites generated by StatiC#. If your templates consist just of HTML-Code... great, then you are done at this point. But for most of the cases, you want to provide additional data like CSS files, javascript, or images. Follow these steps to add those resources to your template.  

Add a folder to your project's root directory and call it `YourThemNameResources`. Within this folder, create a new one called `yourthemename-theme`. Again, this is a StatiC# convention. StatiC# will copy all files and folders you provide in `YourThemeNameResources` to the root directory of the generated website. The user is also capable of using additional resources. To prevent potential conflicts, you are recommended to put everything that has to do with your theme into the separate folder `yourthemename-theme`. There is only one exception: If you want to provide a favicon, put it in `YourThemeNameResources`. If the user provides a favicon, yours will be overridden.  

Tell StatiC# from where to copy your resources by using the `ResourcesPath` property. When following this guide, your files will be placed in the mentioned directory next to the `.dll` of your template. One way to give StatiC# the path of that directory is:

```C#
public string ResourcesPath
{
    get
    {
        string? path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        return Path.Combine(path!, "YourThemeNameResources");
    }
}
```

You have to implement into your project that the resources are copied to the output directory of the website generating process. Let's say you want to provide a `styles.css` file:  
Add the file to your project into `yourthemename-theme`.  
Open your `.csproj` file and add the file as `<None Remove="..." />`:

```
<ItemGroup>
    <None Remove="YourThemeNameResources\yourthemename-theme\styles.css" />
</ItemGroup>
```

Additionally, include the file as `<Content Include="..." />` and set the parameters as shown below. With that set, the file will be copied to the output directory either if the theme is used via a package reference or a project reference:

```
<ItemGroup>
    <Content Include="YourThemeNameResources\yourthemename-theme\styles.css" pack="true">
        <PackageCopyToOutput>true</PackageCopyToOutput>
	<CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </Content>	
</ItemGroup>
```

You have to provide these steps for all your files.  

Your file is available at `/yourthemename-theme/styles.css`. Add this CSS-reference  by using the `MakeHeadHtml()`:

```C#
public string MakeHeadHtml(ISite site, RenderContext context)
{
    return "<link rel=\"stylesheet\" href=\"/yourthemename-theme/styles.css\">";
}
```

This would be equivalent to javascript files. If you want to access images or other files within the \<body> tag, you can link to them the same way.

---

Check out the [default theme](https://github.com/RolandBraunDev/StatiCSharp/blob/master/src/StatiCSharp/DefaultHtmlFactory.cs) to see a template in action. I will be pleased if you give me feedback on this guide so that I can make it better and make the entry to build custom themes with StatiC# as smoothly as possible. It would be great if many developers bring in their template ideas, and there would be a large number of templates to choose from.
