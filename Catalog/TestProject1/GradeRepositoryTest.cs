using Catalog.AppDBContext;
using Catalog.Models;
using Catalog.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Tests.Repositories
{
    [TestFixture]
    public class GradeRepositoryTests
    {
        private ApplicationDBContext _context;
        private GradeRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "GradeRepoTestDb")
                .Options;

            _context = new ApplicationDBContext(options);

            // Seed data
            _context.Grades.AddRange(
                new Grade { Id = 1, StudentId = 1, TeacherId = 10, ClassId = 100, Value = 8 },
                new Grade { Id = 2, StudentId = 2, TeacherId = 10, ClassId = 101, Value = 9 },
                new Grade { Id = 3, StudentId = 1, TeacherId = 11, ClassId = 102, Value = 10}
            );
            _context.SaveChanges();

            _repository = new GradeRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetGradesByTeacherAsync_ReturnsCorrectGrades()
        {
            var result = await _repository.GetGradesByTeacherAsync(10);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(g => g.TeacherId == 10));
        }

        [Test]
        public async Task AddGradeAsync_AddsGradeSuccessfully()
        {
            var newGrade = new Grade
            {
                StudentId = 3,
                TeacherId = 12,
                ClassId = 103,
                Value = 7,
            };

            var result = await _repository.AddGradeAsync(newGrade);

            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Value);

            var allGrades = _context.Grades.ToList();
            Assert.AreEqual(4, allGrades.Count); // 3 initial + 1 new
        }

        [Test]
        public async Task AddGradesAsync_AddsMultipleGrades()
        {
            var newGrades = new List<Grade>
            {
                new Grade { StudentId = 4, TeacherId = 13, ClassId = 104, Value = 6 },
                new Grade { StudentId = 5, TeacherId = 13, ClassId = 104, Value = 9 }
            };

            var result = await _repository.AddGradesAsync(newGrades);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(5, _context.Grades.Count()); // 3 initial + 2 new
        }

        [Test]
        public async Task GetGradeByIdAsync_ValidId_ReturnsGrade()
        {
            var result = await _repository.GetGradeByIdAsync(1);

            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetGradeByIdAsync_InvalidId_ReturnsNull()
        {
            var result = await _repository.GetGradeByIdAsync(999);

            Assert.IsNull(result);
        }

        [Test]
        public async Task DeleteGradeAsync_ValidId_DeletesSuccessfully()
        {
            var result = await _repository.DeleteGradeAsync(2);

            Assert.IsTrue(result);
            Assert.AreEqual(2, _context.Grades.Count()); // one grade removed
        }

        [Test]
        public async Task DeleteGradeAsync_InvalidId_ReturnsFalse()
        {
            var result = await _repository.DeleteGradeAsync(999);

            Assert.IsFalse(result);
            Assert.AreEqual(3, _context.Grades.Count()); // unchanged
        }
    }
}
