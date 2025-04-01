
using Catalog.AppDBContext;
using Catalog.Models;
using Catalog.Repositories;

namespace Catalog.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _context;

        public UserRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public User? GetByUsernameAndPassword(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Name == username && u.Password == password);
        }

        public string? GetUserTypeByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == username);

            if (user == null) return null;

            if (user is Student) return "Student";
            if (user is Teacher) return "Teacher";

            return "User"; 
        }

        public User? GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Name == username);
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.EmailAddress == email);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (existingUser == null) return;

            existingUser.Name = user.Name;
            existingUser.EmailAddress = user.EmailAddress;
            existingUser.Password = user.Password;
            existingUser.Role = user.Role;

            _context.Users.Update(existingUser);
            _context.SaveChanges();
        }
        public Student? GetStudentByUsername(string username)
        {
            return _context.Users
                           .OfType<Student>() 
                           .FirstOrDefault(s => s.Name == username);
        }

    }
}