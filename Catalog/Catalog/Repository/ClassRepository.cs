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
                .Include(c => c.StudentClasses)
                    .ThenInclude(sc => sc.Student)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Information = c.Information,
                    UserId = c.TeacherId ?? 0,
                    Students = c.StudentClasses
                        .Select(sc => new StudentClassDto
                        {
                            StudentId = sc.StudentId,
                            ClassId = sc.ClassId,
                            StudentName = sc.Student.Name
                        }).ToList()
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

        public int AddStudentToClass(string className, string studentName, out string errorMessage)
        {
            errorMessage = string.Empty;

            var student = _context.Students.FirstOrDefault(s => s.Name == studentName);
            if (student == null)
            {
                errorMessage = "Student does not exist.";
                return -1;
            }

            var classEntity = _context.Classes
                .Include(c => c.StudentClasses)
                .FirstOrDefault(c => c.Name == className);
            if (classEntity == null)
            {
                errorMessage = "Class not found.";
                return -1;
            }

            // Check if the student is already in this class
            if (classEntity.StudentClasses.Any(sc => sc.StudentId == student.UserId))
            {
                errorMessage = "Student is already in this class.";
                return -1;
            }

            // Add student to the class
            classEntity.StudentClasses.Add(new StudentClass { StudentId = student.UserId, ClassId = classEntity.Id });
            _context.SaveChanges();

            return student.UserId; // Return student ID
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

        public bool RemoveStudentFromClass(int classId, int studentId, out string errorMessage)
        {
            errorMessage = string.Empty;

            var studentClass = _context.StudentClasses
                .FirstOrDefault(sc => sc.ClassId == classId && sc.StudentId == studentId);

            if (studentClass == null)
            {
                errorMessage = "Student is not enrolled in this class.";
                return false;
            }
            var grades = _context.Grades.Where(g => g.ClassId == classId && g.StudentId == studentId).ToList();
            _context.Grades.RemoveRange(grades);
            _context.StudentClasses.Remove(studentClass);
            _context.SaveChanges();

            return true;
        }
    }
}