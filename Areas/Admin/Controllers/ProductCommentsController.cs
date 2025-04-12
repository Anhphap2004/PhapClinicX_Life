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
    public class ProductCommentsController : Controller
    {
        private readonly ClinicManagementContext _context;

        public ProductCommentsController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductComments
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.ProductComments.Include(p => p.Product).Include(p => p.User);
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/ProductComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.ProductComments
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (productComment == null)
            {
                return NotFound();
            }

            return View(productComment);
        }

        // GET: Admin/ProductComments/Create
        

        // GET: Admin/ProductComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productComment = await _context.ProductComments
                .Include(p => p.Product)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (productComment == null)
            {
                return NotFound();
            }

            return View(productComment);
        }

        // POST: Admin/ProductComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productComment = await _context.ProductComments.FindAsync(id);
            if (productComment != null)
            {
                _context.ProductComments.Remove(productComment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductCommentExists(int id)
        {
            return _context.ProductComments.Any(e => e.CommentId == id);
        }
    }
}
