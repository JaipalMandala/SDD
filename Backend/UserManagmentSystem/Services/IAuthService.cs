using UserManagmentSystem.Models;

namespace UserManagmentSystem.Services
{
    public interface IAuthService
    {
        Task<User> Authenticate(string username, string password);
    }
}
