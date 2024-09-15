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
    public class StarterConfigViewModel : BaseViewModel
    {
        private string _port;
        private string _ipAddress;
        private string _portValidationMessage;
        private string _ipValidationMessage;
        private ICommand _validateCommand;
        private ICommand _closeCommand;
        private IWindowService _windowService;
        private IConfigAppJson _configAppJson;
        public event EventHandler RequestClose;

        public StarterConfigViewModel()
        {
            _windowService = new WindowService();
            ValidateCommand = new RelayCommand(Validate);
            CloseCommand = new RelayCommand(Close);
            _configAppJson = ConfigAppJson.Instance;
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

        public ICommand ValidateCommand
        {
            get => _validateCommand;
            set
            {
                _validateCommand = value;
                OnPropertyChanged(nameof(ValidateCommand));
            }
        }

        public ICommand CloseCommand
        {
            get => _closeCommand;
            set
            {
                _closeCommand = value;
                OnPropertyChanged(nameof(CloseCommand));
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
                _windowService.OpenMainWindow();
                Close(new object());
            }
        }

        private void Close(object parameter)
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
