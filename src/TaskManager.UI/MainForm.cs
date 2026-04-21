using System;
using System.Drawing;
using System.Windows.Forms;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.UI
{
    public class MainForm : Form
    {
        private readonly TaskService _taskService;
        private readonly ReportService _reportService;

        // Controale UI
        private DataGridView _gridTasks;
        private TextBox _txtTaskTitle;
        private ComboBox _cmbTaskType;
        private Button _btnAddTask;
        private Button _btnCompleteTask;
        private Button _btnDeleteTask;

        public MainForm(TaskService taskService, ReportService reportService)
        {
            _taskService = taskService;
            _reportService = reportService;

            InitializeComponent();
            IncarcaTaskuri();
            ActualizeazaSumar();
        }

        private void InitializeComponent()
        {
            this.Text = "Task Manager - SOLID App";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Padding = new Padding(10);

            // Tabelul pentru afisarea sarcinilor (imbunatatit pentru text lung)
            _gridTasks = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 380,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false
            };
            _gridTasks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            // Cand dai dublu-click pe un rand, afisam detaliile
            _gridTasks.CellDoubleClick += GridTasks_CellDoubleClick;

            // Casuta de text pentru titlu
            _txtTaskTitle = new TextBox
            {
                Location = new Point(10, 400),
                Size = new Size(200, 30),
                PlaceholderText = "Scrie titlul task-ului..."
            };

            // Dropdown pentru tipul de task
            _cmbTaskType = new ComboBox
            {
                Location = new Point(220, 400),
                Size = new Size(120, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbTaskType.Items.Add("Deadline Task");
            _cmbTaskType.Items.Add("Recurring Task");
            _cmbTaskType.SelectedIndex = 0; // Selectam primul din lista default

            // Buton Adaugare
            _btnAddTask = new Button
            {
                Text = "Adauga Task",
                Location = new Point(350, 398),
                Size = new Size(100, 30)
            };
            _btnAddTask.Click += BtnAddTask_Click;

            // Buton Finalizare
            _btnCompleteTask = new Button
            {
                Text = "Marcheaza ca Done",
                Location = new Point(470, 398),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen
            };
            _btnCompleteTask.Click += BtnCompleteTask_Click;

            // Buton Stergere
            _btnDeleteTask = new Button
            {
                Text = "Sterge Task",
                Location = new Point(640, 398),
                Size = new Size(100, 30),
                BackColor = Color.LightCoral
            };
            _btnDeleteTask.Click += BtnDeleteTask_Click;

            // Adaugam controalele pe formular
            this.Controls.Add(_gridTasks);
            this.Controls.Add(_txtTaskTitle);
            this.Controls.Add(_cmbTaskType);
            this.Controls.Add(_btnAddTask);
            this.Controls.Add(_btnCompleteTask);
            this.Controls.Add(_btnDeleteTask);
        }

        private void IncarcaTaskuri()
        {
            var tasks = _taskService.GetAllTasks();
            _gridTasks.DataSource = null;
            _gridTasks.DataSource = tasks;
        }

        private void ActualizeazaSumar()
        {
            this.Text = "Task Manager | " + _reportService.GenerateSummary();
        }

        private void BtnAddTask_Click(object? sender, EventArgs e)
        {
            string titluNou = _txtTaskTitle.Text.Trim();

            if (string.IsNullOrEmpty(titluNou))
            {
                MessageBox.Show("Introdu un titlu!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                TaskItem newTask;
                string selectedType = _cmbTaskType.SelectedItem.ToString();

                // Decidem ce tip de obiect cream in functie de meniul derulant
                if (selectedType == "Recurring Task")
                {
                    newTask = new RecurringTask
                    {
                        Title = titluNou,
                        RecurrenceInterval = 7, // Default la 7 zile
                        NotificationType = NotificationType.Console
                    };
                }
                else
                {
                    newTask = new DeadlineTask
                    {
                        Title = titluNou,
                        DueDate = DateTime.UtcNow.AddDays(2), // Default la 2 zile
                        NotificationType = NotificationType.Console
                    };
                }

                _taskService.AddTask(newTask);

                _txtTaskTitle.Clear();
                IncarcaTaskuri();
                ActualizeazaSumar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare validare", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCompleteTask_Click(object? sender, EventArgs e)
        {
            if (_gridTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteaza un task pentru a-l completa!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int taskId = (int)_gridTasks.SelectedRows[0].Cells["Id"].Value;
                _taskService.CompleteTask(taskId);

                IncarcaTaskuri();
                ActualizeazaSumar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteTask_Click(object? sender, EventArgs e)
        {
            if (_gridTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteaza un task pentru a-l sterge!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cerem confirmare inainte de stergere
            var confirmResult = MessageBox.Show("Esti sigur ca vrei sa stergi acest task?", "Confirmare stergere", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    int taskId = (int)_gridTasks.SelectedRows[0].Cells["Id"].Value;
                    _taskService.DeleteTask(taskId);

                    IncarcaTaskuri();
                    ActualizeazaSumar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Eroare stergere", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GridTasks_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Daca utilizatorul da click pe header-ul coloanelor, ignoram
            if (e.RowIndex < 0) return;

            // Extragem datele de pe randul selectat
            var row = _gridTasks.Rows[e.RowIndex];
            string title = row.Cells["Title"].Value?.ToString() ?? "";
            string status = row.Cells["Status"].Value?.ToString() ?? "";
            string type = row.Cells["TaskType"].Value?.ToString() ?? "";

            string mesaj = $"Titlu: {title}\nStatus: {status}\nTip: {type}";

            MessageBox.Show(mesaj, "Detalii Task", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}