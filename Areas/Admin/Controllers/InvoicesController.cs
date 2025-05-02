using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhapClinicX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhapClinicX.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InvoicesController : Controller
    {
        private readonly ClinicManagementContext _context;

        public InvoicesController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/Invoices
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.Invoices.Include(i => i.PhongKham).Include(i => i.User);
            return View(await clinicManagementContext.ToListAsync());
        }
        // GET: Admin/Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.PhongKham)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Package)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }



        // GET: Admin/Invoices/Create
        public IActionResult Create()
        {
            // Lấy danh sách người dùng từ bảng Users
            var users = _context.Users
                .ToList();

            // Truyền danh sách người dùng vào ViewData để hiển thị trong dropdown
            ViewData["UserId"] = new SelectList(users, "UserId", "FullName");

            // Lấy danh sách các gói dịch vụ
            var packages = _context.ServicePackages
                .Where(p => p.IsActive)
                .Select(p => new {
                    packageId = p.PackageId,
                    packageName = p.PackageName,
                    price = p.Price
                }).ToList();

            // Truyền danh sách gói dịch vụ vào ViewData
            ViewData["Packages"] = packages;
            ViewData["PackageId"] = new SelectList(packages, "packageId", "packageName");
            // Lưu trữ danh sách gói dịch vụ dưới dạng JSON để có thể dùng trong JavaScript
            ViewData["PackagesJson"] = JsonConvert.SerializeObject(packages);

            // Lấy danh sách các phòng khám
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham");

            // Khởi tạo đối tượng hóa đơn
            var invoice = new Invoice();

            return View(invoice);
        }


        // POST: Admin/Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserId,TotalAmount,Status,CreatedAt,PhongKhamId,InvoiceType,Method,DiscountAmount,DiscountCode,DiscountId")] Invoice invoice,
            int? PackageId)
        {
            if (ModelState.IsValid)
            {
                // Lưu hóa đơn vào database
                _context.Add(invoice);
                await _context.SaveChangesAsync();

                // Nếu có chọn gói dịch vụ, tạo luôn chi tiết hóa đơn
                if (PackageId.HasValue)
                {
                    var package = await _context.ServicePackages.FindAsync(PackageId.Value);
                    if (package != null)
                    {
                        var invoiceDetail = new InvoiceDetail
                        {
                            InvoiceId = invoice.InvoiceId,
                            ProductId = null,             // Đây là dịch vụ, không phải sản phẩm
                            PackageId = PackageId.Value,
                            Price = package.Price ?? 0
                        };

                        _context.InvoiceDetails.Add(invoiceDetail);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, load lại dữ liệu dropdowns + JSON
            var users = _context.Users.ToList();
            ViewData["UserId"] = new SelectList(users, "UserId", "FullName", invoice.UserId);

            var packages = _context.ServicePackages
                .Where(p => p.IsActive)
                .Select(p => new {
                    packageId = p.PackageId,
                    packageName = p.PackageName,
                    price = p.Price
                }).ToList();

            ViewData["PackageId"] = new SelectList(packages, "packageId", "packageName");
            ViewData["PackagesJson"] = JsonConvert.SerializeObject(packages);
            ViewData["Packages"] = packages;
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", invoice.PhongKhamId);

            return View(invoice);
        }


        // GET: Admin/Invoices/Edit/5
        // GET: Admin/Invoices/Edit/5
        // GET: Admin/Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return NotFound();

            var users = _context.Users.ToList();
            ViewData["UserId"] = new SelectList(users, "UserId", "FullName", invoice.UserId);

            var packages = _context.ServicePackages
                .Where(p => p.IsActive)
                .Select(p => new {
                    packageId = p.PackageId,
                    packageName = p.PackageName,
                    price = p.Price
                }).ToList();

            ViewData["PackageId"] = new SelectList(packages, "packageId", "packageName");

            // Tìm chi tiết có gói dịch vụ (nếu có)
            var existingPackage = await _context.InvoiceDetails
                .Where(d => d.InvoiceId == id && d.PackageId != null)
                .Select(d => d.PackageId)
                .FirstOrDefaultAsync();

            ViewData["SelectedPackageId"] = existingPackage;
            ViewData["PackagesJson"] = JsonConvert.SerializeObject(packages);
            ViewData["Packages"] = packages;

            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", invoice.PhongKhamId);

            return View(invoice);
        }


        // POST: Admin/Invoices/Edit/5
        // POST: Admin/Invoices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("InvoiceId,UserId,TotalAmount,Status,CreatedAt,PhongKhamId,InvoiceType,Method,DiscountAmount,DiscountCode,DiscountId")] Invoice invoice,
            int? PackageId)
        {
            if (id != invoice.InvoiceId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();

                    // Xử lý gói dịch vụ trong chi tiết
                    var existingDetail = await _context.InvoiceDetails
                        .FirstOrDefaultAsync(d => d.InvoiceId == id && d.PackageId != null);

                    if (existingDetail != null)
                    {
                        // Nếu PackageId khác thì cập nhật
                        if (PackageId != existingDetail.PackageId)
                        {
                            _context.InvoiceDetails.Remove(existingDetail);
                            await _context.SaveChangesAsync();

                            if (PackageId.HasValue)
                            {
                                var package = await _context.ServicePackages.FindAsync(PackageId.Value);
                                if (package != null)
                                {
                                    var newDetail = new InvoiceDetail
                                    {
                                        InvoiceId = id,
                                        PackageId = PackageId.Value,
                                        ProductId = null,
                                        Price = package.Price ?? 0
                                    };
                                    _context.InvoiceDetails.Add(newDetail);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    else if (PackageId.HasValue)
                    {
                        // Nếu chưa có detail mà giờ thêm mới
                        var package = await _context.ServicePackages.FindAsync(PackageId.Value);
                        if (package != null)
                        {
                            var newDetail = new InvoiceDetail
                            {
                                InvoiceId = id,
                                PackageId = PackageId.Value,
                                ProductId = null,
                                Price = package.Price ?? 0
                            };
                            _context.InvoiceDetails.Add(newDetail);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Invoices.Any(e => e.InvoiceId == id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            // Load lại dữ liệu nếu lỗi
            var users = _context.Users.ToList();
            ViewData["UserId"] = new SelectList(users, "UserId", "FullName", invoice.UserId);

            var packages = _context.ServicePackages
                .Where(p => p.IsActive)
                .Select(p => new {
                    packageId = p.PackageId,
                    packageName = p.PackageName,
                    price = p.Price
                }).ToList();

            ViewData["PackageId"] = new SelectList(packages, "packageId", "packageName");
            ViewData["PackagesJson"] = JsonConvert.SerializeObject(packages);
            ViewData["Packages"] = packages;

            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", invoice.PhongKhamId);

            return View(invoice);
        }
        // GET: Admin/Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.PhongKham)
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Admin/Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }
    }
}
