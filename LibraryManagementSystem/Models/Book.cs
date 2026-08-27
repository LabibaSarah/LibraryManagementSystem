using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ISBN { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        public int PublicationYear { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public string? Description { get; set; }

        // Foreign Key
        public int CategoryId { get; set; }

        // Navigation Property
        public Category? Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
