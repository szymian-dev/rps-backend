using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class JwtTokenAccessException : Exception
{
    public JwtTokenAccessException()
    {
    }

    protected JwtTokenAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public JwtTokenAccessException(string? message) : base(message)
    {
    }

    public JwtTokenAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}