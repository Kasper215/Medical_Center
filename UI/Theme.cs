using System.Drawing;
using System.Windows.Forms;

namespace MedCenterApp.UI;

public static class Theme
{
    public static readonly Color Primary = Color.FromArgb(0, 120, 215); // Blue
    public static readonly Color PrimaryDark = Color.FromArgb(0, 90, 160);
    public static readonly Color Background = Color.FromArgb(245, 246, 250);
    public static readonly Color Surface = Color.White;
    public static readonly Color Text = Color.FromArgb(40, 40, 40);
    public static readonly Color TextSecondary = Color.FromArgb(100, 100, 100);
    public static readonly Color Danger = Color.FromArgb(220, 53, 69);
    
    public static readonly Font MainFont = new Font("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
    public static readonly Font HeaderFont = new Font("Segoe UI", 12F, FontStyle.Bold);

    public static void ApplyToForm(Form form)
    {
        form.BackColor = Background;
        form.ForeColor = Text;
        form.Font = MainFont;
        form.ShowIcon = false;
        
        foreach (Control control in form.Controls)
        {
            ApplyToControl(control);
        }
    }

    private static void ApplyToControl(Control control)
    {
        control.Font = MainFont;
        
        if (control is Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.Cursor = Cursors.Hand;
            
            if (btn.Text == "Удалить" || btn.Text == "Отменить" || btn.Text == "Отмена")
            {
                btn.BackColor = Danger;
            }
        }
        else if (control is TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Surface;
        }
        else if (control is DataGridView grid)
        {
            grid.BackgroundColor = Surface;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = HeaderFont;
            grid.RowHeadersVisible = false;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 255);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.GridColor = Color.FromArgb(230, 230, 230);
            grid.AllowUserToResizeRows = false;
        }
        else if (control is Panel pnl)
        {
            foreach (Control child in pnl.Controls)
                ApplyToControl(child);
        }
    }
}
