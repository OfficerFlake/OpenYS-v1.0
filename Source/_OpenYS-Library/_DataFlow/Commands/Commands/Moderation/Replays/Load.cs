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
        public static readonly CommandDescriptor OpenYS_Command_Replay_Load = new CommandDescriptor
        {
            _Name = "Replay Load",
            _Version = 1.0,
            _Date = new DateTime(2015, 08, 02),
            _Author = "OfficerFlake",

            _Category = "Replay",
            _Hidden = false,

            _Descrption = "Loads a YFS Replay to be run on the server.",
            _Usage = "/ReplayLoad (FileName)",
            _Commands = new string[] { "/ReplayLoad", "/Replay.Load", "/LoadReplay" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                Requirement.User_Virtual         |
                //Requirement.User_YSFlight      |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                //Requirement.Status_Connected   |
                //Requirement.Status_Flying      |
                //Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Replay_Load_Method,
        };

        public static bool OpenYS_Command_Replay_Load_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (YSFlightReplays.ServerReplay.IsLoading())
                {
                    ThisClient.SendMessage("&eThe Replay is still loading, please wait.");
                    return false;
                }
                if (YSFlightReplays.ServerReplay.IsLoaded())
                {
                    ThisClient.SendMessage("&eThe Replay has finished load. Play with &c/ReplayPlay&e.");
                    return false;
                }
            }
            if (Command._CmdArguments.Count() > 0)
            {
                if (YSFlightReplays.ServerReplay.IsLoading())
                {
                    ThisClient.SendMessage("&eThe Replay is still loading, you can not load another replay until it is done.");
                    return false;
                }
                if (!YSFlightReplays.ServerReplay.IsStopped())
                {
                    ThisClient.SendMessage("&eThe Replay is still streaming, you can not load another replay until it is stopped. You can stop early with &c/ReplayStop&e.");
                    return false;
                }

                //Ready to load.

                string FileName = Command._CmdArguments[0];
                if (!File.Exists("./Replays/" + FileName + ".YFS"))
                {
                    ThisClient.SendMessage("&cCould not find the replay file... aborting.");
                    return false;
                }
                else
                {
                    YSFlightReplays.ServerReplay.LoadReplay("./Replays/" + FileName +".YFS");
                    return true;
                }
            }
            return false;
        }
    }
}