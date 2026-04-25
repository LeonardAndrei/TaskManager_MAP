using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaskManager.Core.Models;
using TaskManager.Core.Services;

namespace TaskManager.UI
{
    public class MainForm : Form
    {
        private readonly TaskService _taskService;
        private readonly ReportService _reportService;

        private DataGridView _gridTasks;
        private TextBox _txtTaskTitle;
        private TextBox _txtTaskDescription;
        private ComboBox _cmbPriority;
        private ComboBox _cmbTaskType;
        private Label _lblDays;
        private NumericUpDown _numDays;
        private Button _btnAddTask;
        private Button _btnCompleteTask;
        private Button _btnDeleteTask;

        private bool _sortAscending = true;
        private const string DateFormat = "dd/MM/yyyy HH:mm";

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
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // 1. CONFIGURARE TABEL (DataGridView)
            _gridTasks = new DataGridView
            {
                Location = new Point(15, 15),
                Size = new Size(955, 350),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };

            _gridTasks.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            _gridTasks.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            _gridTasks.CellDoubleClick += GridTasks_CellDoubleClick;
            _gridTasks.ColumnHeaderMouseClick += GridTasks_ColumnHeaderMouseClick;
            _gridTasks.DataBindingComplete += GridTasks_DataBindingComplete;

            // 2. GRUPARE INPUT-URI
            GroupBox groupInput = new GroupBox { Text = "Adaugare Task Nou", Location = new Point(15, 380), Size = new Size(955, 120) };

            Label lblT = new Label { Text = "Titlu:", Location = new Point(15, 25), AutoSize = true };
            _txtTaskTitle = new TextBox { Location = new Point(15, 45), Size = new Size(180, 25) };

            Label lblD = new Label { Text = "Descriere:", Location = new Point(210, 25), AutoSize = true };
            _txtTaskDescription = new TextBox { Location = new Point(210, 45), Size = new Size(250, 25) };

