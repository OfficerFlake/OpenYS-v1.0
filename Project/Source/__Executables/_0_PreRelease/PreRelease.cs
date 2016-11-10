using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace OpenYS
{
    public class PreRelease
    {
        public static void Main(string[] args)
        {
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

        public static void Run(string[] args)
        {
            #region Start
            Console.Title = "OpenYS PreRelease Script v1.0";
            Console.WriteLine(ConsoleColor.Cyan, "OpenYS PreRelease Script v1.0");
            Console.WriteLine();
            #endregion

            #region Update BuildVersion.txt
            //This Exe location is... $(SolutionDir)/__Executables/_-_Launcher/Bin/$(Config)/
            //We move back down towards $(SolutionDir), then climb into the utilities folder...
            string VersionFile = "../../../../_OpenYS-Library/Utilities/BuildVersion.txt";
            string Prefix = "";
            string Suffix = "";

            string[] Formatted = OYS_DateTime.ToOYSFormat(DateTime.Now);
            string[] BuildInfo = new string[] { Prefix + Formatted[0] + Formatted[1] + Formatted[2] + "(" + Formatted[3] + Formatted[4] + ")" + Suffix };

            Files.FileDelete(VersionFile);
            Files.FilePrepare(VersionFile);
            Files.FileAppend(VersionFile, BuildInfo);


            Console.WriteLine(ConsoleColor.DarkCyan, "Updated Build Version to: &b" + BuildInfo[0]);
            #endregion

            #region Terminate
            Console.WriteLine();
            Console.WriteLine(ConsoleColor.Cyan, "Press any key to continue...");
            Console.Reader.Abort();
            System.Console.ReadKey(true);
            System.Environment.Exit(0);
            #endregion
            return;
        }
    }
}
