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
        public static readonly CommandDescriptor OpenYS_Command_Racing_CreateRace = new CommandDescriptor
        {
            _Name = "CreateRace",
            _Version = 1.0,
            _Date = new DateTime(2015, 12, 23),
            _Author = "OfficerFlake",

            _Category = "Racing",
            _Hidden = false,

            _Descrption = "Creates A Race session to race on.",
            _Usage = "/CreateRace [PathName] (LapCount)",
            _Commands = new string[] { "/CreateRace" },

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
            Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Racing_CreateRace_Method,
        };

        public static bool OpenYS_Command_Racing_CreateRace_Method(Client ThisClient, CommandReader Command)
        {
            #region Didn't Specify Path
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eYou need to specify a path to race on!");
                return false;
            }
            #endregion
            #region Didn't Specify Lap Count
            if (Command._CmdArguments.Count() < 2)
            {
                ThisClient.SendMessage("&eYou need to specify how many laps to race for!");
                return false;
            }
            #endregion
            #region Try Convert Lap Count
            int LapCount = 0;
            bool Failed = !Int32.TryParse(Command._CmdArguments[1], out LapCount);
            #endregion
            #region Lap Count Not Integer
            if (Failed)
            {
                ThisClient.SendMessage("Lap Count given was not an integer! Lap Counts's must be in numeric format!");
                return false;
            }
            #endregion
            #region Don't Have the Path
            if (!World.Objects.PathList
                .Where(y=>y.Type == 15)
                .Select(x => x.Identify.ToUpperInvariant())
                .Contains(Command._CmdArguments[0]
                .ToUpperInvariant()))
            {
                ThisClient.SendMessage("Path not found - use /ListPaths to see compatible racing paths!");
                return false;
            }
            #endregion

            Games.Racing2.Race NewRace = new Games.Racing2.Race();
            NewRace.Path = World.Objects.PathList
            .Where(y => y.Identify.ToUpperInvariant() == Command._CmdArguments[0].ToUpperInvariant()).ToArray()[0];
            NewRace.Owner = ThisClient;
            NewRace.FastestSectors = new float[NewRace.Path.Points.Count];
            NewRace.CumulativeSectors = new float[NewRace.Path.Points.Count];

            NewRace.RaceTypeSetRace();
            NewRace.TotalLapCount = LapCount;

            ThisClient.SendMessage("&eRace Created on Path: \"" + NewRace.Path.Identify + "\". To join in, type \"&c/JoinRace " + NewRace.ID + "&e\".");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&e" + ThisClient.Username + " created a Race at " + NewRace.Path.Identify + ". To join in, type \"&c/JoinRace " + NewRace.ID + "&e\".");
            Games.Racing2.Races.Add(NewRace);
            return true;
        }
    }
}