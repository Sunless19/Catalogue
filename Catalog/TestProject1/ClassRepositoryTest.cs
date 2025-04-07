using Catalog.Models;
using Catalog.Repositories;
using Catalog.AppDBContext;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Catalog.Tests.Repositories
{
    [TestFixture]
    public class ClassRepositoryTests
    {
        private ApplicationDBContext _context;
        private ClassRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "CatalogTestDb")
                .Options;

            _context = new ApplicationDBContext(options);

            // Seed teacher and student using derived types (important for TPH!)
            var teacher = new Teacher { UserId = 1, Name = "Prof. John", Role = "Teacher" };
            var student = new Student { UserId = 2, Name = "Johnny", Role = "Student" };

            var classEntity = new Class
            {
                Id = 1,
                Name = "Mathematics",
                Information = "8th Grade Class",
                TeacherId = 1,
                StudentClasses = new List<StudentClass>()
            };

            _context.Users.AddRange(teacher, student);
            _context.Classes.Add(classEntity);
            _context.SaveChanges();

            _repository = new ClassRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void GetClassesByTeacherId_ReturnsExpectedClass()
        {
            var result = _repository.GetClassesByTeacherId(1).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mathematics", result[0].Name);
        }

        [Test]
        public void AddStudentToClass_ValidStudent_AddsSuccessfully()
        {
            var result = _repository.AddStudentToClass(1, "Johnny", out var error);

            Assert.AreEqual(2, result); // Student ID
            Assert.IsEmpty(error);

            var classEntity = _context.Classes.Include(c => c.StudentClasses).First();
            Assert.AreEqual(1, classEntity.StudentClasses.Count);
        }

        [Test]
        public void AddStudentToClass_StudentAlreadyInClass_ReturnsError()
        {
            _repository.AddStudentToClass(1, "Johnny", out _);

            var result = _repository.AddStudentToClass(1, "Johnny", out var error);

            Assert.AreEqual(-1, result);
            Assert.AreEqual("Student is already in this class.", error);
        }

        [Test]
        public void AddStudentToClass_StudentNotFound_ReturnsError()
        {
            var result = _repository.AddStudentToClass(1, "Nonexistent", out var error);

            Assert.AreEqual(-1, result);
            Assert.AreEqual("Student does not exist.", error);
        }

        [Test]
        public void RemoveStudentFromClass_ValidEnrollment_RemovesSuccessfully()
        {
            _repository.AddStudentToClass(1, "Johnny", out _);

            var result = _repository.RemoveStudentFromClass(1, 2, out var error);

            Assert.IsTrue(result);
            Assert.IsEmpty(error);
        }

        [Test]
        public void RemoveStudentFromClass_NotEnrolled_ReturnsFalse()
        {
            var result = _repository.RemoveStudentFromClass(1, 999, out var error);

            Assert.IsFalse(result);
            Assert.AreEqual("Student is not enrolled in this class.", error);
        }

        [Test]
        public void GetById_ExistingId_ReturnsClass()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Mathematics", result!.Name);
        }

        [Test]
        public void GetByName_ExistingName_ReturnsClass()
        {
            var result = _repository.GetByName("Mathematics");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.Id);
        }
    }
}
