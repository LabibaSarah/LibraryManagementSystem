using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class EBook
    {
        public int Id { get; set; }


        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;


        [Required]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;


        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;


        public string? Description { get; set; }


        // These are generated automatically
        // after the PDF is uploaded.

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;


        public DateTime UploadedAt { get; set; }
            = DateTime.Now;
    }
}