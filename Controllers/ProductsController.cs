﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using System.Diagnostics;


namespace PhapClinicX.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ClinicManagementContext _context;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ClinicManagementContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index(int? categoryID, int page = 1, int pageSize = 8)
        {
            // Lấy sản phẩm mới và đang hoạt động
            ViewBag.ProductNew = await _context.Products
                .Where(p => (p.IsNew == true) && (p.IsActive)).Take(5)
                .ToListAsync();

            // Lấy danh sách loại sản phẩm
            ViewBag.ProductCategories = await _context.ProductCategories.ToListAsync();

            // Tạo truy vấn sản phẩm lọc theo category (nếu có) và đang hoạt động
            var query = _context.Products
                .Where(p =>
                    (!categoryID.HasValue || p.CategoryId == categoryID.Value)
                    && (p.IsActive)
                )
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedDate);

            // Đếm tổng số sản phẩm sau khi lọc
            int totalItems = await query.CountAsync();

            // Lấy sản phẩm theo trang (phân trang)
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Truyền thông tin phân trang và category hiện tại sang View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SelectedCategoryID = categoryID;

            return View(products);
        }



        //public IActionResult Index()
        //{
        //    var Products = _context.Products.Where(p => p.IsActive == true).ToList();
        //    return View(Products);
        //}
        [Route("/san-pham/{alias}-{id}.html")]
        public async Task<IActionResult> Details(string alias, int id)
        {
            if (id == 0 || string.IsNullOrEmpty(alias))
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null || product.Alias != alias)
            {
                return NotFound();
            }

            ViewBag.Category = await _context.ProductCategories
                .Where(p => p.IsActive == true && p.CategoryId == product.CategoryId)
                .ToListAsync(); // Truy vấn bất đồng bộ tốt hơn

            ViewBag.ProductComments = await _context.ProductComments
                .Include(p => p.User)
                .Where(p => p.ProductId == id)
                .Take(5)
                .ToListAsync();

            ViewBag.RelatedProducts = await _context.Products
                .Where(p => p.IsActive && p.CategoryId == product.CategoryId && p.ProductId != product.ProductId).Take(5)
                .ToListAsync(); // Thêm điều kiện tránh lặp chính nó

            var RandomDiscount = await _context.Discounts
      .Where(d => d.IsActive == true && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
      .OrderBy(r => Guid.NewGuid()) // Random
      .FirstOrDefaultAsync();
            if (RandomDiscount == null)
            {
                ViewBag.Discount = 0;
            }
            else
            {
                ViewBag.Discount = RandomDiscount;
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Comment(int ProductId, string Comment)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lấy sản phẩm và kiểm tra hợp lệ
            var product = await _context.Products
                .Where(p => p.ProductId == ProductId)
                .Select(p => new { p.Alias, p.ProductId })
                .FirstOrDefaultAsync();

            if (product == null || string.IsNullOrEmpty(product.Alias))
            {
                return BadRequest("Sản phẩm không tồn tại.");
            }

            // Lưu bình luận
            var newComment = new ProductComment
            {
                ProductId = ProductId,
                UserId = userId.Value,
                Comment = Comment,
                CreatedAt = DateTime.Now
            };

            _context.ProductComments.Add(newComment);
            await _context.SaveChangesAsync();

            // Chuyển hướng về đúng URL
            return RedirectToAction("Details", new { alias = product.Alias, id = ProductId });
        }

    }
}
