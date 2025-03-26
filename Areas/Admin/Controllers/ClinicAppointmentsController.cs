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
    public class ClinicAppointmentsController : Controller
    {
        private readonly ClinicManagementContext _context;

        public ClinicAppointmentsController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/ClinicAppointments
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.ClinicAppointments.Include(c => c.PhongKham);
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/ClinicAppointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinicAppointment = await _context.ClinicAppointments
                .Include(c => c.PhongKham)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (clinicAppointment == null)
            {
                return NotFound();
            }

            return View(clinicAppointment);
        }

        // GET: Admin/ClinicAppointments/Create
        public IActionResult Create()
        {
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham");
            return View();
        }

        // POST: Admin/ClinicAppointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,Fullname,Phone,DateTime,Status,PhongKhamId")] ClinicAppointment clinicAppointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clinicAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", clinicAppointment.PhongKhamId);
            return View(clinicAppointment);
        }

        // GET: Admin/ClinicAppointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinicAppointment = await _context.ClinicAppointments.FindAsync(id);
            if (clinicAppointment == null)
            {
                return NotFound();
            }
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", clinicAppointment.PhongKhamId);
            return View(clinicAppointment);
        }

        // POST: Admin/ClinicAppointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Fullname,Phone,DateTime,Status,PhongKhamId")] ClinicAppointment clinicAppointment)
        {
            if (id != clinicAppointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clinicAppointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClinicAppointmentExists(clinicAppointment.AppointmentId))
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
            ViewData["PhongKhamId"] = new SelectList(_context.PhongKhams, "PhongKhamId", "TenPhongKham", clinicAppointment.PhongKhamId);
            return View(clinicAppointment);
        }

        // GET: Admin/ClinicAppointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clinicAppointment = await _context.ClinicAppointments
                .Include(c => c.PhongKham)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (clinicAppointment == null)
            {
                return NotFound();
            }

            return View(clinicAppointment);
        }

        // POST: Admin/ClinicAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clinicAppointment = await _context.ClinicAppointments.FindAsync(id);
            if (clinicAppointment != null)
            {
                _context.ClinicAppointments.Remove(clinicAppointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClinicAppointmentExists(int id)
        {
            return _context.ClinicAppointments.Any(e => e.AppointmentId == id);
        }
    }
}
