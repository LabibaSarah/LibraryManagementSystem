using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            // =========================
            // DASHBOARD STATISTICS
            // =========================

            // Total Books
            ViewBag.TotalBooks = await _context.Books.CountAsync();

            // Total Registered Users
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();

            // Currently Borrowed Books
            // ReturnDate == null means the book has not been returned yet
            ViewBag.BorrowedBooks = await _context.Transactions
                .CountAsync(t => t.ReturnDate == null);

            // Currently Overdue Books
            // Book is not returned and DueDate has already passed
            ViewBag.OverdueBooks = await _context.Transactions
                .CountAsync(t =>
                    t.ReturnDate == null &&
                    t.DueDate.Date < DateTime.Now.Date);

            return View();
        }


        // GET: /Admin/Users
        public async Task<IActionResult> Users(string? search)
        {
            var users = await _userManager.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();

            // Search by name, university ID or email
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                users = users
                    .Where(u =>
                        u.FullName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        u.UniversityId.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase) ||

                        (u.Email != null &&
                         u.Email.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userRoles[user.Id] =
                    roles.FirstOrDefault() ?? "No Role";
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.Search = search;

            return View(users);
        }


        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting an Admin account
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                TempData["Error"] =
                    "Admin accounts cannot be deleted.";

                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] =
                    "User deleted successfully.";
            }
            else
            {
                TempData["Error"] =
                    "Unable to delete the user.";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}