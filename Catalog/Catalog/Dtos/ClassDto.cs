namespace Catalog.Dtos
{
    public class ClassDto
    {
        public string Name { get; set; }
        public string Information { get; set; }
        public int UserId { get; set; }
        public List<string> Students { get; set; } = new List<string>();
    }
}
