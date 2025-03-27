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
    public class DoctorProfilesController : Controller
    {
        private readonly ClinicManagementContext _context;

        public DoctorProfilesController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/DoctorProfiles
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.DoctorProfiles.Include(d => d.PhongKham);
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/DoctorProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorProfile = await _context.DoctorProfiles
                .Include(d => d.PhongKham)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctorProfile == null)
            {
                return NotFound();
            }

            return View(doctorProfile);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Vui lòng chọn ảnh hợp lệ!");
            }

            // Tạo thư mục nếu chưa tồn tại
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/team");
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

        // GET: Admin/DoctorProfiles/Create
        public IActionResult Create()
        {
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham");
            return View();
        }

        // POST: Admin/DoctorProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,Specialization,Experience,Education,Image,Isactive,Fullname,Phone,PhongKhamId,Introduce,WorkSchedule")] DoctorProfile doctorProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", doctorProfile.PhongKhamId);
            return View(doctorProfile);
        }

        // GET: Admin/DoctorProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorProfile = await _context.DoctorProfiles.FindAsync(id);
            if (doctorProfile == null)
            {
                return NotFound();
            }
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", doctorProfile.PhongKhamId);
            return View(doctorProfile);
        }

        // POST: Admin/DoctorProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,Specialization,Experience,Education,Image,Isactive,Fullname,Phone,PhongKhamId,Introduce,WorkSchedule")] DoctorProfile doctorProfile)
        {
            if (id != doctorProfile.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorProfileExists(doctorProfile.DoctorId))
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
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", doctorProfile.PhongKhamId);
            return View(doctorProfile);
        }

        // GET: Admin/DoctorProfiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorProfile = await _context.DoctorProfiles
                .Include(d => d.PhongKham)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctorProfile == null)
            {
                return NotFound();
            }

            return View(doctorProfile);
        }

        // POST: Admin/DoctorProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorProfile = await _context.DoctorProfiles.FindAsync(id);
            if (doctorProfile != null)
            {
                _context.DoctorProfiles.Remove(doctorProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorProfileExists(int id)
        {
            return _context.DoctorProfiles.Any(e => e.DoctorId == id);
        }
    }
}
