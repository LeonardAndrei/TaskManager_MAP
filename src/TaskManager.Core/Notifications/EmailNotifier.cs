using System;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Notifications
{
    public class EmailNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task)
        {
            Console.WriteLine($"[NOTIFICARE EMAIL] Se trimite email pentru taskul '{task.Title}'...");
        }
    }
}