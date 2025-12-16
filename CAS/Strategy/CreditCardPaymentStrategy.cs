using System;

namespace CAS.Strategy
{
    public class CreditCardPaymentStrategy : IPaymentStrategy
    {
        private readonly string _cardNumber;
        private readonly string _cvv;

        public CreditCardPaymentStrategy(string cardNumber, string cvv)
        {
            _cardNumber = cardNumber;
            _cvv = cvv;
        }

        public string ProcessPayment(decimal amount)
        {
            // In a real app, you would call a Bank API (Stripe/PayPal) here.
            // For this project, we simulate a successful transaction.
            return $"TRANS-CC-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}