using CAS.Models;

namespace CAS.Strategy
{
    public interface IPaymentStrategy
    {
        // Every payment method must implement this
        // It takes the bill amount and returns a "Transaction ID" or success message
        string ProcessPayment(decimal amount);
    }
}