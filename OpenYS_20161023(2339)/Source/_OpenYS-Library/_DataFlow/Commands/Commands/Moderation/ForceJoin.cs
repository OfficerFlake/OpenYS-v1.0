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
        public static readonly CommandDescriptor OpenYS_Command_Moderation_ForceJoin = new CommandDescriptor
        {
            _Name = "Spawn",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Joins Flight.",
            _Usage = "/Spawn",
            _Commands = new string[] { "/Spawn" },

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
                //Requirement.Status_Connected |
                //Requirement.Status_Flying |
                Requirement.Status_NotFlying   |
                //Requirement.Permission_OP |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Moderation_ForceJoin_Method,
        };

        public static bool OpenYS_Command_Moderation_ForceJoin_Method(Client ThisClient, CommandReader Command)
        {
            if (ThisClient.Version <= 20110207)
            {
                ThisClient.SendMessage("You will need the 2014+ version of YSFlight to use this command!");
                return false;
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