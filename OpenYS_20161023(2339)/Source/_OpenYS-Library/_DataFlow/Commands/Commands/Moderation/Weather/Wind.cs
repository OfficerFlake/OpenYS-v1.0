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
        public static readonly CommandDescriptor OpenYS_Command_Weather_Wind = new CommandDescriptor
        {
            _Name = "Wind",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Weather",
            _Hidden = false,

            _Descrption = "Change the server Wind speed to the specified TAF format (HHHSSKT)",
            _Usage = "/Wind ([HDG][KT](\"KT\"))",
            _Commands = new string[] { "/Wind", "/Weather.Wind" },

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
            _Handler = OpenYS_Command_Weather_Wind_Method,
        };

        public static bool OpenYS_Command_Weather_Wind_Method(Client ThisClient, CommandReader Command)
        {
            int Angle;
            int Speed;

            string _Angle;
            string _Speed;
            string TAF;
            if (Command._CmdArguments.Count() < 1)
            {
                Angle = (int)(Math.Atan2(-OpenYS.Weather.WindZ, -OpenYS.Weather.WindX) / Math.PI * 180);
                while (Angle < 0) Angle += 360;
                while (Angle >= 360) Angle -= 360;

                Speed = (int)(Math.Sqrt(Math.Pow(OpenYS.Weather.WindZ, 2) + Math.Pow(OpenYS.Weather.WindX, 2)) * 1.943844492440605);
                //ThisClient.SendMessage("&eServer Wind Angle is currently " + Angle + "deg.");
                //ThisClient.SendMessage("&eServer Wind Speed is currently " + Speed + "kt.");

                if (Speed <= 0) Angle = 0;
                _Angle = Angle.ToString();
                if (_Angle.Length > 3) _Angle = "XXX";
                while (_Angle.Length < 3) _Angle = "0" + _Angle;
                _Speed = Speed.ToString();
                if (_Speed.Length > 2) _Speed = "99";
                while (_Speed.Length < 2) _Speed = "0" + _Speed;

                TAF = _Angle + _Speed + "KT";
                if (TAF == "00000KT") TAF = "calm";
                ThisClient.SendMessage("&eServer Wind is currently " + TAF + ".");
                return true;
            }
            TAF = Command._CmdArguments[0].ToLowerInvariant();
            if (TAF == "calm") TAF = "00000KT";
            uint output = 0;
            if (!UInt32.TryParse(TAF.Substring(0, 3), out output))
            {
                ThisClient.SendMessage("&eUnrecognised value - \"" + Command._CmdArguments[0] + "\".");
                return false;
            }
            Angle = (int)output;
            while (Angle < 0) Angle += 360;
            while (Angle >= 360) Angle -= 360;
            output = 0;
            if(!UInt32.TryParse(TAF.Substring(3, 2), out output))
            {
                ThisClient.SendMessage("&eUnrecognised value - \"" + Command._CmdArguments[0] + "\".");
                return false;
            }
            Speed = (int)output;
            OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather)
            {
                WindZ = (float)((-Math.Cos((double)Angle / 180 * Math.PI)) * Speed * 0.5144444444444444),
                WindX = (float)((-Math.Sin((double)Angle / 180 * Math.PI)) * Speed * 0.5144444444444444)
            };


            TAF = Command._CmdArguments[0].ToUpperInvariant();
            if (TAF == "CALM") TAF = "calm";
            else if (TAF.Substring(3, 2) == "00") TAF = "calm";
            Clients.AllClients.SendPacket(OpenYS.Weather);
            SettingsHandler.SaveAll();
            ThisClient.SendMessage("&eServer Wind set to " + TAF + ".");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the Wind to " + TAF + ".");
            return true;
        }
    }
}