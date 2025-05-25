using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using PhapClinicX.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhapClinicX.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RevenuesController : Controller
    {
        private readonly ClinicManagementContext _context;

        public RevenuesController(ClinicManagementContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> DoanhThuTheoPhongKham(DateOnly? startDate, DateOnly? endDate)
        {
            var query = _context.Revenues.AsQueryable();

            if (startDate != null)
                query = query.Where(r => r.RevenueDate >= startDate);

            if (endDate != null)
                query = query.Where(r => r.RevenueDate <= endDate);

            var result = await query
                .Where(r => r.PhongKhamId != null && r.TotalRevenue != null && r.Product != null && r.Product.ProductSold != null)
                .GroupBy(r => new
                {
                    r.PhongKhamId,
                    TenPhongKham = r.PhongKham.TenPhongKham
                })
                .Select(g => new DoanhThuPhongKhamViewModel
                {
                    PhongKhamId = g.Key.PhongKhamId,
                    TenPhongKham = g.Key.TenPhongKham,
                    TongDoanhThu = g.Sum(r => r.TotalRevenue),
                    TongSanPhamBanDuoc = g.Sum(r => r.Product.ProductSold ?? 0)
                })
                .ToListAsync();

            return View(result);
        }




    }
}
