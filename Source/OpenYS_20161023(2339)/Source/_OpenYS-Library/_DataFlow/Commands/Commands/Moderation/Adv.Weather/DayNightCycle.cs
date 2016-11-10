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
        public static readonly CommandDescriptor OpenYS_Command_Weather_DNCycle = new CommandDescriptor
        {
            _Name = "DayNightCycle",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Adv.Weather",
            _Hidden = false,

            _Descrption = "Toggles whether the server should automatically switch between day/night.",
            _Usage = "/DayNightCycle (Duration)",
            _Commands = new string[] { "/DayNightCycle", "/Weather.DayNightCycle", "/SunTime", "/DayLength" },

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
            _Handler = OpenYS_Command_Weather_DNCycle_Method,
        };

        public static bool OpenYS_Command_Weather_DNCycle_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (OpenYS.AdvancedWeatherOptions._DayNightCycleLength > 0)
                {
                    ThisClient.SendMessage("&eServer enables its day/night cycle (" + OpenYS.AdvancedWeatherOptions._DayNightCycleLength + ").");
                }
                else
                {
                    ThisClient.SendMessage("&eServer disables its day/night cycle.");
                }
                return false;
            }
            uint ID = 0;
            if (!UInt32.TryParse(Command._CmdArguments[0], out ID))
            {
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an integer value!");
                return false;
            }
            if (ID > 0)
            {
                uint CurrentTick = OpenYS.GetServerTimeTicks();
                OpenYS.AdvancedWeatherOptions._DayNightCycleLength = (int)ID;
                OpenYS.SetServerTimeTicks((uint)CurrentTick);
                Random ThisRandom = new Random();
                ThisClient.SendMessage("&aServer day/night cycle set to &a" + ID + "&a Minutes.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set day/night cycle to &a" + ID + "&a Minutes.");
                if (OpenYS.GetServerTimeTicks() > 7000 && OpenYS.GetServerTimeTicks() < 19000)
                {
                    if (ThisRandom.Next(1, 100) == 99) Clients.AllClients.SendMessage("&6Dawn of the first day. (Lol, YSZelda.)");
                    else Clients.AllClients.SendMessage("&6The sun rises... (Day/Night Sequencer Started!)");
                }
                else
                {
                    if (ThisRandom.Next(1, 100) == 99) Clients.AllClients.SendMessage("&5The Dark (k)Night rises. (Lol, YSBatman.)");
                    else Clients.AllClients.SendMessage("&5The sun sets... (Day/Night Sequencer Started!)");
                }
                if (OpenYS.GetServerTimeTicks() > 7000 && OpenYS.GetServerTimeTicks() < 19000) OpenYS._Weather.Lighting = 0;
                else OpenYS._Weather.Lighting = 65537;
                OpenYS.UpdateWeather();
                SettingsHandler.SaveAll();
                return true;
            }
            else
            {
                uint CurrentTick = OpenYS.GetServerTimeTicks();
                OpenYS.AdvancedWeatherOptions._DayNightCycleLength = 0;
                OpenYS.SetServerTimeTicks((uint)CurrentTick);
                ThisClient.SendMessage("&aServer day/night cycle set to &cOFF&a.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set day/night cycle to &cOFF&a.");
                SettingsHandler.SaveAll();
                return true;
            }
        }
    }
}