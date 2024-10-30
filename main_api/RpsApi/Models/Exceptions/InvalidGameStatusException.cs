using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidGameStatusException  :Exception
{
    public InvalidGameStatusException()
    {
    }

    protected InvalidGameStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidGameStatusException(string? message) : base(message)
    {
    }

    public InvalidGameStatusException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}