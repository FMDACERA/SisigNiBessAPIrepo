using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisigNiBessWebApiAdmin.Database.Service;

namespace SisigNiBessWebApiAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupEmailController : ControllerBase
    {
        private readonly IBackupService _backupService;

        public BackupEmailController(IBackupService backupService)
        {
            _backupService = backupService;
        }
        [HttpGet("trigger-email")]
        public IActionResult TriggerBackupEmail()
        {
            // Start the backup task on a background thread without awaiting it here
            _ = Task.Run(async () =>
            {
                try
                {
                    await _backupService.GenerateAndEmailBackupAsync();
                }
                catch (Exception ex)
                {
                    // Log background errors so you can see them in Render logs
                    Console.WriteLine($"Background Backup Error: {ex.Message}");
                }
            });

            // Immediately reply to the cron job so it doesn't time out
            return Ok(new { message = "Backup process started in the background." });
        }
        //[HttpPost("trigger-email")]
        //public async Task<IActionResult> TriggerBackupEmail()
        //{
        //    try
        //    {
        //        await _backupService.GenerateAndEmailBackupAsync();
        //        return Ok(new { message = "Database backup successfully generated and emailed." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "Backup execution failed.", details = ex.Message });
        //    }
        //}
    }
}
