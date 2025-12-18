using CAS.Data;
using CAS.Models;
using CAS.Models.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CAS.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // 1. DASHBOARD (The Observer View)
        // Shows the stats and the new Sidebar Layout
        // =========================================================
        public async Task<IActionResult> Dashboard()
        {
            var doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Account");

            var doctor = await _context.Doctors.FindAsync(doctorId);

            // COUNT: How many requests are waiting?
            // Logic: Appointment is PENDING + Bill is PAID
            var pendingCount = await _context.Appointments
                .Include(a => a.Bill)
                .CountAsync(a => a.DoctorId == doctorId &&
                                 a.Status == AppointmentStatus.Pending &&
                                 a.Bill != null &&
                                 a.Bill.IsPaid);

            // Pass this count to the View (using ViewBag for simplicity)
            ViewBag.PendingCount = pendingCount;

            return View("~/Views/Home/DoctorDashboard.cshtml", doctor);
        }

        // =========================================================
        // 2. VIEW REQUESTS (The "Inbox")
        // Lists all appointments that are Paid but need Approval
        // =========================================================
        public async Task<IActionResult> Appointments()
        {
            var doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Account");

            // FETCH: Get appointments that are Paid + Pending
            var requests = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Bill)
                .Where(a => a.DoctorId == doctorId &&
                            a.Status == AppointmentStatus.Pending &&
                            a.Bill != null &&
                            a.Bill.IsPaid)
                .ToListAsync();

            return View(requests);
        }

        // =========================================================
        // 3. APPROVE APPOINTMENT (The Action)
        // Changes status from Pending -> Confirmed
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> ApproveAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Appointments");
        }





        [HttpPost]
        public async Task<IActionResult> RejectAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment != null)
            {
                // Mark as Cancelled so the patient sees it
                appointment.Status = AppointmentStatus.Cancelled;

                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }

            // Reload the list
            return RedirectToAction("Appointments");
        }

        // =========================================================
        // 4. SCHEDULE (My Calendar)
        // Shows confirmed appointments only
        // =========================================================
        public async Task<IActionResult> Schedule()
        {
            var doctorId = HttpContext.Session.GetInt32("UserId");
            var mySchedule = await _context.Appointments
               .Include(a => a.Patient)
               .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Confirmed)
               .OrderBy(a => a.StartTime)
               .ToListAsync();

            return View(mySchedule);
        }



        [HttpGet]
        public IActionResult Leave()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> BlockSlot(DateTime date, DateTime startTime, int duration)
        {
            var doctorId = HttpContext.Session.GetInt32("UserId");
            if (doctorId == null) return RedirectToAction("Login", "Account");

            // 1. Calculate Time Range
            DateTime start = date.Date + startTime.TimeOfDay;
            DateTime end = start.AddMinutes(duration);

            // 2. FIND CONFLICTS: Check for any existing appointments in this range
            var conflictingAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                            a.Status != AppointmentStatus.Cancelled &&
                            a.Status != AppointmentStatus.Blocked &&
                            a.StartTime < end && a.EndTime > start) // Overlap formula
                .ToListAsync();

            // 3. CANCEL THEM: Loop through conflicts and cancel them
            foreach (var appt in conflictingAppointments)
            {
                appt.Status = AppointmentStatus.Cancelled;
                appt.Reason += " [Cancelled: Doctor on Leave]"; // Append note to reason
                _context.Update(appt);
            }

            // 4. CREATE BLOCK: Add the "Doctor Unavailable" slot
            var blockedSlot = new Appointment
            {
                DoctorId = doctorId.Value,
                PatientId = null, // IMPORTANT: No Patient
                StartTime = start,
                EndTime = end,
                Reason = "Doctor Unavailable / Leave",
                Status = AppointmentStatus.Blocked
            };

            _context.Appointments.Add(blockedSlot);
            await _context.SaveChangesAsync();

            // 5. Done
            return RedirectToAction("Schedule");
        }
    }
}