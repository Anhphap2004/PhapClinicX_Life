using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class BookingController : Controller
    {
        private readonly ClinicManagementContext _context;
        public BookingController(ClinicManagementContext context)
        {
            _context = context;
        }

        [Route("kham-bac-si")]
        public IActionResult KhamBacSi()
        {
            return View();
        }

        [Route(("/booking/bacsi/{id}.html"))]
        public async Task<IActionResult> Detail_doctor(int? id)
        {
           if(id == null || _context.DoctorProfiles == null)
            {
                return NotFound();
            }
            var doctor = await _context.DoctorProfiles.Include(p => p.PhongKham)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            var availableSlots = new List<DateTime>
            {
                DateTime.Now.AddDays(1).Date.AddHours(9),
                DateTime.Now.AddDays(1).Date.AddHours(11),
                DateTime.Now.AddDays(1).Date.AddHours(14),
                DateTime.Now.AddDays(1).Date.AddHours(16),
                DateTime.Now.AddDays(2).Date.AddHours(10),
            };

            ViewBag.AvailableSlots = availableSlots;
            ViewBag.doctor_id = id;
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(int DoctorId, string PatientName, string PhoneNumber, string SelectedSlot)
        {
            if (string.IsNullOrEmpty(PatientName) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SelectedSlot))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin.");
            }

            if (!DateTime.TryParse(SelectedSlot, out DateTime appointmentTime))
            {
                return BadRequest("Ngày giờ không hợp lệ.");
            }

            var newAppointment = new DoctorAppointment
            {
                DoctorId = DoctorId,
                Fullname = PatientName,
                Phone = PhoneNumber,  // Đã đổi từ int -> string
                DateTime = appointmentTime
            };

            _context.DoctorAppointments.Add(newAppointment);
            await _context.SaveChangesAsync();
            // Lưu thông báo vào TempData để hiển thị sau khi redirect
            TempData["SuccessMessage"] = "Đặt lịch thành công! Vui lòng chờ bác sĩ liên hệ.";
            return RedirectToAction("Detail_doctor", new { id = DoctorId });

        }




    }
}
