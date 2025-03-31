using Catalog.Dtos;
using Catalog.Repository;

public class TeacherService : ITeacherRepository
{
    private readonly IClassRepository _classRepository;

    public TeacherService(IClassRepository classRepository)
    {
        _classRepository = classRepository;
    }

    public IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId)
    {
        return _classRepository.GetClassesByTeacherId(teacherId);
    }
}