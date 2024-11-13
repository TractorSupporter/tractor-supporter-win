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
        public static string[] CommandLineArgs { get; private set; }
        public static bool IsInitialized { get; set; } = true;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 0)
            {
                CommandLineArgs = e.Args;
            }
        }
    }

}
