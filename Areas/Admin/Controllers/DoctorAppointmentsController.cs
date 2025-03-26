using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhapClinicX.Models;

namespace PhapClinicX.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DoctorAppointmentsController : Controller
    {
        private readonly ClinicManagementContext _context;

        public DoctorAppointmentsController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/DoctorAppointments
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.DoctorAppointments.Include(d => d.Doctor);
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/DoctorAppointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorAppointment = await _context.DoctorAppointments
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (doctorAppointment == null)
            {
                return NotFound();
            }

            return View(doctorAppointment);
        }

        // GET: Admin/DoctorAppointments/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.DoctorProfiles, "DoctorId", "DoctorId");
            return View();
        }

        // POST: Admin/DoctorAppointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,UserId,DoctorId,DateTime,Status,Fullname,Phone")] DoctorAppointment doctorAppointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorProfiles, "DoctorId", "DoctorId", doctorAppointment.DoctorId);
            return View(doctorAppointment);
        }

        // GET: Admin/DoctorAppointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorAppointment = await _context.DoctorAppointments.FindAsync(id);
            if (doctorAppointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorProfiles, "DoctorId", "DoctorId", doctorAppointment.DoctorId);
            return View(doctorAppointment);
        }

        // POST: Admin/DoctorAppointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,UserId,DoctorId,DateTime,Status,Fullname,Phone")] DoctorAppointment doctorAppointment)
        {
            if (id != doctorAppointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorAppointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorAppointmentExists(doctorAppointment.AppointmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorProfiles, "DoctorId", "DoctorId", doctorAppointment.DoctorId);
            return View(doctorAppointment);
        }

        // GET: Admin/DoctorAppointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorAppointment = await _context.DoctorAppointments
                .Include(d => d.Doctor)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (doctorAppointment == null)
            {
                return NotFound();
            }

            return View(doctorAppointment);
        }

        // POST: Admin/DoctorAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorAppointment = await _context.DoctorAppointments.FindAsync(id);
            if (doctorAppointment != null)
            {
                _context.DoctorAppointments.Remove(doctorAppointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorAppointmentExists(int id)
        {
            return _context.DoctorAppointments.Any(e => e.AppointmentId == id);
        }
    }
}
