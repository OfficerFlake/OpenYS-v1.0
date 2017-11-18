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
        public static readonly CommandDescriptor OpenYS_Command_Replay_Play = new CommandDescriptor
        {
            _Name = "Replay Play",
            _Version = 1.0,
            _Date = new DateTime(2015, 08, 02),
            _Author = "OfficerFlake",

            _Category = "Replay",
            _Hidden = false,

            _Descrption = "Streams a loaded YFS Replay on the server.",
            _Usage = "/ReplayPlay",
            _Commands = new string[] { "/ReplayPlay", "/Replay.Play", "/PlayReplay" },

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
            _Handler = OpenYS_Command_Replay_Play_Method,
        };

        public static bool OpenYS_Command_Replay_Play_Method(Client ThisClient, CommandReader Command)
        {
            if (YSFlightReplays.ServerReplay.IsLoading())
            {
                ThisClient.SendMessage("&eThe Replay is still loading, please wait.");
                return false;
            }
            if (YSFlightReplays.ServerReplay.IsLoaded() & !YSFlightReplays.ServerReplay.IsPlaying())
            {
                YSFlightReplays.ServerReplay.PlayReplay();
                ThisClient.SendMessage("&aPlaying the record...");
                return true;
            }
            if (YSFlightReplays.ServerReplay.IsLoaded() & YSFlightReplays.ServerReplay.IsPlaying())
            {
                ThisClient.SendMessage("&eThe record is already playing...");
                return false;
            }
            return false;
        }
    }
}