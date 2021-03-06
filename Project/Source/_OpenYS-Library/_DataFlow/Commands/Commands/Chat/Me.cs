﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace OpenYS
{
    public static partial class Commands
    {
        public static readonly CommandDescriptor OpenYS_Command_Chat_Me = new CommandDescriptor
        {
            _Name = "Me",
            _Version = 1.0,
            _Date = new DateTime(2013, 10, 01),
            _Author = "OfficerFlake",

            _Category = "Chat",
            _Hidden = false,

            _Descrption = "Broadcasts a third person message to the server.",
            _Usage = "/Me (Message)",
            _Commands = new string[] { "/Me" },

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
            _Handler = OpenYS_Command_Chat_Me_Method,
        };

        public static bool OpenYS_Command_Chat_Me_Method(Client ThisClient, CommandReader Command)
        {
            Clients.AllClients.SendMessage("&d* " + ThisClient.Username + " " + Command._CmdRawArguments);
            return true;
        }
    }
}