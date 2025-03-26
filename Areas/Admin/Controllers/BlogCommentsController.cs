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
    public class BlogCommentsController : Controller
    {
        private readonly ClinicManagementContext _context;

        public BlogCommentsController(ClinicManagementContext context)
        {
            _context = context;
        }

        // GET: Admin/BlogComments
        public async Task<IActionResult> Index()
        {
            var clinicManagementContext = _context.BlogComments.Include(b => b.Blog).Include(b => b.User);
            
            return View(await clinicManagementContext.ToListAsync());
        }

        // GET: Admin/BlogComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogComment = await _context.BlogComments
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (blogComment == null)
            {
                return NotFound();
            }

            return View(blogComment);
        }

        

        // GET: Admin/BlogComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogComment = await _context.BlogComments
                .Include(b => b.Blog)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (blogComment == null)
            {
                return NotFound();
            }

            return View(blogComment);
        }

        // POST: Admin/BlogComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogComment = await _context.BlogComments.FindAsync(id);
            if (blogComment != null)
            {
                _context.BlogComments.Remove(blogComment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogCommentExists(int id)
        {
            return _context.BlogComments.Any(e => e.CommentId == id);
        }
    }
}
