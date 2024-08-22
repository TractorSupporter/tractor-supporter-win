using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;
using TractorSupporter.Interfaces;
using TractorSupporter.Services;

namespace TractorSupporter
{
    /// <summary>
    /// Interaction logic for TSWindow.xaml
    /// </summary>
    public partial class TSWindow : Window
    {
        private bool useMockData = false;

        public TSWindow()
        {
            InitializeComponent();
            setMyIP();

            Thread thdUdpServer = new Thread(new ThreadStart(serverThread));

            thdUdpServer.IsBackground = true;
            thdUdpServer.Start();

            // Working with mocks
            useMockData = bool.Parse(ConfigurationManager.AppSettings["UseMockData"]);
            if (useMockData)
            {
                var configWindow = new MockDataConfigWindow();
                configWindow.Show();
            }
        }

        private void setMyIP()
        {
            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostEntry(hostName)
                .AddressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            tb_MyIP.Text = myIP;
        }

        string data = "";
        string date = "";

        public void serverThread()
        {
            IDataReceiver dataReceiver = useMockData ? new MockDataReceiver() : new UdpDataReceiver(8080);

            while (true)
            {
                Byte[] receivedBytes = dataReceiver.ReceiveData();
                string serializedData = Encoding.ASCII.GetString(receivedBytes);

                using (JsonDocument data = JsonDocument.Parse(serializedData))
                {
                    JsonElement dataRoot = data.RootElement;
                    string extraMessage = dataRoot.GetProperty("extraMessage").GetString()!;
                    double distanceMeasured = dataRoot.GetProperty("distanceMeasured").GetDouble();

                    Dispatcher.Invoke(() =>
                    {
                        tb_IPSender.Text = dataReceiver.GetRemoteIpAddress();
                        tb_IPDestination.Text = tb_IPSender.Text;

                        date = DateTime.Now.ToString("hh:mm:ss");
                        extraMessage = "time: " + date + " | " + extraMessage;
                        tb_ReceivedMessages.AppendText(extraMessage + "\n");
                        tb_ReceivedMessages.ScrollToEnd();

                        tb_DistanceMeasured.Text = distanceMeasured.ToString();
                    });
                }

                // Working with mocks
                if (useMockData)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void b_SendMessage_Click(object sender, RoutedEventArgs e)
        {
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(tb_IPDestination.Text, Convert.ToInt16(tb_Port.Text));
            Byte[] dataToSend = Encoding.ASCII.GetBytes(tb_SendMessage.Text);
            udpClient.Send(dataToSend, dataToSend.Length);
        }
    }
}