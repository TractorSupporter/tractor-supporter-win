using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TractorSupporter.ViewModel
{
    class SettingsWindowViewModel : BaseViewModel
    {
        private ICommand _forwardCommand;
        private ICommand _backCommand;

        public SettingsWindowViewModel()
        {
            ForwardCommand = new RelayCommand(Save);
            BackCommand = new RelayCommand(Close);
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

        private void Save(object parameter)
        {
            Console.WriteLine("Save");
        }
    }
}
