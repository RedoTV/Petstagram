namespace Petsgram.Domain.Exceptions.Auth;

public class TokenValidationException(string message) : 
    Exception(message);