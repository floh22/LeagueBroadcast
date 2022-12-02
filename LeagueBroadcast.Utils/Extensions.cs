using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueBroadcast.Utils
{
    public static class Extensions
    {
        #region Array
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
        #endregion

        #region Byte
        public static byte[] GetSubArray(this byte[] source, int offset, int size)
        {
            byte[] res = new byte[size];
            Buffer.BlockCopy(source, offset, res, 0, size);
            return res;
        }

        public static int ToInt(this byte[] source, int offset = 0)
        {
            return BitConverter.ToInt32(source, offset);
        }

        public static uint ToUInt(this byte[] source, int offset = 0)
        {
            return BitConverter.ToUInt32(source, offset);
        }

        public static short ToShort(this byte[] source, int offset = 0)
        {
            return BitConverter.ToInt16(source, offset);
        }

        public static float ToFloat(this byte[] input, int offset = 0)
        {
            return BitConverter.ToSingle(input, offset);
        }

        public static bool IsASCII(this byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] > 127)
                {
                    return false;
                }
            }
            return true;
        }

        public static string DecodeAscii(this byte[] buffer)
        {
            int count = Array.IndexOf<byte>(buffer, 0, 0);
            if (count < 0) count = buffer.Length;
            return Encoding.ASCII.GetString(buffer, 0, count);
        }

        public static void Write(this byte[] dest, byte[] input) => Write(dest, 0, input);

        public static void Write(this byte[] dest, int offset, byte[] input)
        {
            Array.Copy(input, 0, dest, offset, input.Length);
        }

        public static void Write(this char[] dest, byte[] input)
        {
            Write(dest, 0, input);
        }

        public static void Write(this char[] dest, int offset, byte[] input)
        {
            Array.Copy(input, 0, dest, offset, input.Length);
        }

        //https://stackoverflow.com/a/38625726
        public static int FindPatternIndex(this byte[] buffer, byte[] pattern, int start = 0)
        {
            int maxFirstCharSlot = buffer.Length - pattern.Length + 1;
            for (int i = start; i < maxFirstCharSlot; i++)
            {
                if (buffer[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (buffer[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }


        public static int FindPatternIndex(this byte[] buffer, List<KeyValuePair<int, byte[]>> pattern, int start = 0)
        {
            if (pattern.Count == 0)
                return -1;

            int length = 0;

            foreach(KeyValuePair<int, byte[]> kvp in pattern)
            {
                length += kvp.Key;
                length += kvp.Value.Length;
            }

            int maxFirstCharSlot = buffer.Length - length + 1;

            for (int i = start; i < maxFirstCharSlot; i++)
            {
                if (buffer[i] != pattern[0].Value[0]) // compare only first byte
                    continue;

                int matched = 0;
                int dist = 0;
                //Match each partial pattern iteratively
                while(matched < pattern.Count)
                {
                    //Add offset from last partial pattern
                    dist += pattern[matched].Key;

                    //Iterate through current pattern
                    for(int j = pattern[matched].Value.Length - 1; j >= 1; j--)
                    {
                        if (buffer[dist + j] != pattern[matched].Value[j])
                        {
                            dist = -1;
                            break;
                        }
                        //If partial pattern matches and it is the last, then the full pattern matches
                        if(j == 1 && matched == pattern.Count - 1)
                        {
                            return i;
                        }

                    }
                    if (dist == -1)
                        break;

                    //if dist != -1, then partial pattern is a match
                    matched++;

                    //add length of current partial to length checked
                    dist += pattern[matched].Value.Length;
                }
            }
            return -1;
        }

        public static IEnumerable<int> FindPatternIndices(this byte[] haystack, byte[] needle, int startIndex = 0, bool includeOverlapping = false)
        {
            int matchIndex = haystack.AsSpan(startIndex).IndexOf(needle);
            while (matchIndex >= 0)
            {
                yield return startIndex + matchIndex;
                startIndex += matchIndex + (includeOverlapping ? 1 : needle.Length);
                matchIndex = haystack.AsSpan(startIndex).IndexOf(needle);
            }
        }

        #endregion

        #region Color
        public static string ToSerializedString(this Color c)
        {
            return $"rgb({c.R},{c.G},{c.B})";
        }
        #endregion

        #region Dictionary
        public static T? KeyByValue<T, W>(this Dictionary<T?, W> dict, W val)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            T? key = default;
            foreach (var pair in dict.Where(pair => EqualityComparer<W>.Default.Equals(pair.Value, val)))
            {
                key = pair.Key;
                break;
            }

            return key;
        }
        #endregion

        #region Directory
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
        #endregion

        #region Enum
        public static T Next<T>(this T src) where T : Enum
        {
            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }

        #endregion

        #region Expression
        /// <summary>
        /// Given a MemberExpression (or MemberExpression wrapped in a UnaryExpression), get the name of the property
        /// </summary>
        /// <typeparam name="TDelegate">Type of the delegate</typeparam>
        /// <param name="propertyExpression">Expression describe the property whose name we want to extract</param>
        /// <returns>Name of the property referenced by the expression</returns>
        public static string NameForProperty<TDelegate>(this Expression<TDelegate> propertyExpression)
        {
            Expression body;
            if (propertyExpression.Body is UnaryExpression expression)
                body = expression.Operand;
            else
                body = propertyExpression.Body;

            if (body is not MemberExpression member)
                throw new ArgumentException("Property must be a MemberExpression");

            return member.Member.Name;
        }
        #endregion

        #region HttpClient

        public static async Task<HttpStatusCode> DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            long? contentLength = response.Content.Headers.ContentLength;

            using Stream? download = await response.Content.ReadAsStreamAsync(cancellationToken);

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

        #endregion

        #region int32
        public static byte[] ToByteArray(this int input, Endianness endianness = Endianness.LittleEndian)
        {
            byte[] ret =  new byte[] {
                (byte)(input >> 24),
                (byte)(input >> 16),
                (byte)(input >> 8),
                (byte)(input >> 0),
            };
            if(endianness == Endianness.BigEndian)
                ret = ret.Reverse().ToArray();
            return ret;
        }

        public enum Endianness
        {
            LittleEndian = 0,
            BigEndian = 1
        }
        #endregion

        #region Json
        public static object? ToObject(this JsonElement element, Type returnType, JsonSerializerOptions? options = null)
        {
            ArrayBufferWriter<byte>? bufferWriter = new();
            using (Utf8JsonWriter? writer = new(bufferWriter))
            {
                element.WriteTo(writer);
            }

            return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, returnType, options);
        }

        public static T? ToObject<T>(this JsonElement element)
        {
            string? json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
        public static T? ToObject<T>(this JsonDocument document)
        {
            string? json = document.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
        #endregion

        #region List

        private static int BinarySearch<T>(IList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            var comp = Comparer<T>.Default;
            int lo = 0, hi = list.Count - 1;
            while (lo < hi)
            {
                int m = (hi + lo) / 2;  // this might overflow; be careful.
                if (comp.Compare(list[m], value) < 0) lo = m + 1;
                else hi = m - 1;
            }
            if (comp.Compare(list[lo], value) < 0) lo++;
            return lo;
        }

        public static int FindFirstIndexGreaterThanOrEqualTo<T, U>
                                  (this SortedList<T, U> sortedList, T key)
        {
            return BinarySearch(sortedList.Keys, key);
        }

        #endregion

        #region Object

        //https://stackoverflow.com/a/50128943
        public static bool ArePropertiesNotNull<T>(this T obj)
        {
            return PropertyCache<T>.PublicProperties.All(propertyInfo => propertyInfo.GetValue(obj) != null);
        }

        /// <summary>
        /// Determines whether an object is equal to any of the elements in a sequence.
        /// </summary>
        [Pure]
        public static bool IsEither<T>(this T obj, [NotNull] IEnumerable<T> variants,
            [NotNull] IEqualityComparer<T> comparer)
        {
            variants.GuardNotNull(nameof(variants));
            comparer.GuardNotNull(nameof(comparer));

            return variants.Contains(obj, comparer);
        }

        /// <summary>
        /// Determines whether an object is equal to any of the elements in a sequence.
        /// </summary>
        [Pure]
        public static bool IsEither<T>(this T obj, [NotNull] IEnumerable<T> variants)
            => IsEither(obj, variants, EqualityComparer<T>.Default);

        /// <summary>
        /// Determines whether the object is equal to any of the parameters.
        /// </summary>
        [Pure]
        public static bool IsEither<T>(this T obj, params T[] variants) => IsEither(obj, (IEnumerable<T>)variants);

        public static T GuardNotNull<T>(this T o, string? argName = null)
        {
            if (o == null )
                throw new ArgumentNullException(argName);

            return o;
        }

        public static int GuardNotNegative(this int i, string? argName = null)
        {
            return i >= 0
                ? i
                : throw new ArgumentOutOfRangeException(argName, i, "Cannot be negative");
        }

        #endregion

        #region Stream
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

        #endregion

        #region String
        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", nameof(value));
            List<int> indexes = new();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static Color ToColor(this string str)
        {
            string[]? cleanedColor = str.Replace("rgb(", "").Replace(")", "").Split(",");
            return Color.FromArgb(
                byte.Parse(cleanedColor[0]),
                byte.Parse(cleanedColor[1]),
                byte.Parse(cleanedColor[2])
                );
        }

        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Distance(this string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
        #endregion

        #region Type
        //https://stackoverflow.com/a/19317229
        public static bool ImplementsInterface(this Type type, Type ifaceType)
        {
            Type[] intf = type.GetInterfaces();
            for (int i = 0; i < intf.Length; i++)
            {
                if (intf[i] == ifaceType)
                {
                    return true;
                }
            }

            return false;
        }

        //https://stackoverflow.com/a/63069236
        public static TR? Method<TR>(this Type t, string method, object? obj = null, params object[] parameters)
        {
            return (TR?)t.GetMethod(method)?.Invoke(obj, parameters);
        }
        #endregion

    }
}
