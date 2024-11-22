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

namespace TractorSupporter.ViewModel
{
    public class TSContainerWindowViewModel: BaseViewModel
    {
        private INavigationService _navigationService;
        private LanguageService _languageService;
        private SettingsVisibilityService _settingsVisibilityService;

        public TSContainerWindowViewModel()
        {  
            _navigationService = NavigationService.Instance;
            _languageService = LanguageService.Instance;
            _settingsVisibilityService = SettingsVisibilityService.Instance;
            _settingsVisibilityService.PropertyChanged += OnSettingsVisibilityServicePropertyChanged;
            NavigateToMainPageIfConfigExists();
            IsSettingsVisible = false;
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
