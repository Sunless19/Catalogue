using Catalog.AppDBContext;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly ApplicationDBContext _context;

        public GradeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Grade>> GetGradesByTeacherAsync(int teacherId)
        {
            return await _context.Grades
                .Where(g => g.TeacherId == teacherId)
                .ToListAsync();
        }

        public async Task<List<Grade>> AddGradesAsync(List<Grade> grades)
        {
            _context.Grades.AddRange(grades);
            await _context.SaveChangesAsync();
            return grades;
        }

        public async Task<Grade> AddGradeAsync(Grade grade)
        {
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task<Grade?> GetGradeByIdAsync(int gradeId)
        {
            return await _context.Grades.FindAsync(gradeId);
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

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
