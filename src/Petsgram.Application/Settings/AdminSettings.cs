namespace Petsgram.Application.Settings;

public class AdminSettings
{
    public const string SectionName = "Admin";
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}