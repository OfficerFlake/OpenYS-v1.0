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
        public static readonly CommandDescriptor OpenYS_Command_Flight_VehicleID = new CommandDescriptor
        {
            _Name = "VehicleID",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Flight",
            _Hidden = false,

            _Descrption = "Get your current Vehicle ID.",
            _Usage = "/ID",
            _Commands = new string[] { "/Flight.GetID", "/GetID", "/Flight.VehicleID", "/VehicleID", "/ID" },

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
                Requirement.Status_Flying |
                //Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Flight_VehicleID_Method,
        };

        public static bool OpenYS_Command_Flight_VehicleID_Method(Client ThisClient, CommandReader Command)
        {
            ThisClient.SendMessage("&eYour Vehicle ID is \"" + ThisClient.Vehicle.ID.ToString() + "\".");
            return true;
        }
    }
}