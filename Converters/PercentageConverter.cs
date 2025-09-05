using System.Globalization;
using System.Windows.Data;

namespace FinancialCalculator.Converters
{
    public class PercentConverter : IValueConverter
    {
        // float -> string (e.g., 0.14f → "14.0%")
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float f)
            {
                return f.ToString("P1", culture); // "P1" = Percent with 1 decimal place
            }

            return Binding.DoNothing;
        }

        // string -> float (e.g., "14.0%" → 0.14f)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                s = s.Replace("%", "").Trim();

                if (float.TryParse(s, NumberStyles.Any, culture, out float result))
                {
                    return result / 100f; // convert percent to decimal
                }
            }

            return Binding.DoNothing;
        }
    }
}
