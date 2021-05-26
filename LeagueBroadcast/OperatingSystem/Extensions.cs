using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace LeagueBroadcast.OperatingSystem
{
    public static class Extensions
    {
        public static T Next<T>(this T src) where T : Enum
        {
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        public static Color ToColor(this string str)
        {
            var cleanedColor = str.Replace("rgb(", "").Replace(")", "").Split(",");
            return Color.FromRgb(
                byte.Parse(cleanedColor[0]),
                byte.Parse(cleanedColor[1]),
                byte.Parse(cleanedColor[2])
                );
        }

        public static string ToSerializedString(this Color c)
        {
            return $"rgb({c.R},{c.G},{c.B})";
        }
    }
}
