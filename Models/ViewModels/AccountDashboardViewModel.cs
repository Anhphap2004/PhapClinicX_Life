using System.Collections.Generic;
using PhapClinicX.Models;

namespace PhapClinicX.Models.ViewModels
{
    public class AccountDashboardViewModel
    {
        public User User { get; set; } = null!;
        public List<DoctorAppointment> UpcomingDoctorAppointments { get; set; } = new();
        public List<DoctorAppointment> PastDoctorAppointments { get; set; } = new();
    }
}
