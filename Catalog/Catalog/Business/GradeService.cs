using Catalog.AppDBContext;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Catalog.Services
{
    public class GradeService
    {
        private readonly ApplicationDBContext _context;

        public GradeService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<Grade>> GetGradesByTeacherAsync(int teacherId)
        {
            return await _context.Grades
                .Where(g => g.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<Grade> AddGradeAsync(int teacherId, int studentId, int classId, double value, DateTime date)
        {
            var grade = new Grade
            {
                Value = value,
                Date = date,
                StudentId = studentId,
                ClassId = classId,
                TeacherId = teacherId
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade?> UpdateGradeAsync(int gradeId, double value, DateTime date)
        {
            var grade = await _context.Grades.FindAsync(gradeId);

            if (grade == null)
                return null;

            // Only modify Value and Date
            grade.Value = value;
            grade.Date = date;

            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task<bool> DeleteGradeAsync(int gradeId)
        {
            var grade = await _context.Grades.FindAsync(gradeId);
            if (grade == null)
                return false;

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
