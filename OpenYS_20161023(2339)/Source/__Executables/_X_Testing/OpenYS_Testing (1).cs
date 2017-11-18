using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OpenYS
{
    public static partial class OpenYS_Testing
    {
        public static class Program
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
                //TEST BED HERE

                Console.WriteLine("Sup Brah!");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                new Thread
                (() =>
                {
                    ConsoleWindow = new MainWindow();
                    ConsoleWindow.FormBorderStyle = FormBorderStyle.FixedSingle;
                    ConsoleWindow.Size = new Size(1200, 675);
                    ConsoleWindow.ClientSize = new Size(1200, 675);
                    ConsoleWindow.ShowDialog();
                }
                ).Start();

                while(ConsoleWindow == null)
                {
                    Thread.Sleep(10);
                }
                while (!ConsoleWindow.IsHandleCreated)
                {
                    Thread.Sleep(10);
                }

                NewConsole.WriteToConsole("&fF&eE&dD&cC&bB&aA&99&88&77&66&55&44&33&22&11&00\n");
                NewConsole.WriteToConsole("&K&fF&eE&dD&cC&bB&aA&99&88&77&66&55&44&33&22&11&00\n");
                NewConsole.WriteToConsole("&L&fF&eE&dD&cC&bB&aA&99&88&77&66&55&44&33&22&11&00\n");
                NewConsole.WriteToConsole("&M&fF&eE&dD&cC&bB&aA&99&88&77&66&55&44&33&22&11&00\n");
                NewConsole.WriteToConsole("&N&fF&eE&dD&cC&bB&aA&99&88&77&66&55&44&33&22&11&00\n");
                NewConsole.WriteToConsole("&R&fF&eE&dD&cC&bB&aA&99&88&77&66&55&44&33&22&11&00\n");

                //END TEST BED.
            }
        }

        public static class NewConsole
        {
            public static void WriteToConsole(string Output)
            {
                ConsoleWindow.WriteLine(Output);
            }
        }
        private delegate void AddTextCallback(string text);



        private static MainWindow ConsoleWindow;
    }
}
