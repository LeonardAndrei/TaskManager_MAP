using System;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Notifications
{
    public class ConsoleNotifier : ITaskNotifier
    {
        public void Notify(TaskItem task)
        {
            Console.WriteLine($"[NOTIFICARE CONSOLA] Taskul '{task.Title}' a fost completat cu succes!");
        }
    }
}