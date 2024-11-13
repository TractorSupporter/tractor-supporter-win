using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        private LanguageService _languageService;

        public StarterWindowViewModel()
        {
            //_windowService = new WindowService();
            ForwardCommand = new RelayCommand(Validate);
            BackCommand = new RelayCommand(Close);
            _configAppJson = ConfigAppJson.Instance;
            _navigationService = NavigationService.Instance;
            _languageService = LanguageService.Instance;
            NavigateToMainPageIfConfigExists();
            SetMyIP();
            Port = "8080";
        }

        private void SetMyIP()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostEntry(hostName)
                .AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            IpAddress = myIP;
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

        private void NavigateToMainPageIfConfigExists()
        {
            var configAppJson = ConfigAppJson.Instance;
            AppConfig appConfig = configAppJson.ReadJson();

            if (appConfig != null)
            {
                _languageService.ChangeLanguage(appConfig.Language);
                _navigationService.NavigateToMain();
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
                _configAppJson.CreateJson(Port,
                    IpAddress,
                    true,
                    true,
                    TypeSensor.Ultrasonic,
                    int.Parse(ConfigurationManager.AppSettings["AvoidingDistance"]),
                    int.Parse(ConfigurationManager.AppSettings["AlarmDistance"]),
                    Language.English);
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
