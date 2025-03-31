using Catalog.Dtos;

public interface IClassRepository
{
    IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId);
}
