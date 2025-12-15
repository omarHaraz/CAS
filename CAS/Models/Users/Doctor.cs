using System.ComponentModel.DataAnnotations.Schema;
using CAS.Models; 
namespace CAS.Models.Users
{
    public class Doctor : User
    {
        public int? SpecialtyId { get; set; }

        [ForeignKey("SpecialtyId")]
        public Specialty? Specialty { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}