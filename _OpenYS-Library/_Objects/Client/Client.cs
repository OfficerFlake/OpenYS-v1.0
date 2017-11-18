using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace OpenYS
{
    public static class Clients
    {
        #region Clients Lists
        private static List<Client> List = new List<Client>();
        private static ReaderWriterLockSlim _ClientsListLock = new ReaderWriterLockSlim();
        public static List<Client> AllClients
        {
            get
            {
                _ClientsListLock.EnterReadLock();
                try
                {
                    return (from ThisUser in List where !ThisUser.IsBot() select ThisUser).ToList();
                }
                finally
                {
                    _ClientsListLock.ExitReadLock();
                }
            }
        }
        public static List<Client> YSFClients
        {
            get
            {
                return (from ThisUser in AllClients where ThisUser.IsYSFlightClient() select ThisUser).ToList();
            }
        }
        public static List<Client> LoggedIn
        {
            get
            {
                return (from ThisUser in AllClients where ThisUser.IsLoggedIn() select ThisUser).ToList();
            }
        }
        public static List<Client> Flying
        {
            get
            {
                return (from ThisUser in AllClients where ThisUser.IsFlying() select ThisUser).ToList();
            }
        }
        #endregion
        #region Clients List Management
        public static void AddClient(Client ThisClient)
        {
            _ClientsListLock.EnterWriteLock();
            try
            {
                List.Add(ThisClient);
            }
            finally
            {
                _ClientsListLock.ExitWriteLock();
            }
        }
        public static void RemoveClient(Client ThisClient)
        {
            _ClientsListLock.EnterWriteLock();
            try
            {
                List.RemoveAll(x => x == ThisClient);
            }
            finally
            {
                _ClientsListLock.ExitWriteLock();
            }
        }
        public static void DoHeartbeatMonitor()
        {
            Client[] Temp = Clients.YSFClients.ToArray();
            foreach (Client ThisClient in Temp)
            {
                if (ThisClient.IsFakeClient()) continue;
                if ((DateTime.Now - ThisClient.LastHeartBeat).TotalSeconds >= 60 & (DateTime.Now - ThisClient.LastHeartBeatWarning).TotalSeconds >= 60)
                {
                    ThisClient.LastHeartBeatWarning = DateTime.Now + new TimeSpan(0, 0, 1);
                    Log.Warning("DoHeartBeat Monitor Warning for " + ThisClient.Username);
                    ThisClient.SendMessage("No Activity for 60 Seconds. Please do something or you will be disconnected soon!");
                    continue;
                }
                if ((DateTime.Now - ThisClient.LastHeartBeat).TotalSeconds >= 120 & (DateTime.Now - ThisClient.LastHeartBeatWarning).TotalSeconds <= 120)
                {
                    ThisClient.SendMessage("You have been disconnected due to inactivity!");
                    ThisClient.Disconnect("Disconnected due to inactivity (DoHeartbeatMonitor)");
                }
            }
        }

        #endregion
        #region Post-List-Modifications
        public static List<Client> Include(this List<Client> Clients, Client ThisClient)
        {
            Clients.RemoveAll(x => x == ThisClient);
            Clients.Add(ThisClient);
            return Clients;
        }
        public static List<Client> Exclude(this List<Client> Clients, Client ThisClient)
        {
            List<Client> Output = Clients.ToArray().ToList();
            Output.RemoveAll(x => x == ThisClient);
            return Output;
        }
        #endregion
        #region Send Data
        public static bool SendPacket(this List<Client> Clients, Packets.GenericPacket InPacket)
        {
            _ClientsListLock.EnterReadLock();
            try
            {
                foreach (Client ThisClient in Clients)
                {
                    ThisClient.SendPacket(InPacket);
                }
            }
            finally
            {
                _ClientsListLock.ExitReadLock();
            }
            return true;
        }
        public static bool SendMessage(this List<Client> Clients, string Message, params object[] args)
        {
            if (args == null) args = new object[0];
            if (args.Length > 0)
            {
                try
                {
                    Message = String.Format(Message, args);
                }
                catch (Exception e)
                {
                    Message = e.StackTrace + "\n" + Message;
                }
            }
            _ClientsListLock.EnterReadLock();
            try
            {
                foreach (Client ThisClient in Clients)
                {
                    ThisClient.SendMessage(Message);
                }
            }
            finally
            {
                _ClientsListLock.ExitReadLock();
            }
            return true;
        }
        #endregion
        #region IRC Handlers
        public delegate void IRCOutputHandler(string Output);
        public static List<IRCOutputHandler> _IRCOutputs = new List<IRCOutputHandler>();
        #endregion
        public static List<string> BotNames = new List<string>() { "PHP bot", "serverlist_bot" };
        public static Client NoClient = null;

        public static Client FindByUserName(string Input)
        {
            List<Client> Matches = new List<Client>();
            Client[] CachedList = AllClients.ToArray();
            //EXACT Match
            foreach (Client ThisClient in CachedList)
            {
                if (ThisClient.Username == Input) Matches.Add(ThisClient);
            }
            if (Matches.Count > 0)
            {
                if (Matches.Count == 1) return Matches[0];
                else return NoClient;
            }
            Matches.Clear();
            //Case Insensitive Match
            foreach (Client ThisClient in CachedList)
            {
                if (ThisClient.Username.ToUpperInvariant() == Input.ToUpperInvariant()) Matches.Add(ThisClient);
            }
            if (Matches.Count > 0)
            {
                if (Matches.Count == 1) return Matches[0];
                else return NoClient;
            }
            Matches.Clear();
            //Contains Match...
            foreach (Client ThisClient in CachedList)
            {
                if (ThisClient.Username.ToUpperInvariant().Contains(Input.ToUpperInvariant())) Matches.Add(ThisClient);
            }
            if (Matches.Count > 0)
            {
                if (Matches.Count == 1) return Matches[0];
                else return NoClient;
            }
            Matches.Clear();
            //No match.
            return NoClient;
        }
    }

    public partial class Client
    {
        #region Variables
        #region Connection State
        [Flags]
        private enum FlagsStateConnection
        {
            Disconnected =      1 << 0, //Disconnected from the server.
            Disconnecting =     1 << 1, //Disconnecting from the server.
            Connected =         1 << 2, //Connected to the server.
            Connecting =        1 << 3, //Connecting to the server.
            LoggedIn =          1 << 4, //Logged in to the server.
            LoggingIn =         1 << 5, //Logging in to the server.
        }
        private FlagsStateConnection StateConnection = FlagsStateConnection.Disconnected;
        #region Get Connection State
        public bool IsDisconnected()
        {
            return StateConnection.HasFlag(FlagsStateConnection.Disconnected);
        }
        public bool IsDisconnecting()
        {
            return StateConnection.HasFlag(FlagsStateConnection.Disconnecting);
        }
        public bool IsConnected()
        {
            return StateConnection.HasFlag(FlagsStateConnection.Connected);
        }
        public bool IsConnecting()
        {
            return StateConnection.HasFlag(FlagsStateConnection.Connecting);
        }
        public bool IsLoggedIn()
        {
            return StateConnection.HasFlag(FlagsStateConnection.LoggedIn);
        }
        public bool IsLoggingIn()
        {
            return StateConnection.HasFlag(FlagsStateConnection.LoggingIn);
        }
        #endregion
        #region Set Connection State
        public void ResetConnectionState()
        {
            StateConnection = FlagsStateConnection.Disconnected; //Not sure? Must be disconnected.
        }
        public void SetDisconnected()
        {
            StateConnection |= FlagsStateConnection.Disconnected;
            StateConnection &= ~FlagsStateConnection.Disconnecting;
            StateConnection &= ~FlagsStateConnection.Connected;
            StateConnection &= ~FlagsStateConnection.Connecting;
            StateConnection &= ~FlagsStateConnection.LoggedIn;
            StateConnection &= ~FlagsStateConnection.LoggingIn;
        }
        public void SetDisconnecting()
        {
            StateConnection |= FlagsStateConnection.Disconnected;
            StateConnection |= FlagsStateConnection.Disconnecting;
            StateConnection &= ~FlagsStateConnection.Connected;
            StateConnection &= ~FlagsStateConnection.Connecting;
            StateConnection &= ~FlagsStateConnection.LoggedIn;
            StateConnection &= ~FlagsStateConnection.LoggingIn;
        }
        public void SetConnected()
        {
            StateConnection &= ~FlagsStateConnection.Disconnected;
            StateConnection &= ~FlagsStateConnection.Disconnecting;
            StateConnection |= FlagsStateConnection.Connected;
            StateConnection &= ~FlagsStateConnection.Connecting;
            //StateConnection &= ~FlagsStateConnection.LoggedIn;
            //StateConnection &= ~FlagsStateConnection.LoggingIn;
        }
        public void SetConnecting()
        {
            StateConnection &= ~FlagsStateConnection.Disconnected;
            StateConnection &= ~FlagsStateConnection.Disconnecting;
            StateConnection &= ~FlagsStateConnection.Connected;
            StateConnection |= FlagsStateConnection.Connecting;
            //StateConnection &= ~FlagsStateConnection.LoggedIn;
            //StateConnection &= ~FlagsStateConnection.LoggingIn;
        }
        public void SetLoggedIn()
        {
            StateConnection &= ~FlagsStateConnection.Disconnected;
            StateConnection &= ~FlagsStateConnection.Disconnecting;
            //StateConnection &= ~FlagsStateConnection.Connected;
            //StateConnection &= FlagsStateConnection.Connecting;
            StateConnection |= FlagsStateConnection.LoggedIn;
            StateConnection &= ~FlagsStateConnection.LoggingIn;
        }
        public void SetLoggingIn()
        {
            StateConnection &= ~FlagsStateConnection.Disconnected;
            StateConnection &= ~FlagsStateConnection.Disconnecting;
            //StateConnection &= ~FlagsStateConnection.Connected;
            //StateConnection &= FlagsStateConnection.Connecting;
            StateConnection &= ~FlagsStateConnection.LoggedIn;
            StateConnection |= FlagsStateConnection.LoggingIn;
        }
        #endregion
        #endregion
        #region Activity State
        [Flags]
        private enum FlagsStateActivity
        {
            Idle =          1 << 0,
            Flying =        1 << 1,
        }
        private FlagsStateActivity StateActivity = FlagsStateActivity.Idle;
        #region Get Activity State
        public bool IsIdle()
        {
            return StateActivity.HasFlag(FlagsStateActivity.Idle);
        }
        public bool IsFlying()
        {
            return StateActivity.HasFlag(FlagsStateActivity.Flying);
        }
        #endregion
        #region Set Activity State
        public void ResetActivityState()
        {
            StateActivity = FlagsStateActivity.Idle; //If not sure, must be idle.
        }
        public void SetIdle()
        {
            StateActivity |= FlagsStateActivity.Idle;
            StateActivity &= ~FlagsStateActivity.Flying;
        }
        public void SetFlying()
        {
            StateActivity &= ~FlagsStateActivity.Idle;
            StateActivity |= FlagsStateActivity.Flying;
        }
        #endregion
        #endregion
        #region ClientType
        [Flags]
        private enum FlagsClientType
        {
            Unknown =           1 << 0, //The client has been created but we don't know what type it is yet!
            FakeClient =        1 << 1, //This client has no driving object attached - it's a psuedo object that sits and does nothing.
            YSFlightClient =    1 << 2, //This client is a YSFlight Client.
            OpenYSClient =      1 << 3, //This client is an OpenYS Client.
            Bot =               1 << 4, //This client is a Bot Client - not a player.
            Console =           1 << 5, //This client is an OpenYS Console Object.
            Controller =        1 << 6, //This client is an OpenYS Controller Object.
            OP =                1 << 7, //This client is an OP of the server.
        }
        private FlagsClientType ClientType = FlagsClientType.Unknown;
        #region Get Client Type
        public bool IsUnknownClient()
        {
            return ClientType.HasFlag(FlagsClientType.Unknown);
        }
        public bool IsFakeClient()
        {
            return ClientType.HasFlag(FlagsClientType.FakeClient);
        }
        public bool IsYSFlightClient()
        {
            return ClientType.HasFlag(FlagsClientType.YSFlightClient);
        }
        public bool IsOpenYSClient()
        {
            return ClientType.HasFlag(FlagsClientType.OpenYSClient);
        }
        public bool IsBot()
        {
            return ClientType.HasFlag(FlagsClientType.Bot);
        }
        public bool IsConsole()
        {
            return ClientType.HasFlag(FlagsClientType.Console);
        }
        public bool IsController()
        {
            return ClientType.HasFlag(FlagsClientType.Controller);
        }
        public bool IsOP()
        {
            return ClientType.HasFlag(FlagsClientType.OP);
        }
        #endregion
        #region Set Client Type
        public void ResetClientType()
        {
            ClientType = 0;
        }
        public void SetUnknownClient()
        {
            ClientType |=  FlagsClientType.Unknown;
            ClientType &= ~FlagsClientType.FakeClient;
            ClientType &= ~FlagsClientType.YSFlightClient;
            ClientType &= ~FlagsClientType.OpenYSClient;
            ClientType &= ~FlagsClientType.Bot;
            ClientType &= ~FlagsClientType.Console;
            ClientType &= ~FlagsClientType.Controller;
            ClientType &= ~FlagsClientType.OP;
        }
        public void SetFakeClient()
        {
            ClientType &= ~FlagsClientType.Unknown;
            ClientType |=  FlagsClientType.FakeClient;
            ClientType &= ~FlagsClientType.YSFlightClient;
            ClientType &= ~FlagsClientType.OpenYSClient;
            ClientType &= ~FlagsClientType.Bot;
            //ClientType &= ~FlagsClientType.Console;
            //ClientType &= ~FlagsClientType.Controller;
            //ClientType &= ~FlagsClientType.OP;
        }
        public void SetYSFlightClient()
        {
            ClientType &= ~FlagsClientType.Unknown;
            ClientType &= ~FlagsClientType.FakeClient;
            ClientType |=  FlagsClientType.YSFlightClient;
            //ClientType &= ~FlagsClientType.OpenYSClient;
            ClientType &= ~FlagsClientType.Bot;
            ClientType &= ~FlagsClientType.Console;
            ClientType &= ~FlagsClientType.Controller;
            //ClientType &= ~FlagsClientType.OP;
        }
        public void SetOpenYSClient()
        {
            ClientType &= ~FlagsClientType.Unknown;
            ClientType &= ~FlagsClientType.FakeClient;
            //ClientType |=  FlagsClientType.YSFlightClient;
            ClientType |=  FlagsClientType.OpenYSClient;
            ClientType &= ~FlagsClientType.Bot;
            ClientType &= ~FlagsClientType.Console;
            ClientType &= ~FlagsClientType.Controller;
            //ClientType &= ~FlagsClientType.OP;
        }
        public void SetBot()
        {
            ClientType &= ~FlagsClientType.Unknown;
            ClientType &= ~FlagsClientType.FakeClient;
            //ClientType &= ~FlagsClientType.YSFlightClient;
            //ClientType &= ~FlagsClientType.OpenYSClient;
            ClientType |= FlagsClientType.Bot;
            //ClientType &= ~FlagsClientType.Console;
            //ClientType &= ~FlagsClientType.Controller;
            //ClientType &= ~FlagsClientType.OP;
        }
        public void SetConsole()
        {
            ClientType &= ~FlagsClientType.Unknown;
            ClientType |= FlagsClientType.FakeClient;
            ClientType &= ~FlagsClientType.YSFlightClient;
            ClientType &= ~FlagsClientType.OpenYSClient;
            //ClientType |=  FlagsClientType.Bot;
            ClientType |=  FlagsClientType.Console;
            //ClientType &= ~FlagsClientType.Controller;
            //ClientType &= ~FlagsClientType.OP;
        }
        public void SetController()
        {
            ClientType &= ~FlagsClientType.Unknown;
            ClientType |= FlagsClientType.FakeClient;
            ClientType &= ~FlagsClientType.YSFlightClient;
            ClientType &= ~FlagsClientType.OpenYSClient;
            //ClientType |=  FlagsClientType.Bot;
            //ClientType &= ~FlagsClientType.Console;
            ClientType |= FlagsClientType.Controller;
            //ClientType &= ~FlagsClientType.OP;
        }
        public void OP()
        {
            //ClientType &= ~FlagsClientType.Unknown;
            //ClientType &= ~FlagsClientType.FakeClient;
            //ClientType |=  FlagsClientType.YSFlightClient;
            //ClientType |= FlagsClientType.OpenYSClient;
            //ClientType &= ~FlagsClientType.Bot;
            //ClientType &= ~FlagsClientType.Console;
            //ClientType &= ~FlagsClientType.Controller;
            ClientType |= FlagsClientType.OP;
        }
        public void DeOP()
        {
            //ClientType &= ~FlagsClientType.Unknown;
            //ClientType &= ~FlagsClientType.FakeClient;
            //ClientType |=  FlagsClientType.YSFlightClient;
            //ClientType |= FlagsClientType.OpenYSClient;
            //ClientType &= ~FlagsClientType.Bot;
            //ClientType &= ~FlagsClientType.Console;
            //ClientType &= ~FlagsClientType.Controller;
            ClientType &= ~FlagsClientType.OP;
        }
        #endregion
        #endregion
        #region Messages Info
        public class MessageTypedInfo
        {
            public string Message = "";
            public DateTime Created = DateTime.Now;

            public MessageTypedInfo(string Input)
            {
                Message = Input;
            }
        }
        private List<MessageTypedInfo> PrivateMessagesTyped = new List<MessageTypedInfo>(){new MessageTypedInfo("")};
        public List<MessageTypedInfo> MessagesTyped
        {
            get
            {
                return PrivateMessagesTyped;
            }
            set
            {
                PrivateMessagesTyped = value;

                //Limit the list size to 50 items maximum!
                if (PrivateMessagesTyped.Count > 50) PrivateMessagesTyped = PrivateMessagesTyped.Skip(PrivateMessagesTyped.Count - 50).ToList();
            }
        }
        #endregion
        #region UserInfo
        public string Username = "nameless";
        public uint Version = 20110207;
        #endregion
        #region Heartbeat Monitor
        public DateTime LastHeartBeat = DateTime.Now;
        public DateTime LastHeartBeatWarning = DateTime.Now;
        #endregion
        #region Vehicle
        public Vehicle Vehicle = Vehicles.NoVehicle;
        public uint SpawnTargetVehicle = 0;
        public uint LastVehicleID = 0;
        public uint FormationTarget = 0;
        #endregion
        #region Other
        public bool JoinRequestPending = false;
        public DateTime TimeCreated = DateTime.Now;
        public bool IsDeaf = false;
        #endregion
        #region PacketProcessor
        public delegate bool PacketHandler();
        #endregion
        public ClientIO YSFClient = null;
        public ClientIO YSFServer = null;
        #endregion

        #region SendingData (Shortcuts)
        public bool SendPacket(Packets.GenericPacket InPacket)
        {
            //YSFClient.SendPacket(InPacket);
            YSFClient.Send(InPacket);
            return true;
        }
        public bool SendMessage(string Message)
        {
            if (IsDeaf) return false;
            if (ClientType.HasFlag(FlagsClientType.Console))
            {
                Console.WriteLine(Message);
                return true;
            }
            if (ClientType.HasFlag(FlagsClientType.YSFlightClient)) Message = Message.StripFormatting();
            YSFClient.Send(new Packets.Packet_32_ChatMessage(Message));
            return true;
        }
        #endregion

        #region BugReporting
        public bool BugReport(Exception e)
        {
            if (this == Clients.NoClient) return false;
            if (e is ThreadAbortException) return false;
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();
            Console.WriteLine(Strings.Repeat("=", System.Console.WindowWidth));
            Console.WriteLine(ConsoleColor.Yellow, "OpenYS Has encountered an error");
            Console.WriteLine(ConsoleColor.Yellow, "Thus, clients Packet Processing has been ignored.");
            Console.WriteLine();
            Console.WriteLine(Debug.GetStackTrace(e));
            Console.WriteLine(Strings.Repeat("=", System.Console.WindowWidth));
            string[] Email = Emailing.PrepareBugReportEmail(e);
            Emailing.SendEmail(Email[0], Email[1]);
            return true;
        }
        #endregion
        #region CrashReporting
        public bool CrashReport(Exception e)
        {
            if (this == Clients.NoClient) return false;
            if (e is ThreadAbortException) return false;
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();
            Console.WriteLine(Strings.Repeat("=", System.Console.WindowWidth));
            Console.WriteLine(ConsoleColor.Red, "OpenYS Has encountered an error");
            Console.WriteLine(ConsoleColor.Red, "Thus, client &e" + Username + "&c has been disconnected.");
            Console.WriteLine();
            Console.WriteLine(Debug.GetStackTrace(e));
            Console.WriteLine(Strings.Repeat("=", System.Console.WindowWidth));
            //Console.WriteLine("Preparing Email.");
            string[] Email = Emailing.PrepareBugReportEmail(e);
            Emailing.SendEmail(Email[0], Email[1]);
            Disconnect("Crash Detected, Disconnecting due to CrashReport(e)");
            return true;
        }
        #endregion

        #region Connecting/Disconnecting
        private class ServerAddress
        {
            public string Address = "127.0.0.1";
            public int Port = 7915;

            public ServerAddress(string _Address, int _Port)
            {
                Address = _Address;
                Port = _Port;
            }
        }

        public bool Connect(Socket ConnectingClientSocket, bool IsClientMode)
        {
            lock (this)
            {
                if (this == Clients.NoClient) return false;
                YSFClient.Socket = ConnectingClientSocket;
                StateConnection = Client.FlagsStateConnection.Connecting;
                #region Sneak the username in so we can name the receiver thread!
                Packets.Packet_01_Login LoginPacket = new Packets.Packet_01_Login("nameless", 20110207);
                try
                {
                    byte[] Size = new byte[4];
                    YSFClient.Socket.Receive(Size, 4, SocketFlags.Peek);
                    byte[] Full = new byte[BitConverter.ToUInt32(Size, 0) + 4];
                    YSFClient.Socket.Receive(Full, Full.Length, SocketFlags.Peek);
                    if (Full.Length > 28) Username = Full.ToDataString().Substring(28).Split('\0')[0];
                    else Username = Full.Skip(8).Take(16).ToArray().ToDataString().Split('\0')[0];
                    Version = BitConverter.ToUInt32(Full, 24);
                    LoginPacket = new Packets.Packet_01_Login(Username, Version);
                    //Packets.Packet_29_VersionNotify VersionPacket = new Packets.Packet_29_VersionNotify(Version);
                    //YSFClient.SendPacketNow(VersionPacket);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                #endregion
                #region OYS Client Mode - Select Server!
                //if (IsClientMode)
                //{
                //    string ServerListFile = System.Environment.ExpandEnvironmentVariables("%USERPROFILE%/Documents/YSFLIGHT.COM/YSFLIGHT/config/serverhistory.txt");
                //    if (!Files.FileExists(ServerListFile))
                //    {
                //        SendMessage("Could not load ServerHistory.txt from YSFlight settings folder.");
                //        Disconnect("No Host Server");
                //        return false;
                //    }
                //    string[] ServersList = Files.FileReadAllLines(ServerListFile);
                //    List<ServerAddress> Processed = new List<ServerAddress>();
                //    foreach (string ThisLine in ServersList)
                //    {
                //        if (ThisLine.CountOccurances("(") < 1) continue;
                //        string Address = ThisLine.Split('(')[0];
                //        int Port = 0;
                //        bool Failed = !Int32.TryParse(ThisLine.Split('(')[1].Split(')')[0], out Port);
                //        if (!Failed) Processed.Add(new ServerAddress(Address, Port));
                //    }
                //    //Now show the server list!
                //    SendMessage("Welcome to OpenYS-Tray! Please select your desired server:");
                //    SendMessage("==========================================================");
                //    int Offset = 0;
                //    IPAddress TargetAddress = IPAddress.Parse("127.0.0.1");
                //    int TargetPort = 7915;
                //    while (true)
                //    {
                //    ListServers:
                //        for (int i = 0; (i + Offset) < Processed.Count & i < 9; i++)
                //        {
                //            SendMessage("[" + i.ToString() + "] " + Processed[i + Offset].Address.Resize(64).Split('\0')[0] + ":" + Processed[i + Offset].Port.ToString());
                //        }
                //        SendMessage("[M] More Servers");
                //        SendMessage("[X] Quit");

                //        int Number = -1;
                //        while (true)
                //        {
                //            Packets.GenericPacket InPacket = YSFClient.ReceivePacket();
                //            if (InPacket.Type == 32)
                //            {
                //                Packets.Packet_32_ChatMessage ChatMessage = new Packets.Packet_32_ChatMessage(InPacket, Username);
                //                bool Failed = !Int32.TryParse(ChatMessage.Message.Substring(0, 1), out Number);
                //                if (Failed)
                //                {
                //                    char Upper = ChatMessage.Message.ToUpperInvariant()[0];
                //                    if (Upper == 'M')
                //                    {
                //                        Offset += 10;
                //                        if (Offset >= Processed.Count) Offset = 0;
                //                        SendMessage("Please select your desired server:");
                //                        SendMessage("==================================");
                //                        goto ListServers;
                //                    }
                //                    if (Upper == 'X')
                //                    {
                //                        Disconnect("Quit");
                //                        return false;
                //                    }
                //                }
                //                else
                //                {
                //                    TargetAddress = Dns.GetHostAddresses(Processed[Offset + Number].Address)[0];
                //                    TargetPort = Processed[Offset + Number].Port;
                //                    break;
                //                }
                //            }
                //        }

                //        SendMessage("You Chose: " + Processed[Offset + Number].Address + ":" + Processed[Offset + Number].Port);
                //        SendMessage("    ");
                //        SendMessage("Logging in now...");
                //        SendMessage("=================");

                //        try
                //        {
                //            YSFServer.Socket.Connect(new IPEndPoint(TargetAddress, TargetPort));
                //        }
                //        catch (Exception e)
                //        {
                //            Log.Error(e);
                //            Console.WriteLine(ConsoleColor.Yellow, "Connecting client \"" + Username + "\" was unable to reach the host YSF Server - is it down?");
                //            SendMessage("Unable to reach the host YSF Server - is it down?");
                //            Disconnect("No Host Socket to connect to. (Failed to redirect client to host)");
                //            return false;
                //        }
                //        //Set the Server Packet Handler Here.
                //        //YSFServer.Connect(YSFServer.Socket);
                //        //YSFServer.SendPacket(LoginPacket);
                //        break;
                //    }
                //}

                #endregion
                #region OYS Server Mode - Start Receiver
                YSFClient.ConnectionContext = ClientIO.ConnectionContexts.Client;
                Debug.TestPoint(); //Testing async receiver...
                //YSFClient.IOStart();
                YSFClient.Socket = ConnectingClientSocket;
                YSFClient.StartIO();

                #endregion
                #region OYS Client Mode - Start Receiver
                if (IsClientMode)
                {
                    //YSFServer.IOStart();
                    //YSFServer.ConnectionContext = Client.Connection.ConnectionContexts.Server;
                }
                #endregion
                return true;
            }
        }
        public bool ConnectTray(Socket ConnectingClientSocket, bool IsClientMode)
        {
            lock (this)
            {
                if (this == Clients.NoClient) return false;
                YSFClient.Socket = ConnectingClientSocket;
                #region Sneak the username in so we can name the receiver thread!
                Packets.Packet_01_Login LoginPacket = new Packets.Packet_01_Login("nameless", 20110207);
                try
                {
                    byte[] Size = new byte[4];
                    YSFClient.Socket.Receive(Size, 4, SocketFlags.Peek);
                    byte[] Full = new byte[BitConverter.ToUInt32(Size, 0) + 4];
                    YSFClient.Socket.Receive(Full, Full.Length, SocketFlags.Peek);
                    if (Full.Length > 28) Username = Full.ToDataString().Substring(28).Split('\0')[0];
                    else Username = Full.Skip(8).Take(16).ToArray().ToDataString().Split('\0')[0];
                    Version = BitConverter.ToUInt32(Full, 24);
                    LoginPacket = new Packets.Packet_01_Login(Username, Version);
                    Packets.Packet_29_VersionNotify VersionPacket = new Packets.Packet_29_VersionNotify(Version);
                    //YSFClient.SendPacketNow(VersionPacket);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                #endregion
                #region OYS Tray Mode - Select Server!
                //string ServerListFile = System.Environment.ExpandEnvironmentVariables("%USERPROFILE%/Documents/YSFLIGHT.COM/YSFLIGHT/config/serverhistory.txt");
                //if (!Files.FileExists(ServerListFile))
                //{
                //    SendMessage("Could not load ServerHistory.txt from YSFlight settings folder.");
                //    Disconnect("No Host Server");
                //    return false;
                //}
                //string[] ServersList = Files.FileReadAllLines(ServerListFile);
                //List<ServerAddress> Processed = new List<ServerAddress>();
                //foreach (string ThisLine in ServersList)
                //{
                //    if (ThisLine.CountOccurances("(") < 1) continue;
                //    string Address = ThisLine.Split('(')[0];
                //    int Port = 0;
                //    bool Failed = !Int32.TryParse(ThisLine.Split('(')[1].Split(')')[0], out Port);
                //    if (!Failed) Processed.Add(new ServerAddress(Address, Port));
                //}
                ////Now show the server list!
                //SendMessage("Welcome to OpenYS-Tray! Please select your desired server:");
                //SendMessage("==========================================================");
                //int Offset = 0;
                //IPAddress TargetAddress = IPAddress.Parse("127.0.0.1");
                //int TargetPort = 7915;
                //while (true)
                //{
                //    ListServers:
                //    for (int i = 0; (i + Offset) < Processed.Count & i < 9; i++)
                //    {
                //        SendMessage("[" + i.ToString() +"] " + Processed[i + Offset].Address.Resize(64).Split('\0')[0] + ":" + Processed[i + Offset].Port.ToString());
                //    }
                //    SendMessage("[M] More Servers");
                //    SendMessage("[X] Quit");

                //    int Number = -1;
                //    while (true)
                //    {
                //        Packets.GenericPacket InPacket = YSFClient.ReceivePacket();
                //        if (InPacket.Type == 32)
                //        {
                //            Packets.Packet_32_ChatMessage ChatMessage = new Packets.Packet_32_ChatMessage(InPacket, Username);
                //            bool Failed = !Int32.TryParse(ChatMessage.Message.Substring(0,1), out Number);
                //            if (Failed)
                //            {
                //                char Upper = ChatMessage.Message.ToUpperInvariant()[0];
                //                if (Upper == 'M')
                //                {
                //                    Offset += 10;
                //                    if (Offset >= Processed.Count) Offset = 0;
                //                    SendMessage("Please select your desired server:");
                //                    SendMessage("==================================");
                //                    goto ListServers;
                //                }
                //                if (Upper == 'X')
                //                {
                //                    Disconnect("Quit");
                //                    return false;
                //                }
                //            }
                //            else
                //            {
                //                TargetAddress = Dns.GetHostAddresses(Processed[Offset + Number].Address)[0];
                //                TargetPort = Processed[Offset + Number].Port;
                //                break;
                //            }
                //        }
                //    }

                //    SendMessage("You Chose: " + Processed[Offset + Number].Address + ":" + Processed[Offset + Number].Port);
                //    SendMessage("    ");
                //    SendMessage("Logging in now...");
                //    SendMessage("=================");

                //    try
                //    {
                //        YSFServer.Socket.Connect(new IPEndPoint(TargetAddress, TargetPort));
                //    }
                //    catch (Exception e)
                //    {
                //        Log.Error(e);
                //        Console.WriteLine(ConsoleColor.Yellow, "Connecting client \"" + Username + "\" was unable to reach the host YSF Server - is it down?");
                //        SendMessage("Unable to reach the host YSF Server - is it down?");
                //        Disconnect("No Host Socket to connect to. (Failed to redirect client to host)");
                //        return false;
                //    }
                //    //Set the Server Packet Handler Here.
                //    YSFServer.Connect(YSFServer.Socket);
                //    //YSFServer.SendPacket(LoginPacket);
                //    break;
                //}

                #endregion
                #region OYS Server Mode - Start Receiver
                YSFClient.ConnectionContext = ClientIO.ConnectionContexts.Client;
                #endregion
                #region OYS Client Mode - Start Receiver
                if (IsClientMode)
                {
                    YSFServer.ConnectionContext = ClientIO.ConnectionContexts.Server;
                }
                #endregion
                StateConnection = Client.FlagsStateConnection.Connecting;
                return true;
            }
        }
        public bool Disconnect(string Reason)
        {
            lock (this)
            {
                if (IsDisconnecting()) return false;
                if (IsDisconnected()) return false;

                //Console.WriteLine(ConsoleColor.Red, Utilities.GetSourceCodePosition(1));
                if (IsFakeClient()) return true;
                if (IsDisconnecting())
                {
                    Log.Warning("Disconnect() Called for Client: " + Username + ". Disconnecting Flag is Currently ON, A duplicate has occured and will be ignored.");
                    return false;
                }
                if (IsDisconnected())
                {
                    Log.Warning("Disconnect() Called for Client: " + Username + ". Disconnect Flag is Currently ON, A duplicate has occured and will be ignored.");
                    return false;
                }
                try
                {
                if (YSFClient.Socket.Connected ||
                       (!YSFClient.Socket.Poll(1000, SelectMode.SelectRead) && YSFClient.Socket.Available != 0))
                    {
                        //The socket still exists!
                        //DO NOT DISCONNECT!

                        Log.Warning("Client " + Username + " Socket told to disconnect when it should not be disconnected? (Data Still Available?)");

                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                Log.Warning("Disconnect() Called for Client: " + Username + ". Disconnect Flag is Currently OFF, Setting to ON. (This is completely normal...)");
                Log.Warning("Disconnect() Reason: " + Reason);
                SetDisconnecting();
                #region Despawn Vehicle
                if (Vehicle != Vehicles.NoVehicle)
                {
                    //Need to tell all the other clients the vehicle is removed!
                    Packets.Packet_13_RemoveAirplane RemoveAirplane = new Packets.Packet_13_RemoveAirplane(Vehicle.ID);
                    Packets.Packet_06_Acknowledgement AcknowledgeLeave = new Packets.Packet_06_Acknowledgement(2, RemoveAirplane.ID);
                    foreach (Client OtherClient in Clients.AllClients.Exclude(this))
                    {
                        OtherClient.SendPacket(RemoveAirplane);
                        //OtherClient.GetPacket(AcknowledgeLeave);
                    }
                    if (Vehicle.VirtualCarrierObject_ID != 0)
                    {
                        Packets.Packet_19_RemoveGround RemoveVCO = new Packets.Packet_19_RemoveGround(Vehicle.VirtualCarrierObject_ID);
                        Clients.AllClients.Exclude(this).SendPacket(RemoveVCO);
                    }
                    Vehicles.List.RemoveAll(x=> x == Vehicle);
                }
                #endregion
                Clients.RemoveClient(this); //remove self from the client list.
                #region Inform Players
                if ((this.Username == "nameless" & !this.ClientType.HasFlag(Client.FlagsClientType.YSFlightClient)) | this.IsBot())
                {
                    if (this.Username == "nameless")
                    {
                        if (Settings.Loading.BotPingMessages) Console.WriteLine(ConsoleColor.Magenta, "Server was pinged by an unknown service.");
                    }
                    else if (this.IsBot())
                    {
                        if (Settings.Loading.BotPingMessages) Console.WriteLine(ConsoleColor.Magenta, "Server was pinged by a serverlist.");
                    }
                }
                else foreach (Client OtherClient in Clients.AllClients.Exclude(this).ToArray())
                    {
                        OtherClient.SendMessage("&c" + Username + " left the server.");
                    }
                #endregion
                #region Kill Client.
                try
                {
                    //lock (YSFClient.PacketWaiters)
                    //{
                    //    foreach (Connection.PacketWaiter ThisPacketWaiter in YSFClient.PacketWaiters.ToArray())
                    //    {
                    //        //force all listener threads to invalidate and cancel.
                    //        ThisPacketWaiter.Signal.Set();
                    //        ThisPacketWaiter.Signal.Dispose();
                    //    }
                    //}
                    
                    //YSFClient.Disconnect();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                #endregion
                #region Kill Server.
                try
                {
                    //YSFServer.ReceiveThread.Abort();
                    //foreach (Connection.PacketWaiter ThisPacketWaiter in YSFServer.PacketWaiters.ToArray())
                    //{
                    //    //force all listener threads to invalidate and cancel.
                    //    ThisPacketWaiter.Signal.Set();
                    //    ThisPacketWaiter.Signal.Dispose();
                    //}
                    
                    //YSFServer.Disconnect();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                #endregion
                //YSFClient.PacketLog.Log();
                StateConnection = Client.FlagsStateConnection.Disconnected;
                return true;
            }
        }
        #endregion

        #region Constructors
        public Client()
        {
            Clients.AddClient(this);
        }
        public Client(ClientIO.DelegatePacketHandler FromClientToServerHandler, ClientIO.DelegatePacketHandler FromServerToClientHandler)
        {
            Clients.AddClient(this);
            YSFClient = new ClientIO(this, FromClientToServerHandler);
            YSFServer = new ClientIO(this, FromServerToClientHandler);
        }
        #endregion

        #region Functions and Extensions
        public override string ToString()
        {
            return Username;
        }
        #endregion
    }
}

