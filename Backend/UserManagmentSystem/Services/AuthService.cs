using System.Security.Cryptography;
using System.Text;
using UserManagmentSystem.Data;
using UserManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using UserManagmentSystem.Utilities;

namespace UserManagmentSystem.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        GenerateHashPassword generateHashPassword = new GenerateHashPassword();


        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = new User();
            var users = await _context.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).AsNoTracking().ToListAsync();
            user = users.SingleOrDefault(u => u.Username == username);
            
            if (user == null || !generateHashPassword.IsValidPassword(user.Password, password))
                return null;

            return user;
        }

        public string GenerateHashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();
            }
        }

        private bool IsValidPassword(string hashedPassword, string Password)
        {
            return GenerateHashPassword(Password) == hashedPassword;
        }
    }
}
