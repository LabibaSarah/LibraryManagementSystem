using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NoticeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoticeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: /Notice
        // =========================

        public async Task<IActionResult> Index()
        {
            var notices = await _context.Notices
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View("~/Views/Notices/Index.cshtml", notices);
        }


        // =========================
        // GET: /Notice/Create
        // =========================

        public IActionResult Create()
        {
            return View("~/Views/Notices/Create.cshtml");
        }


        // =========================
        // POST: /Notice/Create
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notice notice)
        {
            if (ModelState.IsValid)
            {
                notice.CreatedAt = DateTime.Now;

                _context.Notices.Add(notice);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Notice created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Notices/Create.cshtml", notice);
        }


        // =========================
        // GET: /Notice/Edit/5
        // =========================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notice = await _context.Notices
                .FindAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            return View("~/Views/Notices/Edit.cshtml", notice);
        }


        // =========================
        // POST: /Notice/Edit/5
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Notice notice)
        {
            if (id != notice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notice);

                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Notice updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticeExists(notice.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return View("~/Views/Notices/Edit.cshtml", notice);
        }


        // =========================
        // GET: /Notice/Delete/5
        // =========================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notice = await _context.Notices
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notice == null)
            {
                return NotFound();
            }

            return View("~/Views/Notices/Delete.cshtml", notice);
        }


        // =========================
        // POST: /Notice/Delete/5
        // =========================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notice = await _context.Notices
                .FindAsync(id);

            if (notice == null)
            {
                return NotFound();
            }

            _context.Notices.Remove(notice);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Notice deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // Check Notice Exists
        // =========================

        private bool NoticeExists(int id)
        {
            return _context.Notices
                .Any(n => n.Id == id);
        }
    }
}
