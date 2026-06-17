using MailKit.Net.Smtp;
using MimeKit;
using MySqlConnector;
using System.IO;
using System.Threading.Tasks;

namespace SisigNiBessWebApiAdmin.Database.Service
{
    public interface IBackupService
    {
        Task GenerateAndEmailBackupAsync();
    }
    public class BackupService : IBackupService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BackupService> _logger;
        public BackupService(IConfiguration configuration, ILogger<BackupService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task GenerateAndEmailBackupAsync()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // 1. Generate the MySQL dump completely in memory
            using var memoryStream = new MemoryStream();

            _logger.LogInformation("Starting MySQL dump generation...");

            using (var conn = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand { Connection = conn })
                {
                    using (var backupEngine = new MySqlBackup(cmd))
                    {
                        await conn.OpenAsync();

                        // Exports the database directly into our memory stream
                        backupEngine.ExportToMemoryStream(memoryStream);

                        await conn.CloseAsync();
                    }
                }
            }

            _logger.LogInformation("MySQL dump completed. Preparing email...");

            // Rewind the stream to the beginning so MailKit can read it properly
            memoryStream.Position = 0;

            // 2. Build the Email Message using MimeKit
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpSettings["SenderName"], smtpSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("Database Administrator", smtpSettings["RecipientEmail"]));
            message.Subject = $"Automated MySQL Backup - {DateTime.Now:yyyy-MM-dd HH:mm}";

            // Create the email body text
            var bodyBuilder = new BodyBuilder
            {
                TextBody = $"Hello Admin,\n\nPlease find attached the scheduled database backup generated on {DateTime.Now}.\n\nRegards,\nSisig Ni Bess - API System"
            };

            // Attach the memory stream as a file
            string fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
            bodyBuilder.Attachments.Add(fileName, memoryStream.ToArray(), ContentType.Parse("application/sql"));
            message.Body = bodyBuilder.ToMessageBody();

            // 3. Send via MailKit SmtpClient
            using var client = new SmtpClient();
            try
            {
                _logger.LogInformation("Connecting to SMTP server...");

                // Connect using STARTTLS (Port 587)
                await client.ConnectAsync(smtpSettings["Server"], int.Parse(smtpSettings["Port"]), MailKit.Security.SecureSocketOptions.StartTls);

                // Authenticate with your mail server
                await client.AuthenticateAsync(smtpSettings["SenderEmail"], smtpSettings["Password"]);

                _logger.LogInformation("Sending email attachment...");
                await client.SendAsync(message);

                _logger.LogInformation("Backup email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the backup email.");
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
