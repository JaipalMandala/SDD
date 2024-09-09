using Microsoft.EntityFrameworkCore;
using UserManagmentSystem.Models;

namespace UserManagmentSystem.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> CreateUserAsync(AddUser user, string password);
        Task<User> UpdateUserAsync(AddUser user);
        Task DeleteUserByIdAsync(int id);
        Task<User> GetUserByNameAsync(string UserName);      

    }
}
