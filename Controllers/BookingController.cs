using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class BookingController : Controller
    {
        private readonly ClinicManagementContext _context;
        public BookingController(ClinicManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
