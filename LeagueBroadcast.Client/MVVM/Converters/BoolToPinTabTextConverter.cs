using System;
using System.Globalization;
using System.Windows.Data;

namespace LeagueBroadcast.Client.MVVM.Converters
{
    public class BoolToPinTabTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return "Unpin Tab";
            }

            return "Pin Tab";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
