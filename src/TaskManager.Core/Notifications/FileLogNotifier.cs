using System;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Notifications
{
    public class FileLogNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task)
        {
            Console.WriteLine($"[NOTIFICARE FISIER] Am scris in fisierul de log ca taskul '{task.Title}' este gata.");
        }
    }
}