using LeagueBroadcast.Utils.Log;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeagueBroadcast.Utils
{

    public class LeagueVersionDeterminedEventArgs : EventArgs
    {
        public StringVersion Patch;

        public LeagueVersionDeterminedEventArgs(StringVersion patch)
        {
            Patch = patch;
        }

        public LeagueVersionDeterminedEventArgs(string patch)
        {
            if (!StringVersion.TryParse(patch, out StringVersion? res))
            {
                $"Could not parse League Version".Error();
            }
            res ??= StringVersion.Zero;
            Patch = res;
        }
    }

    //Adapted from https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Utility/StringVersion.cs
    [JsonConverter(typeof(StringVersionConverter))]
    public sealed class StringVersion : IEquatable<StringVersion>
    {
        private const string ComponentSeparator = ".";

        [JsonInclude]
        private int[] Components { get; }

        public static StringVersion Zero => new(0);

        public static StringVersion CallingAppVersion => new(Parse(Assembly.GetCallingAssembly().GetName().Version!.ToString()).Components.Take(3).ToArray());

        public static StringVersion FullCallingAppVersion => Parse(Assembly.GetCallingAssembly().GetName().Version!.ToString());

        private static StringVersion? _appVersion;
        public static StringVersion AppVersion => GetSimpleLocalVersion();

        public static event EventHandler<LeagueVersionDeterminedEventArgs>? LeagueVersionDetermined;

        private static StringVersion? _leagueVersion;

        public static StringVersion LeagueVersion => _leagueVersion??StringVersion.Zero;


        private static StringVersion? _lcuVersion;

        public static StringVersion LCUVersion => _lcuVersion ?? StringVersion.Zero;

        public static void SetLeagueVersion(StringVersion leagueVersion)
        {
            _leagueVersion = leagueVersion;
        }

        public static void SetLCUClientVersion(StringVersion lcuVersion)
        {
            _lcuVersion = lcuVersion;
        }


        public int Major => Components.Length >= 1 ? Components[0]: 0;
        public int Minor => Components.Length >= 2? Components[1] : 0;
        public int Patch => Components.Length >= 3 ? Components[2] : 0;

#nullable enable
        public static bool TryParse(string? input, out StringVersion? version)
        {
            try
            {
                string[]? components = input?.Split(new[] { ComponentSeparator }, StringSplitOptions.None) ?? throw new ArgumentNullException(nameof(input));
                version = new StringVersion(components.Where(c => Microsoft.VisualBasic.Information.IsNumeric(c)).Select(component => Convert.ToInt32(component)).ToArray());
                return true;
            }
            catch
            {
                version = null;
                return false;
            }
        }

        public static StringVersion Parse(string? input)
        {
            return !TryParse(input, out StringVersion? version) ? Zero : version ?? Zero;
        }

        public StringVersion(params int[]? components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            if (components.Length < 1)
            {
                throw new AggregateException($"{nameof(components)} must contain at least 1 element!");
            }

            if (components.Any(component => component < 0))
            {
                throw new ArgumentException($"{nameof(components)} must not contain any negative elements!");
            }


            Components = components;
        }

        public override string ToString()
        {
            return string.Join(ComponentSeparator, Components);
        }

        public string ToString(int count)
        {
            if(Components.Length < count)
            {
                count = Components.Length;
            }
            return string.Join(ComponentSeparator, Components.SubArray(0, count));
        }

        private static StringVersion GetSimpleLocalVersion()
        {
            if (_appVersion is not null)
                return AppVersion;

            _appVersion = new((Parse(FileVersionInfo.GetVersionInfo("LeagueBroadcast.exe").FileVersion!) ?? Zero).Components.Take(3).ToArray());
            return _appVersion;
        }

        #region IEquatable

        public override bool Equals(object? obj)
        {
            return Equals(obj as StringVersion);
        }

        public bool Equals(StringVersion? other)
        {
            if (other is null)
            {
                return false;
            }

            if (Components.Length != other.Components.Length)
            {
                return false;
            }

            return !Components.Where((component, componentIndex) => component != other.Components[componentIndex]).Any();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Components);
        }

        #endregion

        #region Operators

        public static bool operator ==(StringVersion? a, StringVersion? b)
        {
            return a?.Equals(b) == true;
        }

        public static bool operator !=(StringVersion? a, StringVersion? b)
        {
            return a?.Equals(b) == false;
        }

        public static bool operator >(StringVersion? a, StringVersion? b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            for (var componentIndex = 0; componentIndex < Math.Max(a.Components.Length, b.Components.Length); ++componentIndex)
            {
                var componentA = componentIndex < a.Components.Length ? a.Components[componentIndex] : 0;
                var componentB = componentIndex < b.Components.Length ? b.Components[componentIndex] : 0;

                if (componentA > componentB)
                {
                    return true;
                }

                if (componentA < componentB)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool operator >=(StringVersion? a, StringVersion? b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            for (var componentIndex = 0; componentIndex < Math.Max(a.Components.Length, b.Components.Length); ++componentIndex)
            {
                var componentA = componentIndex < a.Components.Length ? a.Components[componentIndex] : 0;
                var componentB = componentIndex < b.Components.Length ? b.Components[componentIndex] : 0;

                if (componentA > componentB)
                {
                    return true;
                }

                if (componentA < componentB)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator <(StringVersion? a, StringVersion? b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            for (var componentIndex = 0; componentIndex < Math.Max(a.Components.Length, b.Components.Length); ++componentIndex)
            {
                var componentA = componentIndex < a.Components.Length ? a.Components[componentIndex] : 0;
                var componentB = componentIndex < b.Components.Length ? b.Components[componentIndex] : 0;

                if (componentA < componentB)
                {
                    return true;
                }

                if (componentA > componentB)
                {
                    return false;
                }
            }

            return false;
        }

        public static bool operator <=(StringVersion? a, StringVersion? b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            for (var componentIndex = 0; componentIndex < Math.Max(a.Components.Length, b.Components.Length); ++componentIndex)
            {
                var componentA = componentIndex < a.Components.Length ? a.Components[componentIndex] : 0;
                var componentB = componentIndex < b.Components.Length ? b.Components[componentIndex] : 0;

                if (componentA < componentB)
                {
                    return true;
                }

                if (componentA > componentB)
                {
                    return false;
                }
            }

            return true;
        }

        public static StringVersion operator +(StringVersion? a, StringVersion? b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            int[] outComponents = new int[Math.Max(a.Components.Length, b.Components.Length)];
            for (var componentIndex = 0; componentIndex < outComponents.Length; ++componentIndex)
            {
                outComponents[componentIndex] = a.Components.ElementAtOrDefault(componentIndex) + b.Components.ElementAtOrDefault(componentIndex);
            }

            return new(outComponents);
        }

        public static StringVersion operator -(StringVersion? a, StringVersion? b)
        {
            if (a is null)
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (b is null)
            {
                throw new ArgumentNullException(nameof(b));
            }

            int[] outComponents = new int[Math.Max(a.Components.Length, b.Components.Length)];
            for (var componentIndex = 0; componentIndex < outComponents.Length; ++componentIndex)
            {
                outComponents[componentIndex] = a.Components.ElementAtOrDefault(componentIndex) - b.Components.ElementAtOrDefault(componentIndex);
            }

            return new(outComponents);
        }

        #endregion
    }
#nullable disable

    public class StringVersionConverter : JsonConverter<StringVersion>
    {
        public override StringVersion Read(ref Utf8JsonReader reader,
                                      Type typeToConvert,
                                      JsonSerializerOptions options)
        {
            return StringVersion.TryParse(reader.GetString(), out var version) ? version : StringVersion.Zero;
        }

        public override void Write(Utf8JsonWriter writer,
                                   StringVersion value,
                                   JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}

