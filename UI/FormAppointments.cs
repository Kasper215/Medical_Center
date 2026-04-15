using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MedCenterApp.Models;

namespace MedCenterApp.UI;

public class FormAppointments : Form
{
    private ComboBox _cmbDoctors;
    private MonthCalendar _calendar;
    private DataGridView _grid;
    private Button _btnAdd, _btnDelete;

    public FormAppointments()
    {
        InitializeComponent();
        Theme.ApplyToForm(this);
        LoadData();
    }

    private void InitializeComponent()
    {
        this.Text = "Расписание приёмов";
        this.Size = new Size(1000, 600);
        this.StartPosition = FormStartPosition.CenterParent;

        var panelLeft = new Panel { Dock = DockStyle.Left, Width = 350, Padding = new Padding(10) };
        
        var lblDoc = new Label { Text = "Врач:", Location = new Point(10, 20), Size = new Size(50, 20) };
        _cmbDoctors = new ComboBox { Location = new Point(70, 17), Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbDoctors.DataSource = AppDI.Doctors.GetAll();
        _cmbDoctors.DisplayMember = "FullName";
        _cmbDoctors.ValueMember = "Id";
        _cmbDoctors.SelectedIndexChanged += (s, e) => LoadSchedule();

        // MonthCalendar doesn't scale font properly in winforms sometimes, so we leave it as is
        _calendar = new MonthCalendar { Location = new Point(70, 60), MaxSelectionCount = 1 };
        _calendar.DateChanged += (s, e) => LoadSchedule();

        _btnAdd = new Button { Text = "Записать на приём", Location = new Point(70, 240), Width = 260 };
        _btnAdd.Click += (s, e) => BookSelected();

        _btnDelete = new Button { Text = "Перенести / Отменить запись", Location = new Point(70, 290), Width = 260 };
        _btnDelete.Click += (s, e) => CancelSelected();

        panelLeft.Controls.AddRange(new Control[] { lblDoc, _cmbDoctors, _calendar, _btnAdd, _btnDelete });

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            ReadOnly = true,
            AllowUserToAddRows = false
        };

        this.Controls.Add(_grid);
        this.Controls.Add(panelLeft);
    }

    private void LoadData()
    {
        if (_cmbDoctors.Items.Count > 0)
            _cmbDoctors.SelectedIndex = 0;
        LoadSchedule();
    }

    private void LoadSchedule()
    {
        if (_cmbDoctors.SelectedValue == null) return;
        
        int doctorId = (int)_cmbDoctors.SelectedValue;
        var date = _calendar.SelectionStart.Date;

        var appointments = AppDI.Appointments.GetAll()
            .Where(a => a.DoctorId == doctorId && a.DateTime.Date == date)
            .OrderBy(a => a.DateTime)
            .Select(a => new
            {
                ID = a.Id,
                Время = a.DateTime.ToString("HH:mm"),
                Пациент = AppDI.Patients.GetById(a.PatientId)?.FullName ?? "Неизвестен",
                Услуга = a.Service
            }).ToList();

        _grid.DataSource = appointments;
    }

    private void BookSelected()
    {
        if (_cmbDoctors.SelectedValue == null) return;
        
        DateTime selectedDate = _calendar.SelectionStart.Date;
        if (selectedDate < DateTime.Today)
        {
            MessageBox.Show("Записи на прошедшие даты не могут быть созданы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var form = new FormBookAppointment((int)_cmbDoctors.SelectedValue, selectedDate);
        if (form.ShowDialog() == DialogResult.OK)
        {
            LoadSchedule();
        }
    }

    private void CancelSelected()
    {
        if (_grid.CurrentRow != null)
        {
            int id = (int)_grid.CurrentRow.Cells["ID"].Value;
            var appt = AppDI.Appointments.GetById(id);
            if (appt != null && appt.DateTime.Date < DateTime.Today)
            {
                 MessageBox.Show("Архивные записи нельзя изменить.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
            }

            if (MessageBox.Show("Отменить этот прием?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AppDI.Appointments.Delete(id);
                LoadSchedule();
            }
        }
    }
}

public class FormBookAppointment : Form
{
    private int _doctorId;
    private DateTime _date;
    
    private ComboBox _cmbPatients;
    private ComboBox _cmbTime;
    private TextBox _txtService;
    private Button _btnSave, _btnCancel;

    public FormBookAppointment(int doctorId, DateTime date)
    {
        _doctorId = doctorId;
        _date = date;
        InitializeComponent();
        Theme.ApplyToForm(this);
    }

    private void InitializeComponent()
    {
        this.Text = $"Запись на {_date:dd.MM.yyyy}";
        this.Size = new Size(400, 260);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        Label lbl1 = new Label { Text = "Пациент:", Location = new Point(20, 20) };
        _cmbPatients = new ComboBox { Location = new Point(120, 18), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbPatients.DataSource = AppDI.Patients.GetAll();
        _cmbPatients.DisplayMember = "FullName";
        _cmbPatients.ValueMember = "Id";

        Label lbl2 = new Label { Text = "Время:", Location = new Point(20, 60) };
        _cmbTime = new ComboBox { Location = new Point(120, 58), Width = 230, DropDownStyle = ComboBoxStyle.DropDownList };
        // Генерируем слоты
        for (int i = 8; i < 18; i++)
        {
            _cmbTime.Items.Add($"{i:00}:00");
            _cmbTime.Items.Add($"{i:00}:30");
        }
        _cmbTime.SelectedIndex = 0;

        Label lbl3 = new Label { Text = "Услуга:", Location = new Point(20, 100) };
        _txtService = new TextBox { Location = new Point(120, 98), Width = 230 };

        _btnSave = new Button { Text = "Записать", Location = new Point(100, 150), Width = 130 };
        _btnSave.Click += BtnSave_Click;

        _btnCancel = new Button { Text = "Отмена", Location = new Point(240, 150), Width = 120 };
        _btnCancel.Click += (s, e) => this.Close();

        this.Controls.AddRange(new Control[] { lbl1, _cmbPatients, lbl2, _cmbTime, lbl3, _txtService, _btnSave, _btnCancel });
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (_cmbPatients.SelectedValue == null) 
        {
            MessageBox.Show("Выберите пациента.");
            return;
        }

        var timeParts = _cmbTime.Text.Split(':');
        var dt = _date.AddHours(int.Parse(timeParts[0])).AddMinutes(int.Parse(timeParts[1]));

        // Check if slot taken
        var existing = AppDI.Appointments.GetAll().FirstOrDefault(a => a.DoctorId == _doctorId && a.DateTime == dt);
        if (existing != null)
        {
            MessageBox.Show("Это время уже занято другим пациентом. Выберите другое время.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var appt = new Appointment
        {
            DoctorId = _doctorId,
            PatientId = (int)_cmbPatients.SelectedValue,
            DateTime = dt,
            Service = _txtService.Text
        };
        
        AppDI.Appointments.Add(appt);
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
