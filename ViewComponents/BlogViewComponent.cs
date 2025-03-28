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

        public async Task<IViewComponentResult> InvokeAsync(int? categoryID = null)
        {
            var blogs = await _context.Blogs
                .Include(p => p.Category)
                .Include(p => p.BlogComments)
                .Include(p => p.Author)
                .Where(p => p.IsActive == true && (categoryID == null || p.CategoryId == categoryID))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(blogs);
        }





    }
}
