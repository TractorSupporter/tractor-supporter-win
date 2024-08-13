using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace TractorSupporter
{
    /// <summary>
    /// Interaction logic for TSWindow.xaml
    /// </summary>
    public partial class TSWindow : Window
    {
        public TSWindow()
        {
            InitializeComponent();

            setMyIP();

            Thread thdUdpServer = new Thread(new ThreadStart(serverThread));

            thdUdpServer.IsBackground = true;
            thdUdpServer.Start();
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
            UdpClient udpClient = new UdpClient(8080);
            while (true)
            {
                IPEndPoint RemoteIpEndpoint = new IPEndPoint(IPAddress.Any, 8080);
                Byte[] receivedBytes = udpClient.Receive(ref RemoteIpEndpoint);
                string serializedData = Encoding.ASCII.GetString(receivedBytes);

                using (JsonDocument data = JsonDocument.Parse(serializedData))
                {
                    JsonElement dataRoot = data.RootElement;
                    string extraMessage = dataRoot.GetProperty("extraMessage").GetString()!;
                    double distanceMeasured = dataRoot.GetProperty("distanceMeasured").GetDouble();

                    Dispatcher.Invoke(() =>
                    {
                        tb_IPSender.Text = RemoteIpEndpoint.Address.ToString();
                        tb_IPDestination.Text = tb_IPSender.Text;

                        date = DateTime.Now.ToString("hh:mm:ss");
                        extraMessage = "time: " + date + " | " + extraMessage;
                        tb_ReceivedMessages.AppendText(extraMessage + "\n");
                        tb_ReceivedMessages.ScrollToEnd();

                        tb_DistanceMeasured.Text = distanceMeasured.ToString();
                    });
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