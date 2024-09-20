using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.ViewModel
{
    public class StarterWindowViewModel : BaseViewModel
    {
        private string _port;
        private string _ipAddress;
        private string _portValidationMessage;
        private string _ipValidationMessage;
        private ICommand _forwardCommand;
        private ICommand _backCommand;
        private INavigationService _navigationService;
        private IConfigAppJson _configAppJson;

        public StarterWindowViewModel()
        {
            //_windowService = new WindowService();
            ForwardCommand = new RelayCommand(Validate);
            BackCommand = new RelayCommand(Close);
            _configAppJson = ConfigAppJson.Instance;
            _navigationService = NavigationService.Instance;
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

        private void Validate(object parameter)
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
                _configAppJson.createJson(Port, IpAddress);
                _navigationService.NavigateToMain();
                //_windowService.OpenMainWindow();
                //Close(new object());
            }
        }
        public void CloseMainWindow()
        {
            _navigationService.NavigateToSettings();
        }
    }
}
