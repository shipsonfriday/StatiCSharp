using System;

namespace StatiCSharp.Exceptions;

internal class DirectoryNotWriteableException : Exception
{
    public DirectoryNotWriteableException()
    {

    }

    public DirectoryNotWriteableException(string message) : base(message)
    {

    }

    public DirectoryNotWriteableException(string message, Exception inner) : base(message, inner)
    {

    }
}
