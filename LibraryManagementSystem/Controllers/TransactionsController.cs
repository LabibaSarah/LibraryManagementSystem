using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .OrderByDescending(t => t.IssueDate)
                .ToListAsync();

            await LoadDropdowns();

            return View(transactions);
        }

        // POST: Transactions/Issue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue(
            string userId,
            int bookId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Please select a user.";
                return RedirectToAction(nameof(Index));
            }

            // Find user
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Get user's role
            var roles = await _userManager.GetRolesAsync(user);

            string? role = roles.FirstOrDefault();

            // Only Student and Faculty can borrow books
            if (role != "Student" && role != "Faculty")
            {
                TempData["Error"] =
                    "Only Student and Faculty users can borrow books.";

                return RedirectToAction(nameof(Index));
            }

            // Find book
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check available copies
            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] =
                    "This book is currently unavailable.";

                return RedirectToAction(nameof(Index));
            }

            // Find user's active borrowed books
            var activeBorrowCount = await _context.Transactions
                .CountAsync(t =>
                    t.UserId == userId &&
                    t.ReturnDate == null);

            // Borrowing limits
            int borrowLimit = role == "Student" ? 3 : 5;

            if (activeBorrowCount >= borrowLimit)
            {
                TempData["Error"] =
                    $"{role} users can borrow maximum {borrowLimit} books.";

                return RedirectToAction(nameof(Index));
            }

            // Prevent same user from borrowing the same book twice
            var alreadyBorrowed = await _context.Transactions
                .AnyAsync(t =>
                    t.UserId == userId &&
                    t.BookId == bookId &&
                    t.ReturnDate == null);

            if (alreadyBorrowed)
            {
                TempData["Error"] =
                    "This user has already borrowed this book.";

                return RedirectToAction(nameof(Index));
            }

            // Calculate due date
            int borrowingDays = role == "Student" ? 7 : 14;

            var transaction = new Transaction
            {
                UserId = userId,
                BookId = bookId,
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(borrowingDays),
                ReturnDate = null,
                Fine = 0
            };

            // Decrease available copies
            book.AvailableCopies--;

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Book successfully issued to {user.FullName}.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Transactions/Return
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Book)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                TempData["Error"] = "Transaction not found.";
                return RedirectToAction(nameof(Index));
            }

            // Already returned
            if (transaction.ReturnDate != null)
            {
                TempData["Error"] =
                    "This book has already been returned.";

                return RedirectToAction(nameof(Index));
            }

            var returnDate = DateTime.Now;

            transaction.ReturnDate = returnDate;

            // Calculate overdue fine
            if (returnDate.Date > transaction.DueDate.Date)
            {
                int overdueDays =
                    (returnDate.Date - transaction.DueDate.Date).Days;

                transaction.Fine = overdueDays * 5;
            }
            else
            {
                transaction.Fine = 0;
            }

            // Increase available copies
            if (transaction.Book != null)
            {
                transaction.Book.AvailableCopies++;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Book returned successfully. Fine: {transaction.Fine:0.00} Tk.";

            return RedirectToAction(nameof(Index));
        }

        // Load users and books for dropdowns
        private async Task LoadDropdowns()
        {
            var users = await _userManager.Users.ToListAsync();

            var borrowingUsers = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student") ||
                    roles.Contains("Faculty"))
                {
                    borrowingUsers.Add(new
                    {
                        Id = user.Id,
                        Name = $"{user.FullName} ({roles.FirstOrDefault()})"
                    });
                }
            }

            ViewBag.Users = new SelectList(
                borrowingUsers,
                "Id",
                "Name");

            var books = await _context.Books
                .Where(b => b.AvailableCopies > 0)
                .OrderBy(b => b.Title)
                .ToListAsync();

            ViewBag.Books = new SelectList(
                books,
                "Id",
                "Title");
        }
    }
}
