using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace OpenYS
{
    public static partial class Commands
    {
        public static readonly CommandDescriptor OpenYS_Command_Moderation_Version = new CommandDescriptor
        {
            _Name = "Version",
            _Version = 1.0,
            _Date = new DateTime(2015, 08, 05),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = true,

            _Descrption = "See which version of OYS the server is running.",
            _Usage = "/Version",
            _Commands = new string[] { "/Version", "/OYS.Version", "/OpenYS.Version", "/OYSVersion", "/VersionOYS" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.User_Console |
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
            _Handler = OpenYS_Command_Moderation_Version_Method,
        };

        public static bool OpenYS_Command_Moderation_Version_Method(Client ThisClient, CommandReader Command)
        {
            ThisClient.SendMessage("&eOpenYS Version: " + Environment.GetCompilationDate());
            return true;
        }
    }
}