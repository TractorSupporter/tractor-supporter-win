using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TractorSupporter.Model;
using TractorSupporter.Model.Enums;
using TractorSupporter.ViewModel;

namespace TractorSupporter.ViewModel
{
    public class MockDataConfigWindowViewModel : BaseViewModel
    {
        private double _speed;
        private DispatcherTimer _timer;
        private readonly int _timerInterval = 500;
        private TractorState _tractorState;

        public MockDataConfigWindowViewModel()
        {
            InitializeTimer();
        }

        public string ExtraMessage
        {
            get => MockDataReceiver.ExtraMessage;
            set { MockDataReceiver.ExtraMessage = value; OnPropertyChanged(nameof(ExtraMessage)); }
        }

        public double DistanceMeasured
        {
            get => MockDataReceiver.DistanceMeasured;
            set { MockDataReceiver.DistanceMeasured = value; OnPropertyChanged(nameof(DistanceMeasured)); }
        }

        public TractorState TractorState
        {
            get => _tractorState;
            set
            {
                _tractorState = value;
                OnPropertyChanged(nameof(TractorState));
                OnPropertyChanged(nameof(SpeedVisibility));
                if (_tractorState == TractorState.Stationary)
                {
                    Speed = 0;
                }
            }
        }

        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                OnPropertyChanged(nameof(Speed));
                if (_speed > 0)
                {
                    _timer.Start();
                } else
                {
                    _timer.Stop();
                }
            }
        }

        public Visibility SpeedVisibility => _tractorState.Equals(TractorState.Moving) ? Visibility.Visible : Visibility.Collapsed;

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(_timerInterval);
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (MockDataReceiver.DistanceMeasured > 0 && Speed > 0)
            {
                double speedInMetersPerSecond = Speed * 1000 / 3600;
                double distanceChange = speedInMetersPerSecond * (_timerInterval / 10.0);
                MockDataReceiver.DistanceMeasured -= distanceChange;
                OnPropertyChanged(nameof(DistanceMeasured));
            }
        }
    }
}