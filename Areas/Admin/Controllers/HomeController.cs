using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;

namespace PhapClinicX.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClinicManagementContext _context;
        public HomeController(ClinicManagementContext context)
        {
            _context = context;
        }
        [Area("Admin")]
        public async Task<IActionResult> Index()
        {
            var totalSales = await _context.InvoiceDetails.SumAsync(i => i.Quantity * i.Price);
            var totalCost = await _context.Products.SumAsync(p => p.PriceSale * p.ProductSold); // nếu có cột CostPrice
            var productSold = await _context.Products.SumAsync(p => p.ProductSold);
            var topProducts = await _context.Products
                .OrderByDescending(p => p.ProductSold)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalSales = totalSales;
            ViewBag.TotalCost = totalCost;
            ViewBag.ProductSold = productSold;
            ViewBag.TopProducts = topProducts;

            return View();
        }

        public class SalesData
        {
            public string? ProductName { get; set; }
            public int QuantitySold { get; set; }
            public decimal TotalAmount { get; set; }
        }


        [Area("Admin")]
        public async Task<IActionResult> RevenueManagement(DateTime? startDate, DateTime? endDate)
        {
            var invoiceQuery = _context.InvoiceDetails.AsQueryable();

            if (startDate.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(i => i.Invoice != null && i.Invoice.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                invoiceQuery = invoiceQuery.Where(i => i.Invoice != null && i.Invoice.CreatedAt >= endDate.Value);
            }

            var salesData = await invoiceQuery
                .Include(i => i.Product)
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductName = g.FirstOrDefault() == null ? "Chưa có tên sản phẩm" : g.FirstOrDefault().Product.ProductName,
                    QuantitySold = g.Sum(i => i.Quantity),
                    TotalAmount = g.Sum(i => i.Quantity * i.Price)
                })
                .OrderByDescending(g => g.TotalAmount)
                .ToListAsync();

            // Sử dụng toán tử null-coalescing để thay thế null bằng 0
            var totalRevenue = salesData.Sum(s => s.TotalAmount ?? 0); // Tổng doanh thu (nếu null thì dùng 0)
            var totalQuantity = salesData.Sum(s => s.QuantitySold ?? 0); // Tổng số lượng bán (nếu null thì dùng 0)

            decimal averageRevenuePerDay = 0;
            decimal maxDailyRevenue = 0;
            decimal minDailyRevenue = 0;

            if (startDate.HasValue && endDate.HasValue)
            {
                var totalDays = (endDate.Value - startDate.Value).Days + 1;
                if (totalDays > 0)
                {
                    averageRevenuePerDay = totalRevenue / totalDays; // Tính doanh thu trung bình
                }

                var dailyRevenue = await _context.Invoices
                    .Where(i => i.CreatedAt >= startDate.Value && i.CreatedAt <= endDate.Value)
                    .GroupBy(i => i.CreatedAt.Value.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(i => (decimal)(i.TotalAmount ?? 0)) // Chuyển null thành 0
                    })
                    .ToListAsync();

                if (dailyRevenue.Any())
                {
                    maxDailyRevenue = dailyRevenue.Max(d => d.Revenue);  // Doanh thu cao nhất
                    minDailyRevenue = dailyRevenue.Min(d => d.Revenue);  // Doanh thu thấp nhất
                }
            }

            ViewBag.SalesData = salesData;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.StatisticsData = new
            {
                TotalQuantity = totalQuantity,
                AverageRevenue = averageRevenuePerDay,
                MaxDailyRevenue = maxDailyRevenue,
                MinDailyRevenue = minDailyRevenue
            };

            return View();
        }
    }
}
