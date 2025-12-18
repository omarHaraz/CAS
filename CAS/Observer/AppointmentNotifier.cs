using CAS.Models;
using System.Collections.Generic;
using System.Linq;

namespace CAS.Observer
{
    public class AppointmentNotifier : ISubject
    {
        // A list of all listeners (Observers)
        private readonly List<IObserver> _observers = new List<IObserver>();

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(string message, int doctorId)
        {
            // In a real complex app, we would filter observers by DoctorId.
            // For this project, we notify all attached observers (simple simulation).
            foreach (var observer in _observers)
            {
                // We pass null for the appointment here for simplicity, 
                // or we could fetch the real appointment if needed.
                observer.Update(message, null);
            }
        }
    }
}