using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AdmissionInfoSystem.Attributes;

namespace AdmissionInfoSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileController> _logger;

        public FileController(IWebHostEnvironment environment, ILogger<FileController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload-logo")]
        [Authorize] // Cho phép cả admin và university user
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file nào được chọn" });
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif, .webp)" });
                }

                // Kiểm tra kích thước file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "File không được vượt quá 5MB" });
                }

                // Tạo thư mục uploads nếu chưa có
                var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "logos");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Tạo tên file unique
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL của file
                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/logos/{uniqueFileName}";
                
                return Ok(new { 
                    url = fileUrl,
                    fileName = uniqueFileName,
                    message = "Upload thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload logo");
                return StatusCode(500, new { message = "Lỗi khi upload file", error = ex.Message });
            }
        }

        [HttpDelete("delete-logo/{fileName}")]
        [Authorize] // Cho phép cả admin và university user
        public IActionResult DeleteLogo(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "logos", fileName);
                
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { message = "Xóa file thành công" });
                }
                
                return NotFound(new { message = "File không tồn tại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa logo");
                return StatusCode(500, new { message = "Lỗi khi xóa file", error = ex.Message });
            }
        }
    }
} 