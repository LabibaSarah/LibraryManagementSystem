
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // GET: Books
        // =========================================================

        public async Task<IActionResult> Index(
            string? searchString,
            int? categoryId)
        {
            var booksQuery = _context.Books
                .Include(b => b.Category)
                .AsQueryable();


            // Search by Title, Author or ISBN

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                booksQuery = booksQuery.Where(b =>
                    b.Title.Contains(searchString) ||
                    b.Author.Contains(searchString) ||
                    (b.ISBN != null &&
                     b.ISBN.Contains(searchString)));
            }


            // Filter by Category

            if (categoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b =>
                    b.CategoryId == categoryId.Value);
            }


            // Category Dropdown

            await LoadCategoriesAsync(categoryId);


            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;


            var books = await booksQuery
                .OrderBy(b => b.Title)
                .ToListAsync();


            return View(books);
        }


        // =========================================================
        // GET: Books/Details/5
        // =========================================================

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


        // =========================================================
        // GET: Books/Create
        // =========================================================

        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();

            return View();
        }


        // =========================================================
        // POST: Books/Create
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Author,ISBN,Publisher,PublicationYear,TotalCopies,Description,CategoryId")]
            Book book)
        {

            // Available copies are automatically set
            // equal to total copies when a new book is added.

            book.AvailableCopies = book.TotalCopies;


            if (ModelState.IsValid)
            {
                book.CreatedAt = DateTime.Now;

                _context.Books.Add(book);

                await _context.SaveChangesAsync();


                TempData["Success"] =
                    "Book added successfully.";


                return RedirectToAction(nameof(Index));
            }


            // Reload categories if validation fails

            await LoadCategoriesAsync(book.CategoryId);


            return View(book);
        }


        // =========================================================
        // GET: Books/Edit/5
        // =========================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);


            if (book == null)
            {
                return NotFound();
            }


            await LoadCategoriesAsync(book.CategoryId);


            return View(book);
        }


        // =========================================================
        // POST: Books/Edit/5
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Title,Author,ISBN,Publisher,PublicationYear,TotalCopies,AvailableCopies,Description,CategoryId,CreatedAt")]
            Book book)
        {

            if (id != book.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Books.Update(book);

                    await _context.SaveChangesAsync();


                    TempData["Success"] =
                        "Book updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }


                return RedirectToAction(nameof(Index));
            }


            await LoadCategoriesAsync(book.CategoryId);


            return View(book);
        }


        // =========================================================
        // GET: Books/Delete/5
        // =========================================================

        public async Task<IActionResult> Delete(int? id)
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


        // =========================================================
        // POST: Books/Delete/5
        // =========================================================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .FindAsync(id);


            if (book == null)
            {
                return NotFound();
            }


            _context.Books.Remove(book);

            await _context.SaveChangesAsync();


            TempData["Success"] =
                "Book deleted successfully.";


            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // HELPER: Load Categories
        // =========================================================

        private async Task LoadCategoriesAsync(
            int? selectedCategoryId = null)
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();


            ViewBag.Categories = new SelectList(
                categories,
                "Id",
                "Name",
                selectedCategoryId);
        }


        // =========================================================
        // CHECK BOOK EXISTS
        // =========================================================

        private bool BookExists(int id)
        {
            return _context.Books
                .Any(b => b.Id == id);
        }
    }
}