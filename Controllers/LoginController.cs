using Microsoft.AspNetCore.Mvc;
using PhapClinicX.Models;
using BCrypt.Net;

namespace PhapClinicX.Controllers
{
    public class LoginController : Controller
    {
        private readonly ClinicManagementContext _context;

        public LoginController(ClinicManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user, bool rememberMe)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash))
            {
                if (string.IsNullOrEmpty(user.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập không được để trống.");
                }
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    ModelState.AddModelError("PasswordHash", "Mật khẩu không được để trống.");
                }
                return View(user);
            }

            // Tìm người dùng theo Username
            var check = _context.Users.FirstOrDefault(m => m.Username == user.Username);

            // Kiểm tra tài khoản tồn tại không
            if (check == null)
            {
                ModelState.AddModelError("Username", "Tên đăng nhập không tồn tại.");
                return View(user);
            }

            // Kiểm tra mật khẩu
            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, check.PasswordHash))
            {
                ModelState.AddModelError("PasswordHash", "Mật khẩu không đúng.");
                return View(user);
            }


            // Lưu thông tin vào Session
            HttpContext.Session.SetInt32("UserId", check.UserId);
            HttpContext.Session.SetString("Username", check.Username ?? string.Empty);
            HttpContext.Session.SetString("Email", check.Email ?? string.Empty);
            HttpContext.Session.SetString("FullName", check.FullName ?? string.Empty);
            HttpContext.Session.SetString("Phone", check.Phone ?? string.Empty);
            HttpContext.Session.SetInt32("RoleId", check.RoleId ?? 0);

            // Nếu chọn "Ghi nhớ đăng nhập", lưu vào Cookie
            if (rememberMe)
            {
                CookieOptions option = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true
                };
                Response.Cookies.Append("UserId", check.UserId.ToString(), option);
                Response.Cookies.Append("Username", check.Username ?? string.Empty, option);
                Response.Cookies.Append("RoleId", (check.RoleId ?? 0).ToString(), option);
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session
            return RedirectToAction("Index", "Home");
        }

    }
}
