using System.Runtime.Serialization;

namespace RpsApi.Models.Exceptions;

public class SettingsNotFoundException : Exception
{
    public SettingsNotFoundException()
    {
    }

    protected SettingsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public SettingsNotFoundException(string? message) : base(message)
    {
    }

    public SettingsNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}