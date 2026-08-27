using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Notice> Notices { get; set; }

        public DbSet<LibraryStatus> LibraryStatuses { get; set; }

        public DbSet<LibrarySetting> LibrarySettings { get; set; }

        // E-Books
        public DbSet<EBook> EBooks { get; set; }
    }
}