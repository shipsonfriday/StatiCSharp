using System;
using System.Globalization;
using StatiCSharp.Tools;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// The date has to read the way the culture writes it, in wording and in arrangement.
/// </summary>
public class DateTextTests
{
    private static readonly DateOnly TheFourthOfMarch = new(2020, 3, 4);

    [Theory]
    // The old fixed pattern "MMMM dd, yyyy" gave the month name of the culture but always the
    // English arrangement: "März 04, 2020", "4 mars" written as "mars 04", "3月 04, 2020".
    [InlineData("en-US", "March 4, 2020")]
    [InlineData("en-GB", "4 March 2020")]
    [InlineData("de-DE", "4. März 2020")]
    [InlineData("fr-FR", "4 mars 2020")]
    [InlineData("pl-PL", "4 marca 2020")]
    [InlineData("hu-HU", "2020. március 4.")]
    [InlineData("ja-JP", "2020年3月4日")]
    public void TheDateReadsTheWayTheCultureWritesIt(string language, string expected)
    {
        Assert.Equal(expected, DateText.For(TheFourthOfMarch, new CultureInfo(language)));
    }

    [Fact]
    public void TheDayOfTheWeekIsLeftOut()
    {
        // Wherever the culture puts it: English and German put it first, Japanese and
        // Hungarian last. "D" would include it, and an article list reads better without.
        foreach (string language in new[] { "en-US", "de-DE", "fr-FR", "ja-JP", "hu-HU", "pl-PL" })
        {
            var culture = new CultureInfo(language);
            string written = DateText.For(TheFourthOfMarch, culture);
            string weekday = culture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday);

            Assert.DoesNotContain(weekday, written, StringComparison.CurrentCulture);
        }
    }

    [Fact]
    public void NoSeparatorIsLeftWhereTheWeekdayWas()
    {
        foreach (string language in new[] { "en-US", "de-DE", "fr-FR", "ja-JP", "hu-HU", "ar-EG" })
        {
            string written = DateText.For(TheFourthOfMarch, new CultureInfo(language));

            Assert.Equal(written.Trim(), written);
            Assert.DoesNotMatch("^[,،、]", written);
        }
    }

    [Fact]
    public void TheResultDoesNotDependOnTheMachinesCulture()
    {
        // Rendering used to take the machine's culture whenever a culture was not passed in,
        // so an English site built on a German machine read "März".
        CultureInfo previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            Assert.Equal("March 4, 2020", DateText.For(TheFourthOfMarch, new CultureInfo("en-US")));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void EveryInstalledCultureProducesSomething()
    {
        // The pattern is derived from the culture, so a culture with an unusual long date
        // pattern must not end up with an empty or malformed result.
        foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
        {
            string written = DateText.For(TheFourthOfMarch, culture);

            Assert.False(
                string.IsNullOrWhiteSpace(written),
                $"{culture.Name} produced nothing");
        }
    }

    [Fact]
    public void RejectsAMissingCulture()
    {
        Assert.Throws<ArgumentNullException>(() => DateText.For(TheFourthOfMarch, null!));
    }
}
