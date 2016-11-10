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
        public static readonly CommandDescriptor OpenYS_Command_Weather_Variable = new CommandDescriptor
        {
            //BROKEN
            _Name = "VariableWeather",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Adv.Weather",
            _Hidden = false,
            _Disabled = false,

            _Descrption = "Toggles whether the servers weather should update automatically.",
            _Usage = "/VariableWeather (On|Off)",
            _Commands = new string[] { "/VariableWeather", "/Weather.Variable", "/WeatherVariable", "/VW" },

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
            _Handler = OpenYS_Command_Weather_Variable_Method,
        };

        public static bool OpenYS_Command_Weather_Variable_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (OpenYS.AdvancedWeatherOptions._Weather_Variable)
                {
                    ThisClient.SendMessage("&eServer automatically updates the weather.");
                }
                else
                {
                    ThisClient.SendMessage("&eServer does not automatically update the weather.");
                }
                return false;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "ON" | Command._CmdArguments[0].ToUpperInvariant() == "TRUE")
            {
                OpenYS.AdvancedWeatherOptions._WindX = OpenYS._Weather.WindX; //m/s
                OpenYS.AdvancedWeatherOptions._WindY = OpenYS._Weather.WindY; //m/s
                OpenYS.AdvancedWeatherOptions._WindZ = OpenYS._Weather.WindZ; //m/s
                OpenYS.AdvancedWeatherOptions._Fog__ = OpenYS._Weather.Fog; //m;
                OpenYS.AdvancedWeatherOptions._WindX_DUDX1 = 0; //m/s
                OpenYS.AdvancedWeatherOptions._WindY_DUDX1 = 0; //m/s
                OpenYS.AdvancedWeatherOptions._WindZ_DUDX1 = 0; //m/s
                OpenYS.AdvancedWeatherOptions._Fog___DUDX1 = 0; //m
                //OpenYS.Weather = OpenYS._Weather; //force an update.
                OpenYS.AdvancedWeatherOptions._Weather_Variable = true;
                //Clients.AllClients.SendPacket(OpenYS.Weather);
                ThisClient.SendMessage("&aServer automatic weather updates set to &aON&a.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set automatic weather updates to &aON&a.");
                SettingsHandler.SaveAll();
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "OFF" | Command._CmdArguments[0].ToUpperInvariant() == "FALSE")
            {
                OpenYS.AdvancedWeatherOptions._Weather_Variable = false;
                Clients.AllClients.SendPacket(OpenYS.Weather);
                ThisClient.SendMessage("&aServer automatic weather updates set to &cOFF&a.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set automatic weather updates to &cOFF&a.");
                SettingsHandler.SaveAll();
                return true;
            }
            ThisClient.SendMessage("&eUnrecognised option - \"" + Command._CmdArguments[0] + "\".");
            return false;
        }
    }
}