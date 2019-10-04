using System;
using System.Globalization;
using System.Windows.Data;

namespace LevelEditor
{
    class IsStringFilledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    return true;
                }
                return false;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
