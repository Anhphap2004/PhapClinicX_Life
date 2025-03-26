using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;

namespace PhapClinicX.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogsController : Controller
    {
        private readonly ClinicManagementContext _context;

        public BlogsController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/Blogs
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.Blogs.Include(b => b.Author).Include(b => b.Category);
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Vui lòng chọn ảnh hợp lệ!");
            }

            // Tạo thư mục nếu chưa tồn tại
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/blogs");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Tạo tên file duy nhất
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn ảnh để lưu vào database
            string imagePath = uniqueFileName;
            return Ok(imagePath);
        }

        // GET: Admin/Blogs/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "FullName");
            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Admin/Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,CategoryId,Title,Content,Image,AuthorId,ExtraContent,CreatedAt,IsActive,Viewcount")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.CreatedAt = DateTime.Now;

                // Ảnh đã được upload trước đó → chỉ lấy đường dẫn từ input
                if (string.IsNullOrEmpty(blog.Image))
                {
                    blog.Image = "/assets/img/default.jpg"; // Ảnh mặc định nếu không chọn ảnh
                }
                blog.IsActive = true;
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "FullName", blog.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "CategoryId", "CategoryName", blog.CategoryId);
            return View(blog);
        }



        // GET: Admin/Blogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "FullName", blog.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "CategoryId", "CategoryName", blog.CategoryId);
            return View(blog);
        }

        // POST: Admin/Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BlogId,CategoryId,Title,Content,Image,AuthorId,ExtraContent,CreatedAt,IsActive,Viewcount")] Blog blog)
        {
            if (id != blog.BlogId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(b => b.BlogId == id);
                    if (existingBlog == null) return NotFound(); // Nếu không tìm thấy bài viết, trả về lỗi 404

                    // Nếu CreatedAt bị null thì giữ giá trị cũ
                    blog.CreatedAt ??= existingBlog.CreatedAt;

                    // Nếu Viewcount bị null thì giữ giá trị cũ
                    blog.Viewcount ??= existingBlog.Viewcount;
                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.BlogId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "UserId", "FullName", blog.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.BlogCategories, "CategoryId", "CategoryName", blog.CategoryId);
            return View(blog);
        }

        // GET: Admin/Blogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BlogId == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Admin/Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.BlogId == id);
        }
    }
}
