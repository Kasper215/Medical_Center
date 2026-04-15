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
    }

    private void InitializeComponent()
    {
        this.Text = "Авторизация";
        this.Size = new Size(400, 320);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        lblTitle = new Label { Text = "Медицинский Центр", Font = Theme.TitleFont, Location = new Point(0, 30), Size = new Size(400, 40), TextAlign = ContentAlignment.MiddleCenter };
        
        lblLogin = new Label { Text = "Логин", Location = new Point(70, 90), Size = new Size(260, 25) };
        txtLogin = new TextBox { Location = new Point(70, 115), Size = new Size(240, 30), Font = Theme.HeaderFont };
        
        lblPassword = new Label { Text = "Пароль", Location = new Point(70, 155), Size = new Size(260, 25) };
        txtPassword = new TextBox { Location = new Point(70, 180), Size = new Size(240, 30), Font = Theme.HeaderFont, PasswordChar = '*' };

        btnLogin = new Button { Text = "Вход", Location = new Point(70, 230), Size = new Size(110, 35) };
        btnLogin.Click += BtnLogin_Click;

        btnExit = new Button { Text = "Выход", Location = new Point(200, 230), Size = new Size(110, 35) };
        btnExit.Click += (s, e) => Application.Exit();

        this.Controls.Add(lblTitle);
        this.Controls.Add(lblLogin);
        this.Controls.Add(txtLogin);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(btnExit);
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
