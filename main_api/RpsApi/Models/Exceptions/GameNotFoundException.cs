using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class GameNotFoundException : Exception
{
    public GameNotFoundException()
    {
    }

    protected GameNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public GameNotFoundException(string? message) : base(message)
    {
    }

    public GameNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}