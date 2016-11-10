using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

//READ THIS BEFORE YOU START MODIFYING CODE!

//OpenYS DOES implement a few rare eastereggs... To keep them surprises I have hidden the contents of EasterEggs.cs from the release.
//The source will still make function calls to these methods however the methods will always return void.
//If anything, the program will run FASTER on your PC than it does on mine when calling these methods.

namespace OpenYS
{
    public static partial class OpenYS_Client_old
    {
        public class Program
        {
            #region DLL Functions
            [DllImport("User32.dll")]
            static extern IntPtr GetDC(IntPtr hwnd);

            [DllImport("User32.dll")]
            static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);

            [DllImport("user32.dll")]
            static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
            #endregion

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

            public static void Main(string[] args)
            {
                System.Console.Title = "OpenYS Startup";
                System.Console.ForegroundColor = ConsoleColor.Red;
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
                    byte[] RawBytes = File.ReadAllBytes(strTempAssmbPath);
                    MyAssembly = Assembly.Load(RawBytes);
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

            public static string GetActiveWindowTitle()
            {
                const int nChars = 256;
                StringBuilder Buff = new StringBuilder(nChars);
                IntPtr handle = GetForegroundWindow();

                if (GetWindowText(handle, Buff, nChars) > 0)
                {
                    return Buff.ToString();
                }
                return null;
            }

            private static void Run(string[] args)
            {
                //Initialisation
                #region Initialisation
                Thread.CurrentThread.Name = "OpenYS Client Core";
                Threads.List.Add(Thread.CurrentThread);

                Console._LogOutput = Log.ConsoleOutput;
                Console._Process = CommandManager.ProcessConsole;
                Console.ConsolePrompt = "&cOpenYS&f->&f";
                Console.Locked = true;

                if (Console.console_present)
                {
                    System.Console.CursorVisible = false;
                    System.Console.Clear();
                    System.Console.Title = "OpenYS - YSFlight Open Source Client";
                }

                #region OYS_Console
                OpenYS.OpenYSConsole.SetConsole();
                OpenYS.OpenYSConsole.SetFakeClient();
                OpenYS.OpenYSConsole.Username = Settings.Options.ConsoleName;
                OpenYS.OpenYSConsole.OP();
                #endregion
                #region OYS_BackgroundHandler
                OpenYS.OpenYSBackGround.SetController();
                OpenYS.OpenYSBackGround.SetFakeClient();
                OpenYS.OpenYSBackGround.Username = Settings.Options.SchedulerName;
                OpenYS.OpenYSBackGround.OP();
                #endregion
                

                Sequencers.Process = CommandManager.ProcessScheduler;
                Schedulers.Process = CommandManager.ProcessScheduler;

                OpenYS.BuildType = OpenYS._BuildType.Client;
                Environment.EntryAssembly = Assembly.GetExecutingAssembly();
                #endregion

                //Begin Program.
                #region TRY:
#if RELEASE
                try {
#endif
                #endregion
                    #region MAIN LOOP
                    //Loading
                    #region Loading
                    #region ? Argument
                    if (args.Length > 0) if (args[0] == "/?" | args[0] == "?")
                    {
                        if (Console.console_present)
                        {
                            System.Console.WriteLine("OpenYS Open Source Server Project. (C) OfficerFlake 2015");
                            System.Console.WriteLine("    ");
                            System.Console.WriteLine("    Usage: OpenYS_Client.exe [HostAddress] [HostPort] [ListenerPortNumber]");
                            System.Console.WriteLine("    ");
                            System.Console.WriteLine("Thanks for using OpenYS!");
                            System.Environment.Exit(0);
                        }
                    }
                    #endregion
                    #region Clear Logs
                    Files.FileDelete("./Logs/Client_Debug.Log");
                    Files.FileDelete("./Logs/Console.Log");
                    #endregion
                    #region Introduction
                    Console.WriteLine(ConsoleColor.Red, "Welcome to OpenYS Client!");
                    Console.WriteLine();
                    Console.WriteLine(ConsoleColor.DarkYellow, "This client allows your YSF Client to use the extended features of OpenYS!");
                    Console.WriteLine();
                    #endregion
                    #region Version Number
                    Console.WriteLine(ConsoleColor.Red, "You are using version:&e " + Environment.GetCompilationDate());
                    Console.WriteLine();
                    #endregion
                    #region LoadSettings
                    SettingsHandler.LoadAll();
                    SettingsHandler.Monitor();
                    #endregion
                    #region InitialSettings
                    OpenYS.OpenYSConsole.Username = Settings.Options.ConsoleName;
                    OpenYS.OpenYSBackGround.Username = Settings.Options.SchedulerName;
                    OpenYS.Field = new Packets.Packet_04_FieldName(Settings.Loading.FieldName);
                    #endregion
                    #region Check Arguments
                    CheckArguments(args);
                    #endregion
                    #region Load Commands
                    Commands.LoadAll();
                    #endregion
                    #endregion

                    //Connection Handling Services.
                    #region Start Server
                    Server.Listener.Start();
                    OpenYS.YSFListener.ServerStarted.WaitOne();
                    #endregion

                    //Timing Services.
                    #region Lock/Unlock Console Input
                    Console.Locked = true;
                    Console.WriteLine("\r"); //effectively refreshes the prompt...
                    #endregion
                    //Sequencers.LoadAll();
                    //Schedulers.LoadAll();

                    Thread.Sleep(Timeout.Infinite);
                    Console.TerminateConsole("End Of Program");
                    return;
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
                //OpenYS.EXE OutBoundServerIP OutBoundServerPortNumber ConnectionListenerPortNumber
                if (Environment.CommandLineArguments.Length > 0)
                {
                    try
                    {
                        Settings.Server.HostAddress = Dns.GetHostAddresses(Environment.CommandLineArguments[0])[0];
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
                if (Environment.CommandLineArguments.Length > 1)
                {
                    UInt32.TryParse(Environment.CommandLineArguments[1], out Settings.Server.HostPort);
                }
                if (Environment.CommandLineArguments.Length > 2)
                {
                    UInt32.TryParse(Environment.CommandLineArguments[2], out Settings.Server.ListenerPort);
                }
                return true;
            }

        }
    }
}
