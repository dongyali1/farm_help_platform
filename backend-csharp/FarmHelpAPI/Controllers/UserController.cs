using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/users — get all users (matches Python)
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            var result = users.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                Phone = u.Phone,
                Address = u.Address,
                CreatedAt = u.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return Ok(result);
        }

        // PUT /api/users/{id} — update user (matches Python)
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] System.Text.Json.JsonElement request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "用户不存在" });
            }

            if (request.TryGetProperty("username", out var usernameElem))
                user.Username = usernameElem.GetString() ?? user.Username;
            if (request.TryGetProperty("phone", out var phoneElem))
                user.Phone = phoneElem.GetString();
            if (request.TryGetProperty("role", out var roleElem))
                user.Role = roleElem.GetString() ?? user.Role;
            if (request.TryGetProperty("address", out var addressElem))
                user.Address = addressElem.GetString();

            await _context.SaveChangesAsync();

            return Ok(new { message = "用户信息更新成功" });
        }

        // DELETE /api/users/{id} — delete user (matches Python)
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "用户不存在" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "用户删除成功" });
        }
    }
}
