using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using PhapClinicX.Models.Vnpay;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace PhapClinicX.Controllers
{
    public class CartController : Controller
    {
        private readonly ClinicManagementContext _context;

        public CartController(ClinicManagementContext context)
        {
            _context = context;
        }

        public IActionResult Success(string message)
        {
            ViewBag.Message = message;
            return View();
        }


        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            await GetRelatedProducts(userId);

            decimal cartTotal = cartItems.Sum(item => (item.Product?.PriceSale ?? 0) * (item.Quantity ?? 0));
            ViewBag.ShippingFee = cartTotal >= 1000000 ? 0 : 30000;

            // Lấy thông tin giảm giá từ session
            ViewBag.Discount = HttpContext.Session.GetInt32("DiscountAmount") ?? 0;
            ViewBag.CouponCode = HttpContext.Session.GetString("CouponCode");

            // Nếu có mã giảm giá, hiển thị thông tin mã giảm giá
            if (!string.IsNullOrEmpty(ViewBag.CouponCode))
            {
                int? discountId = HttpContext.Session.GetInt32("DiscountId");
                if (discountId.HasValue)
                {
                    ViewBag.DiscountInfo = await _context.Discounts.FirstOrDefaultAsync(d => d.DiscountId == discountId);
                }
            }

            return View(cartItems);
        }

        private async Task GetRelatedProducts(int? userId)
        {
            if (userId == null) return;

            var cartCategoryIds = await _context.Carts
                .Where(c => c.UserId == userId)
                .Join(_context.Products,
                      c => c.ProductId,
                      p => p.ProductId,
                      (c, p) => p.CategoryId)
                .Distinct()
                .ToListAsync();

            var cartProductIds = await _context.Carts
                .Where(c => c.UserId == userId)
                .Select(c => c.ProductId)
                .ToListAsync();

            var relatedProducts = await _context.Products
                .Where(p => cartCategoryIds.Contains(p.CategoryId) && !cartProductIds.Contains(p.ProductId))
                .OrderByDescending(p => p.PriceSale)
                .Take(4)
                .ToListAsync();
   
            ViewBag.RelatedProducts = relatedProducts;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

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

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện thao tác này" });
            }

            var cartItem = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);
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

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }




        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string code)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để áp dụng mã giảm giá";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(code))
            {
                TempData["Message"] = "Vui lòng nhập mã giảm giá";
                return RedirectToAction("Index");
            }

            var coupon = await _context.Discounts
                .FirstOrDefaultAsync(c => c.Code == code && c.IsActive && DateTime.Now >= c.StartDate && DateTime.Now <= c.EndDate);

            if (coupon == null)
            {
                TempData["Message"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn";
                return RedirectToAction("Index");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            decimal cartTotal = cartItems.Sum(item => (item.Product?.PriceSale ?? 0) * (item.Quantity ?? 0));
            decimal discountAmount = Math.Round((cartTotal * coupon.DiscountPercent) / 100);

            // Lưu thông tin giảm giá vào session
            HttpContext.Session.SetString("DiscountAmount", discountAmount.ToString()); // Lưu dạng string
            HttpContext.Session.SetString("CouponCode", code);
            HttpContext.Session.SetInt32("DiscountId", coupon.DiscountId);


            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.Remove("DiscountAmount");
            HttpContext.Session.Remove("CouponCode");
            HttpContext.Session.Remove("DiscountId");

            // Có thể Redirect về lại giỏ hàng
            return RedirectToAction("Index", "Cart");
        }


        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCheckedOut == false)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống.";
                return RedirectToAction("Index");
            }

            decimal cartTotal = cartItems.Sum(c => (c.Quantity ?? 0) * (c.Product?.PriceSale ?? 0));
            decimal shippingFee = cartTotal >= 1_000_000 ? 0 : 30_000;

            // 🎯 Lấy giảm giá từ Session
            decimal discountAmount = decimal.TryParse(HttpContext.Session.GetString("DiscountAmount"), out var d) ? d : 0;
            decimal finalTotal = cartTotal + shippingFee - discountAmount;
            if (finalTotal < 0) finalTotal = 0;

            ViewBag.ProductNames = await _context.Products.ToDictionaryAsync(p => p.ProductId, p => p.ProductName);
            ViewBag.ProductImages = await _context.Products.ToDictionaryAsync(p => p.ProductId, p => p.Image);
            ViewBag.ListPhongKham = await _context.PhongKhams.Where(p => p.Isactive == true).ToListAsync();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                ViewBag.UserAddress = user.Address;
                ViewBag.UserPhone = user.Phone;
                ViewBag.UserName = user.FullName;
            }

            ViewBag.ShippingFee = shippingFee;
            ViewBag.ProductTotal = cartTotal;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.FinalTotal = finalTotal;

            var invoice = new Invoice
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = "Chờ thanh toán",
                TotalAmount = finalTotal,
                InvoiceType = "Product",
                InvoiceDetails = cartItems.Select(c => new InvoiceDetail
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product?.PriceSale
                }).ToList()
            };

            return View("InvoiceConfirmation", invoice);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(string method, int phongKhamId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId && c.IsCheckedOut == false)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Không có sản phẩm nào trong giỏ hàng.";
                return RedirectToAction("Index");
            }

            decimal cartTotal = cartItems.Sum(item => (item.Product?.PriceSale ?? 0) * (item.Quantity ?? 0));
            decimal shippingFee = cartTotal >= 1_000_000 ? 0 : 30_000;

            // 🎯 Lấy giảm giá từ Session
            decimal discountAmount = decimal.TryParse(HttpContext.Session.GetString("DiscountAmount"), out var d) ? d : 0;
            decimal finalTotal = cartTotal + shippingFee - discountAmount;
            if (finalTotal < 0) finalTotal = 0;

            string couponCode = HttpContext.Session.GetString("Code") ?? "";
            int? discountId = HttpContext.Session.GetInt32("DiscountId");

            var invoice = new Invoice
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                Status = "Chờ thanh toán",
                TotalAmount = finalTotal,
                PhongKhamId = phongKhamId,
                InvoiceType = "Sản phẩm",
                Method = method,
                DiscountAmount = discountAmount,
                DiscountCode = couponCode,
                DiscountId = discountId
            };

            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var detail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product?.PriceSale
                };
                await _context.InvoiceDetails.AddAsync(detail);
            }

            await _context.SaveChangesAsync();

            // 🧹 Xóa Session giảm giá sau khi thanh toán
            HttpContext.Session.Remove("DiscountPercent");
            HttpContext.Session.Remove("Code");
            HttpContext.Session.Remove("DiscountId");

            // 🛒 Xóa giỏ hàng
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // 👉 Xử lý các phương thức thanh toán
            if (method == "ChuyenKhoan")
            {
                invoice.Status = "Chờ xác nhận thanh toán";
                await _context.SaveChangesAsync();
                return RedirectToAction("BankTransfer", new { id = invoice.InvoiceId });
            }

            if (method == "VNPAY")
            {
                var payment = new Payment
                {
                    InvoiceId = invoice.InvoiceId,
                    UserId = userId,
                    Amount = finalTotal,
                    Method = method,
                    Status = "Đang xử lý",
                    CreatedAt = DateTime.Now
                };
                await _context.Payments.AddAsync(payment);
                await _context.SaveChangesAsync();

                return RedirectToAction("CreatePaymentUrlVnpay", "Payment", new
                {
                    amount = finalTotal,
                    orderId = invoice.InvoiceId.ToString(),
                    description = $"Thanh toán đơn hàng {invoice.InvoiceId} tại PhapClinicX"
                });
            }
            else
            {
                var payment = new Payment
                {
                    InvoiceId = invoice.InvoiceId,
                    UserId = userId,
                    Amount = finalTotal,
                    Method = method,
                    Status = "Đã Đặt Hàng",
                    CreatedAt = DateTime.Now
                };
                await _context.Payments.AddAsync(payment);
                invoice.Status = "Đã Đặt Hàng";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Success", "Cart");
        }

      


    }
}