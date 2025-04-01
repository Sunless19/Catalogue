using Catalog.Dtos;
using Catalog.Models;
using Catalog.Repositories;
using Catalog.Repository;
using Microsoft.EntityFrameworkCore;

public class ClassService : IClassRepository
{
    private readonly IClassRepository _classRepository;
    private readonly IUserRepository _userRepository;

    public ClassService(IClassRepository classRepository, IUserRepository userRepository)
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

        // Check if student is already in this class
        if (classEntity.StudentClasses.Any(sc => sc.StudentId == student.UserId))
        {
            errorMessage = "Student is already in this class.";
            return false;
        }

        // Add the student to the class
        classEntity.StudentClasses.Add(new StudentClass { StudentId = student.UserId, ClassId = classEntity.Id });
        _classRepository.Save();

        return true;
    }


    public void Update(Class classEntity) => _classRepository.Update(classEntity);
    public Class? GetById(int classId) => _classRepository.GetById(classId);
    public Class? GetByName(string className) => _classRepository.GetByName(className);
    public void Save() => _classRepository.Save();
}
