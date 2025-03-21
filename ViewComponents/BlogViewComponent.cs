using PhapClinicX.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace PhapClinicX.ViewComponents
{
    public class BlogViewComponent : ViewComponent
    {
        private readonly ClinicManagementContext _context;

        public BlogViewComponent(ClinicManagementContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? categoryID)
        {

            // Truy vấn dữ liệu từ database
            var blogs =  await _context.Blogs
                .Include(p => p.BlogComments) // Nạp bình luận
                .Include(p => p.Author) // Nạp thông tin người viết bài
                .Where(p => p.IsActive == true && p.CategoryId == categoryID)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(); // Lấy danh sách bài viết

            // Nếu không có bài viết nào, hiển thị thông báo
            if (blogs == null || !blogs.Any())
            {
                return Content("Không tìm thấy bài viết nào.");
            }

            return View(blogs);
        }



    }
}
