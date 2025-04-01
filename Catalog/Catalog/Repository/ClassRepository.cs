using Catalog.AppDBContext;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Repository; // ✅ Added missing namespace
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Catalog.Repositories // ✅ Changed namespace to match the project structure
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDBContext _context;

        public ClassRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId)
        {
            return _context.Classes
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.StudentClasses) // Include StudentClasses for eager loading
                .ThenInclude(sc => sc.Student)  // Then include actual Student details
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Information = c.Information,
                    UserId = c.TeacherId ?? 0,
                    Students = c.StudentClasses.Select(sc => sc.Student.Name).ToList()
                }).ToList();
        }

        public Class? GetById(int classId)
        {
            return _context.Classes
                .Include(c => c.StudentClasses)
                .ThenInclude(sc => sc.Student)
                .FirstOrDefault(c => c.Id == classId);
        }

        public Class? GetByName(string className)
        {
            return _context.Classes.FirstOrDefault(c => c.Name == className);
        }

        public bool AddStudentToClass(string className, string studentName, out string errorMessage)
        {
            errorMessage = string.Empty;

            var student = _context.Students.FirstOrDefault(s => s.Name == studentName);
            if (student == null)
            {
                errorMessage = "Student does not exist.";
                return false;
            }

            var classEntity = _context.Classes
                .Include(c => c.StudentClasses)
                .FirstOrDefault(c => c.Name == className);
            if (classEntity == null)
            {
                errorMessage = "Class not found.";
                return false;
            }

            // Check if the student is already in this class
            if (classEntity.StudentClasses.Any(sc => sc.StudentId == student.UserId))
            {
                errorMessage = "Student is already in this class.";
                return false;
            }

            // Add student to the class through StudentClass
            classEntity.StudentClasses.Add(new StudentClass { StudentId = student.UserId, ClassId = classEntity.Id });
            _context.SaveChanges();

            return true;
        }

        public void Update(Class classEntity)
        {
            _context.Classes.Update(classEntity);
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}