using System.ComponentModel.DataAnnotations;

namespace CAS.Models.Users
{
    public class Patient : User
    {
        [StringLength(10)]
        public string? InsuranceNumber { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}