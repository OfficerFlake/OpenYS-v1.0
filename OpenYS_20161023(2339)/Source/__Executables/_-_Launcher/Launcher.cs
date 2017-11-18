using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OpenYS
{
    class Launcher
    {
        static void Main(string[] args)
        {
            Console.Title = "OpenYS Development Launcher";
            #region UNDEFINED
#if UNDEFINED
            Console.Title = "OH SNAP!";
            Console.CursorVisible = false;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("WARNING!");
            Console.WriteLine("========");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("STARTUP PROJECT NOT SELECTED!");
            Console.WriteLine("");
            Console.WriteLine("DON'T KNOW WHICH PROGRAM TO RUN! CLIENT? SERVER? PREBUILD?");
            Console.WriteLine("PLEASE LOOK AT SOLUTION CONFIGURATION AND SELECT RUN/IGNORE!");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
#endif
            #endregion
            #region CLIENTMODE
#if _0_OYS_CLIENT
            Directory.SetCurrentDirectory("../../../_1_OpenYS-ClientMode/bin/");
            OpenYS_Client.Program.Launcher(args);
            return;
#endif
            #endregion
            #region SERVERMODE
#if _0_OYS_SERVER
            Directory.SetCurrentDirectory("../../../_1_OpenYS-ServerMode/bin/");
            OpenYS_Server.Program.Launcher(args);
            return;
#endif
            #endregion
            #region PRERELEASE
#if _1_PRERELEASE
            Directory.SetCurrentDirectory("../../../_1_OpenYS-ServerMode/bin/");
            PreRelease.Main(args);
            return;
#endif
            #endregion
            #region RELEASE
#if _2_RELEASE
            Directory.SetCurrentDirectory("../../../_1_OpenYS-ServerMode/bin/");
            Release.Main(args);
            return;
#endif
            #endregion
            #region SETTINGSGUI
#if _3_SETTINGSGUI
            Directory.SetCurrentDirectory("../../../_3_SettingsGUI/bin/");
            SettingsGUI.Launcher(args);
            return;
#endif
            #endregion
            #region TESTING
#if _X_TESTING
            Directory.SetCurrentDirectory("../../../_X_TESTING/bin/");
            OpenYS_Testing.Program.Launcher(args);
            return;
#endif
            #endregion
        }
    }
}