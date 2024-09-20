using System.Windows;
using System.Windows.Controls;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.View;

namespace TractorSupporter.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _mainFrame;
        private static NavigationService _instance;
        private static readonly object _lock = new object();

        public static void Initialize(Frame mainFrame)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new NavigationService(mainFrame);
                }
                else
                {
                    throw new InvalidOperationException("NavigationService is already initialized.");
                }
            }
        }

        public static NavigationService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        throw new InvalidOperationException("NavigationService is not initialized. Call Initialize(Frame) first.");
                    }
                    return _instance;
                }
            }
        }


        private NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
        }

        public void NavigateToSettings()
        {
            _mainFrame.Navigate(new SettingsPage());
        }

        public void NavigateToMain()
        {
            _mainFrame.Navigate(new MainPage());
        }
    }
}
