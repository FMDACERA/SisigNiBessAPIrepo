using Microsoft.VisualBasic;
using SisigNiBessWebApiAdmin.Database.Model;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace SisigNiBessWebApiAdmin.Repository
{
    public class TelegramRepository
    {
        public async static Task SendTelegramMessage()
        {
            var RegDevices = await GetRegisteredDevices();
            var NotifQues = await GetNotificationQues();

            // Use a single HttpClient instance (better performance/memory)

            foreach (var que in NotifQues)
            {
                bool allSentSuccessfully = true;
                string message = "New inventory added for " + que.BRANCH_NAME + " branch.";

                foreach (var regDev in RegDevices)
                {
                    using var client = new HttpClient();

                    string url = $"https://api.telegram.org/bot{regDev.API_TOKEN}/sendMessage";

                    var payload = new { chat_id = regDev.CHAT_ID, text = message };
                    var json = JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        allSentSuccessfully = false; // Mark failure if one user didn't get it
                    }

                    client.Dispose();
                }

                // ONLY delete the record after it has been attempted for ALL users
                if (allSentSuccessfully)
                {
                    await DbServiceRepository.ExecuteNonQueryCommandAsync(
                        "DELETE FROM notification_que WHERE notification_id = " + que.NOTIFICATION_ID);
                }
            }
        }

        private static async Task<List<REGISTERED_DEVICES>> GetRegisteredDevices()
        {
            var devices = await DbServiceRepository.GetDataListAsync<REGISTERED_DEVICES>("SELECT * FROM registered_devices");
            return devices ?? new List<REGISTERED_DEVICES>();
        }

        private static async Task<List<NOTIFICATION_QUE>> GetNotificationQues()
        {
            var NotifQues = await DbServiceRepository.GetDataListAsync<NOTIFICATION_QUE>("SELECT * FROM notification_que");
            return NotifQues ?? new List<NOTIFICATION_QUE>();
        }
    }
}
