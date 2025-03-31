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

    }

    public bool RegisterUser(User user)
    {
        var existingUserByName = _userRepository.GetByUsername(user.Name!);
        var existingUserByEmail = _userRepository.GetByEmail(user.EmailAddress!);

        if (existingUserByName != null || existingUserByEmail != null)
        {
            return false; 
        }

        _userRepository.Add(user);
        _userRepository.Save();

        return true;
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
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
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
