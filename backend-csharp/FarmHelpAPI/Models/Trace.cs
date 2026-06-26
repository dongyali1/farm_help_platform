namespace FarmHelpAPI.Models
{
    public class Trace
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string NodeType { get; set; } = string.Empty; // planting, processing, logistics

        public string? Location { get; set; }

        public DateTime Time { get; set; }

        public string? Description { get; set; }

        public string? Operator { get; set; }

        // Navigation property
        public Product? Product { get; set; }
    }

    public class TraceResponse
    {
        public string NodeType { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string Time { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Operator { get; set; }
    }
}
