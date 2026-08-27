using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LibraryStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /LibraryStatus
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var status = await _context.LibraryStatuses
                .FirstOrDefaultAsync();

            if (status == null)
            {
                status = new LibraryStatus
                {
                    IsOpen = true,
                    OpeningTime = new TimeSpan(8, 0, 0),
                    ClosingTime = new TimeSpan(20, 30, 0),
                    UpdatedAt = DateTime.Now
                };

                _context.LibraryStatuses.Add(status);
                await _context.SaveChangesAsync();
            }

            return View(status);
        }

        // POST: /LibraryStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            bool isOpen,
            TimeSpan openingTime,
            TimeSpan closingTime)
        {
            if (openingTime >= closingTime)
            {
                ModelState.AddModelError(
                    "",
                    "Opening time must be earlier than closing time.");
            }

            if (!ModelState.IsValid)
            {
                var status = new LibraryStatus
                {
                    IsOpen = isOpen,
                    OpeningTime = openingTime,
                    ClosingTime = closingTime
                };

                return View(status);
            }

            var libraryStatus = await _context.LibraryStatuses
                .FirstOrDefaultAsync();

            if (libraryStatus == null)
            {
                libraryStatus = new LibraryStatus();

                _context.LibraryStatuses.Add(libraryStatus);
            }

            libraryStatus.IsOpen = isOpen;
            libraryStatus.OpeningTime = openingTime;
            libraryStatus.ClosingTime = closingTime;
            libraryStatus.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Library status updated successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
