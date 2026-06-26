using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;

namespace FarmHelpAPI.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/stats/dashboard — homepage dashboard stats
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var approvedStatuses = new[] { "active", "approved" };
                var totalProducts = await _context.Products
                    .CountAsync(p => approvedStatuses.Contains(p.Status));
                var totalFarmers = await _context.Users
                    .CountAsync(u => u.Role == "farmer");
                var totalDemands = await _context.Purchases
                    .CountAsync(p => p.Status == "active");
                var totalReviews = await _context.Reviews.CountAsync();

                return Ok(new
                {
                    total_products = totalProducts,
                    total_farmers = totalFarmers,
                    total_demands = totalDemands,
                    total_reviews = totalReviews
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
