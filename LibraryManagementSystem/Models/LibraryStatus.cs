using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class LibraryStatus
    {
        public int Id { get; set; }

        [Display(Name = "Library Open")]
        public bool IsOpen { get; set; } = true;

        [Display(Name = "Opening Time")]
        [Required]
        public TimeSpan OpeningTime { get; set; } = new TimeSpan(8, 0, 0);

        [Display(Name = "Closing Time")]
        [Required]
        public TimeSpan ClosingTime { get; set; } = new TimeSpan(20, 30, 0);

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
