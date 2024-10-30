using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class FileSizeExceededException : Exception
{
    public FileSizeExceededException()
    {
    }

    protected FileSizeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public FileSizeExceededException(string? message) : base(message)
    {
    }

    public FileSizeExceededException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}