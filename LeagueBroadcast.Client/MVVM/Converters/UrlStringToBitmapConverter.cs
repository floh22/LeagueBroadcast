using System;
using System.IO;
using System.Net.Http;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace LeagueBroadcast.Client.MVVM.Converters
{
    public class UrlStringToBitmapConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not string s)
                return null;

            BitmapImage bi = new();

            using HttpClient httpClient = new();
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            using Stream stream = httpClient.GetStreamAsync(s).Result;

            bi.BeginInit();
            bi.StreamSource = stream;
            bi.EndInit();

            stream.Flush();
            stream.Close();
            httpClient.Dispose();

            return bi;
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
