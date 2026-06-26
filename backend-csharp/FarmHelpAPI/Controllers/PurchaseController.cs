using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api/purchases")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/purchases — get all purchases (matches Python)
        [HttpGet]
        public async Task<IActionResult> GetPurchases()
        {
            try
            {
                var purchases = await _context.Purchases.ToListAsync();

                var result = purchases.Select(p => new PurchaseResponse
                {
                    Id = p.Id,
                    BuyerId = p.BuyerId,
                    ProductType = p.ProductType,
                    Quantity = p.Quantity,
                    Budget = p.Budget,
                    Deadline = p.Deadline?.ToString("yyyy-MM-dd"),
                    ContactName = p.ContactName,
                    ContactPhone = p.ContactPhone,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST /api/purchases — create purchase (matches Python)
        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] PurchaseCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.ProductType))
                {
                    return BadRequest(new { error = "需求类型不能为空" });
                }
                if (request.Quantity <= 0)
                {
                    return BadRequest(new { error = "数量必须大于0" });
                }
                if (string.IsNullOrEmpty(request.ContactPhone))
                {
                    return BadRequest(new { error = "请填写联系电话" });
                }

                var purchase = new Purchase
                {
                    BuyerId = request.BuyerId,
                    ProductType = request.ProductType,
                    Quantity = request.Quantity,
                    Budget = request.Budget,
                    Deadline = !string.IsNullOrEmpty(request.Deadline) && DateTime.TryParse(request.Deadline, out var dl) ? dl : null,
                    ContactName = request.ContactName,
                    ContactPhone = request.ContactPhone,
                    Status = "active",
                    CreatedAt = DateTime.Now
                };

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                return Ok(new { message = "采购需求发布成功", purchase_id = purchase.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // PUT /api/purchases/{id} — update purchase status (matches Python)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] System.Text.Json.JsonElement request)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound(new { error = "采购需求不存在" });
            }

            if (request.TryGetProperty("status", out var statusElem))
            {
                purchase.Status = statusElem.GetString() ?? purchase.Status;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "采购需求更新成功" });
        }

        // DELETE /api/purchases/{id} — delete purchase (matches Python)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound(new { error = "采购需求不存在" });
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return Ok(new { message = "采购需求删除成功" });
        }
    }
}
