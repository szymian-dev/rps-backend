using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidGameException : Exception
{
    public InvalidGameException()
    {
    }

    protected InvalidGameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidGameException(string? message) : base(message)
    {
    }

    public InvalidGameException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}