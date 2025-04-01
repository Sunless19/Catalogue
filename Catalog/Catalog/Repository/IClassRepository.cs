using Catalog.Dtos;
using Catalog.Models;

public interface IClassRepository
{
    IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId);
    void Update(Class classEntity);
    Class? GetById(int classId);
    Class? GetByName(string className);
    void Save();
}
