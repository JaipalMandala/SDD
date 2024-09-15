using UserManagmentSystem.Models;

namespace UserManagmentSystem.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }
}
