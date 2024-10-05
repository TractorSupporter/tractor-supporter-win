using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.ViewModel
{
    class SettingsPageViewModel : BaseViewModel
    {
        private string _port;
        private string _ipAddress;
        private string _portValidationMessage;
        private string _ipValidationMessage;
        private bool _option1IsChecked;
        private bool _option2IsChecked;
        private ICommand _forwardCommand;
        private ICommand _backCommand;
        private NavigationService _navigationService;
        private IConfigAppJson _configAppJson;

        public SettingsPageViewModel()
        {
            _navigationService = NavigationService.Instance;
            _configAppJson = ConfigAppJson.Instance;
            ForwardCommand = new RelayCommand(SaveSettings);
            BackCommand = new RelayCommand(CloseSettings);
            setConfigData();
        }

        public bool Option1IsChecked
        {
            get => _option1IsChecked;
            set
            {
                if (_option1IsChecked != value)
                {
                    _option1IsChecked = value;
                    OnPropertyChanged(nameof(_option1IsChecked));
                }
            }
        }

        public bool Option2IsChecked
        {
            get => _option2IsChecked;
            set
            {
                if (_option2IsChecked != value)
                {
                    _option2IsChecked = value;
                    OnPropertyChanged(nameof(_option2IsChecked));
                }
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged(nameof(IpAddress));
            }
        }

        public string PortValidationMessage
        {
            get => _portValidationMessage;
            set
            {
                _portValidationMessage = value;
                OnPropertyChanged(nameof(PortValidationMessage));
            }
        }

        public string IpValidationMessage
        {
            get => _ipValidationMessage;
            set
            {
                _ipValidationMessage = value;
                OnPropertyChanged(nameof(IpValidationMessage));
            }
        }

        public ICommand ForwardCommand
        {
            get => _forwardCommand;
            set
            {
                _forwardCommand = value;
                OnPropertyChanged(nameof(ForwardCommand));
            }
        }

        public ICommand BackCommand
        {
            get => _backCommand;
            set
            {
                _backCommand = value;
                OnPropertyChanged(nameof(BackCommand));
            }
        }

        public void setConfigData()
        {
            AppConfig appConfig = _configAppJson.ReadJson();
            Port = appConfig.Port.ToString();
            IpAddress = appConfig.IpAddress;
            Option1IsChecked = appConfig.Option1;
            Option2IsChecked = appConfig.Option2;
        }

        private void SaveSettings(object parameter)
        {
            bool isValid = true;

            if (int.TryParse(Port, out int portNumber) && portNumber > 0 && portNumber <= 65535)
            {
                PortValidationMessage = string.Empty;
            }
            else
            {
                PortValidationMessage = "Port is invalid. It must be a number between 1 and 65535.";
                isValid = false;
            }

            if (IPAddress.TryParse(IpAddress, out _))
            {
                IpValidationMessage = string.Empty;
            }
            else
            {
                IpValidationMessage = "IP address is invalid.";
                isValid = false;
            }

            if (isValid)
            {
                _configAppJson.CreateJson(Port, IpAddress, Option1IsChecked, Option2IsChecked);
                _navigationService.NavigateToMain();
            }
        }

        private void CloseSettings(object parameter)
        {
            _navigationService.NavigateToMain();
        }
    }
}
