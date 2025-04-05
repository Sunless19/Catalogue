using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Catalog.Models;
using Catalog.Repositories;
using System.Net.Mail;
using System.Net;

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

    public bool SendPasswordResetEmail(string email)
    {
        var user = _userRepository.GetByEmail(email);
        if (user == null) return false;

        var userIdBytes = Encoding.UTF8.GetBytes(user.UserId.ToString());
        var encodedId = Convert.ToBase64String(userIdBytes);

        var resetLink = $"http://localhost:4200/reset-password?id={encodedId}";

        SendEmail(email, resetLink);

        return true;
    }

    public bool ResetPassword(string encodedId, string newPassword)
    {
        try
        {
            var userIdString = Encoding.UTF8.GetString(Convert.FromBase64String(encodedId));
            if (!int.TryParse(userIdString, out int userId))
                return false;

            var user = _userRepository.GetById(userId);
            if (user == null)
                return false;

            user.Password = newPassword;
            _userRepository.Update(user);
            _userRepository.Save();

            return true;
        }
        catch
        {
            return false;
        }
    }

    private void SendEmail(string toEmail, string resetLink)
    {
        try
        {
            var fromAddress = new MailAddress("andreeaangelescu011@gmail.com", "Catalogue");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "gtijsbwygsfexkpb"; 
            const string subject = "Reset Your Password";
            string body = $"Click the link below to reset your password:\n{resetLink}";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            smtp.Send(message);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Failed to send email: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);
        }
    }

}
