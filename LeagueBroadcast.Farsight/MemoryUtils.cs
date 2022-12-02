using LeagueBroadcast.Utils.Log;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace LeagueBroadcast.Farsight
{
    public class MemoryUtils
    {
        public static Process? m_Process;
        public static IntPtr m_pProcessHandle;

        public static int m_iNumberOfBytesRead = 0;
        public static int m_iNumberOfBytesWritten = 0;

        public static int m_baseAddress = 0;

        public static bool IsConnected => m_pProcessHandle != (IntPtr)0;


        public static bool Initialize(Process p)
        {
            m_Process = p;
            m_pProcessHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, m_Process.Id); // Sets Our ProcessHandle
            m_Process.Exited += (s, e) => { m_Process = null; m_pProcessHandle = (IntPtr)0; m_iNumberOfBytesRead = 0; m_iNumberOfBytesWritten = 0; };

            m_baseAddress = m_Process!.MainModule!.BaseAddress.ToInt32();
            "Attached to League Process".Info();

            return true;
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

            if(!ReadProcessMemory((int)m_pProcessHandle, Address, buffer, size, ref m_iNumberOfBytesRead))
            {
                //throw new MemoryReadException("ReadProcessMemory failed");
            }

            return buffer;
        }

        public static byte[] ReadMemory(int Address, int size, ref int numberOfBytesRead)
        {
            var buffer = new byte[size];

            ReadProcessMemory((int)m_pProcessHandle, Address, buffer, size, ref numberOfBytesRead);

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
                return ((T?)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T))) ?? throw new NullReferenceException();
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

        public static void GetSysInfo(out SYSTEM_INFO sysInfo)
        {
            GetSystemInfo(out sysInfo);
        }


        public static int VirtualQueryLeague(IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength)
        {
            return VirtualQueryEx(m_pProcessHandle, lpAddress, out lpBuffer, dwLength);
        }
        #endregion

        #region DllImports

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll")]
        public static extern uint GetProcessId([In] IntPtr Process);


        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool Heap32ListFirst(int snapshot, out HEAPLIST32 lphl);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public unsafe static extern bool Heap32First([In, Out] ref HEAPENTRY32 lphl, [In] uint th32ProcessID, [In] UIntPtr th32HeapID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool Heap32ListNext(int snapshot, out HEAPLIST32 lphl);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public unsafe static extern bool Heap32Next([In, Out] ref HEAPENTRY32 lphe);
        #endregion

        #region Constants


        public const int MEM_COMMIT = 0x00001000;
        public const int PAGE_READWRITE = 0x04;
        public const int PROCESS_WM_READ = 0x0010;


        const int PROCESS_VM_OPERATION = 0x0008;
        const int PROCESS_VM_READ = 0x0010;
        const int PROCESS_VM_WRITE = 0x0020;
        public const int PROCESS_QUERY_INFORMATION = 0x0400;

        
        public const int INVALID_HANDLE_VALUE = -1;

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;
            public IntPtr maximumApplicationAddress;
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct HEAPLIST32
        {
            public uint dwSize;
            public int th32ProcessID;
            public uint AllocationProtect;
            public ulong th32HeapID;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HEAPENTRY32
        {
            public uint dwSize;
            public uint hHandle;
            public ulong dwAddress;
            public uint dwBlockSize;
            public uint dwFlags;
            public uint dwLockCount;
            public uint dwResvd;
            public uint th32ProcessID;
            public ulong th32HeapID;
        }

        [Flags]
        public enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        #endregion
    }

    public class MemoryReadException : Exception
    {
        public MemoryReadException()
        {
        }

        public MemoryReadException(string message)
            : base(message)
        {
        }

        public MemoryReadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
