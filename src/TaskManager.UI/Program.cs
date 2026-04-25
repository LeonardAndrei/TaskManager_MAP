using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Core.Notifications;
using TaskManager.Core.Services;
using TaskManager.Data;

namespace TaskManager.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Crearea colectiei de servicii (Containerul IoC)
            var services = new ServiceCollection();

            // 2. Inregistrarea dependentelor
            // Cand cineva cere ITaskRepository, ITaskReader sau ITaskWriter, dam aceeasi instanta de SqliteTaskRepository (Singleton)
            services.AddSingleton<SqliteTaskRepository>();
            services.AddSingleton<ITaskRepository>(sp => sp.GetRequiredService<SqliteTaskRepository>());
            services.AddSingleton<ITaskReader>(sp => sp.GetRequiredService<SqliteTaskRepository>());
            services.AddSingleton<ITaskWriter>(sp => sp.GetRequiredService<SqliteTaskRepository>());

            // Inregistram serviciile de business
            services.AddTransient<TaskValidator>();
            services.AddTransient<TaskService>();
            services.AddTransient<ReportService>();

            // Inregistram dictionarul de notificatori (OCP din Lab 3)
            services.AddSingleton<IReadOnlyDictionary<NotificationType, ITaskNotifier>>(sp =>
                new Dictionary<NotificationType, ITaskNotifier>
                {
                    { NotificationType.Console, new ConsoleNotifier() },
                    { NotificationType.Email, new EmailNotifier() },
                    { NotificationType.FileLog, new FileLogNotifier() },
                    { NotificationType.Telegram, new TelegramNotifier() }
                });

            // Inregistram Form-ul principal
            services.AddTransient<MainForm>();

            // 3. Construim Provider-ul
            var serviceProvider = services.BuildServiceProvider();

            // 4. Pornim aplicatia cerand MainForm din container
            // Containerul va injecta automat TaskService si ReportService in constructorul MainForm
            var mainForm = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
    }
}