using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;

namespace OpenYS
{
    public static partial class Commands
    {
        public static readonly CommandDescriptor OpenYS_Command_User_Kick = new CommandDescriptor
        {
            _Name = "KickIP",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Kicks all clients from an IP Address.",
            _Usage = "/Kick [IP]",
            _Commands = new string[] { "/Users.Kick", "/Kick", "/KickUser", "/KickIP" },

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
            _Handler = OpenYS_Command_User_Kick_Method,
        };

        public static bool OpenYS_Command_User_Kick_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 1)
            {
                ThisClient.SendMessage("&eSpecify an IPAddress to kick.\n\nUse &a/ListUsers&e to get the IP.");
                return false;
            }
            IPAddress output = IPAddress.None;
            if (!IPAddress.TryParse(Command._CmdArguments[0], out output))
            {
                ThisClient.SendMessage("&eFormat incorrect: Be sure you are using an IPAddress value!");
                return false;
            }
            if (Clients
                .AllClients
                .Exclude(ThisClient)
                .Where(x => x.IsFakeClient())
                .Where(z => (z.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString() == output.ToString())
                .Count() == 0)
            {
                ThisClient.SendMessage("&eNo Client found with that IPAddress!");
                return false;
            }
            foreach (Client OtherClient in Clients
                .AllClients
                .Exclude(ThisClient)
                .Where(x => x.IsConnected())
                .Where(z => (z.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString() == output.ToString()))
            {
                Clients.AllClients.Exclude(OtherClient).SendMessage("&c" + OtherClient.Username + "&c was kicked from the server.");
                OtherClient.SendMessage("You were kicked from the server.");
                OtherClient.Disconnect("Kicked from the Server by " + ThisClient.Username);
            }
            ThisClient.SendMessage("&eKicked all IP's matching &c" + output.ToString() + "&e.");
            return true;
        }
    }
}