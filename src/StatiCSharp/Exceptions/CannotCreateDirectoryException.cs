using System;

namespace StatiCSharp.Exceptions;

internal class CannotCreateDirectoryException : Exception
{
    public CannotCreateDirectoryException()
    {

    }
    
    public CannotCreateDirectoryException(string message) : base(message)
    {

    }

    public CannotCreateDirectoryException(string message, Exception inner) : base(message, inner)
    {

    }
}
