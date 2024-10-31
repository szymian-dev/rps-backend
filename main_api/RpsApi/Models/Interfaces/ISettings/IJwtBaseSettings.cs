namespace RpsApi.Models.Interfaces.ISettings;

public interface IJwtBaseSettings
{
    public string Key { get; set; } 
    public string Issuer { get; set; } 
    public int ExpireMinutes { get; set; }
}