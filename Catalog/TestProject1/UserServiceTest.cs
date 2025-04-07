using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Configuration;
using Catalog.Models;
using Catalog.Repositories;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Tests.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private IConfiguration _configuration;
        private Mock<IConfiguration> _configurationMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "super_secret_jwt_key_123" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings!)
                .Build();

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecureJwtTestKey123!");

                _userService = new UserService(_userRepositoryMock.Object, _configuration);

        }

        [Test]
        public void RegisterUser_UserAlreadyExists_ReturnsFalse()
        {
            var user = new User { Name = "test", EmailAddress = "test@example.com" };

            _userRepositoryMock.Setup(r => r.GetByUsername(user.Name)).Returns(new User());

            var result = _userService.RegisterUser(user);

            Assert.IsFalse(result);
        }

        [Test]
        public void RegisterUser_NewUser_ReturnsTrue()
        {
            var user = new User { Name = "newuser", EmailAddress = "new@example.com" };

            _userRepositoryMock.Setup(r => r.GetByUsername(user.Name)).Returns((User)null);
            _userRepositoryMock.Setup(r => r.GetByEmail(user.EmailAddress)).Returns((User)null);

            var result = _userService.RegisterUser(user);

            Assert.IsTrue(result);
            _userRepositoryMock.Verify(r => r.Add(user), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void Authenticate_InvalidCredentials_ReturnsNull()
        {
            _userRepositoryMock.Setup(r => r.GetByUsernameAndPassword("baduser", "badpass")).Returns((User)null);

            var result = _userService.Authenticate("baduser", "badpass");

            Assert.IsNull(result);
        }

        [Test]
        public void Authenticate_ValidCredentials_ReturnsToken()
        {
            var user = new User { UserId = 1, Name = "user", Role = "Student" };

            _userRepositoryMock.Setup(r => r.GetByUsernameAndPassword("user", "pass")).Returns(user);
            _userRepositoryMock.Setup(r => r.GetUserTypeByUsername("user")).Returns("Student");

            var result = _userService.Authenticate("user", "pass");

            Assert.IsNotNull(result);
            Assert.That(result, Does.Contain("."));
        }

        [Test]
        public void SendPasswordResetEmail_UserNotFound_ReturnsFalse()
        {
            _userRepositoryMock.Setup(r => r.GetByEmail("unknown@example.com")).Returns((User)null);

            var result = _userService.SendPasswordResetEmail("unknown@example.com");

            Assert.IsFalse(result);
        }

        [Test]
        public void SendPasswordResetEmail_UserExists_ReturnsTrue()
        {
            var user = new User { UserId = 42, EmailAddress = "known@example.com" };
            _userRepositoryMock.Setup(r => r.GetByEmail(user.EmailAddress)).Returns(user);

            var result = _userService.SendPasswordResetEmail(user.EmailAddress);

            Assert.IsTrue(result);
        }

        [Test]
        public void ResetPassword_InvalidBase64_ReturnsFalse()
        {
            var result = _userService.ResetPassword("%%%notbase64", "newpass");
            Assert.IsFalse(result);
        }

        [Test]
        public void ResetPassword_UserNotFound_ReturnsFalse()
        {
            var userId = 1;
            var encodedId = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));

            _userRepositoryMock.Setup(r => r.GetById(userId)).Returns((User)null);

            var result = _userService.ResetPassword(encodedId, "newpass");

            Assert.IsFalse(result);
        }

        [Test]
        public void ResetPassword_ValidData_ReturnsTrue()
        {
            var userId = 1;
            var encodedId = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
            var user = new User { UserId = userId, Password = "oldpass" };

            _userRepositoryMock.Setup(r => r.GetById(userId)).Returns(user);

            var result = _userService.ResetPassword(encodedId, "newpass");

            Assert.IsTrue(result);
            Assert.AreEqual("newpass", user.Password);
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
            _userRepositoryMock.Verify(r => r.Save(), Times.Once);
        }
    }
}
