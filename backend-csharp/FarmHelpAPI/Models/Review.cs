namespace FarmHelpAPI.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int Rating { get; set; } // 1-5

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Product? Product { get; set; }
        public User? User { get; set; }
    }

    public class ReviewCreateRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Content { get; set; }
    }

    public class ReviewResponse
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
