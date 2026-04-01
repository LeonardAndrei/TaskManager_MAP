using System;

namespace TaskManager.Core.Models
{
    public enum TaskStatus
    {
        Todo,
        InProgress,
        Done
    }

    public enum TaskType
    {
        Standard,
        Bug,
        Review
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskType Type { get; set; } = TaskType.Standard;

        public void Complete()
        {
            Status = TaskStatus.Done;
        }
    }
}