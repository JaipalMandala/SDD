using Microsoft.AspNetCore.Mvc;
using UserManagmentSystem.Data;
using UserManagmentSystem.Models;
using UserManagmentSystem.Services;

namespace UserManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public UsersController(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateUser([FromBody] AddUser request)
        {
                var isUserExist = await _userService.GetUserByNameAsync(request.Username);
                if (isUserExist != null)
                {
                    return StatusCode(409, $"User '{request.Username}' already exists.");
                }
                var response = await _userService.CreateUserAsync(request, request.Password);
                return Ok(new
                {
                    message = "User Created Successfully",
                    data = response,
                    success = true
                });
        }


        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()  {

            var users = _context.Users.ToList();         
            return Ok(users);
        }

        [HttpPut]
        public async Task<ActionResult<AddUser>> UpdateUser(int Id, AddUser user)
        {
            if (Id != user.Id){
                return BadRequest();
            }

            if (user is null){
                return BadRequest();
            }

            var response = await _userService.UpdateUserAsync(user);
            return Ok(new
            {
                message = "User Details Updated Successfully",
                data = response
            });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            if (Id == null)
            {
                return BadRequest();
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