namespace Petsgram.Application.Settings;

public class RedisSettings
{
    public const string SectionName = "Redis";
    public string ConnectionString { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    
    /// <summary>
    /// Default Expiration in seconds
    /// </summary>
    public int DefaultExpiration { get; set; } = 600;
}