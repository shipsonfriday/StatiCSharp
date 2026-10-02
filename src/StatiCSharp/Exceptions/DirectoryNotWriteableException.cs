namespace StatiCSharp.Exceptions;

/// <summary>
/// Thrown when a directory StatiC# needs exists but cannot be written to.
/// <para>
/// The reason is in <see cref="Exception.InnerException"/>, which carries the exception
/// the file system raised.
/// </para>
/// </summary>
public class DirectoryNotWriteableException : Exception
{
    /// <summary>
    /// Initializes a new instance without a message.
    /// </summary>
    public DirectoryNotWriteableException()
    {
    }

    /// <summary>
    /// Initializes a new instance with a message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public DirectoryNotWriteableException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a message and the exception that caused it.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The exception the file system raised.</param>
    public DirectoryNotWriteableException(string message, Exception inner) : base(message, inner)
    {
    }
}
