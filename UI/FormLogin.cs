using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedCenterApp.UI;

public class FormLogin : Form
{
    private TextBox txtLogin;
    private TextBox txtPassword;
    private Button btnLogin;
    private Button btnExit;
    private Label lblLogin;
    private Label lblPassword;
    private Label lblTitle;

    public FormLogin()
    {
        InitializeComponent();
        Theme.ApplyToForm(this);
        // Correcting background for the card layout
        this.BackColor = Color.FromArgb(235, 237, 242);
    }

    private void InitializeComponent()
    {
        this.Text = "Авторизация";
        this.Size = new Size(450, 480);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(235, 237, 242);

        var card = new Panel
        {
            Size = new Size(350, 360),
            Location = new Point(45, 40),
            BackColor = Color.White,
            Padding = new Padding(20)
        };

        lblTitle = new Label 
        { 
            Text = "Вход в систему", 
            Font = new Font("Segoe UI", 16F, FontStyle.Bold), 
            Dock = DockStyle.Top, 
            Height = 50, 
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Theme.Primary
        };

        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 20, 10, 10) };

        lblLogin = new Label { Text = "Логин", Font = new Font("Segoe UI", 10F), Dock = DockStyle.Top, Height = 25, ForeColor = Theme.TextSecondary };
        txtLogin = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 12F), TextAlign = HorizontalAlignment.Center };
        
        var spacer1 = new Label { Dock = DockStyle.Top, Height = 25 };

        lblPassword = new Label { Text = "Пароль", Font = new Font("Segoe UI", 10F), Dock = DockStyle.Top, Height = 25, ForeColor = Theme.TextSecondary };
        txtPassword = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 12F), TextAlign = HorizontalAlignment.Center, PasswordChar = '*' };

        var spacer2 = new Label { Dock = DockStyle.Top, Height = 40 };

        btnLogin = new Button { Text = "ВОЙТИ", Dock = DockStyle.Top, Height = 45, BackColor = Theme.Primary, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        btnLogin.Click += BtnLogin_Click;

        var spacer3 = new Label { Dock = DockStyle.Top, Height = 10 };

        btnExit = new Button { Text = "ВЫХОД", Dock = DockStyle.Top, Height = 40, FlatStyle = FlatStyle.Flat, ForeColor = Theme.TextSecondary, Font = new Font("Segoe UI", 9F) };
        btnExit.FlatAppearance.BorderSize = 0;
        btnExit.Click += (s, e) => Application.Exit();

        content.Controls.Add(btnExit);
        content.Controls.Add(spacer3);
        content.Controls.Add(btnLogin);
        content.Controls.Add(spacer2);
        content.Controls.Add(txtPassword);
        content.Controls.Add(lblPassword);
        content.Controls.Add(spacer1);
        content.Controls.Add(txtLogin);
        content.Controls.Add(lblLogin);

        card.Controls.Add(content);
        card.Controls.Add(lblTitle);
        
        this.Controls.Add(card);
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        var user = AppDI.AuthService.Login(txtLogin.Text, txtPassword.Text);
        if (user != null)
        {
            var mainForm = new FormMain();
            this.Hide();
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();
        }
        else
        {
            MessageBox.Show("Неверный логин или пароль", "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
