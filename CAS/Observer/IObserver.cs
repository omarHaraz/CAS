using CAS.Models;

namespace CAS.Observer
{
    public interface IObserver
    {
        // The method that gets called when a new notification arrives
        void Update(string message, Appointment appointment);
    }
}