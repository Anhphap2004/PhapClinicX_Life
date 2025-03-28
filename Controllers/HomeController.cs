using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;
using System.Diagnostics;
using System.Net.NetworkInformation;


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
            ViewBag.Faqs = _context.Faqs.ToList();
            ViewBag.discount = _context.Discounts.Where(p => p.IsActive == true).OrderBy(p=>p.StartDate).ToList();
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
