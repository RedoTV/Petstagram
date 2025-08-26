namespace Petsgram.Domain.Exceptions.ExchangeRate;

public class UnknownCurrencyException(string currency) : 
    Exception($"Unsupported or unknown currency: '{currency}'.");