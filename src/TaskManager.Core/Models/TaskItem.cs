using System;

namespace TaskManager.Core.Models
{
    public enum TaskStatus { Todo, InProgress, Done }
    public enum NotificationType { Email, Console, FileLog, Telegram }
    public enum TaskType { Standard, Recurring, Deadline }

    public abstract class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public int Priority { get; set; } = 2;
        public NotificationType NotificationType { get; set; } = NotificationType.Console;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public abstract TaskType TaskType { get; }

        public void Complete()
        {
            if (Status == TaskStatus.Done)
                throw new InvalidOperationException("Task-ul este deja completat!");
            CompleteCore();
            if (Status != TaskStatus.Done)
                throw new InvalidOperationException("Eroare interna: Task-ul trebuia să fie completat!");
        }

        protected virtual void CompleteCore()
        {
            Status = TaskStatus.Done;
        }
    }
}