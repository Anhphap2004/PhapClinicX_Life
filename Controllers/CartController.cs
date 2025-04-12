using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using System.Linq;
using System.Collections.Generic;

namespace PhapClinicX.Controllers
{
    public class CartController : Controller
    {
        private readonly ClinicManagementContext _context;

        public CartController(ClinicManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cartItems = _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            // Lấy các sản phẩm liên quan
            GetRelatedProducts();

            // Tính phí vận chuyển (có thể điều chỉnh theo logic của bạn)
            decimal cartTotal = cartItems.Sum(item => (item.Product?.PriceSale ?? 0) * (item.Quantity ?? 0));
            ViewBag.ShippingFee = cartTotal >= 1000000 ? 0 : 30000;

            // Lấy giảm giá từ session nếu có
            ViewBag.Discount = HttpContext.Session.GetInt32("DiscountAmount") ?? 0;

            return View(cartItems);
        }

        private void GetRelatedProducts()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return;

            // Lấy các category id của sản phẩm trong giỏ hàng
            var cartCategoryIds = _context.Carts
                .Where(c => c.UserId == userId)
                .Join(_context.Products,
                      c => c.ProductId,
                      p => p.ProductId,
                      (c, p) => p.CategoryId)
                .Distinct()
                .ToList();

            // Lấy các sản phẩm cùng category không có trong giỏ hàng
            var cartProductIds = _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => c.ProductId)
                .ToList();

            var relatedProducts = _context.Products
                .Where(p => cartCategoryIds.Contains(p.CategoryId) && !cartProductIds.Contains(p.ProductId))
                .OrderByDescending(p => p.PriceSale)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Chưa đăng nhập thì đá về trang đăng nhập
                return RedirectToAction("Index", "Login");
            }

            var existingCartItem = _context.Carts
                .FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.Carts.Add(cartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int cartId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
            }

            var cartItem = _context.Carts.FirstOrDefault(c => c.CartId == cartId && c.UserId == userId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }

            if (quantity <= 0)
            {
                _context.Carts.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult RemoveItem(int cartId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
            }

            var cartItem = _context.Carts.FirstOrDefault(c => c.CartId == cartId && c.UserId == userId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }

            _context.Carts.Remove(cartItem);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
            }

            var cartItems = _context.Carts.Where(c => c.UserId == userId);
            _context.Carts.RemoveRange(cartItems);
            _context.SaveChanges();

            // Xóa mã giảm giá nếu có
            HttpContext.Session.Remove("DiscountAmount");
            HttpContext.Session.Remove("CouponCode");

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ApplyCoupon(string code)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
            }

            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });
            }

            // Kiểm tra mã giảm giá trong database
            var coupon = _context.Discounts
                .FirstOrDefault(c => c.Code == code && c.IsActive && DateTime.Now >= c.StartDate && DateTime.Now <= c.EndDate);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn" });
            }

            // Lưu thông tin giảm giá vào session
            HttpContext.Session.SetInt32("DiscountAmount", (int)coupon.DiscountPercent);
            HttpContext.Session.SetString("CouponCode", code);

            return Json(new { success = true, message = "Áp dụng mã giảm giá thành công" });
        }

        public IActionResult Checkout()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cartItems = _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm vào giỏ hàng.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng để điền sẵn vào form checkout
            var user = _context.Users.Find(userId);
            ViewBag.User = user;

            // Tính phí vận chuyển
            decimal cartTotal = cartItems.Sum(item => (item.Product?.PriceSale ?? 0) * (item.Quantity ?? 0));
            ViewBag.ShippingFee = cartTotal >= 1000000 ? 0 : 30000;

            // Lấy giảm giá từ session nếu có
            ViewBag.Discount = HttpContext.Session.GetInt32("DiscountAmount") ?? 0;
            ViewBag.CouponCode = HttpContext.Session.GetString("CouponCode");

            return View(cartItems);
        }
    }
}