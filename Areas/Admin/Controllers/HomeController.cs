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
            var totalSales = await _context.InvoiceDetails.SumAsync(i => i.Quantity * i.Price);
            var totalCost = await _context.Products.SumAsync(p => p.PriceSale * p.ProductSold); // nếu có cột CostPrice
            var productSold = await _context.Products.SumAsync(p => p.ProductSold);
            var topProducts = await _context.Products
                .OrderByDescending(p => p.ProductSold)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalSales = totalSales;
            ViewBag.TotalCost = totalCost;
            ViewBag.ProductSold = productSold;
            ViewBag.TopProducts = topProducts;

            return View();
        }
    }
}
