using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OpenYS
{
    public static partial class PacketHandler
    {
        public static partial class ClientMode
        {
            public static partial class FromServerToClient
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
                        case 05:
                            YSFHandle_05_EntityJoined(ThisClient, InPacket);
                            break;
                        case 11:
                            YSFHandle_11_FlightData(ThisClient, InPacket);
                            break;
                        case 13:
                            YSFHandle_13_RemoveAircraft(ThisClient, InPacket);
                            break;
                        case 16:
                            YSFHandle_16_PrepareSimulation(ThisClient, InPacket);
                            break;
                        case 29:
                            //When the Server sends the client the version number, tell the server that this client supports the OpenYS Protocal.
                            //ThisClient.YSFServer.SendPacket(new Packets.Packet_64_29_OpenYS_Handshake(Settings.Loading.OYSNetcodeVersion).ToCustomPacket());

                            ThisClient.SendPacket(InPacket);
                            break;
                        case 64:
                            //OpenYS User Data Packet
                            YSFHandle_64_UserPacket(ThisClient, InPacket);
                            break;
                        default:
                            ThisClient.SendPacket(InPacket);
                            break;
                    }
                    #endregion
                    return true;
                }

                private static bool YSFHandle_05_EntityJoined(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    if (InPacket.Data.Length < 171)
                    {
                        ThisClient.SendPacket(InPacket);
                        return true;
                    }

                    Packets.Packet_05_EntityJoined EntityJoined = new Packets.Packet_05_EntityJoined(InPacket);
                    Vehicle ThisVehicle = new Vehicle();
                    ThisVehicle.Update(EntityJoined);
                    ThisVehicle.TimeStamp = 0; //So the client can have immediate control of the flight data!
                    #region Add Vehicle to Vehicles List
                    lock (Vehicles.List)
                    {
                        Vehicles.List.RemoveAll(x => x == ThisVehicle);
                        Vehicles.List.Add(ThisVehicle);
                        VehiclesHistory.List.Add(ThisVehicle);
                    }
                    //ThisClient.CurrentAircraftJoinPacket = new Packets.Packet_05_EntityJoined(EntityJoined);
                    #endregion

                    if (EntityJoined.IsGround)
                    {
                        ThisClient.SendPacket(InPacket);
                        return true;
                    }
                    if (Clients.YSFClients[0] == ThisClient)
                    {
                        Console.WriteLine("&b" + "Aircraft ID " + EntityJoined.ID + " Created");
                    }
                    if (EntityJoined.OwnerType == 3)
                    {
                        //Console.WriteLine(EntityJoined.Data.ToDebugHexString());
                        //Console.WriteLine(ThisConnection.ThisClient.Info.Username);
                        //Console.WriteLine(EntityJoined.ID);
                        ThisClient.Vehicle = ThisVehicle;
                        ThisClient.SendPacket(InPacket);
                        /*

                        Client.Listener AckCreation = ThisConnection.ThisClient.YSFClient.ThisConnection.SocketDescriptor.SubscribeAcknowledgement(0, EntityJoined.ID);
                        ThisConnection.SocketDescriptor.SendPacket(InPacket);
                        ThisConnection.ThisClient.YSFClient.WaitListener(AckCreation);

                        Packet_09_JoinApproved JoinApproved = new Packets.Packet_09_JoinApproved(new Packets.GenericPacket() { Type = 09 });
                        Client.Listener WaitJoinApproval = ThisConnection.ThisClient.YSFServer.ThisConnection.Subscribe(JoinApproved);
                        ThisConnection.ThisClient.YSFServer.WaitListener(WaitJoinApproval);

                        Client.Listener AcknowledgeJoinApproval = ThisConnection.ThisClient.YSFClient.ThisConnection.SocketDescriptor.SubscribeAcknowledgement(6, 0);
                        ThisConnection.ThisClient.YSFClient.WaitListener(AcknowledgeJoinApproval);

                        Console.WriteLine("ADDED VEHICLE. TOTAL VEHICLES IS NOW: " + Server.Info.Vehicles.Count.ToString());
                        */
                        return true;
                    }
                    ThisClient.SendPacket(InPacket);
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
                            //Console.WriteLine("Missing Aircraft ID: " + FlightData.ID + " for client " + ThisConnection.ThisClient.Info.Username);
                            return false;
                        }
                        SenderVehicle = Vehicles.List.ToArray().Where(x => x.ID == FlightData.ID).ToArray()[0];
                    }
                    #endregion
                    #region ValidateFlightData
                    if (SenderVehicle.TimeStamp > FlightData.TimeStamp) return false;
                    SenderVehicle.Update(FlightData);
                    #endregion
                    ThisClient.SendPacket(InPacket);
                    return true;
                }

                private static bool YSFHandle_13_RemoveAircraft(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    ThisClient.SendPacket(InPacket);
                    Packets.Packet_13_RemoveAirplane RemoveAirplane = new Packets.Packet_13_RemoveAirplane(InPacket);
                    lock (Vehicles.List) Vehicles.List.RemoveAll(x => x.ID == RemoveAirplane.ID);
                    if (Clients.YSFClients[0] == ThisClient)
                    {
                        if (Settings.Flight.LeaveFlightNotification)
                        {
                            Console.WriteLine("&3" + "Aircraft ID " + RemoveAirplane.ID + " Removed");
                        }
                    }
                    //Console.WriteLine("REMOVED VEHICLE. TOTAL VEHICLES IS NOW: " + Server.Info.Vehicles.Count.ToString());
                    return true;
                }

                private static bool YSFHandle_16_PrepareSimulation(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Console.WriteLine(ConsoleColor.Green, ThisClient.Username + " Login Complete!");
                    ThisClient.SendPacket(InPacket);
                    return true;
                }

                private static bool YSFHandle_64_UserPacket(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_64_UserPacket UserPacket = new Packets.Packet_64_UserPacket(InPacket);
                    PacketHandler.ClientMode.FromServerToClient.OYSHandle(ThisClient, UserPacket.ToYSFPacket());
                    return true;
                }
                #endregion
            }
        }
    }
}