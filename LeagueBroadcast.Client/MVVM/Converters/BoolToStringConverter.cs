using System;
using System.Globalization;
using System.Windows.Data;

namespace LeagueBroadcast.Client.MVVM.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public char Separator { get; set; } = ';';

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            var strings = ((string)parameter).Split(Separator);
            var trueString = strings[0];
            var falseString = strings[1];

            var boolValue = (bool)value;
            if (boolValue == true)
            {
                return trueString;
            }
            else
            {
                return falseString;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            var strings = ((string)parameter).Split(Separator);
            var trueString = strings[0];
            var falseString = strings[1];

            var stringValue = (string)value;
            if (stringValue == trueString)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
