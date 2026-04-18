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
            var secret = "Z@mZ1onZ3d232707";
            if (token == secret)
            {
                try
                {
                    await TelegramRepository.SendTelegramMessage();
                    return Ok(new { message = "Queue processed successfully", timestamp = DateTime.UtcNow });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal error: {ex.Message}");
                }
            }
            else
                return StatusCode(500, $"Internal error: invalid secret code.");
        }

        [HttpGet("notify-on-monday")]
        public async Task<IActionResult> NotifyOnMonday([FromHeader(Name = "X-Checkly-Token")] string token)
        {
            var secret = "Z@mZ1onZ3d232707";
            if (token == secret)
            {
                try
                {
                    await TelegramRepository.SendTelegramMondayNotification();
                    return Ok(new { message = "Notification sent successfully", timestamp = DateTime.UtcNow });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal error: {ex.Message}");
                }
            }
            else
                return StatusCode(500, $"Internal error: invalid secret code.");
        }
    }
}
