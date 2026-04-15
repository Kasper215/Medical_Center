using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedCenterApp.UI;

public class FormMain : Form
{
    private MenuStrip _menuStrip;
    private Label _lblStatus;
    private Label _lblWelcome;

    public FormMain()
    {
        InitializeComponent();
        Theme.ApplyToForm(this);
        ApplyMenuTheme();
        UpdateStatus();
    }

    private void InitializeComponent()
    {
        this.Text = "Медицинский Центр - Дашборд";
        this.Size = new Size(900, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        _menuStrip = new MenuStrip();
        _menuStrip.Font = Theme.HeaderFont;
        _menuStrip.Padding = new Padding(10);
        
        var patientsMenu = new ToolStripMenuItem("Пациенты");
        patientsMenu.Click += (s, e) => { new FormPatients().ShowDialog(); UpdateStatus(); };
        
        var doctorsMenu = new ToolStripMenuItem("Врачи");
        doctorsMenu.Click += (s, e) => { new FormDoctors().ShowDialog(); UpdateStatus(); };

        var scheduleMenu = new ToolStripMenuItem("Расписание приёмов");
        scheduleMenu.Click += (s, e) => { new FormAppointments().ShowDialog(); UpdateStatus(); };
        
        var reportsMenu = new ToolStripMenuItem("Отчёты");
        reportsMenu.Click += (s, e) => { new FormReports().ShowDialog(); UpdateStatus(); };

        var exitMenu = new ToolStripMenuItem("Выход");
        exitMenu.Click += (s, e) => this.Close();

        _menuStrip.Items.Add(patientsMenu);
        _menuStrip.Items.Add(doctorsMenu);
        _menuStrip.Items.Add(scheduleMenu);
        _menuStrip.Items.Add(reportsMenu);
        _menuStrip.Items.Add(exitMenu);

        _lblWelcome = new Label 
        { 
            Text = "Добро пожаловать!",
            Location = new Point(30, 80), 
            Size = new Size(800, 40), 
            Font = Theme.TitleFont,
            ForeColor = Theme.PrimaryDark
        };

        _lblStatus = new Label 
        { 
            Location = new Point(30, 130), 
            Size = new Size(800, 300), 
            Font = Theme.HeaderFont 
        };

        this.Controls.Add(_lblWelcome);
        this.Controls.Add(_lblStatus);
        this.Controls.Add(_menuStrip);
        this.MainMenuStrip = _menuStrip;
    }

    private void ApplyMenuTheme()
    {
        _menuStrip.BackColor = Theme.PrimaryDark;
        _menuStrip.ForeColor = Color.White;
        foreach (ToolStripItem item in _menuStrip.Items)
        {
            item.ForeColor = Color.White;
        }
    }

    private void UpdateStatus()
    {
        var user = AppDI.AuthService.CurrentUser;
        var patientsCount = AppDI.Patients.GetAll().Count;
        var doctorsCount = AppDI.Doctors.GetAll().Count;
        var apptsTodayCount = AppDI.Appointments.GetAll().FindAll(a => a.DateTime.Date == DateTime.Today).Count;
        
        string roleStr = user?.Role == "Admin" ? "Администратор" : "Врач";
        
        _lblStatus.Text = $"Текущий пользователь: {user?.Login} ({roleStr})\n\n" +
                          $"Статистика:\n" +
                          $"• Зарегистрировано пациентов: {patientsCount}\n" +
                          $"• Врачей в штате: {doctorsCount}\n" +
                          $"• Записей на прием сегодня: {apptsTodayCount}";
    }
}
