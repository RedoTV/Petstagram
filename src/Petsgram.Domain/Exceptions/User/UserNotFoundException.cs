namespace Petsgram.Domain.Exceptions.User;

public class UserNotFoundException(int id) : 
    Exception($"User with id {id} not found");