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
    }
}
