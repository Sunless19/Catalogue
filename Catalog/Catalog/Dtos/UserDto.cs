public class UserDto
{
    public string Username { get; set; }
    public string Password { get; set; } // în realitate ar trebui hashuit
    public string Role { get; set; }
}
