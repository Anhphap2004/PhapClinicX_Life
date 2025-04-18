using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models.Vnpay;
using PhapClinicX.Services.Vnpay;

namespace PhapClinicX.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IVnPayService vnPayService, ILogger<PaymentController> logger)
        {
            _vnPayService = vnPayService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreatePaymentUrlVnpay(decimal amount, string orderId, string description)
        {
            // Ghi log để debug
            _logger.LogInformation($"Tạo URL thanh toán VNPAY: OrderId={orderId}, Amount={amount}");

            var model = new PaymentInformationModel
            {
                Amount = (double)amount,
                OrderDescription = description,
                OrderType = "billpayment",
                // Không thêm dấu # vào orderId
                OrderId = orderId,
                Name = "Anh Phapp"
            };

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            // Ghi log URL được tạo (loại bỏ thông tin nhạy cảm nếu có)
            _logger.LogInformation($"Đã tạo URL VNPAY thành công, chuyển hướng người dùng");

            return Redirect(url);
        }
    }
}