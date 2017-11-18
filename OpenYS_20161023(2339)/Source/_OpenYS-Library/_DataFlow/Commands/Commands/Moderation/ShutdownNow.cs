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
        public static readonly CommandDescriptor OpenYS_Command_Options_ShutdownNow = new CommandDescriptor
        {
            _Name = "ShutdownNow",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = true,

            _Descrption = "Shutdown the OpenYS Server.",
            _Usage = "/ShutdownNow",
            _Commands = new string[] { "/ShutdownNow", },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                Requirement.Permission_OP |
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
            _Handler = OpenYS_Command_Options_ShutdownNow_Method,
        };

        public static bool OpenYS_Command_Options_ShutdownNow_Method(Client ThisClient, CommandReader Command)
        {
            //ThisClient.SendMessage("&c!!! &fServer Restarting &c!!!");
            //ThisClient.SendMessage("");
            Console.Locked = true;            
            Clients.AllClients.Exclude(ThisClient).SendMessage("&c!!! &f" + ThisClient.Username + " is shutting down the server &c!!!");
            //OpenYS.ResetTimerThread.Abort();
            OpenYS.ShutdownNow();
            return true;
        }
    }
}