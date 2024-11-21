using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException()
    {
    }

    protected UnprocessableEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UnprocessableEntityException(string? message) : base(message)
    {
    }

    public UnprocessableEntityException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}