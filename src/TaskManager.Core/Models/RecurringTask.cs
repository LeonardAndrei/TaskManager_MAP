using System;

namespace TaskManager.Core.Models
{
    public class RecurringTask : TaskItem
    {
        public override TaskType TaskType => TaskType.Recurring;
        
        public int RecurrenceInterval { get; set; }
        public DateTime DueDate { get; set; }

        protected override void CompleteCore()
        {
            Status = TaskStatus.Done;
            DueDate = DueDate.AddDays(RecurrenceInterval);
        }
    }
}