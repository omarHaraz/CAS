using System.ComponentModel.DataAnnotations;

namespace CAS.Models
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } 
    }
}