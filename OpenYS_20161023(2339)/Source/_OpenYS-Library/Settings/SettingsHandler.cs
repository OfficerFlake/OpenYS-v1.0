using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OfficeOpenXml;

namespace OpenYS
{
    public static class SettingsHandler
    {
        #region Data I/O
        public static object _Get(string Key, Type ObjectType, object Default)
        {
            lock (Threads.GenericThreadSafeLock)
            {
                if (!Files.FileExists("./Settings.Dat")) return Default;
                string[] SettingsFileContents = Files.FileReadAllLines("./Settings.Dat");
                string _Value = "";
                foreach (string _ThisLine in SettingsFileContents)
                {
                    if (_ThisLine.ToUpperInvariant().StartsWith("REM")) continue;
                    if (_ThisLine.ToUpperInvariant().StartsWith("//")) continue;
                    if (_ThisLine.ToUpperInvariant().StartsWith(";")) continue;
                    if (_ThisLine.ToUpperInvariant().StartsWith("#")) continue;
                    string ThisLine = _ThisLine;
                    if (ThisLine.Contains("//")) ThisLine = ThisLine.Split(new string[]{"//"}, StringSplitOptions.None)[0];
                    if (ThisLine.Contains(";")) ThisLine = ThisLine.Split(new string[] { ";" }, StringSplitOptions.None)[0];
                    if (ThisLine.Contains("#")) ThisLine = ThisLine.Split(new string[] { "#" }, StringSplitOptions.None)[0];
                    while (ThisLine.EndsWith(" ")) ThisLine = ThisLine.Substring(0, ThisLine.Length - 1);
                    while (ThisLine.EndsWith("\t")) ThisLine = ThisLine.Substring(0, ThisLine.Length - 1);
                    while (ThisLine.Contains("\t\t")) ThisLine = ThisLine.ReplaceAll("\t\t", "\t");
                    while (ThisLine.Contains("  ")) ThisLine = ThisLine.ReplaceAll("  ", " ");
                    if (ThisLine.Split('\t').Length < 2) continue;
                    if (Key.ToUpperInvariant() == ThisLine.Split(new char[] { '\t' }, 2)[0].ToUpperInvariant())
                    {
                        _Value = ThisLine.Split(new char[] { '\t' }, 2)[1];
                    }
                }
                if (_Value == "") return Default;
                try
                {
                    if (ObjectType == typeof(Int16)) return Int16.Parse(_Value);
                    if (ObjectType == typeof(Int32)) return Int32.Parse(_Value);
                    if (ObjectType == typeof(Int64)) return Int64.Parse(_Value);
                    if (ObjectType == typeof(UInt16)) return UInt16.Parse(_Value);
                    if (ObjectType == typeof(UInt32)) return UInt32.Parse(_Value);
                    if (ObjectType == typeof(UInt64)) return UInt64.Parse(_Value);

                    if (ObjectType == typeof(Single)) return Single.Parse(_Value);
                    if (ObjectType == typeof(Double)) return Double.Parse(_Value);

                    if (ObjectType == typeof(Boolean)) return Boolean.Parse(_Value);

                    if (ObjectType == typeof(IPAddress))
                    {
                        IPAddress Out = IPAddress.Parse("127.0.0.1");
                        try
                        {
                            Out = Dns.GetHostAddresses(_Value)[0];
                        }
                        catch
                        {
                        }
                        try
                        {
                            Out = IPAddress.Parse(_Value);
                        }
                        catch
                        {
                        }
                        return Out;
                    }
                    if (ObjectType == typeof(String)) return _Value;

                    if (ObjectType == typeof(Colors.XRGBColor)) return new Colors.XRGBColor(_Value);

                }
                catch
                {
                    return Default;
                }
                return Default;
            }
        }

        public static T Get<T>(string Key, Type ObjectType, T Default)
        {
            try
            {
                return (T)_Get(Key, ObjectType, Default);
            }
            catch (Exception e)
            {
                Log.Error(e);
                Console.WriteLine(e);
                return Default;
            }
        }

