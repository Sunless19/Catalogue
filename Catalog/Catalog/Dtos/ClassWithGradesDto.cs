namespace Catalog.Dtos
{
    public class ClassWithGradesDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public List<GradeEntry> Grades { get; set; } = new();
    }

}