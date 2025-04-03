namespace Catalog.Dtos
{
    public class GradeEntry
    {
        public double Value { get; set; }
        public DateTime Date { get; set; }
    }

    public class MultipleGradesRequest
    {
        public int TeacherId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public List<GradeEntry> Grades { get; set; }
    }

}
