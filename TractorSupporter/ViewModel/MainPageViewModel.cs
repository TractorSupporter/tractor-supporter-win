﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Printing;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.ViewModel
{
    public class MainPageViewModel : BaseViewModel
    {
        private bool _useMockData;
        private string _distanceToObstacle;
        private string _sendMessage;
        private string _ipSender;
        private string _ipDestination;
        private bool _isConnected;
        private ICommand _startConnectionCommand;
        private FlowDocument _receivedMessages;
        private readonly DistanceDataSender _dataSender;
        private INavigationService _navigationService;
        private AppConfig _appConfig;
        private int _port;
        private string _ipAddress;

        public MainPageViewModel()
        {
            _navigationService = NavigationService.Instance;
            _receivedMessages = new FlowDocument();
            StartConnectionCommand = new RelayCommand(StartConnection);
            _appConfig = ConfigAppJson.Instance.readJson();
            _port = _appConfig.Port;
            InitMockConfigWindow();
            //_dataSender = new DistanceDataSender("DistancePipe");
        }

        public string DistanceToObstacle
        {
            get => _distanceToObstacle;
            set { _distanceToObstacle = value; OnPropertyChanged(nameof(DistanceToObstacle)); }
        }

        public FlowDocument ReceivedMessages
        {
            get => _receivedMessages;
            set { _receivedMessages = value; OnPropertyChanged(nameof(ReceivedMessages)); }
        }

        public string SendMessage
        {
            get => _sendMessage;
            set { _sendMessage = value; OnPropertyChanged(nameof(SendMessage)); }
        }

        public string IPSender
        {
            get => _ipSender;
            set { _ipSender = value; OnPropertyChanged(nameof(IPSender)); }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set { _isConnected = value; OnPropertyChanged(nameof(IsConnected)); }
        }

        public ICommand StartConnectionCommand
        {
            get => _startConnectionCommand;
            set
            {
                _startConnectionCommand = value;
                OnPropertyChanged(nameof(StartConnectionCommand));
            }
        }

        private void InitMockConfigWindow()
        {
            _useMockData = bool.Parse(ConfigurationManager.AppSettings["UseMockData"]);
            if (_useMockData)
            {
                var configWindow = new MockDataConfigWindow();
                configWindow.Show();
            }
        }

        private void StartServerThread()
        {
            Thread thdUdpServer = new Thread(new ThreadStart(ServerThread));
            thdUdpServer.IsBackground = true;
            thdUdpServer.Start();
        }

        private void ServerThread()
        {
            IDataReceiver dataReceiver = _useMockData ? new MockDataReceiver() : new UdpDataReceiver(_port);

            while (IsConnected)
            {
                Byte[] receivedBytes = dataReceiver.ReceiveData();
                string serializedData = Encoding.ASCII.GetString(receivedBytes);

                using (JsonDocument data = JsonDocument.Parse(serializedData))
                {
                    JsonElement dataRoot = data.RootElement;
                    string extraMessage = dataRoot.GetProperty("extraMessage").GetString()!;
                    double distanceMeasured = dataRoot.GetProperty("distanceMeasured").GetDouble();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        IPSender = dataReceiver.GetRemoteIpAddress();
                        _ipDestination = IPSender;

                        AddParagraphToReceivedMessages(extraMessage);

                        DistanceToObstacle = distanceMeasured.ToString();
                        //_dataSender.SendDistanceData(distanceMeasured);
                    });
                }

                if (_useMockData)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public ICommand SendMessageCommand => new RelayCommand(SendMessageExecute);

        private void AddParagraphToReceivedMessages(string extraMessage)
        { 
            string datetime = DateTime.Now.ToString("hh:mm:ss");
            extraMessage = "time: " + datetime + " | " + extraMessage;
            Paragraph newParagraph = new Paragraph(new Run(extraMessage));

            if (ReceivedMessages.Blocks.FirstBlock != null)
            {
                ReceivedMessages.Blocks.InsertBefore(ReceivedMessages.Blocks.FirstBlock, newParagraph);
            }
            else
            {
                ReceivedMessages.Blocks.Add(newParagraph);
            }
        }
        private void SendMessageExecute(object parameter)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(_ipDestination, Convert.ToInt16(parameter));
            Byte[] dataToSend = Encoding.ASCII.GetBytes(SendMessage);
            udpClient.Send(dataToSend, dataToSend.Length);
        }

        private void StartConnection(object parameter)
        {
            IsConnected = !IsConnected;
            if (IsConnected)
            {
                StartServerThread();
            }
        }

        public void CloseMainWindow()
        {
            _navigationService.NavigateToSettings();
        }
    }
}
