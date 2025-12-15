using Microsoft.AspNetCore.Mvc;
using CAS.Data;
using CAS.Models;
using System.Linq; 

namespace CAS.Controllers
{
    public class SpecialtyController : Controller
    {
        private readonly AppDbContext _context;

        public SpecialtyController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allSpecialties = _context.Specialties.ToList();
            return View(allSpecialties);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                _context.Specialties.Add(specialty);
                _context.SaveChanges();
                return RedirectToAction("Index"); 
            }
            return View(specialty);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = _context.Specialties.FirstOrDefault(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult DeleteConfirmed(int id)
        {
            var specialty = _context.Specialties.Find(id);

            if (specialty == null)
            {
                return NotFound();
            }



            _context.Specialties.Remove(specialty);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}