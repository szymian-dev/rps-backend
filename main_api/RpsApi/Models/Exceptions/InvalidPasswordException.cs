using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidPasswordException : Exception
{
    public InvalidPasswordException()
    {
    }

    protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidPasswordException(string? message) : base(message)
    {
    }

    public InvalidPasswordException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}