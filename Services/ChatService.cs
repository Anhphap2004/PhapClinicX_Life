using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
using PhapClinicX.Models.ViewModels;

namespace PhapClinicX.Services
{
    public class ChatService : IChatService
    {
        private readonly ClinicManagementContext _context;
        private static readonly string[] DoctorKeywords = { "bác sĩ", "bac si", "doctor", "bs", "chuyên khoa", "special" };
        private static readonly string[] BookingKeywords = { "đặt lịch", "dat lich", "book", "hẹn", "hen", "lịch của tôi", "lich cua toi" };
        private static readonly string[] BranchKeywords = { "chi nhánh", "phòng khám", "phong kham", "branch" };
        private static readonly string[] ServiceKeywords = { "dịch vụ", "dich vu", "khám", "kham", "service", "giá", "gia" };
        private static readonly string[] FaqKeywords = { "hỏi đáp", "câu hỏi", "faq", "thắc mắc", "hoi dap" };

        public ChatService(ClinicManagementContext context)
        {
            _context = context;
        }

        public async Task<ChatReply> AskAsync(ChatQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.Message))
            {
                return new ChatReply { Answer = "Bạn hãy nhập nội dung cần hỏi." };
            }

            var text = query.Message.ToLowerInvariant();

            if (ContainsAny(text, BookingKeywords) && query.UserId.HasValue)
            {
                return await BuildUserAppointmentReply(query.UserId.Value);
            }

            if (ContainsAny(text, DoctorKeywords))
            {
                return await BuildDoctorListReply(text);
            }

            if (ContainsAny(text, BranchKeywords))
            {
                return await BuildBranchReply();
            }

            if (ContainsAny(text, ServiceKeywords))
            {
                return await BuildServiceReply(text);
            }

            if (ContainsAny(text, FaqKeywords))
            {
                return await BuildFaqReply();
            }

            // Fallback: try best-effort doctor search, else generic prompt
            var quickDoctor = await BuildDoctorListReply(text, fallbackOnly: true);
            if (!string.IsNullOrEmpty(quickDoctor.Answer) && quickDoctor.Answer.Contains("-"))
            {
                return quickDoctor;
            }

