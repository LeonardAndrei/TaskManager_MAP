using System;
using System.Collections.Generic;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repository;
        private readonly TaskValidator _validator;
        private readonly IReadOnlyDictionary<NotificationType, ITaskNotifier> _notifiers;

        public TaskService(
            ITaskRepository repository,
            TaskValidator validator,
            IReadOnlyDictionary<NotificationType, ITaskNotifier> notifiers)
        {
            _repository = repository;
            _validator = validator;
            _notifiers = notifiers;
        }

        public void AddTask(TaskItem task)
        {
            _validator.Validate(task);
            _repository.Add(task);
        }

        public IEnumerable<TaskItem> GetAllTasks()
        {
            return _repository.GetAll();
        }

        public void CompleteTask(int id)
        {
            var task = _repository.GetById(id);

            if (task == null)
                throw new ArgumentException($"Eroare: Nu am gasit niciun task cu ID-ul {id}!");

            task.Complete();

            _repository.Update(task);

            if (_notifiers.TryGetValue(task.NotificationType, out var notifier))
            {
                notifier.Notify(task);
            }
        }

        public void DeleteTask(int id)
        {
            _repository.Delete(id);
        }
    }
}