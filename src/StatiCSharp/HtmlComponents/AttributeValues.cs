namespace StatiCSharp.HtmlComponents;

/// <summary>
/// The value of an <c>input</c> element's <c>type</c> attribute.
/// </summary>
public enum InputType
{
    /// <summary>A push button with no default behaviour.</summary>
    Button,
    /// <summary>A check box.</summary>
    Checkbox,
    /// <summary>A colour picker.</summary>
    Color,
    /// <summary>A date, without a time.</summary>
    Date,
    /// <summary>A date and a time, without a time zone. Renders as <c>datetime-local</c>.</summary>
    DateTimeLocal,
    /// <summary>An email address.</summary>
    Email,
    /// <summary>A file picker.</summary>
    File,
    /// <summary>A value that is submitted but not shown.</summary>
    Hidden,
    /// <summary>A graphical submit button.</summary>
    Image,
    /// <summary>A month and a year.</summary>
    Month,
    /// <summary>A number.</summary>
    Number,
    /// <summary>A password, obscured while typing.</summary>
    Password,
    /// <summary>One of a set of radio buttons.</summary>
    Radio,
    /// <summary>A number picked from a range, shown as a slider.</summary>
    Range,
    /// <summary>A button that resets the form.</summary>
    Reset,
    /// <summary>A search field.</summary>
    Search,
    /// <summary>A button that submits the form.</summary>
    Submit,
    /// <summary>A telephone number.</summary>
    Tel,
    /// <summary>A single line of text. The default when no type is given.</summary>
    Text,
    /// <summary>A time, without a date.</summary>
    Time,
    /// <summary>A url.</summary>
    Url,
    /// <summary>A week and a year.</summary>
    Week,
}

/// <summary>
/// Where a link opens. These are the four keywords; a link may also name a browsing context of
/// your own, which is what the <c>string</c> overload is for.
/// </summary>
public enum LinkTarget
{
    /// <summary>In the same context. Renders as <c>_self</c>.</summary>
    Self,
    /// <summary>In a new one. Renders as <c>_blank</c>.</summary>
    Blank,
    /// <summary>In the parent context. Renders as <c>_parent</c>.</summary>
    Parent,
    /// <summary>In the topmost context. Renders as <c>_top</c>.</summary>
    Top,
}

/// <summary>
/// What a <c>link</c> element relates to. A <c>rel</c> attribute may hold several of these
/// separated by spaces, which is what the <c>string</c> overload is for.
/// </summary>
public enum LinkRelation
{
    /// <summary>An alternate version of the document.</summary>
    Alternate,
    /// <summary>The author of the document.</summary>
    Author,
    /// <summary>The preferred url of the document.</summary>
    Canonical,
    /// <summary>A host to resolve ahead of time. Renders as <c>dns-prefetch</c>.</summary>
    DnsPrefetch,
    /// <summary>Help for the document.</summary>
    Help,
    /// <summary>An icon for the document.</summary>
    Icon,
    /// <summary>The licence of the document.</summary>
    License,
    /// <summary>A web app manifest.</summary>
    Manifest,
    /// <summary>A module to fetch and compile ahead of time.</summary>
    ModulePreload,
    /// <summary>The next document in a series.</summary>
    Next,
    /// <summary>Where to send a pingback.</summary>
    Pingback,
    /// <summary>An origin to connect to ahead of time.</summary>
    Preconnect,
    /// <summary>A resource likely needed next.</summary>
    Prefetch,
    /// <summary>A resource needed for this document.</summary>
    Preload,
    /// <summary>The previous document in a series.</summary>
    Prev,
    /// <summary>A search interface for the document.</summary>
    Search,
    /// <summary>A style sheet.</summary>
    Stylesheet,
}

/// <summary>
/// How a form submits.
/// </summary>
public enum FormMethod
{
    /// <summary>The values go into the url.</summary>
    Get,
    /// <summary>The values go into the request body.</summary>
    Post,
    /// <summary>Closes the dialog the form is in, without submitting.</summary>
    Dialog,
}

