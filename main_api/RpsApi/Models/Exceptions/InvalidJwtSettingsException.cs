using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidJwtSettingsException : Exception
{
    public InvalidJwtSettingsException()
    {
    }

    protected InvalidJwtSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidJwtSettingsException(string? message) : base(message)
    {
    }

    public InvalidJwtSettingsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}