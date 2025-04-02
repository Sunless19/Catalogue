using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Models
{
    public class Grade
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [Required]
        public double Value { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Foreign key for Student
        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        // Foreign key for Class
        [Required]
        public int ClassId { get; set; }
        [ForeignKey("ClassId")]
        public Class Class { get; set; }

        // Foreign key for Teacher
        [Required]
        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; }
    }
}
