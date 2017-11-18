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
        public static readonly CommandDescriptor OpenYS_Command_Flight_ResumeFlight = new CommandDescriptor
        {
            _Name = "ResumeFlight",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Flight",
            _Hidden = false,

            _Descrption = "Respawn at the last known position of the given vehicle ID.",
            _Usage = "/Rejoin (ID)",
            _Commands = new string[] { "/Flight.Resume", "/ResumeFlight", "/Resume", "/Rejoin" },

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
                Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Flight_ResumeFlight_Method,
        };

        public static bool OpenYS_Command_Flight_ResumeFlight_Method(Client ThisClient, CommandReader Command)
        {
            #region Use Last ID
            uint output = 0;
            if (Command._CmdArguments.Count() < 1)
            {
                output = ThisClient.LastVehicleID;
            }
            #endregion
            #region Use Specified ID
            else
            {
                if (!UInt32.TryParse(Command._CmdArguments[0], out output))
                {
                    ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an integer value!");
                    return false;
                }
            }
            #endregion
            #region ID doesn't exist!
            if (!VehiclesHistory.List.Select(x => x.ID).Contains(output))
            {
                ThisClient.SendMessage("Specified Vehicle ID not found!");
                return false;
            }
            #endregion
            ThisClient.SpawnTargetVehicle = output;
            if (ThisClient.Version <= 20110207)
            {
                ThisClient.SendMessage("Ready to rejoin, send a join request!");
                return true;
            }
            else
            {
                Packets.Packet_47_ForceJoin ForceJoin = new Packets.Packet_47_ForceJoin();
                ThisClient.SendPacket(ForceJoin);
                return true;
            }
        }
    }
}