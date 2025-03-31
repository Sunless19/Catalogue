using Catalog.AppDBContext;
using Catalog.Dtos;

public class ClassRepository : IClassRepository
{
    private readonly ApplicationDBContext _context;

    public ClassRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public IEnumerable<ClassDto> GetClassesByTeacherId(int teacherId)
    {
        return _context.Classes
            .Where(c => c.TeacherId == teacherId)  // Fix: Change UserId to TeacherId
            .Select(c => new ClassDto
            {
                Name = c.Name,
                Information = c.Information,
                UserId = c.TeacherId ?? 0  // Fix: Use TeacherId
            })
            .ToList();
    }
}
