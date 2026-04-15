using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MedCenterApp.Models;

namespace MedCenterApp.UI;

public class FormDoctors : Form
{
    private DataGridView _grid;
    private TextBox _txtSearch;
    private Button _btnAdd, _btnEdit, _btnDelete;

    public FormDoctors()
    {
        InitializeComponent();
        Theme.ApplyToForm(this);
        LoadData();
    }

    private void InitializeComponent()
    {
        this.Text = "Управление врачами";
        this.Size = new Size(1000, 600);
        this.StartPosition = FormStartPosition.CenterParent;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(10) };
        
        var lblSearch = new Label { Text = "Поиск (ФИО / Спец):", Location = new Point(10, 25), AutoSize = true };
        _txtSearch = new TextBox { Location = new Point(150, 22), Width = 200 };
        _txtSearch.TextChanged += (s, e) => LoadData();
        
        _btnAdd = new Button { Text = "Добавить", Location = new Point(370, 17), Width = 120 };
        _btnAdd.Click += (s, e) => { new FormDoctorEdit(null).ShowDialog(); LoadData(); };

        _btnEdit = new Button { Text = "Редактировать", Location = new Point(500, 17), Width = 150 };
        _btnEdit.Click += (s, e) => EditSelected();

        _btnDelete = new Button { Text = "Удалить", Location = new Point(660, 17), Width = 100 };
        _btnDelete.Click += (s, e) => DeleteSelected();

        panelTop.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnAdd, _btnEdit, _btnDelete });
        
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
        var doctors = AppDI.Doctors.GetAll();
        if (!string.IsNullOrWhiteSpace(_txtSearch.Text))
        {
            var search = _txtSearch.Text.ToLower();
            doctors = doctors.Where(p => 
                (p.LastName + " " + p.FirstName + " " + p.MiddleName).ToLower().Contains(search) || 
                p.Specialty.ToLower().Contains(search)).ToList();
        }
        _grid.DataSource = null;
        _grid.DataSource = doctors;
    }

    private void EditSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is Doctor p)
        {
            new FormDoctorEdit(p).ShowDialog();
            LoadData();
        }
    }

    private void DeleteSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is Doctor p)
        {
            bool hasFutureAppointments = AppDI.Appointments.GetAll().Any(a => a.DoctorId == p.Id && a.DateTime >= DateTime.Now);
            if (hasFutureAppointments)
            {
                MessageBox.Show("Невозможно удалить врача. У него есть запланированные приемы в будущем!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить врача {p.FullName}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                AppDI.Doctors.Delete(p.Id);
                LoadData();
            }
        }
    }
}

public class FormDoctorEdit : Form
{
    private Doctor _doctor;
    private TextBox _txtLastName, _txtFirstName, _txtMiddleName, _txtSpecialty, _txtPhone;
    private Button _btnSave, _btnCancel;

    public FormDoctorEdit(Doctor? doctor)
    {
        _doctor = doctor ?? new Doctor();
        InitializeComponent();
        Theme.ApplyToForm(this);
        Theme.SetPhoneFormat(_txtPhone);
    }

    private void InitializeComponent()
    {
        this.Text = _doctor.Id == 0 ? "Новый врач" : "Редактирование врача";
        this.Size = new Size(400, 320);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;

        Label lbl1 = new Label { Text = "Фамилия *", Location = new Point(20, 20), Width = 120 };
        _txtLastName = new TextBox { Location = new Point(150, 18), Width = 200, Text = _doctor.LastName };

        Label lbl2 = new Label { Text = "Имя *", Location = new Point(20, 60), Width = 120 };
        _txtFirstName = new TextBox { Location = new Point(150, 58), Width = 200, Text = _doctor.FirstName };

        Label lbl8 = new Label { Text = "Отчество", Location = new Point(20, 100), Width = 120 };
        _txtMiddleName = new TextBox { Location = new Point(150, 98), Width = 200, Text = _doctor.MiddleName };

        Label lbl3 = new Label { Text = "Специальность *", Location = new Point(20, 140), Width = 120 };
        _txtSpecialty = new TextBox { Location = new Point(150, 138), Width = 200, Text = _doctor.Specialty };

        Label lbl4 = new Label { Text = "Телефон", Location = new Point(20, 180), Width = 120 };
        _txtPhone = new TextBox { Location = new Point(150, 178), Width = 200, Text = _doctor.Phone };

        _btnSave = new Button { Text = "Сохранить", Location = new Point(110, 230), Width = 120 };
        _btnSave.Click += BtnSave_Click;

        _btnCancel = new Button { Text = "Отмена", Location = new Point(240, 230), Width = 120 };
        _btnCancel.Click += (s, e) => this.Close();

        this.Controls.AddRange(new Control[] { lbl1, _txtLastName, lbl2, _txtFirstName, lbl8, _txtMiddleName, lbl3, _txtSpecialty, lbl4, _txtPhone, _btnSave, _btnCancel });
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        _doctor.LastName = _txtLastName.Text;
        _doctor.FirstName = _txtFirstName.Text;
        _doctor.MiddleName = _txtMiddleName.Text;
        _doctor.Specialty = _txtSpecialty.Text;
        _doctor.Phone = _txtPhone.Text;

        if (!AppDI.Validator.ValidateDoctor(_doctor, out string error))
        {
            MessageBox.Show(error, "Ошибка валидации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_doctor.Id == 0)
            AppDI.Doctors.Add(_doctor);
        else
            AppDI.Doctors.Update(_doctor);

        this.Close();
    }
}
