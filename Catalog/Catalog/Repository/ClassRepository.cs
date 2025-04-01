using Catalog.AppDBContext;
using Catalog.Dtos;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDBContext _context;

    public ClassRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId)
    {
        return _context.Classes.Where(c => c.TeacherId == teacherId).Select(c => new ClassDto
        {
            Id = c.Id,
            Name = c.Name,
            Information = c.Information,
            UserId = c.TeacherId ?? 0,
            Students = c.Students.Select(s => s.Name).ToList()
        }).ToList();
    }


    public void Update(Class classEntity)
    {
        _context.Classes.Update(classEntity);
        _context.SaveChanges();
    }
    public Class? GetById(int classId)
    {
        return _context.Classes.FirstOrDefault(c => c.Id == classId);
    }

    public Class? GetByName(string className)
    {
        return _context.Classes.FirstOrDefault(c => c.Name == className);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
