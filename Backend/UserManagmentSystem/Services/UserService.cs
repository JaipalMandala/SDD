using System.Security.Cryptography;
using System.Text;
using UserManagmentSystem.Data;
using UserManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;
using UserManagmentSystem.Utilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<List<User>> GetAllUsersAsync()
        {
            var usersList = await _context.Users.OrderByDescending(date => date.UpdatedDate)
                            .Include(x => x.UserRoles)
                            .ThenInclude(x => x.Role)
                            .AsNoTracking()
                            .ToListAsync();
            return usersList;
        }

        public async Task<User> GetUserByIdAsync(int Id)
        {
            var user = await _context.Users
                                     .AsNoTracking()
                                     .Include(x => x.UserRoles)
                                     .ThenInclude(x => x.Role)
                                     .SingleOrDefaultAsync(x => x.Id == Id);

            return user;
        }

        public async Task<User> GetUserByNameAsync(string UserName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == UserName);
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

                foreach (var roleId in user.RoleIds) // mapping roles and inserting in to database
                {
                    var userRole = new UserRole { UserId = item.Id, RoleId = roleId };
                    _context.UserRoles.Add(userRole);
                }

                await _context.SaveChangesAsync();
            }

            return item;
        }

        public async Task<User> UpdateUserAsync(AddUser updateUserObject)
        {
            var user =await _context.Users.SingleOrDefaultAsync(x => x.Username == updateUserObject.Username);

            if (user.Id != updateUserObject.Id)
            {
                throw new InvalidOperationException("User already existed with same User Name");
            }
            if (user == null)
            {
                throw new Exception($"User with ID {updateUserObject.Id} not found.");
            }

            UpdateUserProperties(user, updateUserObject);
            await UpdateUserRolesAsync(user, updateUserObject.RoleIds);
            await _context.SaveChangesAsync();

            return user;
        }

        private void UpdateUserProperties(User user, AddUser updateUserObject)
        {
            user.FirstName = updateUserObject.FirstName;
            user.LastName = updateUserObject.LastName;
            user.Email = updateUserObject.Email;
            user.IsActive = updateUserObject.IsActive;
            user.UpdatedDate = DateTime.UtcNow;
            user.UpdatedBy = updateUserObject.UpdatedBy;
        }

        private async Task UpdateUserRolesAsync(User user, IEnumerable<int> newRoleIds)
        {
            var existingUserRoles = await _context.UserRoles
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            _context.UserRoles.RemoveRange(existingUserRoles);

            var newUserRoles = newRoleIds.Select(roleId => new UserRole { UserId = user.Id, RoleId = roleId });
            await _context.UserRoles.AddRangeAsync(newUserRoles);
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

    }
}
