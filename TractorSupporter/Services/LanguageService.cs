using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TractorSupporter.Model.Enums;
using System.Windows;

namespace TractorSupporter.Services;

public partial class LanguageService
{
    public void ChangeLanguage(Language selectedLanguage)
    {
        var dict = new ResourceDictionary();
        switch (selectedLanguage)
        {
            case Language.English:
                dict.Source = new Uri("Dictionary-en-US.xaml", UriKind.Relative);
                break;
            case Language.Polish:
                dict.Source = new Uri("Dictionary-pl-PL.xaml", UriKind.Relative);
                break;
        }

        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(dict);
    }
}

#region Class structure 
public partial class LanguageService
{
    private static readonly Lazy<LanguageService> _lazyInstance = new(() => new LanguageService());
    public static LanguageService Instance => _lazyInstance.Value;
}
#endregion
