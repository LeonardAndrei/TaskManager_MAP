using System;
using System.Collections.Generic;
using NUnit.Framework;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Data;

namespace TaskManager.Tests
{
    [TestFixture]
    public class Lab4Tests
    {
        // 1. Test ISP - Verificam ca ReportService primeste DOAR ITaskReader
        [Test]
        public void ReportService_PoateFiConstruitCuInMemoryRepository_DemonstreazaISP()
        {
            // Arrange
            // InMemoryTaskRepository implementeaza ambele roluri (ITaskReader si ITaskWriter)
            ITaskReader reader = new InMemoryTaskRepository();

            // Act
            // Dar ReportService primeste si are acces doar la portiunea de citire!
            var reportService = new ReportService(reader);

            // Assert
            Assert.That(reportService, Is.Not.Null);
        }

        // 2. Test validare sumar ReportService
        [Test]
        public void GenerateSummary_ReturneazaNumarulCorectDeSarcini()
        {
            // Arrange
            var repo = new InMemoryTaskRepository();

            var t1 = new DeadlineTask { Title = "Task 1" };
            var t2 = new DeadlineTask { Title = "Task 2" };
            repo.Add(t1);
            repo.Add(t2);

            var reportService = new ReportService(repo);

            // Act
            var summary = reportService.GenerateSummary();

            // Assert
            Assert.That(summary, Does.Contain("Total sarcini: 2"));
            Assert.That(summary, Does.Contain("Finalizate: 0"));
        }

        // 3. Test OCP/DIP - MockNotifier este apelat corect prin interfata
        [TestCase(NotificationType.Console)]
        [TestCase(NotificationType.Email)]
        [TestCase(NotificationType.FileLog)]
        public void TaskService_ApeleazaNotifierulCorect_CandUnTaskEsteComplet(NotificationType type)
        {
            // Arrange
            var repo = new InMemoryTaskRepository();
            var validator = new TaskValidator();
            var mockNotifier = new MockNotifier();

            // Construim dictionarul cerut de serviciu
            var notifiers = new Dictionary<NotificationType, ITaskNotifier>
            {
                { type, mockNotifier }
            };

            // TaskService nu stie ca primeste un mock, el depinde de abstractie (DIP)
            var service = new TaskService(repo, validator, notifiers);

            var task = new DeadlineTask
            {
                Title = "Test Notification",
                NotificationType = type
            };
            repo.Add(task);

            // Act
            service.CompleteTask(task.Id);

            // Assert
            Assert.That(mockNotifier.A_Fost_Apelat, Is.True, $"Notifierul pentru tipul {type} nu a fost apelat.");
        }
    }

    // Clasa Mock care implementeaza ITaskNotifier doar pentru teste
    public class MockNotifier : ITaskNotifier
    {
        public bool A_Fost_Apelat { get; private set; } = false;

        public void Notify(TaskItem task)
        {
            A_Fost_Apelat = true;
        }
    }
}