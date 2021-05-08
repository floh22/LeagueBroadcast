using System;
using System.Collections.Generic;
using System.Text;

namespace LeagueBroadcast.Common.Utils
{
    public static class ByteUtils
    {
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
                if(input[i] > 127)
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
    }
}
