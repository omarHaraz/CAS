using CAS.Facade; 
using CAS.Data;
using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CAS.Controllers
{
    public class BookingController : Controller
    {
        private readonly IAppointmentFacade _appointmentFacade;
        private readonly AppDbContext _context;

        // Dependency Injection: Inject the Facade we created
        public BookingController(IAppointmentFacade appointmentFacade, AppDbContext context)
        {
            _appointmentFacade = appointmentFacade;
            _context = context; // Used only to populate the "Doctors" dropdown
        }

        // 1. GET: Show the Booking Page
        [HttpGet]
        public async Task<IActionResult> Book()
        {
            // Fetch doctors to show in the dropdown menu
            var doctors = await _context.Doctors.Include(d => d.Specialty).ToListAsync();
            ViewBag.Doctors = new SelectList(doctors, "Id", "Username");
            return View();
        }

        // 2. API: Get Available Slots (Called by JavaScript)
        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int doctorId, DateTime date)
        {
            var slots = await _appointmentFacade.GetAvailableSlotsAsync(doctorId, date);
            return Json(slots.Select(s => s.ToString("HH:mm")));
        }

        // 3. POST: Confirm the Booking (Updated with Automatic Billing)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(int doctorId, string time, string reason)
        {
            // 1. Get current patient ID
            int? patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null) return RedirectToAction("Login", "Account");

            // 2. Parse the time
            if (!DateTime.TryParse(time, out DateTime selectedTime))
            {
                return BadRequest("Invalid time selected.");
            }

            var newAppointment = new Appointment
            {
                PatientId = patientId.Value,
                DoctorId = doctorId,
                StartTime = selectedTime,
                EndTime = selectedTime.AddMinutes(30),
                Reason = reason,
                Status = AppointmentStatus.Pending
            };

            try
            {
                // 3. Save the Appointment using the Facade
                var bookedAppointment = await _appointmentFacade.BookAppointmentAsync(newAppointment);

                // =========================================================
                // 4. NEW: Automatically Create a Bill ($200 Fee)
                // =========================================================
                var newBill = new Bill
                {
                    AppointmentId = bookedAppointment.Id,
                    Amount = 200.00m, // You can change this fee later
                    IsPaid = false,
                    PaymentMethod = null,
                    TransactionId = null
                };

                // Add to database context directly
                _context.Bills.Add(newBill);
                await _context.SaveChangesAsync();
                // =========================================================

                // 5. Success! Go to confirmation page
                return RedirectToAction("BookingConfirmation", new { id = bookedAppointment.Id });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);

                // Reload list for the view in case of error
                var doctors = await _context.Doctors.Include(d => d.Specialty).ToListAsync();
                ViewBag.Doctors = new SelectList(doctors, "Id", "Username");
                return View("Book");
            }
        }

        // 4. GET: Show Confirmation Page
        public IActionResult BookingConfirmation(int id)
        {
            ViewBag.AppointmentId = id;
            return View();
        }

        // Inside BookingController.cs

        // 5. GET: View My Appointments History
        public async Task<IActionResult> MyAppointments()
        {
            // 1. Get current patient ID
            int? patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null) return RedirectToAction("Login", "Account");

            // 2. Fetch data using the Facade
            // (Ensure your IAppointmentFacade has GetPatientAppointmentsAsync defined)
            var appointments = await _appointmentFacade.GetPatientAppointmentsAsync(patientId.Value);

            // 3. Pass data to the View
            return View(appointments);
        }
    }
}