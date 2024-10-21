using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException()
    {
    }

    protected InvalidTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidTokenException(string? message) : base(message)
    {
    }

    public InvalidTokenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}