using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace LeagueBroadcast.Client.MVVM.Converters
{
    [ValueConversion(typeof(String), typeof(Color))]
    internal class StringToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string valueString)
            {
                return null;
            }
            try
            {
                return valueString.Contains('#') ? FromHex(valueString) : FromReadable(valueString);
            }
            catch { return null; }
        }

        private Color FromHex(string colorString)
        {
            return (Color)ColorConverter.ConvertFromString(colorString);
        }

        private Color FromReadable(string colorString)
        {
            var cleanedColor = colorString.Replace("rgb(", "").Replace(")", "").Split(",");
            return Color.FromRgb(
                byte.Parse(cleanedColor[0]),
                byte.Parse(cleanedColor[1]),
                byte.Parse(cleanedColor[2])
                );
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Color c)
            {
                return null;
            }

            try
            {
                return $"rgb({c.R},{c.G},{c.B})";
            }
            catch { return null; }
        }
    }
}
