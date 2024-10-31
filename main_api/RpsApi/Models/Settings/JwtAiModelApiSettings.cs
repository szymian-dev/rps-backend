using RpsApi.Models.Interfaces.ISettings;

namespace RpsApi.Models.Settings;

public class JwtAiModelApiSettings : IJwtBaseSettings
{
    public string Key { get; set; } = String.Empty;
    public string Issuer { get; set; } = String.Empty;
    public int ExpireMinutes { get; set; }
}