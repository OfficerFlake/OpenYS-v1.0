using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenYS
{
        public static partial class Server
        {
            #region Connection Listener
            public static class Listener
            {
                private static Socket ListenerSocket;
                private static Thread ServerListenerThread;
                private static ManualResetEvent ServerClosed = new ManualResetEvent(true);

                public static bool Start()
                {
                    //ListenerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    OpenYS.OpenYSConsole.YSFClient.OpenYSSupport = Settings.Options.AllowOYSFramework;
                    OpenYS.OpenYSConsole.SetLoggedIn();
                    ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ServerClosed = new ManualResetEvent(false);
                    ServerListenerThread = Threads.Add(() => _Start(), "Server Listener Thread");
                    return true;
                }

                public static bool _Start()
                {
                    #region TRY:
#if RELEASE
                try {
#endif
                    #endregion
                        #region Listener
                        lock (ListenerSocket)
                        {
                            try
                            {
                                ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                ListenerSocket.Bind(OpenYS.YSFListener.EndPoint);
                            }
                            catch (Exception e)
                            {
                                Console.TerminateConsole("Server Port in Use!");
                                Log.Error(e);
                                return false;
                            }

                            ListenerSocket.Listen(16);

                            switch (OpenYS.BuildType)
                            {
                                case OpenYS._BuildType.None:
                                    goto default;

                                case OpenYS._BuildType.Client:
                                    Console.WriteLine(ConsoleColor.DarkYellow, "Clients will be redirected to: &e" + Settings.Server.HostAddress.ToString() + ":" + Settings.Server.HostPort.ToString() + "&6...");
                                    Console.WriteLine();
                                    Console.WriteLine(ConsoleColor.Red, "Listening for Clients on Port: " + Settings.Server.ListenerPort.ToString() + "...");
                                    Console.WriteLine(Strings.Repeat("=", ("Listening for Clients on Port: " + Settings.Server.ListenerPort.ToString() + "...").Length));
                                    break;

                                case OpenYS._BuildType.Server:
                                    Console.WriteLine(ConsoleColor.Cyan, "Listening for Clients on Port: " + Settings.Server.ListenerPort.ToString() + "...");
                                    Console.WriteLine(Strings.Repeat("=", ("Listening for Clients on Port: " + Settings.Server.ListenerPort.ToString() + "...").Length));
                                    break;

                                default:
                                    Console.WriteLine("&eNot in an active Server or Client release... Server not launched.");
                                    Console.WriteLine(Strings.Repeat("=", ("Not in an active Server or Client release... Server not launched.").Length));
                                    break;
                            }
                            Console.WriteLine();

                            ServerClosed.Reset();
                            OpenYS.YSFListener.ServerStarted.Set();

                            if (OpenYS.BuildType == OpenYS._BuildType.None)
                            {
                                ServerClosed.Set();
                                ListenerSocket.Close();
                                ListenerSocket.Dispose();
                            }

                            while (!Environment.TerminateSignal.WaitOne(0))
                            {
                                AcceptNewClient();
                            }
                            ServerClosed.Set();
                            OpenYS.YSFListener.ServerStarted.Reset();
                        }
                        return true;
                        #endregion
                    #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException) return false;
                    Log.Error(e);
                    Console.TerminateConsole(e);
                    //Socket Closed.
                    return false;
                }
#endif
                    #endregion
                }

                public static bool AcceptNewClient()
                {
                    Socket ConnectingClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    #region TRY:
#if RELEASE
                    try
                    {
#endif
                    #endregion
                        ConnectingClientSocket.NoDelay = true;
                        ConnectingClientSocket = ListenerSocket.Accept();

                        if (Settings.Server.ConnectionLocked)
                        {
                            ConnectingClientSocket.Shutdown(SocketShutdown.Both);
                            ConnectingClientSocket.Close();
                            Console.WriteLine(ConsoleColor.Yellow, "Connecting Client was dismissed as the server is &cLOCKED&e.");
                            return false;
                        }
                        if (ConnectingClientSocket == null) return false;

                        //Console.Write(ConsoleColor.Green, "Got a New Client from ");
                        //Console.WriteLine(ConsoleColor.DarkYellow, (ConnectingClientSocket.RemoteEndPoint as IPEndPoint).Address.ToString());

                        //ConnectingObject.YSFClient.Handle = PacketHandler.ClientMode.FromClientToServer.YSFHandle;
                        Client ConnectingObject = null;

                        if (OpenYS.BuildType == OpenYS._BuildType.Client)
                        {
                            ConnectingObject = new Client(PacketHandler.ClientMode.FromClientToServer.YSFHandle, PacketHandler.ClientMode.FromServerToClient.YSFHandle);
                            ConnectingObject.Connect(ConnectingClientSocket, true);
                        }
                        if (OpenYS.BuildType == OpenYS._BuildType.Server )
                        {
                            ConnectingObject = new Client(PacketHandler.ServerMode.FromClientToServer.YSFHandle, null);
                            ConnectingObject.Connect(ConnectingClientSocket, false);
                        }

                        return true;
                    #region CATCH
#if RELEASE
                    }
                    catch (Exception e)
                    {
                        if (e is ThreadAbortException)
                        {
                            return false;
                        }
                        Console.WriteLine("Client Failed To Connect...");
                        Log.Error(e);
                        return false;
                    }
#endif
                        #endregion
                }

                public static bool Stop()
                {
                    foreach (Client ThisClient in Clients.YSFClients)
                    {
                        ThisClient.Disconnect("Server shutting down.");
                    }
                    DateTime Delay = DateTime.Now;
                    while (Clients.YSFClients.Count > 0 & (DateTime.Now - Delay).TotalSeconds < 5)
                    {
                        Thread.Sleep(10);
                    }
                    foreach (Client ThisClient in Clients.YSFClients)
                    {
                        Debug.WriteLine("Stray Client" + ThisClient.Username);
                    }
                    Vehicles.List.Clear();
                    ListenerSocket.Close();
                    ListenerSocket.Dispose();
                    //ServerClosed.WaitOne();
                    ServerListenerThread.Abort();
                    return false;
                }
            }
            #endregion
        }
}