using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
namespace PhapClinicX.Middleware
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _next;
        public RoleMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            // Lấy RoleId từ Session (hoặc Cookie nếu cần)
            int roleId = context.Session.GetInt32("RoleId") ?? 0; // Mặc định là 0 nếu không có

            string path = context.Request.Path.ToString().ToLower(); // Lấy URL hiện tại

            // Kiểm tra nếu User thường (RoleId != 1) cố truy cập vào /admin
            if (path.StartsWith("/admin") && roleId != 1)
            {
                // Chuyển hướng về trang chính của User
                context.Response.Redirect("/Home/Index");
                return;
            }

            await _next(context); // Cho phép request tiếp tục nếu hợp lệ
        }
    }
}
