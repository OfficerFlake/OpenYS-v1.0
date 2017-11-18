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
        public static readonly CommandDescriptor OpenYS_Command_Options_ConsoleName = new CommandDescriptor
        {
            _Name = "ConsoleName",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Changes the console name.",
            _Usage = "/ConsoleName (NewName)",
            _Commands = new string[] { "/ConsoleName", "/Options.ConsoleName", },

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
            _Handler = OpenYS_Command_Options_ConsoleName_Method,
        };

        public static bool OpenYS_Command_Options_ConsoleName_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eCurrent server name: &2" + OpenYS.OpenYSConsole.Username + "&e.");
                return false;
            }
            OpenYS.OpenYSConsole.Username = Command._CmdRawArguments;
            SettingsHandler.SaveAll();
            ThisClient.SendMessage("&aServer name is now: &2" + OpenYS.OpenYSConsole.Username + "&a.");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the server name to: &2" + OpenYS.OpenYSConsole.Username + "&a.");
            return true;
        }
    }
}