using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using TractorSupporter.Model;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.ViewModel;

namespace TractorSupporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] CommandLineArgs { get; private set; }
        public static bool IsInitialized { get; set; } = true;

        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            ServiceCollection servicecollection = new();
            servicecollection.ConfigureServices();

            ServiceProvider = servicecollection.BuildServiceProvider();
        }

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

public static class ServiceCollectionExtentions
{
    public static void ConfigureServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddSingleton<IAvoidingService, AvoidingService>();
        services.AddSingleton<IAlarmService, AlarmService>();
        services.AddSingleton<MainPageViewModel>();
        services.AddSingleton<IGPSConnectionService, GPSConnectionService>();
        services.AddSingleton<IReceivedDataFormatter,  ReceivedDataFormatter>();
        services.AddSingleton<IMockDataConfigWindowViewModel, MockDataConfigWindowViewModel>();
        services.AddSingleton<IMockDataReceiver, MockDataReceiver>();
        services.AddSingleton<ILidarDistanceService, LidarDistanceService>();
        services.AddSingleton<IDataReceiverGPS, DataReceiverGPS>();
        services.AddSingleton<IDataSenderGPS, DataSenderGPS>();
    }
}
