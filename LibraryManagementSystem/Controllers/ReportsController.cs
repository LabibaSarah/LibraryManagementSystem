using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Reports
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            // Total books
            var totalBooks = await _context.Books
                .SumAsync(b => b.TotalCopies);

            // Available books
            var availableBooks = await _context.Books
                .SumAsync(b => b.AvailableCopies);

            // Total transactions
            var totalTransactions = await _context.Transactions
                .CountAsync();

            // Today's issues
            var todayIssues = await _context.Transactions
                .CountAsync(t =>
                    t.IssueDate.Date == today);

            // Overdue books
            var overdueBooks = await _context.Transactions
                .CountAsync(t =>
                    t.ReturnDate == null &&
                    t.DueDate.Date < today);

            // Total fine
            var totalFine = await _context.Transactions
                .SumAsync(t => t.Fine);

            // Most borrowed books
            var mostBorrowedBooks = await _context.Transactions
                .Include(t => t.Book)
                .GroupBy(t => new
                {
                    t.BookId,
                    BookTitle = t.Book != null
                        ? t.Book.Title
                        : "Unknown Book"
                })
                .Select(g => new
                {
                    Title = g.Key.BookTitle,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(10)
                .ToListAsync();

            // Monthly borrowing report
            var monthlyBorrowing = await _context.Transactions
                .GroupBy(t => new
                {
                    t.IssueDate.Year,
                    t.IssueDate.Month
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Take(12)
                .ToListAsync();

            ViewBag.TotalBooks = totalBooks;
            ViewBag.AvailableBooks = availableBooks;
            ViewBag.TotalTransactions = totalTransactions;
            ViewBag.TodayIssues = todayIssues;
            ViewBag.OverdueBooks = overdueBooks;
            ViewBag.TotalFine = totalFine;
            ViewBag.MostBorrowedBooks = mostBorrowedBooks;
            ViewBag.MonthlyBorrowing = monthlyBorrowing;

            return View();
        }
    }
}
