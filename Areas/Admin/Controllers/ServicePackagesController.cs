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
    public class ServicePackagesController : Controller
    {
        private readonly ClinicManagementContext _context;

        public ServicePackagesController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/ServicePackages
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServicePackages.ToListAsync());
        }

        // GET: Admin/ServicePackages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePackage = await _context.ServicePackages
                .FirstOrDefaultAsync(m => m.PackageId == id);
            if (servicePackage == null)
            {
                return NotFound();
            }

            return View(servicePackage);
        }

        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Vui lòng chọn ảnh hợp lệ!");
            }

            // Tạo thư mục nếu chưa tồn tại
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/img/ServicePackages");
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

        // GET: Admin/ServicePackages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ServicePackages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PackageId,PackageName,Price,IsActive,Image,Title,Date")] ServicePackage servicePackage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicePackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicePackage);
        }

        // GET: Admin/ServicePackages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePackage = await _context.ServicePackages.FindAsync(id);
            if (servicePackage == null)
            {
                return NotFound();
            }
            return View(servicePackage);
        }

        // POST: Admin/ServicePackages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PackageId,PackageName,Price,IsActive,Image,Title,Date")] ServicePackage servicePackage)
        {
            if (id != servicePackage.PackageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicePackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicePackageExists(servicePackage.PackageId))
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
            return View(servicePackage);
        }

        // GET: Admin/ServicePackages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePackage = await _context.ServicePackages
                .FirstOrDefaultAsync(m => m.PackageId == id);
            if (servicePackage == null)
            {
                return NotFound();
            }

            return View(servicePackage);
        }

        // POST: Admin/ServicePackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicePackage = await _context.ServicePackages.FindAsync(id);
            if (servicePackage != null)
            {
                _context.ServicePackages.Remove(servicePackage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicePackageExists(int id)
        {
            return _context.ServicePackages.Any(e => e.PackageId == id);
        }
    }
}
