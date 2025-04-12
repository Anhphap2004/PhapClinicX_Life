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
    public class PhongKhamsController : Controller
    {
        private readonly ClinicManagementContext _context;

        public PhongKhamsController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/PhongKhams
        public async Task<IActionResult> Index()
        {
            return View(await _context.PhongKhams.ToListAsync());
        }

        // GET: Admin/PhongKhams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Đợi truy vấn lấy danh sách appointment hoàn tất trước khi đi tiếp
            var appointments = await _context.ClinicAppointments
                .Include(c => c.PhongKham)
                .Where(p => p.PhongKhamId == id)
                .ToListAsync();

            var phongKham = await _context.PhongKhams
                .FirstOrDefaultAsync(m => m.PhongKhamId == id);

            if (phongKham == null)
            {
                return NotFound();
            }
            ViewBag.doctor = await _context.DoctorProfiles.Include(p => p.PhongKham).Where(p => p.PhongKhamId == id).ToListAsync();
            ViewBag.clinicManagementContext = appointments;
            return View(phongKham);
        }


        // GET: Admin/PhongKhams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PhongKhams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhongKhamId,TenPhongKham,DiaChi,SoDienThoai,Email,NgayThanhLap,Image,Isactive,Introduce")] PhongKham phongKham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phongKham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phongKham);
        }

        // GET: Admin/PhongKhams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongKham = await _context.PhongKhams.FindAsync(id);
            if (phongKham == null)
            {
                return NotFound();
            }
            return View(phongKham);
        }

        // POST: Admin/PhongKhams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhongKhamId,TenPhongKham,DiaChi,SoDienThoai,Email,NgayThanhLap,Image,Isactive,Introduce")] PhongKham phongKham)
        {
            if (id != phongKham.PhongKhamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phongKham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongKhamExists(phongKham.PhongKhamId))
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
            return View(phongKham);
        }

        // GET: Admin/PhongKhams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phongKham = await _context.PhongKhams
                .FirstOrDefaultAsync(m => m.PhongKhamId == id);
            if (phongKham == null)
            {
                return NotFound();
            }

            return View(phongKham);
        }

        // POST: Admin/PhongKhams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phongKham = await _context.PhongKhams.FindAsync(id);
            if (phongKham != null)
            {
                _context.PhongKhams.Remove(phongKham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongKhamExists(int id)
        {
            return _context.PhongKhams.Any(e => e.PhongKhamId == id);
        }
    }
}
