using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TractorSupporter.Services;

namespace TractorSupporter
{
    /// <summary>
    /// Interaction logic for MockDataConfigWindow.xaml
    /// </summary>
    public partial class MockDataConfigWindow : Window
    {
        private double Speed { get; set; }
        private DispatcherTimer timer;
        private readonly int timerInterval = 500;

        public MockDataConfigWindow()
        {
            InitializeTimer();
            InitializeComponent();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (MockDataReceiver.DistanceMeasured > 0 && Speed > 0)
            {
                double speedInMetersPerSecond = Speed * 1000 / 3600;
                double distanceChange = speedInMetersPerSecond * (timerInterval / 10.0);
                MockDataReceiver.DistanceMeasured -= distanceChange;
                tbDistanceMeasured.Text = MockDataReceiver.DistanceMeasured.ToString("F2");
            }
        }

        private void tbExtraMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            MockDataReceiver.ExtraMessage = tbExtraMessage.Text;
        }

        private void tbDistanceMeasured_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(tbDistanceMeasured.Text, out double distance))
            {
                MockDataReceiver.DistanceMeasured = distance;
                tbDistanceMeasured.Background = System.Windows.Media.Brushes.White;
            }
            else
            {
                tbDistanceMeasured.Background = System.Windows.Media.Brushes.LightCoral;
            }
        }

        private void tbSpeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(tbSpeed.Text, out double speed))
            {
                Speed = speed;
                tbSpeed.Background = System.Windows.Media.Brushes.White;

                if (Speed > 0)
                {
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                }
            }
            else
            {
                tbSpeed.Background = System.Windows.Media.Brushes.LightCoral;
                timer.Stop();
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rbMoving.IsChecked == true)
            {
                tbSpeedLabel.Visibility = Visibility.Visible;
                tbSpeed.Visibility = Visibility.Visible;
            }
            else
            {
                tbSpeedLabel.Visibility = Visibility.Collapsed;
                tbSpeed.Visibility = Visibility.Collapsed;
                timer.Stop();
            }
        }
    }
}
