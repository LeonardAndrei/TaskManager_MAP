using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskNotifier
    {
        void Notify(TaskItem task);
    }
}