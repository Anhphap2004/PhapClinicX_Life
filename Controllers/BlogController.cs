using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;
using Microsoft.EntityFrameworkCore;
namespace PhapClinicX.Controllers
{
    public class BlogController : Controller
    {
        private readonly ClinicManagementContext _context;
        public BlogController(ClinicManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.blogcategories = _context.BlogCategories.Where(p => p.IsActive == true).ToList();
            return View();
        }
    }
}
