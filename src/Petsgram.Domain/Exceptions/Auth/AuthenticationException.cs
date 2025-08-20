namespace Petsgram.Domain.Exceptions.Auth;

public class AuthenticationException(string message) : 
    Exception(message);