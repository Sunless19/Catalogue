using Catalog.AppDBContext;
using Catalog.Models;
using Catalog.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;

namespace Catalog.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private ApplicationDBContext _context;
        private UserRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "UserRepoTestDb")
                .Options;

            _context = new ApplicationDBContext(options);

            var student = new Student
            {
                UserId = 1,
                Name = "john",
                EmailAddress = "john@student.com",
                Password = "pass123",
                Role = "Student"
            };

            var teacher = new Teacher
            {
                UserId = 2,
                Name = "mrs.smith",
                EmailAddress = "smith@school.com",
                Password = "teachpass",
                Role = "Teacher"
            };

            _context.Users.AddRange(student, teacher);
            _context.SaveChanges();

            _repository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetByUsernameAndPassword_ValidCredentials_ReturnsUser()
        {
            var result = _repository.GetByUsernameAndPassword("john", "pass123");

            Assert.IsNotNull(result);
            Assert.AreEqual("john", result!.Name);
        }

        [Test]
        public void GetByUsernameAndPassword_InvalidCredentials_ReturnsNull()
        {
            var result = _repository.GetByUsernameAndPassword("john", "wrongpass");

            Assert.IsNull(result);
        }

        [Test]
        public void GetUserTypeByUsername_Student_ReturnsStudent()
        {
            var result = _repository.GetUserTypeByUsername("john");

            Assert.AreEqual("Student", result);
        }

        [Test]
        public void GetUserTypeByUsername_Teacher_ReturnsTeacher()
        {
            var result = _repository.GetUserTypeByUsername("mrs.smith");

            Assert.AreEqual("Teacher", result);
        }

        [Test]
        public void GetUserTypeByUsername_NotFound_ReturnsNull()
        {
            var result = _repository.GetUserTypeByUsername("unknown");

            Assert.IsNull(result);
        }

        [Test]
        public void GetByUsername_ExistingUser_ReturnsUser()
        {
            var result = _repository.GetByUsername("john");

            Assert.IsNotNull(result);
            Assert.AreEqual("john", result!.Name);
        }

        [Test]
        public void GetByEmail_ExistingEmail_ReturnsUser()
        {
            var result = _repository.GetByEmail("john@student.com");

            Assert.IsNotNull(result);
            Assert.AreEqual("john", result!.Name);
        }

        [Test]
        public void GetAll_ReturnsAllUsers()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Add_And_Save_AddsNewUser()
        {
            var newUser = new User
            {
                Name = "newuser",
                EmailAddress = "new@user.com",
                Password = "abc123",
                Role = "User"
            };

            _repository.Add(newUser);
            _repository.Save();

            var result = _context.Users.FirstOrDefault(u => u.Name == "newuser");

            Assert.IsNotNull(result);
            Assert.AreEqual("new@user.com", result!.EmailAddress);
        }

        [Test]
        public void Update_ExistingUser_UpdatesSuccessfully()
        {
            var user = _repository.GetByUsername("john")!;
            user.EmailAddress = "updated@email.com";
            user.Password = "newpass";

            _repository.Update(user);

            var updatedUser = _repository.GetByUsername("john");

            Assert.AreEqual("updated@email.com", updatedUser!.EmailAddress);
            Assert.AreEqual("newpass", updatedUser.Password);
        }

        [Test]
        public void GetStudentByUsername_Valid_ReturnsStudent()
        {
            var result = _repository.GetStudentByUsername("john");

            Assert.IsNotNull(result);
            Assert.AreEqual("john", result!.Name);
        }

        [Test]
        public void GetStudentByUsername_NotAStudent_ReturnsNull()
        {
            var result = _repository.GetStudentByUsername("mrs.smith");

            Assert.IsNull(result);
        }

        [Test]
        public void GetById_ValidId_ReturnsUser()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("john", result!.Name);
        }

        [Test]
        public void GetById_InvalidId_ReturnsNull()
        {
            var result = _repository.GetById(999);

            Assert.IsNull(result);
        }
    }
}
