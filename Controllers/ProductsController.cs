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
            var Products = _context.Products.Where(p=>p.IsActive == true).ToList();
            return View(Products);
        }
    }
}
