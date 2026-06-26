using System.ComponentModel.DataAnnotations;

namespace FarmHelpAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // farmer, consumer, admin

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Avatar { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }  // 农户地址 / 农产地点

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Message { get; set; } = "登录成功";
        public UserInfo User { get; set; } = new();
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "consumer";

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }

    public class RegisterResponse
    {
        public string Message { get; set; } = "注册成功";
        public int UserId { get; set; }
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
