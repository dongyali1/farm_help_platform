using Microsoft.AspNetCore.Mvc;

namespace FarmHelpAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // POST /api/upload — upload file (matches Python)
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "没有选择文件" });
            }

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
            {
                return BadRequest(new { error = "不支持的文件格式" });
            }

            var uploadsPath = Path.Combine(_env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filename = Guid.NewGuid().ToString("N") + ext;
            var filePath = Path.Combine(uploadsPath, filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"/uploads/{filename}";
            return Ok(new { url = fileUrl });
        }
    }
}
