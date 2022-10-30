using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading;

namespace RyzenCoreQuality
{
    internal class Program
    {
        public class Details
        {
            public int key;
            public int value;
            public DateTime dt;
        }
        static void Main(string[] args)
        {
            Console.Title = "Ryzen Core Quality";
            EventLog[] remoteEventLogs = EventLog.GetEventLogs();
            List<Details> Found = new List<Details>();
            foreach (EventLog log in remoteEventLogs)
            {
                if(log.Log == "System")
                {
                    foreach (EventLogEntry log2 in log.Entries)
                    {
                        if(log2.InstanceId == 55)
                        {
                            Details d = new Details();
                            d.key = int.Parse(log2.Message.Substring(log2.Message.IndexOf("processor ") + 10, 2)); ;
                            d.value = int.Parse(log2.Message.Substring(log2.Message.IndexOf("Maximum performance percentage: ") + 32, 3)); ;
                            d.dt = log2.TimeGenerated;
                            Found.Add(d);
                        }
                    }
                    break;
                }
            }
            Found.Sort((x, y) => DateTime.Compare(y.dt, x.dt));
            List<int> Reported = new List<int>();
            int core = 0;
            ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in myProcessorObject.Get()){Console.WriteLine(obj["Name"]); }
            foreach (Details details in Found)
            {
                if (!Reported.Contains(details.key))
                {
                    Reported.Add(details.key);
                    if (details.key % 2 == 0) { Console.Write("Core: " + core++ + " Rank: " + details.value + Environment.NewLine); }
                }
            }
            Console.WriteLine("How Many Core Probes (10)");
            int testlen = 10;
            try
            {
                testlen = int.Parse(Console.ReadLine());
            }
            catch { }
            int corelen = core;
            DateTime dt = DateTime.Now.AddSeconds(1);
            string cores = "11";
            core = 0;
            Console.WriteLine("Starting Core Test.");
            List<double> clocks = new List<double>();
            while (corelen > 0)
            {
                if (DateTime.Now > dt)
                {
                    clocks.Add(GetCPUInfo(Convert.ToInt32(cores, 2)));
                    if (clocks.Count > testlen)
                    {
                        Console.WriteLine("Core: " + core++ + " Passed AVG Boost: " + averageclock(clocks));
                        clocks.Clear();
                        cores += "00";
                        corelen--;
                    }
                    dt = DateTime.Now.AddSeconds(1);
                }
            }
            Console.ReadLine();
        }

        public static string averageclock(List<double> c)
        {
            double d = 0;
            foreach(double dd in c){d += dd;}
            d = d / c.Count;
            return d.ToString();
        }

        private static void InfiniteLoop()
        {
            while (true)
            {
              //Do Nothing
            }
        }
        private static double GetCPUInfo(int corenum)
        {
            foreach (ProcessThread pt in Process.GetCurrentProcess().Threads){pt.ProcessorAffinity = (IntPtr)corenum; }
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor Information", "% Processor Performance", "_Total");
            Thread loop = new Thread(() => InfiniteLoop());
            loop.Start();
            double cpuValue = cpuCounter.NextValue();
            cpuValue = cpuCounter.NextValue();
            foreach (ManagementObject obj in new ManagementObjectSearcher("SELECT *, Name FROM Win32_Processor").Get())
            {
                double value = ((Convert.ToDouble(obj["MaxClockSpeed"]) / 1000) * cpuValue / 100);
                loop.Abort();
                return value;
            }
            loop.Abort();
            return 0;
        }
    }
}