            return new ChatReply
            {
                Answer = "Mình là trợ lý phòng khám. Bạn có thể hỏi: danh sách bác sĩ, dịch vụ và giá, chi nhánh, FAQ, hoặc xem lịch của bạn (cần đăng nhập)."
            };
        }

        private async Task<ChatReply> BuildUserAppointmentReply(int userId)
        {
            var now = DateTime.Now;
            var upcoming = await _context.DoctorAppointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.PhongKham)
                .Where(a => a.UserId == userId && a.DateTime.HasValue && a.DateTime.Value >= now && a.Status)
                .OrderBy(a => a.DateTime)
                .Take(3)
                .ToListAsync();

            if (!upcoming.Any())
            {
                return new ChatReply { Answer = "Bạn chưa có lịch sắp tới. Bạn muốn đặt lịch mới không?" };
            }

            var sb = new StringBuilder();
            sb.AppendLine("Bạn có các lịch sắp tới:");
            foreach (var appt in upcoming)
            {
                sb.AppendLine($"- {appt.DateTime:dd/MM HH:mm} với bác sĩ {appt.Doctor?.Fullname} tại {appt.Doctor?.PhongKham?.TenPhongKham}");
            }

            return new ChatReply
            {
                Answer = sb.ToString().TrimEnd(),
                Sources = upcoming.Select(a => $"DoctorAppointment#{a.AppointmentId}").ToList()
            };
        }

        private async Task<ChatReply> BuildDoctorListReply(string text = "", bool fallbackOnly = false)
        {
            var query = _context.DoctorProfiles
                .Include(d => d.PhongKham)
                .Where(d => d.Isactive == true);

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(d =>
                    (d.Fullname ?? "").Contains(text) ||
                    (d.Specialization ?? "").Contains(text));
            }

            var doctors = await query
                .OrderBy(d => d.Fullname)
                .Take(6)
                .ToListAsync();

            if (!doctors.Any())
            {
                if (fallbackOnly)
                {
                    return new ChatReply { Answer = string.Empty };
                }
                return new ChatReply { Answer = "Hiện chưa tìm thấy bác sĩ phù hợp." };
            }

            var sb = new StringBuilder();
            sb.AppendLine("Một số bác sĩ đang nhận lịch:");
            foreach (var d in doctors)
            {
                sb.AppendLine($"- {d.Fullname} ({d.Specialization}) tại {d.PhongKham?.TenPhongKham}");
            }
            sb.Append("Bạn muốn đặt với ai? Hãy cho tôi tên hoặc chuyên khoa.");

            return new ChatReply
            {
                Answer = sb.ToString(),
                Sources = doctors.Select(d => $"Doctor#{d.DoctorId}").ToList()
            };
        }

        private static bool ContainsAny(string text, string[] keywords)
        {
            return keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<ChatReply> BuildBranchReply()
        {
            var branches = await _context.PhongKhams
                .Where(b => b.Isactive)
                .OrderBy(b => b.TenPhongKham)
                .Take(5)
                .ToListAsync();

            if (!branches.Any())
            {
                return new ChatReply { Answer = "Hiện chưa có chi nhánh nào mở." };
            }

            var sb = new StringBuilder();
            sb.AppendLine("Các chi nhánh đang hoạt động:");
            foreach (var b in branches)
            {
                sb.AppendLine($"- {b.TenPhongKham} | Địa chỉ: {b.DiaChi} | Điện thoại: {b.SoDienThoai}");
            }
            sb.Append("Bạn muốn đặt lịch tại chi nhánh nào?");

            return new ChatReply
            {
                Answer = sb.ToString(),
                Sources = branches.Select(b => $"Branch#{b.PhongKhamId}").ToList()
            };
        }

        private async Task<ChatReply> BuildServiceReply(string text)
        {
            var query = _context.Services.Where(s => s.IsActive);
            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(s => (s.ServiceName ?? "").Contains(text) || (s.Detail ?? "").Contains(text));
            }

            var services = await query
                .OrderBy(s => s.ServiceName)
                .Take(5)
                .ToListAsync();

            if (!services.Any())
            {
                return new ChatReply { Answer = "Chưa tìm thấy dịch vụ phù hợp. Bạn có thể hỏi tên dịch vụ cụ thể (ví dụ: khám tổng quát, xét nghiệm)." };
            }

            var sb = new StringBuilder();
            sb.AppendLine("Một số dịch vụ:");
            foreach (var s in services)
            {
                var price = s.Price.HasValue ? $" | Giá: {s.Price.Value:n0}đ" : string.Empty;
                sb.AppendLine($"- {s.ServiceName}{price}");
            }
            sb.Append("Bạn cần đặt dịch vụ nào hoặc muốn xem chi nhánh phù hợp?");

            return new ChatReply
            {
                Answer = sb.ToString(),
                Sources = services.Select(s => $"Service#{s.ServiceId}").ToList()
            };
        }

        private async Task<ChatReply> BuildFaqReply()
        {
            var faqs = await _context.Faqs
                .OrderByDescending(f => f.CreatedAt)
                .Take(3)
                .ToListAsync();

            if (!faqs.Any())
            {
                return new ChatReply { Answer = "Hiện chưa có mục hỏi đáp. Bạn hãy đặt câu hỏi cụ thể, mình sẽ cố gắng hỗ trợ." };
            }

            var sb = new StringBuilder();
            sb.AppendLine("Một vài hỏi đáp gần đây:");
            foreach (var f in faqs)
            {
                sb.AppendLine($"- {f.Question}: {f.Answer}");
            }
            sb.Append("Bạn muốn biết thêm điều gì?");

            return new ChatReply
            {
                Answer = sb.ToString(),
                Sources = faqs.Select(f => $"FAQ#{f.Id}").ToList()
            };
        }
    }
}
