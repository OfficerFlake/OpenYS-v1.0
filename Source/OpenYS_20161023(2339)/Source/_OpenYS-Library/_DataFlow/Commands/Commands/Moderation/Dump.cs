using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using OfficeOpenXml;
using System.Collections;

namespace OpenYS
{
    public static partial class Commands
    {
        public static readonly CommandDescriptor OpenYS_Command_Moderation_Dump = new CommandDescriptor
        {
            _Name = "Dump",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = true,

            _Descrption = "Dumps the entire programs data out to a debug log file.",
            _Usage = "Dump",
            _Commands = new string[] { "/Dump" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                Requirement.User_Console |
                //Requirement.User_YSFlight      |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                //Requirement.Status_Connected   |
                //Requirement.Status_Flying      |
                //Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Moderation_Dump_Method,
        };

        public static bool OpenYS_Command_Moderation_Dump_Method_Old(Client ThisClient, CommandReader Command)
        {
            Console.Write("&cDumping Data... ");
            Process ThisProcess = Process.GetCurrentProcess();
            Log.PrepareDataDump();
            Log.DataDump("//Executing Process Info");
            Log.DataDump("    " + "Total Threads:   " + ThisProcess.Threads.OfType<System.Diagnostics.ProcessThread>().Count().ToString());
            Log.DataDump("    " + "Active Threads:  " + ThisProcess.Threads.OfType<System.Diagnostics.ProcessThread>().Where(t => t.ThreadState == System.Diagnostics.ThreadState.Running).Count().ToString());
            Log.DataDump("    " + "Managed Threads: " + Threads.List.Count.ToString());

            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;

            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            ramCounter = new PerformanceCounter("Memory", "Available MBytes");

            string CPUUsage = cpuCounter.NextValue() + "%";
            string RAMUsage = (ThisProcess.WorkingSet64 / 1024 / 1024).ToString() + "MB";

            Log.DataDump("    " + "CPU Usage:       " + CPUUsage);
            Log.DataDump("    " + "RAM Usage:       " + RAMUsage);

            Log.DataDump("//Thread Info");
            lock (Threads.List)
            {
                foreach (Thread ThisThread in Threads.List.ToArray())
                {
                    Log.DataDump("    " + "Thread: " + ThisThread.Name);
                }
            }
            Console.WriteLine("&cDone.");
            return true;
        }

        public static bool OpenYS_Command_Moderation_Dump_Method(Client ThisClient, CommandReader Command)
        {
            //Prepare To Dump.
            Console.Write("&cDumping Data... ");
            Process ThisProcess = Process.GetCurrentProcess();
            string LogTime = DateTime.Now.ToOYSLongDateTime();
            string LogName = "./Logs/DumpLogs/" + LogTime + ".xlsx";
            FileInfo Filename = new FileInfo(LogName);

            //Export DumpLog Template...
            Directories.DirectoryPrepare("./Logs");
            Directories.DirectoryPrepare("./Logs/DumpLogs");

            if (!DumpLogFileTemplate(LogName))
            {
                Console.WriteLine("&cFAILED TO WRITE TEMPLATE XLSX, ABANDONED DUMP.");
                return false;
            }

            //Dumplog exported, now build a list of all classes.
            List<FieldInfo> Infos = IterateOverEverything();

            //Populate the template.
            using (ExcelPackage xlPackage = new ExcelPackage(Filename))
            {
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                worksheet.Cells[2, 1].Value = "As At " + LogTime;

                for (int i = 0; i < Infos.Count; i++)
                {
                    string Name = Infos[i].Name == null ? "" : Infos[i].Name;
                    worksheet.Cells[i + 4, 1].Value = Name;
                    string Value = Infos[i].GetValue(null) == null ? "" : Infos[i].GetValue(null).ToString();
                    worksheet.Cells[i + 4, 2].Value = Value;
                }

                //Finally...
                xlPackage.Save();
            }

            //Done.
            Console.WriteLine("&cDone: \"" + LogName + "\".");
            return true;
        }

        private class Pairing
        {
            public string Key;
            public string Val;

            public Pairing(string _Key, string _Val)
            {
                Key = _Key;
                Val = _Val;
            }
        }

        private static bool DumpLogFileTemplate(string Output)
        {
            try
            {
                var resourceName = "OpenYS.DumpLogTemplate.xlsx";
                string[] Names = System.Reflection.Assembly.GetAssembly(typeof(Log)).GetManifestResourceNames();

                using (Stream stream = System.Reflection.Assembly.GetAssembly(typeof(Log)).GetManifestResourceStream(resourceName))
                {
                    using (System.IO.FileStream fileStream = new System.IO.FileStream(Output, System.IO.FileMode.Create))
                    {
                        for (int i = 0; i < stream.Length; i++)
                        {
                            fileStream.WriteByte((byte)stream.ReadByte());
                        }
                        fileStream.Close();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static List<FieldInfo> IterateOverEverything()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x=>x.FullName.ToUpperInvariant().StartsWith("OPENYS")).ToArray();
            List<FieldInfo> FieldInfos = new List<FieldInfo>();
            foreach (Assembly ThisAssembly in assemblies)
            {
                FieldInfo[][] Temp = ThisAssembly.GetTypes().Select(y => y.GetFields(BindingFlags.Public | BindingFlags.Static)).ToArray();
                foreach (FieldInfo[] TheseFields in Temp)
                {
                    foreach (FieldInfo ThisField in TheseFields)
                    {
                        FieldInfos.Add(ThisField);
                    }
                }
            }
            return FieldInfos;
        }

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }
    }
}