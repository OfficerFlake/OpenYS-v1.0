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
        public static readonly CommandDescriptor OpenYS_Command_Options_Weapons = new CommandDescriptor
        {
            _Name = "EnableWeapons",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Toggles whether the server enables its \"Weapons\" setting.",
            _Usage = "/Weapons (On|Off)",
            _Commands = new string[] { "/Weapons", "/Options.Weapons", "/EnableWeapons" },

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
            _Handler = OpenYS_Command_Options_Weapons_Method,
        };

        public static bool OpenYS_Command_Options_Weapons_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (OpenYS.Weapons.Enabled)
                {
                    ThisClient.SendMessage("&eServer enables its \"Weapons\" setting.");
                }
                else
                {
                    ThisClient.SendMessage("&eServer disables its \"Weapons\" setting.");
                }
                return false;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "ON" | Command._CmdArguments[0].ToUpperInvariant() == "TRUE")
            {
                OpenYS.Weapons.Enabled = true;
                Clients.AllClients.SendPacket(OpenYS.Weapons);
                SettingsHandler.SaveAll();
                ThisClient.SendMessage("&aServer \"Weapons\" set to &aON&a.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set \"Weapons\" to &aON&a.");
                return true;
            }
            if (Command._CmdArguments[0].ToUpperInvariant() == "OFF" | Command._CmdArguments[0].ToUpperInvariant() == "FALSE")
            {
                OpenYS.Weapons.Enabled = false;
                Clients.AllClients.SendPacket(OpenYS.Weapons);
                SettingsHandler.SaveAll();
                ThisClient.SendMessage("&aServer \"Weapons\" set to &cOFF&a.");
                Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set \"Weapons\" to &cOFF&a.");
                return true;
            }
            ThisClient.SendMessage("&eUnrecognised option - \"" + Command._CmdArguments[0] + "\".");
            return false;
        }
    }
}