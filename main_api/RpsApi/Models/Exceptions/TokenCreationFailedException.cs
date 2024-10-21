namespace RpsApi.Models.Exceptions;

public class TokenCreationFailedException : Exception
{
    public TokenCreationFailedException(string message) : base(message)
    {
    }
    
    public TokenCreationFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public TokenCreationFailedException()
    {
    }
}