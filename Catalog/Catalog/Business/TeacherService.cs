using Catalog.Dtos;
using Catalog.Repositories;
using Catalog.Repository;

public class TeacherService : ITeacherRepository
{
    private readonly IClassRepository _classRepository;
    private readonly IUserRepository _userRepository;

    public TeacherService(IClassRepository classRepository, IUserRepository userRepository)
    {
        _classRepository = classRepository;
        _userRepository = userRepository;
    }

    public IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId)
    {
        return _classRepository.GetClassesByTeacherId(teacherId);
    }

    public bool AddStudentToClass(string className, string studentName, out string errorMessage)
    {
        errorMessage = string.Empty;

        var student = _userRepository.GetStudentByUsername(studentName);
        if (student == null)
        {
            errorMessage = "Student does not exist.";
            return false;
        }

        var classEntity = _classRepository.GetByName(className);
        if (classEntity == null)
        {
            errorMessage = "Class not found.";
            return false;
        }

        if (classEntity.Students.Contains(student))
        {
            errorMessage = "Student already exists in this class.";
            return false;
        }

        classEntity.Students.Add(student);
        _classRepository.Save();

        return true;
    }
}