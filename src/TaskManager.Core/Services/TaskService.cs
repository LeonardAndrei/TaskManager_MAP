using System;
using System.Collections.Generic;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Core.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public void AddTask(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                throw new ArgumentException("Eroare: Titlul task-ului nu poate fi gol!");
            }

            if (task.Title.Length > 50)
            {
                throw new ArgumentException("Eroare: Titlul este prea lung (maxim 50 caractere)!");
            }

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
            {
                throw new ArgumentException($"Eroare: Nu am găsit niciun task cu ID-ul {id}!");
            }

            task.Complete();

            _repository.Update(task);
        }

        public void DeleteTask(int id)
        {
            _repository.Delete(id);
        }
    }
}