/// <summary>
/// What a <c>button</c> does.
/// </summary>
public enum ButtonType
{
    /// <summary>Submits the form. The default when no type is given.</summary>
    Submit,
    /// <summary>Resets the form.</summary>
    Reset,
    /// <summary>Nothing by itself.</summary>
    Button,
}

/// <summary>
/// What a table header cell is the header for.
/// </summary>
public enum CellScope
{
    /// <summary>The rest of the row.</summary>
    Row,
    /// <summary>The rest of the column.</summary>
    Col,
    /// <summary>The remaining rows of its row group.</summary>
    RowGroup,
    /// <summary>The remaining columns of its column group.</summary>
    ColGroup,
}

/// <summary>
/// Turns a typed attribute value into the text the html standard uses for it.
/// <para>
/// Written out rather than derived from the member name, because the two differ wherever the
/// standard uses a hyphen - <c>datetime-local</c>, <c>dns-prefetch</c> - and because the list
/// then says exactly what is written. Every switch ends in an arm that throws, and a test walks
/// every member of every enum, so a member added without a value here cannot stay unnoticed.
/// </para>
/// </summary>
internal static class AttributeValue
{
    internal static string Of(InputType type) => type switch
    {
        InputType.Button => "button",
        InputType.Checkbox => "checkbox",
        InputType.Color => "color",
        InputType.Date => "date",
        InputType.DateTimeLocal => "datetime-local",
        InputType.Email => "email",
        InputType.File => "file",
        InputType.Hidden => "hidden",
        InputType.Image => "image",
        InputType.Month => "month",
        InputType.Number => "number",
        InputType.Password => "password",
        InputType.Radio => "radio",
        InputType.Range => "range",
        InputType.Reset => "reset",
        InputType.Search => "search",
        InputType.Submit => "submit",
        InputType.Tel => "tel",
        InputType.Text => "text",
        InputType.Time => "time",
        InputType.Url => "url",
        InputType.Week => "week",
        _ => throw Unknown(type),
    };

    internal static string Of(LinkTarget target) => target switch
    {
        LinkTarget.Self => "_self",
        LinkTarget.Blank => "_blank",
        LinkTarget.Parent => "_parent",
        LinkTarget.Top => "_top",
        _ => throw Unknown(target),
    };

    internal static string Of(LinkRelation relation) => relation switch
    {
        LinkRelation.Alternate => "alternate",
        LinkRelation.Author => "author",
        LinkRelation.Canonical => "canonical",
        LinkRelation.DnsPrefetch => "dns-prefetch",
        LinkRelation.Help => "help",
        LinkRelation.Icon => "icon",
        LinkRelation.License => "license",
        LinkRelation.Manifest => "manifest",
        LinkRelation.ModulePreload => "modulepreload",
        LinkRelation.Next => "next",
        LinkRelation.Pingback => "pingback",
        LinkRelation.Preconnect => "preconnect",
        LinkRelation.Prefetch => "prefetch",
        LinkRelation.Preload => "preload",
        LinkRelation.Prev => "prev",
        LinkRelation.Search => "search",
        LinkRelation.Stylesheet => "stylesheet",
        _ => throw Unknown(relation),
    };

    internal static string Of(FormMethod method) => method switch
    {
        FormMethod.Get => "get",
        FormMethod.Post => "post",
        FormMethod.Dialog => "dialog",
        _ => throw Unknown(method),
    };

    internal static string Of(ButtonType type) => type switch
    {
        ButtonType.Submit => "submit",
        ButtonType.Reset => "reset",
        ButtonType.Button => "button",
        _ => throw Unknown(type),
    };

    internal static string Of(CellScope scope) => scope switch
    {
        CellScope.Row => "row",
        CellScope.Col => "col",
        CellScope.RowGroup => "rowgroup",
        CellScope.ColGroup => "colgroup",
        _ => throw Unknown(scope),
    };

    private static ArgumentOutOfRangeException Unknown<T>(T value)
        where T : struct, Enum
        => new(nameof(value), value, $"{typeof(T).Name} has no html value for {value}.");
}
