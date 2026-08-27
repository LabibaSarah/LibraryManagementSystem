using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // GET: Categories
        // =========================

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }


        // =========================
        // GET: Categories/Create
        // =========================

        public IActionResult Create()
        {
            return View();
        }


        // =========================
        // POST: Categories/Create
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Category created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }


        // =========================
        // GET: Categories/Edit/5
        // =========================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        // =========================
        // POST: Categories/Edit/5
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);

                    await _context.SaveChangesAsync();

                    TempData["Success"] =
                        "Category updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            return View(category);
        }


        // =========================
        // GET: Categories/Details/5
        // =========================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        // =========================
        // GET: Categories/Delete/5
        // =========================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        // =========================
        // POST: Categories/Delete/5
        // =========================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }


            // Check if books are using this category

            var hasBooks = await _context.Books
                .AnyAsync(b => b.CategoryId == id);

            if (hasBooks)
            {
                TempData["Error"] =
                    "This category cannot be deleted because books are assigned to it.";

                return RedirectToAction(nameof(Index));
            }


            // Delete category

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Category deleted successfully.";

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // Category Exists
        // =========================

        private bool CategoryExists(int id)
        {
            return _context.Categories
                .Any(c => c.Id == id);
        }
    }
}