using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using PhapClinicX.Models;
using System.Threading.Tasks;

public class RegisterController : Controller
{
    private readonly ClinicManagementContext _context;

    public RegisterController(ClinicManagementContext context)
    {
        _context = context;
    }

    // Trang đăng ký
    public IActionResult Register()
    {
        return View();
    }

    // Xử lý đăng ký và lưu thông tin vào cơ sở dữ liệu
    [HttpPost]
    public async Task<IActionResult> Register(User model)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra nếu email đã tồn tại trong cơ sở dữ liệu
            var existingUser = await _context.Users
                .Where(u => u.Email == model.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if(existingUser.Email == model.Email)
                {
                    TempData["Error"] = "Email đã tồn tại!";
                }
                return RedirectToAction("Register","Register");
            }

            // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                RoleId = 3, // hoặc 1 tùy vào quyền người dùng
            };

            // Lưu người dùng vào cơ sở dữ liệu
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập!";
            // Sau khi lưu thành công, chuyển hướng người dùng đến trang chào mừng
            return RedirectToAction("Index", "Home");  // Chuyển hướng đến Home/Index
        }

        // Nếu ModelState không hợp lệ, trả lại view với thông báo lỗi
        return View(model);
    }
}