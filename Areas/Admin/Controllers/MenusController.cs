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
    public class MenusController : Controller
    {
        private readonly ClinicManagementContext _context;

        public MenusController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/Menus
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.Menus.Include(m => m.Parent);
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/Menus/Create
        // GET: Admin/Menus/Create
        public IActionResult Create()
        {
            var menus = _context.Menus
                .Select(m => new SelectListItem { Value = m.MenuId.ToString(), Text = m.MenuName })
                .ToList();

            // Thêm lựa chọn mặc định "Không có menu cha" với giá trị 0
            menus.Insert(0, new SelectListItem { Value = "0", Text = "Không có menu cha", Selected = true });

            ViewBag.ParentId = new SelectList(menus, "Value", "Text");

            return View();
        }




        // POST: Admin/Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuId,MenuName,Url,Position,ParentId,IsActive")] Menu menu)
        {
            if (menu.ParentId == 0) // Nếu chọn "Không có menu cha"
            {
                menu.ParentId = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var menuList = _context.Menus
                .Select(m => new { m.MenuId, m.MenuName })
                .ToList();

            ViewData["ParentId"] = new SelectList(menuList, "MenuId", "MenuName", menu.ParentId);

            return View(menu);
        }


        // GET: Admin/Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            var menus = _context.Menus
               .Select(m => new SelectListItem { Value = m.MenuId.ToString(), Text = m.MenuName })
               .ToList();

            // Thêm lựa chọn mặc định "Không có menu cha" với giá trị 0
            menus.Insert(0, new SelectListItem { Value = "0", Text = "Không có menu cha", Selected = true });

            ViewBag.ParentId = new SelectList(menus, "Value", "Text");

            return View(menu);
        }

        // POST: Admin/Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuId,MenuName,Url,Position,ParentId,IsActive")] Menu menu)
        {
            if (id != menu.MenuId)
            {
                return NotFound();
            }
            if (menu.ParentId == 0)
            {
                menu.ParentId = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.MenuId))
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

            var menuList = _context.Menus
    .Select(m => new { m.MenuId, m.MenuName })
    .ToList();

            ViewData["ParentId"] = new SelectList(menuList, "MenuId", "MenuName", menu.ParentId);
           
            return View(menu);
        }

        // GET: Admin/Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = await _context.Menus
                .Include(m => m.Parent)
                .FirstOrDefaultAsync(m => m.MenuId == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Admin/Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return _context.Menus.Any(e => e.MenuId == id);
        }
    }
}
