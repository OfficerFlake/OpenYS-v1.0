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
        public static partial class ServerMode
        {
            public static partial class FromClientToServer
            {
                #region SERVER

                public static bool OYSHandle(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region PacketSwitch
                    if (InPacket == null | InPacket == Packets.NoPacket)
                    {
                        return false;
                    }
                    switch (InPacket.Type)
                    {
                        case 11:
                            OYSHandle_11_FlightData(ThisClient, InPacket);
                            break;
                        case 29:
                            OYSHandle_29_HandShake(ThisClient, InPacket);
                            break;
                        default:
#if DEBUG
                        //Console.WriteLine("Unknown Custom Packet From " + ThisClient.Username);
                        //Console.WriteLine("-Type: " +  InPacket.Type.ToString());
                        //Console.WriteLine("-Data: " +  InPacket.Data.ToString());
#endif
                            break;
                    }
                    #endregion
                    return true;
                }

                public static bool OYSHandle_11_FlightData(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Console.WriteLine("Got Formation Data.");
                #region PrepareFormationData
                    Packets.Packet_64_11_FormationData FormationData = new Packets.Packet_64_11_FormationData(InPacket);
                    if (Vehicles.List.ToArray().Where(x => x.ID == FormationData.TargetID).Count() <= 0)
                    {
                        Console.WriteLine("Target N/A");
                        return false;
                    }
                    if (Vehicles.List.ToArray().Where(x => x.ID == FormationData.SenderID).Count() <= 0)
                    {
                        Console.WriteLine("Sender N/A");
                        return false;
                    }
                    Vehicle SenderVehicle = Vehicles.List.ToArray().Where(x => x.ID == FormationData.SenderID).ToArray()[0];
                    Vehicle TargetVehicle = Vehicles.List.ToArray().Where(x => x.ID == FormationData.TargetID).ToArray()[0];
                    #endregion
                #region ValidateFlightData
                    if (SenderVehicle.TimeStamp > FormationData.TimeStamp)
                    {
                        Console.WriteLine("OLD FORM");
                        return false;
                    }
                    SenderVehicle.Update(FormationData);
                #endregion
                #region UpdateSendersVehicle
                    SenderVehicle.V_PosX = TargetVehicle.V_PosX;
                    SenderVehicle.V_PosY = TargetVehicle.V_PosY;
                    SenderVehicle.V_PosZ = TargetVehicle.V_PosZ;
                #endregion
                #region BasicFlightData
                    Packets.Packet_11_FlightData FlightDataOut = new Packets.Packet_11_FlightData(3);
                    float TDelta = (float)(DateTime.Now - TargetVehicle.LastUpdated).TotalSeconds;
                    FlightDataOut.ID = FormationData.SenderID;
                    FlightDataOut.PosX = TargetVehicle.PosX + FormationData.PosX + (TDelta * (TargetVehicle.V_PosX / 10));
                    FlightDataOut.PosY = TargetVehicle.PosY + FormationData.PosY + (TDelta * (TargetVehicle.V_PosY / 10));
                    FlightDataOut.PosZ = TargetVehicle.PosZ + FormationData.PosZ + (TDelta * (TargetVehicle.V_PosZ / 10));
                    FlightDataOut.V_PosX = TargetVehicle.V_PosX;
                    FlightDataOut.V_PosY = TargetVehicle.V_PosY;
                    FlightDataOut.V_PosZ = TargetVehicle.V_PosZ;
                    FlightDataOut.HdgX = FormationData.HdgX;
                    FlightDataOut.HdgY = FormationData.HdgY;
                    FlightDataOut.HdgZ = FormationData.HdgZ;
                    FlightDataOut.V_HdgX = FormationData.V_HdgX;
                    FlightDataOut.V_HdgY = FormationData.V_HdgY;
                    FlightDataOut.V_HdgZ = FormationData.V_HdgZ;
                    FlightDataOut.Weight_Fuel = 100;
                    FlightDataOut.Weight_Payload = 100;
                    FlightDataOut.Weight_SmokeOil = 100;
                #endregion
                #region Animation
                    FlightDataOut.Anim_Aileron = FormationData.Anim_Aileron;
                    FlightDataOut.Anim_Boards = FormationData.Anim_Boards;
                    FlightDataOut.Anim_BombBay = FormationData.Anim_BombBay;
                    FlightDataOut.Anim_Brake = FormationData.Anim_Brake;
                    FlightDataOut.Anim_Elevator = FormationData.Anim_Elevator;
                    FlightDataOut.Anim_Flaps = FormationData.Anim_Flaps;
                    FlightDataOut.Anim_Gear = FormationData.Anim_Gear;
                    FlightDataOut.Anim_Nozzle = FormationData.Anim_Nozzle;
                    FlightDataOut.Anim_Reverse = FormationData.Anim_Reverse;
                    FlightDataOut.Anim_Rudder = FormationData.Anim_Rudder;
                    FlightDataOut.Anim_Throttle = FormationData.Anim_Throttle;
                    FlightDataOut.Anim_Trim = FormationData.Anim_Trim;
                    FlightDataOut.Anim_VGW = FormationData.Anim_VGW;
                    FlightDataOut._Anim_Flags = FormationData._Anim_Flags;
                #endregion
                #region Send FlightData
                    FlightDataOut.TimeStamp = (float)(DateTime.Now - OpenYS.TimeStarted).TotalSeconds;
                    FormationData.TimeStamp = (float)(DateTime.Now - OpenYS.TimeStarted).TotalSeconds;
                    foreach (Client OtherClient in Clients.AllClients.ToArray())
                    {
                        if (OtherClient.Vehicle != null)
                        {
                            if (FlightDataOut.ID == OtherClient.Vehicle.ID) continue;
                            //Console.WriteLine("-Did nothing.");
                        }
                        if (OtherClient.YSFClient.OpenYSSupport)
                        {
                            ThisClient.SendPacket(FormationData.ToCustomPacket());
                            //OtherClient.SendPacket(FlightDataOut);
                        }
                        if (!OtherClient.YSFClient.OpenYSSupport)
                        {
                            ThisClient.SendPacket(FlightDataOut);
                        }
                    }
                #endregion
                    return true;
                }

                public static bool OYSHandle_29_HandShake(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_64_29_OpenYS_Handshake HandShake = new Packets.Packet_64_29_OpenYS_Handshake(InPacket);
                    if (HandShake.Version == Settings.Loading.OYSNetcodeVersion)
                    {
                        ThisClient.YSFClient.OpenYSSupport = true;
                        ThisClient.SetOpenYSClient();
                        Console.WriteLine(ConsoleColor.Green, ThisClient.Username + " - Client Supports Extended OpenYS Protocal Version: " + HandShake.Version.ToString());
                    }
                    else
                    {
                        Console.WriteLine(ConsoleColor.Red, ThisClient.Username + " - Different Client Extended OpenYS Protocal Version: " + HandShake.Version.ToString());
                    }
                    return true;
                }
                #endregion
            }
        }
    }
}
