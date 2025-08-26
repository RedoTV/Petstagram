using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Petsgram.Application.DTOs;
using Petsgram.Application.Interfaces.Caching;
using Petsgram.Application.Interfaces.ExchangeRate;
using Petsgram.Application.Settings;
using Petsgram.Domain.Exceptions.ExchangeRate;

namespace Petsgram.Infrastructure.Services.ExchangeRate;

public class ExchangeRateApiService : IExchangeRateService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly ExchangeRateSettings _settings;

    public ExchangeRateApiService(
        HttpClient httpClient, 
        ICacheService cacheService, 
        IOptions<ExchangeRateSettings> settings)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
        _settings = settings.Value;
    }

    public async Task<decimal> GetRateAsync(string fromCurrency, string toCurrency)
    {
        var normalizedFrom = NormalizeCurrencyCode(fromCurrency);
        var normalizedTo = NormalizeCurrencyCode(toCurrency);
        
        if (normalizedFrom == normalizedTo) 
            return 1m;

        var exchangeTable = await GetExchangeRateTableAsync(normalizedFrom);
        if (exchangeTable.ConversionRates.TryGetValue(normalizedTo, out var directRate)) 
            return directRate;

        var baseCurrency = NormalizeCurrencyCode(_settings.DefaultBaseCurrency);
        var baseTable = await GetExchangeRateTableAsync(baseCurrency);

        if (!baseTable.ConversionRates.TryGetValue(normalizedFrom, out var baseToFromRate))
            throw new UnknownCurrencyException(normalizedFrom);
        
        if (!baseTable.ConversionRates.TryGetValue(normalizedTo, out var baseToToRate))
            throw new UnknownCurrencyException(normalizedTo);
        
        if (baseToFromRate == 0m)
            throw new RateUnavailableException("Zero cross rate encountered");

        return baseToToRate / baseToFromRate;
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        var exchangeRate = await GetRateAsync(fromCurrency, toCurrency);
        return amount * exchangeRate;
    }

    private async Task<ExchangeRateResponse> GetExchangeRateTableAsync(
        string baseCurrency, 
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"exchange_rates:{baseCurrency}";
        var cachedTable = await _cacheService.ReadAsync<ExchangeRateResponse>(cacheKey, cancellationToken);
        
        if (cachedTable is not null) 
            return cachedTable;

        var requestUrl = $"{_settings.BaseUrl.TrimEnd('/')}/{_settings.ApiKey}/latest/{baseCurrency}";
        var apiResponse = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(requestUrl, cancellationToken);
        
        if (apiResponse is null)
            throw new RateUnavailableException("Invalid API response received");

        ValidateApiResponse(apiResponse, baseCurrency);
        
        var normalizedResponse = NormalizeConversionRates(apiResponse);
        var cacheExpiration = TimeSpan.FromSeconds(_settings.CacheSeconds);
        
        await _cacheService.WriteAsync(cacheKey, normalizedResponse, cacheExpiration, cancellationToken);
        return normalizedResponse;
    }

    private static void ValidateApiResponse(ExchangeRateResponse response, string expectedBaseCurrency)
    {
        if (!string.Equals(response.Result, "success", StringComparison.OrdinalIgnoreCase))
            throw new RateUnavailableException($"API returned error: {response.Result}");
            
        if (!string.Equals(response.BaseCode, expectedBaseCurrency, StringComparison.OrdinalIgnoreCase))
            throw new RateUnavailableException(
                $"Base currency mismatch: expected '{expectedBaseCurrency}', received '{response.BaseCode}'");
    }

    private static ExchangeRateResponse NormalizeConversionRates(ExchangeRateResponse response)
    {
        var normalizedRates = new Dictionary<string, decimal>(StringComparer.Ordinal);
        
        foreach (var (currencyCode, rate) in response.ConversionRates)
            normalizedRates[NormalizeCurrencyCode(currencyCode)] = rate;
            
        response.ConversionRates = normalizedRates;
        return response;
    }

    private static string NormalizeCurrencyCode(string? currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode)) 
            throw new UnknownCurrencyException(currencyCode ?? "<null>");
            
        return currencyCode.Trim().ToUpperInvariant();
    }
}