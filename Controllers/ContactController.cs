using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class ContactController : Controller
    {
        private readonly ClinicManagementContext _context;
        public ContactController(ClinicManagementContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Send(string fullname, string email, string message, int phone)
        {
            
           var contact = new Contact()
           {
               Fullname = fullname,
               Email = email,
               Message = message,
               Phone = phone
           };


            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Gửi Liên Hệ thành công!";
            return RedirectToAction("Index"); 
            
        }
    }
}
