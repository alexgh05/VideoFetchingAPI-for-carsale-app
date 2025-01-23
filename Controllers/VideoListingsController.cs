using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VideoUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoListingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VideoListingsController> _logger;

        public VideoListingsController(ApplicationDbContext context, ILogger<VideoListingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("getvid")]
        public async Task<IActionResult> GetVideoListings()
        {
            try
            {
                var videoListings = await _context.userslistings.ToListAsync();

                if (videoListings == null || !videoListings.Any())
                {
                    _logger.LogWarning("No video listings found.");
                    return NotFound();
                }

             
               

                return Ok(videoListings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching video listings.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo([FromForm] userslisting userlisting, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedVideos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, file.FileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                userlisting.Video_path = filePath;

                // Log the userlisting object to see what's being received
                _logger.LogInformation("Received listing: {@userlisting}", userlisting);

                // Validate the model state
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    _logger.LogWarning("Model validation failed: {@Errors}", errors);
                    return BadRequest(ModelState);
                }

                // Check if the user exists
                var userExists = _context.usersdatas.Any(u => u.Username == userlisting.Username && u.Userphone == userlisting.Phone_number);
                if (!userExists)
                {
                    return BadRequest("User does not exist.");
                }

                _context.userslistings.Add(userlisting);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Video uploaded successfully", VideoId = userlisting.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the video.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }








    }
}
