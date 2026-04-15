using System;
using System.Windows.Forms;
using MedCenterApp.UI;

namespace MedCenterApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new FormLogin());
    }
}