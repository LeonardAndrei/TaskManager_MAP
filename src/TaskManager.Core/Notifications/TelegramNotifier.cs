using System;
using System.Net.Http;
using System.Threading.Tasks;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Notifications
{
    public class TelegramNotifier : ITaskNotifier
    {
        // Pune aici datele tale preluate de la pasul A
        private readonly string _botToken = "8690542107:AAFpi7G-oZb0FIA3jwp3E9dF7Y6lAzLq0yk";
        private readonly string _chatId = "8683052135";
        private static readonly HttpClient _httpClient = new HttpClient();

        public void Notify(TaskItem task)
        {
            string message = $"✅ Task-ul '{task.Title}' a fost marcat ca DONE!";
            string url = $"https://api.telegram.org/bot{_botToken}/sendMessage?chat_id={_chatId}&text={Uri.EscapeDataString(message)}";

            // Trimitem mesajul catre API-ul Telegram
            Task.Run(() => _httpClient.GetAsync(url));
        }
    }
}