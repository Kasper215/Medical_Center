using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MedCenterApp.Models;

namespace MedCenterApp.UI;

public class FormPatients : Form
{
    private DataGridView _grid;
    private TextBox _txtSearch;
    private Button _btnAdd, _btnEdit, _btnDelete, _btnHistory;

    public FormPatients()
    {
        InitializeComponent();
        Theme.ApplyToForm(this);
        LoadData();
    }

    private void InitializeComponent()
    {
        this.Text = "Управление пациентами";
        this.Size = new Size(1000, 600);
        this.StartPosition = FormStartPosition.CenterParent;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(10) };
        
        var lblSearch = new Label { Text = "Поиск (ФИО или Тел):", Location = new Point(10, 25), AutoSize = true };
        _txtSearch = new TextBox { Location = new Point(170, 22), Width = 200 };
        _txtSearch.TextChanged += (s, e) => LoadData();
        
        _btnAdd = new Button { Text = "Добавить", Location = new Point(400, 17), Width = 120 };
        _btnAdd.Click += (s, e) => { new FormPatientEdit(null).ShowDialog(); LoadData(); };

        _btnEdit = new Button { Text = "Редактировать", Location = new Point(530, 17), Width = 150 };
        _btnEdit.Click += (s, e) => EditSelected();

        _btnDelete = new Button { Text = "Удалить", Location = new Point(690, 17), Width = 100 };
        _btnDelete.Click += (s, e) => DeleteSelected();

        _btnHistory = new Button { Text = "История посещений", Location = new Point(800, 17), Width = 180, BackColor = Theme.PrimaryDark };
        _btnHistory.Click += (s, e) => ShowHistory();

        panelTop.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnAdd, _btnEdit, _btnDelete, _btnHistory });
        
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            ReadOnly = true,
            AllowUserToAddRows = false
        };

        this.Controls.Add(_grid);
        this.Controls.Add(panelTop);
    }

    private void LoadData()
    {
        var patients = AppDI.Patients.GetAll();
        if (!string.IsNullOrWhiteSpace(_txtSearch.Text))
        {
            var search = _txtSearch.Text.ToLower();
            patients = patients.Where(p => 
                (p.LastName + " " + p.FirstName + " " + p.MiddleName).ToLower().Contains(search) || 
                p.Phone.Contains(search)).ToList();
        }
        _grid.DataSource = null;
        _grid.DataSource = patients;
    }

    private void EditSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is Patient p)
        {
            new FormPatientEdit(p).ShowDialog();
            LoadData();
        }
    }

    private void DeleteSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is Patient p)
        {
            bool hasAppointments = AppDI.Appointments.GetAll().Any(a => a.PatientId == p.Id);
            if (hasAppointments)
            {
                MessageBox.Show("Невозможно удалить пациента, так как у него есть история приёмов.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить пациента {p.FullName}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AppDI.Patients.Delete(p.Id);
                LoadData();
            }
        }
    }

    private void ShowHistory()
    {
        if (_grid.CurrentRow?.DataBoundItem is Patient p)
        {
            var history = AppDI.Appointments.GetAll()
                .Where(a => a.PatientId == p.Id)
                .OrderByDescending(a => a.DateTime)
                .Select(a => $"{a.DateTime:dd.MM.yyyy HH:mm} - Врач: {AppDI.Doctors.GetById(a.DoctorId)?.FullName} - Услуга: {a.Service}")
                .ToList();
            
            string text = history.Any() ? string.Join("\n", history) : "Нет записей о посещениях.";
            MessageBox.Show(text, $"История: {p.FullName}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

public class FormPatientEdit : Form
{
    private Patient _patient;
    private TextBox _txtLastName, _txtFirstName, _txtMiddleName, _txtPhone, _txtAddress, _txtPolicy;
    private DateTimePicker _dtpBirth;
    private Button _btnSave, _btnCancel;

    public FormPatientEdit(Patient? patient)
    {
        _patient = patient ?? new Patient();
        InitializeComponent();
        Theme.ApplyToForm(this);
        Theme.SetPhoneFormat(_txtPhone);
        Theme.SetOnlyDigits(_txtPolicy);
    }

    private void InitializeComponent()
    {
        this.Text = _patient.Id == 0 ? "Новый пациент" : "Редактирование пациента";
        this.Size = new Size(420, 420);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        Label lbl1 = new Label { Text = "Фамилия *", Location = new Point(20, 20), Width = 120 };
        _txtLastName = new TextBox { Location = new Point(150, 18), Width = 220, Text = _patient.LastName };

        Label lbl2 = new Label { Text = "Имя *", Location = new Point(20, 60), Width = 120 };
        _txtFirstName = new TextBox { Location = new Point(150, 58), Width = 220, Text = _patient.FirstName };

        Label lbl8 = new Label { Text = "Отчество", Location = new Point(20, 100), Width = 120 };
        _txtMiddleName = new TextBox { Location = new Point(150, 98), Width = 220, Text = _patient.MiddleName };

        Label lbl3 = new Label { Text = "Дата рождения", Location = new Point(20, 140), Width = 120 };
        _dtpBirth = new DateTimePicker { Location = new Point(150, 138), Width = 220, Format = DateTimePickerFormat.Short, Value = _patient.Id == 0 ? DateTime.Today.AddYears(-20) : _patient.BirthDate };

        Label lbl4 = new Label { Text = "Телефон *", Location = new Point(20, 180), Width = 120 };
        _txtPhone = new TextBox { Location = new Point(150, 178), Width = 220, Text = _patient.Phone };

        Label lbl5 = new Label { Text = "Адрес", Location = new Point(20, 220), Width = 120 };
        _txtAddress = new TextBox { Location = new Point(150, 218), Width = 220, Text = _patient.Address };

        Label lbl6 = new Label { Text = "Номер полиса", Location = new Point(20, 260), Width = 120 };
        _txtPolicy = new TextBox { Location = new Point(150, 258), Width = 220, Text = _patient.PolicyNumber };

        _btnSave = new Button { Text = "Сохранить", Location = new Point(130, 310), Width = 120 };
        _btnSave.Click += BtnSave_Click;

        _btnCancel = new Button { Text = "Отмена", Location = new Point(260, 310), Width = 120 };
        _btnCancel.Click += (s, e) => this.Close();

        this.Controls.AddRange(new Control[] { lbl1, _txtLastName, lbl2, _txtFirstName, lbl8, _txtMiddleName, lbl3, _dtpBirth, lbl4, _txtPhone, lbl5, _txtAddress, lbl6, _txtPolicy, _btnSave, _btnCancel });
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        _patient.LastName = _txtLastName.Text;
        _patient.FirstName = _txtFirstName.Text;
        _patient.MiddleName = _txtMiddleName.Text;
        _patient.BirthDate = _dtpBirth.Value;
        _patient.Phone = _txtPhone.Text;
        _patient.Address = _txtAddress.Text;
        _patient.PolicyNumber = _txtPolicy.Text;

        if (!AppDI.Validator.ValidatePatient(_patient, out string error))
        {
            MessageBox.Show(error, "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_patient.Id == 0)
            AppDI.Patients.Add(_patient);
        else
            AppDI.Patients.Update(_patient);

        this.Close();
    }
}
