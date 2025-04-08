using Catalog.Controllers;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Catalog.Tests.Controllers
{
    [TestFixture]
    public class GradeControllerTests
    {
        private Mock<IGradeService> _gradeServiceMock;
        private GradeController _controller;

        [SetUp]
        public void Setup()
        {
            _gradeServiceMock = new Mock<IGradeService>();
            _controller = new GradeController(_gradeServiceMock.Object);
        }

        [Test]
        public async Task AddGrade_ValidRequest_ReturnsOk()
        {
            var request = new GradeRequest
            {
                TeacherId = 1,
                StudentId = 2,
                ClassId = 3,
                Value = 9,
                Date = DateTime.Today,
                Assignments = "Test 1"
            };

            var mockGrade = new Grade
            {
                Id = 1,
                TeacherId = 1,
                StudentId = 2,
                ClassId = 3,
                Value = 9,
                Date = DateTime.Today,
            };

            _gradeServiceMock.Setup(s => s.AddGradeAsync(
                request.TeacherId,
                request.StudentId,
                request.ClassId,
                request.Value,
                request.Date)).ReturnsAsync(mockGrade);

            var result = await _controller.AddGrade(request) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task DeleteGrade_GradeExists_ReturnsOk()
        {
            _gradeServiceMock.Setup(s => s.DeleteGradeAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteGrade(1) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task DeleteGrade_GradeNotFound_ReturnsNotFound()
        {
            _gradeServiceMock.Setup(s => s.DeleteGradeAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeleteGrade(99) as NotFoundObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
