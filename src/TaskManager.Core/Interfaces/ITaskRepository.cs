using System.Collections.Generic;
using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces
{
    public interface ITaskReader
    {
        IReadOnlyList<TaskItem> GetAll();
        TaskItem? GetById(int id);
    }

    public interface ITaskWriter
    {
        void Add(TaskItem task);
        void Update(TaskItem task);
        void Delete(int id);
    }

    public interface ITaskRepository : ITaskReader, ITaskWriter
    {
    }
}