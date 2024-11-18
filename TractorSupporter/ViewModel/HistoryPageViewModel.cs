using System.Windows.Input;
using TractorSupporter.Services;

namespace TractorSupporter.ViewModel;

public class HistoryPageViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private ICommand _forwardCommand;
    private ICommand _backCommand;

    public HistoryPageViewModel()
    {
        _navigationService = NavigationService.Instance;
        ForwardCommand = new RelayCommand(CloseHistory);
        BackCommand = new RelayCommand(CloseHistory);
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

    private void CloseHistory(object parameter)
    {
        _navigationService.NavigateToMain();
    }
}