            Label lblP = new Label { Text = "Prioritate:", Location = new Point(480, 25), AutoSize = true };
            _cmbPriority = new ComboBox { Location = new Point(480, 45), Size = new Size(100, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbPriority.Items.AddRange(new object[] { "1 - Low", "2 - Medium", "3 - High" });
            _cmbPriority.SelectedIndex = 1;

            Label lblTy = new Label { Text = "Tip Task:", Location = new Point(600, 25), AutoSize = true };
            _cmbTaskType = new ComboBox { Location = new Point(600, 45), Size = new Size(120, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbTaskType.Items.AddRange(new object[] { "Deadline Task", "Recurring Task" });
            _cmbTaskType.SelectedIndexChanged += CmbTaskType_SelectedIndexChanged;

            _lblDays = new Label { Text = "Zile pana la deadline:", Location = new Point(740, 25), AutoSize = true };
            _numDays = new NumericUpDown { Location = new Point(740, 45), Size = new Size(60, 25), Minimum = 1, Value = 2 };

            groupInput.Controls.AddRange(new Control[] { lblT, _txtTaskTitle, lblD, _txtTaskDescription, lblP, _cmbPriority, lblTy, _cmbTaskType, _lblDays, _numDays });

            // 3. BUTOANE ACTIUNI
            _btnAddTask = new Button { Text = "Adauga Task", Location = new Point(15, 520), Size = new Size(120, 40), BackColor = Color.LightSkyBlue, FlatStyle = FlatStyle.Flat };
            _btnAddTask.Click += BtnAddTask_Click;

            _btnCompleteTask = new Button { Text = "Marcheaza ca FINALIZAT", Location = new Point(150, 520), Size = new Size(180, 40), BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat };
            _btnCompleteTask.Click += BtnCompleteTask_Click;

            _btnDeleteTask = new Button { Text = "Sterge Task", Location = new Point(850, 520), Size = new Size(120, 40), BackColor = Color.LightCoral, FlatStyle = FlatStyle.Flat };
            _btnDeleteTask.Click += BtnDeleteTask_Click;

            _cmbTaskType.SelectedIndex = 0;

            this.Controls.AddRange(new Control[] { _gridTasks, groupInput, _btnAddTask, _btnCompleteTask, _btnDeleteTask });
        }

        private void IncarcaTaskuri(IEnumerable<TaskItem>? customList = null)
        {
            var tasks = customList?.ToList() ?? _taskService.GetAllTasks().ToList();
            _gridTasks.DataSource = null;
            _gridTasks.DataSource = tasks;
        }

        private void GridTasks_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (_gridTasks.Columns.Count == 0) return;

            // 1. Ascundem coloana ID
            if (_gridTasks.Columns.Contains("Id"))
                _gridTasks.Columns["Id"].Visible = false;

            // 2. Redenumim coloanele (Priority devine Prioritate!)
            Dictionary<string, string> headers = new Dictionary<string, string> {
                { "Title", "Titlu" }, { "Description", "Descriere" },
                { "Status", "Status" }, { "Priority", "Prioritate" },
                { "NotificationType", "Notificare" }, { "TaskType", "Tip" }
            };

            foreach (var header in headers)
            {
                if (_gridTasks.Columns.Contains(header.Key))
                {
                    _gridTasks.Columns[header.Key].HeaderText = header.Value;
                }
            }

            if (_gridTasks.Columns.Contains("Description"))
            {
                _gridTasks.Columns["Description"].Width = 200;
            }

            // 3. Setam formatul coloanei de data existenta
            if (_gridTasks.Columns.Contains("CreatedAt"))
            {
                _gridTasks.Columns["CreatedAt"].HeaderText = "Creat la";
                _gridTasks.Columns["CreatedAt"].DefaultCellStyle.Format = DateFormat;
            }

            // 4. Adaugam coloana custom pentru Termen (Fara bara oblica)
            if (!_gridTasks.Columns.Contains("Termen"))
            {
                _gridTasks.Columns.Add("Termen", "Termen");
            }

            // 5. Aranjam ordinea coloanelor ca sa arate logic (Datele una langa alta)
            if (_gridTasks.Columns.Contains("Title")) _gridTasks.Columns["Title"].DisplayIndex = 0;
            if (_gridTasks.Columns.Contains("Description")) _gridTasks.Columns["Description"].DisplayIndex = 1;
            if (_gridTasks.Columns.Contains("Status")) _gridTasks.Columns["Status"].DisplayIndex = 2;
            if (_gridTasks.Columns.Contains("Priority")) _gridTasks.Columns["Priority"].DisplayIndex = 3;
            if (_gridTasks.Columns.Contains("TaskType")) _gridTasks.Columns["TaskType"].DisplayIndex = 4;

            // Mutam Datele aici!
            if (_gridTasks.Columns.Contains("CreatedAt")) _gridTasks.Columns["CreatedAt"].DisplayIndex = 5;
            if (_gridTasks.Columns.Contains("Termen")) _gridTasks.Columns["Termen"].DisplayIndex = 6;

            if (_gridTasks.Columns.Contains("NotificationType")) _gridTasks.Columns["NotificationType"].DisplayIndex = 7;

            // 6. Completam valorile custom pentru Termen
            foreach (DataGridViewRow row in _gridTasks.Rows)
            {
                if (row.DataBoundItem is TaskItem task)
                {
                    if (task is DeadlineTask dt)
                        row.Cells["Termen"].Value = dt.DueDate.ToString(DateFormat);
                    else if (task is RecurringTask rt)
                        row.Cells["Termen"].Value = $"Repetare: {rt.RecurrenceInterval} zile";
                }
            }

            // 7. Scoatem selectia automata de pe primul element!
            _gridTasks.ClearSelection();
        }

        private void CmbTaskType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _lblDays.Text = _cmbTaskType.Text == "Recurring Task" ? "Interval (zile):" : "Zile pana la deadline:";
            _numDays.Value = _cmbTaskType.Text == "Recurring Task" ? 7 : 2;
        }

        private void BtnAddTask_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtTaskTitle.Text))
            {
                MessageBox.Show("Titlul este obligatoriu!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int p = int.Parse(_cmbPriority.Text.Substring(0, 1));
                TaskItem task = _cmbTaskType.Text == "Recurring Task"
                    ? new RecurringTask { Title = _txtTaskTitle.Text, Description = _txtTaskDescription.Text, Priority = p, RecurrenceInterval = (int)_numDays.Value, NotificationType = NotificationType.Telegram }
                    : new DeadlineTask { Title = _txtTaskTitle.Text, Description = _txtTaskDescription.Text, Priority = p, DueDate = DateTime.UtcNow.AddDays((double)_numDays.Value), NotificationType = NotificationType.Telegram };

                _taskService.AddTask(task);
                _txtTaskTitle.Clear(); _txtTaskDescription.Clear();
                IncarcaTaskuri();
                ActualizeazaSumar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCompleteTask_Click(object? sender, EventArgs e)
        {
            if (_gridTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteaza un task din tabel!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)_gridTasks.SelectedRows[0].Cells["Id"].Value;
            try
            {
                _taskService.CompleteTask(id);
                IncarcaTaskuri();
                ActualizeazaSumar();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnDeleteTask_Click(object? sender, EventArgs e)
        {
            if (_gridTasks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecteaza un task din tabel!", "Atentie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Stergi sarcina selectata?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _taskService.DeleteTask((int)_gridTasks.SelectedRows[0].Cells["Id"].Value);
                IncarcaTaskuri();
                ActualizeazaSumar();
            }
        }

        private void GridTasks_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            var column = _gridTasks.Columns[e.ColumnIndex];
            string prop = column.DataPropertyName;
            var list = _taskService.GetAllTasks();

            if (column.Name == "Termen")
            {
                var sortedTermen = _sortAscending
                    ? list.OrderBy(t => t is DeadlineTask dt ? dt.DueDate : DateTime.MaxValue).ToList()
                    : list.OrderByDescending(t => t is DeadlineTask dt ? dt.DueDate : DateTime.MaxValue).ToList();

                _sortAscending = !_sortAscending;
                IncarcaTaskuri(sortedTermen);
                return;
            }

            if (string.IsNullOrEmpty(prop)) return;

            var sorted = _sortAscending
                ? list.OrderBy(t => t.GetType().GetProperty(prop)?.GetValue(t)).ToList()
                : list.OrderByDescending(t => t.GetType().GetProperty(prop)?.GetValue(t)).ToList();

            _sortAscending = !_sortAscending;
            IncarcaTaskuri(sorted);
        }

        private void ActualizeazaSumar() => this.Text = "Task Manager | " + _reportService.GenerateSummary();

        private void GridTasks_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var t = (TaskItem)_gridTasks.Rows[e.RowIndex].DataBoundItem;
            string detaliu = t is DeadlineTask dt ? dt.DueDate.ToString(DateFormat) : (t is RecurringTask rt ? rt.RecurrenceInterval.ToString() : "-");
            MessageBox.Show($"TITLU: {t.Title}\nDESCRIERE: {t.Description}\nSTATUS: {t.Status}\nTERMEN: {detaliu}", "Detalii Task");
        }
    }
}