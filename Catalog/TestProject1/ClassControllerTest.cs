using Catalog.Controllers;
using Catalog.Dtos;
using Catalog.Repositories;
using Catalog.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Catalog.Tests.Controllers
{
    [TestFixture]
    public class ClassControllerTests
    {
        private Mock<IClassRepository> _classRepoMock;
        private ClassController _controller;

        [SetUp]
        public void Setup()
        {
            _classRepoMock = new Mock<IClassRepository>();
            _controller = new ClassController(_classRepoMock.Object);
        }

        private void SetUserContextWithUserId(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public void GetClasses_WithValidToken_ReturnsClasses()
        {
            SetUserContextWithUserId(1);

            var mockClasses = new List<ClassDto>
            {
                new ClassDto { Id = 1, Name = "Math", Information = "Grade 8", UserId = 1 }
            };

            _classRepoMock.Setup(x => x.GetClassesByTeacherId(1)).Returns(mockClasses);

            var result = _controller.GetClasses() as OkObjectResult;

            Assert.IsNotNull(result);
            var data = result.Value as List<ClassDto>;
            Assert.AreEqual(1, data.Count);
        }

        [Test]
        public void GetClasses_NoToken_ReturnsUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // no user
            };

            var result = _controller.GetClasses() as UnauthorizedObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [Test]
        public void GetStudentClassesWithGrades_StudentFound_ReturnsOk()
        {
            var mockData = new List<ClassWithGradesDto>
            {
                new ClassWithGradesDto
                {
                    ClassId = 1,
                    ClassName = "Math",
                    Grades = new List<GradeEntry> { new GradeEntry { Value = 10 } }
                }
            };

            _classRepoMock.Setup(x => x.GetClassesWithGradesByStudentId(1))
                .Returns(mockData);

            var result = _controller.GetStudentClassesWithGrades(1);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var value = okResult.Value as List<ClassWithGradesDto>;
            Assert.AreEqual(1, value.Count);
        }

        [Test]
        public void GetStudentClassesWithGrades_StudentNotFound_ReturnsNotFound()
        {
            _classRepoMock.Setup(x => x.GetClassesWithGradesByStudentId(999))
                .Returns(new List<ClassWithGradesDto>());

            var result = _controller.GetStudentClassesWithGrades(999);
            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [Test]
        public void AddStudentToClass_InvalidRequest_ReturnsBadRequest()
        {
            var result = _controller.AddStudentToClass(null) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }


        [Test]
        public void DeleteStudent_InvalidRequest_ReturnsBadRequest()
        {
            var request = new ClassController.DeleteStudentRequest
            {
                ClassId = 0,
                StudentId = 0
            };

            var result = _controller.DeleteStudent(request) as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

    }
}
