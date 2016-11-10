using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenYS
{
    public static class YSFlightReplays
    {
        public static Replay ServerReplay = new Replay();

        public class Replay
        {
            private double CurrentTime = 0;
            private double PreviousTime = 0;
            private bool Loaded = false;
            private bool Loading = false;
            private bool Playing = false;
            private List<uint> AircraftIDs = new List<uint>();
            private List<PacketEvent> Records = new List<PacketEvent>();

            private PacketEvent ProcessYSFlightAircraftRecord(string[] Input, uint ID)
            {
                Packets.Packet_11_FlightData Output = new Packets.Packet_11_FlightData(3);
                if (Input.Length != 4)
                {
                    Debug.WriteLine("Failed to load a YSFlight Replay Record!");
                    foreach(string ThisString in Input)
                    {
                        Debug.WriteLine("    " + ThisString);
                    }
                    return null;
                }
                string[] RecordArray = new string[27];
                try
                {
                    RecordArray = (Input[0] + " " + Input[1] + " " + Input[2] + " " + Input[3]).Split(' ');
                    //^^ This is MUCH faster!

                    //RecordArray[0] = Input[0].Split(' ')[0];    //TimeStamp
                    
                    //RecordArray[1] = Input[1].Split(' ')[0];    //PosX - Float
                    //RecordArray[2] = Input[1].Split(' ')[1];    //PosY - Float
                    //RecordArray[3] = Input[1].Split(' ')[2];    //PosZ - Float
                    //RecordArray[4] = Input[1].Split(' ')[3];    //HdgX - Float
                    //RecordArray[5] = Input[1].Split(' ')[4];    //HdgY - Float
                    //RecordArray[6] = Input[1].Split(' ')[5];    //HdgZ - Float
                    //RecordArray[7] = Input[1].Split(' ')[6];    //LoadFactor - Float

                    //RecordArray[8]  = Input[2].Split(' ')[0];   //AirState Direct
                    //RecordArray[9]  = Input[2].Split(' ')[1];   //VGW 0-255
                    //RecordArray[10] = Input[2].Split(' ')[2];   //Boards 0-255
                    //RecordArray[11] = Input[2].Split(' ')[3];   //Gear 0-255
                    //RecordArray[12] = Input[2].Split(' ')[4];   //Flap 0-255
                    //RecordArray[13] = Input[2].Split(' ')[5];   //Brake 0-255
                    //RecordArray[14] = Input[2].Split(' ')[6];   //Smoke 0-255
                    //RecordArray[15] = Input[2].Split(' ')[7];   //??? - Leave 0
                    //RecordArray[16] = Input[2].Split(' ')[8];   //Anim Flags Direct
                    //RecordArray[17] = Input[2].Split(' ')[9];   //Strength 0-255
                    //RecordArray[18] = Input[2].Split(' ')[10];  //Throttle 0-100
                    //RecordArray[19] = Input[2].Split(' ')[11];  //Elevator - Signed Byte
                    //RecordArray[20] = Input[2].Split(' ')[12];  //Aileron - Signed Byte
                    //RecordArray[21] = Input[2].Split(' ')[13];  //Rudder - Signed Byte
                    //RecordArray[22] = Input[2].Split(' ')[14];  //Trim - Signed Byte
                    //RecordArray[23] = Input[2].Split(' ')[15];  //Thrust Vector 0-100
                    //RecordArray[24] = Input[2].Split(' ')[16];  //Thrust Reverse 0-100
                    //RecordArray[25] = Input[2].Split(' ')[17];  //Bomb Bay 0-100

                    //RecordArray[26] = Input[3].Split(' ')[0];   //Terminator
                }
                catch
                {
                    Debug.WriteLine("Failed to format a YSFlight Replay Record!");
                    foreach (string ThisString in Input)
                    {
                        Debug.WriteLine("    " + ThisString);
                    }
                    return null;
                }
                try
                {
                    Output.TimeStamp =  Single.Parse(RecordArray[0]);
                    if (Output.TimeStamp < 0) Output.TimeStamp = 0;
                    Output.ID = ID;

                    Output.PosX = Single.Parse(RecordArray[1]);
                    Output.PosY = Single.Parse(RecordArray[2]);
                    Output.PosZ = Single.Parse(RecordArray[3]);
                    Output.HdgX = (short)(Single.Parse(RecordArray[4]) / Math.PI * 32767);
                    Output.HdgY = (short)(Single.Parse(RecordArray[5]) / Math.PI * 32767);
                    Output.HdgZ = (short)(Single.Parse(RecordArray[6]) / Math.PI * 32767);
                    Output.LoadFactor = (short)(Single.Parse(RecordArray[7]) * 10);

                    Output.FlightState = Byte.Parse(RecordArray[8]);
                    Output.Anim_VGW = (byte)(Byte.Parse(RecordArray[9]) / 255d * 100d);
                    Output.Anim_Boards = (byte)(Byte.Parse(RecordArray[10]) / 255d * 100d);
                    Output.Anim_Gear = (byte)(Byte.Parse(RecordArray[11])/255d*100d);
                    Output.Anim_Flaps = (byte)(Byte.Parse(RecordArray[12]) / 255d * 100d);
                    Output.Anim_Brake = (byte)(Byte.Parse(RecordArray[13]) / 255d * 100d);
                    Output.Anim_Smoke = (Byte.Parse(RecordArray[14]) > 0);
                    //15 == ???;
                    Output._Anim_Flags |= Byte.Parse(RecordArray[16]);
                    Output.Strength = Byte.Parse(RecordArray[17]);
                    Output.Anim_Throttle = Byte.Parse(RecordArray[18]);
                    Output.Anim_Elevator = SByte.Parse(RecordArray[19]);
                    Output.Anim_Aileron = SByte.Parse(RecordArray[20]);
                    Output.Anim_Rudder = SByte.Parse(RecordArray[21]);
                    Output.Anim_Trim = SByte.Parse(RecordArray[22]);
                    Output.Anim_Nozzle = (byte)(Byte.Parse(RecordArray[23]) / 255d * 100d);
                    Output.Anim_Reverse = (byte)(Byte.Parse(RecordArray[24]) / 255d * 100d);
                    Output.Anim_BombBay = (byte)(Byte.Parse(RecordArray[25]) / 255d * 100d);

                    //26 == Terminator;
                }
                catch
                {
                    Debug.WriteLine("Failed to allocate a YSFlight Replay Record!");
                    foreach (string ThisString in Input)
                    {
                        Debug.WriteLine("    " + ThisString);
                    }
                    return null;
                }
                return new PacketEvent(Output.TimeStamp, Output);
            }

            public bool LoadReplay(string FileName)
            {
                #region Deny if already loading!
                if (Loading)
                {
                    Debug.WriteLine("Failure to load YSFlight Replay (Already Loading! Can't Dual Process!)");
                    return false;
                }
                #endregion
                #region Deny if already loaded!
                if (Loaded)
                {
                    Debug.WriteLine("Failure to load YSFlight Replay (Already Loaded! Can't OverWrite!)");
                    return false;
                }
                #endregion
                #region Deny if file not found!
                if (!Files.FileExists(FileName))
                {
                    Debug.WriteLine("Failure to load YSFlight Replay (File Not Found):\n    " + FileName);
                    return false;
                }
                #endregion
                #region load records to buffer
                string[] Contents = Files.FileReadAllLines(FileName);
                #endregion
                #region clear the records, ready to write to.
                CurrentTime = 0;
                Records.Clear();
                #endregion

                Console.WriteLine("&eLoading Replay...");
                Loading = true;

                Threads.Add(() => 

                    {
                        string Mode = "NONE";

                        #region Aircraft Data
                        string CurrentAC_Identify = "NULL";
                        uint CurrentAC_ID = 0;
                        uint CurrentAC_IFF = 0;
                        float CurrentAC_Fuel = 100;
                        string CurrentAC_Tag = "Replay";
                        #endregion

                        for (int CurrentLineNumber = 0; CurrentLineNumber < Contents.Length; CurrentLineNumber++)
                        {
                            #region prepare variables
                            string ThisLine = Contents[CurrentLineNumber];
                            string Keyword = ThisLine;
                            string Arguments = "";
                            if (Keyword.Contains(' ')) {
                                Arguments = Keyword.Split(new char[] {' '},2)[1];
                                Keyword = Keyword.Split(' ')[0].ToUpperInvariant();
                            }

                            #endregion

                            #region No Aircraft at the moment.
                            if (Mode == "NONE")
                            {
                                if (Keyword == "AIRPLANE")
                                {
                                    Mode = "AIRPLANE";
                                    CurrentAC_ID = World.Objects.GetNextID() | 256 * 256;
                                    AircraftIDs.Add(CurrentAC_ID);
                                    if (ThisLine.Contains(' ')) CurrentAC_Identify = ThisLine.Split(' ')[1];
                                    continue;
                                }
                            }
                            #endregion
                            #region Working on an aircraft.
                            if (Mode == "AIRPLANE")
                            {
                                #region get IFF
                                if (Keyword == "IDENTIFY")
                                {
                                    UInt32.TryParse(Arguments, out CurrentAC_IFF);
                                    continue;
                                }
                                #endregion
                                #region get Tag
                                if (Keyword == "IDANDTAG")
                                {
                                    if (Arguments.Contains(' '))
                                    {
                                        CurrentAC_Tag = Arguments.Split(new char[] { ' ' }, 2)[1];
                                        if (CurrentAC_Tag == "\"\"") CurrentAC_Tag = "Replay";
                                        continue;
                                    }
                                }
                                #endregion
                                #region get airpcmnd
                                if (Keyword == "AIRPCMND")
                                {
                                    if (Arguments.Contains(' '))
                                    {
                                        string[] SubArguments = Arguments.Split(' ');
                                        #region get fuel
                                        if (SubArguments.Length > 1)
                                        {
                                    
                                            try
                                            {
                                                if (SubArguments[0].ToUpperInvariant() == "INITFUEL")
                                                {
                                                    try
                                                    {
                                                        CurrentAC_Fuel = (float)MetaData._Aircraft.FindByName(CurrentAC_Identify).Cache().WEIGFUEL;
                                                    }
                                                    catch
                                                    {
                                                        Debug.WriteLine("Failed to set default fuel!");
                                                    }
                                                    try
                                                    {
                                                        CurrentAC_Fuel = CurrentAC_Fuel / 100 * UInt32.Parse(SubArguments[1].Split('%')[0]);
                                                    }
                                                    catch
                                                    {
                                                        Debug.WriteLine("Failed to load fuel: " + ThisLine);
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                Debug.WriteLine("Failed to process AIRPCMND: " + ThisLine);
                                            }
                                        }
                                        #endregion
                                        continue;
                                    }
                                }
                                #endregion
                                #region process flight records.
                                if (Keyword == "NUMRECOR")
                                {
                                    int RecordsCount = 0;
                                    if (Arguments.Contains(' ')) Int32.TryParse(Arguments.Split(' ')[0], out RecordsCount);
                                    CurrentLineNumber++;
                                    List<Packets.Packet_11_FlightData> ThisAircraftFlightDataRecords = new List<Packets.Packet_11_FlightData>();
                                    for (int i = 0; i < RecordsCount; i++) {
                                        string[] RecordContents = new string[4];
                                        try
                                        {
                                            RecordContents[0] = Contents[CurrentLineNumber];
                                            RecordContents[1] = Contents[CurrentLineNumber+1];
                                            RecordContents[2] = Contents[CurrentLineNumber+2];
                                            RecordContents[3] = Contents[CurrentLineNumber+3];
                                        }
                                        catch
                                        {
                                            Debug.WriteLine("Error reading lines for YSF Record!");
                                            continue;
                                        }
                                        CurrentLineNumber += 4;
                                        PacketEvent ThisPacketEvent = ProcessYSFlightAircraftRecord(RecordContents, CurrentAC_ID);
                                        Packets.Packet_11_FlightData FlightData = new Packets.Packet_11_FlightData(ThisPacketEvent.Packet);
                                        FlightData.Weight_Fuel = CurrentAC_Fuel;
                                        FlightData.Weight_SmokeOil = 100;
                                        FlightData.Weight_Payload = 0;
                                        #region spawn the aircraft
                                        if (i == 0)
                                        {
                                            //first event, need to add a creation packet!
                                            Packets.Packet_05_EntityJoined EntityJoined = new Packets.Packet_05_EntityJoined();
                                            EntityJoined.ID = CurrentAC_ID;
                                            EntityJoined.IFF = CurrentAC_IFF;
                                            EntityJoined.PosX = FlightData.PosX;
                                            EntityJoined.PosY = FlightData.PosY;
                                            EntityJoined.PosZ = FlightData.PosZ;
                                            EntityJoined.RotX = (float)(FlightData.HdgX / 32767 * Math.PI);
                                            EntityJoined.RotY = (float)(FlightData.HdgY / 32767 * Math.PI);
                                            EntityJoined.RotZ = (float)(FlightData.HdgZ / 32767 * Math.PI);
                                            EntityJoined.Identify = CurrentAC_Identify;
                                            EntityJoined.OwnerName = CurrentAC_Tag;
                                            EntityJoined.IsOwnedByThisPlayer = false;
                                            EntityJoined.IsOwnedByOtherPlayer = true;
                                            EntityJoined.IsAircraft = true;

                                            Records.Add(new PacketEvent(FlightData.TimeStamp, EntityJoined));
                                        }
                                        #endregion
                                        if (i > 0)
                                        {
                                            Packets.Packet_11_FlightData PrevFlightData = ThisAircraftFlightDataRecords.ToArray()[i - 1];
                                            float TimeDifference = FlightData.TimeStamp - PrevFlightData.TimeStamp;
                                            float ScalingFactor = 1f / TimeDifference;

                                            //short _X_Left = (ushort)FlightData.HdgX - (ushort)ThisAircraftFlightDataRecords.ToArray()[i - 1].HdgX; //350-010 = 340; //010-350 = -340
                                            //short _X_Right = (ushort)ThisAircraftFlightDataRecords.ToArray()[i - 1].HdgX - (ushort)FlightData.HdgX; //

                                            float ChangeYDeg = Numbers.AngleAcuteDifference(FlightData.HdgX.ToDegrees(), PrevFlightData.HdgX.ToDegrees());
                                            float ChangeXDeg = Numbers.AngleAcuteDifference(FlightData.HdgY.ToDegrees(), PrevFlightData.HdgY.ToDegrees());
                                            float ChangeZDeg = Numbers.AngleAcuteDifference(FlightData.HdgZ.ToDegrees(), PrevFlightData.HdgZ.ToDegrees());

                                            //FlightData.V_HdgX = (float)(ChangeXDeg * ScalingFactor / 180 * Math.PI);
                                            //FlightData.V_HdgY = (float)(ChangeYDeg * ScalingFactor / 180 * Math.PI);
                                            //FlightData.V_HdgZ = (float)(ChangeZDeg * ScalingFactor / 180 * Math.PI);
                                            //FlightData.V_HdgX = 0;
                                            //FlightData.V_HdgY = 2;
                                            //FlightData.V_HdgZ = 0;
                                            FlightData.V_PosX = (short)((FlightData.PosX - PrevFlightData.PosX) * ScalingFactor * 10);
                                            FlightData.V_PosY = (short)((FlightData.PosY - PrevFlightData.PosY) * ScalingFactor * 10);
                                            FlightData.V_PosZ = (short)((FlightData.PosZ - PrevFlightData.PosZ) * ScalingFactor * 10);
                                        }
                                        ThisPacketEvent.Packet = FlightData;
                                        Records.Add(ThisPacketEvent);
                                        ThisAircraftFlightDataRecords.Add(FlightData);
                                        #region despawn the aircraft
                                        if (i == RecordsCount - 1)
                                        {
                                            //Last record, need to destory the aircraft now!
                                            Packets.Packet_13_RemoveAirplane RemoveACPacket = new Packets.Packet_13_RemoveAirplane(CurrentAC_ID);
                                            Records.Add(new PacketEvent(FlightData.TimeStamp, RemoveACPacket));
                                        }
                                        continue;
                                        #endregion
                                    }
                                    Mode = "NONE";
                                    CurrentLineNumber--;
                                }
                                #endregion
                            }
                            #endregion
                        }
                        Loaded = true;
                        Loading = false;
                        Console.WriteLine("&aLoaded Replay.");
                    }, "Replay Loader: " + FileName);

                return true;
            }

            public bool PlayReplay()
            {
                Playing = true;
                return true;
            }

            public bool PauseReplay()
            {
                Playing = false;
                return true;
            }

            public bool StopReplay()
            {
                Playing = false;
                CurrentTime = 0;
                Loaded = false;
                #region despawn the aircraft
                foreach (uint ThisID in AircraftIDs)
                {
                    Packets.Packet_13_RemoveAirplane ThisRemoveAirplane = new Packets.Packet_13_RemoveAirplane(ThisID);

                    lock (Vehicles.List) Vehicles.List.RemoveAll(x => x.ID == ThisRemoveAirplane.ID);

                    foreach (Client ThisClient in Clients.YSFClients)
                    {
                        Packets.Packet_06_Acknowledgement AcknowledgeLeave = new Packets.Packet_06_Acknowledgement(2, ThisRemoveAirplane.ID);
                        //ThisClient.SendPacketGetPacket(ThisRemoveAirplane, AcknowledgeLeave);
                        ThisClient.SendPacket(ThisRemoveAirplane);
                    }
                    continue;
                }
                #endregion

                return true;
            }

            public bool SendUpdate()
            {
                if (!Loaded)
                {
                    return false;
                }
                if (Playing)
                {
                    CurrentTime += 0.1;
                }
                #region GetCurrentPackets
                List<Packets.GenericPacket> PacketsToSend =
                    (
                        from ThisPacketEvent in Records
                        where ThisPacketEvent.TimeStamp >= PreviousTime
                        where ThisPacketEvent.TimeStamp < CurrentTime
                        select ThisPacketEvent.Packet
                    ).ToList();
                if (PacketsToSend.Count == 0)
                {
                    return false;
                }
                List<Packets.Packet_11_FlightData> FlightDataPacketsToSend =
                    (
                        from ThisFlightDataPacket in PacketsToSend
                        where ThisFlightDataPacket.Type == 11
                        select new Packets.Packet_11_FlightData(ThisFlightDataPacket)
                    ).OrderByDescending(x => x.TimeStamp).ToList();
                #endregion

                List<Client> CurrentClients = Clients.LoggedIn;
                foreach (uint ThisACID in AircraftIDs)
                {
                    #region GetFlightDataFromThisAircraft
                    List<Packets.Packet_11_FlightData> ThisAircraftFlightData = 
                        (
                            from ThisFlightDataPacket in FlightDataPacketsToSend
                            where ThisFlightDataPacket.ID == ThisACID
                            select ThisFlightDataPacket
                        ).ToList();
                    #endregion
                    if (ThisAircraftFlightData.Count < 1) continue;

                    #region GetLatestFlightData
                    Packets.Packet_11_FlightData NewesetFlightData =
                        (from ThisFlightDataPacket in FlightDataPacketsToSend
                         where ThisFlightDataPacket.ID == ThisACID
                         select ThisFlightDataPacket).ToArray().Last();

                    #endregion

                    #region UpdateFlightData
                    foreach (Vehicle ThisVehicle in Vehicles.List.Where(x=>x.ID == ThisACID))
                    {
                        ThisVehicle.Update(NewesetFlightData);
                    }
                    #endregion

                    float difference = (float)(NewesetFlightData.TimeStamp - PreviousTime);
                    foreach (Client ThisClient in CurrentClients)
                    {
                        #region SetTimeToNow
                        NewesetFlightData.TimeStamp = (float)((DateTime.Now - OpenYS.TimeStarted).TotalSeconds - (CurrentTime - PreviousTime) + difference);
                        #endregion

                        //Debug.WriteLine(NewesetFlightData.TimeStamp + " Ready to send to " + ThisClient.Username);
                        ThisClient.SendPacket(NewesetFlightData);
                        //Debug.WriteLine(NewesetFlightData.TimeStamp + " Send Flight Data.");
                    }
                    continue;
                }

                foreach (Packets.GenericPacket ThisPacket in PacketsToSend)
                {
                    if (ThisPacket.Type == 11) continue;

                    #region Join Data
                    if (ThisPacket.Type == 05)
                    {
                        Packets.Packet_05_EntityJoined ThisJoinData = new Packets.Packet_05_EntityJoined(ThisPacket);

                        #region Create Vehicle
                        Vehicle ThisVehicle = new Vehicle();
                        ThisVehicle.Update(ThisJoinData);
                        //ThisJoinData = ThisVehicle.GetJoinPacket(false);
                        #endregion

                        #region Add Vehicle to Vehicles List
                        Vehicles.List.Add(ThisVehicle);
                        VehiclesHistory.List.Add(ThisVehicle);
                        #endregion

                        foreach (Client ThisClient in CurrentClients)
                        {
                            Packets.Packet_06_Acknowledgement Acknowledgement;
                            if (ThisJoinData.IsAircraft) Acknowledgement = new Packets.Packet_06_Acknowledgement(0, ThisJoinData.ID);
                            else Acknowledgement = new Packets.Packet_06_Acknowledgement(1, 0);
                            //ThisClient.SendPacketGetPacket(ThisJoinData, Acknowledgement);
                            ThisClient.SendPacket(ThisJoinData);
                        }
                        continue;
                    }
                    #endregion

                    #region Leave Data
                    if (ThisPacket.Type == 13)
                    {
                        Packets.Packet_13_RemoveAirplane ThisRemoveAirplane = new Packets.Packet_13_RemoveAirplane(ThisPacket);

                        lock (Vehicles.List) Vehicles.List.RemoveAll(x => x.ID == ThisRemoveAirplane.ID);

                        foreach (Client ThisClient in CurrentClients)
                        {
                            Packets.Packet_06_Acknowledgement AcknowledgeLeave = new Packets.Packet_06_Acknowledgement(2, ThisRemoveAirplane.ID);
                            //ThisClient.SendPacketGetPacket(ThisRemoveAirplane, AcknowledgeLeave);
                            ThisClient.SendPacket(ThisRemoveAirplane);
                        }
                        continue;
                    }
                    #endregion

                    CurrentClients.SendPacket(ThisPacket);
                }
                if (Playing)
                {
                    PreviousTime = CurrentTime;
                }
                return true;
            }

            public bool IsLoading()
            {
                return Loading;
            }

            public bool IsLoaded()
            {
                return Loaded;
            }

            public bool IsPlaying()
            {
                return Playing;
            }

            public bool IsStopped()
            {
                return (!Playing & (CurrentTime == 0));
            }
        }

        public class PacketEvent
        {
            public float TimeStamp;
            public Packets.GenericPacket Packet;

            public PacketEvent(float _TimeStamp, Packets.GenericPacket _Packet)
            {
                TimeStamp = _TimeStamp;
                Packet = _Packet;
            }
        }
    }
}