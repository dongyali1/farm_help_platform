using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api/trace")]
    [ApiController]
    public class TraceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TraceController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/trace/{code} — query trace info by trace code string (matches Python)
        [HttpGet("{code}")]
        public async Task<IActionResult> GetTraceByCode(string code)
        {
            var traceCode = await _context.TraceCodes
                .Include(t => t.Product)
                .FirstOrDefaultAsync(t => t.Code == code);

            if (traceCode == null)
            {
                return NotFound(new { error = "溯源码不存在" });
            }

            var traces = await _context.Traces
                .Where(t => t.ProductId == traceCode.ProductId)
                .OrderBy(t => t.Time)
                .ToListAsync();

            var result = traces.Select(t => new TraceResponse
            {
                NodeType = t.NodeType,
                Location = t.Location,
                Time = t.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                Description = t.Description,
                Operator = t.Operator
            });

            return Ok(result);
        }

        // POST /api/trace/generate-code — generate a trace code (matches Python)
        [HttpPost("generate-code")]
        public IActionResult GenerateTraceCode([FromBody] System.Text.Json.JsonElement request)
        {
            if (!request.TryGetProperty("product_id", out var productIdElem))
            {
                return BadRequest(new { error = "缺少商品ID" });
            }

            int productId = productIdElem.GetInt32();
            string code = $"SN{productId}{Guid.NewGuid().ToString().Replace("-", "")[..12]}";

            var traceCode = new TraceCode
            {
                Code = code,
                ProductId = productId,
                CreatedAt = DateTime.Now
            };

            _context.TraceCodes.Add(traceCode);
            _context.SaveChanges();

            return Ok(new { code, message = "溯源码生成成功" });
        }
    }
}
