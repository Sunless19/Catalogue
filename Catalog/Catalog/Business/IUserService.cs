using Catalog.Models;

public interface IUserService
{
    string? Authenticate(string username, string password);
    bool RegisterUser(User user);
    bool ResetPassword(string encodedId, string newPassword);
    bool SendPasswordResetEmail(string email);
}