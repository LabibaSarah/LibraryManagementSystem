using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Data;

namespace LibraryManagementSystem.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BookId { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public decimal Fine { get; set; } = 0;

        // Navigation Properties
        public LibraryManagementSystem.Data.ApplicationUser? User { get; set; }

        public Book? Book { get; set; }
    }
}
