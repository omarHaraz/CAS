using CAS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CAS.Facade 
{
    public interface IAppointmentFacade
    {
        Task<IEnumerable<DateTime>> GetAvailableSlotsAsync(int doctorId, DateTime date);

        Task<Appointment> BookAppointmentAsync(Appointment appointment);

        Task<IEnumerable<Appointment>> GetDoctorScheduleAsync(int doctorId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId);
    }
}