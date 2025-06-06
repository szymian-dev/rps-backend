﻿using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class DatabaseException : Exception
{
    public DatabaseException()
    {
    }

    protected DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public DatabaseException(string? message) : base(message)
    {
    }

    public DatabaseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}