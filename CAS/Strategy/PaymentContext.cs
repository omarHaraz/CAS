namespace CAS.Strategy
{
    public class PaymentContext
    {
        private IPaymentStrategy _paymentStrategy;

        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            _paymentStrategy = strategy;
        }

        public string ExecutePayment(decimal amount)
        {
            if (_paymentStrategy == null)
            {
                return "Error: No payment method selected.";
            }

            return _paymentStrategy.ProcessPayment(amount);
        }
    }
}