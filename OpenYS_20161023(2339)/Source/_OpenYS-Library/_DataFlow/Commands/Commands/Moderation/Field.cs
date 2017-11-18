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
        public static readonly CommandDescriptor OpenYS_Command_Options_Field = new CommandDescriptor
        {
            _Name = "Field",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Changes the server Field.",
            _Usage = "/Field (NewFieldName)",
            _Commands = new string[] { "/Field", "/Options.Field", "/Map" },

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
            _Handler = OpenYS_Command_Options_Field_Method,
        };

        public static bool OpenYS_Command_Options_Field_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                if (OpenYS.Field != null)
                {
                    ThisClient.SendMessage("&eCurrent Field: &a" + OpenYS.Field.FieldName + "&e.");
                }
                else
                {
                    ThisClient.SendMessage("&cERROR: Server Field handler not found.");
                }
                return false;
            }
            if (Command._CmdRawArguments.Contains(' ')) Command._CmdRawArguments = '"' + Command._CmdRawArguments + '"';
            if (MetaData._Scenery.List.Where(x => x.Identify.ToUpperInvariant().StartsWith(Command._CmdRawArguments.ToUpperInvariant())).Count() <= 0)
            {
                ThisClient.SendMessage("&eTarget Field not found: " + Command._CmdRawArguments + "&e.");
                return false;
            }
            if (Environment.CommandLineArguments.Length == 0)
            {
                Environment.CommandLineArguments = new string[] { Settings.Loading.YSFlightDirectory, MetaData._Scenery.List.Where(x => x.Identify.ToUpperInvariant().StartsWith(Command._CmdRawArguments.ToUpperInvariant())).ToArray()[0].Identify, Settings.Server.ListenerPort.ToString() };
            }
            else if (Environment.CommandLineArguments.Length > 1)
            {
                Environment.CommandLineArguments[1] = MetaData._Scenery.List.Where(x => x.Identify.ToUpperInvariant().StartsWith(Command._CmdRawArguments.ToUpperInvariant())).ToArray()[0].Identify;
            }
            Settings.Loading.FieldName = Environment.CommandLineArguments[1];
            SettingsHandler.SaveAll();
            ThisClient.SendMessage("&cTarget Server Field changed to &e" + Environment.CommandLineArguments[1] + "&c.");
            ThisClient.SendMessage("&cTO CHANGE THE Field NOW, RESTART THE SERVER!");
            //Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " set the server name to: &2" + OpenYS.OpenYSConsole.Info.Username + "&a.");
            return true;
        }
    }
}