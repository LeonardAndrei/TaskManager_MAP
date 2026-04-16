using System;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class TaskValidator
    {
        public void Validate(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Eroare: Titlul task-ului nu poate fi gol!");

            if (task.Title.Length > 200)
                throw new ArgumentException("Eroare: Titlul nu poate depasi 200 de caractere!");

            if (task is DeadlineTask deadlineTask)
            {
                if (deadlineTask.DueDate <= DateTime.UtcNow)
                {
                    throw new ArgumentException("Eroare: Termenul limita trebuie sa fie in viitor!");
                }
            }
        }
    }
}