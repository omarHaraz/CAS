using CAS.Data;
using CAS.Models;
using CAS.Strategy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CAS.Controllers
{
    public class BillingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PaymentContext _paymentContext;

        // Dependency Injection: Inject Database AND PaymentContext
        public BillingController(AppDbContext context, PaymentContext paymentContext)
        {
            _context = context;
            _paymentContext = paymentContext;
        }

        // =========================================================
        // API 1: GET /Billing/Index
        // Shows all unpaid bills for the logged-in patient
        // =========================================================
        public async Task<IActionResult> Index()
        {
            var patientId = HttpContext.Session.GetInt32("UserId");
            if (patientId == null) return RedirectToAction("Login", "Account");

            // Fetch unpaid bills linked to this patient's appointments
            var bills = await _context.Bills
                .Include(b => b.Appointment)
                .ThenInclude(a => a.Doctor)
                .Where(b => b.Appointment.PatientId == patientId.Value && !b.IsPaid)
                .ToListAsync();

            return View(bills);
        }

        // =========================================================
        // API 2: GET /Billing/Pay/{id}
        // Opens the Payment Page for a specific Bill
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Pay(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null || bill.IsPaid) return RedirectToAction("Index");

            return View(bill);
        }

        // =========================================================
        // API 3: POST /Billing/ProcessPayment
        // The CORE logic that uses the Strategy Pattern
        // =========================================================
        // 3. POST: Process Payment (Updated to Confirm Appointment)
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int billId, string paymentMethod)
        {
            // 1. Get the Bill (and include the Appointment so we can update it)
            var bill = await _context.Bills
                .Include(b => b.Appointment) // <--- CRITICAL: Load the appointment
                .FirstOrDefaultAsync(b => b.Id == billId);

            if (bill == null) return NotFound();

            // 2. Select Strategy (Illusion)
            IPaymentStrategy strategy = null;
            if (paymentMethod == "CreditCard")
            {
                strategy = new CreditCardPaymentStrategy("4242-4242", "999");
            }
            else if (paymentMethod == "Cash")
            {
                strategy = new CashPaymentStrategy();
            }
            else
            {
                return RedirectToAction("Index");
            }

            // 3. Fake Delay
            await Task.Delay(2000);

            // 4. Process Payment
            _paymentContext.SetPaymentStrategy(strategy);
            string transactionResult = _paymentContext.ExecutePayment(bill.Amount);

            // 5. UPDATE BILL STATUS
            bill.IsPaid = true;
            bill.PaymentMethod = paymentMethod;
            bill.TransactionId = transactionResult;

            // =========================================================
            // 6. NEW: UPDATE APPOINTMENT STATUS TO 'CONFIRMED'
            // =========================================================
            if (bill.Appointment != null)
            {
                bill.Appointment.Status = AppointmentStatus.Confirmed;
            }
            // =========================================================

            _context.Bills.Update(bill);
            await _context.SaveChangesAsync();

            return RedirectToAction("PaymentSuccess");
        }
        public IActionResult PaymentSuccess()
        {
            return View();
        }
    }
}