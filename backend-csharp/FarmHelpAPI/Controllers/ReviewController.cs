using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/reviews — get all reviews (matches Python)
        [HttpGet]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .ToListAsync();

            var result = reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product?.Name ?? "",
                UserId = r.UserId,
                Username = r.User?.Username ?? "",
                Rating = r.Rating,
                Content = r.Content,
                CreatedAt = r.CreatedAt
            });

            return Ok(result);
        }

        // POST /api/reviews — add review (matches Python)
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewCreateRequest request)
        {
            var review = new Review
            {
                UserId = request.UserId,
                ProductId = request.ProductId,
                Rating = request.Rating,
                Content = request.Content,
                CreatedAt = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "评价成功" });
        }

        // DELETE /api/reviews/{id} — delete review (matches Python)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound(new { error = "评价不存在" });
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "评价删除成功" });
        }
    }
}
