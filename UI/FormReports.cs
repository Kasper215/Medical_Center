using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MedCenterApp.Models;

namespace MedCenterApp.UI;

public class FormReports : Form
{
    private ComboBox _cmbReportType;
    private DateTimePicker _dtpFrom, _dtpTo;
    private Button _btnGenerate, _btnExport, _btnPrint;
    private DataGridView _grid;
    private string _currentReportHtml = "";

    public FormReports()
    {
        InitializeComponent();
        Theme.ApplyToForm(this);
    }

    private void InitializeComponent()
    {
        this.Text = "Отчёты";
        this.Size = new Size(1100, 600);
        this.StartPosition = FormStartPosition.CenterParent;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(10) };

        _cmbReportType = new ComboBox { Location = new Point(10, 20), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbReportType.Items.AddRange(new string[] { "Отчёт по посещениям за период", "Отчёт по врачам (загруженность)" });
        _cmbReportType.SelectedIndex = 0;

        Label lbl1 = new Label { Text = "С:", Location = new Point(320, 23), Width = 20, AutoSize = true };
        _dtpFrom = new DateTimePicker { Location = new Point(350, 20), Width = 110, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-7) };

        Label lbl2 = new Label { Text = "По:", Location = new Point(470, 23), Width = 30, AutoSize = true };
        _dtpTo = new DateTimePicker { Location = new Point(510, 20), Width = 110, Format = DateTimePickerFormat.Short };

        _btnGenerate = new Button { Text = "Сформировать", Location = new Point(630, 17), Width = 160 };
        _btnGenerate.Click += BtnGenerate_Click;

        _btnExport = new Button { Text = "Экспорт в HTML", Location = new Point(800, 17), Width = 160, Enabled = false };
        _btnExport.Click += BtnExport_Click;

        _btnPrint = new Button { Text = "Печать", Location = new Point(970, 17), Width = 110, Enabled = false };
        _btnPrint.Click += BtnPrint_Click;

        panelTop.Controls.AddRange(new Control[] { _cmbReportType, lbl1, _dtpFrom, lbl2, _dtpTo, _btnGenerate, _btnExport, _btnPrint });

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

    private void BtnGenerate_Click(object? sender, EventArgs e)
    {
        var fromDate = _dtpFrom.Value.Date;
        var toDate = _dtpTo.Value.Date.AddDays(1).AddSeconds(-1);
        
        var appointments = AppDI.Appointments.GetAll()
            .Where(a => a.DateTime >= fromDate && a.DateTime <= toDate)
            .ToList();

        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.AppendLine("<html><head><meta charset='utf-8'><style>table { width:100%; border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; text-align: left; } th { background-color: #f2f2f2; }</style></head><body>");
        htmlBuilder.AppendLine($"<h2>{_cmbReportType.Text}</h2>");
        htmlBuilder.AppendLine($"<p><b>Период:</b> с {fromDate:dd.MM.yyyy} по {toDate:dd.MM.yyyy}</p>");
        htmlBuilder.AppendLine($"<p><b>Дата формирования:</b> {DateTime.Now:dd.MM.yyyy HH:mm}</p>");

        if (_cmbReportType.SelectedIndex == 0)
        {
            var data = appointments.Select(a => new
            {
                Дата = a.DateTime.ToString("dd.MM.yyyy HH:mm"),
                Врач = AppDI.Doctors.GetById(a.DoctorId)?.FullName,
                Пациент = AppDI.Patients.GetById(a.PatientId)?.FullName,
                Услуга = a.Service
            }).ToList();

            _grid.DataSource = data;

            // HTML Report
            htmlBuilder.AppendLine("<table><tr><th>Дата</th><th>Врач</th><th>Пациент</th><th>Услуга</th></tr>");
            foreach(var item in data)
                htmlBuilder.AppendLine($"<tr><td>{item.Дата}</td><td>{item.Врач}</td><td>{item.Пациент}</td><td>{item.Услуга}</td></tr>");
            htmlBuilder.AppendLine("</table>");
        }
        else if (_cmbReportType.SelectedIndex == 1)
        {
            var data = appointments
                .GroupBy(a => a.DoctorId)
                .Select(g => new
                {
                    Врач = AppDI.Doctors.GetById(g.Key)?.FullName,
                    Количество_приемов = g.Count()
                }).ToList();
                
            _grid.DataSource = data;

            // HTML Report
            htmlBuilder.AppendLine("<table><tr><th>Врач</th><th>Количество приемов</th></tr>");
            foreach(var item in data)
                htmlBuilder.AppendLine($"<tr><td>{item.Врач}</td><td>{item.Количество_приемов}</td></tr>");
            htmlBuilder.AppendLine("</table>");
        }

        htmlBuilder.AppendLine("</body></html>");
        _currentReportHtml = htmlBuilder.ToString();

        _btnExport.Enabled = true;
        _btnPrint.Enabled = true;

        if (_grid.Columns.Count > 0)
        {
            Theme.ApplyToForm(this); // reapplying just to grid if columns rebuilt
        }
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "HTML Document|*.html", FileName = "Report.html" })
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, _currentReportHtml, Encoding.UTF8);
                MessageBox.Show("Отчет успешно сохранен!", "Экспорт", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void BtnPrint_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("В данной версии системы функция печати отправляет документ на виртуальный принтер. Сохраните отчет в HTML для физической печати.", "Печать", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
