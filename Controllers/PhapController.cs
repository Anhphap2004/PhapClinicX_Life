using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class PhapController : Controller
    {
        private readonly ClinicManagementContext _context;
        public PhapController(ClinicManagementContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> HashPasswords()
        {
            var users = _context.Users.ToList(); // Lấy danh sách người dùng

            foreach (var user in users)
            {
                if (!user.PasswordHash.StartsWith("$2a$")) // Nếu chưa mã hóa thì mới mã hóa
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Đã cập nhật mật khẩu thành BCrypt thành công!");
        }

    }
}
