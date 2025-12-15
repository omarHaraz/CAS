using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAS.Models.Users; 

namespace CAS.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Time")]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled";


        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient? Patient { get; set; }

        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor? Doctor { get; set; }

        [Display(Name = "Specialty")]
        public int SpecialtyId { get; set; }

        [ForeignKey("SpecialtyId")]
        public Specialty? Specialty { get; set; }
    }
}