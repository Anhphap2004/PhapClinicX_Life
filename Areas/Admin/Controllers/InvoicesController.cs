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
                .Include(i => i.PhongKham)
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails) // Thêm Include này để lấy chi tiết hoá đơn luôn nè!
                    .ThenInclude(d => d.Product) // Nếu có sản phẩm trong chi tiết hoá đơn
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }


        // GET: Admin/Invoices/Create
        public IActionResult Create()
        {
            // Giả sử bạn đã lưu UserId trong session khi người dùng đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId"); // Hoặc bạn có thể lấy từ Claims nếu dùng Identity
            if (!userId.HasValue)
            {
                // Nếu không có UserId trong session, bạn có thể thông báo lỗi hoặc điều hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Gán giá trị UserId vào đối tượng invoice mới
            var invoice = new Invoice
            {
                UserId = userId.Value
            };

            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", invoice.UserId);

            return View(invoice);
        }

        // POST: Admin/Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,TotalAmount,Status,CreatedAt,PhongKhamId,InvoiceType,Method,DiscountAmount,DiscountCode,DiscountId")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", invoice.PhongKhamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", invoice.UserId);
            return View(invoice);
        }


        // GET: Admin/Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", invoice.PhongKhamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", invoice.UserId);
            return View(invoice);
        }

        // POST: Admin/Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceId,UserId,TotalAmount,Status,CreatedAt,PhongKhamId,InvoiceType,Method,DiscountAmount,DiscountCode,DiscountId")] Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceId))
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
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", invoice.PhongKhamId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", invoice.UserId);
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
