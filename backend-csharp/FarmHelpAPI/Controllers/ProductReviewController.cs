using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api/admin/products")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductReviewController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/admin/products/pending — get pending products for review
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProducts()
        {
            var products = await _context.Products
                .Include(p => p.Farmer)
                .Where(p => p.Status == "pending")
                .ToListAsync();

            var result = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Stock = p.Stock,
                Description = p.Description,
                Images = p.Images?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList() ?? new List<string>(),
                Origin = p.Origin,
                Location = p.Location,
                Status = p.Status,
                FarmerId = p.FarmerId,
                FarmerName = p.Farmer?.Username ?? "未知",
                FarmerPhone = p.Farmer?.Phone ?? "",
                FarmerAddress = p.Farmer?.Address ?? "",
                CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(result);
        }

        // GET /api/admin/products/all — get ALL products for admin management
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Farmer)
                .ToListAsync();

            var result = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Stock = p.Stock,
                Description = p.Description,
                Images = p.Images?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList() ?? new List<string>(),
                Origin = p.Origin,
                Location = p.Location,
                Status = p.Status,
                FarmerId = p.FarmerId,
                FarmerName = p.Farmer?.Username ?? "未知",
                FarmerPhone = p.Farmer?.Phone ?? "",
                FarmerAddress = p.Farmer?.Address ?? "",
                CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(result);
        }

        // PUT /api/admin/products/{id}/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "商品不存在" });
            }

            product.Status = "approved";
            await _context.SaveChangesAsync();

            return Ok(new { message = "商品审核通过" });
        }

        // PUT /api/admin/products/{id}/reject
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "商品不存在" });
            }

            product.Status = "rejected";
            await _context.SaveChangesAsync();

            return Ok(new { message = "商品已拒绝" });
        }

        // PUT /api/admin/products/{id}/offline
        [HttpPut("{id}/offline")]
        public async Task<IActionResult> OfflineProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "商品不存在" });
            }

            product.Status = "offline";
            await _context.SaveChangesAsync();

            return Ok(new { message = "商品已下架" });
        }
    }
}
