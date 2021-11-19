using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MappingInjection
{
    class Program
    {
        public static int FindProcessIDByName(string processname)
        {
            int processpid = 0;

            Process[] processlist = Process.GetProcesses();

            foreach (Process process_instance in processlist)
            {
                // Console.WriteLine("Process: {0} ID: {1}", p.ProcessName, p.Id);
                if (process_instance.ProcessName.ToLower() == processname)
                {
                    // Console.WriteLine("Find: {0}", p.Id);
                    // System.Threading.Thread.Sleep(50000);
                    processpid = process_instance.Id;
                    return processpid;
                }
                // System.Threading.Thread.Sleep(100);
            }
            return processpid;
        }
        static void Main(string[] args)
        {
            int processpid = FindProcessIDByName("powershell");
            // Console.WriteLine(processpid);
            // System.Threading.Thread.Sleep(50000);
            if (processpid != 0)
            {
                // MappingEarlyBirdInjection.MappingEarlyBirdInject(processpid);
                MappingInjection.MappingInject(processpid);
            }
        }
    }
}
