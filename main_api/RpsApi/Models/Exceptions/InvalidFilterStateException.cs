using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidFilterStateException : Exception
{
    public InvalidFilterStateException()
    {
    }

    protected InvalidFilterStateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidFilterStateException(string? message) : base(message)
    {
    }

    public InvalidFilterStateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}