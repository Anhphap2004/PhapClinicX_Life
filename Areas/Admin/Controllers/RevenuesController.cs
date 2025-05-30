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
            var query = _context.Revenues
                .Include(r => r.PhongKham)
                .AsQueryable();

            if (startDate != null)
                query = query.Where(r => r.RevenueDate >= startDate);

            if (endDate != null)
                query = query.Where(r => r.RevenueDate <= endDate);

            var result = await query
                .Where(r => r.PhongKhamId != null && r.TotalRevenue != null && r.Quantity != null)
                .GroupBy(r => new
                {
                    r.PhongKhamId,
                    TenPhongKham = r.PhongKham.TenPhongKham
                })
                .Select(g => new DoanhThuPhongKhamViewModel
                {
                    PhongKhamId = g.Key.PhongKhamId,
                    TenPhongKham = g.Key.TenPhongKham,
                    TongDoanhThu = g.Sum(r => r.TotalRevenue ?? 0),
                    TongSanPhamBanDuoc = g.Sum(r => r.Quantity ?? 0)
                })
                .ToListAsync();

            return View(result);
        }
        public async Task<IActionResult> RevenueProduct(DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.InvoiceDetails
                .Include(x => x.Product)
                .Include(x => x.Invoice)
                    .ThenInclude(i => i.PhongKham)
                .Where(x => x.ProductId != null && x.Invoice != null);

            // Lọc theo khoảng ngày nếu có
            if (tuNgay.HasValue)
                query = query.Where(x => x.Invoice!.CreatedAt >= tuNgay.Value);

            if (denNgay.HasValue)
                query = query.Where(x => x.Invoice!.CreatedAt <= denNgay.Value);

            // Truy vấn dữ liệu đã gộp đầy đủ
            var data = await query
                .Select(x => new SanPhamDaBanViewModel
                {
                    NgayBan = x.Invoice!.CreatedAt ?? DateTime.MinValue,
                    TenSanPham = x.Product!.ProductName ?? "Chưa có tên",
                    TenPhongKham = x.Invoice.PhongKham!.TenPhongKham ?? "Không xác định",
                    SoLuong = x.Quantity ?? 0,
                    GiaGoc = x.Product.Price,
                    GiaBan = x.Price ?? 0,
                    Image = x.Product.Image,
                    TongTien = (x.Quantity ?? 0) * (x.Price ?? 0)
                })
                .OrderByDescending(x => x.NgayBan)
                .ToListAsync();

            return View(data);
        }





    }
}
