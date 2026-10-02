using System.Globalization;

namespace StatiCSharp.Tools;

/// <summary>
/// Writes a date out the way a culture writes it.
/// <para>
/// Public like <see cref="UrlSlug"/>, and for the same reason: every theme renders dates, and
/// the obvious way to do it is wrong. A fixed pattern such as <c>"MMMM dd, yyyy"</c> takes the
/// month name from the culture but keeps an English arrangement, so a German site reads
/// "März 04, 2020" instead of "4. März 2020" and a Polish one reads "marca 04, 2020",
/// where "marca" is the form that only makes sense after the day.
/// </para>
/// </summary>
public static class DateText
{
    /// <summary>
    /// The date in the culture's own wording and arrangement, without the day of the week.
    /// </summary>
    /// <param name="date">The date to write.</param>
    /// <param name="culture">The culture to write it for, usually the website's language.</param>
    /// <returns>The date, e.g. "March 4, 2020" for en-US and "4. März 2020" for de-DE.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="culture"/> is null.</exception>
    public static string For(DateOnly date, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        return date.ToString(PatternOf(culture), culture);
    }

    /// <summary>
    /// The culture's long date pattern with the day of the week taken out.
    /// <para>
    /// There is no standard pattern for a long date without the weekday - <c>"D"</c> includes
    /// it - and an article list reads better without "Wednesday,". Removing the token works
    /// wherever it sits: English and German put it first, Japanese and Hungarian last.
    /// </para>
    /// </summary>
    private static string PatternOf(CultureInfo culture)
    {
        string pattern = culture.DateTimeFormat.LongDatePattern
            .Replace("dddd", string.Empty, StringComparison.Ordinal);

        // Whatever is left of the separator the weekday sat next to, including the Arabic and
        // the ideographic comma. Dots are kept: the Hungarian pattern ends in one that belongs
        // to the day, as in "2020. március 4.".
        return pattern.Trim(' ', ',', '\u060C', '\u3001');
    }
}
