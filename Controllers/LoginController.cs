using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class LoginController : Controller
    {
        private readonly ClinicManagementContext _context;
        public LoginController(ClinicManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(User model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .Where(u => u.Email == model.Email)
                    .FirstOrDefault();

                if (user != null)
                {
                    // 🚀 Sửa lỗi: So sánh mật khẩu người dùng nhập với mật khẩu đã hash trong DB
                    if (BCrypt.Net.BCrypt.Verify(model.PasswordHash, user.PasswordHash))
                    {
                        // Đảm bảo không có giá trị null khi lưu vào Session
                        HttpContext.Session.SetString("UserId", user.UserId.ToString() ?? "0");
                        HttpContext.Session.SetString("FullName", user.FullName ?? ""); // Nếu FullName null -> ""
                        HttpContext.Session.SetString("Email", user.Email ?? ""); // Nếu Email null -> ""
                        HttpContext.Session.SetString("RoleId", user.RoleId?.ToString() ?? "0"); // Nếu RoleId null -> "0"

                        return RedirectToAction("Index", "Home");
                    }

                    else
                    {
                        TempData["Error"] = "Mật khẩu không đúng!";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Email không tồn tại!";
                    return View();
                }
            }
            return View();
        }

    }
}
