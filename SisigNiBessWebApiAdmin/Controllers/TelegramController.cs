using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisigNiBessWebApiAdmin.Repository;

namespace SisigNiBessWebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        [HttpGet("process-queue")]
        public async Task<IActionResult> ProcessQueue([FromHeader(Name = "X-Checkly-Token")] string token)
        {
            // 1. Simple Security Check
            // Set this secret in your Render Environment Variables
            var secret = "Z@mZ1onZ3d232707";

            if (token == secret)
            {
                try
                {
                    // 2. Call your method
                    await TelegramRepository.SendTelegramMessage();

                    return Ok(new { message = "Queue processed successfully", timestamp = DateTime.UtcNow });
                }
                catch (Exception ex)
                {
                    // If something goes wrong, Checkly will see the 500 error
                    return StatusCode(500, $"Internal error: {ex.Message}");
                }
            }
            else
                return StatusCode(500, $"Internal error: invalid secret code.");


        }
    }
}
