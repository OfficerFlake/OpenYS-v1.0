using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OpenYS
{
    public static class Schedulers
    {
        public static List<Scheduler> List = new List<Scheduler>();
        public delegate void Processor(string Output);
        public static Processor Process = DummyProcessor;

        public static void Poll()
        {
            foreach (Scheduler ThisScheduler in Schedulers.List.ToArray())
            {
                ThisScheduler.Process();
            }
        }

        public static int LoadAll()
        {
            int LoadedCount = 0;
            if (Directories.DirectoryExists("./Schedulers"))
            {
                string[] Filenames = Directories.DirectoryGetFilenames("./Schedulers");
                string[] SchedulerLists = Filenames.Where(x => x.ToUpperInvariant().StartsWith("SCHED") && x.ToUpperInvariant().EndsWith(".DAT")).ToArray();
                foreach (string ThisFileName in SchedulerLists)
                {
                    string[] SchedulerStrings = Files.FileReadAllLines("./Schedulers/" + ThisFileName);
                    foreach (string ThisString in SchedulerStrings)
                    {
                        if (ThisString.ToUpperInvariant().StartsWith("REM")) continue;
                        if (ThisString.ToUpperInvariant() == "") continue;
                        try
                        {
                            string temp = ThisString;
                            temp = ThisString.ReplaceAll("\t", " ");
                            string Date = temp.Split(new char[] { ' ' }, 2)[0];
                            string Message = temp.Split(new char[] { ' ' }, 2)[1];

                            //YYYYMMDD(HH:mm:ss)
                            DateTime TargetDateTime = new DateTime(0);
                            int Year = 0;
                            int Month = 0;
                            int Day = 0;
                            int Hour = 0;
                            int Minute = 0;
                            int Second = 0;
                            bool Failed = false;
                            Failed |= !Int32.TryParse(Date.Substring(0, 4), out Year);
                            Failed |= !Int32.TryParse(Date.Substring(4, 2), out Month);
                            Failed |= !Int32.TryParse(Date.Substring(6, 2), out Day);
                            Failed |= !Int32.TryParse(Date.Substring(9, 2), out Hour);
                            Failed |= !Int32.TryParse(Date.Substring(12, 2), out Minute);
                            Failed |= !Int32.TryParse(Date.Substring(15, 2), out Second);
                            string dateString = Date;
                            CultureInfo enUS = new CultureInfo("en-US");
                            if (!Failed) DateTime.TryParseExact(dateString, "yyyyMMdd(hh:mm:ss)", enUS, DateTimeStyles.None, out TargetDateTime);

                            if ((TargetDateTime - DateTime.Now).TotalMinutes > 15)
                            {
                                //Console.WriteLine(DateTime.Now.Second + " TOO FAR!");
                                //This scheduler will not happen in the next fifteen minutes,
                                //we will attempt to reload it in the next 10 minute cycle tick!
                                //We set the timer to 15 just as a safeguard to ensure the thread will happen!
                                //(RARE race condition that the scheduler lines up perfectly, and doesn't load it!)
                                continue;
                            }

                            if ((TargetDateTime - DateTime.Now).TotalMinutes < 0)
                            {
                                //Console.WriteLine(DateTime.Now.Second + " TOO EARLY!");
                                //This scheduler is already overdue, it will be ignored!
                                continue;
                            }

                            //Console.WriteLine(DateTime.Now.Second + " OK!");
                            Schedulers.List.Add(new Scheduler(TargetDateTime, Message));
                            LoadedCount++;
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                            return 0;
                        }
                    }
                }
            }
            return LoadedCount;
        }

        public static void StopAll()
        {
            lock (List)
            {
                foreach (Scheduler ThisScheduler in List.ToArray())
                {
                    ThisScheduler._Counter.Abort();
                    List.RemoveAll(x => x == ThisScheduler);
                }
            }
        }

        public static void DummyProcessor(string Input)
        {
            //Do Nothing...
        }
    }
    public class Scheduler
    {
        public DateTime TriggerTime;
        public string Command;
        public Thread _Counter;

        public Scheduler(DateTime _TriggerTime, string _Command)
        {
            TriggerTime = _TriggerTime;
            Command = _Command;
            //_Counter = Threads.Add(() => _Count(), "ScheduleThread => \"" + Command + "\"");
        }

        public bool Process()
        {
            if (DateTime.Now <= TriggerTime)
            {
                return false;
            }
            Schedulers.Process(Command);
            Schedulers.List.Remove(this);
            return true;

        }
    }
}
