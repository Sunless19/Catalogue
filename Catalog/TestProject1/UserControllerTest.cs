using Catalog.Controllers;
using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Catalog.Tests.Controllers
{
    [TestFixture]
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Test]
        public void Login_ValidCredentials_ReturnsOk()
        {
            var request = new LoginRequest { Username = "user", Password = "pass" };
            _userServiceMock.Setup(s => s.Authenticate("user", "pass")).Returns("mock-token");

            var result = _controller.Login(request) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var request = new LoginRequest { Username = "user", Password = "wrong" };
            _userServiceMock.Setup(s => s.Authenticate("user", "wrong")).Returns((string?)null);

            var result = _controller.Login(request) as UnauthorizedObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }
    }
}
