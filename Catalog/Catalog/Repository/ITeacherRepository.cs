using Catalog.Dtos;

namespace Catalog.Repository
{
    public interface ITeacherRepository
    {
        IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId);
    }
}
