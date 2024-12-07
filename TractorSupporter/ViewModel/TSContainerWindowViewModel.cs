using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.Services;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.ViewModel
{
    public class TSContainerWindowViewModel: BaseViewModel
    {
        private INavigationService _navigationService;
        private LanguageService _languageService;
        private SettingsVisibilityService _settingsVisibilityService;
        private HistoryVisibilityService _historyVisibilityService;
        private MockDataReceiver _mockDataReceiver;
        private IReceivedDataFormatter _receivedDataFormatter;

        public TSContainerWindowViewModel(IReceivedDataFormatter receivedDataFormatter)
        {  
            _navigationService = NavigationService.Instance;
            _languageService = LanguageService.Instance;
            _settingsVisibilityService = SettingsVisibilityService.Instance;
            _settingsVisibilityService.PropertyChanged += OnSettingsVisibilityServicePropertyChanged;
            _historyVisibilityService = HistoryVisibilityService.Instance;
            _historyVisibilityService.PropertyChanged += OnHistoryVisibilityServicePropertyChanged;
            _mockDataReceiver = MockDataReceiver.Instance;
            _receivedDataFormatter = receivedDataFormatter;
            NavigateToMainPageIfConfigExists();
            IsSettingsVisible = false;
            IsHistoryVisible = true;
        }

        private void OnHistoryVisibilityServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(_historyVisibilityService.IsHistoryVisible))
            {
                OnPropertyChanged(nameof(IsHistoryVisible));
            }
        }

        public bool IsHistoryVisible
        {
            get => _historyVisibilityService.IsHistoryVisible;
            set => _historyVisibilityService.IsHistoryVisible = value;
        }

        public bool IsSettingsVisible
        {
            get => _settingsVisibilityService.IsSettingsVisible;
            set => _settingsVisibilityService.IsSettingsVisible = value;
        }

        private void NavigateToMainPageIfConfigExists()
        {
            var configAppJson = ConfigAppJson.Instance;
            AppConfig appConfig = configAppJson.ReadJson();

            if (appConfig != null)
            {
                _languageService.ChangeLanguage(appConfig.Language);
                _mockDataReceiver.ChangeInnerMock(appConfig.SelectedSensorType == TypeSensor.Laser);
                _receivedDataFormatter.ChangeMode(appConfig.SelectedSensorType == TypeSensor.Laser);
                _navigationService.NavigateToMain();
            } else
            {
                _navigationService.NavigateToStarter();
            }
        }

        private void OnSettingsVisibilityServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(_settingsVisibilityService.IsSettingsVisible))
            {
                OnPropertyChanged(nameof(IsSettingsVisible));
            }
        }
    }
}
