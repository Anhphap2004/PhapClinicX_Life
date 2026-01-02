using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;
namespace PhapClinicX.Controllers
{
    public class BookingController : Controller
    {
        private readonly ClinicManagementContext _context;
        private static readonly TimeSpan SlotDuration = TimeSpan.FromMinutes(30);
        public BookingController(ClinicManagementContext context)
        {
            _context = context;
        }

        [Route("kham-bac-si")]
        public IActionResult KhamBacSi()
        {
            return View();
        }

        [Route("kham-phong-kham")]
        public IActionResult KhamPhongKham()
        {
            return View();
        }

        [Route(("/booking/bacsi/{id}.html"))]
        public async Task<IActionResult> Detail_doctor(int? id)
        {
            if (id == null || _context.DoctorProfiles == null)
            {
                return NotFound();
            }
            var doctor = await _context.DoctorProfiles.Include(p => p.PhongKham)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewBag.doctor_id = id;
            return View(doctor);
        }

        [HttpPost]
        public async Task<IActionResult> DatLich(int DoctorId, string PatientName, string PhoneNumber, string SelectedSlot)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("Bạn cần đăng nhập để đặt lịch.");
            }
            var doctor = await _context.DoctorProfiles.AsNoTracking().FirstOrDefaultAsync(p => p.DoctorId == DoctorId && p.Isactive == true);
            if (doctor == null)
            {
                TempData["ErrorMessage"] = "Bác sĩ không tồn tại hoặc đang tạm ngưng nhận lịch.";
                return RedirectToAction("KhamBacSi");
            }
            if (string.IsNullOrEmpty(PatientName) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SelectedSlot))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin.");
            }

            if (!DateTime.TryParse(SelectedSlot, out DateTime appointmentTime))
            {
                return BadRequest("Ngày giờ không hợp lệ.");
            }

            if (appointmentTime <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Thời gian đặt lịch phải ở tương lai.";
                return RedirectToAction("Detail_doctor", new { id = DoctorId });
            }

            var slotStart = appointmentTime;
            var slotEnd = appointmentTime.Add(SlotDuration);
            var windowStart = slotStart - SlotDuration;

            var doctorHasConflict = await _context.DoctorAppointments
                .AnyAsync(a => a.Status
                    && a.DoctorId == DoctorId
                    && a.DateTime.HasValue
                    && a.DateTime.Value >= windowStart
                    && a.DateTime.Value < slotEnd);

            if (doctorHasConflict)
            {
                TempData["ErrorMessage"] = "Khung giờ này đã được đặt. Vui lòng chọn thời gian khác.";
                return RedirectToAction("Detail_doctor", new { id = DoctorId });
            }

            var userHasConflict = await _context.DoctorAppointments
                .AnyAsync(a => a.Status
                    && a.UserId == userId.Value
                    && a.DateTime.HasValue
                    && a.DateTime.Value >= windowStart
                    && a.DateTime.Value < slotEnd);

            if (userHasConflict)
            {
                TempData["ErrorMessage"] = "Bạn đã có lịch trùng khung giờ này.";
                return RedirectToAction("Detail_doctor", new { id = DoctorId });
            }

            var newAppointment = new DoctorAppointment
            {
                DoctorId = DoctorId,
                Fullname = PatientName,
                Phone = PhoneNumber,
                DateTime = appointmentTime,
                UserId = userId.Value,
                Status = false,
            };

            _context.DoctorAppointments.Add(newAppointment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đặt lịch thành công! Vui lòng chờ bác sĩ liên hệ.";
            return RedirectToAction("Detail_doctor", new { id = DoctorId });

        }
        [HttpPost]
        public async Task<IActionResult> cancelAppointment(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var appointment = await _context.DoctorAppointments.FirstOrDefaultAsync(p => p.AppointmentId == id && p.UserId == userId);
            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Lịch hẹn không tồn tại hoặc không thuộc về bạn.";
                return RedirectToAction("Index", "Account", new { id = userId });
            }

            appointment.Status = false;
            _context.DoctorAppointments.Update(appointment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã huỷ lịch hẹn thành công!";

            return RedirectToAction("Index", "Account", new { id = userId });
        }

        [Route("/booking/chi-nhanh-kham/{id}.html")]
        public async Task<IActionResult> Detail_branch(int? id)
        {
            if (id == null || _context.PhongKhams == null)
            {
                return NotFound();
            }
            var branch = await _context.PhongKhams
                .FirstOrDefaultAsync(m => m.PhongKhamId == id);
            if (branch == null)
            {
                return NotFound();
            }

            ViewBag.PhongKhamId = id;
            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> DatLichPhongKham(int PhongKhamId, string PatientName, string PhoneNumber, string SelectedSlot)
        {
            if (string.IsNullOrEmpty(PatientName) || string.IsNullOrEmpty(PhoneNumber) || string.IsNullOrEmpty(SelectedSlot))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin.");
            }
            var branch = await _context.PhongKhams.AsNoTracking().FirstOrDefaultAsync(p => p.PhongKhamId == PhongKhamId);
            if (branch == null)
            {
                TempData["ErrorMessage"] = "Chi nhánh không tồn tại.";
                return RedirectToAction("KhamPhongKham");
            }
            if (!DateTime.TryParse(SelectedSlot, out DateTime appointmentTime))
            {
                return BadRequest("Ngày giờ không hợp lệ.");
            }
            if (appointmentTime <= DateTime.Now)
            {
                TempData["ErrorMessage"] = "Thời gian đặt lịch phải ở tương lai.";
                return RedirectToAction("Detail_branch", new { id = PhongKhamId });
            }

            var slotStart = appointmentTime;
            var slotEnd = appointmentTime.Add(SlotDuration);
            var windowStart = slotStart - SlotDuration;

            var duplicateBooking = await _context.ClinicAppointments
                .AnyAsync(a => a.Status
                    && a.PhongKhamId == PhongKhamId
                    && a.DateTime.HasValue
                    && a.DateTime.Value >= windowStart
                    && a.DateTime.Value < slotEnd);

            if (duplicateBooking)
            {
                TempData["ErrorMessage"] = "Khung giờ này tại chi nhánh đã kín. Vui lòng chọn giờ khác.";
                return RedirectToAction("Detail_branch", new { id = PhongKhamId });
            }
            var newAppointment = new ClinicAppointment
            {
                PhongKhamId = PhongKhamId,
                Fullname = PatientName,
                Phone = PhoneNumber,
                DateTime = appointmentTime,
                Status = false,

            };
            _context.ClinicAppointments.Add(newAppointment);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đặt lịch thành công! Vui lòng đến chi nhánh đúng hẹn.";
            return RedirectToAction("Detail_branch", new { id = PhongKhamId });
        }



    }
}
