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
        public static partial class ServerMode
        {
            public static partial class FromClientToServer
            {
                public static bool YSFHandle(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region SWITCH
                    if (InPacket == null | InPacket == Packets.NoPacket)
                    {
                        return false;
                    }
                    switch (InPacket.Type)
                    {
                        case 0:
                            //Disconnecting Client... Ignore.
                            break;
                        case 1:
                            YSFHandle_01_Login(ThisClient, InPacket);
                            break;
                        case 4:
                            //Field Name - Ignore.
                            break;
                        case 6:
                            //Acknowledge Packet - Ignore.
                            break;
                        case 7:
                            //Aircraft smoke color, handle it later...
                            Clients.AllClients.SendPacket(InPacket);
                            break;
                        case 8:
                            YSFHandle_08_JoinRequest(ThisClient, InPacket);
                            break;
                        case 9:
                            //Join Approved - Ignore.
                            break;
                        case 10:
                            //Join Denied - Ignore.
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
                        case 16:
                            //Prepare Simulation - Ignore.
                            break;
                        case 17:
                            //Heartbeat.
                            ThisClient.LastHeartBeat = DateTime.Now;
                            break;
                        case 18:
                            //X locks onto Y.
                            Clients.AllClients.Exclude(ThisClient).SendPacket(InPacket);
                            break;
                        case 20:
                            //Spawn Particle or Ordinance                            
                            Clients.AllClients.Exclude(ThisClient).SendPacket(InPacket);
                            break;
                        case 22:
                            //Damage
                            Clients.AllClients.SendPacket(InPacket);
                            break;
                        case 25:
                            ThisClient.SendPacket(new Packets.Packet_10_JoinDenied());
                            //SendMessage("Error Handling Join Request - Please Re-Join!");
                            break;
                        case 32:
                            YSFHandle_32_ChatMessage(ThisClient, InPacket);
                            break;
                        case 33:
                            //Weather
                            YSFHandle_33_Weather(ThisClient, InPacket);
                            break;
                        case 34:
                            ThisClient.SendPacket(new Packets.Packet_10_JoinDenied());
                            ThisClient.SendMessage("Error Handling Join Request - Please Re-Join!");
                            break;
                        case 36:
                            //aircraft loading, handle it later...
                            YSFHandle_36_WeaponsLoading(ThisClient, InPacket);
                            break;
                        case 37:
                            YSFHandle_37_ListUser(ThisClient, InPacket);
                            break;
                        case 43:
                            //MiscCommand - Ignore.
                            break;
                        case 44:
                            //Aircraft List - Ignore.
                            break;
                        case 64:
                            //OpenYS User Data Packet
                            YSFHandle_64_UserPacket(ThisClient, InPacket);
                            break;
                        default:
#if DEBUG
                            //Console.WriteLine("Unknown Packet From " + ThisClient.Username);
                            //Console.WriteLine("-Type: " +  InPacket.Type.ToString());
                            //Console.WriteLine("-Data: " +  InPacket.Data.ToString());
#endif
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
                    ThisClient.SetYSFlightClient();
                    ThisClient.SetConnected();
                    ThisClient.SetLoggingIn();
                    ThisClient.SetIdle();
                    #endregion

                    #region Make OP
                    if (Settings.Loading.AutoOPs)
                    {
                        //The below checks if the user is from a local ip endpoint (From Wikipedias definitions...)
                        //If so, the user is made an OP.
                        if ((ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).AddressFamily == AddressFamily.InterNetwork && ThisClient.Username != "PHP bot")
                        {
                            int byte0 = Int32.Parse((ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString().Split('.')[0]);
                            int byte1 = Int32.Parse((ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString().Split('.')[1]);
                            int byte2 = Int32.Parse((ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString().Split('.')[2]);
                            int byte3 = Int32.Parse((ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString().Split('.')[3]);

                            //10.*.*.*
                            if (byte0 == 10) goto MakeOP;

                            //127.0.0.1
                            if (byte0 == 127 &
                                byte1 == 0 &
                                byte2 == 0 &
                                byte3 == 1) goto MakeOP;

                            //192.168.*.*
                            if (byte0 == 192 &
                                byte1 == 168) goto MakeOP;

                            //172.16.*.* - 172.31.*.*
                            if (byte0 == 172 &
                                byte1 >= 16 &
                                byte1 <= 31) goto MakeOP;

                            //NOT a local ip, so not an OP.
                            goto NotOP;


                        MakeOP:
                            if (Settings.Options.AllowOPs)
                            {
                                //Console.WriteLine(ConsoleColor.Red, "Connecting Client is from a local ip address (" + (ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString() + ").\nMade them an OP.");
                                ThisClient.OP();
                            }

                        NotOP:
                            //Do Nothing.
                            ;
                        }
                    }
                    #endregion

                    #region Get Complete UserName (Old Clients)
                    if (ThisClient.Version <= 20120207 & ThisClient.Username.Length >= 15)
                    {
                        ThisClient.SendMessage("You are using an old version of YSFlight");
                        ThisClient.SendMessage("Please verify your username before continuing!");
                        ThisClient.SendMessage("");
                        ThisClient.SendMessage("Please type a blank string. (Press F12 and then press enter)");
                        ThisClient.SendMessage("IT IS IMPORTANT YOU DON'T TYPE ANYTHING, JUST A BLANK LINE!");
                        while (true) {
                            string EmptyString = "";
                            Packets.GenericPacket _NewPacket = ThisClient.YSFClient.ReceivePacket();
                            if (_NewPacket.Type == 32)
                            {
                                Packets.Packet_32_ChatMessage MessagePacket = new Packets.Packet_32_ChatMessage(_NewPacket);
                                EmptyString = MessagePacket.FullMessage;
                                if (!EmptyString.StartsWith("(") | !EmptyString.EndsWith(")"))
                                {
                                    ThisClient.SendMessage("Sorry, that doesn't look right... YOU MUST USE A BLANK STRING. Try again!");
                                    continue;
                                }
                                ThisClient.Username = EmptyString.Substring(1, EmptyString.Length - 2);
                                Debug.WriteLine("Got Username from old client! (" + ThisClient.Username + ")");
                                ThisClient.SendMessage("Thanks for that! Logging you in...");
                                ThisClient.SendMessage("(You wouldn't have this problem if you just upgrade YSF Version you know...)");
                                break;
                            }
                            else
                            {
                                ThisClient.YSFClient.ProcessPacket(_NewPacket);
                            }

                        }
                    }
                    #endregion

                    #region Send Join Message
                    //finally, send the jointext.
                    if (Files.FileExists("AutoMessages/_1_StartLogIn.txt") & Settings.Loading.SendConnectedWelcomeMessage)
                    {
                        string[] msg = Files.FileReadAllLines("AutoMessages/_1_StartLogIn.txt");
                        string output = "";
                        foreach (string ThisLine in msg)
                        {
                            if (output.Length > 0) output += "\n";
                            output += ThisLine;
                        }
                        //Send the join info packet.
                        ThisClient.SendMessage(output);
                    }
                    #endregion

                    #region Bot Handling
                    if (Clients.BotNames.Contains(ThisClient.Username)) ThisClient.SetBot();

                    if (!ThisClient.IsBot())
                    {

                        if (ThisClient.IsOP()) Console.WriteLine(ConsoleColor.Yellow, ThisClient.Username + "&e Logging in... " + (ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString());
                        else Console.WriteLine(ConsoleColor.Yellow, ThisClient.Username + " Logging in... &7" + (ThisClient.YSFClient.Socket.RemoteEndPoint as IPEndPoint).Address.ToString());
                    }
                    #endregion

                    #region OpenYS Handshake
                    //When the client sends the server a login packet, tell the client this server supports the OpenYS protocal!
                    if (Settings.Options.AllowOYSFramework)
                    {
                        ThisClient.SendPacket(new Packets.Packet_64_29_OpenYS_Handshake(Settings.Loading.OYSNetcodeVersion).ToCustomPacket());
                    }
                    #endregion

                    #region Inform Players
                    if (Settings.Flight.JoinFlightNotification)
                    {
                        if (!ThisClient.IsBot()) Clients.AllClients.Exclude(ThisClient).SendMessage("&a" + ThisClient.Username + " Joined the server.");
                    }
                    #endregion

                    #region Send Version(29)
                    //Build Version (29)
                    uint NetcodeVersion = ThisClient.Version;
                    if (Settings.Loading.YSFNetcodeVersion != 0)
                    {
                        //If the netcode version is set to 0, use the clients version (Silences the "You are using a different version..." error.)
                        NetcodeVersion = Settings.Loading.YSFNetcodeVersion;
                    }
                    Packets.Packet_29_VersionNotify VersionPacket = new Packets.Packet_29_VersionNotify(NetcodeVersion);
                    Packets.Packet_06_Acknowledgement AcknowledgeVersionPacket = new Packets.Packet_06_Acknowledgement(9, 0);
                    ClientIO.WaitForPacketObject WaitForAcknowledgeVersion = ThisClient.YSFClient.BeginWait(AcknowledgeVersionPacket);

                    ThisClient.SendPacket(VersionPacket);
                    #endregion

                    #region Send MissilesOption (31)
                    //Build MissilesOption (31)
                    Packets.Packet_31_MissilesOption MissilesOption = OpenYS.Missiles;
                    Packets.Packet_06_Acknowledgement AcknowledgeMissilesPacket = new Packets.Packet_06_Acknowledgement(10, 0);
                    ClientIO.WaitForPacketObject WaitForAcknowledgeMissiles = ThisClient.YSFClient.BeginWait(AcknowledgeMissilesPacket);
                    ThisClient.SendPacket(MissilesOption);
                    #endregion

                    #region Send WeaponsOption (39)
                    //Build WeaponsOption (39)
                    Packets.Packet_39_WeaponsOption WeaponsOption = OpenYS.Weapons;
                    Packets.Packet_06_Acknowledgement AcknolwedgeWeapons = new Packets.Packet_06_Acknowledgement(11, 0);
                    ClientIO.WaitForPacketObject WaitForAcknolwedgeWeapons = ThisClient.YSFClient.BeginWait(AcknolwedgeWeapons);

                    //Send WeaponsOption (39)
                    ThisClient.SendPacket(WeaponsOption);
                    #endregion

                    #region Get Version(06:09)
                    if (!ThisClient.IsBot())
                    {
                        int Failures = 0;
                        while (!WaitForAcknowledgeVersion.EndWait(500))
                        {
                            Failures++;
                            ThisClient.SendPacket(VersionPacket);
                            if (Failures >= 5)
                            {
                                Debug.WriteLine("Failed to get Acknolwedgement of Version from " + ThisClient.Username);
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Send UsernameDistance(41)
                    //Build UsernameDistance (41)
                    Packets.Packet_41_UsernameDistance UsernameDistance = OpenYS.UsernameDistance;
                    ClientIO.WaitForPacketObject WaitForUsernameDistance = ThisClient.YSFClient.BeginWait(UsernameDistance);

                    //Send UsernameDistance (41)
                    ThisClient.SendPacket(UsernameDistance);
                    #endregion

                    #region Get MissilesOption(06:10)
                    if (!ThisClient.IsBot())
                    {
                        int Failures = 0;
                        while (!WaitForAcknowledgeMissiles.EndWait(500))
                        {
                            Failures++;
                            ThisClient.SendPacket(MissilesOption);
                            if (Failures >= 5)
                            {
                                Debug.WriteLine("Failed to get Acknolwedgement of Missiles from " + ThisClient.Username);
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Send RadarAltitude(43)
                    //Build RadarAltitude (43)
                    Packets.Packet_43_MiscCommand RadarAltitude = OpenYS.RadarAltitude;
                    ClientIO.WaitForPacketObject WaitForRadarAltitude = ThisClient.YSFClient.BeginWait(RadarAltitude);

                    //Send RadarAltitude (43)
                    ThisClient.SendPacket(RadarAltitude);
                    #endregion

                    #region Send No External View(43)
                    //Build RadarAltitude (43)
                    Packets.Packet_43_MiscCommand NoExAirView = OpenYS.NoExternalAirView;
                    ClientIO.WaitForPacketObject WaitForNoExtAirView = ThisClient.YSFClient.BeginWait(NoExAirView);
                    //Listener Listen43NoExAirView = //ThisClient.YSFClient.ExpectPacket(RadarAltitude);

                    //Send RadarAltitude (43)
                    if (NoExAirView.Argument == "TRUE") ThisClient.SendPacket(NoExAirView);
                    #endregion

                    #region Get WeaponsOption(06:11)
                    if (!ThisClient.IsBot())
                    {
                        int Failures = 0;
                        while (!WaitForAcknolwedgeWeapons.EndWait(500))
                        {
                            Failures++;
                            ThisClient.SendPacket(WeaponsOption);
                            if (Failures >= 5)
                            {
                                Debug.WriteLine("Failed to get Acknolwedgement of Weapons from " + ThisClient.Username);
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Send Field(04)
                    //Build Field (04)
                    Packets.Packet_04_FieldName FieldName = OpenYS.Field;
                    ClientIO.WaitForPacketObject WaitForFieldName = ThisClient.YSFClient.BeginWait(FieldName);

                    //Send Field (04)
                    ThisClient.SendPacket(FieldName);
                    #endregion

                    #region Get Field(04)
                    //Get Field (4)
                    if (!ThisClient.IsBot())
                    {
                        int Failures = 0;
                        while (!WaitForFieldName.EndWait(500))
                        {
                            Failures++;
                            ThisClient.SendPacket(FieldName);
                            if (Failures >= 5)
                            {
                                Debug.WriteLine("Failed to get Acknolwedgement of FieldName from " + ThisClient.Username);
                                break;
                            }
                        }
                    }
                    #endregion

                    #region Get WeatherRequest(33)
                    //Deprecated. Server should pick this up by itself now.
                    //ThisClient.SendPacket(ServerWeatherResponse);
                    #endregion

                    #region SendUserList (If Bot!)
                    if (ThisClient.IsBot())
                    {
                        YSFHandle_37_ListUser(ThisClient, new Packets.Packet_37_ListUser());
                    }
                    #endregion

                    #region Send AircraftList(44)
                    //Process the Aircraft List.
                    List<MetaData.Aircraft> MetaAircraftList = new List<MetaData.Aircraft>();
                    int Percentage = 10;
                    for (int i = 0; i < MetaData._Aircraft.List.Count; i++)
                    {
                        #region Tell YSClient the Percentage
                        if ((decimal)i / (decimal)(MetaData._Aircraft.List.Count - 1) * 100 >= Percentage)
                        {
                            if (Settings.Loading.SendLoadingPercentNotification)
                            {
                                if (Percentage == 100) ThisClient.SendMessage("Sending Aircraft List: " + Percentage + "% Complete!");
                                else ThisClient.SendMessage("Sending Aircraft List: " + Percentage + "% Complete...");
                            }
                            Percentage += 10;
                        }
                        #endregion

                        MetaAircraftList.Add(MetaData._Aircraft.List[i]);
                        if (MetaAircraftList.Count >= Settings.Loading.AircraftListPacketSize)
                        {
                            #region Prepare Aircraft List
                            //Build AircraftList (44)
                            Packets.Packet_44_AircraftList ThisAircraftListPacket = new Packets.Packet_44_AircraftList
                                (
                                1,
                                (byte)MetaAircraftList.Count(),
                                MetaAircraftList.Select(y => y.Identify.Split('\0')[0]).ToArray()
                                );
                            MetaAircraftList.Clear();
                            #endregion

                            #region Send AircraftList(44)
                            //Client.Connection.PacketWaiter Listen44AircraftList = //ThisClient.YSFClient.ExpectPacket(new Packets.Packet_44_AircraftList(ThisAircraftListPacket));
                            //Send AircraftList (44)
                            if (!ThisClient.IsBot())
                            {
                                new ClientIO.SocketOperation
                                    (
                                        new ClientIO.DataEvent(ClientIO.SendOperation, ThisClient, ThisAircraftListPacket),
                                        new ClientIO.DataEvent(ClientIO.ReceiveOperation, ThisClient, ThisAircraftListPacket)
                                    ).Run();
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region Disconnect Bots
                    if (Settings.Loading.KickBots)
                    {
                        if (ThisClient.IsBot())
                        {
                            ThisClient.SendMessage("Domo arigato for checking this OpenYS server, anonymous robot-sama. Sayonara! ^_^");
                            ThisClient.Disconnect("Bot kicked after logging in.");
                            return true;
                        }
                    }
                    #endregion

                    #region Update FogColor
                    if (Settings.Weather.Advanced.EnableFogColor & ThisClient.Version > 20110207)
                    {
                        Packets.Packet_48_FogColor FogColor = new Packets.Packet_48_FogColor(OpenYS.AdvancedWeatherOptions.FogColor.Red, OpenYS.AdvancedWeatherOptions.FogColor.Green, OpenYS.AdvancedWeatherOptions.FogColor.Blue);
                        ThisClient.SendPacket(FogColor);
                    }
                    #endregion
                    #region Update SkyColor
                    if (Settings.Weather.Advanced.EnableSkyColor & ThisClient.Version > 20110207)
                    {
                        Packets.Packet_49_SkyColor SkyColor = new Packets.Packet_49_SkyColor(OpenYS.AdvancedWeatherOptions.SkyColor.Red, OpenYS.AdvancedWeatherOptions.SkyColor.Green, OpenYS.AdvancedWeatherOptions.SkyColor.Blue);
                        ThisClient.SendPacket(SkyColor);
                    }
                    #endregion
                    #region Update GndColor
                    if (Settings.Weather.Advanced.EnableGndColor & ThisClient.Version > 20110207)
                    {
                        Packets.Packet_50_GroundColor GndColor = new Packets.Packet_50_GroundColor(OpenYS.AdvancedWeatherOptions.GndColor.Red, OpenYS.AdvancedWeatherOptions.GndColor.Green, OpenYS.AdvancedWeatherOptions.GndColor.Blue);
                        ThisClient.SendPacket(GndColor);
                    }
                    #endregion

                    #region Send PrepareSimulation(16)
                    //Build Prepare Simulation (16)
                    Packets.Packet_16_PrepareSimulation PrepareSimulation = new Packets.Packet_16_PrepareSimulation();

                    //Send Prepare Simulation (16)
                    ThisClient.SendPacket(PrepareSimulation);
                    ThisClient.SetLoggedIn();
                    #endregion

                    #region Get PrepareSimulation(06:07)
                    //Get Acknowledgement (7:0) (Acknowledge Packets.Packet_16_PrepareSimulation)
                    if (!ThisClient.IsBot())
                    {
                        //ThisClient.YSFClient.ExpectAcknowledgement(7, 0);
                    }
                    #endregion

                    #region Complete Login!
                    Console.WriteLine(ConsoleColor.Green, ThisClient.Username + " Login Complete!");
                    if (Settings.Loading.SendLoginCompleteNotification)
                    {
                        ThisClient.SendMessage("*** Login Complete! ***");
                    }
                    #endregion

                    #region Send WelcomeText
                    //finally, send the loggedin.
                    if (Files.FileExists("AutoMessages/_1_EndLogIn.txt") && Settings.Loading.SendLogInCompleteWelcomeMessage)
                    {
                        string[] msg = Files.FileReadAllLines("AutoMessages/_1_EndLogIn.txt");
                        string output = "";
                        foreach (string ThisLine in msg)
                        {
                            if (output.Length > 0) output += "\n";
                            output += ThisLine;
                        }
                        //Send the join info packet.
                        ThisClient.SendMessage(output);
                    }
                    #endregion

                    #region Send Entities(05)
                    //Create all the other players aircraft.
                    //foreach (Client OtherClient in Clients.AllClients.Where(x => x.Vehicle != null).Where(y => y.Vehicle != Vehicles.NoVehicle).ToArray())
                    //{
                    //    //Client.Connection.PacketWaiter ThisListener = //ThisClient.YSFClient.ExpectAcknowledgement(0, OtherClient.Vehicle.ID);
                    //    Packets.Packet_06_Acknowledgement AcknowledgeJoin = new Packets.Packet_06_Acknowledgement(0, OtherClient.Vehicle.ID);
                    //    ThisClient.SendPacketGetPacket(OtherClient.Vehicle.GetJoinPacket(false), AcknowledgeJoin);
                    //}

                    foreach (Vehicle ThisVehicle in Vehicles.List.ToArray())
                    {
                        Packets.Packet_06_Acknowledgement AcknowledgeJoin = new Packets.Packet_06_Acknowledgement(0, ThisVehicle.ID);
                        //ThisClient.SendPacketGetPacket(ThisVehicle.GetJoinPacket(false), AcknowledgeJoin);
                        ThisClient.SendPacket(ThisVehicle.GetJoinPacket(false));

                        Packets.Packet_36_WeaponsConfig Loading = ThisVehicle.WeaponsLoading;
                        ThisClient.SendPacket(Loading);

                        if (ThisVehicle.VirtualCarrierObject_ID != 0)
                        {
                            MetaData.Ground VirtualCarrierObject = ThisVehicle.VirtualCarrierObject_MetaData;
                            ThisVehicle.VirtualCarrierObject_MetaData = VirtualCarrierObject;

                            if (VirtualCarrierObject != MetaData._Ground.None)
                            {
                                Packets.Packet_05_EntityJoined VCOJoined; 
                                VCOJoined = new Packets.Packet_05_EntityJoined();
                                VCOJoined.IsGround = true;
                                VCOJoined.ID = ThisVehicle.VirtualCarrierObject_ID;
                                VCOJoined.IFF = ThisVehicle.IFF;
                                VCOJoined.PosX = ThisVehicle.PosX;
                                VCOJoined.PosY = ThisVehicle.PosY;
                                VCOJoined.PosZ = ThisVehicle.PosZ;
                                VCOJoined.RotX = (float)(ThisVehicle.HdgX / 32767d * Math.PI);
                                VCOJoined.RotY = (float)(ThisVehicle.HdgY / 32767d * Math.PI);
                                VCOJoined.RotZ = (float)(ThisVehicle.HdgZ / 32767d * Math.PI);
                                VCOJoined.Identify = VirtualCarrierObject.Identify;
                                VCOJoined.OwnerName = ThisVehicle.OwnerName;
                                VCOJoined.IsOwnedByThisPlayer = false;

                                ThisClient.SendPacket(VCOJoined);
                            }
                        }
                    }

                    //Create all the ground objects.
                    if (Settings.Options.AllowGrounds)
                    {

                        foreach (World.Objects.Ground ThisGround in World.Objects.GroundList.ToArray())
                        {
                            Packets.Packet_05_EntityJoined GroundJoin = new Packets.Packet_05_EntityJoined();
                            GroundJoin.IsGround = true;
                            GroundJoin.ID = ThisGround.ID;
                            GroundJoin.Identify = ThisGround.Identify;
                            GroundJoin.OwnerName = ThisGround.Tag;
                            GroundJoin.IFF = ThisGround.IFF;
                            GroundJoin.PosX = ThisGround.Position.X;
                            GroundJoin.PosY = ThisGround.Position.Y;
                            GroundJoin.PosZ = ThisGround.Position.Z;
                            GroundJoin.RotX = (float)(ThisGround.Attitude.X / 180 * Math.PI);
                            GroundJoin.RotY = (float)(ThisGround.Attitude.Y / 180 * Math.PI);
                            GroundJoin.RotZ = (float)(ThisGround.Attitude.Z / 180 * Math.PI);

                            ThisClient.SendPacket(GroundJoin);
                        }
                    }
                    #endregion

                    #region DEBUG TESTING
#if DEBUG
                    Packets.Packet_05_EntityJoined GroundJoin2 = new Packets.Packet_05_EntityJoined();
                    GroundJoin2.IsGround = true;
                    GroundJoin2.ID = 99999;
                    GroundJoin2.Identify = "[OPENYS]CVX-15V_DEBUG";
                    GroundJoin2.OwnerName = "DEBUG TEST";
                    GroundJoin2.IFF = 0;
                    GroundJoin2.PosX = 0;
                    GroundJoin2.PosY = 0;
                    GroundJoin2.PosZ = 100;
                    GroundJoin2.RotX = 0;
                    GroundJoin2.RotY = 0;
                    GroundJoin2.RotZ = 0;

                    ThisClient.SendPacket(GroundJoin2);

                    Packets.Packet_21_GroundData GroundData2 = new Packets.Packet_21_GroundData(1);
                    GroundData2._Anim_Flags = 0;
                    GroundData2._CPU_Flags = 0;
                    GroundData2.HdgX = 0;
                    GroundData2.HdgY = 0;
                    GroundData2.HdgZ = 0;
                    GroundData2.ID = 99999;
                    GroundData2.PosX = 0;
                    GroundData2.PosY = 0;
                    GroundData2.PosZ = 100;
                    GroundData2.Strength = 10;
                    GroundData2.TimeStamp = OpenYS.ServerTimestamp();
                    GroundData2.V_PosX = 0;
                    GroundData2.V_PosY = 0;
                    GroundData2.V_PosZ = 10; //this is being ignored! Works for "TRUCK" ground object...
                    GroundData2.V_Rotation = 0;

                    ThisClient.SendPacket(GroundData2);
#endif
                    #endregion

                    return true;

                }

                private static bool YSFHandle_08_JoinRequest(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region Join Request Pending
                    if (ThisClient.JoinRequestPending)
                    {
                        Debug.WriteLine("Join Request ALREADY Pending!!!");
                        ThisClient.SendMessage("Oh Dear - looks like there was a problem with your last join request, let's try again!");
                        Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                        ThisClient.SendPacket(JoinDenied);
                        ThisClient.JoinRequestPending = false;
                        return true;
                    }
                    ThisClient.JoinRequestPending = true;
                    #endregion

                    #region Convert To JoinRequest
                    Packets.Packet_08_JoinRequest JoinRequest = new Packets.Packet_08_JoinRequest(InPacket);
                    #region Re-Write To JoinRequest if ResumingFlight
                    if (ThisClient.SpawnTargetVehicle > 0)
                    {
                        Vehicle TargetVehicle = Vehicles.NoVehicle;
                        TargetVehicle = VehiclesHistory.Find(ThisClient.SpawnTargetVehicle);
                        if (TargetVehicle == Vehicles.NoVehicle)
                        {
                            ThisClient.SendMessage("Can't resume flight as " + ThisClient.SpawnTargetVehicle + " because the vehiclehistory cannot be found!");
                            Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                            ThisClient.SendPacket(JoinDenied);
                            ThisClient.JoinRequestPending = false;
                            ThisClient.SpawnTargetVehicle = 0;
                            return true;
                        }
                        JoinRequest = new Packets.Packet_08_JoinRequest(InPacket);
                        JoinRequest.AircraftName = TargetVehicle.Identify;
                        JoinRequest.IFF = TargetVehicle.IFF;
                    }

                    #endregion
                    #endregion

                    #region Deny Requests if server is join locked!
                    if (Settings.Server.JoinLocked)
                    {
                        //Reject the join request - don't have the start position requested!
                        Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                        ThisClient.SendPacket(JoinDenied);
                        ThisClient.SendMessage("Join request denied - Server is join locked!");
                        ThisClient.JoinRequestPending = false;
                        return false;
                    }
                    #endregion

                    #region Acknowledge Join Request
                    //Acknowledge Join Request.
                    Packets.Packet_06_Acknowledgement AcknowledgeRequest = new Packets.Packet_06_Acknowledgement(5, 0);
                    ThisClient.SendPacket(AcknowledgeRequest);
                    #endregion

                    #region Check For Requested STP
                    //Check if Server has STP - deny if it doesn't!
                    if (!World.Objects.StartPositionList.Select(x => Strings.Resize(x.Identify, 32)).Contains(JoinRequest.StartPositionName))
                    {
                        //Reject the join request - don't have the start position requested!
                        Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                        ThisClient.SendPacket(JoinDenied);
                        ThisClient.SendMessage("Join request denied - Server does not have that start position installed!");
                        ThisClient.JoinRequestPending = false;
                        return false;
                    }
                    World.Objects.StartPosition StartPosition = World.Objects.StartPositionList.Where(x => Strings.Resize(x.Identify, 32) == JoinRequest.StartPositionName).ToArray()[0];
                    #endregion

                    #region Check For Requested Aircraft
                    if (MetaData._Aircraft.FindByName(JoinRequest.AircraftName) == MetaData._Aircraft.None)
                    {
                        //Reject the join request - don't have the aircraft requested!
                        Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                        ThisClient.SendPacket(JoinDenied);
                        ThisClient.SendMessage("Join request denied - Server does not have that aircraft installed!");
                        ThisClient.JoinRequestPending = false;
                        return false;
                    }
                    MetaData.Aircraft MetaAircraft = MetaData._Aircraft.FindByName(JoinRequest.AircraftName);
                    CachedData.Aircraft CachedAircraft = MetaAircraft.Cache();
                    #endregion

                    #region Build EntityJoined(05)
                    Packets.Packet_05_EntityJoined EntityJoined = new Packets.Packet_05_EntityJoined();
                    EntityJoined.IsAircraft = true;
                    EntityJoined.ID = World.Objects.GetNextID() | 256 * 256;
                    EntityJoined.IFF = JoinRequest.IFF;
                    EntityJoined.PosX = (float)StartPosition.Position.X;
                    EntityJoined.PosY = (float)StartPosition.Position.Y;
                    EntityJoined.PosZ = (float)StartPosition.Position.Z;
                    EntityJoined.RotX = (float)((StartPosition.Attitude.X) / 180 * Math.PI);
                    EntityJoined.RotY = (float)((StartPosition.Attitude.Y) / 180 * Math.PI);
                    EntityJoined.RotZ = (float)((StartPosition.Attitude.Z) / 180 * Math.PI);
                    EntityJoined.Identify = JoinRequest.AircraftName;
                    EntityJoined.OwnerName = ThisClient.Username;
                    EntityJoined.IsOwnedByThisPlayer = true;

                    #region Re-Write EntityJoined if Resuming Flight
                    if (ThisClient.SpawnTargetVehicle > 0)
                    {
                        Vehicle TargetVehicle = Vehicles.NoVehicle;
                        TargetVehicle = VehiclesHistory.Find(ThisClient.SpawnTargetVehicle);
                        if (TargetVehicle == Vehicles.NoVehicle)
                        {
                            ThisClient.SendMessage("Can't resume flight as " + ThisClient.SpawnTargetVehicle + " because the vehiclehistory cannot be found!");
                            Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                            ThisClient.SendPacket(JoinDenied);
                            ThisClient.JoinRequestPending = false;
                            ThisClient.SpawnTargetVehicle = 0;
                            return true;
                        }
                        Packets.Packet_05_EntityJoined OldEntityJoined = EntityJoined;
                        EntityJoined = TargetVehicle.GetJoinPacket(true);
                        EntityJoined.ID = OldEntityJoined.ID;
                        EntityJoined.OwnerName = ThisClient.Username;
                    }
                    #endregion
                    #endregion

                    #region Build Flight Data Packet
                    Packets.Packet_11_FlightData FlightData = new Packets.Packet_11_FlightData(3);
                    FlightData.ID = EntityJoined.ID;

                    FlightData.Weight_Fuel = (float)(((double)(CachedAircraft.WEIGFUEL)) * (JoinRequest.FuelPercent / 100d));
                    FlightData.Weight_SmokeOil = 100;
                    FlightData.Ammo_GUN = (ushort)CachedAircraft.INITIGUN;
                    if (!OpenYS.Weapons.Enabled) FlightData.Ammo_GUN = 0;
                    FlightData.Strength = (short)CachedAircraft.STRENGTH;
                    FlightData.Anim_Throttle = (byte)(StartPosition.Throttle * 100);
                    if (StartPosition.Gear) FlightData.Anim_Gear = 255;
                    FlightData.TimeStamp = 0;
                    FlightData.PosX = EntityJoined.PosX;
                    FlightData.PosY = EntityJoined.PosY;
                    if (FlightData.PosY < 1) FlightData.PosY = 1;
                    FlightData.PosZ = EntityJoined.PosZ;
                    FlightData.HdgX = (short)(StartPosition.Attitude.X / 180 * 32767);
                    FlightData.HdgY = (short)(StartPosition.Attitude.Y / 180 * 32767);
                    FlightData.HdgZ = (short)(StartPosition.Attitude.Z / 180 * 32767);

                    FlightData.V_PosX = (short)(Math.Sin(-StartPosition.Attitude.X * Math.PI / 180) * (StartPosition.Speed * 10)); //meters per second.
                    FlightData.V_PosZ = (short)(Math.Cos(-StartPosition.Attitude.X * Math.PI / 180) * (StartPosition.Speed * 10)); //meters per second.

                    #region Turn on the brakes if Velocity == 0
                    if (FlightData.V_PosX == 0 && FlightData.V_PosZ == 0 && Settings.Flight.SpawnChocks)
                    {
                        //turn the brake on if the aircraft is spawning STOPPED.
                        FlightData.Anim_Brake = 100;
                    }
                    #endregion

                    #region Replace FlightData if resuming flight
                    if (ThisClient.SpawnTargetVehicle > 0)
                    {
                        Vehicle TargetVehicle = Vehicles.NoVehicle;
                        TargetVehicle = VehiclesHistory.Find(ThisClient.SpawnTargetVehicle);
                        if (TargetVehicle == Vehicles.NoVehicle)
                        {
                            ThisClient.SendMessage("Can't resume flight as " + ThisClient.SpawnTargetVehicle + " because the vehiclehistory cannot be found!");
                            Packets.Packet_10_JoinDenied JoinDenied = new Packets.Packet_10_JoinDenied();
                            ThisClient.SendPacket(JoinDenied);
                            ThisClient.JoinRequestPending = false;
                            ThisClient.SpawnTargetVehicle = 0;
                            return true;
                        }
                        Packets.Packet_11_FlightData OldFlightData = FlightData;
                        FlightData.PosX = TargetVehicle.PosX;
                        FlightData.PosY = TargetVehicle.PosY;
                        FlightData.PosZ = TargetVehicle.PosZ;
                        FlightData.HdgX = (short)TargetVehicle.HdgX;
                        FlightData.HdgY = (short)TargetVehicle.HdgY;
                        FlightData.HdgZ = (short)TargetVehicle.HdgZ;
                        FlightData.V_PosX = TargetVehicle.V_PosX;
                        FlightData.V_PosY = TargetVehicle.V_PosY;
                        FlightData.V_PosZ = TargetVehicle.V_PosZ;
                        //FlightData.V_HdgX = TargetVehicle.V_HdgX;
                        //FlightData.V_HdgY = TargetVehicle.V_HdgY;
                        //FlightData.V_HdgZ = TargetVehicle.V_HdgZ;
                        FlightData.Weight_Fuel = TargetVehicle.Weight_Fuel;
                        FlightData.Weight_SmokeOil = (short)TargetVehicle.Weight_SmokeOil;
                        FlightData.Ammo_GUN = (ushort)TargetVehicle.CachedAircraft.INITIGUN;
                        if (!OpenYS.Weapons.Enabled) FlightData.Ammo_GUN = 0;
                        FlightData.Strength = (short)TargetVehicle.CachedAircraft.STRENGTH;
                        FlightData.Anim_Throttle = TargetVehicle.Anim_Throttle;
                        FlightData.Anim_Boards = TargetVehicle.Anim_Boards;
                        FlightData.Anim_BombBay = TargetVehicle.Anim_BombBay;
                        FlightData.Anim_Gear = TargetVehicle.Anim_Gear;
                        FlightData.Anim_Nozzle = TargetVehicle.Anim_Nozzle;
                        FlightData.Anim_VGW = TargetVehicle.Anim_VGW;
                    }
                    #endregion
                    #endregion

                    #region Create Vehicle
                    Vehicle ThisVehicle = new Vehicle();
                    ThisVehicle.MetaAircraft = MetaAircraft;
                    ThisVehicle.CachedAircraft = CachedAircraft;
                    ThisVehicle.Update(EntityJoined);
                    ThisVehicle.Update(FlightData);
                    Packets.Packet_05_EntityJoined VCOJoined = null; //BE VERY CAREFUL WITH THIS!;
                    if (CachedAircraft.OYS_CARRIER != "NULL")
                    {
                        MetaData.Ground VirtualCarrierObject = MetaData._Ground.FindByName(CachedAircraft.OYS_CARRIER);
                        ThisVehicle.VirtualCarrierObject_MetaData = VirtualCarrierObject;

                        if (VirtualCarrierObject != MetaData._Ground.None)
                        {
                            VCOJoined = new Packets.Packet_05_EntityJoined();
                            VCOJoined.IsGround = true;
                            VCOJoined.ID = World.Objects.GetNextID() | 256 * 256;
                            VCOJoined.IFF = JoinRequest.IFF;
                            VCOJoined.PosX = (float)StartPosition.Position.X;
                            VCOJoined.PosY = (float)StartPosition.Position.Y;
                            VCOJoined.PosZ = (float)StartPosition.Position.Z;
                            VCOJoined.RotX = (float)((StartPosition.Attitude.X) / 180 * Math.PI);
                            VCOJoined.RotY = (float)((StartPosition.Attitude.Y) / 180 * Math.PI);
                            VCOJoined.RotZ = (float)((StartPosition.Attitude.Z) / 180 * Math.PI);
                            VCOJoined.Identify = CachedAircraft.OYS_CARRIER;
                            VCOJoined.OwnerName = ThisClient.Username;
                            VCOJoined.IsOwnedByThisPlayer = false;

                            ThisVehicle.VirtualCarrierObject_ID = VCOJoined.ID;
                        }
                    }
                    #endregion

                    #region Prepare Acknolwedgement Reponse.
                    Packets.Packet_06_Acknowledgement AcknowledgeJoin;
                    if (EntityJoined.IsAircraft) AcknowledgeJoin = new Packets.Packet_06_Acknowledgement(0, EntityJoined.ID);
                    else AcknowledgeJoin = new Packets.Packet_06_Acknowledgement(1, 0);
                    #endregion

                    #region Send Owner Join Data
                    //ThisClient.SendPacket(EntityJoined);
                    new ClientIO.SocketOperation
                        (
                            new ClientIO.DataEvent(ClientIO.SendOperation, ThisClient, EntityJoined),
                            new ClientIO.DataEvent(ClientIO.ReceiveOperation, ThisClient, AcknowledgeJoin)
                        ).Run();

                    ThisClient.SendPacket(FlightData);

                    #region Send VCO Join Data
                    if (VCOJoined != null)
                    {
                        ThisClient.SendPacket(VCOJoined);
                    }
                    #endregion
                    #endregion

                    #region Send Owner Join Approved
                    Packets.Packet_09_JoinApproved JoinApproved = new Packets.Packet_09_JoinApproved();
                    Packets.Packet_06_Acknowledgement AcknowledgeJoinApproved = new Packets.Packet_06_Acknowledgement(6, 0);
                    //ThisClient.SendPacketGetPacket(JoinApproved, AcknowledgeJoinApproved);
                    ThisClient.SendPacket(JoinApproved);
                    ThisClient.JoinRequestPending = false;
                    ThisClient.LastVehicleID = EntityJoined.ID;
                    if (ThisClient.SpawnTargetVehicle > 0)
                    {
                        ThisClient.SendMessage("Successfully re-joined flight " + ThisClient.SpawnTargetVehicle + ". Type /ID to get the new flight id!");
                        ThisClient.SpawnTargetVehicle = 0;
                    }
                    #endregion

                    #region Add Vehicle to Vehicles List
                    ThisClient.Vehicle = ThisVehicle;
                    Vehicles.List.Add(ThisVehicle);
                    VehiclesHistory.List.Add(ThisVehicle);
                    ThisClient.SetFlying();
                    #endregion

                    #region Send Others Join Data
                    foreach (Client OtherClient in Clients.AllClients.Exclude(ThisClient).ToArray())
                    {
                        //OtherClient.SendPacket(ThisVehicle.GetJoinPacket(false));
                        //Console.WriteLine("Sent Join Data To " + OtherClient.Username);
                        if (Settings.Flight.JoinFlightNotification)
                        {
                            OtherClient.SendMessage("&b" + ThisClient.Username + " took off (" + EntityJoined.Identify.ReplaceAll("\0", "") + ")");
                        }
                        //OtherClient.SendPacketGetPacket(ThisVehicle.GetJoinPacket(false), AcknowledgeJoin);
                        if (OtherClient.IsFakeClient()) continue;
                        new ClientIO.SocketOperation
                            (
                                new ClientIO.DataEvent(ClientIO.SendOperation, OtherClient, ThisVehicle.GetJoinPacket(false)),
                                new ClientIO.DataEvent(ClientIO.ReceiveOperation, OtherClient, AcknowledgeJoin)
                            ).Run();
                        //OtherClient.SendPacket(ThisVehicle.GetJoinPacket(false));
                        if (VCOJoined != null)
                        {
                            OtherClient.SendPacket(VCOJoined);
                        }
                        //Console.WriteLine("Recv Join Data From " + OtherClient.Username);
                    }
                    #endregion
                    return true;
                }

                private static bool YSFHandle_11_FlightData(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Debug.BugReport("Investigate Lights.");

                    //Preparation...
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
                            //Debug.WriteLine("Missing Aircraft ID: " + FlightData.ID + " for client " + ThisClient.Username);
                            return false;
                        }
                        SenderVehicle = Vehicles.List.ToArray().Where(x => x.ID == FlightData.ID).ToArray()[0];
                    }
                    #endregion
                    if (SenderVehicle.ID != ThisClient.Vehicle.ID) return false; //do nothing if not the same entity!
                    #region ValidateFlightData
                    if (SenderVehicle.TimeStamp > FlightData.TimeStamp)
                    {
                        //Debug.WriteLine("OLD DATA:" + SenderVehicle.TimeStamp + " > " + FlightData.TimeStamp);
                        return false;
                    }
                    //Debug.WriteLine("NEW DATA:" + SenderVehicle.TimeStamp + " <= " + FlightData.TimeStamp);
                    SenderVehicle.Update(FlightData);
                    #endregion

                    //pre-send operations
                    #region Disable Smoke if required
                    if (!Settings.Flight.EnableSmoke) FlightData.Anim_Smoke = false;
                    #endregion

                    //SEND to all other clients!
                    #region Update Ground Data
                    if (SenderVehicle.VirtualCarrierObject_ID != 0)
                    {
                        Packets.Packet_21_GroundData GroundData = SenderVehicle.GetGroundData();
                        GroundData.TimeStamp = (float)(DateTime.Now - OpenYS.TimeStarted).TotalSeconds;
                        GroundData.ID = SenderVehicle.VirtualCarrierObject_ID;
                        #region Send Ground Data
                        foreach (Client OtherClient in Clients.YSFClients.ToArray())
                        {
                            if (!OtherClient.IsLoggedIn())
                            {
                                Debug.WriteLine("Did not send VCO ground data packet for " + GroundData.ID + " to " + OtherClient.Username + " because they are not logged in.");
                                continue;
                            }
                            OtherClient.SendPacket(GroundData);
                        }
                        #endregion
                    }

                    #endregion
                    #region Formation Data Packet
                    #region ValidateFormation
                    Vehicle TargetVehicle;
                    lock (Vehicles.List)
                    {
                        if (ThisClient.FormationTarget == 0) goto HandleFlightData;
                        if (Vehicles.List.ToArray().Where(x => x.ID == ThisClient.FormationTarget).Count() <= 0) goto HandleFlightData;
                        if (!FlightData.Anim_Light_Land) goto HandleFlightData;
                        TargetVehicle = Vehicles.List.ToArray().Where(x => x.ID == ThisClient.FormationTarget).ToArray()[0];
                        double Distance = Math.Sqrt(Math.Pow(FlightData.PosX - TargetVehicle.PosX, 2) + Math.Pow(FlightData.PosY - TargetVehicle.PosY, 2) + Math.Pow(FlightData.PosZ - TargetVehicle.PosZ, 2));
                        double Velocity = Math.Sqrt(Math.Pow(FlightData.V_PosX, 2) + Math.Pow(FlightData.V_PosY, 2) + Math.Pow(FlightData.V_PosZ, 2));
                    }
                    #endregion
                    #region PrepareFormationPacket
                    Packets.Packet_64_11_FormationData FormationData = new Packets.Packet_64_11_FormationData(5);
                    FormationData.TimeStamp = FlightData.TimeStamp;
                    FormationData.SenderID = FlightData.ID;
                    FormationData.TargetID = ThisClient.FormationTarget;
                    #endregion
                    #region Set Position and Orientation
                    FormationData.PosX = FlightData.PosX - TargetVehicle.PosX;
                    FormationData.PosY = FlightData.PosY - TargetVehicle.PosY;
                    FormationData.PosZ = FlightData.PosZ - TargetVehicle.PosZ;
                    FormationData.PosX = 0;
                    FormationData.PosY = 0;
                    FormationData.PosZ = 0;
                    FormationData.HdgX = FlightData.HdgX;
                    FormationData.HdgY = FlightData.HdgY;
                    FormationData.HdgZ = FlightData.HdgZ;
                    FormationData.V_HdgX = 0;
                    FormationData.V_HdgY = 0;
                    FormationData.V_HdgZ = 0;
                    #endregion
                    #region Animations
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
                    FormationData._Anim_Flags = FlightData._Anim_Flags;
                    if (!Settings.Flight.EnableSmoke) FormationData.Anim_Smoke = false;
                    #endregion

                    //This actually sends the formation flight data out!
                    OYSHandle(ThisClient, FormationData);
                    goto PostSendEvents;
                    #endregion
                    #region Standard Data Packet
                    HandleFlightData:
                    //standardise the timestamp.
                    FlightData.TimeStamp = (float)((DateTime.Now - OpenYS.TimeStarted).TotalSeconds);

                    //SEND to all other clients!
                    #region Send Flight Data
                    foreach (Client OtherClient in Clients.LoggedIn.Exclude(ThisClient).ToArray())
                    {
                        OtherClient.SendPacket(FlightData);
                        //Console.WriteLine("Sent Flight Data To " + OtherClient.Username);
                    }
                    #endregion
                    goto PostSendEvents;
                    #endregion

                    //post-send operations
                    PostSendEvents:
                    #region Aerial Refueling
                    if (Settings.Flight.EnableMidAirRefueling)
                    {
                        foreach (Vehicle OtherVehicle in Vehicles.List)
                        {
                            int RefuelDistance = 100;
                            if (!OtherVehicle.Refuelling) continue; //The aircraft does NOT want to refuel you!
                            if (OtherVehicle.ID == FlightData.ID) continue; //can't refuel yourself!
                            if (OtherVehicle.IFF != ThisClient.Vehicle.IFF) continue; //can't refuel if not on the same team!
                            if (OtherVehicle.Weight_Fuel < 10000)
                            {
                                if (OtherVehicle.Refuelling && OtherVehicle == ThisClient.Vehicle)
                                {
                                    //Tell the refueling host they have run out of excess fuel for refueling.
                                    ThisClient.SendMessage("&aYou no longer have enough fuel to act as a refuelling tanker. You need 10,000KG or more. You now have " + OtherVehicle.Weight_Fuel.ToString() + "KG.");
                                    OtherVehicle.Refuelling = false;
                                    continue;
                                }
                                //This aircraft is not refueling other aircraft, move on.
                                continue;
                            }
                            if (Vehicles.GetDistance(ThisClient.Vehicle, OtherVehicle) > RefuelDistance) continue; //can't refuel if not close enough!
                            float ChangeinKG = 5;
                            if (ThisClient.Vehicle.Weight_Fuel + 5 > SenderVehicle.CachedAircraft.WEIGFUEL)
                            {
                                ChangeinKG = (float)SenderVehicle.CachedAircraft.WEIGFUEL - ThisClient.Vehicle.Weight_Fuel;
                                //If refueling this step would over refuel the receiver, set the fuel accordingly.
                            }

                            Packets.Packet_30_AirCommand AddFuel = new Packets.Packet_30_AirCommand(ThisClient.Vehicle.ID, "INITFUEL", (ThisClient.Vehicle.Weight_Fuel + ChangeinKG).ToString() + "KG");
                            Clients.AllClients.SendPacket(AddFuel);

                            Packets.Packet_30_AirCommand SubtractFuel = new Packets.Packet_30_AirCommand(OtherVehicle.ID, "INITFUEL", (OtherVehicle.Weight_Fuel - ChangeinKG).ToString() + "KG");
                            Clients.AllClients.SendPacket(SubtractFuel);

                            /*
                            Packets.Packet_30_AirCommand ReduceStrength = new Packets.Packet_30_AirCommand(new Packets.GenericPacket());
                            ReduceStrength.ID = ThisClient.Vehicle.ID;
                            ReduceStrength.Command = "*66";
                            ReduceStrength.Argument = "8";
                            Clients.AllClients.SendPacket(ReduceStrength);
                            */
                            //^^ This works, just need to re-house this into a repair hanger method.

                            break; //if there are more compatible vehicles, the refuel rate can go up dramatically, that is NOT what we want!
                        }
                    }
                    #endregion
                    #region Racing Calculations.
                    Games.Racing2.Update(ThisClient);
                    #endregion
                    #region Bomberman
                    if (Settings.Novelties.LaunchBombsFromGuns)
                    {
                        if (FlightData.Anim_Guns)
                        {
                            Packets.Packet_20_OrdinanceLaunch BombsAway = new Packets.Packet_20_OrdinanceLaunch();
                            BombsAway.OrdinanceType = (ushort)Packets.Packet_20_OrdinanceLaunch.OrdinanceTypes.B500;
                            BombsAway.PosX = FlightData.PosX;
                            BombsAway.PosY = FlightData.PosY;
                            BombsAway.PosZ = FlightData.PosZ;
                            BombsAway.HdgX = (float)((FlightData.HdgX / 32767.0d) * Math.PI);
                            BombsAway.HdgY = (float)((FlightData.HdgY / 32767.0d) * Math.PI);
                            BombsAway.HdgZ = (float)((FlightData.HdgZ / 32767.0d) * Math.PI);
                            BombsAway.InitVelocity = 1000;
                            BombsAway.BurnoutDistance = 30000;
                            BombsAway.MaximumDamage = 35;
                            BombsAway.SenderType = 0;
                            BombsAway.SenderID = FlightData.ID;
                            BombsAway.MaximumVelocity = 1000;
                            Clients.AllClients.SendPacket(BombsAway);
                        }
                    }
                    #endregion
                    return true;
                }

                private static bool YSFHandle_12_Unjoin(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_12_Unjoin Unjoin = new Packets.Packet_12_Unjoin(InPacket);
                    Packets.Packet_13_RemoveAirplane RemoveAirplane = new Packets.Packet_13_RemoveAirplane(Unjoin.ID);

                    if (ThisClient.Vehicle != Vehicles.NoVehicle | ThisClient.Vehicle != null)
                    {
                        if (ThisClient.Vehicle.ID == Unjoin.ID)
                        {
                            if (ThisClient.Vehicle.VirtualCarrierObject_ID != 0)
                            {
                                Packets.Packet_19_RemoveGround RemoveVCO = new Packets.Packet_19_RemoveGround(ThisClient.Vehicle.VirtualCarrierObject_ID);
                                Clients.AllClients.SendPacket(RemoveVCO);
                            }
                            if (Settings.Flight.LeaveFlightNotification)
                            {
                                Clients.AllClients.Exclude(ThisClient).SendMessage("&3" + ThisClient.Username + " left the airplane.");
                            }
                            Vehicles.List.RemoveAll(x => x.ID == Unjoin.ID);
                            ThisClient.Vehicle = Vehicles.NoVehicle;
                            ThisClient.SetIdle();
                        }
                    }
                    foreach (Client OtherClient in Clients.AllClients.Exclude(ThisClient))
                    {
                        Packets.Packet_06_Acknowledgement AcknowledgeLeave = new Packets.Packet_06_Acknowledgement(2, RemoveAirplane.ID);
                        //OtherClient.SendPacketGetPacket(RemoveAirplane, AcknowledgeLeave);
                        OtherClient.SendPacket(RemoveAirplane);
                    }
                    Vehicles.List.RemoveAll(x => x.ID == Unjoin.ID); //again just in case...
                    return true;
                }

                private static bool YSFHandle_13_RemoveAircraft(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_13_RemoveAirplane RemoveAirplane = new Packets.Packet_13_RemoveAirplane(InPacket);
                    ThisClient.SendPacket(new Packets.Packet_06_Acknowledgement(2,RemoveAirplane.ID));
                    return true;
                }

                private static bool YSFHandle_32_ChatMessage(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    if (ThisClient.MessagesTyped.Count > 0)
                    {
                        if ((DateTime.Now - ThisClient.MessagesTyped.Last().Created).TotalSeconds < 1)
                        {
                            ThisClient.SendMessage("You may only send one chat message a second, slow down!");
                            return false;
                        }
                    }
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

                private static bool YSFHandle_33_Weather(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    if (InPacket.Data.Length == 0)
                    {
                        #region Send Weather(33)
                        //Build Weather (33)
                        Packets.Packet_33_Weather ServerWeatherResponse = new Packets.Packet_33_Weather(OpenYS.Weather);
                        Packets.Packet_06_Acknowledgement ServerWeatherAcknowledgement = new Packets.Packet_06_Acknowledgement(4, 0);
                        if (!ThisClient.IsBot())
                        {
                            //ThisClient.SendPacketGetPacket(ServerWeatherResponse, ServerWeatherAcknowledgement);
                            ThisClient.SendPacket(ServerWeatherResponse);
                        }
                        else ThisClient.SendPacket(ServerWeatherResponse);
                        #endregion
                        return true;
                    }
                    return true;
                }

                private static bool YSFHandle_36_WeaponsLoading(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_36_WeaponsConfig Loading = new Packets.Packet_36_WeaponsConfig(InPacket);
                    CachedData.Aircraft CachedAircraft = CachedData._Aircraft.None;
                    try
                    {
                        CachedAircraft = Vehicles.List.Where(x => x.ID == Loading.ID).ToArray()[0].CachedAircraft;
                    }
                    catch
                    {
                        Debug.WriteLine("Failed to find the vehicle to get the cached-aircraft info...???");
                    }
                    Loading._RemoveAllWeapon(Packets.Packet_36_WeaponsConfig.WeaponTypes.FLR);
                    Loading._AddWeapon(Packets.Packet_36_WeaponsConfig.WeaponTypes.FLR, (ushort)CachedAircraft.MAXNMFLR);
                    ThisClient.Vehicle.WeaponsLoading = Loading;
                    Clients.LoggedIn.SendPacket(Loading);
                    return true;
                }

                private static bool YSFHandle_37_ListUser(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    if (!Settings.Options.AllowListUsers)
                    {
                        return false;
                    }
                    foreach (Client OtherClient in Clients.YSFClients)
                    {
                        short ClientType = 0;
                        short IFF = 0;
                        uint ID = 0;
                        string Identify = "";
                        
                        if (OtherClient.Vehicle != Vehicles.NoVehicle)
                        {
                            ClientType = 0;
                            if (OtherClient.IsOP()) ClientType = 2;
                            if (OtherClient.Vehicle != null) ClientType += 1;
                            IFF = (short)OtherClient.Vehicle.IFF;
                            ID = OtherClient.Vehicle.ID;
                        }
                        Identify = OtherClient.Username;
                        Packets.Packet_37_ListUser ListUser = new Packets.Packet_37_ListUser(ClientType, IFF, ID, Identify);
                        ThisClient.SendPacket(ListUser);
                    }
                    return true;
                }

                private static bool YSFHandle_64_UserPacket(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    Packets.Packet_64_UserPacket UserPacket = new Packets.Packet_64_UserPacket(InPacket);
                    OYSHandle(ThisClient, UserPacket.ToYSFPacket());
                    return true;
                }
            }
        }
    }
}