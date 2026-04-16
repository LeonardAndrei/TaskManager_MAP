using System;
using System.Collections.Generic;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Data;
using TaskManager.Core.Notifications;
using TaskManager.Core.Interfaces;

// 1. Initializarea aplicatiei (Aici era problema ta, lipseau liniile astea)
var repository = new SqliteTaskRepository();
var validator = new TaskValidator();
var notifiers = new Dictionary<NotificationType, ITaskNotifier>
{
    { NotificationType.Console, new ConsoleNotifier() },
    { NotificationType.Email, new EmailNotifier() },
    { NotificationType.FileLog, new FileLogNotifier() }
};

var taskService = new TaskService(repository, validator, notifiers);

// 2. Meniul aplicatiei
while (true)
{
    Console.Clear();
    Console.WriteLine("=== TASK MANAGER ===");
    Console.WriteLine("1. Vezi toate task-urile");
    Console.WriteLine("2. Adauga un Deadline Task");
    Console.WriteLine("3. Adauga un Recurring Task");
    Console.WriteLine("4. Marcheaza un task ca DONE");
    Console.WriteLine("5. Sterge un task");
    Console.WriteLine("6. Iesire");
    Console.Write("\nAlege o optiune: ");

    var input = Console.ReadLine();

    try
    {
        Console.WriteLine();
        switch (input)
        {
            case "1":
                AfiseazaTaskuri(taskService);
                break;
            case "2":
                Console.Write("Titlu task: ");
                var titluDeadline = Console.ReadLine();
                var deadlineTask = new DeadlineTask
                {
                    Title = titluDeadline,
                    DueDate = DateTime.UtcNow.AddDays(2),
                    NotificationType = NotificationType.Console
                };
                taskService.AddTask(deadlineTask);
                Console.WriteLine("Deadline Task adaugat cu succes!");
                break;
            case "3":
                Console.Write("Titlu task: ");
                var titluRecurent = Console.ReadLine();
                var recurringTask = new RecurringTask
                {
                    Title = titluRecurent,
                    RecurrenceInterval = 7,
                    NotificationType = NotificationType.Console
                };
                taskService.AddTask(recurringTask);
                Console.WriteLine("Recurring Task adaugat cu succes!");
                break;
            case "4":
                Console.Write("Introdu ID-ul task-ului pe care vrei sa il termini: ");
                if (int.TryParse(Console.ReadLine(), out int idDone))
                {
                    taskService.CompleteTask(idDone);
                }
                else
                {
                    Console.WriteLine("ID invalid!");
                }
                break;
            case "5":
                Console.Write("Introdu ID-ul task-ului pe care vrei sa il stergi: ");
                if (int.TryParse(Console.ReadLine(), out int idDelete))
                {
                    taskService.DeleteTask(idDelete);
                    Console.WriteLine("Task sters cu succes!");
                }
                else
                {
                    Console.WriteLine("ID invalid!");
                }
                break;
            case "6":
                return;
            default:
                Console.WriteLine("Optiune invalida. Incearca din nou.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[EROARE] {ex.Message}");
    }

    Console.WriteLine("\nApasa tasta ENTER pentru a continua...");
    Console.ReadLine();
}

static void AfiseazaTaskuri(TaskService service)
{
    var tasks = service.GetAllTasks();
    Console.WriteLine("--- LISTA TASK-URI ---");
    foreach (var task in tasks)
    {
        Console.WriteLine($"[{task.Id}] {task.Title} | Tip: {task.TaskType} | Status: {task.Status}");
    }
}