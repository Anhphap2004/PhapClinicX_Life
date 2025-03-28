using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace PhapClinicX.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ClinicManagementContext _context;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ClinicManagementContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            var Products = _context.Products.Where(p => p.IsActive == true).ToList();
            return View(Products);
        }

        [Route("/san-pham/{alias}-{id}.html")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Category = await _context.ProductCategories
                .Where(p => p.IsActive == true && p.CategoryId == product.CategoryId)
                .ToListAsync(); // Truy vấn bất đồng bộ tốt hơn

            ViewBag.RelatedProducts = await _context.Products
                .Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.ProductId != product.ProductId)
                .ToListAsync(); // Thêm điều kiện tránh lặp chính nó
            return View(product);
        }
    }
}
