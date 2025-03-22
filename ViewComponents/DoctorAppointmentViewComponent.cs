using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.ViewComponents
{
    public class DoctorAppointment : ViewComponent
    {
        private readonly ClinicManagementContext _context;
        public DoctorAppointment(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var doctorpoint = await _context.DoctorProfiles.Where(p => p.Isactive == true).ToListAsync();
            return View(doctorpoint);
        }

    }
}
