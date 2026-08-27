namespace LibraryManagementSystem.Models
{
    public class LibrarySetting
    {
        public int Id { get; set; }

        public bool IsOpen { get; set; }

        public TimeSpan OpeningTime { get; set; }

        public TimeSpan ClosingTime { get; set; }
    }
}
