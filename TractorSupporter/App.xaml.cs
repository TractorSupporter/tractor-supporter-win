using System.Configuration;
using System.Data;
using System.Windows;
using TractorSupporter.Model;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var configAppJson = ConfigAppJson.Instance;
            AppConfig appConfig = configAppJson.readJson();

            if (appConfig != null)
            {
                StartupUri = new Uri("View/MainWindow.xaml", UriKind.Relative);
            }
            else
            {
                StartupUri = new Uri("View/StarterConfigWindow.xaml", UriKind.Relative);
            }
        }
    }

}
