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
        public static readonly CommandDescriptor OpenYS_Command_Weather_Night = new CommandDescriptor
        {
            _Name = "Night",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Weather",
            _Hidden = true,

            _Descrption = "Change the server time to Night.",
            _Usage = "/Night",
            _Commands = new string[] { "/Night", "/Weather.Night" },

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
            _Handler = OpenYS_Command_Weather_Night_Method,
        };

        public static bool OpenYS_Command_Weather_Night_Method(Client ThisClient, CommandReader Command)
        {
            OpenYS.SetServerTimeTicks(0);
            Settings.Weather.Time = 0000;
            SettingsHandler.SaveAll();
            //OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now;
            ThisClient.SendMessage("&aServer time set to NIGHT.");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the time to NIGHT.");
            return true;
        }
    }
}