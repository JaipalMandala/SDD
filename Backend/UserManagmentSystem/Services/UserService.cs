using System.Security.Cryptography;
using System.Text;
using UserManagmentSystem.Data;
using UserManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using UserManagmentSystem.Utilities;

namespace UserManagmentSystem.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        GenerateHashPassword hashPassword = new GenerateHashPassword();

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Authenticate(string username, string password)
        {
           var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            var users = await _context.Users
    .Include(x => x.UserRoles)
        .ThenInclude(x => x.Role)
    .AsNoTracking()
    .ToListAsync();

            user = users.SingleOrDefault(u => u.Username == username);

            if (user == null || !hashPassword.IsValidPassword(user.Password, password))
                return null;

            return user;
        }

        public async Task<User> CreateUserAsync(AddUser user, string password)
        {
            var item = new User();
            if (await _context.Users.AnyAsync(x => x.Username == user.Username))
                throw new Exception("Username \"" + user.Username + "\" is already taken");
            else
            {
                item = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = hashPassword.CreateHashPassword(password),
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedBy = user.CreatedBy,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedBy = user.UpdatedBy,
                    UpdatedDate = DateTime.UtcNow,
                    IsActive = true

                };

                await _context.Users.AddAsync(item);
                await _context.SaveChangesAsync();


                foreach (var roleId in user.RoleIds)
                {
                    var userRole = new UserRole { UserId = item.Id, RoleId = roleId };
                    _context.UserRoles.Add(userRole);
                }

                await _context.SaveChangesAsync();
            }

            return item;
        }

        public async Task<User> UpdateUserAsync(AddUser user)
        {
            var item = new User
            {
                Username = user.Username,
                Password = HashPassword(user.Password),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            foreach (var roleId in user.RoleIds)
            {
                var userRole = new UserRole { UserId = item.Id, RoleId = roleId };
                _context.UserRoles.Add(userRole);
            }
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByNameAsync(string UserName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == UserName);
            return user;
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedPasswordBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedPasswordBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPasswordHash(string hashedPassword, string Password)
        {
            return HashPassword(Password) == hashedPassword;
        }
    }
}
