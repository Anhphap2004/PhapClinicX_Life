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

        [Route("/blog/{id}.html")]
        public async Task <IActionResult> Detail(int? id)
        {
            if(id == null || _context.Blogs ==null)
            {
                return NotFound();
            }
            ViewBag.BlogId = id;
            var blog = await _context.Blogs.Include(p=>p.Author).Where(p => p.IsActive == true).FirstOrDefaultAsync(m => m.BlogId == id);
            return View(blog);

        }
    }
}
