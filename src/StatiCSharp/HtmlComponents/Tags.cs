using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents;

/// <summary>
/// Creates html elements without writing <c>new</c>, which makes a nested tree read
/// closer to the markup it produces.
/// <para>
/// Import it statically and the keyword disappears from every line:
/// <code>
/// using static StatiCSharp.HtmlComponents.Tags;
///
/// Body(
///     Div(
///         Article(
///             H1(title),
///             Div(Text(content)).Class("content"))).Class("wrapper"))
/// </code>
/// </para>
/// <para>
/// Every method is named after its element type, so this is purely a matter of leaving
/// out <c>new</c> - there is no second vocabulary. The constructors stay available and
/// produce identical output.
/// </para>
/// <para>
/// Two type names differ from the tag they produce, and both have a short form named
/// after the tag: <see cref="P(string)"/> for <see cref="Paragraph(string)"/> and
/// <see cref="Img(string)"/> for <see cref="Image(string)"/>. The other eighteen types
/// already carry their tag's name.
/// </para>
/// </summary>
public static class Tags
{
    /// <summary>Creates an empty an anchor element.</summary>
    public static A A() => new();

    /// <summary>Creates an anchor element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static A A(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an anchor element with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static A A(string text) => new(text);

    /// <summary>Creates an empty an article.</summary>
    public static Article Article() => new();

    /// <summary>Creates an article with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Article Article(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an article with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Article Article(string text) => new(text);

    /// <summary>Creates an empty a body.</summary>
    public static Body Body() => new();

    /// <summary>Creates a body with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Body Body(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a body with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Body Body(string text) => new(text);

    /// <summary>Creates an empty a div.</summary>
    public static Div Div() => new();

    /// <summary>Creates a div with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Div Div(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a div with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Div Div(string text) => new(text);

    /// <summary>Creates an empty a footer.</summary>
    public static Footer Footer() => new();

    /// <summary>Creates a footer with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Footer Footer(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a footer with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Footer Footer(string text) => new(text);

    /// <summary>Creates an empty a header.</summary>
    public static Header Header() => new();

    /// <summary>Creates a header with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Header Header(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a header with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Header Header(string text) => new(text);

    /// <summary>Creates an empty an h1.</summary>
    public static H1 H1() => new();

    /// <summary>Creates an h1 with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static H1 H1(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an h1 with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static H1 H1(string text) => new(text);

    /// <summary>Creates an empty an h2.</summary>
    public static H2 H2() => new();

    /// <summary>Creates an h2 with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static H2 H2(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an h2 with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static H2 H2(string text) => new(text);

    /// <summary>Creates an empty an h3.</summary>
    public static H3 H3() => new();

    /// <summary>Creates an h3 with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static H3 H3(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an h3 with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static H3 H3(string text) => new(text);

    /// <summary>Creates an empty an h4.</summary>
    public static H4 H4() => new();

    /// <summary>Creates an h4 with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static H4 H4(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an h4 with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static H4 H4(string text) => new(text);

    /// <summary>Creates an empty an h5.</summary>
    public static H5 H5() => new();

    /// <summary>Creates an h5 with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static H5 H5(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an h5 with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static H5 H5(string text) => new(text);

    /// <summary>Creates an empty an h6.</summary>
    public static H6 H6() => new();

    /// <summary>Creates an h6 with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static H6 H6(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates an h6 with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static H6 H6(string text) => new(text);

    /// <summary>Creates an empty a list item.</summary>
    public static Li Li() => new();

    /// <summary>Creates a list item with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Li Li(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a list item with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Li Li(string text) => new(text);

    /// <summary>Creates an empty an unordered list.</summary>
    public static Ul Ul() => new();

    /// <summary>Creates an unordered list with the given content.</summary>
    /// <param name="content">The list items inside it.</param>
    public static Ul Ul(params Li[] content) => new(content);

    /// <summary>Creates an empty an ordered list.</summary>
    public static Ol Ol() => new();

    /// <summary>Creates an ordered list with the given content.</summary>
    /// <param name="content">The list items inside it.</param>
    public static Ol Ol(params Li[] content) => new(content);

    /// <summary>Creates an empty a nav.</summary>
    public static Nav Nav() => new();

