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
        public static readonly CommandDescriptor OpenYS_Command_Flight_Form = new CommandDescriptor
        {
            _Name = "Form",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Flight",
            _Hidden = false,

            _Descrption = "Chooses A Formation Target based off Vehicle ID.\n\nTo fly formation on a chosen aircraft, turn your aircraft lights on.\nTo leave the formation at any time, type \"&a/Form 0&e\"",
            _Usage = "/Form [ID]",
            _Commands = new string[] { "/Flight.Form", "/Form" },

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
            _Handler = OpenYS_Command_Flight_Form_Method,
        };

        public static bool OpenYS_Command_Flight_Form_Method(Client ThisClient, CommandReader Command)
        {
            uint ID = 0;
            if (Command._CmdArguments.Count() < 1)
            {
                if (ThisClient.FormationTarget == 0)
                {
                    ThisClient.SendMessage("&eYou are not currently forming on an aircraft.");
                }
                else
                {
                    ThisClient.SendMessage("&eCurrently forming on \"" + ThisClient.FormationTarget.ToString() + "\".");
                }
                return false;
            }
            if (!UInt32.TryParse(Command._CmdArguments[0], out ID))
            {
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an integer value!");
                return false;
            }
            if (ID == ThisClient.Vehicle.ID)
            {
                ThisClient.SendMessage("&eYou cannot form on yourself!");
                return false;
            }
            ThisClient.FormationTarget = ID;
            ThisClient.SendMessage("&eSucessfully set Formation Target to \"" + ThisClient.FormationTarget.ToString() + "\".");
            return true;
        }
    }
}