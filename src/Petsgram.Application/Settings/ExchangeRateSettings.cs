namespace Petsgram.Application.Settings;

public class ExchangeRateSettings
{
    public const string SectionName = "ExchangeRateApi";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string DefaultBaseCurrency { get; set; } = string.Empty;
    public int CacheSeconds { get; set; } = 600;
}