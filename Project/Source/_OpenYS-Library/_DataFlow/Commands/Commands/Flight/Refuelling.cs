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
        public static readonly CommandDescriptor OpenYS_Command_Flight_Refuelling = new CommandDescriptor
        {
            _Name = "Refuelling",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Flight",
            _Hidden = false,

            _Descrption = "Allows your aircraft to act as a refuelling tanker.\n\nYou will need:\n    -Over 10,000KG of usable fuel loaded.\n    -To be on the same IFF as your thirsty friends.\n\nOther aircraft will only refuel if they are no more than 100 meters away.",
            _Usage = "/Refuelling (On|Off)",
            _Commands = new string[] { "/Flight.Refuelling", "/Tanker", "/SupplyFuel", "/Refuel", "/Refuelling" },

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
            _Handler = OpenYS_Command_Flight_Refuelling_Method,
        };

        public static bool OpenYS_Command_Flight_Refuelling_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (!ThisClient.Vehicle.Refuelling)
                {
                    if (ThisClient.Vehicle.Weight_Fuel > 10000)
                    {
                        ThisClient.SendMessage("&eYou are not allowing other aircraft to refuel off you, but you are able to! Use &a\"/Refuelling On&e\" to enable!");
                    }
                    else
                    {
                        ThisClient.SendMessage("&eYou are not allowing other aircraft to refuel, and you do not have enough fuel on board to do so! (need 10,000KG+)");
                    }
                }
                else
                {
                    if (ThisClient.Vehicle.Weight_Fuel > 10000)
                    {
                        ThisClient.SendMessage("&eYou are allowing other aircraft to refuel off you.\n\nYou can supply another " + (ThisClient.Vehicle.Weight_Fuel - 10000).ToString() + "KG of fuel.");
                    }
                    else
                    {
                        ThisClient.SendMessage("&eYou are allowing other aircraft to refuel off you, but you have run out of available fuel to refuel other aircraft with! (need 10,000KG+)");
                    }
                }
                return false;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "ON" | Command._CmdArguments[0].ToUpperInvariant() == "TRUE")
            {
                ThisClient.Vehicle.Refuelling = true;
                if (ThisClient.Vehicle.Weight_Fuel < 10000)
                {
                    ThisClient.SendMessage("&aRefuelling turned &aON&e, but you do not have enough fuel to refuel other aircraft (Need 10,000KG+)");
                }
                else
                {
                    ThisClient.SendMessage("&aRefuelling turned &aON&e.");
                }
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "OFF" | Command._CmdArguments[0].ToUpperInvariant() == "FALSE")
            {
                ThisClient.Vehicle.Refuelling = false;
                ThisClient.SendMessage("&aRefuelling turned &cOFF&a.");
                return true;
            }
            ThisClient.SendMessage("&eUnrecognised option - \"" + Command._CmdArguments[0] + "\".");
            return false;
        }
    }
}