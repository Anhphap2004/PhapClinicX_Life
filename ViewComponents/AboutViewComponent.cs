using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.ViewComponents
{
   
    public class AboutViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;
        public AboutViewComponent(ClinicManagementContext context)
        { 
            
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var about = await _context.Abouts.ToListAsync();
            return View(about);
        }
    }
}
