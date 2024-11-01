using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
    {
    }

    protected ForbiddenAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ForbiddenAccessException(string? message) : base(message)
    {
    }

    public ForbiddenAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}