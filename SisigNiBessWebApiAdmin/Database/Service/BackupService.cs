using MailKit.Net.Smtp;
using MimeKit;
using MySqlConnector;
using Org.BouncyCastle.Tls;
using System.IO;
using System.Text;
using System.Text.Json;
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
            // 1. Read the Connection String smoothly
            string connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new KeyNotFoundException("Could not find 'ConnectionStrings:DefaultConnection' in appsettings.json");

            // 2. Read Brevo settings sections using the correct colon notation
            string apiKey = _configuration["BrevoSettings:ApiKey"]
                ?? throw new KeyNotFoundException("Could not find 'BrevoSettings:ApiKey' in appsettings.json");

            string senderEmail = _configuration["BrevoSettings:SenderEmail"]
                ?? throw new KeyNotFoundException("Could not find 'BrevoSettings:SenderEmail' in appsettings.json");
           
            string senderName = _configuration["BrevoSettings:SenderName"]
                ?? throw new KeyNotFoundException("Could not find 'BrevoSettings:SenderName' in appsettings.json");

            string recipientEmail = _configuration["BrevoSettings:RecipientEmail"]
                ?? throw new KeyNotFoundException("Could not find 'BrevoSettings:RecipientEmail' in appsettings.json");

            // --- Database Dump Logic ---
            using var memoryStream = new MemoryStream();
            using (var conn = new MySqlConnection(connectionString))
            {
                using (var cmd = new MySqlCommand { Connection = conn })
                {
                    using (var backupEngine = new MySqlBackup(cmd))
                    {
                        await conn.OpenAsync();
                        backupEngine.ExportToMemoryStream(memoryStream);
                        await conn.CloseAsync();
                    }
                }
            }

            // --- HTTP Email Payload Sending Logic ---
            byte[] backupBytes = memoryStream.ToArray();
            string fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            
            string superScret =  (_configuration["BrevoSettings:KeyPart1"] ?? "") + (_configuration["BrevoSettings:KeyPart2"] ?? "");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("api-key", superScret);

            var emailPayload = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = recipientEmail, name = "System Admin" } },
                subject = $"Automated Database Backup - {DateTime.Now:yyyy-MM-dd}",
                htmlContent = "<h3>Database Backup</h3><p>Please find attached the scheduled MySQL dump file.</p>",
                attachment = new[]
                {
            new
            {
                content = Convert.ToBase64String(backupBytes),
                name = fileName
            }
        }
            };

            string jsonString = System.Text.Json.JsonSerializer.Serialize(emailPayload);
            var httpContent = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                string errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Brevo API Error: {errorDetails}");
            }
        }
    }
}
