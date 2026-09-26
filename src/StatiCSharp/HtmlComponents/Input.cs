using StatiCSharp.Interfaces;

namespace StatiCSharp.HtmlComponents
{
    /// <summary>
    /// A representation of a &lt;input /&gt; element.
    /// Call the Render() method to turn it into an HTML string.
    /// </summary>
    public class Input : HtmlElement, IHtmlComponent
    {
        private protected override string TagName
        {
            get { return "input"; }
        }

        private protected override bool VoidElement
        {
            get { return true; }
        }

        /// <summary>
        /// Initiate a new empty input element.
        /// </summary>
        public Input()
        {
            // No action needed, because the base class already initialized an empty List<IHtmlComponent>.
            // But because this is a void element the content is ignored through the rendering process.
        }

        /// <summary>
        /// The type attribute specifies the type of &lt;input&gt; element to display.<br/>
        /// If the type attribute is not specified, the default type is "text".
        /// <para>Check <see href="https://www.w3schools.com/tags/att_input_type.asp">W3Schools</see> for an overview.</para>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Input Type(string t)
        {
            Attributes["type"] = t;
            return this;
        }

        /// <summary>
        /// Hint for expected file type in file upload controls.
        /// </summary>
        /// <param name="accept"></param>
        /// <returns></returns>
        public Input Accept(string accept)
        {
            Attributes["accept"] = accept;
            return this;
        }

        /// <summary>
        /// alt attribute for the image type. Required for accessibility.
        /// </summary>
        /// <param name="alt"></param>
        /// <returns></returns>
        public Input Alt(string alt)
        {
            Attributes["alt"] = alt;
            return this;
        }

        /// <summary>
        /// Whether the command or control is checked.
        /// </summary>
        /// <returns></returns>
        public Input Checked()
        {
            Attributes["checked"] = null;
            return this;
        }

        /// <summary>
        /// Whether the form control is disabled
        /// </summary>
        /// <returns></returns>
        public Input Disabled()
        {
            Attributes["disabled"] = null;
            return this;
        }

        /// <summary>
        /// Associates the control with a form element.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public Input Form(string form)
        {
            Attributes["form"] = form;
            return this;
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public Input Max(float max)
        {
            Attributes["max"] = max.ToString();
            return this;
        }

        /// <summary>
        /// Maximum length (number of characters) of value.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public Input MaxLength(int max)
        {
            Attributes["maxlength"] = max.ToString();
            return this;
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        public Input Min(float min)
        {
            Attributes["min"] = min.ToString();
            return this;
        }

        /// <summary>
        /// Minimum length (number of characters) of value.
        /// </summary>
        /// <param name="min"></param>
        /// <returns></returns>
        public Input MinLength(int min)
        {
            Attributes["minlength"] = min.ToString();
            return this;
        }

        /// <summary>
        /// Name of the form control. Submitted with the form as part of a name/value pair
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Input Name(string name)
        {
            Attributes["name"] = name;
            return this;
        }

        /// <summary>
        /// Text that appears in the form control when it has no value set.
        /// </summary>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public Input Placeholder(string placeholder)
        {
            Attributes["placeholder"] = placeholder;
            return this;
        }

        /// <summary>
        /// The value is not editable.
        /// </summary>
        /// <returns></returns>
        public Input ReadOnly()
        {
            Attributes["readonly"] = null;
            return this;
        }

        /// <summary>
        /// A value is required or must be check for the form to be submittable.
        /// </summary>
        /// <returns></returns>
        public Input Required()
        {
            Attributes["required"] = null;
            return this;
        }

        /// <summary>
        /// Specifies how much of the input is shown. Basically creates same result as setting CSS width property with a few specialities. The actual unit of the value depends on the input type.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Input Size(int size)
        {
            Attributes["size"] = size.ToString();
            return this;
        }

        /// <summary>
        /// The initial value of the control.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Input Value(string value)
        {
            Attributes["value"] = value;
            return this;
        }

        /// <summary>
        /// A wildcard to set custom attributes if the needed attribute is not supported yet.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Input Attribute(string key, string? value)
        {
            Attributes[key] = value;
            return this;
        }

        /// <summary>
        /// A wildcard to set custom attributes if the needed attribute is not supported yet.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Input Attribute(string key, int value)
        {
            Attributes[key] = value.ToString();
            return this;
        }
    }
}
