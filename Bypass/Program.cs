using System;
using System.Collections.Generic;
using System.IO;

namespace Bypass
{
	// Token: 0x02000004 RID: 4
	internal class Program
	{
		// Token: 0x06000025 RID: 37 RVA: 0x000022B4 File Offset: 0x000004B4
		private static void CheckAndSuspendSingleThread(uint threadId, IntPtr modBase, IntPtr loop_code_p, IntPtr startAddr_toKill)
		{
			IntPtr hThread = Toolhelp32.OpenThread(88U, false, threadId);
			if (!(Toolhelp32.GetThreadStartAddress(hThread) == startAddr_toKill))
			{
				return;
			}
			for (;;)
			{
				byte[] array = new byte[1232];
				BitConverter.GetBytes(1048577).CopyTo(array, 48);
				Toolhelp32.GetThreadContext(hThread, array);
				BitConverter.GetBytes(loop_code_p.ToInt64()).CopyTo(array, 248);
				BitConverter.GetBytes(1048577).CopyTo(array, 48);
				Toolhelp32.SetThreadContext(hThread, array);
				Console.WriteLine("Killed Thread with ID: " + threadId.ToString());
				Program.IsThreadKilled = true;
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002350 File Offset: 0x00000550
		private static bool KillThreadIn(string processExeName, string moduleName, long startAddressRVA)
		{
			Program.IsThreadKilled = false;
			uint processPid = Toolhelp32.GetProcessPid(processExeName);
			if (processPid == 4294967295U)
			{
				Console.WriteLine("Couldn't find process!");
				return false;
			}
			IntPtr hProcess = Mari.OpenProcess(2097151U, false, processPid);
			IntPtr moduleBase = Toolhelp32.GetModuleBase(processPid, moduleName);
			IntPtr intPtr = Mari.VirtualAllocEx(hProcess, new IntPtr(0L), 4U, 4096U, 64U);
			uint num = 0U;
			Mari.WriteProcessMemory(hProcess, intPtr, new byte[]
			{
				235,
				254,
				144,
				144
			}, 4U, ref num);
			if (num != 4U)
			{
				Console.WriteLine("Couldn't write JMP to " + intPtr.ToString());
				return false;
			}
			uint[] allThreadIDs = Toolhelp32.GetAllThreadIDs(processPid);
			if (allThreadIDs.Length == 0)
			{
				Console.WriteLine("Failed to get Thread ID's");
				return false;
			}
			IntPtr startAddr_toKill = new IntPtr((long)moduleBase + startAddressRVA);
			uint[] array = allThreadIDs;
			for (int i = 0; i < array.Length; i++)
			{
				Program.CheckAndSuspendSingleThread(array[i], moduleBase, intPtr, startAddr_toKill);
			}
			return Program.IsThreadKilled;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002430 File Offset: 0x00000630
		private static void Main(string[] args)
		{
			Console.Title = "Cheat Engine Bypass";
			Console.ForegroundColor = ConsoleColor.Yellow;
			try
			{
				List<Program.GameList> list = new List<Program.GameList>();
				using (StreamReader streamReader = new StreamReader("Threads.txt"))
				{
					int num = 0;
					while (!streamReader.EndOfStream)
					{
						string text = streamReader.ReadLine();
						Program.GameList gameList = default(Program.GameList);
						Console.WriteLine("Game ID=" + num++.ToString() + ":");
						gameList.processname = text.Trim();
						Console.WriteLine("Process Name  = \"" + gameList.processname + "\"");
						text = streamReader.ReadLine();
						text = text.Trim();
						string[] array = text.Split(new char[]
						{
							'+'
						});
						if (array.Length != 2)
						{
							Console.WriteLine("Threads.txt is corrupted expected an Address of format \"test.exe+0xDEADBEEF\"");
							break;
						}
						gameList.modname = array[0];
						gameList.startRVA = (long)Convert.ToUInt64(array[1], 16);
						Console.WriteLine(string.Concat(new string[]
						{
							"Start Address = \"",
							gameList.modname,
							"+0x",
							Convert.ToString(gameList.startRVA, 16),
							"\"\n"
						}));
						list.Add(gameList);
					}
				}
				int num2 = 0;
				while (list.Count > 1)
				{
					Console.WriteLine("Which Game do you want to use?");
					Console.Write("Enter the ID: ");
					if (int.TryParse(Console.ReadLine(), out num2))
					{
						if (num2 < list.Count && num2 >= 0)
						{
							break;
						}
						Console.WriteLine("Index out of bounds!");
					}
					else
					{
						Console.WriteLine("You have to enter a number!");
					}
				}
				if (Program.KillThreadIn(list[num2].processname, list[num2].modname, list[num2].startRVA))
				{
					Console.WriteLine("Success");
				}
				else
				{
					Console.WriteLine("Something went wrong, try running as Admin!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Threads.txt could not be Read:");
				Console.WriteLine(ex.Message);
			}
			Console.ReadKey();
		}

		// Token: 0x0400001E RID: 30
		private static bool IsThreadKilled;

		// Token: 0x0200000D RID: 13
		private struct GameList
		{
			// Token: 0x0400005D RID: 93
			public long startRVA;

			// Token: 0x0400005E RID: 94
			public string processname;

			// Token: 0x0400005F RID: 95
			public string modname;
		}
	}
}
