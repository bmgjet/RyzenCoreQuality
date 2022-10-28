using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RyzenCoreQuality
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Ryzen Core Quality";
            EventLog[] remoteEventLogs = EventLog.GetEventLogs();
            Dictionary<int, int> Found = new Dictionary<int, int>();
            foreach (EventLog log in remoteEventLogs)
            {
                if(log.Log == "System")
                {
                    foreach (EventLogEntry log2 in log.Entries)
                    {
                        if(log2.InstanceId == 55)
                        {
                            int core = int.Parse(log2.Message.Substring(log2.Message.IndexOf("processor ") + 10, 2));
                            int rank = int.Parse(log2.Message.Substring(log2.Message.IndexOf("Maximum performance percentage: ") + 32, 3));
                            if (!Found.ContainsKey(core)){Found.Add(core, rank);}
                        }
                    }
                    break;
                }
            }
            int count = 0; 
            foreach(KeyValuePair<int,int> details in Found){if (details.Key % 2 == 0){Console.Write("Core: " + count++ + " Rank: " + details.Value + Environment.NewLine);}}
            Console.ReadLine();
        }
    }
}
