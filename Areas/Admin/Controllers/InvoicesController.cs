using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PhapClinicX.Models;
using PhapClinicX.Models.ViewModels;
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

            return View(invoice);
        }


        // POST: Admin/Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("UserId,TotalAmount,Status,CreatedAt,PhongKhamId,InvoiceType,Method,DiscountAmount,DiscountCode,DiscountId")] Invoice invoice,
        {
            var selectedPackageQuantities = BuildSelectionDictionary(packageIds, packageQuantities);
            var selectedProductQuantities = BuildSelectionDictionary(productIds, productQuantities);

            var invoiceDetails = new List<InvoiceDetail>();
            decimal totalAmount = 0;

            totalAmount += await AppendPackageDetails(packageIds, packageQuantities, invoiceDetails);
            totalAmount += await AppendProductDetails(productIds, productQuantities, invoiceDetails);

            if (!invoiceDetails.Any())
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất một gói dịch vụ hoặc sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                invoice.TotalAmount = totalAmount;
                invoice.CreatedAt ??= DateTime.Now;
                // Lưu hóa đơn vào database
                _context.Add(invoice);
                await _context.SaveChangesAsync();

                {

                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(invoice);
        }


        // GET: Admin/Invoices/Edit/5
        // GET: Admin/Invoices/Edit/5
        // GET: Admin/Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            if (invoice == null) return NotFound();




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
        {
            if (id != invoice.InvoiceId) return NotFound();

            var selectedPackageQuantities = BuildSelectionDictionary(packageIds, packageQuantities);
            var selectedProductQuantities = BuildSelectionDictionary(productIds, productQuantities);

            if (ModelState.IsValid)
            {
                try
                {



                            {
                                }
                    {
                        {
                            await _context.SaveChangesAsync();
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

        private void LoadSelections(object? selectedUserId, object? selectedPhongKhamId,
            Dictionary<int, int> selectedPackages,
            Dictionary<int, int> selectedProducts)
        {
            var users = _context.Users.ToList();
            ViewData["UserId"] = new SelectList(users, "UserId", "FullName", selectedUserId);

            var packageGroups = GetPackageGroups();
            var productGroups = GetProductGroups();

            ViewData["PackageGroups"] = packageGroups;
            ViewData["ProductGroups"] = productGroups;
            ViewData["PackageGroupsJson"] = JsonConvert.SerializeObject(packageGroups);
            ViewData["ProductGroupsJson"] = JsonConvert.SerializeObject(productGroups);
            ViewData["SelectedPackageQuantities"] = selectedPackages;
            ViewData["SelectedProductQuantities"] = selectedProducts;

            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", selectedPhongKhamId);
        }

        private List<PackageSelectionGroup> GetPackageGroups()
        {
            return _context.ServicePackages
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.PackageId,
                    p.PackageName,
                    p.Price,
                    CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chưa phân loại"
                })
                .AsEnumerable()
                .GroupBy(p => new { p.CategoryId, p.CategoryName })
                .Select(g => new PackageSelectionGroup
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName ?? "Chưa phân loại",
                    Packages = g.Select(p => new PackageSelectionItem
                    {
                        PackageId = p.PackageId,
                        PackageName = p.PackageName ?? string.Empty,
                        Price = p.Price ?? 0
                    })
                    .OrderBy(p => p.PackageName)
                    .ToList()
                })
                .OrderBy(g => g.CategoryName)
                .ToList();
        }

        private List<ProductSelectionGroup> GetProductGroups()
        {
            return _context.Products
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    Price = p.PriceSale ?? p.Price,
                    CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category != null ? p.Category.CategoryName : "Chưa phân loại"
                })
                .AsEnumerable()
                .GroupBy(p => new { p.CategoryId, p.CategoryName })
                .Select(g => new ProductSelectionGroup
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName ?? "Chưa phân loại",
                    Products = g.Select(p => new ProductSelectionItem
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName ?? string.Empty,
                        Price = p.Price
                    })
                    .OrderBy(p => p.ProductName)
                    .ToList()
                })
                .OrderBy(g => g.CategoryName)
                .ToList();
        }

        private Dictionary<int, int> BuildSelectionDictionary(int[]? ids, int[]? quantities)
        {
            var result = new Dictionary<int, int>();

            if (ids == null || ids.Length == 0) return result;

            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var qty = (quantities != null && quantities.Length > i) ? quantities[i] : 1;
                if (qty <= 0) qty = 1;

                result[id] = qty;
            }

            return result;
        }

        private async Task<decimal> AppendPackageDetails(int[]? packageIds, int[]? packageQuantities, List<InvoiceDetail> details)
        {
            if (packageIds == null || packageIds.Length == 0) return 0;

            var total = 0m;
            var packageIdList = packageIds.Distinct().ToList();
            var packages = await _context.ServicePackages
                .Where(p => packageIdList.Contains(p.PackageId))
                .ToListAsync();

            for (var i = 0; i < packageIds.Length; i++)
            {
                var packageId = packageIds[i];
                var quantity = (packageQuantities != null && packageQuantities.Length > i) ? packageQuantities[i] : 1;
                if (quantity <= 0) quantity = 1;

                var package = packages.FirstOrDefault(p => p.PackageId == packageId);
                if (package == null) continue;

                var unitPrice = package.Price ?? 0;
                total += unitPrice * quantity;

                details.Add(new InvoiceDetail
                {
                    PackageId = package.PackageId,
                    ProductId = null,
                    Quantity = quantity,
                    Price = unitPrice
                });
            }

            return total;
        }

        private async Task<decimal> AppendProductDetails(int[]? productIds, int[]? productQuantities, List<InvoiceDetail> details)
        {
            if (productIds == null || productIds.Length == 0) return 0;

            var total = 0m;
            var productIdList = productIds.Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIdList.Contains(p.ProductId) && p.IsActive)
                .ToListAsync();

            for (var i = 0; i < productIds.Length; i++)
            {
                var productId = productIds[i];
                var quantity = (productQuantities != null && productQuantities.Length > i) ? productQuantities[i] : 1;
                if (quantity <= 0) quantity = 1;

                var product = products.FirstOrDefault(p => p.ProductId == productId);
                if (product == null) continue;

                var unitPrice = product.PriceSale ?? product.Price;
                total += unitPrice * quantity;

                details.Add(new InvoiceDetail
                {
                    ProductId = product.ProductId,
                    PackageId = null,
                    Quantity = quantity,
                    Price = unitPrice
                });
            }

            return total;
        }
    }
}
