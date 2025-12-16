using CAS.Data;
using CAS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAS.Facade
{
    public class AppointmentFacade : IAppointmentFacade
    {
        private readonly AppDbContext _context;

        public AppointmentFacade(AppDbContext context)
        {
            _context = context;
        }

        // 1. Book an Appointment
        public async Task<Appointment> BookAppointmentAsync(Appointment appointment)
        {
            var isBooked = await _context.Appointments
                .AnyAsync(a => a.DoctorId == appointment.DoctorId &&
                               a.Status != AppointmentStatus.Cancelled &&
                               a.StartTime == appointment.StartTime);

            if (isBooked)
            {
                throw new InvalidOperationException("This time slot is already booked.");
            }

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        // 2. Get Available Slots
        public async Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date)
        {
            var startHour = 9;
            var endHour = 17;
            var appointmentDuration = TimeSpan.FromMinutes(30);

            var potentialSlots = new List<DateTime>();
            var currentSlot = date.Date.AddHours(startHour);

            while (currentSlot.Hour < endHour)
            {
                potentialSlots.Add(currentSlot);
                currentSlot = currentSlot.Add(appointmentDuration);
            }

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.StartTime.Date == date.Date &&
                            a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.StartTime)
                .ToListAsync();

            return potentialSlots.Except(bookedSlots);
        }

        // 3. Get Doctor Schedule (for Doctor Dashboard)
        public async Task<IEnumerable<Appointment>> GetDoctorScheduleAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && a.StartTime >= startDate && a.StartTime <= endDate)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        // -----------------------------------------------------------
        // 4. THIS WAS MISSING: Get Patient Appointments History
        // -----------------------------------------------------------
        public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)        // Eager load the Doctor so we can show their name
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.StartTime) // Show newest appointments first
                .ToListAsync();
        }
    }
}