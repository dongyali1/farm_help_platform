using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/products?category=&farmer_id= — consumer view (only approved products)
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] string? category, [FromQuery] string? farmer_id)
        {
            var query = _context.Products
                .Include(p => p.Farmer)
                .Where(p => p.Status == "approved")
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }
            if (!string.IsNullOrEmpty(farmer_id) && int.TryParse(farmer_id, out int fid))
            {
                query = query.Where(p => p.FarmerId == fid);
            }

            var products = await query.ToListAsync();

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
                FarmerName = p.Farmer?.Username ?? "",
                FarmerPhone = p.Farmer?.Phone ?? "",
                FarmerAddress = p.Farmer?.Address ?? "",
                CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(result);
        }

        // GET /api/farmer/products?farmer_id= — farmer's own products (all statuses)
        [HttpGet("farmer/products")]
        public async Task<IActionResult> GetFarmerProducts([FromQuery] string? farmer_id)
        {
            if (string.IsNullOrEmpty(farmer_id))
            {
                return BadRequest(new { error = "缺少农户ID" });
            }

            if (!int.TryParse(farmer_id, out int fid))
            {
                return BadRequest(new { error = "农户ID必须是数字" });
            }

            var products = await _context.Products
                .Include(p => p.Farmer)
                .Where(p => p.FarmerId == fid)
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
                FarmerName = p.Farmer?.Username ?? "",
                FarmerPhone = p.Farmer?.Phone ?? "",
                FarmerAddress = p.Farmer?.Address ?? "",
                CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(result);
        }

        // POST /api/products — add product
        [HttpPost("products")]
        public async Task<IActionResult> AddProduct([FromBody] ProductCreateRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Category = request.Category,
                Price = request.Price,
                Stock = request.Stock,
                Description = request.Description,
                Images = request.Images,
                Origin = request.Origin,
                Location = request.Location,
                FarmerId = request.FarmerId,
                Status = request.Status,
                CreatedAt = DateTime.Now
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "商品发布成功", product_id = product.Id });
        }

        // PUT /api/products/{id} — update product
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductCreateRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "商品不存在" });
            }

            if (!string.IsNullOrEmpty(request.Name)) product.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Category)) product.Category = request.Category;
            if (request.Price > 0) product.Price = request.Price;
            if (request.Stock >= 0) product.Stock = request.Stock;
            if (request.Description != null) product.Description = request.Description;
            if (request.Images != null) product.Images = request.Images;
            if (request.Origin != null) product.Origin = request.Origin;
            if (request.Location != null) product.Location = request.Location;

            await _context.SaveChangesAsync();

            return Ok(new { message = "商品更新成功" });
        }

        // DELETE /api/products/{id} — delete product
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { error = "商品不存在" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "商品删除成功" });
        }
    }
}
