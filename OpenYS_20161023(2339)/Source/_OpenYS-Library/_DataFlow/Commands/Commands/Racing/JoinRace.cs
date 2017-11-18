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
        public static readonly CommandDescriptor OpenYS_Command_Racing_JoinRace = new CommandDescriptor
        {
            _Name = "JoinRace",
            _Version = 1.0,
            _Date = new DateTime(2015, 12, 23),
            _Author = "OfficerFlake",

            _Category = "Racing",
            _Hidden = false,

            _Descrption = "Joins a Race session.",
            _Usage = "/JoinRace [RaceID]",
            _Commands = new string[] { "/JoinRace" },

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
            //Requirement.Status_Flying |
            //Requirement.Status_NotFlying   |
            Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Racing_JoinRace_Method,
        };

        public static bool OpenYS_Command_Racing_JoinRace_Method(Client ThisClient, CommandReader Command)
        {
            #region Didn't Specify Race ID
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&aYou need to specify a race id to join!");
                return false;
            }
            #endregion
            #region Convert Race ID.
            bool Failed = false;
            int TargetRace = 0;
            Failed = !Int32.TryParse(Command._CmdArguments[0], out TargetRace);
            #endregion
            #region Race ID not an Integer
            if (Failed)
            {
                ThisClient.SendMessage("RaceID given was not an integer! RaceID's must be in numeric format!");
                return false;
            }
            #endregion
            #region Race ID not found
            if (Games.Racing2.Races.Where(x => x.ID == TargetRace).Count() < 1)
            {
                ThisClient.SendMessage("No Race with that ID exists!");
                return false;
            }
            #endregion
            #region Race Already Started!
            Games.Racing2.Race JoinedRace = Games.Racing2.Races.Where(x => x.ID == TargetRace).ToArray()[0];
            if (!JoinedRace.Status.IsNotStarted() & !JoinedRace.RaceTypeIsTimeTrial())
            {
                ThisClient.SendMessage("That race has already been started!");
                return false;
            }
            #endregion
            Games.Racing2.RaceDescription._Racer ThisRacer = JoinedRace.AddRacer(ThisClient);
            ThisClient.SendMessage("Added you to the race! Have fun!");
            return true;
        }
    }
}