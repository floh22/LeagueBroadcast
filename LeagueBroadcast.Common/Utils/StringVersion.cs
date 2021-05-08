using System;
using System.Linq;
using Newtonsoft.Json;

namespace LeagueBroadcast.Common.Utils
{
    //https://github.com/Johannes-Schneider/GoldDiff/blob/master/GoldDiff.Shared/Utility/StringVersion.cs
    public sealed class StringVersion : IEquatable<StringVersion>
    {
        private const string ComponentSeparator = ".";

        [JsonProperty]
        private int[] Components { get; }

        public static StringVersion Zero => new StringVersion(0);
#nullable enable
        public static bool TryParse(string? input, out StringVersion? version)
        {
            try
            {
                var components = input?.Split(new[] { ComponentSeparator }, StringSplitOptions.None) ?? throw new ArgumentNullException(nameof(input));
                version = new StringVersion(components.Select(component => Convert.ToInt32(component)).ToArray());
                return true;
            }
            catch
            {
                version = null;
                return false;
            }
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

        #region IEquatable

        public override bool Equals(object? obj)
        {
            return Equals(obj as StringVersion);
        }

        public bool Equals(StringVersion? other)
        {
            if (ReferenceEquals(other, null))
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
            if (ReferenceEquals(a, null))
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (ReferenceEquals(b, null))
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
            if (ReferenceEquals(a, null))
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (ReferenceEquals(b, null))
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
            if (ReferenceEquals(a, null))
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (ReferenceEquals(b, null))
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
            if (ReferenceEquals(a, null))
            {
                throw new ArgumentNullException(nameof(a));
            }

            if (ReferenceEquals(b, null))
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

        #endregion
    }
#nullable disable
}
