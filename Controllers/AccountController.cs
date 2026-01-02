using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using PhapClinicX.Models.ViewModels;
namespace PhapClinicX.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClinicManagementContext _context;
        public AccountController(ClinicManagementContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([Bind(Prefix = "User")] User user)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn" });
                }

                var existingUser = await _context.Users.FindAsync(userId);
                if (existingUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Cập nhật các thông tin
                existingUser.FullName = user.FullName;
                existingUser.Username = user.Username;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.Address = user.Address;

                _context.Update(existingUser);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Giữ nguyên action Index hiện tại
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || _context.Users == null)
            {
                return NotFound();
            }
        .Include(p => p.Doctor)
            .ThenInclude(p => p.PhongKham)
        .ToListAsync();
        }

        //public async Task<IActionResult> Index()
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null || _context.Users == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.Users.Where(p=>p.IsActive == true).FirstOrDefaultAsync(m => m.UserId == userId);
        //    ViewBag.History = await _context.DoctorAppointments
        //.Include(p => p.Doctor)
        //    .ThenInclude(p => p.PhongKham)
        //.Where(p => p.Status == true && p.UserId == userId)
        //.ToListAsync();
        //    return View(user);
        //}

        public async Task<IActionResult> ViewInvoices()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 
            if (userId == null)
            {
                return RedirectToAction("Index", "Login"); 
            }

            var invoices = await _context.Invoices
                .Include(i => i.InvoiceDetails)
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
