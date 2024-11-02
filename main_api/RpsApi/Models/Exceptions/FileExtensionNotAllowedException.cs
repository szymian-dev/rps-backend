using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class FileExtensionNotAllowedException : Exception
{
    public FileExtensionNotAllowedException()
    {
    }

    protected FileExtensionNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public FileExtensionNotAllowedException(string? message) : base(message)
    {
    }

    public FileExtensionNotAllowedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}