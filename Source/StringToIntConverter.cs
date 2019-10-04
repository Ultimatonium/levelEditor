using System;
using System.Globalization;
using System.Windows.Data;

namespace LevelEditor
{
    class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                if (int.TryParse(s, out int i))
                {
                    return i;
                }
            }
            return null;
        }
    }
}
