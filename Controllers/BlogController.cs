using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace PhapClinicX.Controllers
{
    public class BlogController : Controller
    {
        private readonly ClinicManagementContext _context;
        public BlogController(ClinicManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId, int page = 1)
        {
            int pageSize = 8;

            // Query ban đầu
            var query = _context.Blogs.Include(p => p.Author).AsQueryable();

            // Nếu có lọc theo category
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            int totalItems = query.Count();

            // Phân trang nè
            var blogPosts = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gửi sang view
            ViewBag.blogPosts = blogPosts;
            ViewBag.blogcategories = _context.BlogCategories.Where(p => p.IsActive == true).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

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
            var RandomDiscount = await _context.Discounts
                .Where(d => d.IsActive == true && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                .OrderBy(r => Guid.NewGuid()) // Random
                .FirstOrDefaultAsync();
            if (RandomDiscount == null)
            {
                ViewBag.Discount = 0;
            }
            else
            {
                ViewBag.Discount = RandomDiscount;
            }
            ViewBag.blogComments = await _context.BlogComments
      .Include(p => p.User) // Lấy thông tin User
      .Where(p => p.BlogId == id)
      .OrderBy(p => p.CreatedAt)
      .ToListAsync();


            ViewBag.BlogId = id;
            return View(blog);

        }


        [HttpPost]
        public async Task<IActionResult> Comment(int BlogId, string Comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Kiểm tra BlogId có hợp lệ không
            var blogExists = await _context.Blogs.AnyAsync(b => b.BlogId == BlogId);
            if (!blogExists)
            {
                return BadRequest("Bài viết không tồn tại."); // Tránh lỗi khóa ngoại
            }

            var newComment = new BlogComment
            {
                BlogId = BlogId,
                UserId = userId.Value,  // Đảm bảo UserId không null
                Comment = Comment,
                CreatedAt = DateTime.Now
            };

            _context.BlogComments.Add(newComment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detail", "Blog", new { id = BlogId });
        }

    }
}
