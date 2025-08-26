namespace Petsgram.Domain.Exceptions.User;

public class UserValidationException(string message) : 
    Exception(message);