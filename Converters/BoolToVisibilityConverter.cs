using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FinancialCalculator.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value is bool boolValue)
            {
                bool invert = false;
                if(parameter != null) bool.TryParse(parameter.ToString(), out invert);

                if (invert) boolValue = !boolValue;

                if(boolValue) return Visibility.Visible;
                else return Visibility.Collapsed;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;

                if (parameter != null && bool.TryParse(parameter.ToString(), out bool invert) && invert) result = !result;

                return result;
            }

            return Binding.DoNothing;
        }
    }
}
