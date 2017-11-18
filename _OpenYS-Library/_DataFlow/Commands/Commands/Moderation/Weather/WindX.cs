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
        public static readonly CommandDescriptor OpenYS_Command_Weather_WindX = new CommandDescriptor
        {
            _Name = "WindX",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Weather",
            _Hidden = false,

            _Descrption = "Change the server Wind speed (m/s) on the X axis (East/West).",
            _Usage = "/WindX (Speed)",
            _Commands = new string[] { "/WindX", "/Weather.WindX" },

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
            _Handler = OpenYS_Command_Weather_WindX_Method,
        };

        public static bool OpenYS_Command_Weather_WindX_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eServer WindX speed is currently " + OpenYS.Weather.WindX + "m/s.");
                return true;
            }

            float output = 0;
            if (!Single.TryParse(Command._CmdArguments[0], out output))
            {
                ThisClient.SendMessage("&eUnrecognised value - \"" + Command._CmdArguments[0] + "\".");
                return false;
            }
            OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { WindX = output };
            Clients.AllClients.SendPacket(OpenYS.Weather);
            SettingsHandler.SaveAll();
            ThisClient.SendMessage("&eServer WindX speed set to " + output + "m/s.");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the WindX speed to " + output + "m/s.");
            return true;
        }
    }
}