using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Bypass
{
    internal class Program
    {
        private static bool IsThreadKilled;

        private static void CheckAndSuspendSingleThread(uint threadId, IntPtr modBase, IntPtr loopCodePtr, IntPtr startAddrToKill)
        {
            IntPtr hThread = Toolhelp32.OpenThread(88U, false, threadId);

            if (Toolhelp32.GetThreadStartAddress(hThread) != startAddrToKill)
            {
                return;
            }

            while (true)
            {
                byte[] context = new byte[1232];
                BitConverter.GetBytes(1048577).CopyTo(context, 48);
                Toolhelp32.GetThreadContext(hThread, context);
                BitConverter.GetBytes(loopCodePtr.ToInt64()).CopyTo(context, 248);
                BitConverter.GetBytes(1048577).CopyTo(context, 48);
                Toolhelp32.SetThreadContext(hThread, context);
                Console.WriteLine("Killed Thread with the following ID: " + threadId);
                IsThreadKilled = true;
            }
        }

        private static bool KillThreadIn(string processExeName, string moduleName, long startAddressRVA, out bool gameNotFound)
        {
            gameNotFound = false;
            IsThreadKilled = false;

            uint processPid = Toolhelp32.GetProcessPid(processExeName);

            if (processPid == 4294967295U) //hex: 0xFFFFFFFF
            {
                Console.WriteLine("Couldn't find process! Please make sure the game is open before using this tool.");
                gameNotFound = true;
                return false;
            }

            IntPtr hProcess = Mari.OpenProcess(2097151U, false, processPid);
            if (hProcess == IntPtr.Zero)
            {
                Console.WriteLine("Failed to open process. Please ensure the tool is run with Administrator privileges.");
                return false;
            }

            IntPtr moduleBase = Toolhelp32.GetModuleBase(processPid, moduleName);
            IntPtr loopCodePtr = Mari.VirtualAllocEx(hProcess, IntPtr.Zero, 4U, 4096U, 64U);

            uint numWritten = 0;
            Mari.WriteProcessMemory(hProcess, loopCodePtr, new byte[] { 235, 254, 144, 144 }, 4U, ref numWritten);

            if (numWritten != 4U)
            {
                Console.WriteLine("Couldn't write JMP to " + loopCodePtr);
                return false;
            }

            uint[] allThreadIDs = Toolhelp32.GetAllThreadIDs(processPid);

            if (allThreadIDs.Length == 0)
            {
                Console.WriteLine("Failed to get Thread IDs");
                return false;
            }

            IntPtr startAddrToKill = IntPtr.Add(moduleBase, (int)startAddressRVA);

            foreach (uint threadId in allThreadIDs)
            {
                CheckAndSuspendSingleThread(threadId, moduleBase, loopCodePtr, startAddrToKill);
            }

            return IsThreadKilled;
        }

        private static void Main(string[] args)
        {
            Console.Title = "Cheat Engine Bypass | UC Version";
            Console.ForegroundColor = ConsoleColor.Cyan;

            bool restarting = true;
            while (restarting)
            {
                try
                {
                    List<GameList> games = new List<GameList>();
                    
                    using (StreamReader reader = new StreamReader("Threads.txt"))
                    {
                        int gameId = 0;

                        while (!reader.EndOfStream)
                        {
                            string processName = reader.ReadLine().Trim();
                            Console.WriteLine($"Game ID={gameId++}:");
                            Console.WriteLine($"Process Name  = \"{processName}\"");

                            string addressLine = reader.ReadLine().Trim();
                            string[] parts = addressLine.Split('+');

                            if (parts.Length != 2)
                            {
                                Console.WriteLine("Threads.txt is corrupted, expected an address of format \"example.exe+0xDEADBEEF\"");
                                break;
                            }

                            string moduleName = parts[0];
                            long startRVA = Convert.ToInt64(parts[1], 16);

                            Console.WriteLine($"Start Address = \"{moduleName}+0x{startRVA:X}\"");
                            games.Add(new GameList { processname = processName, modname = moduleName, startRVA = startRVA });
                        }
                    }

                    int selectedGameId = 0;

                    while (games.Count > 1)
                    {
                        Console.WriteLine("Which Game do you want to use?");
                        Console.Write("Enter the ID: ");

                        if (int.TryParse(Console.ReadLine(), out selectedGameId) && selectedGameId >= 0 && selectedGameId < games.Count)
                        {
                            break;
                        }

                        Console.WriteLine("Invalid ID!");
                    }

                    bool gameNotFound;
                    if (KillThreadIn(games[selectedGameId].processname, games[selectedGameId].modname, games[selectedGameId].startRVA, out gameNotFound))
                    {
                        Console.WriteLine("Success");
                    }
                    else
                    {
                        if (!gameNotFound)
                        {
                            Console.WriteLine("Something went wrong. Ensure that the tool is run with Administrator privileges.");
                        }
                        Console.WriteLine("Waiting 3 seconds...");
                        Thread.Sleep(3000);
                        Console.Clear();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Threads.txt could not be read:");
                    Console.WriteLine(ex.Message);

                    try
                    {
                        using (StreamWriter writer = new StreamWriter("Threads.txt"))
                        {
                            writer.WriteLine("s2_mp64_ship.exe");
                            writer.WriteLine("s2_mp64_ship.exe+0x226670");
                            writer.WriteLine("s2_sp64_ship.exe");
                            writer.WriteLine("s2_sp64_ship.exe+0x5B3D0");
                        }

                        Console.WriteLine("Default Threads.txt file created successfully | Waiting 3 seconds before restarting.");
                        Thread.Sleep(3000);
                        Console.Clear();
                    }
                    catch (Exception createEx)
                    {
                        Console.WriteLine("Failed to create default Threads.txt file:");
                        Console.WriteLine(createEx.Message);
                    }
                }


            }
        }

        private struct GameList
        {
            public long startRVA;
            public string processname;
            public string modname;
        }
    }
}
