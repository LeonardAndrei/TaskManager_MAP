using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Data
{
    public class SqliteTaskRepository : ITaskRepository
    {
        private const string ConnectionString = "Data Source=tasks.db";

        public SqliteTaskRepository()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    Status TEXT NOT NULL,
                    Priority INTEGER NOT NULL,
                    TaskType TEXT NOT NULL,
                    NotificationType TEXT NOT NULL,
                    DueDate TEXT,
                    RecurrenceInterval INTEGER,
                    CreatedAt TEXT NOT NULL
                );";
            command.ExecuteNonQuery();
        }

        public void Add(TaskItem task)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT INTO Tasks (Title, Description, Status, Priority, TaskType, NotificationType, DueDate, RecurrenceInterval, CreatedAt)
                VALUES ($title, $desc, $status, $priority, $type, $notifType, $dueDate, $recInterval, $createdAt)";

            command.Parameters.AddWithValue("$title", task.Title);
            command.Parameters.AddWithValue("$desc", task.Description ?? "");
            command.Parameters.AddWithValue("$status", task.Status.ToString());
            command.Parameters.AddWithValue("$priority", task.Priority);
            command.Parameters.AddWithValue("$type", task.TaskType.ToString());
            command.Parameters.AddWithValue("$notifType", task.NotificationType.ToString());
            command.Parameters.AddWithValue("$createdAt", task.CreatedAt.ToString("O"));

            if (task is DeadlineTask dt)
            {
                command.Parameters.AddWithValue("$dueDate", dt.DueDate.ToString("O"));
                command.Parameters.AddWithValue("$recInterval", DBNull.Value);
            }
            else if (task is RecurringTask rt)
            {
                command.Parameters.AddWithValue("$dueDate", rt.DueDate.ToString("O"));
                command.Parameters.AddWithValue("$recInterval", rt.RecurrenceInterval);
            }

            command.ExecuteNonQuery();
        }

        public IReadOnlyList<TaskItem> GetAll()
        {
            var tasks = new List<TaskItem>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Tasks";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(MapReaderToTask(reader));
            }
            return tasks;
        }

        public TaskItem GetById(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Tasks WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapReaderToTask(reader);
            }
            return null;
        }

        public void Update(TaskItem task)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"
                UPDATE Tasks 
                SET Status = $status 
                WHERE Id = $id";

            command.Parameters.AddWithValue("$status", task.Status.ToString());
            command.Parameters.AddWithValue("$id", task.Id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Tasks WHERE Id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.ExecuteNonQuery();
        }

        // Metoda care transforma ce vine din baza de date in obiecte C#
        private TaskItem MapReaderToTask(SqliteDataReader reader)
        {
            var type = Enum.Parse<TaskType>(reader.GetString(5));
            TaskItem task;

            if (type == TaskType.Deadline)
            {
                task = new DeadlineTask { DueDate = DateTime.Parse(reader.GetString(7)) };
            }
            else
            {
                task = new RecurringTask { RecurrenceInterval = 1 };
            }

            task.Id = reader.GetInt32(0);
            task.Title = reader.GetString(1);
            task.Status = Enum.Parse<TaskManager.Core.Models.TaskStatus>(reader.GetString(3));

            return task;
        }
    }
}