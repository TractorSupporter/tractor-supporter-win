using System.Windows;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.View;

namespace TractorSupporter.Services
{
    public class WindowService : IWindowService
    {
        public void OpenMainWindow()
        {
            MainWindow window = new MainWindow();
            window.Show();
        }

        public void OpenSettingsWindow()
        {
            SettingsWindow window = new SettingsWindow();
            window.Show();
        }
    }
}
