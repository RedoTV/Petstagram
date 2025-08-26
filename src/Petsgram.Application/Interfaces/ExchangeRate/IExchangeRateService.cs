namespace Petsgram.Application.Interfaces.ExchangeRate;

/// <summary>
/// Service for getting exchange rates and converting amounts between currencies.
/// </summary>
public interface IExchangeRateService
{
    /// <summary>
    /// Gets the exchange rate from one currency to another (per 1 unit).
    /// </summary>
    Task<decimal> GetRateAsync(string fromCurrency, string toCurrency);
    
    /// <summary>
    /// Converts an amount from one currency to another.
    /// </summary>
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
}