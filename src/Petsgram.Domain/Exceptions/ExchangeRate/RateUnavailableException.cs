namespace Petsgram.Domain.Exceptions.ExchangeRate;

public class RateUnavailableException(string message) : 
    Exception(message);