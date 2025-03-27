using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ClinicManagementContext _context;
        public ServicesController(ClinicManagementContext context)
        {
            _context = context;
        } 
        //[Route("goi-dich-vu")]
        public IActionResult Index()
        {
            return View();
        }
      
        
        //[Route("khuyen-mai")]
        public IActionResult KhuyenMai()
        {
            var firstVoucher = _context.Discounts
     .Where(p => p.IsActive)
     .OrderByDescending(p => p.StartDate) // Lấy voucher mới nhất
     .FirstOrDefault(); // Lấy 1 voucher đầu tiên

            ViewBag.voucher = firstVoucher != null ? new List<Discount> { firstVoucher } : new List<Discount>();

            ViewBag.AllVoucher = _context.Discounts
                .Where(p => p.IsActive && (firstVoucher == null || p.DiscountId != firstVoucher.DiscountId)) // Loại bỏ voucher đầu tiên
                .OrderByDescending(p => p.StartDate) // Giữ nguyên sắp xếp mới nhất lên đầu
                .ToList();


            return View();
        }
    }
}