        public static bool Set(string Key, object Value)
        {
            lock (Threads.GenericThreadSafeLock)
            {
                string[] SettingsFileContents;
                if (!Files.FileExists("./Settings.Dat")) SettingsFileContents = new string[] { };
                else SettingsFileContents = Files.FileReadAllLines("./Settings.Dat");
                List<string> SettingsFileOutput = new List<string>();
                bool found = false;
                foreach (string _ThisLine in SettingsFileContents)
                {
                    if (_ThisLine.ToUpperInvariant().StartsWith("REM") |
                        _ThisLine.ToUpperInvariant().StartsWith("//") |
                        _ThisLine.ToUpperInvariant().StartsWith("#") |
                        _ThisLine.ToUpperInvariant().StartsWith(";") |
                        _ThisLine == "")
                    {
                        SettingsFileOutput.Add(_ThisLine);
                        continue;
                    }
                    string ThisLine = _ThisLine;
                    while (ThisLine.Contains("\t\t")) ThisLine = ThisLine.ReplaceAll("\t\t", "\t");
                    while (ThisLine.Contains("  ")) ThisLine = ThisLine.ReplaceAll("  ", " ");
                    if (ThisLine.Split('\t').Length < 2)
                    {
                        SettingsFileOutput.Add(_ThisLine);
                        continue;
                    }
                    if (Key.ToUpperInvariant() == ThisLine.Split(new char[] { '\t' }, 2)[0].ToUpperInvariant())
                    {
                        string ReplaceValue = ThisLine.Split(new char[] { '\t' }, 2)[1].Split('\t')[0];
                        if (ReplaceValue.Length > 0)
                        {
                            if (Value.GetType() == typeof(IPAddress))
                            {
                                string[] Array = Dns.GetHostAddresses(ReplaceValue).Select(x => x.ToString()).ToArray();
                                bool Contained = Array.Contains(Value.ToString());
                                if (Array.Contains(Value.ToString()))
                                {
                                    Value = ReplaceValue;
                                }
                            }
                            SettingsFileOutput.Add(_ThisLine.Replace(ReplaceValue, Value.ToString()));
                        }
                        else
                        {
                            SettingsFileOutput.Add(_ThisLine);
                        }
                        found = true;
                    }
                    else
                    {
                        SettingsFileOutput.Add(_ThisLine);
                        continue;
                    }
                }
                if (!found) SettingsFileOutput.Add(Key + "\t" + Value);
                Files.FileWrite("./Settings.Dat", SettingsFileOutput.ToArray());
                return true;
            }
        }
        #endregion

        #region Settings File Monitoring
        public static FileSystemWatcher SettingsFileWatcher = new FileSystemWatcher();

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static bool Monitor()
        {
            SettingsFileWatcher.Path = Directory.GetCurrentDirectory();
            SettingsFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            SettingsFileWatcher.Filter = "Settings.xlsx";
            SettingsFileWatcher.Changed += new FileSystemEventHandler(SettingsFileChanged);
            SettingsFileWatcher.EnableRaisingEvents = true;
            //Console.WriteLine("Monitoring: " + Directory.GetCurrentDirectory() + "\\Settings.Dat");
            return true;
        }

