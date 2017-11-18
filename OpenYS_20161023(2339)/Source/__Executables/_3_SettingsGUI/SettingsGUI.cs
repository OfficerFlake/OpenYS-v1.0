using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using OfficeOpenXml;

namespace OpenYS
{
    public static partial class SettingsGUI
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Launcher(string[] args)
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
            Main(args);
        }
        public static void Main(string[] args)
        {
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

        public static SettingsGUI_Form MainForm = null;

        public static void Run(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new SettingsGUI_Form();
            Load();
            Application.Run(MainForm);
        }

        public class CachedSetting
        {
            public string Key;
            public string Val;
            public string Rem;

            public CachedSetting(string _Key, string _Val, string _Rem)
            {
                Key = _Key;
                Val = _Val;
                Rem = _Rem;
            }
        }

        public static List<CachedSetting> SettingsList = new List<CachedSetting>();

        public static DataTable ToDataTable(this List<CachedSetting> Input)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("Key");
            dt.Columns.Add("Value");
            dt.Columns.Add("Remarks");
            foreach (CachedSetting ThisSetting in Input)
            {
                dt.Rows.Add(ThisSetting.Key, ThisSetting.Val, ThisSetting.Rem);
            }

            return dt;
        }

        public static List<CachedSetting> LoadAll()
        {
            List<CachedSetting> Output = new List<CachedSetting>();
            FileInfo File = new FileInfo("./Settings.xlsx");
            if (!File.Exists)
            {
                SettingsHandler.DumpSettingsFileTemplate("./Settings.xlsx");
            }
            //This framework is a real pain in the ARSE!
            //It uses non zero based indexes!
            try
            {
                using (ExcelPackage xlPackage = new ExcelPackage(File))
                {
                    // get the first worksheet in the workbook
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                    for (int i = 4; i < 1024; i++)
                    {
                        object _Key = worksheet.Cells[i, 1].Value;
                        string Key = _Key == null ? "" : _Key.ToString();
                        if (Key == "")
                        {
                            continue;
                        }
                        object _Val = worksheet.Cells[i, 2].Value;
                        string Val = _Val == null ? "" : _Val.ToString();
                        object _Rem = worksheet.Cells[i, 3].Value;
                        string Rem = _Rem == null ? "" : _Rem.ToString();
                        Output.Add(new CachedSetting(Key, Val, Rem));
                    }

                } // the using statement calls Dispose() which closes the package.
                return Output;
            }
            catch
            {
                MessageBox.Show("File I/O Error while trying to read the settings file!\n\nPlease make sure the settings file is not currently in use!");
                return Output;
            }
        }

        public static bool Save()
        {
            FileInfo File = new FileInfo("./Settings.xlsx");
            //This framework is a real pain in the ARSE!
            //It uses non zero based indexes!
            using (ExcelPackage xlPackage = new ExcelPackage(File))
            {
                // get the first worksheet in the workbook
                ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];
                SettingsList.OrderBy(x => x.Key);
                worksheet.Cells[2, 1].Value = "Last Updated " + DateTime.Now.ToOYSShortDateTime();
                for (int i = 0; i < MainForm.DataGridObject.Rows.Count; i++)
                {
                    worksheet.Cells[i+4, 1].Value = MainForm.DataGridObject.Rows[i].Cells[0].Value;
                    worksheet.Cells[i + 4, 2].Value = MainForm.DataGridObject.Rows[i].Cells[1].Value;
                    worksheet.Cells[i + 4, 3].Value = MainForm.DataGridObject.Rows[i].Cells[2].Value;
                }
                xlPackage.Save();

            } // the using statement calls Dispose() which closes the package.
            return true;
        }

        public static bool Load()
        {
            SettingsList = LoadAll();
            MainForm.DataGridObject.DataSource = SettingsList.ToDataTable();
            MainForm.DataGridObject.Columns[0].Width = 250;
            MainForm.DataGridObject.Columns[1].Width = 100;
            MainForm.DataGridObject.Columns[2].Width = 228;
            return true;
        }
    }
}
