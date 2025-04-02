using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;

namespace PhapClinicX.Controllers
{
    public class AboutController : Controller
    {
        private readonly ClinicManagementContext _context;
        public AboutController(ClinicManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.Faqs = _context.Faqs.ToList();
            return View();
        }
        [Route("Chi-Nhanh")]
        public IActionResult Branch()
        {
            return View();
        }

        [Route("Chi-Tiet-Phong-Kham/{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var phongKham = await _context.PhongKhams
                .FirstOrDefaultAsync(p => p.PhongKhamId == id && p.Isactive); 

            if (phongKham == null)
            {
                return NotFound(); 
            }

            return View(phongKham); 
        }

    }
}
