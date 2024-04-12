using Microsoft.AspNetCore.Mvc;

namespace FileBrowserAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileBrowserController : ControllerBase
    {
        private readonly string _rootDirectory; // Root directory path
        private readonly string _uploadDirectory;

        public FileBrowserController()
        {
            // Configure the root directory (make it configurable via a variable)
            _rootDirectory = @"C:\Users"; // Example root directory path
            _uploadDirectory = @"C:\upload";

            if (!Directory.Exists(_uploadDirectory))
            {
                try
                {
                    // Create the directory if it doesn't exist
                    Directory.CreateDirectory(_uploadDirectory);
                    Console.WriteLine("Directory created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error creating directory: " + ex.Message);
                }
            }
        }

        [HttpGet("browse")]
        public IActionResult Browse(string path = "")
        {
            string fullPath = Path.Combine(_rootDirectory, path);
            if (!Directory.Exists(fullPath))
                return NotFound("Directory not found.");

            var directories = Directory.GetDirectories(fullPath);
            var files = Directory.GetFiles(fullPath);
            var fileInfos = new List<object>();
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                fileInfos.Add(new { Name = Path.GetFileName(file), Size = fileInfo.Length });
            }

            return Ok(new { Directories = directories, Files = fileInfos });
        }
                
        [HttpGet]
        public IActionResult GetFileContent(string fileName)
        {
            // Check if the file exists
            if (!System.IO.File.Exists(fileName))
                return NotFound("File not found.");

            try
            {
                // Read the content of the file
                string fileContent = System.IO.File.ReadAllText(fileName);

                // Return the file content as the response
                return Ok(fileContent);
            }
            catch (Exception ex)
            {
                // Return an error response if an exception occurs
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Generate a unique file name
                var fileName = Path.Combine(_uploadDirectory, file.FileName);

                using (var stream = new FileStream(fileName, FileMode.Create))
                {
                    // Save the uploaded file to the server
                    await file.CopyToAsync(stream);
                }

                return Ok("File uploaded successfully!");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }
        
    }
}



