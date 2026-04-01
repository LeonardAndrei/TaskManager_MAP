using System;
using TaskManager.Core.Models;
using TaskManager.Core.Services;
using TaskManager.Data;

var repository = new InMemoryTaskRepository();
var taskService = new TaskService(repository);

try
{
    Console.WriteLine("Adaugam taskuri");


    taskService.AddTask(new TaskItem { Title = "Fa curat pe desktop", Type = TaskType.Standard });
    taskService.AddTask(new TaskItem { Title = "Rezolva bug-urile din codul de la Minesweeper", Type = TaskType.Bug });
    taskService.AddTask(new TaskItem { Title = "Task de sters", Type = TaskType.Review });

    AfiseazaTaskuri(taskService);

    Console.WriteLine("\n\n---------------------");
    Console.WriteLine("Completam primul task");
    taskService.CompleteTask(1);

    AfiseazaTaskuri(taskService);

    Console.WriteLine("\n\n---------------------");
    Console.WriteLine("Stergem al treilea task");
    taskService.DeleteTask(3);

    AfiseazaTaskuri(taskService);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"\n[EROARE DE VALIDARE] {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"\n[EROARE SISTEM] {ex.Message}");
}

Console.WriteLine("\nApasa tasta ENTER pentru a inchide consola...");
Console.ReadLine();

static void AfiseazaTaskuri(TaskService service)
{
    Console.WriteLine("\n> Lista de task-uri:");

    var allTasks = service.GetAllTasks();

    foreach (var task in allTasks)
    {
        Console.WriteLine($"  [{task.Id}] {task.Title} | Tip: {task.Type} | Status: {task.Status}");
    }
}