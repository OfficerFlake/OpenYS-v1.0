using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenYS
{
    public static partial class OpenYS_Server
    {
        public class Program
        {
            public static void Launcher(string[] args)
            {
                //workaround for the development launcher.
                #region DEBUG
#if DEBUG
                Directory.SetCurrentDirectory("./Debug/");
#endif
                #endregion
                #region RELEASE
#if RELEASE
                Directory.SetCurrentDirectory("./Release/");
#endif
                #endregion
                Main(args);
            }

            #region Hide Console DLL's
            [DllImport("kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            const int SW_HIDE = 0;
            const int SW_SHOW = 5;
            #endregion

            public static void Main(string[] args)
            {

                System.Console.Title = "OpenYS Startup";
                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.WriteLine("About to Launch OYS!");
                System.Console.WriteLine();
                System.Console.WriteLine("If this doesn't succeed, there may be an issue with your .Net 4.0+ Install!");
                System.Console.WriteLine(".Net 4.0+ can always be uninstalled and re-installed to fix it!");
                System.Console.WriteLine();
                System.Console.WriteLine("... That said, it is VERY rare that .Net has a problem!");
                System.Console.WriteLine();
                //it is necessary to seperate Main and Run because of DLL loading...
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromLibrariesFolder);
                Run(args);
            }

            #region LoadFromLibrariesFolder
            private static Assembly LoadFromLibrariesFolder(object sender, ResolveEventArgs args)
            {
                //unblock all the files first!
                string[] FileNames = Directory.GetFiles("./Libraries/");
                foreach (string ThisFileName in FileNames)
                {
                    try
                    {
                        Unblock(ThisFileName);
                    }
                    catch
                    {
                    }
                }

                //This handler is called only when the common language runtime tries to bind to the assembly and fails.

                //Retrieve the list of referenced assemblies in an array of AssemblyName.
                Assembly MyAssembly, objExecutingAssembly;
                string strTempAssmbPath = "";
                MyAssembly = null;

                try
                {
                    objExecutingAssembly = Assembly.GetExecutingAssembly();
                    if (args.RequestingAssembly != null) objExecutingAssembly = args.RequestingAssembly;
                    AssemblyName[] arrReferencedAssmbNames = objExecutingAssembly.GetReferencedAssemblies();

                    //Loop through the array of referenced assembly names.
                    foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
                    {
                        //Check for the assembly names that have raised the "AssemblyResolve" event.
                        if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
                        {
                            //Build the path of the assembly from where it has to be loaded.                
                            strTempAssmbPath = "./Libraries/" + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                            break;
                        }

                    }


                    //Load the assembly from the specified path.     
                    if (strTempAssmbPath != "")
                    {
                        byte[] RawBytes = File.ReadAllBytes(strTempAssmbPath);
                        MyAssembly = Assembly.Load(RawBytes);
                    }
                    else
                    {
                        //Do Nothing...
                    }
                }
                catch (Exception e)
                {
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Clear();
                    System.Console.WriteLine("Failed to Launch OYS!");
                    System.Console.WriteLine();
                    System.Console.WriteLine("Specifically, Failed to load DLL: " + args.Name.Substring(0, args.Name.IndexOf(",")));
                    System.Console.WriteLine("Are you sure ALL the .DLL's are in the ./Libraries/ Folder?");
                    System.Console.WriteLine("");
                    System.Console.WriteLine("(Exception Info Follows)");
                    System.Console.WriteLine(e.ToString());
                    System.Console.WriteLine("");
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("[SCROLL UP TO SEE FULL DETAILS!]");
                    while (true)
                    {
                        System.Console.ReadKey(true);
                    }
                }
                //Return the loaded assembly.
                return MyAssembly;
            }

            #region Unblock File
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool DeleteFile(string name);

            private static bool Unblock(string fileName)
            {
                return DeleteFile(fileName + ":Zone.Identifier");
            }
            #endregion
            #endregion

            private static void Run(string[] args)
            {
                //Initialisation
                #region Initialisation
                Thread.CurrentThread.Name = "OpenYS Server Core";
                Threads.List.Add(Thread.CurrentThread);

                Console._LogOutput = Log.ConsoleOutput;
                Console._Process = CommandManager.ProcessConsole;
                Console.OutputLoggers.Add(MainWindow.Console.WriteLine);
                Console.ConsolePrompt = "&3OpenYS&f->&f";
                Console.Locked = true;

                System.Console.CursorVisible = false;
                System.Console.Title = "OpenYS - YSFlight Open Source Server";
                System.Console.Clear();

                #region HideConsole   
                var __consolewindow = GetConsoleWindow();
                ShowWindow(__consolewindow, SW_HIDE);
                #endregion

                #region OYS_Console
                OpenYS.OpenYSConsole.SetConsole();
                OpenYS.OpenYSConsole.SetFakeClient();
                OpenYS.OpenYSConsole.Username = Settings.Options.ConsoleName;
                OpenYS.OpenYSConsole.OP();
                OpenYS.OpenYSConsole.YSFClient.ConnectionContext = ClientIO.ConnectionContexts.Connectionless;
                #endregion
                #region OYS_BackgroundHandler
                OpenYS.OpenYSBackGround.SetController();
                OpenYS.OpenYSBackGround.SetFakeClient();
                OpenYS.OpenYSBackGround.Username = Settings.Options.SchedulerName;
                OpenYS.OpenYSBackGround.OP();
                OpenYS.OpenYSBackGround.YSFClient.ConnectionContext = ClientIO.ConnectionContexts.Connectionless;
                #endregion

                Sequencers.Process = CommandManager.ProcessScheduler;
                Schedulers.Process = CommandManager.ProcessScheduler;

                OpenYS.BuildType = OpenYS._BuildType.Server;
                Environment.EntryAssembly = Assembly.GetExecutingAssembly();
                #endregion

                //Start MainWindow
                #region CreateWindow
                MainWindow ConsoleWindow = null; //ERROR POINT! RESOLVED BELOW, SAFE.
                ManualResetEvent HasLoaded = new ManualResetEvent(false);
                new Thread(() =>
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    ConsoleWindow = new MainWindow();
                    ConsoleWindow.DisableMenuStrip();
                    Application.Run(ConsoleWindow); //RUNS ON CURRENT THREAD!
                    HasLoaded.Set();
                }).Start();

                if (!HasLoaded.WaitOne(500))
                {
                    //Something is wrong, not loaded okay!
                    Debug.WriteLine("Failed to load the MainWindow???");
                }
                #endregion

                //Begin Program.
                #region TRY:
#if RELEASE
                try
                {
#endif
                #endregion
                #region MAIN LOOP
                while (!Environment.TerminateSignal.WaitOne(0))
                    {
                        //Clear Outputs.
                        #region Clear Outputs
                        Console.Clear();
                        ConsoleWindow.DisableMenuStrip();
                        ConsoleWindow.DisableConsoleInput();
                        ConsoleWindow.ClearOutputWindow();
                        ConsoleWindow.FocusOnWindow();
                        ConsoleWindow.FocusOnConsoleInput();
                        #endregion

                        //Loading
                        #region Loading
                        #region ? Argument
                        if (args.Length > 0) if (args[0] == "/?" | args[0] == "?")
                            {
                                System.Console.WriteLine("OpenYS Open Source Server Project. (C) OfficerFlake 2015");
                                System.Console.WriteLine("    ");
                                System.Console.WriteLine("    Usage: OpenYS_Client.exe [HostAddress] [HostPort] [ListenerPortNumber]");
                                System.Console.WriteLine("    ");
                                System.Console.WriteLine("Thanks for using OpenYS!");
                                System.Environment.Exit(0);
                            }
                        #endregion
                        #region Clear Logs
                        Files.FileDelete("./Logs/Client_Debug.Log");
                        Files.FileDelete("./Logs/Console.Log");
                        #endregion
                        #region Introduction
                        Console.WriteLine("&3&LWelcome to OpenYS Client!");
                        Console.WriteLine();
                        #endregion
                        #region Version Number
                        Console.WriteLine(ConsoleColor.DarkCyan, "You are using version:&e " + Environment.GetCompilationDate());
                        Console.WriteLine();
                        #endregion
                        #region LoadSettings
                        SettingsHandler.LoadAll();
                        SettingsHandler.Monitor();
                        #endregion
                        #region InitialSettings
                        OpenYS.OpenYSConsole.Username = Settings.Options.ConsoleName;
                        OpenYS.OpenYSBackGround.Username = Settings.Options.SchedulerName;
                        Environment.OwnerName = Settings.Options.OwnerName;
                        Environment.OwnerEmail = Settings.Options.OwnerEmail;
                        Environment.ServerName = Settings.Options.ServerName;
                        OpenYS.Field = new Packets.Packet_04_FieldName(Settings.Loading.FieldName);
                        #endregion
                        #region Check Arguments
                        CheckArguments(args);
                        #endregion
                        #region Load Commands
                        Commands.LoadAll();
                        #endregion
                        #region Load YSF
                        FormatYSFDirectory();
                        if (!Directories.DirectoryExists(Settings.Loading.YSFlightDirectory)) Console.TerminateConsole("YSFlight Directory Not Found.");
                        LoadAllMetaData();
                        if (!World.Load(OpenYS.Field.FieldName)) Console.TerminateConsole("FLD Name not found.");
                        #endregion
                        #region Load Games
                        Games.Racing.Initialise();
                    //Console.WriteLine();
                    #endregion
                        #region Loading Complete!
                        Console.WriteLine("");
                        Console.WriteLine(ConsoleColor.DarkYellow, "Loading Complete!");
                        Console.WriteLine("");
                        #endregion
                        #endregion

                        //Connection Handling Services.
                        #region Start Server
                        Server.Listener.Start();
                        OpenYS.YSFListener.ServerStarted.WaitOne();
                        #endregion

                        #region External IP & IRC
                        Threads.Add(() =>
                        {
                            #region IRC
                            IRC.Init();
                            IRC.Start();
                            #endregion
                            #region External IP
                            //Console.WriteLine();
                            //Console.WriteLine("&6Fetching External IP...");
                            //Console.WriteLine();
                            var ExternalIP = Environment.ExternalIP;
                            //Console.WriteLine("&6External IP: " + Environment.ExternalIP.ToString());
                            #endregion
                        }, "IRC/External IP Loading...");
                        //Console.WriteLine();
                        #endregion

                        //Timing Services.
                        #region Restart Timer
                        OpenYS.StartResetTimer();
                        #endregion
                        #region Lock/Unlock Console Input
                        Console.Locked = false;
                        Console.WriteLine("\r"); //effectively refreshes the prompt...
                        #endregion
                        #region SetServerTime
                        OpenYS.SetServerTimeTicks(Settings.Weather.Time * 10);
                        #endregion
                        int SeqLoaded = Sequencers.LoadAll();
                        int SchLoaded = Schedulers.LoadAll();

                        //Loading Complete.
                        ConsoleWindow.EnableMenuStrip();
                        ConsoleWindow.EnableConsoleInput();
                        ConsoleWindow.FocusOnConsoleInput();

                        Thread.Sleep(10);
                        int LoopsPassed = 0;

                        while (!OpenYS.Signal_ResetServer.WaitOne(0))
                        {
                            #region Start Tick Update!
                            DateTime StartUpdate = DateTime.Now;
                            #endregion

                            OpenYS.PollResetTimer();
                            //OpenYS.MicroTick();
                            //OpenYS.MacroTick();
                            //OpenYS.UpdateTimeOfDay();
                            //Schedulers.Poll();
                            //Sequencers.Poll();
                            //YSFlightReplays.ServerReplay.SendUpdate();

                            #region End Tick Update!
                            DateTime EndUpdate = DateTime.Now;
                            #endregion
                            #region MicroTick Delay
                            OpenYS.Time_LastMicroTick = DateTime.Now;
                            int ThisUpdateTotalTime = (int)Math.Ceiling(((TimeSpan)(EndUpdate - StartUpdate)).TotalMilliseconds);
                            if (ThisUpdateTotalTime < 100)
                            {
                                Thread.Sleep(100 - ThisUpdateTotalTime); //Micro Tick Delay.
                            }
                            //No Delay, resume immediately! - The update took over 100m/s!
                            #endregion
                            LoopsPassed++;
                        }
                        //We should reset now.
                        //OpenYS.TickUpdaterThread.Abort();
                        Schedulers.StopAll();
                        Sequencers.StopAll();
                        Server.Listener.Stop();
                        foreach(Client ThisClient in Clients.AllClients)
                        {
                            ThisClient.Disconnect(OpenYS.ShutdownOnNextRestart ? "Server shutting down." : "Server restarting.");
                        }
                        Console.Clear();
                        args = Environment.CommandLineArguments;
                        if (OpenYS.ShutdownOnNextRestart) Environment.QuitNow();
                        //Utilities.Restart();
                    }
                    #endregion
                #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException) return;
                    Log.Error(e);
                    #region TRY:
#if RELEASE
                    try
                    {
#endif
                        #endregion
                        #region Terminate
                        Console.Locked = true;
                        Console.WriteLine(ConsoleColor.Red, "OpenYS Has been terminated with error:");
                        Console.WriteLine();
                        Console.WriteLine(Debug.GetStackTrace(e));
                        Emailing.SendCrashReportEmail(e);
                        Thread.Sleep(5000);
                        //RemovePDB();
                        System.Environment.Exit(0);
                        Console.WriteLine();
                        Console.Terminate.Set();
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
                        #endregion
                    #region CATCH
#if RELEASE
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine("Safe Restart Failed... Sorry!");
                        Console.WriteLine(e2.Message);
                        Log.Error(e2);
                        Thread.Sleep(Timeout.Infinite);
                        Console.TerminateConsole("End Of Program");
                        return;
                    }
#endif
                        #endregion
                }
#endif
                #endregion
            }

            static bool CheckArguments(string[] args)
            {
                Environment.CommandLineArguments = args;

                //ARGUMENT STRUCTURE
                //
                //OpenYS.EXE YSFlightDirectoryPath FieldName ConnectionListenerPortNumber
                //Load the target directory from the first command line argument.
                if (Environment.CommandLineArguments.Length > 0)
                {
                    Settings.Loading.YSFlightDirectory = Environment.CommandLineArguments[0];
                    //Console.WriteLine(Utilities.CommandLineArgs[0]);
                }
                Settings.Loading.YSFlightDirectory = Settings.Loading.YSFlightDirectory.ReplaceAll("\\", "/");

                //if the YSFlight directory doesn't exist, leave the program.
                if (!Directories.DirectoryExists(Settings.Loading.YSFlightDirectory))
                {
                    Console.WriteLine("YSFlight Directory not Found. Check command line argument 0 is the YSFlight Directory path.");
                    Console.WriteLine();
                    Console.WriteLine("If no command line argument is given, check YSFlight is installed in the default directory.");
                    Console.TerminateConsole("YSF Directory Missing!");
                    return false;
                }

                if (Environment.CommandLineArguments.Length > 1) OpenYS.Field.FieldName = Environment.CommandLineArguments[1];

                if (Environment.CommandLineArguments.Length > 2)
                {
                    UInt32.TryParse(Environment.CommandLineArguments[2], out Settings.Server.ListenerPort);
                }
                return true;
            }

            static bool FormatYSFDirectory()
            {
                while (Settings.Loading.YSFlightDirectory.EndsWith("/")) Settings.Loading.YSFlightDirectory = Settings.Loading.YSFlightDirectory.Substring(0, Settings.Loading.YSFlightDirectory.Length - 1);
                Settings.Loading.YSFlightDirectory += "/";
                Console.Write(ConsoleColor.DarkYellow, "YSFlight Directory is: ");
                Console.WriteLine(ConsoleColor.Cyan, Settings.Loading.YSFlightDirectory);
                return true;
            }

            static bool LoadAllMetaData()
            {
                Console.WriteLine();
                Console.WriteLine(ConsoleColor.DarkYellow, "Loading YSFlight MetaData:");
                MetaData._Aircraft.LoadAll();
                Console.Write(ConsoleColor.White, "    Loaded ");
                if (MetaData._Aircraft.List.Count == 0) Console.Write(ConsoleColor.Red, MetaData._Aircraft.List.Count.ToString());
                else Console.Write(ConsoleColor.Green, MetaData._Aircraft.List.Count.ToString());
                Console.WriteLine(ConsoleColor.White, " Aircraft");

                MetaData._Ground.LoadAll();
                Console.Write(ConsoleColor.White, "    Loaded ");
                if (MetaData._Ground.List.Count == 0) Console.Write(ConsoleColor.Red, MetaData._Ground.List.Count.ToString());
                else Console.Write(ConsoleColor.Green, MetaData._Ground.List.Count.ToString());
                Console.WriteLine(ConsoleColor.White, " Ground Objects");

                MetaData._Scenery.LoadAll();
                Console.Write(ConsoleColor.White, "    Loaded ");
                if (MetaData._Scenery.List.Count == 0) Console.Write(ConsoleColor.Red, MetaData._Scenery.List.Count.ToString());
                else Console.Write(ConsoleColor.Green, MetaData._Scenery.List.Count.ToString());
                Console.WriteLine(ConsoleColor.White, " Scenery");

                Console.WriteLine();
                return true;
            }
        }
    }
}