        private static void SettingsFileChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Settings file updated manually. To Reload, type \"&a/ReloadSettings&e\".");
            System.Console.ForegroundColor = ConsoleColor.White;
            SettingsFileWatcher.EnableRaisingEvents = false;
            //LoadAll();
            SettingsFileWatcher.EnableRaisingEvents = true;
        }
        #endregion

        #region Save/Load
        internal static bool _LoadDirect(string Variable, string Value)
        {
            try
            {
                string[] Broken = Variable.Split('.');
                Type TargetClass = typeof(Settings);
                for (int i = 0; i < Broken.Length; i++)
                {
                    if (i < Broken.Length - 1)
                    {
                        TargetClass = TargetClass.GetNestedType(Broken[i]);
                    }

                    else
                    {
                        FieldInfo ThisField = TargetClass.GetField(Broken[i]);
                        ThisField.SetValue(null, _Convert(ThisField.FieldType, Value, ThisField.GetValue(null)));
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        internal static object _Convert(Type ObjectType, string _Value, object Default)
        {
            try
            {
                if (ObjectType == typeof(Int16)) return Int16.Parse(_Value);
                if (ObjectType == typeof(Int32)) return Int32.Parse(_Value);
                if (ObjectType == typeof(Int64)) return Int64.Parse(_Value);
                if (ObjectType == typeof(UInt16)) return UInt16.Parse(_Value);
                if (ObjectType == typeof(UInt32)) return UInt32.Parse(_Value);
                if (ObjectType == typeof(UInt64)) return UInt64.Parse(_Value);

                if (ObjectType == typeof(Single)) return Single.Parse(_Value);
                if (ObjectType == typeof(Double)) return Double.Parse(_Value);

                if (ObjectType == typeof(Boolean)) return Boolean.Parse(_Value);

                if (ObjectType == typeof(IPAddress))
                {
                    IPAddress Out = IPAddress.Parse("127.0.0.1");
                    try
                    {
                        Out = Dns.GetHostAddresses(_Value)[0];
                    }
                    catch
                    {
                    }
                    try
                    {
                        Out = IPAddress.Parse(_Value);
                    }
                    catch
                    {
                    }
                    return Out;
                }
                if (ObjectType == typeof(String)) return _Value;

                if (ObjectType == typeof(Colors.XRGBColor)) return new Colors.XRGBColor(_Value);

                return Default;
            }
            catch
            {
                return Default;
            }
        }

        internal static object _GetDirect(string Variable)
        {
            try
            {
                string[] Broken = Variable.Split('.');
                Type TargetClass = typeof(Settings);
                for (int i = 0; i < Broken.Length; i++)
                {
                    if (i < Broken.Length - 1)
                    {
                        TargetClass = TargetClass.GetNestedType(Broken[i]);
                    }

                    else
                    {
                        FieldInfo ThisField = TargetClass.GetField(Broken[i]);
                        return ThisField.GetValue(null);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool LoadAll()
        {
            //Console.WriteLine("&6Loading Settings...");
            FileInfo File = new FileInfo("./Settings.xlsx");
            if (!File.Exists)
            {
                DumpSettingsFileTemplate("./Settings.xlsx");
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
                        if (!SettingsHandler._LoadDirect(Key, Val))
                        {
                            Log.Warning(("&3Load Fail: " + Key).Resize(System.Console.WindowWidth));
                        }
                    }

                } // the using statement calls Dispose() which closes the package.
                Console.Write("\r" + Strings.Repeat(" ", System.Console.WindowWidth - 1) + "\r");
                //Thread.Sleep(10);
                //Console.WriteLine("");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("\r&cFailed to load settings - file in use? Close Settings.xlsx and reload!");
                Console.WriteLine("");
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool SaveAll()
        {
            SettingsFileWatcher.EnableRaisingEvents = false;
            FileInfo File = new FileInfo("./Settings.xlsx");
            //This framework is a real pain in the ARSE!
            //It uses non zero based indexes!
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
                    object NewVal = SettingsHandler._GetDirect(Key);
                    if (NewVal == null)
                    {
                        Log.Warning(("&3Save Fail: " + Key).Resize(System.Console.WindowWidth));
                        NewVal = Val;
                    }
                    if (NewVal.GetType() == typeof(IPAddress))
                    {
                        IPAddress _IP = (IPAddress)NewVal;
                        IPAddress[] _Matches = new IPAddress[0];
                        bool NetIsUp = false;
                        try
                        {
                            Ping ping = new Ping();

                            //ping googles dns server to determine if the net connection is available!
                            PingReply pingStatus = ping.Send(IPAddress.Parse("8.8.8.8"));

                            if (pingStatus.Status == IPStatus.Success)
                            {
                                NetIsUp = true;
                            }
                        }
                        catch
                        {
                        }

                        try
                        {
                            if (NetIsUp) _Matches = Dns.GetHostAddresses(Val);
                        }
                        catch
                        {
                            //No net connection, can't resolve...
                        }
                        if (!NetIsUp) continue; //can't verify the IP Address, let's leave it the way it is for now...
                        if (_Matches.Select(x => x.ToString()).Contains(_IP.ToString())) continue; //no change required!
                        else worksheet.Cells[i, 2].Value = NewVal;
                        continue;
                    }
                    if (NewVal.ToString() == Val) continue;
                    worksheet.Cells[i, 2].Value = NewVal;
                }
                xlPackage.Save();

            } // the using statement calls Dispose() which closes the package.
            try
            {
                SettingsFileWatcher.EnableRaisingEvents = true;
            }
            catch
            {
                //Path not of legal form? Stop watching...
            }
            return true;
        }

        public static bool DumpSettingsFileTemplate(string Output)
        {
            try
            {
                var resourceName = "OpenYS.SettingsTemplate.xlsx";
                string[] Names = System.Reflection.Assembly.GetAssembly(typeof(Settings)).GetManifestResourceNames();

                using (Stream stream = System.Reflection.Assembly.GetAssembly(typeof(Settings)).GetManifestResourceStream(resourceName))
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
        #endregion
    }
}