    /// <summary>Creates a nav with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Nav Nav(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a nav with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Nav Nav(string text) => new(text);

    /// <summary>Creates an empty a paragraph.</summary>
    public static Paragraph Paragraph() => new();

    /// <summary>Creates a paragraph with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Paragraph Paragraph(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a paragraph with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Paragraph Paragraph(string text) => new(text);

    /// <summary>Creates an empty a span.</summary>
    public static Span Span() => new();

    /// <summary>Creates a span with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Span Span(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a span with the given text as its content.</summary>
    /// <param name="text">The text inside it.</param>
    public static Span Span(string text) => new(text);

    /// <summary>Creates an image.</summary>
    public static Image Image() => new();

    /// <summary>Creates an image with the given source.</summary>
    /// <param name="source">The value of the src attribute.</param>
    public static Image Image(string source) => new(source);

    /// <summary>Creates an input element.</summary>
    public static Input Input() => new();

    /// <summary>Creates text. Written as it is, so encode it first if it is not yours.</summary>
    /// <param name="text">The text.</param>
    public static Text Text(string text) => new(text);

    /// <summary>Creates an empty paragraph. Short for <see cref="Paragraph()"/>.</summary>
    public static Paragraph P() => new();

    /// <summary>Creates a paragraph with the given content. Short for <see cref="Paragraph(IHtmlComponent[])"/>.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Paragraph P(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a paragraph with the given text. Short for <see cref="Paragraph(string)"/>.</summary>
    /// <param name="text">The text inside it.</param>
    public static Paragraph P(string text) => new(text);

    /// <summary>Creates an image. Short for <see cref="Image()"/>.</summary>
    public static Image Img() => new();

    /// <summary>Creates an image with the given source. Short for <see cref="Image(string)"/>.</summary>
    /// <param name="source">The value of the src attribute.</param>
    public static Image Img(string source) => new(source);

    /// <summary>Creates an empty &lt;main&gt; element.</summary>
    public static Main Main() => new();

    /// <summary>Creates a &lt;main&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Main Main(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;main&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Main Main(string text) => new(text);

    /// <summary>Creates an empty &lt;section&gt; element.</summary>
    public static Section Section() => new();

    /// <summary>Creates a &lt;section&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Section Section(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;section&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Section Section(string text) => new(text);

    /// <summary>Creates an empty &lt;aside&gt; element.</summary>
    public static Aside Aside() => new();

    /// <summary>Creates a &lt;aside&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Aside Aside(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;aside&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Aside Aside(string text) => new(text);

    /// <summary>Creates an empty &lt;figure&gt; element.</summary>
    public static Figure Figure() => new();

    /// <summary>Creates a &lt;figure&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Figure Figure(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;figure&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Figure Figure(string text) => new(text);

    /// <summary>Creates an empty &lt;figcaption&gt; element.</summary>
    public static Figcaption Figcaption() => new();

    /// <summary>Creates a &lt;figcaption&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Figcaption Figcaption(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;figcaption&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Figcaption Figcaption(string text) => new(text);

    /// <summary>Creates an empty &lt;strong&gt; element.</summary>
    public static Strong Strong() => new();

    /// <summary>Creates a &lt;strong&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Strong Strong(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;strong&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Strong Strong(string text) => new(text);

    /// <summary>Creates an empty &lt;em&gt; element.</summary>
    public static Em Em() => new();

    /// <summary>Creates a &lt;em&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Em Em(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;em&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Em Em(string text) => new(text);

    /// <summary>Creates an empty &lt;small&gt; element.</summary>
    public static Small Small() => new();

    /// <summary>Creates a &lt;small&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Small Small(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;small&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Small Small(string text) => new(text);

    /// <summary>Creates an empty &lt;mark&gt; element.</summary>
    public static Mark Mark() => new();

    /// <summary>Creates a &lt;mark&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Mark Mark(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;mark&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Mark Mark(string text) => new(text);

    /// <summary>Creates an empty &lt;code&gt; element.</summary>
    public static Code Code() => new();

    /// <summary>Creates a &lt;code&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Code Code(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;code&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Code Code(string text) => new(text);

