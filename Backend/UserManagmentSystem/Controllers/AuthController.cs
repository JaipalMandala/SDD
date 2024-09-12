using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagmentSystem.Models;
using UserManagmentSystem.Services;

namespace UserManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthController(IUserService userService, IConfiguration configuration, IAuthService authService)
        {
            _userService = userService;
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LogIn request)
        {
            var user = await _authService.Authenticate(request.Username, request.Password);
            if (user == null) return BadRequest(new
            {
                message = "Invalid User name or Password",
                success = false,
            });

            // generating JWT token
            var token = GenerateToken(user);

            return Ok(new
            {
                Token = token,
                User = user,
                success = true,
            });
        }

        // To generate token
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
              //  new Claim(ClaimTypes.Role,user.UserRoles),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }

}
