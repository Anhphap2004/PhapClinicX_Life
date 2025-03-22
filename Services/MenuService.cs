using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PhapClinicX.Models;

public class MenuService : IMenuService
{
    private readonly ClinicManagementContext _context;

    public MenuService(ClinicManagementContext context)
    {
        _context = context;
    }

    public async Task<List<Menu>> GetActiveMenusAsync()
    {
        return await _context.Menus
            .Where(m => m.IsActive == true)
            .Include(m => m.InverseParent) // Load menu con
            .OrderBy(m => m.Position)
            .ToListAsync();
    }
}
