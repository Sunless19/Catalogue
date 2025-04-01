using Catalog.Models;

namespace Catalog.Repositories
{
    public interface IUserRepository
    {
        public string? GetUserTypeByUsername(string username);
        User? GetByUsername(string username);

        User? GetByUsernameAndPassword(string username, string password);
        User? GetByEmail(string email);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);

        void Save();

        Student? GetStudentByUsername(string username);
    }
}
