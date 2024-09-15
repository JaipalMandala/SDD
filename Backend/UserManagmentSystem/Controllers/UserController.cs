using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using UserManagmentSystem.Data;
using UserManagmentSystem.Models;
using UserManagmentSystem.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UserManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 //   [Authorize(Roles = "Admin, User")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateUser([FromBody] AddUser request)
        {
            var isUserExist = await _userService.GetUserByNameAsync(request.Username);
            if (isUserExist != null)
            {
                return BadRequest(new
                {
                    message = "User already exists.",
                    success = false
                });
            }
            var response = await _userService.CreateUserAsync(request, request.Password);
            return Ok(new
            {
                message = "User Created Successfully",
                data = response,
                success = true
            });
        }

        [HttpGet("{id}")]
        // [Authorize(Policy = "UserPolicy")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        [HttpGet]
        [Route("AllUsers")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<PagedResult<User>>> GetAll(
                                        [FromQuery] string searchTerm,
                                        [FromQuery] string sortBy = "UpdatedDate",
                                        [FromQuery] string sortOrder = "desc",
                                        [FromQuery] int page = 1,
                                        [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllUsersAsync();

            var query = users.AsQueryable();

            // Apply search
            if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length > 2)
            {
                query = query.Where(i =>
                                (i.FirstName != null && i.FirstName.ToLower().Contains(searchTerm)) ||
                                (i.LastName != null && i.LastName.ToLower().Contains(searchTerm)) ||
                                (i.Username != null && i.Username.ToLower().Contains(searchTerm)) ||
                                (i.Email != null && i.Email.ToLower().Contains(searchTerm))
    );
            }

            // Apply sorting
            switch (sortBy.ToLower())
            {
                case "firstName":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(i => i.FirstName) : query.OrderBy(i => i.FirstName);
                    break;
                case "lastName":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(i => i.LastName) : query.OrderBy(i => i.LastName);
                    break;
                case "userName":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(i => i.Username) : query.OrderBy(i => i.Username);
                    break;
                case "email":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(i => i.Email) : query.OrderBy(i => i.Email);
                    break;
                case "id":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(i => i.Id) : query.OrderBy(i => i.Id);
                    break;
                default:
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(i => i.UpdatedDate) : query.OrderBy(i => i.UpdatedDate);
                    break;
            }

            // Apply pagination
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<User>
            {
                Items = items,
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        [HttpPut]
        [Route("UpdateUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AddUser>> UpdateUser(int Id, AddUser user)
        {
            if (Id != user.Id)
            {
                return BadRequest(new
                {
                    message = "Some thing went wrong, Please check the user details",
                    success = false
                });
            }

            if (user is null)
            {
                return BadRequest(new
                {
                    message = "Some thing went wrong, Please check the user details",
                    success = false
                });
            }

            var response = await _userService.UpdateUserAsync(user);
            return Ok(new
            {
                message = "User Details Updated Successfully",
                data = response,
                success = true
            });
        }

        [HttpDelete]
        [Route("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            if (Id == null || Id == 0)
            {
                return BadRequest(new
                {
                    message = "User not found",
                    success = false
                });
            }
            await _userService.DeleteUserByIdAsync(Id);
            return Ok(new
            {
                message = "User Deleted Successfully",
                status = true
            });
        }


    }
}