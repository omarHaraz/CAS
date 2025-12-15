using CAS.Data;
using CAS.Models;
using CAS.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CAS.Controllers
{
    public class DoctorManagementController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorManagementController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var doctors = _context.Doctors
                .Include(d => d.Specialty)
                .ToList();

            return View(doctors);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var doctor = _context.Doctors.Find(id);

            if (doctor == null)
            {
                return NotFound();
            }


            _context.Doctors.Remove(doctor);
            _context.SaveChanges();


            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SpecialtyList = new SelectList(_context.Specialties.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Doctor doctor, int specialtyId)
        {
            doctor.Role = "Doctor";

            if (ModelState.IsValid)
            {
                doctor.SpecialtyId = specialtyId;
                _context.Doctors.Add(doctor);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.SpecialtyList = new SelectList(_context.Specialties.ToList(), "Id", "Name", specialtyId);
            return View(doctor);
        }



        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = _context.Doctors
                .Include(d => d.Specialty)
                .FirstOrDefault(d => d.Id == id);

            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.SpecialtyList = new SelectList(_context.Specialties.ToList(), "Id", "Name", doctor.SpecialtyId);

            return View(doctor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Doctor doctor, int specialtyId)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            doctor.Role = "Doctor";



            if (ModelState.IsValid)
            {
                try
                {
                    doctor.SpecialtyId = specialtyId;

                    _context.Update(doctor);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Doctors.Any(e => e.Id == doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            ViewBag.SpecialtyList = new SelectList(_context.Specialties.ToList(), "Id", "Name", specialtyId);
            return View(doctor);
        }
    }
}