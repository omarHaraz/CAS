using CAS.Data;
using CAS.Models;
using CAS.Strategy;
using CAS.Observer; // <--- ADD THIS NAMESPACE
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CAS.Controllers
{
    public class BillingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PaymentContext _paymentContext;
        private readonly AppointmentNotifier _notifier; // <--- 1. Add the Notifier

        // Inject the Notifier in the constructor
        public BillingController(AppDbContext context, PaymentContext paymentContext, AppointmentNotifier notifier)
        {
            _context = context;
            _paymentContext = paymentContext;
            _notifier = notifier; // <--- 2. Initialize it
        }

        public async Task<IActionResult> Index()
        {
            var patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null) return RedirectToAction("Login", "Account");

            var bills = await _context.Bills
                .Include(b => b.Appointment)
                .ThenInclude(a => a.Doctor)
                .Where(b => b.Appointment.PatientId == patientId.Value && !b.IsPaid)
                .ToListAsync();

            return View(bills);
        }

        [HttpGet]
        public async Task<IActionResult> Pay(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null || bill.IsPaid) return RedirectToAction("Index");

            return View(bill);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int billId, string paymentMethod)
        {
            // Include Appointment so we know WHICH Doctor to notify
            var bill = await _context.Bills
                .Include(b => b.Appointment)
                .FirstOrDefaultAsync(b => b.Id == billId);

            if (bill == null) return NotFound();

            // 1. Strategy (The Illusion)
            IPaymentStrategy strategy = null;
            if (paymentMethod == "CreditCard")
            {
                strategy = new CreditCardPaymentStrategy("FAKE-CARD", "999");
            }
            else if (paymentMethod == "Cash")
            {
                strategy = new CashPaymentStrategy();
            }
            else
            {
                return RedirectToAction("Index");
            }

            // 2. Fake Delay
            await Task.Delay(2000);

            // 3. Execute Payment
            _paymentContext.SetPaymentStrategy(strategy);
            string transactionResult = _paymentContext.ExecutePayment(bill.Amount);

            // 4. Update Bill Logic
            bill.IsPaid = true;
            bill.PaymentMethod = paymentMethod;
            bill.TransactionId = transactionResult;

            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();

            // =========================================================
            // 5. OBSERVER PATTERN TRIGGER (Send the Alert!)
            // =========================================================
            string message = $"Appointment #{bill.AppointmentId} has been paid.";

            // Notify the specific doctor linked to this appointment
            _notifier.Notify(message, bill.Appointment.DoctorId);
            // =========================================================

            return RedirectToAction("PaymentSuccess");
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }
    }
}