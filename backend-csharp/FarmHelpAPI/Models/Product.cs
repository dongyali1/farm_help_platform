using System.ComponentModel.DataAnnotations;

namespace FarmHelpAPI.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? Description { get; set; }

        [StringLength(500)]
        public string? Images { get; set; }

        [StringLength(100)]
        public string? Origin { get; set; }  // 产地

        [StringLength(200)]
        public string? Location { get; set; }  // 农产地点

        [StringLength(20)]
        public string Status { get; set; } = "pending"; // pending, approved, rejected, active, offline

        public int FarmerId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public User? Farmer { get; set; }
    }

    public class ProductCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }
        public string? Images { get; set; }
        public string? Origin { get; set; }
        public string? Location { get; set; }
        public int FarmerId { get; set; }
        public string Status { get; set; } = "pending";
    }

    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Description { get; set; }
        public List<string> Images { get; set; } = new();
        public string? Origin { get; set; }
        public string? Location { get; set; }
        public string Status { get; set; } = string.Empty;
        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;
        public string? FarmerPhone { get; set; }
        public string? FarmerAddress { get; set; }
        public string? CreatedAt { get; set; }
    }
}
