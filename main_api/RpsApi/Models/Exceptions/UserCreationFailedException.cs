using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class UserCreationFailedException : Exception
{
    public UserCreationFailedException()
    {
    }

    protected UserCreationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UserCreationFailedException(string? message) : base(message)
    {
    }

    public UserCreationFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}