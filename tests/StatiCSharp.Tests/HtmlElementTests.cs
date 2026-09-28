using StatiCSharp.HtmlComponents;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Smoke test over the public HTML component API.
/// </summary>
public class HtmlElementTests
{
    [Fact]
    public void Render_WritesTagWithAttributeAndContent()
    {
        string html = new Div("hello").Class("wrapper").Render();

        Assert.Equal("<div class=\"wrapper\">hello</div>", html);
    }
}
