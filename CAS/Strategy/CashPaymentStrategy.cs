using System;

namespace CAS.Strategy
{
    public class CashPaymentStrategy : IPaymentStrategy
    {
        public string ProcessPayment(decimal amount)
        {
            // Cash is simple. It's always approved if the receptionist takes it.
            return $"TRANS-CASH-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}