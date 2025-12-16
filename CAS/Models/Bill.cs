using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAS.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentMethod { get; set; } 
        public string? TransactionId { get; set; }

        // Navigation Property
        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }
    }
}