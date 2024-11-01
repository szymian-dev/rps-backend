using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class GameNotCreatedException : Exception
{
    public GameNotCreatedException()
    {
    }

    protected GameNotCreatedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public GameNotCreatedException(string? message) : base(message)
    {
    }

    public GameNotCreatedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}