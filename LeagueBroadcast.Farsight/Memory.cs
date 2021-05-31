using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LeagueBroadcast.Common;

namespace LeagueBroadcast.Farsight
{
    //https://github.com/C0reTheAlpaca/C0reExternal-Base-v2/blob/master/Memory.cs
    public class Memory
    {
        public static Process m_Process;
        public static IntPtr m_pProcessHandle;

        public static int m_iNumberOfBytesRead = 0;
        public static int m_iNumberOfBytesWritten = 0;

        public static int m_baseAddress = 0;

        public static bool IsConnected => m_pProcessHandle != (IntPtr)0;


        public static bool Initialize(Process p )
        {
            m_Process = p;
            m_pProcessHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, m_Process.Id); // Sets Our ProcessHandle
            m_Process.Exited += (s, e) => { m_Process = null; m_pProcessHandle = (IntPtr)0; m_iNumberOfBytesRead = 0; m_iNumberOfBytesWritten = 0; };

            m_baseAddress = m_Process.MainModule.BaseAddress.ToInt32();
            Log.Verbose("Attached to League Process");

            return true;
        }

        public static IntPtr GetModuleAddress(string ModuleName)
        {
            try
            {
                foreach (ProcessModule ProcMod in m_Process.Modules)
                {
                    Log.Verbose($"Checking Module {ProcMod.ModuleName}");

                    if (ModuleName == ProcMod.ModuleName)
                    {
                        return ProcMod.BaseAddress;
                    }
                }
            }
            catch { }
            return new IntPtr(-1);
        }

        public static T ReadMemory<T>(int Address) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T)); // Get ByteSize Of DataType
            byte[] buffer = new byte[ByteSize]; // Create A Buffer With Size Of ByteSize
            ReadProcessMemory((int)m_pProcessHandle, Address, buffer, buffer.Length, ref m_iNumberOfBytesRead); // Read Value From Memory

            return ByteArrayToStructure<T>(buffer); // Transform the ByteArray to The Desired DataType
        }

        public static byte[] ReadMemory(int Address, int size)
        {
            var buffer = new byte[size];

            ReadProcessMemory((int)m_pProcessHandle, Address, buffer, size, ref m_iNumberOfBytesRead);

            return buffer;
        }

        public static float[] ReadMatrix<T>(int Address, int MatrixSize) where T : struct
        {
            int ByteSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[ByteSize * MatrixSize]; // Create A Buffer With Size Of ByteSize * MatrixSize
            ReadProcessMemory((int)m_pProcessHandle, Address, buffer, buffer.Length, ref m_iNumberOfBytesRead);

            return ConvertToFloatArray(buffer); // Transform the ByteArray to A Float Array (PseudoMatrix ;P)
        }

        public static void WriteMemory<T>(int Address, object Value)
        {
            byte[] buffer = StructureToByteArray(Value); // Transform Data To ByteArray 

            WriteProcessMemory((int)m_pProcessHandle, Address, buffer, buffer.Length, out m_iNumberOfBytesWritten);
        }

        public static void WriteMemory<T>(int Address, char[] Value)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(Value);

            WriteProcessMemory((int)m_pProcessHandle, Address, buffer, buffer.Length, out m_iNumberOfBytesWritten);
        }

        #region Transformation
        public static float[] ConvertToFloatArray(byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            float[] floats = new float[bytes.Length / 4];

            for (int i = 0; i < floats.Length; i++)
                floats[i] = BitConverter.ToSingle(bytes, i * 4);

            return floats;
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        private static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);

            byte[] arr = new byte[len];

            IntPtr ptr = Marshal.AllocHGlobal(len);

            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }

        public static int GetChampionObjectSize(int Address)
        {
            var res = new MEMORY_BASIC_INFORMATION();
            VirtualQueryEx((int)m_pProcessHandle, Address, out res, Convert.ToUInt32(Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION))));
            Log.Verbose("Champ region size:" + res.RegionSize);
            return res.RegionSize > 0? res.RegionSize : 0x3A00;
        }
        #endregion

        #region DllImports

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(int hProcess, int lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        #endregion

        #region Constants

        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public uint AllocationProtect;
            public int RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        #endregion
    }
}
