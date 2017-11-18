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
        public static readonly CommandDescriptor OpenYS_Command_Flight_SmokeColor = new CommandDescriptor
        {
            _Name = "SmokeColor",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Flight",
            _Hidden = false,

            _Descrption = "Change your current Smoke Color.",
            _Usage = "/SmokeColor [GeneratorID|\"ALL\"] [Red] [Green] [Blue]",
            _Commands = new string[] { "/Flight.SmokeColor", "/SmokeColor" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.User_Console       |
                Requirement.User_YSFlight |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                Requirement.Status_Connected |
                Requirement.Status_Flying |
                //Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Flight_SmokeColor_Method,
        };

        public static bool OpenYS_Command_Flight_SmokeColor_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 4)
            {
                ThisClient.SendMessage("&eFormat incorrect: Use instead: \"/Smoke Index Red Green Blue\"!");
                return false;
            }

            byte Index = 0;
            byte Red = 255;
            byte Green = 255;
            byte Blue = 255;
            bool DoAll = false;

            if (Command._CmdArguments[0].ToUpperInvariant() == "ALL") DoAll = true;
            if (!DoAll & !Byte.TryParse(Command._CmdArguments[0], out Index))
            {
                ThisClient.SendMessage("&eGenerator Format incorrect: Be sure you are using values between 0 and 255!");
                return false;
            }

            if (!Byte.TryParse(Command._CmdArguments[1], out Red) & !Byte.TryParse(Command._CmdArguments[2], out Green) & !Byte.TryParse(Command._CmdArguments[3], out Blue))
            {
                ThisClient.SendMessage("&eSmoke Color Format incorrect: Be sure you are using values between 0 and 255!");
                return false;
            }

            if (DoAll)
            {
                for (byte i = 0; i < 8; i++)
                {
                    Packets.Packet_07_SmokeColor SmokeColor = new Packets.Packet_07_SmokeColor(ThisClient.Vehicle.ID, i, Red, Green, Blue);
                    Clients.AllClients.SendPacket(SmokeColor);
                }
            }
            else
            {
                Packets.Packet_07_SmokeColor SmokeColor = new Packets.Packet_07_SmokeColor(ThisClient.Vehicle.ID, Index, Red, Green, Blue);
                Clients.AllClients.SendPacket(SmokeColor);
            }
            ThisClient.SendMessage("&eSmoke Color is now: " + Red + " " + Green + " " + Blue + ".");
            return true;
        }
    }
}