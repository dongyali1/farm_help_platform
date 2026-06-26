using Microsoft.AspNetCore.Mvc;
using FarmHelpAPI.Data;
using FarmHelpAPI.Models;

namespace FarmHelpAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST /api/login - matches Python exactly
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return Unauthorized(new { error = "用户名或密码错误" });
            }

            return Ok(new LoginResponse
            {
                Message = "登录成功",
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    Phone = user.Phone,
                    Address = user.Address
                }
            });
        }

        // POST /api/register - matches Python exactly
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            if (_context.Users.Any(u => u.Username == request.Username))
            {
                return BadRequest(new { error = "用户名已存在" });
            }

            var user = new User
            {
                Username = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                Phone = request.Phone,
                Address = request.Address,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new RegisterResponse
            {
                Message = "注册成功",
                UserId = user.Id
            });
        }
    }
}
