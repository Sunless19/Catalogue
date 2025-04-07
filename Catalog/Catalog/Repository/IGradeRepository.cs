using Catalog.Models;

namespace Catalog.Repositories
{
    public interface IGradeRepository
    {
        Task<List<Grade>> GetGradesByTeacherAsync(int teacherId);
        Task<List<Grade>> AddGradesAsync(List<Grade> grades);
        Task<Grade> AddGradeAsync(Grade grade);
        Task<Grade?> GetGradeByIdAsync(int gradeId);
        Task<bool> DeleteGradeAsync(int gradeId);
        Task SaveAsync();
    }
}