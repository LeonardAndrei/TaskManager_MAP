using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Data;

namespace TaskManager.Tests
{
    public class TaskServiceTests
    {
        private InMemoryTaskRepository _repository;
        private TaskValidator _validator;
        private MockNotifier _mockNotifier;
        private TaskService _service;

        public class MockNotifier : ITaskNotifier
        {
            public bool WasCalled { get; private set; } = false;
            public void Notify(TaskItem task) { WasCalled = true; }
        }

        [SetUp]
        public void Setup()
        {
            _repository = new InMemoryTaskRepository();
            _validator = new TaskValidator();
            _mockNotifier = new MockNotifier();

            var notifiers = new Dictionary<NotificationType, ITaskNotifier>
            {
                { NotificationType.Console, _mockNotifier }
            };

            _service = new TaskService(_repository, _validator, notifiers);
        }

        [Test]
        public void Validate_TitluGol_AruncaExceptie()
        {
            var task = new DeadlineTask { Title = "" };
            Assert.Throws<ArgumentException>(() => _validator.Validate(task));
        }

        [Test]
        public void Validate_TitluPreaLung_AruncaExceptie()
        {
            var task = new DeadlineTask { Title = new string('A', 201) };
            Assert.Throws<ArgumentException>(() => _validator.Validate(task));
        }

        [Test]
        public void Validate_DueDateInTrecut_AruncaExceptie()
        {
            var task = new DeadlineTask { Title = "Test", DueDate = DateTime.UtcNow.AddDays(-1) };
            Assert.Throws<ArgumentException>(() => _validator.Validate(task));
        }

        [Test]
        public void CompleteTask_SarcinaDejaDone_AruncaExceptie()
        {
            var task = new DeadlineTask { Id = 1, Title = "Test", DueDate = DateTime.UtcNow.AddDays(1) };
            _repository.Add(task);

            _service.CompleteTask(1);

            Assert.Throws<InvalidOperationException>(() => _service.CompleteTask(1));
        }

        [TestCase("Deadline")]
        [TestCase("Recurring")]
        public void CompleteTask_SeteazaStatusDone_IndiferentDeSubtip(string tipTask)
        {
            TaskItem task;
            if (tipTask == "Deadline")
            {
                task = new DeadlineTask { Id = 1, Title = "Test D", DueDate = DateTime.UtcNow.AddDays(1) };
            }
            else
            {
                task = new RecurringTask { Id = 1, Title = "Test R", RecurrenceInterval = 5 };
            }

            _repository.Add(task);
            _service.CompleteTask(1);

            var updatedTask = _repository.GetById(1);

            Assert.That(updatedTask.Status, Is.EqualTo(TaskManager.Core.Models.TaskStatus.Done));
        }

        [Test]
        public void CompleteTask_DeclanseazaSistemulDeNotificari()
        {
            var task = new DeadlineTask { Id = 1, Title = "Test", NotificationType = NotificationType.Console, DueDate = DateTime.UtcNow.AddDays(1) };
            _repository.Add(task);

            _service.CompleteTask(1);

            Assert.That(_mockNotifier.WasCalled, Is.True);
        }

        [Test]
        public void AddTask_SalveazaCuSuccesInRepository()
        {
            var task = new RecurringTask { Title = "Task Nou", RecurrenceInterval = 1 };

            _service.AddTask(task);
            var tasks = _service.GetAllTasks().ToList();

            Assert.That(tasks.Count, Is.EqualTo(1));
            Assert.That(tasks[0].Title, Is.EqualTo("Task Nou"));
        }
    }
}