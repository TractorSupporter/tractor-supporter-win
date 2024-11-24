using System.ComponentModel;

namespace TractorSupporter.Services;

public partial class HistoryVisibilityService : INotifyPropertyChanged
{
    private bool _isHistoryVisible;
    public bool IsHistoryVisible
    {
        get => _isHistoryVisible;
        set
        {
            _isHistoryVisible = value;
            OnPropertyChanged(nameof(IsHistoryVisible));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

#region Class structure 
public partial class HistoryVisibilityService
{
    private static readonly HistoryVisibilityService _instance = new HistoryVisibilityService();
    public static HistoryVisibilityService Instance => _instance;
}
#endregion
