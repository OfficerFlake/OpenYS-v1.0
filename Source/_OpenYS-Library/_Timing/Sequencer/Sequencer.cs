using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OpenYS
{
    public static class Sequencers
    {
        public static List<Sequencer> List = new List<Sequencer>();
        public delegate void Processor(string Output);
        public static Processor Process = DummyProcessor;

        public static int LoadAll()
        {
            int LoadedCount = 0;
            if (Directories.DirectoryExists("./Sequencers"))
            {
                //Console.WriteLine("!");
                string[] Filenames = Directories.DirectoryGetFilenames("./Sequencers");
                string[] SequencerLists = Filenames.Where(x => x.ToUpperInvariant().StartsWith("SEQ") && x.ToUpperInvariant().EndsWith(".DAT")).ToArray();
                foreach (string ThisFileName in SequencerLists)
                {
                    string[] SequencerStrings = Files.FileReadAllLines("./Sequencers/" + ThisFileName);
                    string SequencerTitle = ThisFileName;
                    List<string> SequencerCommands = new List<string>();
                    foreach (string ThisString in SequencerStrings)
                    {
                        if (ThisString.ToUpperInvariant().StartsWith("REM")) continue;
                        if (ThisString.ToUpperInvariant() == "") continue;
                        try
                        {
                            SequencerCommands.Add(ThisString);
                        }
                        catch (Exception e)
                        {
                            Log.Error(e);
                        }
                    }
                    Sequencers.List.Add(new Sequencer(SequencerTitle, SequencerCommands));
                    LoadedCount++;
                }
            }
            return LoadedCount;
        }

        public static void StopAll()
        {
            lock (List)
            {
                foreach (Sequencer ThisSequencer in List.ToArray())
                {
                    ThisSequencer.Stop();
                }
            }
        }

        public static void Start()
        {
            lock (List)
            {
                foreach (Sequencer ThisSequencer in List.ToArray())
                {
                    ThisSequencer.Start();
                }
            }
        }

        public static void Restart()
        {
            lock (List)
            {
                foreach (Sequencer ThisSequencer in List.ToArray())
                {
                    ThisSequencer.Restart();
                }
            }
        }

        public static void Poll()
        {
            lock (List)
            {
                foreach (Sequencer ThisSequencer in List.ToArray())
                {
                    ThisSequencer.Poll();
                }
            }
        }

        public static void DummyProcessor(string Input)
        {
            //Do Nothing...
        }
    }

    public class OLD_Sequencer
    {
        public List<String> _Commands;
        public Thread _Counter;

        public OLD_Sequencer(string Title, List<String> _LoadCommands)
        {
            _Commands = _LoadCommands;
            _Counter = Threads.Add(() => _Count(), "SequencerThread => \"" + Title + "\"");
        }

        public bool _Count()
        {
            Restart:
            foreach (string ThisCommand in _Commands)
            {
                if (ThisCommand.ToUpperInvariant() == "/RESTARTSEQUENCE" |
                    ThisCommand.ToUpperInvariant() == "/RESTARTSEQUENCER" |
                    ThisCommand.ToUpperInvariant() == "/SEQUENCE.RESTART" |
                    ThisCommand.ToUpperInvariant() == "/SEQUENCER.RESTART")
                {
                    goto Restart;
                }
                if (ThisCommand.ToUpperInvariant() == "/STOPSEQUENCE" |
                    ThisCommand.ToUpperInvariant() == "/STOPSEQUENCER" |
                    ThisCommand.ToUpperInvariant() == "/SEQUENCE.STOP" |
                    ThisCommand.ToUpperInvariant() == "/SEQUENCER.STOP")
                {
                    break;
                }
                Sequencers.Process(ThisCommand);
            }
            return true;
        }
    }

    public class Sequencer
    {
        public List<String> Commands = new List<String>();
        public DateTime SuspendUntil = DateTime.Now;
        public int Position = 0;
        public string Title = "Sequencer";

        public Sequencer(string _Title, List<String> _Commands)
        {
            Title = _Title;
            Commands = _Commands;
        }

        public void Poll()
        {
            if (SuspendUntil.Ticks == 0 | Position >= Commands.Count | DateTime.Now < SuspendUntil)
            {
                return;
            }
            if (Commands[Position].ToUpperInvariant() == "/RESTARTSEQUENCE" |
                Commands[Position].ToUpperInvariant() == "/RESTARTSEQUENCER" |
                Commands[Position].ToUpperInvariant() == "/SEQUENCE.RESTART" |
                Commands[Position].ToUpperInvariant() == "/SEQUENCER.RESTART")
            {
                Position = 0;
            }
            if (Commands[Position].ToUpperInvariant() == "/STOPSEQUENCE" |
                Commands[Position].ToUpperInvariant() == "/STOPSEQUENCER" |
                Commands[Position].ToUpperInvariant() == "/SEQUENCE.STOP" |
                Commands[Position].ToUpperInvariant() == "/SEQUENCER.STOP")
            {
                SuspendUntil = new DateTime(0);
                return;
            }
            if (Commands[Position].ToUpperInvariant().StartsWith("/SLEEP"))
            {
                int SleepTimeMicroSeconds = 0;
                Int32.TryParse(Commands[Position].Replace('\t', ' ').Split(' ')[1], out SleepTimeMicroSeconds);
                SuspendUntil = DateTime.Now + new TimeSpan(0, 0, 0, 0, SleepTimeMicroSeconds);
                Position++;
                return;
            }
            Sequencers.Process(Commands[Position]);
            Position++;
        }

        public void Stop()
        {
            SuspendUntil = new DateTime(0);
        }

        public void Start()
        {
            SuspendUntil = DateTime.Now;
        }

        public void Restart()
        {
            Position = 0;
            Start();
        }
    }
}