using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class UserAlreadyExistsException: Exception
{
    public UserAlreadyExistsException()
    {
    }

    protected UserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UserAlreadyExistsException(string? message) : base(message)
    {
    }

    public UserAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}