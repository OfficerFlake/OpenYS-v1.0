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
        public static readonly CommandDescriptor OpenYS_Command_Chat_ListUser = new CommandDescriptor
        {
            _Name = "ListUsers",
            _Version = 1.0,
            _Date = new DateTime(2013, 10, 01),
            _Author = "OfficerFlake",

            _Category = "Chat",
            _Hidden = false,

            _Descrption = "Shows a list of online players.",
            _Usage = "/ListUsers",
            _Commands = new string[] { "/ListUser", "/ListUsers", "/User", "/Users" },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.User_Console |
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
            _Handler = OpenYS_Command_Chat_ListUser_Method,
        };

        public static bool OpenYS_Command_Chat_ListUser_Method(Client ThisClient, CommandReader Command)
        {
            if (!ThisClient.IsConsole())
            {
                ThisClient.SendMessage("&aList of Users Online:    &e" + Clients.AllClients.Select(x => x.Username).ToList().ToStringList());
                return true;
            }
            else
            {
                string output = "";
                foreach (Client OtherClient in Clients.AllClients)
                {
                    if (OtherClient == null) continue;
                    if (OtherClient.IsFakeClient()) continue;
                    if (output.Length > 0) output += "\n";
                    output += "&8[";
                    if (OtherClient.Vehicle != null & OtherClient.Vehicle != Vehicles.NoVehicle)
                    {
                        string vehiclestring = OtherClient.Vehicle.ID.ToString();
                        while (vehiclestring.Length < 5) vehiclestring = " " + vehiclestring;
                        output += "&f" + vehiclestring.Substring(0, 5);
                    }
                    else output += "&7-----";
                    output += "&8] ";
                    string uname = OtherClient.Username;
                    if (uname.Length > 16) uname = uname.Substring(0, 16);
                    output += "&2" + uname;
                    output += Strings.Repeat(" ", 17 - uname.Length);
                    if (OtherClient.YSFClient.Socket == null)
                    {
                        output += "&7<No IP Address Available>";
                        continue;
                    }
                    try
                    {
                        IPEndPoint remoteIpEndPoint = OtherClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint;
                        if (remoteIpEndPoint.Address.ToString().Count(x => x == '.') != 3 | remoteIpEndPoint.Address.IsIPv6Multicast)
                        {
                            output += "&7" + remoteIpEndPoint.Address.ToString();
                            continue;
                        }
                        string Byte0 = remoteIpEndPoint.Address.ToString().Split('.')[0];
                        string Byte1 = remoteIpEndPoint.Address.ToString().Split('.')[1];
                        string Byte2 = remoteIpEndPoint.Address.ToString().Split('.')[2];
                        string Byte3 = remoteIpEndPoint.Address.ToString().Split('.')[3];
                        //while (Byte0.Length < 3) Byte0 = "0" + Byte0;
                        //while (Byte1.Length < 3) Byte1 = "0" + Byte1;
                        //while (Byte2.Length < 3) Byte2 = "0" + Byte2;
                        //while (Byte3.Length < 3) Byte3 = "0" + Byte3;
                        output += "&8" + Byte0 + "." + Byte1 + "." + Byte2 + "." + Byte3;
                    }
                    catch
                    {
                        output += "&7<Error Parsing IP Address>";
                        continue;
                    }
                }
                if (output.Length == 0) output = "&7<No users to list>";
                Console.WriteLine(output);
                return true;
            }
        }
    }
}