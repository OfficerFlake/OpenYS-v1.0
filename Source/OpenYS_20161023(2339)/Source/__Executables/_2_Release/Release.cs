using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace OpenYS
{
    public class Release
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

        public static void Run(string[] args)
        {
            #region Start
            Console.Title = "OpenYS Release Script v1.1";
            Console.WriteLine(ConsoleColor.White,      "OpenYS Release Script v1.1");
            Console.WriteLine(ConsoleColor.DarkCyan, "==========================");
            Console.WriteLine();
            #endregion

            Debug.Console_ShowAllDebugTestPoints();

            string CurrentDirectory = Directory.GetCurrentDirectory();
            string RootPath = "../../../..";
            string OutputPath = "./" + RootPath + "/" + "OpenYS_" + Environment.GetCompilationDate();
            Directories.RemoveMatching("*", OutputPath);
            Thread.Sleep(100);
            string LibraryPath = "./" + RootPath + "/" + "_OpenYS-Library";


            string ExecutablesPath = "./" + RootPath + "/" + "__Executables";
            //create ROOT Folder Named "OpenYS_" + BuildVersion + ".txt"
            Directory.CreateDirectory(OutputPath);
           
            //in ROOT folder create folders "Source" and "Release"
            Directory.CreateDirectory(OutputPath + "/" + "Source");
            Directory.CreateDirectory(OutputPath + "/" + "Release");

            //Copy files "_OpenYS-Library/Licence.url" to ROOT
            File.Copy(LibraryPath + "/" + "Licence.url", OutputPath + "/" + "Licence.url", true);

            //Copy files "_OpenYS-Library/YSFHQ Release Thread.url" to ROOT
            File.Copy(LibraryPath + "/" + "YSFHQ Release Thread.url", OutputPath + "/" + "YSFHQ Release Thread.url", true);

            string OutputExecutablesPath = OutputPath + "/" + "Source" + "/" + "__Executables";
            string OutputExternalLibrariesPath = OutputPath + "/" + "Source" + "/" + "_External-Libraries";
            string OutputLibrariesPath = OutputPath + "/" + "Source" + "/" + "_OpenYS-Library";
            Directory.CreateDirectory(OutputExecutablesPath);
            Directory.CreateDirectory(OutputExternalLibrariesPath);
            Directory.CreateDirectory(OutputLibrariesPath);


            //Copy directories "__Executubales" to "ROOT/Source"
            Directories.CopyAll(RootPath + "/" + "__Executables", OutputExecutablesPath);

            //Copy directories "_External-Libraries" to "ROOT/Source"
            Directories.CopyAll(RootPath + "/" + "_External-Libraries", OutputExternalLibrariesPath);

            //Copy directories "_OpenYS-Library" to "ROOT/Source"
            Directories.CopyAll(RootPath + "/" + "_OpenYS-Library", OutputLibrariesPath);

            //foreach directory in "ROOT/Source" <RECURSIVE>
                //delete folder named "bin"
            Directories.RemoveMatching("bin", OutputPath);
                //delete folder named "obj"
            Directories.RemoveMatching("obj", OutputPath);
            //Create new folder "PreRelease" in "ROOT/Release"

            //Copy contents of "__Executables/_1_OpenYS-ClientMode/bin/Release/" to "ROOT/Release/PreRelease"
            //Copy contents of "__Executables/_1_OpenYS-ServerMode/bin/Release/" to "ROOT/Release/PreRelease" <OVERWRITE>
            //create new folder named "Release" in "ROOT/Release/PreRelease"
            //create new folder named "Libraries" in "ROOT/Release/PreRelease"
            //move files "*.DLL" to "ROOT/Release/PreRelease/Libraries"
            //move files "*.PDB" to "ROOT/Release/PreRelease/Release"

            //copy contents of "_OpenYS-Library/__Supplement/" to "ROOT/Release/PreRelease/"
            Directories.CopyAll(OutputLibrariesPath + "/" + "__Supplement", OutputPath + "/" + "Release");
            Directory.CreateDirectory(OutputPath + "/" + "Release" + "/" + "Debug");
            Directory.CreateDirectory(OutputPath + "/" + "Release" + "/" + "Libraries");
            Directories.CopyAll(ExecutablesPath + "/" + "_1_OpenYS-ServerMode/bin/Release/Debug", OutputPath + "/" + "Release/Debug");
            Directories.CopyAll(ExecutablesPath + "/" + "_1_OpenYS-ServerMode/bin/Release/Libraries", OutputPath + "/" + "Release/Libraries");
            File.Copy(RootPath + "/" + "OpenYS.sln", OutputPath + "/" + "Source" + "/" + "OpenYS.sln", true);
            //File.Copy(ExecutablesPath + "/" + "_-_Launcher/bin/_2_Release/OpenYS-ClientMode.exe", OutputPath + "/" + "Release" + "/" + "OpenYS-ClientMode.exe", true);
            File.Copy(ExecutablesPath + "/" + "_-_Launcher/bin/_2_Release/OpenYS-ServerMode.exe", OutputPath + "/" + "Release" + "/" + "OpenYS-ServerMode.exe", true);
            File.Copy(ExecutablesPath + "/" + "_-_Launcher/bin/_2_Release/OpenYS-SettingsGUI.exe", OutputPath + "/" + "Release" + "/" + "OpenYS-SettingsGUI.exe", true);


            //delete "__Executables/_3_Installer/Resources/"
            //create "__Executables/_3_Installer/Resources/"
            //Copy "ROOT/Release/PreRelease/" to "__Executables/_3_Installer/Resources/"
            
            //Ready to PUBLISH at this point, throw user a message saying to run publish!

            //Publish will build the OYS installer, zip the resulting exe, and publish to the OYS Web Server.

            #region Terminate
            Console.WriteLine();
            Console.WriteLine(ConsoleColor.DarkCyan, "Build Completed Successfully.");
            Console.WriteLine();
            Console.WriteLine(ConsoleColor.White, "Press any key to continue...");
            Console.Reader.Abort();
            System.Console.ReadKey(true);
            System.Environment.Exit(0);
            #endregion
            return;
        }
    }
}
