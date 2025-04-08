using Catalog.Dtos;
using Catalog.Models;
using Catalog.Repositories;
using Catalog.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.Tests.Services
{
    [TestFixture]
    public class GradeServiceTests
    {
        private Mock<IGradeRepository> _gradeRepositoryMock;
        private GradeService _gradeService;

        [SetUp]
        public void SetUp()
        {
            _gradeRepositoryMock = new Mock<IGradeRepository>();
            _gradeService = new GradeService(_gradeRepositoryMock.Object);
        }

        [Test]
        public async Task GetGradesByTeacherAsync_ShouldReturnGrades()
        {
            // Arrange
            int teacherId = 1;
            var grades = new List<Grade>
            {
                new Grade { Id = 1, TeacherId = teacherId, Value = 9 },
                new Grade { Id = 2, TeacherId = teacherId, Value = 8 }
            };
            _gradeRepositoryMock.Setup(repo => repo.GetGradesByTeacherAsync(teacherId))
                                .ReturnsAsync(grades);

            // Act
            var result = await _gradeService.GetGradesByTeacherAsync(teacherId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            _gradeRepositoryMock.Verify(repo => repo.GetGradesByTeacherAsync(teacherId), Times.Once);
        }

        [Test]
        public async Task AddMultipleGradesAsync_ShouldAddGradesCorrectly()
        {
            // Arrange
            int teacherId = 1, studentId = 2, classId = 3;
            var gradeEntries = new List<GradeEntry>
            {
                new GradeEntry { Value = 10, Date = DateTime.Now},
                new GradeEntry { Value = 9, Date = DateTime.Now }
            };

            var expectedGrades = new List<Grade>
            {
                new Grade { Value = 10, StudentId = studentId, ClassId = classId, TeacherId = teacherId },
                new Grade { Value = 9, StudentId = studentId, ClassId = classId, TeacherId = teacherId}
            };

            _gradeRepositoryMock.Setup(repo => repo.AddGradesAsync(It.IsAny<List<Grade>>()))
                                .ReturnsAsync(expectedGrades);

            // Act
            var result = await _gradeService.AddMultipleGradesAsync(teacherId, studentId, classId, gradeEntries);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            _gradeRepositoryMock.Verify(repo => repo.AddGradesAsync(It.IsAny<List<Grade>>()), Times.Once);
        }

        [Test]
        public async Task AddGradeAsync_ShouldReturnAddedGrade()
        {
            // Arrange
            int teacherId = 1, studentId = 2, classId = 3;
            double value = 8.5;
            DateTime date = DateTime.Today;
            string assignments = "Tema speciala";

            var expectedGrade = new Grade
            {
                Value = value,
                Date = date,
                StudentId = studentId,
                ClassId = classId,
                TeacherId = teacherId
            };

            _gradeRepositoryMock.Setup(repo => repo.AddGradeAsync(It.IsAny<Grade>()))
                                .ReturnsAsync(expectedGrade);

            // Act
            var result = await _gradeService.AddGradeAsync(teacherId, studentId, classId, value, date);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(value, result.Value);
            _gradeRepositoryMock.Verify(repo => repo.AddGradeAsync(It.IsAny<Grade>()), Times.Once);
        }

        [Test]
        public async Task UpdateGradeAsync_ShouldUpdateGrade_WhenGradeExists()
        {
            // Arrange
            int gradeId = 1;
            double newValue = 9;
            DateTime newDate = DateTime.Today;

            var existingGrade = new Grade { Id = gradeId, Value = 8 };
            _gradeRepositoryMock.Setup(repo => repo.GetGradeByIdAsync(gradeId))
                                .ReturnsAsync(existingGrade);

            // Act
            var result = await _gradeService.UpdateGradeAsync(gradeId, newValue, newDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newValue, result.Value);
            _gradeRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
        }

        [Test]
        public async Task UpdateGradeAsync_ShouldReturnNull_WhenGradeDoesNotExist()
        {
            // Arrange
            int gradeId = 99;
            _gradeRepositoryMock.Setup(repo => repo.GetGradeByIdAsync(gradeId))
                                .ReturnsAsync((Grade)null);

            // Act
            var result = await _gradeService.UpdateGradeAsync(gradeId, 9, DateTime.Today);

            // Assert
            Assert.IsNull(result);
            _gradeRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteGradeAsync_ShouldReturnTrue_WhenDeleteSucceeds()
        {
            // Arrange
            int gradeId = 1;
            _gradeRepositoryMock.Setup(repo => repo.DeleteGradeAsync(gradeId))
                                .ReturnsAsync(true);

            // Act
            var result = await _gradeService.DeleteGradeAsync(gradeId);

            // Assert
            Assert.IsTrue(result);
            _gradeRepositoryMock.Verify(repo => repo.DeleteGradeAsync(gradeId), Times.Once);
        }

        [Test]
        public async Task DeleteGradeAsync_ShouldReturnFalse_WhenDeleteFails()
        {
            // Arrange
            int gradeId = 1;
            _gradeRepositoryMock.Setup(repo => repo.DeleteGradeAsync(gradeId))
                                .ReturnsAsync(false);

            // Act
            var result = await _gradeService.DeleteGradeAsync(gradeId);

            // Assert
            Assert.IsFalse(result);
            _gradeRepositoryMock.Verify(repo => repo.DeleteGradeAsync(gradeId), Times.Once);
        }
    }
}
