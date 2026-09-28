using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
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
        /// <param name="content">The elements or components inside it.</param>
        public static Ul Ul(params IHtmlComponent[] content) => new(content);

        /// <summary>Creates an unordered list with the given text as its content.</summary>
        /// <param name="text">The text inside it.</param>
        public static Ul Ul(string text) => new(text);

        /// <summary>Creates an empty an ordered list.</summary>
        public static Ol Ol() => new();

        /// <summary>Creates an ordered list with the given content.</summary>
        /// <param name="content">The elements or components inside it.</param>
        public static Ol Ol(params IHtmlComponent[] content) => new(content);

        /// <summary>Creates an ordered list with the given text as its content.</summary>
        /// <param name="text">The text inside it.</param>
        public static Ol Ol(string text) => new(text);

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
    }
}
