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
           
            var blog = await _context.Blogs.Include(p=>p.Author).Where(p => p.IsActive == true).FirstOrDefaultAsync(m => m.BlogId == id);

            if (blog == null)
            {
                return NotFound(); // Nếu không tìm thấy bài viết
            }
            blog.Viewcount++;
            _context.Update(blog);
            await _context.SaveChangesAsync();
            ViewBag.RelatedBlogs = _context.Blogs.Include(p=>p.Category).Where(p => p.IsActive == true && p.BlogId != id && p.CategoryId == blog.CategoryId).OrderByDescending(p => p.CreatedAt).Take(4).ToList();
            ViewBag.BlogCategories = _context.BlogCategories
    .Select(c => new CategoryWithCountViewModel
    {
        CategoryId = c.CategoryId,
        CategoryName = c.CategoryName,
        BlogCount = _context.Blogs.Count(b => b.CategoryId == c.CategoryId && b.IsActive == true)
    })
    .ToList() ?? new List<CategoryWithCountViewModel>(); // Tránh null



            ViewBag.BlogId = id;
            return View(blog);

        }
    }
}
