using Catalog.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var token = _userService.Authenticate(request.Username, request.Password);
            if (token == null)
                return Unauthorized(new { Message = "Invalid credentials." });

            return Ok(new { Token = token });
        }



        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (request.Role != "Student" && request.Role != "Teacher")
            {
                return BadRequest(new { Message = "Rolul trebuie să fie 'Student' sau 'Teacher'." });
            }

            User user = request.Role == "Student"
                ? new Student
                {
                    Name = request.Username,
                    EmailAddress = request.Email,
                    Password = request.Password,
                    Role = "Student"
                }
                : new Teacher
                {
                    Name = request.Username,
                    EmailAddress = request.Email,
                    Password = request.Password,
                    Role = "Teacher"
                };

            var success = _userService.RegisterUser(user);

            if (!success)
            {
                return BadRequest(new { Message = "Există deja un user cu acest nume sau email." });
            }

            return Ok(new { Message = "Utilizator înregistrat cu succes." });
        }

    }
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "User";
    }

}
