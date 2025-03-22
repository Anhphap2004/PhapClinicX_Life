using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;

namespace PhapClinicX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClinicManagementContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ClinicManagementContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.doctor = _context.DoctorProfiles.Where(p=> p.Isactive == true).ToList();
            ViewBag.blogcategories = _context.BlogCategories.Where(p=>p.IsActive==true).ToList();
            ViewBag.Faqs = _context.Faqs.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
