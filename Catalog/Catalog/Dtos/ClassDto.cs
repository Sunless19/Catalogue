namespace Catalog.Dtos
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Information { get; set; }
        public int UserId { get; set; }
        public List<StudentClassDto> Students { get; set; } = new List<StudentClassDto>();
    }
}
