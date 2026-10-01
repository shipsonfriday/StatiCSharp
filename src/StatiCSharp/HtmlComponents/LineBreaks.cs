using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;br&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Br : HtmlElement<Br>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "br";

        /// <inheritdoc/>
        protected override bool VoidElement => true;

        /// <summary>Initiate a new br element.</summary>
        public Br() {{ }}
    }

    /// <summary>
    /// A representation of a &lt;hr&gt; element.
    /// <para>Call the Render() method to turn it into an HTML string.</para>
    /// </summary>
    public class Hr : HtmlElement<Hr>, IHtmlComponent
    {
        /// <inheritdoc/>
        protected override string TagName => "hr";

        /// <inheritdoc/>
        protected override bool VoidElement => true;

        /// <summary>Initiate a new hr element.</summary>
        public Hr() {{ }}
    }
}
