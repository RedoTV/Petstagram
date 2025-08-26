using System.Text.Json.Serialization;

namespace Petsgram.Application.DTOs;

public class ExchangeRateResponse
{
    [JsonPropertyName("result")]
    public string Result { get; set; } = string.Empty;
        
    [JsonPropertyName("base_code")]
    public string BaseCode { get; set; } = string.Empty;
        
    [JsonPropertyName("conversion_rates")]
    public Dictionary<string, decimal> ConversionRates { get; set; } = new();
}