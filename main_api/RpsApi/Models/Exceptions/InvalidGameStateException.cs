using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidGameStateException : Exception
{
    public InvalidGameStateException()
    {
    }

    protected InvalidGameStateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidGameStateException(string? message) : base(message)
    {
    }

    public InvalidGameStateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}