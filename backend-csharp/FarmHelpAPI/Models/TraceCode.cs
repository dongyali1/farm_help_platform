using System.ComponentModel.DataAnnotations;

namespace FarmHelpAPI.Models
{
    public class TraceCode
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public int ProductId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Product? Product { get; set; }
    }
}
