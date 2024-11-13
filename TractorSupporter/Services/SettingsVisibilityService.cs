using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Services;

public partial class SettingsVisibilityService : INotifyPropertyChanged
{
    private bool _isSettingsVisible;
    public bool IsSettingsVisible
    {
        get => _isSettingsVisible;
        set
        {
            _isSettingsVisible = value;
            OnPropertyChanged(nameof(IsSettingsVisible));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

#region Class structure 
public partial class SettingsVisibilityService
{
    private static readonly SettingsVisibilityService _instance = new SettingsVisibilityService();
    public static SettingsVisibilityService Instance => _instance;
}
#endregion
