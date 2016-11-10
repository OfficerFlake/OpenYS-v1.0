using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace OpenYS
{
    public static partial class Commands
    {
        public static void LoadAll()
        {
            //Logger.Console.WriteLine("&bLoadingCommands...");
            #region NORMAL COMMANDS
            foreach (var ThisCmdDesciptorRaw in typeof(Commands).GetFields().Where(x => x.FieldType == typeof(CommandDescriptor)).ToArray())
            {
                try
                {
                    CommandDescriptor ThisCmdDescriptor = (CommandDescriptor)ThisCmdDesciptorRaw.GetValue(ThisCmdDesciptorRaw.FieldType); //(CommandDescriptor)Convert.ChangeType(ThisCmdDesciptorRaw.FieldType, typeof(CommandDescriptor));
                    //Logger.Console.WriteLine("Found: " + ThisCmdDescriptor._Name);
                    Register(ThisCmdDescriptor);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Console.WriteLine("&cOpenYS.Commands.LoadAll Failed: " + ThisCmdDesciptorRaw.Name);
                    continue;
                }
            }
            #endregion
            #region DEBUGGING COMMANDS
#if DEBUG
            foreach (var ThisCmdDesciptorRaw in typeof(Commands).GetFields().Where(x => x.FieldType == typeof(DebuggingCommandDescriptor)).ToArray())
            {
                try
                {
                    DebuggingCommandDescriptor ThisCmdDescriptor = (DebuggingCommandDescriptor)ThisCmdDesciptorRaw.GetValue(ThisCmdDesciptorRaw.FieldType); //(CommandDescriptor)Convert.ChangeType(ThisCmdDesciptorRaw.FieldType, typeof(CommandDescriptor));
                    //Logger.Console.WriteLine("Found: " + ThisCmdDescriptor._Name);
                    Register(ThisCmdDescriptor);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Console.WriteLine("&cOpenYS.Commands.LoadAll Failed: " + ThisCmdDesciptorRaw.Name);
                    continue;
                }
            }
#endif
            #endregion
            //Logger.Console.WriteLine("&bLoadingComplete.");
        }
    }
}