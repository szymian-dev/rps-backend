namespace RpsApi.Models.Settings;

public class CookieSettings
{
    public string SameSite { get; set; } = "None";
    public bool Secure { get; set; } = true;
    public bool HttpOnly { get; set; } = true;
}