namespace Catalog.Models
{
    public class Student : User
    {
        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>();
    }
}