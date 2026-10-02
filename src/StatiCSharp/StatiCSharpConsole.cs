using System.Diagnostics;
using System.Text;

namespace StatiCSharp;

internal static class StatiCSharpConsole
{
    /// <summary>
    /// Writes a text to the console, just like <see cref="Console.WriteLine()"/>. If no console is available, the text is written to the debug console.
    /// </summary>
    /// <param name="text"></param>
    public static void WriteLine(string text)
    {
        if (Environment.UserInteractive)
        {
            Console.WriteLine(text);
            return;
        }
        Debug.WriteLine(text);
    }
}
