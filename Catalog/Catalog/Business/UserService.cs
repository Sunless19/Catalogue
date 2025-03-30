using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Catalog.Models;
using Catalog.Repositories;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly string _jwtKey;

    public UserService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _jwtKey = configuration["Jwt:Key"];

        if (!_userRepository.GetAll().Any())
        {
            var initialUsers = new List<User>
            {
                new Student
                {
                    Name = "student1",
                    EmailAddress = "student1@example.com",
                    Password = "1234",
                    Role = "Student"
                },
                new Teacher
                {
                    Name = "teacher1",
                    EmailAddress = "teacher1@example.com",
                    Password = "abcd",
                    Role = "Teacher"
                }
            };

            foreach (var user in initialUsers)
            {
                _userRepository.Add(user);
            }

            _userRepository.Save();
        }
    }

    public string? Authenticate(string username, string password)
    {
        var user = _userRepository.GetByUsernameAndPassword(username, password);
        if (user == null) return null;

        var userType = _userRepository.GetUserTypeByUsername(username);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, userType ?? "User") 
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

}
