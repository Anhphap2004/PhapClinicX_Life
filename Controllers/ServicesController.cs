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
        //[Route("goi-dich-vu")
        public async Task<IActionResult> Index(int? categoryId)
        {
            // Lấy tất cả danh mục dịch vụ
            ViewBag.Servicecategories = await _context.ServiceCategories.ToListAsync();

            // Nếu categoryId có giá trị, lọc các ServicePackage theo CategoryId, nếu không thì lấy tất cả gói dịch vụ
            ViewBag.ServicePackages = await _context.ServicePackages
                .Where(p => (!categoryId.HasValue || p.CategoryId == categoryId.Value) && p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();

            return View();
        }


        [Route("khuyen-mai")]
        public async Task<IActionResult> KhuyenMai()
        {
            var firstVoucher = await _context.Discounts
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.StartDate) // Lấy voucher mới nhất
                .FirstOrDefaultAsync(); // Lấy 1 voucher đầu tiên

            ViewBag.voucher = firstVoucher != null ? new List<Discount> { firstVoucher } : new List<Discount>();

            ViewBag.AllVoucher = await _context.Discounts
                .Where(p => p.IsActive && (firstVoucher == null || p.DiscountId != firstVoucher.DiscountId)) // Loại bỏ voucher đầu tiên
                .OrderByDescending(p => p.StartDate) // Giữ nguyên sắp xếp mới nhất lên đầu
                .ToListAsync();

            return View();
        }

        [Route("/khuyen-mai/{id}")]
        public async Task<IActionResult> DetailVoucher(int? id)
        {
            if (id == null || _context.Discounts == null) // Sửa lại `||` thay vì `|`
            {
                return NotFound();
            }

            var voucher = await _context.Discounts
                .FirstOrDefaultAsync(p => p.DiscountId == id); // Tối ưu hóa điều kiện lọc

            if (voucher == null)
            {
                return NotFound();
            }
            ViewBag.voucher = _context.Discounts
                .Where(p => p.IsActive && p.DiscountId != id) // Loại bỏ voucher hiện tại
                .OrderByDescending(p => p.StartDate) // Giữ nguyên sắp xếp mới nhất lên đầu
                .Take(3) // Lấy 3 voucher
                .ToList();
            return View(voucher);
        }

        [Route("/servicepackage/{id}")]
        public async Task<IActionResult> ServicePackageDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var servicepackage = await _context.ServicePackages
                .FirstOrDefaultAsync(p => p.PackageId == id);
            if (servicepackage == null)
            {
                return NotFound();
            }
            ViewBag.servicepackagenew = _context.ServicePackages
                .Where(p => p.IsActive && p.PackageId != id)
                .OrderByDescending(p => p.Date)
                .Take(3)
                .ToList();
            return View(servicepackage);
        }

    }
}
