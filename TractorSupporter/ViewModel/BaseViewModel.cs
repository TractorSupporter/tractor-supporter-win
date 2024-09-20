using System.ComponentModel;

namespace TractorSupporter.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler RequestClose;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Close(object parameter)
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}