using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class AiModelApiException : Exception
{
    public AiModelApiException()
    {
    }

    protected AiModelApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AiModelApiException(string? message) : base(message)
    {
    }

    public AiModelApiException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}