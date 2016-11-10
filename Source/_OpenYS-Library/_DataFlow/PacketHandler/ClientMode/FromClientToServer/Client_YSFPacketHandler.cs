using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace OpenYS
{
    public static partial class PacketHandler
    {
        public static partial class ClientMode
        {
            public static partial class FromClientToServer
            {
                #region CLIENT
                public static bool YSFHandle(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region SWITCH
                    if (InPacket == null | InPacket == Packets.NoPacket)
                    {
                        return false;
                    }
                    switch (InPacket.Type)
                    {
                        case 1:
                            YSFHandle_01_Login(ThisClient, InPacket);
                            break;
                        case 11:
                            YSFHandle_11_FlightData(ThisClient, InPacket);
                            break;
                        case 12:
                            YSFHandle_12_Unjoin(ThisClient, InPacket);
                            break;
                        case 13:
                            YSFHandle_13_RemoveAircraft(ThisClient, InPacket);
                            break;
                        case 32:
                            YSFHandle_32_ChatMessage(ThisClient, InPacket);
                            break;
                        default:
                            //ThisClient.YSFServer.SendPacket(InPacket);
                            break;
                    }
                    #endregion
                    return true;
                }

                private static bool YSFHandle_01_Login(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region Get Login(01)
                    Packets.Packet_01_Login LoginPacket = new Packets.Packet_01_Login(InPacket);
                    ThisClient.Username = LoginPacket.Username;
                    ThisClient.Version = LoginPacket.Version;
                    #endregion

                    bool isBot = (ThisClient.Username.ToUpperInvariant() == "PHP_BOT");
                    if (!isBot) Console.WriteLine(ConsoleColor.Yellow, ThisClient.Username + " Logging in...");

                    Clients.AllClients.SendMessage("&a" + ThisClient.Username + " Joined the server.");
                    //ThisClient.YSFServer.SendPacket(InPacket);
                    return true;
                }

                private static bool YSFHandle_11_FlightData(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region Prepare Flight Data
                    //Prepare FlightData Packet
                    //Check if the server has the vehicle specified.
                    //Deny old flightdatapackets.
                    Packets.Packet_11_FlightData FlightData = new Packets.Packet_11_FlightData(InPacket);
                    Vehicle SenderVehicle;
                    lock (Vehicles.List)
                    {
                        if (Vehicles.List.ToArray().Where(x => x.ID == FlightData.ID).Count() <= 0)
                        {
                            Debug.WriteLine("Missing Aircraft ID: " + FlightData.ID + " for client " + ThisClient.Username);
                            return false;
                        }
                        SenderVehicle = Vehicles.List.ToArray().Where(x => x.ID == FlightData.ID).ToArray()[0];
                    }
                    #endregion
                    #region Update...
                    #region ValidateFlightData
                    if (SenderVehicle.TimeStamp > FlightData.TimeStamp)
                    {
                        Debug.WriteLine("OLD DATA:" + SenderVehicle.TimeStamp + " > " + FlightData.TimeStamp);
                        return false;
                    }
                    Debug.WriteLine("NEW DATA:" + SenderVehicle.TimeStamp + " <= " + FlightData.TimeStamp);
                    float Difference = FlightData.TimeStamp - SenderVehicle.TimeStamp;
                    OpenYS_Link.Stats.total_flight_seconds += Difference;
                    SenderVehicle.Update(FlightData);
                    #endregion
                    #endregion
                    #region FormationData
                    #region Validate
                    Vehicle TargetVehicle;
                    lock (Vehicles.List)
                    {
                        FlightData = new Packets.Packet_11_FlightData(InPacket);
                        if (!ThisClient.YSFServer.OpenYSSupport) goto HandleFlightData;
                        if (ThisClient.FormationTarget == 0) goto HandleFlightData;
                        if (Vehicles.List.ToArray().Where(x => x.ID == ThisClient.FormationTarget).Count() <= 0) goto HandleFlightData;
                        if (!FlightData.Anim_Light_Land) goto HandleFlightData;
                        TargetVehicle = Vehicles.List.ToArray().Where(x => x.ID == ThisClient.FormationTarget).ToArray()[0];
                        double Distance = Math.Sqrt(Math.Pow(FlightData.PosX - TargetVehicle.PosX, 2) + Math.Pow(FlightData.PosY - TargetVehicle.PosY, 2) + Math.Pow(FlightData.PosZ - TargetVehicle.PosZ, 2));
                        double Velocity = Math.Sqrt(Math.Pow(FlightData.V_PosX, 2) + Math.Pow(FlightData.V_PosY, 2) + Math.Pow(FlightData.V_PosZ, 2));
                        //if (Distance > Velocity / 25) goto HandleFlightData; //V / 10 / 2.5 eg: 1000cm/s -> 100m/s -> 40m. (at roughly 200 knots) or 80m at 400knots, 160m at 800 knots...
                    }
                    #endregion
                    #region BuildFormationData
                    Packets.Packet_64_11_FormationData FormationData = new Packets.Packet_64_11_FormationData(5);
                    FormationData.TimeStamp = FlightData.TimeStamp;
                    FormationData.SenderID = FlightData.ID;
                    FormationData.TargetID = TargetVehicle.ID;
                    FormationData.Version = 5;
                    FormationData._Anim_Flags = FlightData._Anim_Flags;

                    FormationData.PosX = FlightData.PosX - TargetVehicle.PosX;
                    FormationData.PosY = FlightData.PosY - TargetVehicle.PosY;
                    FormationData.PosZ = FlightData.PosZ - TargetVehicle.PosZ;
                    FormationData.HdgX = FlightData.HdgX;
                    FormationData.HdgY = FlightData.HdgY;
                    FormationData.HdgZ = FlightData.HdgZ;
                    FormationData.V_HdgX = (short)FlightData.V_HdgX;
                    FormationData.V_HdgY = (short)FlightData.V_HdgY;
                    FormationData.V_HdgZ = (short)FlightData.V_HdgZ;

                    //Need to add the below to the formation data packet...
                    FormationData.Anim_Aileron = FlightData.Anim_Aileron;
                    FormationData.Anim_Boards = FlightData.Anim_Boards;
                    FormationData.Anim_BombBay = FlightData.Anim_BombBay;
                    FormationData.Anim_Brake = FlightData.Anim_Brake;
                    FormationData.Anim_Elevator = FlightData.Anim_Elevator;
                    FormationData.Anim_Flaps = FlightData.Anim_Flaps;
                    FormationData.Anim_Gear = FlightData.Anim_Gear;
                    FormationData.Anim_Nozzle = FlightData.Anim_Nozzle;
                    FormationData.Anim_Reverse = FlightData.Anim_Reverse;
                    FormationData.Anim_Rudder = FlightData.Anim_Rudder;
                    FormationData.Anim_Throttle = FlightData.Anim_Throttle;
                    FormationData.Anim_Trim = FlightData.Anim_Trim;
                    FormationData.Anim_VGW = FlightData.Anim_VGW;
                    #endregion

                    FormationData.PosX = 0;
                    FormationData.PosY = 0;
                    FormationData.PosZ = 0;
                    //ThisClient.YSFServer.SendPacket(FormationData.ToCustomPacket());
                    //ThisClient.LastFlightDataPacket = FlightData;
                    //Send FormationData to the server.
                    return true;
                    #endregion
                    #region NormalFlightData
                HandleFlightData:
                    //ThisClient.YSFServer.SendPacket(FlightData);
                    return true;
                    #endregion
                }

                private static bool YSFHandle_12_Unjoin(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    //ThisClient.YSFServer.SendPacket(InPacket);

                    //Re-Synchronise...
                    OpenYS_Link.OYS_Link_Response Response = OpenYS_Link.Get(OpenYS_Link.Stats._id, "stats_total_flight_seconds");
                    double OldTime = 0;
                    bool Failed = !Double.TryParse(Response.Response, out OldTime);

                    if (!Response.Success | Failed)
                    {
                        ThisClient.SendMessage("Failed to update your flight time due to a server error! Sorry about that!");
                        return true;
                    }



                    //Update OYS_LINK...
                    double difference = OpenYS_Link.Stats.total_flight_seconds - OldTime;
                    Response = OpenYS_Link.Set(OpenYS_Link.Stats._id, "stats_total_flight_seconds", OpenYS_Link.Stats.total_flight_seconds);
                    if (Response.Success)
                    {
                        ThisClient.SendMessage("Successfully updated your flight time by " + Math.Round(difference.AsSeconds().TotalHours, 2).ToString() + " hours.");
                        ThisClient.SendMessage("Your new total flight time is " + Math.Round(OpenYS_Link.Stats.total_flight_seconds.AsSeconds().TotalHours, 2).ToString() + " hours.");

                        return true;
                    }
                    else
                    {
                        ThisClient.SendMessage("Failed to update your flight time due to a server error! Sorry about that!");
                        return true;
                    }

                }

                private static bool YSFHandle_13_RemoveAircraft(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    //ThisClient.YSFServer.SendPacket(InPacket);

                    //Update OYS_LINK...
                    //OpenYS_Link.Set(OpenYS_Link.Stats._id, "stats_total_flight_seconds", OpenYS_Link.Stats.total_flight_seconds);
                    return true;
                }

                private static bool YSFHandle_32_ChatMessage(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_32_ChatMessage ChatMessage = new Packets.Packet_32_ChatMessage(InPacket, ThisClient.Username);
                    lock (ThisClient.MessagesTyped)
                    {
                        if ((ChatMessage.Message) == "/" & ThisClient.MessagesTyped.Count == 0)
                        {
                            ThisClient.SendMessage("No previously typed commands...");
                            return false;
                        }
                        if ((ChatMessage.Message) != "/") ThisClient.MessagesTyped.Add(new Client.MessageTypedInfo(ChatMessage.Message));
                    }
                    CommandManager.Process(ThisClient, ChatMessage.Message);
                    return true;
                }
                #endregion
            }
        }
    }
}