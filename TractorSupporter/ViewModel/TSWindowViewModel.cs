using System.Configuration;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using TractorSupporter.Model;
using TractorSupporter.ViewModel;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;
using TractorSupporter.Services;
using TractorSupporter.Services.Interfaces;

namespace TractorSupporter.ViewModel
{
    public class TSWindowViewModel : BaseViewModel
    {
        private bool useMockData;
        private readonly TSDataSender _dataSender;
        private string _myIP;
        private string _ipSender;
        private string _ipDestination;
        private string _sendMessage;
        private string _distanceMeasured;
        private FlowDocument _receivedMessages;

        public TSWindowViewModel()
        {

            _receivedMessages = new FlowDocument();
            InitMockConfigWindow();
            SetMyIP();
            StartServerThread();
            //_dataSender = new DistanceDataSender("DistancePipe");
        }

        public string MyIP
        {
            get => _myIP;
            set { _myIP = value; OnPropertyChanged(nameof(MyIP)); }
        }

        public string IPSender
        {
            get => _ipSender;
            set { _ipSender = value; OnPropertyChanged(nameof(IPSender)); }
        }

        public string IPDestination
        {
            get => _ipDestination;
            set { _ipDestination = value; OnPropertyChanged(nameof(IPDestination)); }
        }

        public string SendMessage
        {
            get => _sendMessage;
            set { _sendMessage = value; OnPropertyChanged(nameof(SendMessage)); }
        }

        public string DistanceMeasured
        {
            get => _distanceMeasured;
            set { _distanceMeasured = value; OnPropertyChanged(nameof(DistanceMeasured)); }
        }

        public FlowDocument ReceivedMessages
        {
            get => _receivedMessages;
            set { _receivedMessages = value; OnPropertyChanged(nameof(ReceivedMessages)); }
        }


        private void SetMyIP()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostEntry(hostName)
                .AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            MyIP = myIP;
        }

        private void InitMockConfigWindow()
        {
            useMockData = bool.Parse(ConfigurationManager.AppSettings["UseMockData"]);
            if (useMockData)
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
            IDataReceiverAsync espDataReceiver = useMockData ? new MockDataReceiver() : UdpDataReceiver.Instance(8080);

            while (true)
            {
                Byte[] receivedBytes = espDataReceiver.ReceiveDataAsync().Result;
                string serializedData = Encoding.ASCII.GetString(receivedBytes);

                using (JsonDocument data = JsonDocument.Parse(serializedData))
                {
                    JsonElement dataRoot = data.RootElement;
                    string extraMessage = dataRoot.GetProperty("extraMessage").GetString()!;
                    double distanceMeasured = dataRoot.GetProperty("distanceMeasured").GetDouble();

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        IPSender = espDataReceiver.GetRemoteIpAddress();
                        IPDestination = IPSender;

                        string date = DateTime.Now.ToString("hh:mm:ss");
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

                        DistanceMeasured = distanceMeasured.ToString();
                        //_dataSender.SendDistanceData(distanceMeasured);
                    });
                }

                if (useMockData)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public ICommand SendMessageCommand => new RelayCommand(SendMessageExecute);

        private void SendMessageExecute(object parameter)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(IPDestination, Convert.ToInt16(parameter));
            Byte[] dataToSend = Encoding.ASCII.GetBytes(SendMessage);
            udpClient.Send(dataToSend, dataToSend.Length);
        }
    }
}