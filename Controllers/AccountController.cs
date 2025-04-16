using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClinicManagementContext _context;
        public AccountController(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.Where(p=>p.IsActive).FirstOrDefaultAsync(m => m.UserId == id);
            return View(user);
        }
    
    }
}
