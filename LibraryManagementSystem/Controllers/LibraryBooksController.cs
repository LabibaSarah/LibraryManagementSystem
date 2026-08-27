using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    // Books browsing is public
    public class LibraryBooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LibraryBooksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // ==========================================
        // BOOK LIST - PUBLIC
        // ==========================================

        // GET: /LibraryBooks
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? search,
            int? categoryId)
        {
            var query = _context.Books
                .Include(b => b.Category)
                .AsQueryable();


            // Search by Title, Author, ISBN or Description

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Contains(search) ||
                    (b.ISBN != null &&
                     b.ISBN.Contains(search)) ||
                    (b.Description != null &&
                     b.Description.Contains(search)));
            }


            // Category Filter

            if (categoryId.HasValue)
            {
                query = query.Where(b =>
                    b.CategoryId == categoryId.Value);
            }


            var books = await query
                .OrderBy(b => b.Title)
                .ToListAsync();


            ViewBag.Categories =
                await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;


            return View(books);
        }


        // ==========================================
        // BOOK DETAILS - PUBLIC
        // ==========================================

        // GET: /LibraryBooks/Details/5

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);


            if (book == null)
            {
                return NotFound();
            }


            return View(book);
        }


        // ==========================================
        // RESERVE BOOK - LOGIN REQUIRED
        // ==========================================

        // POST: /LibraryBooks/Reserve/5

        [HttpPost]
        [Authorize(Roles = "Student,Faculty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(int id)
        {
            var userId = _userManager.GetUserId(User);


            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }


            // Find book

            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);


            if (book == null)
            {
                return NotFound();
            }


            // Reservation is only needed
            // when no copy is available

            if (book.AvailableCopies > 0)
            {
                TempData["Error"] =
                    "This book is currently available. You can borrow it instead.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }


            // Prevent duplicate active reservation

            var existingReservation =
                await _context.Reservations
                    .FirstOrDefaultAsync(r =>
                        r.UserId == userId &&
                        r.BookId == id &&
                        (r.Status == "Pending" ||
                         r.Status == "Approved"));


            if (existingReservation != null)
            {
                TempData["Error"] =
                    "You already have an active reservation for this book.";

                return RedirectToAction(
                    nameof(Details),
                    new { id });
            }


            // Create reservation

            var reservation = new Reservation
            {
                UserId = userId,
                BookId = id,
                ReservationDate = DateTime.Now,
                Status = "Pending"
            };


            _context.Reservations.Add(reservation);

            await _context.SaveChangesAsync();


            TempData["Success"] =
                "Book reserved successfully. Your reservation is pending approval.";


            return RedirectToAction(
                nameof(Details),
                new { id });
        }
    }
}