    /// <summary>Creates an empty &lt;pre&gt; element.</summary>
    public static Pre Pre() => new();

    /// <summary>Creates a &lt;pre&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Pre Pre(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;pre&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Pre Pre(string text) => new(text);

    /// <summary>Creates an empty &lt;blockquote&gt; element.</summary>
    public static Blockquote Blockquote() => new();

    /// <summary>Creates a &lt;blockquote&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Blockquote Blockquote(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;blockquote&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Blockquote Blockquote(string text) => new(text);

    /// <summary>Creates an empty &lt;cite&gt; element.</summary>
    public static Cite Cite() => new();

    /// <summary>Creates a &lt;cite&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Cite Cite(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;cite&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Cite Cite(string text) => new(text);

    /// <summary>Creates an empty &lt;abbr&gt; element.</summary>
    public static Abbr Abbr() => new();

    /// <summary>Creates a &lt;abbr&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Abbr Abbr(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;abbr&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Abbr Abbr(string text) => new(text);

    /// <summary>Creates an empty &lt;sub&gt; element.</summary>
    public static Sub Sub() => new();

    /// <summary>Creates a &lt;sub&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Sub Sub(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;sub&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Sub Sub(string text) => new(text);

    /// <summary>Creates an empty &lt;sup&gt; element.</summary>
    public static Sup Sup() => new();

    /// <summary>Creates a &lt;sup&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Sup Sup(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;sup&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Sup Sup(string text) => new(text);

    /// <summary>Creates an empty &lt;time&gt; element.</summary>
    public static Time Time() => new();

    /// <summary>Creates a &lt;time&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Time Time(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;time&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Time Time(string text) => new(text);

    /// <summary>Creates a &lt;link&gt; element.</summary>
    public static Link Link() => new();

    /// <summary>Creates a &lt;meta&gt; element.</summary>
    public static Meta Meta() => new();

    /// <summary>Creates an empty &lt;script&gt; element.</summary>
    public static Script Script() => new();

    /// <summary>Creates a &lt;script&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Script Script(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;script&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Script Script(string text) => new(text);

    /// <summary>Creates an empty &lt;style&gt; element.</summary>
    public static Style Style() => new();

    /// <summary>Creates a &lt;style&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Style Style(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;style&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Style Style(string text) => new(text);

    /// <summary>Creates a &lt;br&gt; element.</summary>
    public static Br Br() => new();

    /// <summary>Creates a &lt;hr&gt; element.</summary>
    public static Hr Hr() => new();

    /// <summary>Creates an empty &lt;dl&gt; element.</summary>
    public static Dl Dl() => new();

    /// <summary>Creates a &lt;dl&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Dl Dl(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;dl&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Dl Dl(string text) => new(text);

    /// <summary>Creates an empty &lt;dt&gt; element.</summary>
    public static Dt Dt() => new();

    /// <summary>Creates a &lt;dt&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Dt Dt(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;dt&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Dt Dt(string text) => new(text);

    /// <summary>Creates an empty &lt;dd&gt; element.</summary>
    public static Dd Dd() => new();

    /// <summary>Creates a &lt;dd&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Dd Dd(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;dd&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Dd Dd(string text) => new(text);

    /// <summary>Creates an empty &lt;details&gt; element.</summary>
    public static Details Details() => new();

    /// <summary>Creates a &lt;details&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Details Details(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;details&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Details Details(string text) => new(text);

    /// <summary>Creates an empty &lt;summary&gt; element.</summary>
    public static Summary Summary() => new();

    /// <summary>Creates a &lt;summary&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Summary Summary(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;summary&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Summary Summary(string text) => new(text);

    /// <summary>Creates an empty &lt;table&gt; element.</summary>
    public static Table Table() => new();

    /// <summary>Creates a &lt;table&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Table Table(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;table&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Table Table(string text) => new(text);

    /// <summary>Creates an empty &lt;caption&gt; element.</summary>
    public static Caption Caption() => new();

    /// <summary>Creates a &lt;caption&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Caption Caption(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;caption&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Caption Caption(string text) => new(text);

    /// <summary>Creates an empty &lt;thead&gt; element.</summary>
    public static Thead Thead() => new();

