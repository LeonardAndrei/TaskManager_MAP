using System.Collections.Generic;
using System.Linq;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Data
{
    public class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskItem> _tasks = new List<TaskItem>();
        private int _nextId = 1;

        public IEnumerable<TaskItem> GetAll()
        {
            return _tasks;
        }

        public TaskItem GetById(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public void Add(TaskItem task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
        }

        public void Update(TaskItem task)
        {
            var existingTask = GetById(task.Id);
            if (existingTask != null)
            {
                existingTask.Title = task.Title;
                existingTask.Status = task.Status;
                existingTask.Type = task.Type;
            }
        }

        public void Delete(int id)
        {
            var task = GetById(id);
            if (task != null)
            {
                _tasks.Remove(task);
            }
        }
    }
}