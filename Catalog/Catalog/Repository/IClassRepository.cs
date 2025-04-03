using Catalog.Dtos;
using Catalog.Models;

namespace Catalog.Repository
{
    public interface IClassRepository
    {
        IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId);
        int AddStudentToClass(int classId, string studentName, out string errorMessage);
        void Update(Class classEntity);
        Class? GetById(int classId);
        Class? GetByName(string className);
        void Save();

        bool RemoveStudentFromClass(int classId, int studentId, out string errorMessage);
    }
}
