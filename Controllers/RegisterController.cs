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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Kiểm tra nếu email, username hoặc số điện thoại đã tồn tại
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Username || u.Phone == model.Phone);

        if (existingUser != null)
        {
            if (existingUser.Email == model.Email)
            {
                ModelState.AddModelError("Email", "⚠ Email đã được đăng ký!");
            }
            if (existingUser.Username == model.Username)
            {
                ModelState.AddModelError("Username", "⚠ Tên đăng nhập đã tồn tại!");
            }
            if (existingUser.Phone == model.Phone)
            {
                ModelState.AddModelError("Phone", "⚠ Số điện thoại đã được sử dụng!");
            }

            return View(model); // Giữ nguyên dữ liệu khi có lỗi
        }

        // Kiểm tra mật khẩu có bị null không (tránh lỗi ArgumentNullException)
        if (string.IsNullOrWhiteSpace(model.PasswordHash))
        {
            ModelState.AddModelError("PasswordHash", "⚠ Mật khẩu không hợp lệ!");
            return View(model);
        }

        // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.PasswordHash);

        var user = new User
        {
            Username = model.Username,
            Phone = model.Phone,
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = hashedPassword,
            RoleId = 3 // Gán quyền mặc định
        };

        // Lưu người dùng vào cơ sở dữ liệu
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "🎉 Đăng ký thành công. Vui lòng đăng nhập!";
        return RedirectToAction("Login", "Auth");
    }



}