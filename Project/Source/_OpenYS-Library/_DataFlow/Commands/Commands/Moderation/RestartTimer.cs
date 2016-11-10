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
        public static readonly CommandDescriptor OpenYS_Command_Options_RestartTimer = new CommandDescriptor
        {
            _Name = "RestartTimer",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Toggles the servers restart timer duration.",
            _Usage = "/RestartTimer (Duration)",
            _Commands = new string[] { "/RestartTimer", "/Options.RestartTimer" },

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
            _Handler = OpenYS_Command_Options_RestartTimer_Method,
        };

        public static bool OpenYS_Command_Options_RestartTimer_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (Settings.Server.RestartTimer > 0)
                {
                    ThisClient.SendMessage("&eServer Restart Timer is set to " + Settings.Server.RestartTimer + " minutes.");
                    ThisClient.SendMessage("&eServer is schedualed to restart in " + (Settings.Server.RestartTimer - (int)((DateTime.Now - OpenYS.Time_ResetTimerStarted).TotalMinutes)) + " minutes.");
                    return true;
                }
                if (Settings.Server.RestartTimer == 0)
                {
                    ThisClient.SendMessage("&eServer Restart Timer is disabled.");
                    return true;
                }
                else
                {
                    ThisClient.SendMessage("&eServer is restarting.");
                }
                return false;
            }
            uint output = 0;
            if (!UInt32.TryParse(Command._CmdArguments[0], out output))
            {
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an integer value!");
                return false;
            }
            //OpenYS.ResetTimerThread.Abort();
            Clients.AllClients.SendMessage("&6*** Current Server Restart Timer Stopped! ***");
            Settings.Server.RestartTimer = (int)output;
            if (Settings.Server.RestartTimer == 0)
            {
                ThisClient.SendMessage("&aServer Restart Timer &cDISABLED&a.");
                SettingsHandler.SaveAll();
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " &cDISABLED&a the Server Restart Timer.");
            }
            else
            {
                ThisClient.SendMessage("&aServer Restart Timer set to " + Settings.Server.RestartTimer + " minutes.");
                SettingsHandler.SaveAll();
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the Server Restart Timer to " + Settings.Server.RestartTimer + " minutes.");
            }
            return true;
        }
    }
}