using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Bypass
{
	// Token: 0x02000002 RID: 2
	internal class Mari
	{
		// Token: 0x06000001 RID: 1
		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

		// Token: 0x06000002 RID: 2
		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, ref uint lpNumberOfBytesRead);

		// Token: 0x06000003 RID: 3
		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, ref uint lpNumberOfBytesWritten);

		// Token: 0x06000004 RID: 4
		[DllImport("kernel32.dll")]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		// Token: 0x06000005 RID: 5 RVA: 0x00002048 File Offset: 0x00000248
		public static string ReadCString(IntPtr hProcess, IntPtr p, uint maxSize = 128U)
		{
			uint num = 0U;
			byte[] array = new byte[maxSize];
			Mari.ReadProcessMemory(hProcess, p, array, maxSize, ref num);
			if (num == 0U)
			{
				return "";
			}
			string @string = Encoding.UTF8.GetString(array, 0, (int)num);
			int length = @string.IndexOf('\0');
			return @string.Substring(0, length);
		}

		// Token: 0x04000001 RID: 1
		public const uint SYNCHRONIZE = 1048576U;

		// Token: 0x04000002 RID: 2
		public const uint STANDARD_RIGHTS_REQUIRED = 983040U;

		// Token: 0x04000003 RID: 3
		public const uint FULL_ACCESS = 2097151U;

		// Token: 0x04000004 RID: 4
		public const uint MEM_COMMIT = 4096U;

		// Token: 0x04000005 RID: 5
		public const uint PAGE_EXECUTE = 16U;

		// Token: 0x04000006 RID: 6
		public const uint PAGE_EXECUTE_READ = 32U;

		// Token: 0x04000007 RID: 7
		public const uint PAGE_EXECUTE_READWRITE = 64U;

		// Token: 0x04000008 RID: 8
		public const uint PAGE_EXECUTE_WRITECOPY = 128U;

		// Token: 0x04000009 RID: 9
		public const uint PAGE_NOACCESS = 1U;

		// Token: 0x0400000A RID: 10
		public const uint PAGE_READONLY = 2U;

		// Token: 0x0400000B RID: 11
		public const uint PAGE_READWRITE = 4U;

		// Token: 0x0400000C RID: 12
		public const uint PAGE_WRITECOPY = 8U;
	}
}
