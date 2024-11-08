using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class InvalidSettingsException : Exception
{
    public InvalidSettingsException()
    {
    }

    protected InvalidSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public InvalidSettingsException(string? message) : base(message)
    {
    }

    public InvalidSettingsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}