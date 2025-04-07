using NUnit.Framework;
using Moq;
using Catalog.Models;
using Catalog.Repositories;
using System.Collections.Generic;
using System.Linq;
using Catalog.Repository;

namespace Catalog.Tests.Services
{
    [TestFixture]
    public class ClassServiceTests
    {
        private Mock<IClassRepository> _classRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private ClassService _classService;

        [SetUp]
        public void SetUp()
        {
            _classRepoMock = new Mock<IClassRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _classService = new ClassService(_classRepoMock.Object, _userRepoMock.Object);
        }

        [Test]
        public void AddStudentToClass_StudentDoesNotExist_ReturnsMinusOne()
        {
            _userRepoMock.Setup(r => r.GetStudentByUsername("john")).Returns((Student)null);

            var result = _classService.AddStudentToClass(1, "john", out var errorMessage);

            Assert.AreEqual(-1, result);
            Assert.AreEqual("Student does not exist.", errorMessage);
        }

        [Test]
        public void AddStudentToClass_ClassDoesNotExist_ReturnsMinusOne()
        {
            var student = new Student { UserId = 10 };
            _userRepoMock.Setup(r => r.GetStudentByUsername("john")).Returns(student);
            _classRepoMock.Setup(r => r.GetById(1)).Returns((Class)null);

            var result = _classService.AddStudentToClass(1, "john", out var errorMessage);

            Assert.AreEqual(-1, result);
            Assert.AreEqual("Class not found.", errorMessage);
        }

        [Test]
        public void AddStudentToClass_StudentAlreadyInClass_ReturnsMinusOne()
        {
            var student = new Student { UserId = 10 };
            var classEntity = new Class
            {
                Id = 1,
                StudentClasses = new List<StudentClass> { new StudentClass { StudentId = 10 } }
            };

            _userRepoMock.Setup(r => r.GetStudentByUsername("john")).Returns(student);
            _classRepoMock.Setup(r => r.GetById(1)).Returns(classEntity);

            var result = _classService.AddStudentToClass(1, "john", out var errorMessage);

            Assert.AreEqual(-1, result);
            Assert.AreEqual("Student is already in this class.", errorMessage);
        }

        [Test]
        public void AddStudentToClass_Valid_AddsStudentAndReturnsId()
        {
            var student = new Student { UserId = 10 };
            var classEntity = new Class
            {
                Id = 1,
                StudentClasses = new List<StudentClass>()
            };

            _userRepoMock.Setup(r => r.GetStudentByUsername("john")).Returns(student);
            _classRepoMock.Setup(r => r.GetById(1)).Returns(classEntity);

            var result = _classService.AddStudentToClass(1, "john", out var errorMessage);

            Assert.AreEqual(10, result);
            Assert.AreEqual(string.Empty, errorMessage);
            Assert.IsTrue(classEntity.StudentClasses.Any(sc => sc.StudentId == 10));
            _classRepoMock.Verify(r => r.Save(), Times.Once);
        }

        [Test]
        public void RemoveStudentFromClass_ClassDoesNotExist_ReturnsFalse()
        {
            _classRepoMock.Setup(r => r.GetById(1)).Returns((Class)null);

            var result = _classService.RemoveStudentFromClass(1, 10, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Class not found.", errorMessage);
        }

        [Test]
        public void RemoveStudentFromClass_StudentNotInClass_ReturnsFalse()
        {
            var classEntity = new Class
            {
                Id = 1,
                StudentClasses = new List<StudentClass>()
            };

            _classRepoMock.Setup(r => r.GetById(1)).Returns(classEntity);

            var result = _classService.RemoveStudentFromClass(1, 10, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Student is not enrolled in this class.", errorMessage);
        }

        [Test]
        public void RemoveStudentFromClass_StudentInClass_RemovesAndReturnsTrue()
        {
            var studentClass = new StudentClass { StudentId = 10 };
            var classEntity = new Class
            {
                Id = 1,
                StudentClasses = new List<StudentClass> { studentClass }
            };

            _classRepoMock.Setup(r => r.GetById(1)).Returns(classEntity);

            var result = _classService.RemoveStudentFromClass(1, 10, out var errorMessage);

            Assert.IsTrue(result);
            Assert.AreEqual(string.Empty, errorMessage);
            Assert.IsEmpty(classEntity.StudentClasses);
            _classRepoMock.Verify(r => r.Save(), Times.Once);
        }
    }
}
