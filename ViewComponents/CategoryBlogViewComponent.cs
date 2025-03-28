using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using System.Linq;
using System.Threading.Tasks;

public class CategoryBlogViewComponent : ViewComponent
{
    private readonly ClinicManagementContext _context;

    public CategoryBlogViewComponent(ClinicManagementContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _context.BlogCategories.Where(p=>p.IsActive ==true)
            .ToListAsync();

        return View(categories);
    }
}
