using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAS.Models.Users;

namespace CAS.Models
{
    // 1. UPDATE ENUM
    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Blocked 
    }

    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // 2. MAKE PATIENT NULLABLE
        public int? PatientId { get; set; } // <--- Changed from int to int?

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; } // <--- Added '?'

        public int DoctorId { get; set; }
        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string Reason { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public virtual Bill Bill { get; set; }
    }
}