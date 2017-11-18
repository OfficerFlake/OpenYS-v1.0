using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace OpenYS
{
    public static class Console
    {
        #region Color Functions
        public static void ConsoleHandler(string ThisMessage)
        {
            if (ThisMessage.StartsWith("\r"))
            {
                int Top = System.Console.CursorTop;
                if (Top != 0)
                {
                    System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                    ThisMessage = ThisMessage.Remove(0, 1); // "\r" is ONE character, not two.
                }
            }
            //System.Console.WriteLine("1");
            foreach (string msgToAppend in Converter(ThisMessage))
            {
                switch (msgToAppend.ToLowerInvariant()[1])
                {
                    case '0':
                        System.Console.ForegroundColor = ConsoleColor.Black;
                        System.Console.BackgroundColor = ConsoleColor.White;
                        break;
                    case '1':
                        System.Console.ForegroundColor = ConsoleColor.DarkBlue;
                        break;
                    case '2':
                        System.Console.ForegroundColor = ConsoleColor.DarkGreen;
                        break;
                    case '3':
                        System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case 'y':
                    case '4':
                        System.Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                    case 'm':
                    case 'i':
                    case '5':
                        System.Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        break;
                    case '6':
                        System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case '7':
                        System.Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case '8':
                        System.Console.ForegroundColor = ConsoleColor.DarkGray;
                        break;
                    case '9':
                        System.Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case 'h':
                    case 'a':
                        System.Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case 'p':
                    case 'b':
                        System.Console.ForegroundColor = ConsoleColor.Cyan;
                        break;
                    case 'w':
                    case 'c':
                        System.Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case 'd':
                        System.Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    case 's':
                    case 'r':
                    case 'e':
                        System.Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    default:
                        System.Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                System.Console.Write(msgToAppend.Remove(0, 2));
                if (msgToAppend.ToLowerInvariant()[1] == '0') System.Console.BackgroundColor = ConsoleColor.Black;
            }
        }
        #endregion

        #region DummyFunctions
        private static bool _DummyCommandProcessor(object ProcessorObject, string Input)
        {
            return false;
        }
        private static bool _DummyOutputLogger(string Input)
        {
            return false;
        }
        #endregion

        #region Variables
        public static ManualResetEvent ConsoleUnlocked = new ManualResetEvent(true);
        public static ManualResetEvent ConsoleReady = new ManualResetEvent(false);
        private static List<ConsoleWriter> ConsoleOutput = new List<ConsoleWriter>();
        private static string ConsoleInput = "";
        private static int InputPosition = 0;

        public static ManualResetEvent Terminate = new ManualResetEvent(false);
        public static string ConsolePrompt = "&8OpenYS&8->&f";
        public delegate void CommandProcessor(string Input);
        public static CommandProcessor _Process = delegate { };
        public delegate bool ConsoleOutputLogger(string Output, params object[] Args);
        public static ConsoleOutputLogger _LogOutput = delegate { return false; };
        private static string _LastConsoleOutput = "";
        private static string OutputLine = "";
        private static bool _Locked = true;

        public static bool Locked
        {
            get
            {
                return _Locked;
            }
            set
            {
                _Locked = value;
                Console.Write("");
            }
        }
        public static List<String> CommandsTyped = new List<String>();
        public static int CommandHighlighted = 0;
        public static Thread Writer = Threads.Prepare(() => WriterThread(), "Console Output");
        public static Thread Reader = Threads.Prepare(() => ReaderThread(), "Console Input");

        public class ConsoleWriter
        {
            public ManualResetEvent Listener;
            public string Output;

            public ConsoleWriter(string Input)
            {
                Output = Input;
                Listener = new ManualResetEvent(false);
            }
        }
        #endregion

        #region Constructor
        static Console()
        {
            try
            {
                Writer.Start();
                System.Console.Write("");
                CommandsTyped.Add("<No More Previous Commands>");
                Reader.Start();
            }
            catch (Exception e)
            {
                Log.Error(e);
                System.Console.WriteLine("Error launching the OYS Console! Restart the app?");
                System.Console.ReadKey();
                System.Environment.Exit(1);
            }
            //System.Console.WriteLine("");
        }
        #endregion

        #region ConsoleIO
        public static void WriterThread()
        {
            if (!console_present) return;
            #region TRY:
#if RELEASE
                try {
#endif
            #endregion
                #region Writer
            while (true)
            {
                ConsoleReady.WaitOne();
                ConsoleUnlocked.Reset();
                ConsoleWriter[] _Array;
                lock (ConsoleOutput)
                {
                    string InputLine = ConsoleInput;
                    int _StartIndex = 0;
                    while (InputLine.Length + ConsolePrompt.Length > System.Console.WindowWidth)
                    {
                        if (_StartIndex >= InputPosition)
                        {
                            InputLine = InputLine.Substring(0, System.Console.WindowWidth - ConsolePrompt.Length);
                            break;
                        }
                        InputLine = InputLine.Substring(1, InputLine.Length - 1);
                        _StartIndex++;
                        //ConsoleInput.Substring(0, InputPosition) + ThisChar.KeyChar + ConsoleInput.Substring(InputPosition);
                    }
                    InputLine += " ";
                    _Array = ConsoleOutput.ToArray();
                    foreach (ConsoleWriter ThisWriter in _Array)
                    {
                        if (_LastConsoleOutput.EndsWith("\n"))
                        {
                            System.Console.SetCursorPosition(0, System.Console.CursorTop);
                            System.Console.Write("\r" + new string(' ', System.Console.WindowWidth));
                            System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                            //string ThisMessage = BreakDownString(ThisWriter.Output);
                            ConsoleHandler(ThisWriter.Output);
                            _LastConsoleOutput = ThisWriter.Output;
                        }
                        else
                        {
                            //string ThisMessage = BreakDownString(ThisWriter.Output);
                            ConsoleHandler(ThisWriter.Output);
                            _LastConsoleOutput = ThisWriter.Output;
                        }
                        OutputLine += ThisWriter.Output;
                        if (ThisWriter.Output.EndsWith("\n"))
                        {
                            string Output = Strings.StripFormatting(OutputLine);
                            string HTMLString = OutputLine.Substring(0, OutputLine.Length - 1);
                            _LogOutput(HTMLString);
                            OutputLine = "";
                            if (!Locked)
                            {
                                System.Console.Write("\r" + new string(' ', System.Console.WindowWidth));
                                System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                                ConsoleHandler(ConsolePrompt + InputLine);
                            }
                        }
                        ConsoleOutput.RemoveAll(x => x == ThisWriter);
                        ThisWriter.Listener.Set();
                    }
                    if (_LastConsoleOutput.EndsWith("\n"))
                    {
                        if (!Locked)
                        {
                            int i = InputPosition;
                            try
                            {
                                System.Console.SetCursorPosition(0, System.Console.CursorTop);
                                System.Console.Write("\r" + new string(' ', System.Console.WindowWidth));
                                System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                                ConsoleHandler(ConsolePrompt);
                                System.Console.Write(InputLine.Substring(0, InputPosition - _StartIndex));
                                System.Console.BackgroundColor = ConsoleColor.DarkGray;
                                System.Console.Write(InputLine.Substring(InputPosition - _StartIndex, 1));
                                System.Console.BackgroundColor = ConsoleColor.Black;
                                System.Console.Write(InputLine.Substring(InputPosition - _StartIndex + 1));
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                        }
                    }
                }
                //ConsoleOutput.Clear();
                ConsoleReady.Reset();
                ConsoleUnlocked.Set();
            }
            #endregion
            #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    TerminateConsole(e);
                    return;
                }
#endif
            #endregion
        }
        public static void ReaderThread()
        {
            if (!console_present) return;
            #region TRY:
#if RELEASE
                try {
#endif
            #endregion
                #region Reader
                while (!Terminate.WaitOne(0)) //WaitOne(0) will return immediately if it is set or not...
                {
                    ConsoleKeyInfo ThisChar = System.Console.ReadKey(true);
                    if (Locked) continue;
                    #region Key Switch
                    switch (ThisChar.Key)
                    {
                        case ConsoleKey.Backspace:
                            if (ConsoleInput.Length > 0 && InputPosition > 0)
                            {
                                ConsoleInput = ConsoleInput.Substring(0, InputPosition-1) + ConsoleInput.Substring(InputPosition);
                                CommandsTyped[CommandsTyped.Count - 1] = ConsoleInput;
                                InputPosition -= 1;
                                if (InputPosition < 0) InputPosition = 0;
                            }
                            break;
                        case ConsoleKey.Delete:
                            if (ConsoleInput.Length > InputPosition)
                            {
                                ConsoleInput = ConsoleInput.Substring(0, InputPosition) + ConsoleInput.Substring(InputPosition+1);
                            }
                            break;
                        case ConsoleKey.Escape:
                            ConsoleInput = "";
                            InputPosition = 0;
                            CommandsTyped[CommandsTyped.Count - 1] = ConsoleInput;
                            break;
                        case ConsoleKey.Enter:
                            if (ConsoleInput != "")
                            {
                                CommandsTyped[CommandsTyped.Count - 1] = ConsoleInput;
                                CommandsTyped.Add(ConsoleInput);
                                CommandHighlighted = CommandsTyped.Count() - 1;
                                ConsoleInput = "";
                                InputPosition = 0;
                                _Process(CommandsTyped[CommandsTyped.Count - 1]);
                            }
                            break;
                        case ConsoleKey.UpArrow:
                            if (CommandHighlighted > 0)
                            {
                                CommandHighlighted--;
                                ConsoleInput = CommandsTyped[CommandHighlighted];
                            }
                            InputPosition = ConsoleInput.Length;
                            break;
                        case ConsoleKey.DownArrow:
                            if (CommandHighlighted == CommandsTyped.Count - 1)
                            {
                                ConsoleInput = "";
                                break;
                            }
                            CommandHighlighted++;
                            ConsoleInput = CommandsTyped[CommandHighlighted];
                            if (CommandHighlighted == CommandsTyped.Count - 1)
                            {
                                ConsoleInput = "";
                            }
                            InputPosition = ConsoleInput.Length;
                            break;
                        case ConsoleKey.LeftArrow:
                            InputPosition -= 1;
                            if (InputPosition < 0) InputPosition = 0;
                            break;
                        case ConsoleKey.RightArrow:
                            InputPosition += 1;
                            if (InputPosition > ConsoleInput.Length) InputPosition = ConsoleInput.Length;
                            break;
                        case ConsoleKey.Home:
                            InputPosition = 0;
                            break;
                        case ConsoleKey.End:
                            InputPosition = ConsoleInput.Length;
                            break;
                        default:
                            if (char.IsControl(ThisChar.KeyChar) && ThisChar.Key != ConsoleKey.UpArrow && ThisChar.Key != ConsoleKey.UpArrow) break;
                            else
                            {
                                ConsoleInput = ConsoleInput.Substring(0, InputPosition) + ThisChar.KeyChar + ConsoleInput.Substring(InputPosition);
                                InputPosition += 1;
                                CommandsTyped[CommandsTyped.Count - 1] = ConsoleInput;
                            }
                            break;
                    }
                    #endregion
                    if (ConsoleUnlocked.WaitOne(1000))
                    {
                        ConsoleReady.Set();
                    }
                }
                #endregion
            #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    TerminateConsole(e);
                    return;
                }
#endif
            #endregion
        }
        #endregion

        #region Functions
        public static string[] Converter(string input)
        {

            List<string> output = new List<string>();
            string nextoutput = "";
            char nextcolor = 'f';
            if (input == null) return output.ToArray();
            if (input.IndexOf('&') == -1)
            {
                output.Add("&f" + input);
                return output.ToArray();
            }
            else
            {
                for (int i = 0; i < input.Length; i++)
                {
                    goto Skip;
                    ColorFound:
                        i++;
                    Skip:
                    if (i > 0)
                    {
                        if (input[i] == '\\')
                        {
                            i++;
                            nextoutput += input[i];
                            continue;
                        }
                    }
                    if (input[i] == '&')
                    {
                        if (input.Length > i + 1)
                        {
                            foreach (char colorid in "0123456789abcdef")
                            {
                                if (colorid == input.ToLowerInvariant()[i + 1])
                                {
                                    output.Add("&" + nextcolor + nextoutput);
                                    nextoutput = "";
                                    nextcolor = colorid;
                                    goto ColorFound;
                                }
                            }
                            foreach (char colorid in "syprhwmi")
                            {
                                if (colorid == input.ToLowerInvariant()[i + 1])
                                {
                                    output.Add("&" + nextcolor + nextoutput);
                                    nextoutput = "";
                                    nextcolor = colorid;
                                    goto ColorFound;
                                }
                            }
                            //Didn't need to change color...
                            //nextoutput += input[i];
                        }
                        else
                        {
                            Debug.WriteLine("!");
                        }
                    }
                    else if (i > 0)
                    {
                        if (input[i - 1] == '&')
                        {
                            if (i > 1)
                            {
                                if (input[i - 2] == '\\')
                                {
                                    nextoutput += input[i];
                                    continue;
                                }
                            }
                            foreach (char colorid in "0123456789abcdef")
                            {
                                if (colorid == input[i])
                                {
                                    continue;
                                }
                            }
                            foreach (char colorid in "syprhwmi")
                            {
                                if (colorid == input.ToLowerInvariant()[i])
                                {
                                    output.Add("&" + nextcolor + nextoutput);
                                    nextoutput = "";
                                    nextcolor = colorid;
                                    continue;
                                }
                            }
                            continue;
                        }
                    }
                    if (input[i] == '&') nextoutput += "\\";
                    nextoutput += input[i];
                }
                output.Add("&" + nextcolor + nextoutput);
                return output.ToArray();
            }
        }

        private static void _WriteLine(object message)
        {
            if (!console_present) return;
            string process = message.ToString() + "\n";
            process = process.ReplaceAll("\t", "    ");
            string output = "";
            int HorizontalPosition = 0;
            for (int i = 0; i < process.Length; i++)
            {
                //if (process[i] == '\n' && i + 1 >= System.Console.BufferWidth) continue; //console auto new lines for us!
                if (process[i] == '\n') HorizontalPosition = 0;
                if (process[i] == '\r') HorizontalPosition = 0;
                if (HorizontalPosition >= System.Console.BufferWidth)
                {
                    //no newline required as buffer automatically jumps to next!
                    output += "> ";
                    HorizontalPosition = 2;
                }
                if (process[i] == '\t')
                {
                    if (HorizontalPosition % 4 == 0) HorizontalPosition += 4;
                    else HorizontalPosition += 4 - (HorizontalPosition % 4);
                }
                output += process[i];
                HorizontalPosition++;
                if (process[i] == '&') HorizontalPosition -= 2;
            }

            _Write(output);
        }
        private static void _Write(object message)
        {
            if (!console_present) return;
            string output = message.ToString();
            ConsoleWriter ThisWriter = new ConsoleWriter(output);  
            if (ConsoleUnlocked.WaitOne(1000))
            {
                lock (ConsoleOutput) ConsoleOutput.Add(ThisWriter);
                ConsoleReady.Set();
                if (!ThisWriter.Listener.WaitOne(5000))
                {
                    Debug.WriteLine("Failed To Write Message: " + ThisWriter.Output);
                }
            }
            //ConsoleHandler(message.ToString());
        }
        public static void WriteLine(System.ConsoleColor Color, object message)
        {
            if (!console_present) return;
            string output = Color.AsString() + message.ToString() + "\n";
            _Write(output);
        }
        public static void WriteLine(object message)
        {
            if (!console_present) return;
            string output = message.ToString() + "\n";
            _Write(output);
        }
        public static void WriteLine(string Message, params object[] Args)
        {
            if (!console_present) return;
            if (Args == null) Args = new object[0];
            if (Args.Length > 0)
            {
                try
                {
                    Message = String.Format(Message, Args);
                }
                catch (Exception e)
                {
                    Message = e.StackTrace + "\n" + Message;
                }
            }
            string output = Message.ToString() + "\n";
            _Write(output);
        }
        public static void Write(System.ConsoleColor Color, object message)
        {
            if (!console_present) return;
            string output = Color.AsString() + message.ToString();
            Write(output);
        }
        public static void Write(object message)
        {
            if (!console_present) return;
            string output = message.ToString();
            Write(output);
        }
        public static void Write(string Message, params object[] Args)
        {
            if (!console_present) return;
            if (Args == null) Args = new object[0];
            if (Args.Length > 0)
            {
                try
                {
                    Message = String.Format(Message, Args);
                }
                catch (Exception e)
                {
                    Message = e.StackTrace + "\n" + Message;
                }
            }
            string output = Message;
            _Write(output);
        }
        public static void WriteLine()
        {
            if (!console_present) return;
            string output = "\n";
            _Write(output);
        }
        public static char ReadKey()
        {
            if (!console_present) return '\0';
            ConsoleUnlocked.Reset();
            char output = System.Console.ReadKey().KeyChar;
            ConsoleUnlocked.Set();
            return output;
        }
        public static string ReadLine()
        {
            if (!console_present) return "\0";
            ConsoleUnlocked.Reset();
            string output = System.Console.ReadLine();
            ConsoleUnlocked.Set();
            return output;
        }
        public static void Clear()
        {
            if (!console_present) return;
            if (ConsoleUnlocked.WaitOne(1000))
            {
                System.Console.Clear();
            }
        }
        public static ConsoleColor ForegroundColor
        {
            get { if (!console_present) return ConsoleColor.White; return System.Console.ForegroundColor; }
            set { if (!console_present) return; System.Console.ForegroundColor = value; }
        }
        public static ConsoleColor BackgroundColor
        {
            get { if (!console_present) return ConsoleColor.Black; return System.Console.BackgroundColor; }
            set { if (!console_present) return; System.Console.BackgroundColor = value; }
        }
        public static string Title
        {
            get { if (!console_present) return ""; return System.Console.Title; }
            set { if (!console_present) return; System.Console.Title = value; }
        }
        /// <summary>
        /// Immediately closes OpenYS due to exception and shows the user the exception code.
        /// </summary>
        /// <param name="e"></param>
        public static void TerminateConsole(Exception e)
        {
            if (!console_present) return;
            //DumpPDB();

            if (e is ThreadAbortException) return;
            Console.Locked = true;
            Console.WriteLine(ConsoleColor.Red, "OpenYS Has been terminated with error:");
            Console.WriteLine();
            Console.WriteLine(Debug.GetStackTrace(e));
            string[] Email = Emailing.PrepareCrashReportEmail(e);
            //SendEmail(Email[0], Email[1]);
            Thread.Sleep(5000);
            //RemovePDB();
            System.Environment.Exit(0);
            TerminateNow();
        }

        /// <summary>
        /// Immediately closes OpenYS and shows the user the reason.
        /// </summary>
        /// <param name="e"></param>
        public static void TerminateConsole(string Reason)
        {
            if (!console_present) return;
            Console.WriteLine();
            Console.WriteLine(ConsoleColor.Red, "OpenYS Has been terminated for the following reason:");
            Console.WriteLine(ConsoleColor.Yellow, " " + Reason);
            Console.WriteLine();
            TerminateNow();
        }

        /// <summary>
        /// Private function only visable to the utilities functions. This actually causes the environment to exit and the program to end.
        /// </summary>
        private static void TerminateNow()
        {
            if (!console_present)
            {
                System.Environment.Exit(0);
                return;
            }
            //Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Terminate.Set();
            Console.Reader.Abort();
            for (int i = 10 - 1; i > 0; i--)
            {
                Console.WriteLine(String.Format("\r&cOpenYS Will Shutdown In &f{0}&c Seconds.   ", i + 1));
                Thread.Sleep(1000);
            }
            Console.WriteLine(String.Format("\r&cOpenYS Will Shutdown In &f1&c Second.   "));
            Thread.Sleep(1000);
            for (int i = 3; i > 0; i--)
            {
                Console.WriteLine("\r&c!!!&f OpenYS Shutting Down &c!!!                            ");
                Thread.Sleep(500);
                Console.WriteLine("\r                                                  ");
                Thread.Sleep(500);
            }
            System.Environment.Exit(0);
        }
        private static void RestartNow()
        {
            if (!console_present)
            {
                Environment.RestartNow();
                return;
            }
            //Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Terminate.Set();
            Console.Reader.Abort();
            for (int i = 10 - 1; i > 0; i--)
            {
                Console.WriteLine(String.Format("\r&cOpenYS Will Restart In &f{0}&c Seconds.   ", i + 1));
                Thread.Sleep(1000);
            }
            Console.WriteLine(String.Format("\r&cOpenYS Will Restart In &f1&c Second.   "));
            Thread.Sleep(1000);
            for (int i = 3; i > 0; i--)
            {
                Console.WriteLine("\r&c!!!&f OpenYS Restarting &c!!!                            ");
                Thread.Sleep(500);
                Console.WriteLine("\r                                                  ");
                Thread.Sleep(500);
            }
            Environment.RestartNow();
        }

        private static string BreakDownString(string Input)
        {
            if (!console_present)
            {
                return "";
            }
            string Username = Input.Split(')')[0];
            int CurLen = 0;
            string Indent = "    ";
            int MaxWidth = System.Console.BufferWidth;

            string output = Username;
            List<string> BuildingString = Input.Substring(Username.Length).Split(' ').ToList();

            while (BuildingString.Count > 0)
            {
                if (BuildingString[0].StartsWith("\r"))
                {
                    output += BuildingString[0];
                    CurLen = 0;
                    BuildingString[0] = BuildingString[0].Substring(1);
                }
                if (output != "") output += " ";
                if (CurLen > MaxWidth)
                {
                    output += "\n";
                    output += Indent;
                    CurLen = 4;
                }
                else if (BuildingString[0].Length + CurLen > MaxWidth)
                {
                    output += "\n";
                    output += Indent;
                    CurLen = 4;
                }
                string temp = BuildingString[0];
                while (temp.Length > MaxWidth - Indent.Length & !temp.StartsWith("\r"))
                {
                    int Limit = MaxWidth > temp.Length ? temp.Length : MaxWidth;
                    output += temp.Substring(0, Limit - Indent.Length);
                    //output += " ";
                    temp = temp.Substring(Limit - Indent.Length);
                    output += "\n";
                    output += Indent;
                    CurLen = 4;
                }
                output += temp;
                CurLen += temp.Length;
                CurLen++;
                BuildingString.RemoveAt(0);
            }

            return output;

        }

        private static bool? _console_present;
        public static bool console_present
        {
            get
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    if (_console_present == null)
                    {
                        _console_present = true;
                        try { int window_height = System.Console.WindowHeight; }
                        catch { _console_present = false; }
                    }
                    return _console_present.Value;
                }
            }
        }
        #endregion
    }
}