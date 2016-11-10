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
        public static readonly CommandDescriptor OpenYS_Command_Weather_Turbulemce = new CommandDescriptor
        {
            _Name = "Turbulence",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Adv.Weather",
            _Hidden = false,

            _Descrption = "Toggles whether the server should automatically add turbulence to weather packets.",
            _Usage = "/Turbulence (Percentage)",
            _Commands = new string[] { "/Turbulence", "/Weather.Turbulence", "/Gust", "/Gusts" },

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
            _Handler = OpenYS_Command_Weather_Turbulemce_Method,
        };

        public static bool OpenYS_Command_Weather_Turbulemce_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (OpenYS.AdvancedWeatherOptions._Turbulence > 0)
                {
                    ThisClient.SendMessage("&eServer turbulence is " + OpenYS.AdvancedWeatherOptions._Turbulence + ".");
                }
                else
                {
                    ThisClient.SendMessage("&eServer disables turbulence.");
                }
                return false;
            }
            uint ID = 0;
            if (!UInt32.TryParse(Command._CmdArguments[0], out ID)) {
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an integer value!");
                return false;
            }
            if (ID > 0)
            {
                OpenYS.AdvancedWeatherOptions._Turbulence = (int)ID;
                ThisClient.SendMessage("&aServer turbulence set to &a" + ID + "&a percent.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set turbulence to &a" + ID + "&a percent.");
                SettingsHandler.SaveAll();
                return true;
            }
            else
            {
                OpenYS.AdvancedWeatherOptions._Turbulence = 0;
                ThisClient.SendMessage("&aServer turbulence set to &cOFF&a.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set turbulence to &cOFF&a.");
                SettingsHandler.SaveAll();
                return true;
            }
        }
    }
}