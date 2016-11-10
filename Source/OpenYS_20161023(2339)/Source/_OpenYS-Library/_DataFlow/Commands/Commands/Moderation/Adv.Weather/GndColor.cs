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
        public static readonly CommandDescriptor OpenYS_Command_Weather_GndColor = new CommandDescriptor
        {
            _Name = "GndColor",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Change the current Ground Color.",
            _Usage = "/GndColor ([Red] [Green] [Blue])",
            _Commands = new string[] { "/GndColor", "/GroundColor" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.User_Console       |
                //Requirement.User_YSFlight |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                //Requirement.Status_Connected |
                //Requirement.Status_Flying |
                //Requirement.Status_NotFlying   |
                Requirement.Permission_OP |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Weather_GndColor_Method,
        };

        public static bool OpenYS_Command_Weather_GndColor_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eFormat incorrect: Use instead: \"/GroundColor Red Green Blue\"!");
                return false;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "DEFAULT" | Command._CmdArguments[0].ToUpperInvariant() == "OFF")
            {
                Settings.Weather.Advanced.EnableGndColor = false;
                ThisClient.SendMessage("&eGroundColor turned &cOFF&e.");
                SettingsHandler.SaveAll();
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "ON")
            {
                Settings.Weather.Advanced.EnableGndColor = true;
                ThisClient.SendMessage("&eGroundColor turned &aON&e.");
                SettingsHandler.SaveAll();
                return true;
            }
            if (Command._CmdArguments.Count() < 3)
            {
                ThisClient.SendMessage("&eFormat incorrect: Use instead: \"/GroundColor Red Green Blue\"!");
                return false;
            }

            byte Red = 255;
            byte Green = 255;
            byte Blue = 255;

            if (!Byte.TryParse(Command._CmdArguments[0], out Red) & !Byte.TryParse(Command._CmdArguments[1], out Green) & !Byte.TryParse(Command._CmdArguments[2], out Blue))
            {
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using values between 0 and 255!");
                return false;
            }

            Settings.Weather.Advanced.EnableGndColor = true;
            OpenYS.AdvancedWeatherOptions.GndColor = new Colors.XRGBColor(Red, Green, Blue);
            Packets.Packet_50_GroundColor GndColor = new Packets.Packet_50_GroundColor(Red, Green, Blue);
            Clients.AllClients.SendPacket(GndColor);
            ThisClient.SendMessage("&eGround Color is now: " + GndColor.Red + " " + GndColor.Green + " " + GndColor.Blue + ".");
            SettingsHandler.SaveAll();
            return true;
        }
    }
}