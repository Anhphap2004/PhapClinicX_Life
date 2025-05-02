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
            var user = await _context.Users.Where(p=>p.IsActive == true).FirstOrDefaultAsync(m => m.UserId == id);
            return View(user);
        }

        public async Task<IActionResult> ViewInvoices()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 
            if (userId == null)
            {
                return RedirectToAction("Index", "Login"); 
            }

            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product).Include(d=>d.InvoiceDetails).ThenInclude(d=>d.Package)
                .Where(i => i.UserId == userId) 
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return View(invoices);
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var invoice = await _context.Invoices
                .Include(i => i.User).Include(i=>i.PhongKham) // ✨ Thêm dòng này nè!
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product).Include(d => d.InvoiceDetails).ThenInclude(d => d.Package)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.UserId == userId);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }
    }
}
