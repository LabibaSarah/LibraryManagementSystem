using LibraryManagementSystem.Data;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        [Required]
        public int BookId { get; set; }

        public Book? Book { get; set; }

        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
    }
}
