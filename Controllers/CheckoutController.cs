using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using PhapClinicX.Models.Vnpay;
using PhapClinicX.Services.Vnpay;
using System.Text.Json; // Thêm namespace này
namespace PhapClinicX.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ClinicManagementContext _context;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(IVnPayService vnPayService, ClinicManagementContext context, ILogger<CheckoutController> logger)
        {
            _vnPayService = vnPayService;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            _logger.LogInformation($"Nhận callback từ VNPAY: Success={response.Success}, OrderId={response.OrderId}");

            if (response.Success)
            {
                try
                {
                    var vnpTxnRef = Request.Query["vnp_TxnRef"].ToString();
                    _logger.LogInformation($"Raw vnp_TxnRef: {vnpTxnRef}");

                    // Kiểm tra xem vnpTxnRef có phải là mã giao dịch VNPAY hay không
                    if (string.IsNullOrEmpty(vnpTxnRef))
                    {
                        _logger.LogWarning("Không nhận được mã tham chiếu từ VNPAY");
                        return RedirectToAction("Success", "Cart", new { message = "Không nhận được mã giao dịch từ VNPAY" });
                    }

                    // Nếu mã là số lớn (timestamp), thử tìm hóa đơn qua bảng mapping
                    if (long.TryParse(vnpTxnRef, out long txnRefLong))
                    {
                        // Tìm hóa đơn gần nhất đang chờ thanh toán
                        var pendingInvoice = await _context.Invoices
                            .Where(i => i.Status == "Chờ thanh toán" && i.Method == "VNPAY")
                            .OrderByDescending(i => i.CreatedAt)
                            .FirstOrDefaultAsync();

                        if (pendingInvoice != null)
                        {
                            // Cập nhật trạng thái Invoice
                            pendingInvoice.Status = "Đã thanh toán (VNPAY)";

                            // Tìm và cập nhật Payment tương ứng
                            var payment = await _context.Payments
                                .Where(p => p.InvoiceId == pendingInvoice.InvoiceId && p.Status == "Đang xử lý")
                                .FirstOrDefaultAsync();

                            if (payment != null)
                            {
                                payment.Status = "Đã thanh toán";
                                _logger.LogInformation($"Đã cập nhật payment ID: {payment.PaymentId}");
                            }
                            else
                            {
                                // Nếu không tìm thấy payment, tạo mới
                                var newPayment = new Payment
                                {
                                    InvoiceId = pendingInvoice.InvoiceId,
                                    UserId = pendingInvoice.UserId,
                                    Amount = pendingInvoice.TotalAmount ?? 0, // Thêm cast cho decimal?
                                    Method = "VNPAY",
                                    Status = "Đã thanh toán",
                                    CreatedAt = DateTime.Now
                                };
                                _context.Payments.Add(newPayment);
                                _logger.LogInformation($"Đã tạo payment mới cho invoice ID: {pendingInvoice.InvoiceId}");
                            }

                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Đã cập nhật hoàn tất invoice ID: {pendingInvoice.InvoiceId} với mã GD: {vnpTxnRef}");

                            return RedirectToAction("Success", "Cart", new { message = "Thanh toán thành công qua VNPAY!" });
                        }
                        else
                        {
                            _logger.LogWarning($"Không tìm thấy hóa đơn đang chờ thanh toán");
                            return RedirectToAction("Success", "Cart", new { message = "Không tìm thấy hóa đơn cần thanh toán" });
                        }
                    }
                    // Thử xử lý theo cách thông thường nếu là mã hóa đơn thực
                    else if (int.TryParse(vnpTxnRef, out int invoiceId))
                    {
                        var invoice = await _context.Invoices.FindAsync(invoiceId);

                        if (invoice != null)
                        {
                            // Cập nhật trạng thái Invoice
                            invoice.Status = "Đã thanh toán (VNPAY)";

                            // Tìm và cập nhật Payment tương ứng
                            var payment = await _context.Payments
                                .Where(p => p.InvoiceId == invoice.InvoiceId && p.Status == "Đang xử lý")
                                .FirstOrDefaultAsync();

                            if (payment != null)
                            {
                                payment.Status = "Đã thanh toán";
                                _logger.LogInformation($"Đã cập nhật payment ID: {payment.PaymentId}");
                            }
                            else
                            {
                                // Nếu không tìm thấy payment, tạo mới
                                var newPayment = new Payment
                                {
                                    InvoiceId = invoice.InvoiceId,
                                    UserId = invoice.UserId,
                                    Amount = invoice.TotalAmount ?? 0, // Thêm cast cho decimal?
                                    Method = "VNPAY",
                                    Status = "Đã thanh toán",
                                    CreatedAt = DateTime.Now
                                };
                                _context.Payments.Add(newPayment);
                                _logger.LogInformation($"Đã tạo payment mới cho invoice ID: {invoice.InvoiceId}");
                            }

                            await _context.SaveChangesAsync();
                            _logger.LogInformation($"Cập nhật thành công invoice ID: {invoiceId}");

                            return RedirectToAction("Success", "Cart", new { message = "Thanh toán thành công qua VNPAY!" });
                        }
                        else
                        {
                            _logger.LogWarning($"Không tìm thấy hóa đơn ID: {invoiceId}");
                            return RedirectToAction("Success", "Cart", new { message = $"Không tìm thấy hóa đơn ID: {invoiceId}" });
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Không thể xử lý mã tham chiếu: {vnpTxnRef}");
                        return RedirectToAction("Success", "Cart", new { message = $"Mã giao dịch không hợp lệ: {vnpTxnRef}" });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi xử lý callback VNPAY");
                    return RedirectToAction("Success", "Cart", new { message = "Có lỗi xảy ra khi xử lý thanh toán!" });
                }
            }

            _logger.LogWarning("Thanh toán VNPAY thất bại hoặc bị hủy");
            return RedirectToAction("Success", "Cart", new { message = "Thanh toán thất bại hoặc bị hủy!" });
        }


    }
}
