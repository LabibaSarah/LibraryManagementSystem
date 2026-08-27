using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // HOME
        // =========================

        public async Task<IActionResult> Index()
        {
            // Load all book categories from database
            ViewBag.Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View();
        }


        // =========================
        // OTHER PAGES
        // =========================

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult EResources()
        {
            return View();
        }

        public IActionResult Thesis()
        {
            return View();
        }

        public IActionResult Research()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> EBooks()
        {
            var ebooks = await _context.EBooks
                .OrderByDescending(e => e.UploadedAt)
                .ToListAsync();

            return View(ebooks);
        }

        public IActionResult ResearchArticles()
        {
            return View();
        }

        public IActionResult Journals()
        {
            return View();
        }
    }
}