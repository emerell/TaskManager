using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager
{
    internal static class ProcessData
    {
        private static readonly Thread UpdateDbThread;
        private static readonly Thread UpdateEntriesThread;

        internal static Dictionary<int, Process> Processes { get; set; }

        static ProcessData()
        {
            Processes = new Dictionary<int, Process>();
            UpdateDbThread = new Thread(UpdateDb);
            UpdateDbThread.Start();
            UpdateEntriesThread = new Thread(UpdateEntries);
            UpdateEntriesThread.Start();
        }

        internal static void Close()
        {
            UpdateEntriesThread.Join(1500);
            UpdateDbThread.Join(4000);
        }

        private static async void UpdateDb()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (Processes)
                    {
                        List<System.Diagnostics.Process> processes = System.Diagnostics.Process.GetProcesses().ToList();
                        IEnumerable<int> keys = Processes.Keys.ToList()
                            .Where(id => processes.All(proc => proc.Id != id));
                        foreach (int key in keys)
                        {
                            Processes.Remove(key);
                        }

                        foreach (System.Diagnostics.Process proc in processes)
                        {
                            if (!Processes.ContainsKey(proc.Id))
                            {
                                try
                                {
                                    Processes[proc.Id] = new Process(proc);
                                }
                                catch (NullReferenceException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                catch (InvalidOperationException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                });
                Thread.Sleep(5000);
            }
        }

        private static async void UpdateEntries()
        {
            while (true)
            {
                await Task.Run(() =>
                {
                    lock (Processes)
                    {
                        foreach (int id in Processes.Keys.ToList())
                        {
                            System.Diagnostics.Process process;
                            try
                            {
                                process = System.Diagnostics.Process.GetProcessById(id);
                            }
                            catch (ArgumentException)
                            {
                                Processes.Remove(id);
                                continue;
                            }

                            Processes[id].CpuTaken = (int)Processes[id].CpuCounter.NextValue();
                            Processes[id].ThreadsNumber = process.Threads.Count;
                            Processes[id].RamTaken = (int)(Processes[id].RamCounter.NextValue() / 1024 / 1024);
                        }
                    }
                });
                Thread.Sleep(2000);
            }
        }
    }
}
