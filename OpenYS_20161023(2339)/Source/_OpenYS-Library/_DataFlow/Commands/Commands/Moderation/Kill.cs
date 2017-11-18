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
        public static readonly CommandDescriptor OpenYS_Command_Flight_Kill = new CommandDescriptor
        {
            _Name = "Kill",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Kills a Vehicle ID.",
            _Usage = "/Kill [ID]",
            _Commands = new string[] { "/Flight.Kill", "/Kill", "/Flight.KillID", "/KillID" },

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
            _Handler = OpenYS_Command_Flight_Kill_Method,
        };

        public static bool OpenYS_Command_Flight_Kill_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eSpecify a Vehicle ID to kill.\n\nUse &a/ListUsers&e to get the ID.");
                return false;
            }
            uint output = 0;
            if (!UInt32.TryParse(Command._CmdArguments[0], out output))
            {
                //didn't get a number... try and find a client with that name?
                Client MatchingClient = Clients.FindByUserName(Command._CmdRawArguments);
                if (MatchingClient != Clients.NoClient)
                {
                    //did match a client name!
                    if (MatchingClient.Vehicle != Vehicles.NoVehicle)
                    {
                        //client has a vehicle!
                        output = MatchingClient.Vehicle.ID;
                        goto Kill;
                    }
                    else
                    {
                        ThisClient.SendMessage("&eThat client isn't flying...");
                        return false;
                    }
                }
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an integer value!");
                return false;
            }
            if (Vehicles.List.Where(x => x.ID == output).Count() == 0)
            {
                ThisClient.SendMessage("&eThat Vehicle doesn't exist!");
                return false;
            }
            Kill:
            Packets.Packet_13_RemoveAirplane KillID = new Packets.Packet_13_RemoveAirplane(output);
            Clients.AllClients.SendPacket(KillID);
            ThisClient.SendMessage("&eKilled &c" + output.ToString() + "&e.");
            Clients.AllClients.Exclude(ThisClient).SendMessage("&e" + ThisClient.Username + " Killed VehicleID &c" + output.ToString() + "&e.");
            return true;
        }
    }
}