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
        public static readonly CommandDescriptor OpenYS_Command_Administration_Password = new CommandDescriptor
        {
            _Name = "Password",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Logs into the server as an authorised admin. DO NOT TYPE \"/PASSWORD YOURPASSHERE\". THE SERVER WILL ASK YOU FOR YOUR PASSWORD!",
            _Usage = "/Password",
            _Commands = new string[] { "/Password", "/Authenticate" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.Permission_OP |
                Requirement.User_YSFlight      |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                Requirement.Status_Connected   |
                //Requirement.Status_Flying      |
                Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Administration_Password_Method,
        };

        public static bool OpenYS_Command_Administration_Password_Method(Client ThisClient, CommandReader Command)
        {
            Console.WriteLine("&c" + ThisClient.Username + " is trying to log in to the server as an administrator...");
            if (Settings.Administration.AdminPassword == "")
            {
                ThisClient.SendMessage("&cNo Admin Password is set for the server, cannot log you in!");
                return false;
            }
            ThisClient.SendMessage("&9OpenYS Server Authentication System");
            ThisClient.SendMessage("&f===================================");
            ThisClient.SendMessage("");
            ThisClient.SendMessage("&9Ready to log you in to the server. All messages are now being securely caught, and will not be passed onto the server.");
            ThisClient.SendMessage("&9Please also note that you are deaf to all messages while you you are in this interface!");
            ThisClient.SendMessage("");
            ThisClient.SendMessage("Please enter the admin password.");
            Console.WriteLine("&e" + ThisClient.Username + " temporarily deafened as part of the authentication process...");
            ThisClient.IsDeaf = true;
            while (true)
            {
                Packets.GenericPacket InPacket = ThisClient.YSFClient.ReceivePacket();
                if (InPacket.Type == 32)
                {
                    ThisClient.IsDeaf = false;
                    Packets.Packet_32_ChatMessage ChatMessage = new Packets.Packet_32_ChatMessage(InPacket, ThisClient.Username);
                    if (ChatMessage.Message == Settings.Administration.AdminPassword)
                    {
                        Console.WriteLine("&a" + ThisClient.Username + " has successfully authenticated as a server administrator.");
                        ThisClient.SendMessage("&aSuccessfully logged in as an administrator... Returning you back to the server.");
                        Clients.YSFClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " has successfully authenticated as a server administrator.");
                        ThisClient.OP();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("&c" + ThisClient.Username + " failed to authenticate as an administrator.");
                        ThisClient.SendMessage("&cAdmin Password Incorrect... Returning you back to the server.");
                        break;
                    }
                }
                else
                {
                    if (InPacket.Type == 08)
                    {
                        ThisClient.IsDeaf = false;
                        ThisClient.SendMessage("&cJoin Requested Detected... Returning you back to the server.");
                        ThisClient.YSFClient.ProcessPacket(InPacket);
                        break;
                    }
                    ThisClient.YSFClient.ProcessPacket(InPacket);
                }
            }
            return true;
        }
    }
}