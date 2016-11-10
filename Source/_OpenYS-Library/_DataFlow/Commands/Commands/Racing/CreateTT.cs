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
        public static readonly CommandDescriptor OpenYS_Command_Racing_CreateTT = new CommandDescriptor
        {
            _Name = "CreateTT",
            _Version = 1.0,
            _Date = new DateTime(2015, 12, 23),
            _Author = "OfficerFlake",

            _Category = "Racing",
            _Hidden = false,

            _Descrption = "Creates A TimeTrial session to race on.",
            _Usage = "/CreateTT [PathName] (Minutes)",
            _Commands = new string[] { "/CreateTT" },

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
            _Handler = OpenYS_Command_Racing_CreateTT_Method,
        };

        public static bool OpenYS_Command_Racing_CreateTT_Method(Client ThisClient, CommandReader Command)
        {
            #region Didn't Specify Path
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eYou need to specify a path to race on!");
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
            #region Specified Duration
            TimeSpan RaceDuration = new TimeSpan(0, 0, 0);
            if (Command._CmdArguments.Count() > 1)
            {
                double Duration = 0;
                bool Failed = !Double.TryParse(Command._CmdArguments[1], out Duration);
                if (Failed)
                {
                    ThisClient.SendMessage("Duration in Minutes given was not an integer! Durations must be in numeric format!");
                    return false;
                }
                RaceDuration = new TimeSpan(0, 0, 0, 0, (int)(Duration * 60 * 1000));
            }
            #endregion

            Games.Racing2.Race NewTT = new Games.Racing2.Race();
            NewTT.Path = World.Objects.PathList
            .Where(y => y.Identify.ToUpperInvariant() == Command._CmdArguments[0].ToUpperInvariant()).ToArray()[0];
            NewTT.Owner = ThisClient;
            NewTT.FastestSectors = new float[NewTT.Path.Points.Count];
            NewTT.CumulativeSectors = new float[NewTT.Path.Points.Count];

            NewTT.RaceTypeSetTimeTrial();
            NewTT.RaceEnds = DateTime.Now + RaceDuration;

            ThisClient.SendMessage("&eTimeTrial Created on Path: \"" + NewTT.Path.Identify + "\". To join in, type \"&c/JoinRace " + NewTT.ID + "&e\".");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&e" + ThisClient.Username + " created a TimeTrial at " + NewTT.Path.Identify + ". To join in, type \"&c/JoinRace " + NewTT.ID + "&e\".");
            Games.Racing2.Races.Add(NewTT);
            return true;
        }
    }
}