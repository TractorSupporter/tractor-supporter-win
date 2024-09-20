using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TractorSupporter.Services;

namespace TractorSupporter.ViewModel
{
    class SettingsPageViewModel : BaseViewModel
    {
        private ICommand _forwardCommand;
        private ICommand _backCommand;
        private NavigationService _navigationService;

        public SettingsPageViewModel()
        {
            _navigationService = NavigationService.Instance;
            ForwardCommand = new RelayCommand(SaveSettings);
            BackCommand = new RelayCommand(CloseSettings);
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

        private void SaveSettings(object parameter)
        {
            Console.WriteLine("Save");
            _navigationService.NavigateToMain();
        }

        private void CloseSettings(object parameter)
        {
            Console.WriteLine("Close");
            _navigationService.NavigateToMain();
        }
    }
}
