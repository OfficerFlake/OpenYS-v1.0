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
        public static readonly CommandDescriptor OpenYS_Command_Weather_Info = new CommandDescriptor
        {
            _Name = "Weather",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Weather",
            _Hidden = false,

            _Descrption = "Displays current weather information",
            _Usage = "/Weather",
            _Commands = new string[] { "/Weather", "/TAF", "/ATIS" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.User_Console       |
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
            _Handler = OpenYS_Command_Weather_Info_Method,
        };

        public static bool OpenYS_Command_Weather_Info_Method(Client ThisClient, CommandReader Command)
        {
            int Angle;
            int Speed;

            string _Angle;
            string _Speed;
            string TAF;

            Angle = 90 - (int)(Math.Atan2(OpenYS.Weather.WindZ, OpenYS.Weather.WindX) / Math.PI * 180) + 180;
            while (Angle < 0) Angle += 360;
            while (Angle >= 360) Angle -= 360;

            Speed = (int)(Math.Sqrt(Math.Pow(OpenYS.Weather.WindZ, 2) + Math.Pow(OpenYS.Weather.WindX, 2)) * 1.943844492440605);

            if (Speed <= 0) Angle = 0;
            _Angle = Angle.ToString();
            if (_Angle.Length > 3) _Angle = "XXX";
            while (_Angle.Length < 3) _Angle = "0" + _Angle;
            _Speed = Speed.ToString();
            if (_Speed.Length > 2) _Speed = "99";
            while (_Speed.Length < 2) _Speed = "0" + _Speed;

            TAF = _Angle + _Speed + "KT";
            if (TAF == "00000KT") TAF = "calm";

            string DayNight = OpenYS.GetServerTimeString() + "J";
            string Vis = ((int)Math.Floor(OpenYS.Weather.Fog / 1000)).ToString();
            while (Vis.Length < 2) Vis = "0" + Vis;
            if (Vis.Length > 2) Vis = "99";
            ThisClient.SendMessage("&eWeather: " + "T-" + DayNight + " W-" + TAF + " V-" + Vis);
            return true;
        }
    }
}