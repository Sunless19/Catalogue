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

    public int AddStudentToClass(string className, string studentName, out string errorMessage)
    {
        errorMessage = string.Empty;
        var student = _userRepository.GetStudentByUsername(studentName);
        if (student == null)
        {
            errorMessage = "Student does not exist.";
            return -1;
        }

        var classEntity = _classRepository.GetByName(className);
        if (classEntity == null)
        {
            errorMessage = "Class not found.";
            return -1;
        }

        // Check if the student is already in this class
        if (classEntity.StudentClasses.Any(sc => sc.StudentId == student.UserId))
        {
            errorMessage = "Student is already in this class.";
            return -1;
        }

        // Add student to the class
        classEntity.StudentClasses.Add(new StudentClass { StudentId = student.UserId, ClassId = classEntity.Id });
        _classRepository.Save();

        return student.UserId; // Return student ID
    }
    public bool RemoveStudentFromClass(int classId, int studentId, out string errorMessage)
    {
        errorMessage = string.Empty;

        var classEntity = _classRepository.GetById(classId);
        if (classEntity == null)
        {
            errorMessage = "Class not found.";
            return false;
        }

        var studentClass = classEntity.StudentClasses.FirstOrDefault(sc => sc.StudentId == studentId);
        if (studentClass == null)
        {
            errorMessage = "Student is not enrolled in this class.";
            return false;
        }

        classEntity.StudentClasses.Remove(studentClass);
        _classRepository.Save();

        return true;
    }


    public void Update(Class classEntity) => _classRepository.Update(classEntity);
    public Class? GetById(int classId) => _classRepository.GetById(classId);
    public Class? GetByName(string className) => _classRepository.GetByName(className);
    public void Save() => _classRepository.Save();
}
