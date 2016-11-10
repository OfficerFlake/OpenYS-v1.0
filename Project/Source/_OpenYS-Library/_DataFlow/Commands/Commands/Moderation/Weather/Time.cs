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
        public static readonly CommandDescriptor OpenYS_Command_Weather_Time = new CommandDescriptor
        {
            _Name = "Time",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Weather",
            _Hidden = false,

            _Descrption = "Change the server time.",
            _Usage = "/Time (HHMM|Dawn|Day|Dusk|Night)",
            _Commands = new string[] { "/Time", "/Weather.Time" },

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
            _Handler = OpenYS_Command_Weather_Time_Method,
        };

        public static bool OpenYS_Command_Weather_Time_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eServer time is currently " + OpenYS.GetServerTimeString() + "J.");
                return false;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "DAY")
            {
                OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { Lighting = 0 };
                int incrementticks = (int)(12 * 1000) + (int)Math.Round(0 * 1000d / 60d);
                TimeSpan Adjustment = new TimeSpan(0, 0, 0, 0, (int)(incrementticks / 24000d * OpenYS.AdvancedWeatherOptions._DayNightCycleLength * 60 * 1000d));
                OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now - Adjustment;
                Clients.AllClients.SendPacket(OpenYS.Weather);
                ThisClient.SendMessage("&aServer time set to 1200J.");
                Settings.Weather.Time = 1200;
                SettingsHandler.SaveAll();
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the time to 1200J.");
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "NIGHT")
            {
                OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { Lighting = 65537 };
                int incrementticks = (int)(0 * 1000) + (int)Math.Round(0 * 1000d / 60d);
                TimeSpan Adjustment = new TimeSpan(0, 0, 0, 0, (int)(incrementticks / 24000d * OpenYS.AdvancedWeatherOptions._DayNightCycleLength * 60 * 1000d));
                OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now - Adjustment;
                Clients.AllClients.SendPacket(OpenYS.Weather);
                ThisClient.SendMessage("&aServer time set to 0000J.");
                Settings.Weather.Time = 0000;
                SettingsHandler.SaveAll();
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the time to 0000J.");
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "DAWN")
            {
                OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { Lighting = 0 };
                int incrementticks = (int)(6 * 1000) + (int)Math.Round(0 * 1000d / 60d);
                TimeSpan Adjustment = new TimeSpan(0, 0, 0, 0, (int)(incrementticks / 24000d * OpenYS.AdvancedWeatherOptions._DayNightCycleLength * 60 * 1000d));
                OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now - Adjustment;
                Clients.AllClients.SendPacket(OpenYS.Weather);
                ThisClient.SendMessage("&aServer time set to 0600J.");
                Settings.Weather.Time = 0600;
                SettingsHandler.SaveAll();
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the time to 0600J.");
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "DUSK")
            {
                OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { Lighting = 65537 };
                Clients.AllClients.SendPacket(OpenYS.Weather);
                SettingsHandler.SaveAll();
                OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now;
                int incrementticks = (int)(18 * 1000) + (int)Math.Round(0 * 1000d / 60d);
                TimeSpan Adjustment = new TimeSpan(0, 0, 0, 0, (int)(incrementticks / 24000d * OpenYS.AdvancedWeatherOptions._DayNightCycleLength * 60 * 1000d));
                OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now - Adjustment;
                ThisClient.SendMessage("&aServer time set to 1800J.");
                Settings.Weather.Time = 1800;
                SettingsHandler.SaveAll();
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the time to 1800J.");
                return true;
            }
            try
            {
                String Input = Command._CmdArguments[0];
                if (Input.Length == 5 && Input.ToUpperInvariant().EndsWith("J"))
                {
                    Input = Input.Substring(0, Input.Length - 1);
                }
                if (Input.Length != 4)
                {
                    ThisClient.SendMessage("&eTime must be in 24 Hour Format!");
                    return false;
                }
                uint hours = UInt32.Parse(Input.ToUpperInvariant().Substring(0, 2));
                uint minutes = UInt32.Parse(Input.ToUpperInvariant().Substring(2, 2));
                if (hours >= 24)
                {
                    ThisClient.SendMessage("&aServer Time Invalid! Must be between 0000 and 2359!");
                }
                if (minutes >= 60)
                {
                    ThisClient.SendMessage("&aServer Time Invalid! Minutes >= 60???");
                }
                uint incrementticks = (hours * 1000) + (uint)Math.Round(minutes * 1000d / 60d);
                OpenYS.SetServerTimeTicks(incrementticks);
                Settings.Weather.Time = OpenYS.GetServerTimeUint();
                SettingsHandler.SaveAll();
                ThisClient.SendMessage("&aServer time set to " + OpenYS.GetServerTimeString() + "J.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the time to " + OpenYS.GetServerTimeString() + "J.");
                return true;
            }
            catch
            {
            }
            ThisClient.SendMessage("&eUnrecognised time - \"" + Command._CmdArguments[0] + "\".");
            return false;
        }
    }
}