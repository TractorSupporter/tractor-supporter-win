﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.ViewModel;

class SettingsPageViewModel : BaseViewModel
{
    private string _port;
    private string _portValidationMessage;
    private string _ipValidationMessage;
    private bool _avoidingMechanismIsChecked;
    private bool _alarmMechanismIsChecked;
    private TypeSensor _selectedSensorType;
    private TypeTurn _selectedTurnDirection;
    private int _alarmDistance;
    private int _avoidingDistance;
    private Language _selectedLanguage;
    private ICommand _forwardCommand;
    private ICommand _backCommand;
    private NavigationService _navigationService;
    private LanguageService _languageService;
    private IMockDataReceiver _mockDataReceiver;
    private IReceivedDataFormatter _receivedDataFormatter;
    private IConfigAppJson _configAppJson;
    private IAvoidingService _avoidingService;
    private IAlarmService _alarmService;
    private IDataSenderUDP _dataSenderUDP;


    public SettingsPageViewModel(IReceivedDataFormatter receivedDataFormatter)
    {
        _dataSenderUDP = DataSenderUDP.Instance;
        _alarmService = App.ServiceProvider.GetRequiredService<IAlarmService>();
        _navigationService = NavigationService.Instance;
        _configAppJson = ConfigAppJson.Instance;
        _languageService = LanguageService.Instance;
        _mockDataReceiver = App.ServiceProvider.GetRequiredService<IMockDataReceiver>();
        _receivedDataFormatter = receivedDataFormatter;
        _avoidingService = App.ServiceProvider.GetRequiredService<IAvoidingService>();
        ForwardCommand = new RelayCommand(SaveSettings);
        BackCommand = new RelayCommand(CloseSettings);
        setConfigData();
    }

    public Language SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
                string languageCode = _selectedLanguage.GetDescription();
            }
        }
    }

    public int AlarmDistance
    {
        get => _alarmDistance;
        set
        {
            if (_alarmDistance != value)
            {
                _alarmDistance = value;
                OnPropertyChanged(nameof(AlarmDistance));
            }
        }
    }

    public int AvoidingDistance
    {
        get => _avoidingDistance;
        set
        {
            if (_avoidingDistance != value)
            {
                _avoidingDistance = value;
                OnPropertyChanged(nameof(AvoidingDistance));
            }
        }
    }

    public TypeTurn SelectedTurnDirection
    {
        get => _selectedTurnDirection;
        set
        {
            if (_selectedTurnDirection != value)
            {
                _selectedTurnDirection = value;
                OnPropertyChanged(nameof(SelectedTurnDirection));
            }
        }
    }

    public TypeSensor SelectedSensorType
    {
        get => _selectedSensorType;
        set
        {
            if (_selectedSensorType != value)
            {
                _selectedSensorType = value;
                OnPropertyChanged(nameof(SelectedSensorType));
            }
        }
    }

    public bool AvoidingMechanismIsChecked
    {
        get => _avoidingMechanismIsChecked;
        set
        {
            if (_avoidingMechanismIsChecked != value)
            {
                _avoidingMechanismIsChecked = value;
                OnPropertyChanged(nameof(AvoidingMechanismIsChecked));
            }
        }
    }

    public bool AlarmMechanismIsChecked
    {
        get => _alarmMechanismIsChecked;
        set
        {
            if (_alarmMechanismIsChecked != value)
            {
                _alarmMechanismIsChecked = value;
                OnPropertyChanged(nameof(AlarmMechanismIsChecked));
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
        AvoidingMechanismIsChecked = appConfig.IsAvoidingMechanismTurnedOn;
        AlarmMechanismIsChecked = appConfig.IsAlarmMechanismTurnedOn;
        SelectedSensorType = appConfig.SelectedSensorType;
        AlarmDistance = appConfig.AlarmDistance;
        AvoidingDistance = appConfig.AvoidingDistance;
        SelectedLanguage = appConfig.Language;
        SelectedTurnDirection = appConfig.SelectedTurnDirection;
        
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

        if (isValid)
        {
            _configAppJson.CreateJson(Port, AvoidingMechanismIsChecked, AlarmMechanismIsChecked, SelectedSensorType, AvoidingDistance, AlarmDistance, SelectedLanguage, SelectedTurnDirection);
            _configAppJson.ReadJson();
            _languageService.ChangeLanguage(SelectedLanguage);
            _mockDataReceiver.ChangeInnerMock(SelectedSensorType == TypeSensor.Laser);
            _receivedDataFormatter.ChangeMode(SelectedSensorType == TypeSensor.Laser);
            _avoidingService.ChangeConfig(SelectedSensorType == TypeSensor.Laser);
            _alarmService.ChangeConfig(SelectedSensorType == TypeSensor.Laser);
            _dataSenderUDP.ChangeConfig(portNumber);
            _alarmService.AlarmDistance = AlarmDistance;
            _avoidingService.AvoidingDistance = AvoidingDistance;

            _navigationService.NavigateToMain();
        }
    }

    private void CloseSettings(object parameter)
    {
        _navigationService.NavigateToMain();
    }
}
