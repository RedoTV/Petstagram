namespace Petsgram.Domain.Exceptions.User;

public class UserAlreadyExistsException(string userName) : 
    Exception($"User with username '{userName}' already exists");