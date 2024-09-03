using System;
using System.Globalization;
using System.Windows.Data;
using TractorSupporter.Model.Enums;

namespace TractorSupporter.ViewModel
{
    public class TractorStateToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TractorState state && parameter is string stateString)
            {
                return state.ToString() == stateString;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked && parameter is string stateString)
            {
                return Enum.Parse(typeof(TractorState), stateString);
            }
            return Binding.DoNothing;
        }
    }
}