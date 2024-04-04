using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FileBrowserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileBrowserController : ControllerBase
    {
        private readonly string _rootDirectory;

        public FileBrowserController()
        {
            _rootDirectory = @"C:\Users\jeanr\OneDrive\Desktop\TestProject\TempFolder"; // Replace with your root directory path
        }

        [HttpGet("browse")]
        public IActionResult Browse(string path)
        {
            var fullPath = Path.Combine(_rootDirectory, path);
            if (!Directory.Exists(fullPath))
                return NotFound("Directory not found.");

            var directories = Directory.GetDirectories(fullPath);
            var files = Directory.GetFiles(fullPath);

            return Ok(new { Directories = directories, Files = files });
        }

        [HttpGet("filecontent")]
        public IActionResult GetFileContent(string path)
        {
            var fullPath = Path.Combine(_rootDirectory, path);
            Console.WriteLine("Full Path: " + fullPath); // Log the full path

            if (!System.IO.File.Exists(fullPath))
            {
                Console.WriteLine("File does not exist."); // Log if file does not exist
                return NotFound("File not found.");
            }

            var content = System.IO.File.ReadAllText(fullPath);

            return Ok(content);
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(_rootDirectory, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("File uploaded successfully.");
        }

        [HttpGet("download/{fileName}")]
        public IActionResult Download(string fileName)
        {
            var filePath = Path.Combine(_rootDirectory, fileName);
            
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return File(fileStream, "application/octet-stream", fileName);
        }
    }
}