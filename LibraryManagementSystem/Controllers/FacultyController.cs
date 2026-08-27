using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacultyController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =========================================================
        // FACULTY DASHBOARD
        // =========================================================

        // GET: /Faculty
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Get all transactions of current faculty
            var transactions = await _context.Transactions
                .Include(t => t.Book)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.IssueDate)
                .ToListAsync();


            // Currently borrowed books
            var borrowedCount = transactions
                .Count(t => t.ReturnDate == null);


            // Overdue books
            var overdueCount = transactions
                .Count(t =>
                    t.ReturnDate == null &&
                    DateTime.Now.Date > t.DueDate.Date);


            // Total fine
            var totalFine = transactions
                .Sum(t => t.Fine);


            // Send statistics to View
            ViewBag.BorrowedCount = borrowedCount;
            ViewBag.OverdueCount = overdueCount;
            ViewBag.TotalFine = totalFine;


            return View(transactions);
        }


        // =========================================================
        // FACULTY PROFILE
        // =========================================================

        // GET: /Faculty/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            return View(user);
        }


        // =========================================================
        // UPDATE FACULTY PROFILE
        // =========================================================

        // POST: /Faculty/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            string fullName,
            string? universityId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }


            // Full Name validation
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Full Name is required.");
            }


            // University ID is OPTIONAL for Faculty
            // So there is no required validation here.


            if (!ModelState.IsValid)
            {
                return View(user);
            }


            // Update Full Name
            user.FullName = fullName.Trim();


            // Update University ID
            // Faculty can leave it empty.
            if (string.IsNullOrWhiteSpace(universityId))
            {
                user.UniversityId = "";
            }
            else
            {
                user.UniversityId = universityId.Trim();
            }


            // Save changes
            var result = await _userManager.UpdateAsync(user);


            if (result.Succeeded)
            {
                TempData["Success"] =
                    "Profile updated successfully.";

                return RedirectToAction(nameof(Profile));
            }


            // Display Identity errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }


            return View(user);
        }


        // =========================================================
        // CHANGE PASSWORD
        // =========================================================

        // GET: /Faculty/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        // POST: /Faculty/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            // Current password validation
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                ModelState.AddModelError(
                    "currentPassword",
                    "Current password is required.");
            }


            // New password validation
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError(
                    "newPassword",
                    "New password is required.");
            }


            // Confirm password validation
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError(
                    "confirmPassword",
                    "Please confirm your new password.");
            }


            // Password match validation
            if (!string.IsNullOrWhiteSpace(newPassword) &&
                newPassword != confirmPassword)
            {
                ModelState.AddModelError(
                    "confirmPassword",
                    "The new passwords do not match.");
            }


            if (!ModelState.IsValid)
            {
                return View();
            }


            // Get current faculty
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }


            // Change password
            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);


            if (result.Succeeded)
            {
                TempData["Success"] =
                    "Password changed successfully.";

                return RedirectToAction(nameof(Profile));
            }


            // Display password errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }


            return View();
        }
    }
}