using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Threading;

namespace LeagueBroadcast.Common.Utils
{
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

        public static StringVersion AppVersion => GetSimpleLocalVersion();


        public int Major => Components[0];
        public int Minor => Components[1];
        public int Patch => Components[2];

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


            if(components.Length < 3)
            {
                
                components = components.Concat(Enumerable.Repeat(0, 3 - components.Length)).ToArray();
            }

            Components = components;
        }

        public override string ToString()
        {
            return string.Join(ComponentSeparator, Components);
        }

        public string ToString(int count)
        {
            return string.Join(ComponentSeparator, Components.SubArray(0, count));
        }


        private static StringVersion GetSimpleLocalVersion()
        {
            return new((Parse(FileVersionInfo.GetVersionInfo("LeagueBroadcast.exe").FileVersion!) ?? Zero).Components.Take(3).ToArray());
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


//temp put it in here
public static class TemporaryExtensions
{
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        T[] result = new T[length];
        Array.Copy(array, offset, result, 0, length);
        return result;
    }

    public static void Empty(this DirectoryInfo directory)
    {
        foreach (FileInfo file in directory.EnumerateFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo subDirectory in directory.EnumerateDirectories())
        {
            subDirectory.Delete(true);
        }
    }

    public static async Task<HttpStatusCode> DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
    {
        // Get the http headers first to examine the content length
        using HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        long? contentLength = response.Content.Headers.ContentLength;

        using Stream? download = await response.Content.ReadAsStreamAsync();

        // Ignore progress reporting when no progress reporter was 
        // passed or when the content length is unknown
        if (progress == null || !contentLength.HasValue)
        {
            await download.CopyToAsync(destination, cancellationToken);
            return response.StatusCode;
        }

        // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
        Progress<long>? relativeProgress = new(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
        // Use extension method to report progress while downloading
        await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
        progress.Report(1);
        return response.StatusCode;
    }

    public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (!source.CanRead)
        {
            throw new ArgumentException("Has to be readable", nameof(source));
        }

        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (!destination.CanWrite)
        {
            throw new ArgumentException("Has to be writable", nameof(destination));
        }

        if (bufferSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        byte[] buffer = new byte[bufferSize];
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = await source.ReadAsync(buffer, cancellationToken).ConfigureAwait(false)) != 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            progress?.Report(totalBytesRead);
        }
    }
}
