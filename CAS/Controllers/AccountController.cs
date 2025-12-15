using Microsoft.AspNetCore.Mvc;
using CAS.Data;
using CAS.Factories;
using CAS.Models.Users;

namespace CAS.Controllers
{
    public class AccountController : Controller
    {

        private readonly AppDbContext _context;
        private readonly IUserFactory _userFactory;

        public AccountController(AppDbContext context, IUserFactory userFactory)
        {
            _context = context;
            _userFactory = userFactory;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string email, string password, string role)
        {
            try
            {
                User newUser = _userFactory.CreateUser(role);
                newUser.Username = username;
                newUser.Email = email;
                newUser.Password = password;
                newUser.Role = role;
                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            catch (ArgumentException ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                if (user.Role == "Doctor")
                {
                    return RedirectToAction("DoctorDashboard", "Home");
                }
                else if (user.Role == "Patient")
                {
                    return RedirectToAction("PatientDashboard", "Home");
                }
                else if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Home");
                }
            }
            ViewBag.Error = "Invalid Username or Password";
            return View();
        }
    }
}
