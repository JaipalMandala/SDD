using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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
            if(request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new
                {
                    message = "Invalid Credentails",
                    success = false,
                });
            }
            var user = await _authService.AuthenticateAsync(request.Username, request.Password);
            if (user == null) return Unauthorized(new
            {
                message = "Invalid User name or Password",
                success = false,
            });

           // var listRoles = new List<string>();
           // var roles = user.UserRoles.Select(s => new { s.Role.RoleName }).ToList();
            //  roleNames = string.Join(", ", roles.Select(s => s.RoleName));
             //roleNames.AddRange(roles);
           //    var enumerableList = roles.AsEnumerable();

            var userRoles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

            //   roleName.Add(roles);
            //   foreach (var item in roles)
            //   {
            //      listRoles.Add(item.ToString());
            //   }
            // generating JWT token
            var token = GenerateToken(user, userRoles);

            return Ok(new
            {
                Token = token,
                User = user,
                UserRoles = userRoles,
                success = true,
            });
        }

        // To generate token
        private string GenerateToken(User user, List<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtTokenGeneration");
          //  var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            // Create claims based on user information
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
        };

            // Add roles as claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"] // Ensure this matches the configuration
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
