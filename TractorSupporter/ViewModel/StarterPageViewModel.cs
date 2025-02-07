﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.Services.Interfaces;
using TractorSupporter.Services;
using System.Configuration;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.ViewModel
{
    public class StarterPageViewModel: BaseViewModel
    {
        private string _port;
        private string _portValidationMessage;
        private string _ipValidationMessage;
        private ICommand _forwardCommand;
        private ICommand _backCommand;
        private INavigationService _navigationService;
        private IConfigAppJson _configAppJson;
        private LanguageService _languageService;
        private SettingsVisibilityService _settingsVisibilityService;
        private HistoryVisibilityService _historyVisibilityService;

        public StarterPageViewModel()
        {
            //_windowService = new WindowService();
            ForwardCommand = new RelayCommand(Validate);
            BackCommand = new RelayCommand(Close);
            _configAppJson = ConfigAppJson.Instance;
            _navigationService = NavigationService.Instance;
            _languageService = LanguageService.Instance;
            _historyVisibilityService = HistoryVisibilityService.Instance;
            _settingsVisibilityService = SettingsVisibilityService.Instance;
            SetMyIP();
            Port = "8080";
        }

        private void SetMyIP()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostEntry(hostName)
                .AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        public bool IsSettingsVisible
        {
            get => _settingsVisibilityService.IsSettingsVisible;
            set => _settingsVisibilityService.IsSettingsVisible = value;
        }

        public bool IsHistoryVisible
        {
            get => _historyVisibilityService.IsHistoryVisible;
            set => _historyVisibilityService.IsHistoryVisible = value;
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

            if (isValid)
            {
                _configAppJson.CreateJson(Port,
                    true,
                    true,
                    TypeSensor.Laser,
                    int.Parse(ConfigurationManager.AppSettings["AvoidingDistance"]),
                    int.Parse(ConfigurationManager.AppSettings["AlarmDistance"]),
                    Language.English, TypeTurn.Auto);
                _navigationService.NavigateToMain();
                //_windowService.OpenMainWindow();
                //Close(new object());
            }
        }
        public void SettingsCloseMainWindow()
        {
            _navigationService.NavigateToSettings();
        }

        public void HistoryCloseMainWindow()
        {
            _navigationService.NavigateToHistory();
        }
    }
}
