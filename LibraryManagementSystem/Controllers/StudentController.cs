using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // =====================================================
        // STUDENT DASHBOARD
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Get student's transactions
            var transactions = await _context.Transactions
                .Include(t => t.Book)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.IssueDate)
                .ToListAsync();


            // Get student's reservations
            var reservations = await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservationDate)
                .ToListAsync();


            // =================================================
            // STUDENT STATISTICS
            // =================================================

            // Currently borrowed
            ViewBag.BorrowedCount = transactions
                .Count(t => t.ReturnDate == null);


            // Currently overdue
            ViewBag.OverdueCount = transactions
                .Count(t =>
                    t.ReturnDate == null &&
                    t.DueDate.Date < DateTime.Now.Date);


            // Total fine
            ViewBag.TotalFine = transactions
                .Sum(t => t.Fine);


            // Send reservations to View
            ViewBag.Reservations = reservations;


            return View(transactions);
        }


        // =====================================================
        // PROFILE - GET
        // =====================================================

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


        // =====================================================
        // PROFILE - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            string fullName,
            string universityId,
            string? email,
            string? phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }


            // Validate Full Name
            if (string.IsNullOrWhiteSpace(fullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Full Name is required.");
            }


            // Validate University ID
            if (string.IsNullOrWhiteSpace(universityId))
            {
                ModelState.AddModelError(
                    "UniversityId",
                    "University ID is required.");
            }


            // Validate Email
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Email is required.");
            }


            if (!ModelState.IsValid)
            {
                user.FullName = fullName;
                user.UniversityId = universityId;
                user.Email = email;
                user.PhoneNumber = phoneNumber;

                return View(user);
            }


            // Update basic profile information
            user.FullName = fullName.Trim();
            user.UniversityId = universityId.Trim();
            user.PhoneNumber = phoneNumber?.Trim();


            // Update email through Identity
            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();

                var emailResult =
                    await _userManager.SetEmailAsync(user, email);

                if (!emailResult.Succeeded)
                {
                    foreach (var error in emailResult.Errors)
                    {
                        ModelState.AddModelError(
                            "Email",
                            error.Description);
                    }

                    return View(user);
                }


                // If Username is the email,
                // keep username synchronized.
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    user.UserName = email;
                }
            }


            // Save remaining profile changes
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


        // =====================================================
        // CHANGE PASSWORD - GET
        // =====================================================

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        // =====================================================
        // CHANGE PASSWORD - POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }


            // Check current password
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                ModelState.AddModelError(
                    "currentPassword",
                    "Current password is required.");
            }


            // Check new password
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                ModelState.AddModelError(
                    "newPassword",
                    "New password is required.");
            }


            // Confirm password
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(
                    "confirmPassword",
                    "New passwords do not match.");
            }


            if (!ModelState.IsValid)
            {
                return View();
            }


            var result =
                await _userManager.ChangePasswordAsync(
                    user,
                    currentPassword,
                    newPassword);


            if (result.Succeeded)
            {
                TempData["Success"] =
                    "Password changed successfully.";

                return RedirectToAction(nameof(Profile));
            }


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
