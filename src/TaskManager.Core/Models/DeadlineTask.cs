using System;

namespace TaskManager.Core.Models
{
    public class DeadlineTask : TaskItem
    {
        public override TaskType TaskType => TaskType.Deadline;
        
        public DateTime DueDate { get; set; }

        protected override void CompleteCore()
        {
            Status = TaskStatus.Done;
        }
    }
}