    /// <summary>Creates a &lt;thead&gt; element with the given content.</summary>
    /// <param name="content">The rows inside it.</param>
    public static Thead Thead(params Tr[] content) => new(content);

    /// <summary>Creates an empty &lt;tbody&gt; element.</summary>
    public static Tbody Tbody() => new();

    /// <summary>Creates a &lt;tbody&gt; element with the given content.</summary>
    /// <param name="content">The rows inside it.</param>
    public static Tbody Tbody(params Tr[] content) => new(content);

    /// <summary>Creates an empty &lt;tfoot&gt; element.</summary>
    public static Tfoot Tfoot() => new();

    /// <summary>Creates a &lt;tfoot&gt; element with the given content.</summary>
    /// <param name="content">The rows inside it.</param>
    public static Tfoot Tfoot(params Tr[] content) => new(content);

    /// <summary>Creates an empty &lt;tr&gt; element.</summary>
    public static Tr Tr() => new();

    /// <summary>Creates a &lt;tr&gt; element with the given content.</summary>
    /// <param name="content">The cells inside it.</param>
    public static Tr Tr(params ITableCell[] content) => new(content);

    /// <summary>Creates an empty &lt;th&gt; element.</summary>
    public static Th Th() => new();

    /// <summary>Creates a &lt;th&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Th Th(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;th&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Th Th(string text) => new(text);

    /// <summary>Creates an empty &lt;td&gt; element.</summary>
    public static Td Td() => new();

    /// <summary>Creates a &lt;td&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Td Td(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;td&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Td Td(string text) => new(text);

    /// <summary>Creates an empty &lt;form&gt; element.</summary>
    public static Form Form() => new();

    /// <summary>Creates a &lt;form&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Form Form(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;form&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Form Form(string text) => new(text);

    /// <summary>Creates an empty &lt;label&gt; element.</summary>
    public static Label Label() => new();

    /// <summary>Creates a &lt;label&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Label Label(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;label&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Label Label(string text) => new(text);

    /// <summary>Creates an empty &lt;button&gt; element.</summary>
    public static Button Button() => new();

    /// <summary>Creates a &lt;button&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Button Button(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;button&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Button Button(string text) => new(text);

    /// <summary>Creates an empty &lt;select&gt; element.</summary>
    public static Select Select() => new();

    /// <summary>Creates a &lt;select&gt; element with the given content.</summary>
    /// <param name="content">The options inside it.</param>
    public static Select Select(params Option[] content) => new(content);

    /// <summary>Creates an empty &lt;option&gt; element.</summary>
    public static Option Option() => new();

    /// <summary>Creates a &lt;option&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Option Option(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;option&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Option Option(string text) => new(text);

    /// <summary>Creates an empty &lt;textarea&gt; element.</summary>
    public static Textarea Textarea() => new();

    /// <summary>Creates a &lt;textarea&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Textarea Textarea(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;textarea&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Textarea Textarea(string text) => new(text);

    /// <summary>Creates an empty &lt;picture&gt; element.</summary>
    public static Picture Picture() => new();

    /// <summary>Creates a &lt;picture&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Picture Picture(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;picture&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Picture Picture(string text) => new(text);

    /// <summary>Creates a &lt;source&gt; element.</summary>
    public static Source Source() => new();

    /// <summary>Creates an empty &lt;video&gt; element.</summary>
    public static Video Video() => new();

    /// <summary>Creates a &lt;video&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Video Video(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;video&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Video Video(string text) => new(text);

    /// <summary>Creates an empty &lt;audio&gt; element.</summary>
    public static Audio Audio() => new();

    /// <summary>Creates a &lt;audio&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Audio Audio(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;audio&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Audio Audio(string text) => new(text);

    /// <summary>Creates an empty &lt;iframe&gt; element.</summary>
    public static Iframe Iframe() => new();

    /// <summary>Creates a &lt;iframe&gt; element with the given content.</summary>
    /// <param name="content">The elements or components inside it.</param>
    public static Iframe Iframe(params IHtmlComponent[] content) => new(content);

    /// <summary>Creates a &lt;iframe&gt; element with the given text.</summary>
    /// <param name="text">The text inside it.</param>
    public static Iframe Iframe(string text) => new(text);
}
