using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;

namespace PhapClinicX.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClinicManagementContext _context;
        public HomeController(ClinicManagementContext context)
        {
            _context = context;
        }
        [Area("Admin")]
        public async Task<IActionResult> Index()
        {
            var totalSales = await _context.Revenues.SumAsync(i=>i.TotalRevenue);
            var totalCost = await _context.Products.SumAsync(p => p.Price * p.ProductSold); 
            var productSold = await _context.Products.SumAsync(p => p.ProductSold);
         

            ViewBag.TotalSales = totalSales;
            ViewBag.TotalCost = totalCost;
            ViewBag.ProductSold = productSold;

            return View();
        }

       

    }
}
