using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bypass
{
    // Token: 0x02000003 RID: 3
    internal class Toolhelp32
    {
        // Token: 0x06000007 RID: 7
        [DllImport("kernel32.dll")]
        public static extern bool GetThreadContext(IntPtr hThread, byte[] lpContext);

        // Token: 0x06000008 RID: 8
        [DllImport("kernel32.dll")]
        public static extern bool SetThreadContext(IntPtr hThread, byte[] lpContext);

        // Token: 0x06000009 RID: 9
        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        // Token: 0x0600000A RID: 10
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hSnapshot);

        // Token: 0x0600000B RID: 11
        [DllImport("kernel32.dll")]
        public static extern bool Heap32ListFirst(IntPtr hSnapshot, ref Toolhelp32.HEAPLIST32 lphl);

        // Token: 0x0600000C RID: 12
        [DllImport("kernel32.dll")]
        public static extern bool Heap32ListNext(IntPtr hSnapshot, ref Toolhelp32.HEAPLIST32 lphl);

        // Token: 0x0600000D RID: 13
        [DllImport("kernel32.dll")]
        public static extern bool Heap32First(IntPtr hSnapshot, ref Toolhelp32.HEAPENTRY32 lphe, uint th32ProcessID, uint th32HeapID);

        // Token: 0x0600000E RID: 14
        [DllImport("kernel32.dll")]
        public static extern bool Heap32Next(IntPtr hSnapshot, ref Toolhelp32.HEAPENTRY32 lphe);

        // Token: 0x0600000F RID: 15
        [DllImport("kernel32.dll")]
        public static extern bool Toolhelp32ReadProcessMemory(uint th32ProcessID, IntPtr lpBaseAddress, IntPtr lpBuffer, uint cbRead, IntPtr lpNumberOfBytesRead);

        // Token: 0x06000010 RID: 16
        [DllImport("kernel32.dll")]
        public static extern bool Process32FirstW(IntPtr hSnapshot, ref Toolhelp32.PROCESSENTRY32W lppe);

        // Token: 0x06000011 RID: 17
        [DllImport("kernel32.dll")]
        public static extern bool Process32NextW(IntPtr hSnapshot, ref Toolhelp32.PROCESSENTRY32W lppe);

        // Token: 0x06000012 RID: 18
        [DllImport("kernel32.dll")]
        public static extern bool Process32First(IntPtr hSnapshot, ref Toolhelp32.PROCESSENTRY32 lppe);

        // Token: 0x06000013 RID: 19
        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(IntPtr hSnapshot, ref Toolhelp32.PROCESSENTRY32 lppe);

        // Token: 0x06000014 RID: 20
        [DllImport("kernel32.dll")]
        public static extern bool Thread32First(IntPtr hSnapshot, ref Toolhelp32.THREADENTRY32 lpte);

        // Token: 0x06000015 RID: 21
        [DllImport("kernel32.dll")]
        public static extern bool Thread32Next(IntPtr hSnapshot, ref Toolhelp32.THREADENTRY32 lpte);

        // Token: 0x06000016 RID: 22
        [DllImport("kernel32.dll")]
        public static extern bool Module32FirstW(IntPtr hSnapshot, ref Toolhelp32.MODULEENTRY32W lpme);

        // Token: 0x06000017 RID: 23
        [DllImport("kernel32.dll")]
        public static extern bool Module32NextW(IntPtr hSnapshot, ref Toolhelp32.MODULEENTRY32W lpme);

        // Token: 0x06000018 RID: 24
        [DllImport("kernel32.dll")]
        public static extern bool Module32First(IntPtr hSnapshot, ref Toolhelp32.MODULEENTRY32W lpme);

        // Token: 0x06000019 RID: 25
        [DllImport("kernel32.dll")]
        public static extern bool Module32Next(IntPtr hSnapshot, ref Toolhelp32.MODULEENTRY32W lpme);

        // Token: 0x0600001A RID: 26
        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        // Token: 0x0600001B RID: 27
        [DllImport("ntdll.dll")]
        public static extern uint NtQueryInformationThread(IntPtr hThread, uint info_class, byte[] buffer, uint buffer_size, ref uint returnLength);

        // Token: 0x0600001C RID: 28
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, ref IntPtr lpTargetHandle, uint dwDesiredAccess, bool bInheritHandle, uint dwOptions);

        // Token: 0x0600001D RID: 29
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        // Token: 0x0600001E RID: 30
        [DllImport("kernel32.dll")]
        public static extern int SuspendThread(IntPtr hThread);

        // Token: 0x0600001F RID: 31
        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);

        // Token: 0x06000020 RID: 32 RVA: 0x00002098 File Offset: 0x00000298
        public static uint GetProcessPid(string name)
        {
            IntPtr intPtr = Toolhelp32.CreateToolhelp32Snapshot(2U, 0U);
            uint num = uint.MaxValue;
            if (intPtr == new IntPtr(-1))
            {
                return num;
            }
            Toolhelp32.PROCESSENTRY32 processentry = default(Toolhelp32.PROCESSENTRY32);
            processentry.dwSize = (uint)Marshal.SizeOf<Toolhelp32.PROCESSENTRY32>(processentry);
            for (bool flag = Toolhelp32.Process32First(intPtr, ref processentry); flag && num == 4294967295U; flag = Toolhelp32.Process32Next(intPtr, ref processentry))
            {
                if (name.Equals(processentry.szExeFile))
                {
                    num = processentry.th32ProcessID;
                }
            }
            Toolhelp32.CloseHandle(intPtr);
            return num;
        }

        // Token: 0x06000021 RID: 33 RVA: 0x0000210C File Offset: 0x0000030C
        public static IntPtr GetModuleBase(uint dwProcessId, string name)
        {
            IntPtr intPtr = Toolhelp32.CreateToolhelp32Snapshot(8U, dwProcessId);
            IntPtr hModule = new IntPtr(-1);
            if (intPtr == new IntPtr(-1))
            {
                return hModule;
            }
            Toolhelp32.MODULEENTRY32W moduleentry32W = default(Toolhelp32.MODULEENTRY32W);
            moduleentry32W.dwSize = (uint)Marshal.SizeOf<Toolhelp32.MODULEENTRY32W>(moduleentry32W);
            for (bool flag = Toolhelp32.Module32First(intPtr, ref moduleentry32W); flag && hModule == new IntPtr(-1); flag = Toolhelp32.Module32Next(intPtr, ref moduleentry32W))
            {
                if (name.Equals(moduleentry32W.szModule))
                {
                    hModule = moduleentry32W.hModule;
                }
            }
            Toolhelp32.GetLastError();
            Toolhelp32.CloseHandle(intPtr);
            return hModule;
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002198 File Offset: 0x00000398
        public static IntPtr GetThreadStartAddress(IntPtr hThread)
        {
            IntPtr intPtr = new IntPtr(0);
            IntPtr result = new IntPtr(0L);
            IntPtr handle = Process.GetCurrentProcess().Handle;
            if (!Toolhelp32.DuplicateHandle(handle, hThread, handle, ref intPtr, 64U, false, 0U))
            {
                Console.WriteLine("Failed to duplicate thread handle, ignoring this thread");
                return result;
            }
            uint num = 0U;
            byte[] array = new byte[8];
            uint num2 = Toolhelp32.NtQueryInformationThread(intPtr, 9U, array, 8U, ref num);
            if (num != 8U)
            {
                Console.WriteLine("Failed to read all of ThreadStartAddress, only read " + num.ToString());
                return result;
            }
            Toolhelp32.CloseHandle(intPtr);
            if (num2 == 0U)
            {
                result = new IntPtr(BitConverter.ToInt64(array, 0));
            }
            return result;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00002230 File Offset: 0x00000430
        public static uint[] GetAllThreadIDs(uint dwProcessId)
        {
            IntPtr intPtr = Toolhelp32.CreateToolhelp32Snapshot(4U, dwProcessId);
            if (intPtr == new IntPtr(-1))
            {
                return null;
            }
            Toolhelp32.THREADENTRY32 threadentry = default(Toolhelp32.THREADENTRY32);
            threadentry.dwSize = (uint)Marshal.SizeOf<Toolhelp32.THREADENTRY32>(threadentry);
            List<uint> list = new List<uint>();
            bool flag = Toolhelp32.Thread32First(intPtr, ref threadentry);
            while (flag)
            {
                if (threadentry.th32OwnerProcessID == dwProcessId)
                {
                    list.Add(threadentry.th32ThreadID);
                }
                flag = Toolhelp32.Thread32Next(intPtr, ref threadentry);
            }
            Toolhelp32.CloseHandle(intPtr);
            return list.ToArray();
        }

        // Token: 0x0400000D RID: 13
        public const uint TH32CS_SNAPHEAPLIST = 1U;

        // Token: 0x0400000E RID: 14
        public const uint TH32CS_SNAPPROCESS = 2U;

        // Token: 0x0400000F RID: 15
        public const uint TH32CS_SNAPTHREAD = 4U;

        // Token: 0x04000010 RID: 16
        public const uint TH32CS_SNAPMODULE = 8U;

        // Token: 0x04000011 RID: 17
        public const uint TH32CS_SNAPMODULE32 = 16U;

        // Token: 0x04000012 RID: 18
        public const uint TH32CS_SNAPALL = 15U;

        // Token: 0x04000013 RID: 19
        public const uint TH32CS_INHERIT = 2147483648U;

        // Token: 0x04000014 RID: 20
        public const int INVALID_HANDLE_VALUE = -1;

        // Token: 0x04000015 RID: 21
        public const uint HF32_DEFAULT = 1U;

        // Token: 0x04000016 RID: 22
        public const uint HF32_SHARED = 2U;

        // Token: 0x04000017 RID: 23
        public const uint LF32_FIXED = 1U;

        // Token: 0x04000018 RID: 24
        public const uint LF32_FREE = 2U;

        // Token: 0x04000019 RID: 25
        public const uint LF32_MOVEABLE = 4U;

        // Token: 0x0400001A RID: 26
        public const uint THREAD_QUERY_INFORMATION = 64U;

        // Token: 0x0400001B RID: 27
        public const uint THREAD_GET_CONTEXT = 8U;

        // Token: 0x0400001C RID: 28
        public const uint THREAD_SET_CONTEXT = 16U;

        // Token: 0x0400001D RID: 29
        public const uint THREAD_SUSPEND_RESUME = 2U;

        // Token: 0x02000006 RID: 6
        public struct HEAPLIST32
        {
            // Token: 0x04000020 RID: 32
            public uint dwSize;

            // Token: 0x04000021 RID: 33
            public uint th32ProcessID;

            // Token: 0x04000022 RID: 34
            public uint th32HeapID;

            // Token: 0x04000023 RID: 35
            public uint dwFlags;
        }

        // Token: 0x02000007 RID: 7
        public struct HEAPENTRY32
        {
            // Token: 0x04000024 RID: 36
            public uint dwSize;

            // Token: 0x04000025 RID: 37
            public IntPtr hHandle;

            // Token: 0x04000026 RID: 38
            public uint dwAddress;

            // Token: 0x04000027 RID: 39
            public uint dwBlockSize;

            // Token: 0x04000028 RID: 40
            public uint dwFlags;

            // Token: 0x04000029 RID: 41
            public uint dwLockCount;

            // Token: 0x0400002A RID: 42
            public uint dwResvd;

            // Token: 0x0400002B RID: 43
            public uint th32ProcessID;

            // Token: 0x0400002C RID: 44
            public uint th32HeapID;
        }

        // Token: 0x02000008 RID: 8
        public struct PROCESSENTRY32W
        {
            // Token: 0x0400002D RID: 45
            public uint dwSize;

            // Token: 0x0400002E RID: 46
            public uint cntUsage;

            // Token: 0x0400002F RID: 47
            public uint th32ProcessID;

            // Token: 0x04000030 RID: 48
            public UIntPtr th32DefaultHeapID;

            // Token: 0x04000031 RID: 49
            public uint th32ModuleID;

            // Token: 0x04000032 RID: 50
            public uint cntThreads;

            // Token: 0x04000033 RID: 51
            public uint th32ParentProcessID;

            // Token: 0x04000034 RID: 52
            public int pcPriClassBase;

            // Token: 0x04000035 RID: 53
            public uint dwFlags;

            // Token: 0x04000036 RID: 54
            public string szExeFile;
        }

        // Token: 0x02000009 RID: 9
        public struct PROCESSENTRY32
        {

            // Token: 0x04000038 RID: 56
            internal uint dwSize;

            // Token: 0x04000039 RID: 57
            internal uint cntUsage;

            // Token: 0x0400003A RID: 58
            internal uint th32ProcessID;

            // Token: 0x0400003B RID: 59
            internal IntPtr th32DefaultHeapID;

            // Token: 0x0400003C RID: 60
            internal uint th32ModuleID;

            // Token: 0x0400003D RID: 61
            internal uint cntThreads;

            // Token: 0x0400003E RID: 62
            internal uint th32ParentProcessID;

            // Token: 0x0400003F RID: 63
            internal int pcPriClassBase;

            // Token: 0x04000040 RID: 64
            internal uint dwFlags;

            // Token: 0x04000041 RID: 65
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExeFile;
        }

        // Token: 0x0200000A RID: 10
        public struct THREADENTRY32
        {
            // Token: 0x04000042 RID: 66
            public uint dwSize;

            // Token: 0x04000043 RID: 67
            public uint cntUsage;

            // Token: 0x04000044 RID: 68
            public uint th32ThreadID;

            // Token: 0x04000045 RID: 69
            public uint th32OwnerProcessID;

            // Token: 0x04000046 RID: 70
            public int tpBasePri;

            // Token: 0x04000047 RID: 71
            public int tpDeltaPri;

            // Token: 0x04000048 RID: 72
            public uint dwFlags;
        }

        // Token: 0x0200000B RID: 11
        public struct MODULEENTRY32W
        {
            // Token: 0x04000049 RID: 73
            public uint dwSize;

            // Token: 0x0400004A RID: 74
            public uint th32ModuleID;

            // Token: 0x0400004B RID: 75
            public uint th32ProcessID;

            // Token: 0x0400004C RID: 76
            public uint GlblcntUsage;

            // Token: 0x0400004D RID: 77
            public uint ProccntUsage;

            // Token: 0x0400004E RID: 78
            public IntPtr modBaseAddr;

            // Token: 0x0400004F RID: 79
            public uint modBaseSize;

            // Token: 0x04000050 RID: 80
            public IntPtr hModule;

            // Token: 0x04000051 RID: 81
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szModule;

            // Token: 0x04000052 RID: 82
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExePath;
        }

        // Token: 0x0200000C RID: 12
        public struct MODULEENTRY32
        {
            // Token: 0x04000053 RID: 83
            public uint dwSize;

            // Token: 0x04000054 RID: 84
            public uint th32ModuleID;

            // Token: 0x04000055 RID: 85
            public uint th32ProcessID;

            // Token: 0x04000056 RID: 86
            public uint GlblcntUsage;

            // Token: 0x04000057 RID: 87
            public uint ProccntUsage;

            // Token: 0x04000058 RID: 88
            public IntPtr modBaseAddr;

            // Token: 0x04000059 RID: 89
            public uint modBaseSize;

            // Token: 0x0400005A RID: 90
            public IntPtr hModule;

            // Token: 0x0400005B RID: 91
            public string szModule;

            // Token: 0x0400005C RID: 92
            public string szExePath;
        }
    }
}
