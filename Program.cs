using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

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
            foreach (ManagementObject obj in myProcessorObject.Get()){Console.WriteLine(obj["Name"]);}
            foreach (Details details in Found)
            {
                if(!Reported.Contains(details.key))
                {
                    Reported.Add(details.key);
                    if (details.key % 2 == 0) { Console.Write("Core: " + core++ + " Rank: " + details.value + Environment.NewLine); }
                }
            }
            Console.ReadLine();
        }
    }
}
