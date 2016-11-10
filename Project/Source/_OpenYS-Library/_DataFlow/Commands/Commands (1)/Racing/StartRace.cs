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
        public static readonly CommandDescriptor OpenYS_Command_Racing_StartRace = new CommandDescriptor
        {
            _Name = "StartRace",
            _Version = 1.0,
            _Date = new DateTime(2015, 12, 23),
            _Author = "OfficerFlake",

            _Category = "Racing",
            _Hidden = false,

            _Descrption = "Starts the race session and blocks new users from joining.",
            _Usage = "/StartRace [RaceID]",
            _Commands = new string[] { "/StartRace" },

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
            _Handler = OpenYS_Command_Racing_StartRace_Method,
        };

        public static bool OpenYS_Command_Racing_StartRace_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eYou need to specify a race id to start!");
                return false;
            }
            bool Failed = false;
            int TargetRace = 0;
            Failed = !Int32.TryParse(Command._CmdArguments[0], out TargetRace);
            if (Failed) ThisClient.SendMessage("&eRaceID given was not an integer! RaceID's must be in numeric format!");
            bool NoRace = false;
            if (Games.Racing2.Races.Where(x => x.ID == TargetRace).Count() < 1)
            {
                NoRace = true;
            }
            Failed |= (NoRace);
            if (Failed)
            {
                ThisClient.SendMessage("&eNo Race found with that ID!");
                return false;
            }
            if (!NoRace)
            {
                Games.Racing2.Race StartedRace = Games.Racing2.Races.Where(x => x.ID == TargetRace).ToArray()[0];
                if (ThisClient != StartedRace.Owner)
                {
                    ThisClient.SendMessage("&eOnly " + StartedRace.Owner + " can start the race!");
                    return false;
                }
                Debug.TestPoint();
                //switch (StartedRace.Status.CurrentState.State)
                //{
                //    case Games.Racing2.RaceDescription._Status._RaceStateFlags.NotStarted:
                //        if (StartedRace.RaceTypeIsTimeTrial()) goto case Games.Racing2.RaceDescription._Status._RaceStateFlags.PreStart;
                //        ThisClient.SendMessage("&eThe race is not yet prestarted! Prestart the race first!");
                //        return false;
                //    case Games.Racing2.RaceDescription._Status._RaceStateFlags.PreStart:
                //        if (StartedRace.RaceTypeIsRace())
                //        {
                //            #region Countdown Start
                //            Threads.Add(() =>
                //            {
                //                List<Client> ClientsInRace = StartedRace.Racers.Select(x => x.Client).ToList();
                //                ClientsInRace.SendMessage("&eThe Race will start shortly! Ensure you are completely stopped or you will get a jump start penalty!");
                //                Clients.AllClients.Except(ClientsInRace).ToList().SendMessage("&eRace " + StartedRace.ID + " is about to begin!");
                //                Thread.Sleep(7000);
                //                StartedRace.EnableJumpStartDetection = true;
                //                ClientsInRace.SendMessage("&e3");
                //                Thread.Sleep(1000);
                //                ClientsInRace.SendMessage("&e2");
                //                Thread.Sleep(1000);
                //                ClientsInRace.SendMessage("&e1");
                //                Thread.Sleep(1000);
                //                foreach (Games.Racing2.RaceDescription._Racer ThisRacer in StartedRace.Racers)
                //                {
                //                    ThisRacer.TimeStamp_LapStarted = ThisRacer.Client.Vehicle.TimeStamp;
                //                    ThisRacer.TimeStamp_LastCheckpoint = ThisRacer.Client.Vehicle.TimeStamp;
                //                    ThisRacer.TimeStamp_RaceStarted = ThisRacer.Client.Vehicle.TimeStamp;
                //                    ThisRacer.CurrentLap = 1;
                //                }
                //                StartedRace.EnableJumpStartDetection = false;
                //                StartedRace.Status.SetStarted();
                //                StartedRace.RaceStarted = DateTime.Now;
                //                StartedRace.RaceEnds = StartedRace.RaceStarted;
                //                ClientsInRace.SendMessage("&eGO!");
                //                Clients.AllClients.Except(ClientsInRace).ToList().SendMessage("&eRace " + StartedRace.ID + " has begun!");
                //            }, "START RACE");
                //            #endregion
                //            return true;
                //        }
                //        if (StartedRace.RaceTypeIsTimeTrial())
                //        {
                //            List<Client> ClientsInRace = StartedRace.Racers.Select(x => x.Client).ToList();
                //            foreach (Games.Racing2.RaceDescription._Racer ThisRacer in StartedRace.Racers)
                //            {
                //                ThisRacer.TimeStamp_LapStarted = ThisRacer.Client.Vehicle.TimeStamp;
                //                ThisRacer.TimeStamp_LastCheckpoint = ThisRacer.Client.Vehicle.TimeStamp;
                //                ThisRacer.TimeStamp_RaceStarted = ThisRacer.Client.Vehicle.TimeStamp;
                //                ThisRacer.CurrentLap = 1;
                //            }
                //            StartedRace.Status.SetStarted();
                //            StartedRace.RaceStarted = DateTime.Now;
                //            StartedRace.RaceEnds = StartedRace.RaceStarted;
                //            ClientsInRace.SendMessage("&e## TIME TRIAL STARTED ##");
                //            Clients.AllClients.Except(ClientsInRace).ToList().SendMessage("&eRace " + StartedRace.ID + " has begun!");
                //        }
                //        return false;
                //    case Games.Racing2.RaceDescription._Status._RaceStateFlags.Started:
                //        ThisClient.SendMessage("&eThe race is already running!");
                //        return false;
                //    case Games.Racing2.RaceDescription._Status._RaceStateFlags.RedFlagged:
                //        ThisClient.SendMessage("&eThe race has been red flagged!");
                //        return false;
                //    case Games.Racing2.RaceDescription._Status._RaceStateFlags.Ended:
                //        ThisClient.SendMessage("&eThe race is over!");
                //        return false;
                //}
                ThisClient.SendMessage("Some Logic Error in the StartRace Command???");
                return false;
            }
            else
            {
                ThisClient.SendMessage("Some Logic Error in the StartRace Command???");
                return true;
            }
        }
    }
}