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

        [HttpPost("trigger-email")]
        public async Task<IActionResult> TriggerBackupEmail()
        {
            try
            {
                await _backupService.GenerateAndEmailBackupAsync();
                return Ok(new { message = "Database backup successfully generated and emailed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Backup execution failed.", details = ex.Message });
            }
        }
    }
}
