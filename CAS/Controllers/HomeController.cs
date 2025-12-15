using System.Diagnostics;
using CAS.Models;
using Microsoft.AspNetCore.Mvc;
using CAS.Data; // <-- NEEDED TO ACCESS YOUR DATABASE CONTEXT
using Microsoft.EntityFrameworkCore; // NEEDED FOR .Include()
using CAS.Models.Users; // NEEDED FOR Doctor model

namespace CAS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context; // <-- ADDED DATABASE CONTEXT

        // Modified Constructor to inject ILogger and AppDbContext
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context; // Initialize the context
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DoctorDashboard()
        {
            int? doctorId = HttpContext.Session.GetInt32("UserId");

            if (doctorId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var doctor = _context.Doctors
                .Include(d => d.Specialty)
                .FirstOrDefault(d => d.Id == doctorId.Value);

            if (doctor == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            return View(doctor);
        }

        public IActionResult PatientDashboard()
        {
            return View();
        }

        public IActionResult AdminDashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}