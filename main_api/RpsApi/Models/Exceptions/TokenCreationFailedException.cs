using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class TokenCreationFailedException : Exception
{
    public TokenCreationFailedException()
    {
    }

    protected TokenCreationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public TokenCreationFailedException(string? message) : base(message)
    {
    }

    public TokenCreationFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}