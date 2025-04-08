using Catalog.Dtos;
using Catalog.Models;

namespace Catalog.Services
{
    public interface IGradeService
    {
        Task<Grade> AddGradeAsync(int teacherId, int studentId, int classId, double value, DateTime date);
        Task<List<Grade>> AddMultipleGradesAsync(int teacherId, int studentId, int classId, List<GradeEntry> grades);
        Task<bool> DeleteGradeAsync(int gradeId);
        Task<List<Grade>> GetGradesByTeacherAsync(int teacherId);
        Task<Grade?> UpdateGradeAsync(int gradeId, double value, DateTime date);
    }
}