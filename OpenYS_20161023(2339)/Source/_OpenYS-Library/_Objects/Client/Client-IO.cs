using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenYS
{
    public partial class ClientIO
    {
        #region Client Object
        public Client Client = null;
        #endregion
        #region Socket
        public Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
        #endregion
        #region PacketHandler
        public delegate bool DelegatePacketHandler(Client CallingClient, Packets.GenericPacket ReceivedPacket);
        public static bool DummyPacketHandler(Client ThisClient, Packets.GenericPacket ReceivedPacket)
        {
            Debug.WriteLine("&cPacket sent from " + Thread.CurrentThread.Name + " went to dummy handler... Nothing happened!");
            return false;
        }
        public DelegatePacketHandler PacketHandler = DummyPacketHandler;
        #endregion
        #region Connection Context
        public static class ConnectionContexts
        {
            public const string Unknown = "Unknown";
            public const string Client = "Client";
            public const string Server = "Server";
            public const string Connectionless = "Connectionless";
        }
        public string ConnectionContext = ConnectionContexts.Unknown;
        #endregion
        #region OYS Framework Support?
        public bool OpenYSSupport = false;
        #endregion
        #region Client Side? (ClientMode Only)
        public bool IsClientSide = false;
        #endregion

        #region Constructor
        public ClientIO(Client _Client, DelegatePacketHandler _Handler)
        {
            Client = _Client;
            PacketHandler = _Handler;
        }
        #endregion

        #region Sending/Receiving (Socket Level Operations)
        private class SocketReadObject
        {
            // Size
            public byte[] sizebuffer = new byte[4];
            public int size = 0;

            // Data
            public byte[] databuffer = null;

            // Operating...
            public int bytesreceivedsofar = 0;
        }

        private class SocketSendObject
        {
            public Packets.GenericPacket outpacket = null;
        }

        private void ReceiveHeader()
        {
            lock (Client)
            {
                try
                {
                    if (ConnectionContext == ConnectionContexts.Connectionless) return;
                    if (Client.IsDisconnecting()) return;
                    if (Client.IsDisconnected()) return;

                    //Debug.WriteLine("Test Async");
                    // Create the state object.
                    SocketReadObject state = new SocketReadObject();

                    // Begin receiving the data from the remote device.
                    //Debug.WriteLine("Start Receiving");
                    Socket.BeginReceive(state.sizebuffer, 0, 4, 0,
                        new AsyncCallback(ReceiveHeaderCallback), state);
                }
                catch (SocketException e)
                {
                    Client.Disconnect("Remote client forcibly closed the connection.");
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Client.Disconnect("Failed to receive packet header.");
                    return;
                }
            }
        }
        private void ReceiveHeaderCallback(IAsyncResult ar)
        {
            lock (Client)
            {
                try
                {
                    if (ConnectionContext == ConnectionContexts.Connectionless) return;
                    if (Client.IsDisconnecting()) return;
                    if (Client.IsDisconnected()) return;

                    // Retrieve the state object and the client socket 
                    // from the asynchronous state object.
                    SocketReadObject state = (SocketReadObject)ar.AsyncState;
                    // Read data from the remote device.
                    int bytesRead = Socket.EndReceive(ar);
                    if (bytesRead == 0)
                    {
                        //End Of Stream
                        //Debug.WriteLine("End of DataStrem");
                        Client.Disconnect("Recv'd 0 data when trying to receive packet header.");
                    }
                    state.bytesreceivedsofar += bytesRead;
                    if (state.bytesreceivedsofar < 4)
                    {
                        // need more data, so store the data received so far.
                        //state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                        //  Get the rest of the data.
                        //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        //new AsyncCallback(ReceiveHeaderCallback), state);
                        Socket.BeginReceive(state.sizebuffer, state.bytesreceivedsofar, 4 - state.bytesreceivedsofar, SocketFlags.None,
                        new AsyncCallback(ReceiveHeaderCallback), state);
                        return;
                    }
                    else
                    {
                        // All bytes have been received.
                        ReceiveData(state);
                    }
                }
                catch (SocketException e)
                {
                    Client.Disconnect("Remote client forcibly closed the connection.");
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Client.Disconnect("Generic Error when trying to receive packet header.");
                    return;
                }
            }
        }

        private void ReceiveData(SocketReadObject state)
        {
            lock (Client)
            {
                try
                {
                    if (ConnectionContext == ConnectionContexts.Connectionless) return;
                    if (Client.IsDisconnecting()) return;
                    if (Client.IsDisconnected()) return;

                    // Get Size...
                    state.size = (int)(BitConverter.ToUInt32(state.sizebuffer, 0));
                    state.bytesreceivedsofar = 0;
                    state.databuffer = new byte[state.size];

                    // Begin receiving the data from the remote device.
                    Socket.BeginReceive(state.databuffer, state.bytesreceivedsofar, state.size - state.bytesreceivedsofar, SocketFlags.None,
                        new AsyncCallback(ReceiveDataCallback), state);
                }
                catch (SocketException e)
                {
                    Client.Disconnect("Remote client forcibly closed the connection.");
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    Client.Disconnect("Failed to receive packet body.");
                    return;
                }
            }
        }
        private void ReceiveDataCallback(IAsyncResult ar)
        {
            Packets.GenericPacket NewPacket = Packets.NoPacket;
            lock (Client)
            {
                try
                {
                    if (ConnectionContext == ConnectionContexts.Connectionless) return;
                    if (Client.IsDisconnecting()) return;
                    if (Client.IsDisconnected()) return;

                    // Retrieve the state object and the client socket 
                    // from the asynchronous state object.
                    SocketReadObject state = (SocketReadObject)ar.AsyncState;
                    // Read data from the remote device.
                    int bytesRead = Socket.EndReceive(ar);
                    if (bytesRead == 0)
                    {
                        //End Of Stream
                        //Debug.WriteLine("End of DataStream");
                        Client.Disconnect("Recv'd 0 data when trying to receive packet body.");
                    }
                    state.bytesreceivedsofar += bytesRead;
                    if (state.bytesreceivedsofar < state.size)
                    {
                        Socket.BeginReceive(state.databuffer, state.bytesreceivedsofar, state.size - state.bytesreceivedsofar, SocketFlags.None,
                            new AsyncCallback(ReceiveDataCallback), state);
                        return;
                    }
                    else
                    {
                        //Debug.WriteLine(state.sizebuffer.ToDebugHexString() + state.databuffer.ToDebugHexString());
                        //Debug.TestPoint();

                        // All bytes have been received.
                        NewPacket = new Packets.GenericPacket(state.sizebuffer.Concat(state.databuffer).ToArray());
                        //Debug.WriteLine("End Receiving");
                    }
                }
                catch (SocketException e)
                {
                    Client.Disconnect("Remote client forcibly closed the connection.");
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Client.Disconnect("Generic Error when trying to receive packet body.");
                    return;
                }
            }

            //Outside of lock now, let another receive process begin!
            foreach (WaitForPacketObject ThisWaitForPacketObject in WaitForPacketList.ToArray())
            {
                if (NewPacket == null) break;
                if (NewPacket == Packets.NoPacket) break;
                if (NewPacket.GetRawString() == ThisWaitForPacketObject.Packet.GetRawString()) //Must match based on strings, not bytes...
                {
                    //Packets match.
                    ThisWaitForPacketObject.Trigger.Set(); //Set the trigger...
                    WaitForPacketList.Remove(ThisWaitForPacketObject); //Remove self.
                    //ThisWaitForPacketObject.Trigger.Dispose(); //Remove the trigger once set.
                    //Debug.WriteLine(Client.Username + " End Wait");
                }
            }
            ProcessPacket(NewPacket);
            //Thread.Sleep(500);
        }

        public Packets.GenericPacket ReceivePacket()
        {
            if (ConnectionContext == ConnectionContexts.Connectionless) return Packets.NoPacket;
            lock (Client) //only can be called one at a time per client!
            {
                try
                {
                RESTART:
                    byte[] Size = new byte[4];
                    byte[] _Buffer = new byte[4];
                    int ReceivedBytes = 0;
                    while (ReceivedBytes < 4)
                    {
                        int ThisReceivedBytes = Socket.Receive(_Buffer, 4 - ReceivedBytes, SocketFlags.None);
                        if (ThisReceivedBytes == 0) Client.Disconnect("SocketDataReceiver Not able to get the Size of the Packet? (Received 0 Data.)"); //Will never return 0 unless shutdown!
                        System.Buffer.BlockCopy(_Buffer, 0, Size, ReceivedBytes, ThisReceivedBytes);
                        ReceivedBytes += ThisReceivedBytes;
                    }

                    int RecvAmmt = (int)BitConverter.ToUInt32(Size, 0);
                    if (RecvAmmt > 8192 | RecvAmmt < 0)
                    {
                        //critical error in data receipt on the socket, flush it!
                        int Available = Socket.Available;
                        byte[] Garbage = new byte[Available];
                        Socket.Receive(Garbage, Available, SocketFlags.None);
                        Log.Warning("Flushed Socket for Client " + Client.Username + " as data size was corrupted!");
                        Debug.WriteLine("Flushed Socket for Client " + Client.Username + " as data size was corrupted!");
                        Log.Packets(new List<string>() { new Packets.GenericPacket(Garbage).GetRawBytes().ToDebugHexString() });
                        //flushed the socket, start again!
                        goto RESTART;
                    }
                    _Buffer = new byte[RecvAmmt];
                    ReceivedBytes = 0;
                    byte[] Data = new byte[RecvAmmt];
                    while (ReceivedBytes < RecvAmmt)
                    {
                        int ThisReceivedBytes = Socket.Receive(_Buffer, RecvAmmt - ReceivedBytes, SocketFlags.None);
                        if (ThisReceivedBytes == 0) Client.Disconnect("SocketDataReceiver Not able to get the Remaining Data of the Packet? (Received 0 Data.)"); //Will never return 0 unless shutdown!
                        System.Buffer.BlockCopy(_Buffer, 0, Data, ReceivedBytes, ThisReceivedBytes);
                        ReceivedBytes += ThisReceivedBytes;
                    }

                    Packets.GenericPacket Output = new Packets.GenericPacket(Size.Concat(Data).ToArray());
                    //OnPacketReceivedEvent(Output);
                    return Output;
                }
                catch (SocketException)
                {
                    Client.Disconnect("Failed to receive data - Socket Closed Exception.");
                    return Packets.NoPacket;
                }
                catch (ObjectDisposedException)
                {
                    Client.Disconnect("Failed to receive data - Socket Disposed Exception.");
                    return Packets.NoPacket;
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Client.Disconnect("SocketDataReceiver Generic Error - See the Error Log for Details!");
                    return Packets.NoPacket;
                }
            }
        }

        public class WaitForPacketObject
        {
            public Packets.GenericPacket Packet = new Packets.GenericPacket();
            public ManualResetEvent Trigger = new ManualResetEvent(false);
            public ClientIO Parent = null; //ERROR POINT! BE CAREFUL HERE!

            public WaitForPacketObject(ClientIO ThisClientIO, Packets.GenericPacket InPacket)
            {
                Packet = InPacket;
                Parent = ThisClientIO; //ERROR POINT RESOLVED.
                ThisClientIO.WaitForPacketList.Add(this);
                Debug.TestPoint();
            }            

            public bool EndWait(int Timeout)
            {
                return Trigger.WaitOne(Timeout);
            }
        }
        private volatile List<WaitForPacketObject> WaitForPacketList = new List<WaitForPacketObject>();

        public bool WaitFor(int Timeout, Packets.GenericPacket Packet)
        {
            return BeginWait(Packet).EndWait(Timeout);
        }

        public class DataEvent
        {
            private bool IsSend;
            public bool Send
            {
                get
                {
                    return IsSend;
                }
                set
                {
                    IsSend = value;
                }
            }
            public bool Receive
            {
                get
                {
                    return !IsSend;
                }
                set
                {
                    IsSend = !value;
                }
            }
            public Packets.GenericPacket Packet;
            public Client Client;

            public DataEvent(bool _IsSend, Client _Client, Packets.GenericPacket _Packet)
            {
                IsSend = _IsSend;
                Client = _Client;
                Packet = _Packet;
            }
        }

        public static bool SendOperation = true;
        public static bool ReceiveOperation = false;

        public class SocketOperation
        {
            public List<DataEvent> Operations = new List<DataEvent>();

            public SocketOperation(params DataEvent[] DataEvents)
            {
                Operations = DataEvents.ToList();
            }
            
            public bool Run()
            {
                try
                {
                    List<WaitForPacketObject> UpcomingReceipts = new List<WaitForPacketObject>();
                    DataEvent LastSendEvent = null;
                    for (int i = 0; i < Operations.Count; i++)
                    {
                        DataEvent ThisDataEvent = Operations[i];
                        int Failures = 0;
                        if (ThisDataEvent.Receive) //receive event.
                        {
                            if (UpcomingReceipts.Count > 0) //and upcoming receipts > 0.
                            {
                                if (ThisDataEvent.Packet.GetRawString() == UpcomingReceipts[0].Packet.GetRawString()) //if expected if first in the receipt list...
                                {
                                    if (LastSendEvent == null) //No last send? How can we verify? What's the point in receiving?
                                    {
                                        continue;
                                    }
                                    //Debug.WriteLine("About to sniff...");
                                    while (!UpcomingReceipts[0].EndWait(500)) //wait for the next packet in the list.
                                    {
                                        Failures++;
                                        LastSendEvent.Client.YSFClient.Send(LastSendEvent.Packet); //Resend last send packet, try and get the repsonse again.
                                        if (Failures >= 5)
                                        {
                                            Debug.WriteLine("Failed to receive expected packet from " + ThisDataEvent.Client.Username + ". Break here to investigate.");
                                            break;
                                        }
                                    }
                                    UpcomingReceipts.RemoveAt(0); //Got it, Pop the first receipt operation off the stack.
                                    continue;
                                }
                                else
                                {
                                    //expected is not first in the list...
                                    Debug.WriteLine("Expected packet is not first in the list... Confused?");
                                }
                                UpcomingReceipts.Clear(); //completed the deferred receipts.
                                continue;
                            }
                            else //no upcoming receipts, but on a receive event.
                            {
                                Failures = 0;
                                while (Failures < 5 & !ThisDataEvent.Client.YSFClient.WaitFor(500, ThisDataEvent.Packet))
                                {
                                    Failures++;
                                }
                                if (Failures >= 5)
                                {
                                    Debug.WriteLine("Failed to receive expected packet from " + ThisDataEvent.Client.Username + ". Break here to investigate.");
                                }
                                continue;
                            }
                        }
                        if (ThisDataEvent.Send)
                        {
                            //Is there another operation ahead?
                            int j = i+1;
                            while (j < Operations.Count)
                            {
                                DataEvent NextDataEvent = Operations[j];
                                //is it a receive operation?
                                if (NextDataEvent.Receive)
                                {
                                    //get ready to receive it.
                                    //Debug.WriteLine("Sniffed ahead a future receipt!");
                                    UpcomingReceipts.Add(NextDataEvent.Client.YSFClient.BeginWait(NextDataEvent.Packet));
                                }
                                j++;
                            }

                            //Debug.WriteLine("Start SocketOp Send");
                            ThisDataEvent.Client.SendPacket(ThisDataEvent.Packet);
                            LastSendEvent = ThisDataEvent;
                            //Debug.WriteLine("End SocketOp Send");
                            continue;
                        }
                    }
                }
                catch
                {
                    Debug.WriteLine("Error occured in async socket operation... Aborted!");
                    return false;
                }
                return true;
            }
        }

        public WaitForPacketObject BeginWait(Packets.GenericPacket InPacket)
        {
            //Debug.WriteLine(Client.Username + " Start Wait");
            return new WaitForPacketObject(this, InPacket);
        }

        public void Send(Packets.GenericPacket Output)
        {
            try
            {
                if (ConnectionContext == ConnectionContexts.Connectionless) return;
                if (Client.IsDisconnecting()) return;
                if (Client.IsDisconnected()) return;
                // Begin sending the data to the remote device.

                SocketSendObject SendOp = new SocketSendObject();
                SendOp.outpacket = Output;
                Socket.BeginSend(Output.GetRawBytes(), 0, (int)Output.Size + 4, SocketFlags.None,
                    new AsyncCallback(SendCallback), SendOp);
            }
            catch (SocketException e)
            {
                Client.Disconnect("Remote client forcibly closed the connection.");
                return;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                if (ConnectionContext == ConnectionContexts.Connectionless) return;
                if (Client.IsDisconnecting()) return;
                if (Client.IsDisconnected()) return;

                // Retrieve the socket from the state object.
                SocketSendObject SendOp = (SocketSendObject)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = Socket.EndSend(ar);
                //Debug.WriteLine("Sent " + bytesSent.ToString() + " bytes.");

                if (bytesSent < SendOp.outpacket.Size + 4)
                {
                    Debug.WriteLine("Didn't send all packet data???");
                }

                // All bytes have been sent.
            }
            catch (SocketException e)
            {
                Client.Disconnect("Remote client forcibly closed the connection.");
                return;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
        #endregion

        #region I/O
        public void StartIO()
        {
            ReceiveHeader();
        }

        public void ProcessPacket(Packets.GenericPacket Output)
        {
            ReceiveHeader(); //Start on the next packet immediately!
            //Debug.WriteLine("Start Processing");
            _ProcessPacket(Output);
            //Debug.WriteLine("End Processing");
        }

        #region I/O (-- PROCESS)
        public int ProcessorRecursionDepth = 0;
        public bool _ProcessPacket(Packets.GenericPacket NewPacket)
        {
            try
            {
                if (NewPacket == null | NewPacket == Packets.NoPacket)
                {
                    Debug.WriteLine("&eGot a \"NULL\" Packet from " + Client.Username + ". Terminating the connection.");
                    return false;
                }
                else
                {
                    ProcessorRecursionDepth++;
                    #region Recursion Depth Test - Abort if too deep!
                    if (ProcessorRecursionDepth > 20)
                    {
                        Console.WriteLine("&c" + Client.Username + " recursion depth > 20. For OYS safety, connection terminated!");
                        Client.Disconnect("Data Recusrion Depth > 20.");
                        return false;
                    }
                    #endregion
                    PacketHandler(Client, NewPacket);
                    ProcessorRecursionDepth--;
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
        }
        #endregion
        #endregion
    }

    public class Client_OLD
    {
        public bool Send(Packets.GenericPacket Output)
        {
            return false;
            //try
            //{
            //    if (Client.ClientType.HasFlag(Client.FlagsClientType.FakeClient))
            //    {
            //        return true;
            //    }
            //    if (!Client.Disconnecting.WaitOne(0)) Socket.Send(Output.GetRawBytes());
            //    return true;
            //}
            //catch (SocketException)
            //{
            //    Connection.Client.Disconnect("Failed to send data - Socket Closed Exception.");
            //    return false;
            //}
            //catch (ObjectDisposedException)
            //{
            //    Connection.Client.Disconnect("Failed to send data - Socket Disposed Exception.");
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    if (Client.IsBot())
            //    {
            //        Client.Disconnect("Bot Disconnected.");
            //        return false; //Don't care if bots get errors...
            //    }
            //    Log.Error(e);
            //    Client.Disconnect("Data Send Failure.");
            //    return false;
            //}
        }

        public Packets.GenericPacket Receive()
        {
            return Packets.NoPacket;
            //lock (Client) //only can be called one at a time per client!
            //{
            //    try
            //    {
            //    RESTART:
            //        byte[] Size = new byte[4];
            //        byte[] _Buffer = new byte[4];
            //        int ReceivedBytes = 0;
            //        while (ReceivedBytes < 4)
            //        {
            //            int ThisReceivedBytes = Socket.Receive(_Buffer, 4 - ReceivedBytes, SocketFlags.None);
            //            if (ThisReceivedBytes == 0) Client.Disconnect("SocketDataReceiver Not able to get the Size of the Packet? (Received 0 Data.)"); //Will never return 0 unless shutdown!
            //            System.Buffer.BlockCopy(_Buffer, 0, Size, ReceivedBytes, ThisReceivedBytes);
            //            ReceivedBytes += ThisReceivedBytes;
            //        }

            //        int RecvAmmt = (int)BitConverter.ToUInt32(Size, 0);
            //        if (RecvAmmt > 8192 | RecvAmmt < 0)
            //        {
            //            //critical error in data receipt on the socket, flush it!
            //            int Available = Socket.Available;
            //            byte[] Garbage = new byte[Available];
            //            Socket.Receive(Garbage, Available, SocketFlags.None);
            //            Log.Warning("Flushed Socket for Client " + Client.Username + " as data size was corrupted!");
            //            Debug.WriteLine("Flushed Socket for Client " + Client.Username + " as data size was corrupted!");
            //            Log.Packets(new List<string>() { new Packets.GenericPacket(Garbage).GetRawBytes().ToDebugHexString() });
            //            //flushed the socket, start again!
            //            goto RESTART;
            //        }
            //        _Buffer = new byte[RecvAmmt];
            //        ReceivedBytes = 0;
            //        byte[] Data = new byte[RecvAmmt];
            //        while (ReceivedBytes < RecvAmmt)
            //        {
            //            int ThisReceivedBytes = Socket.Receive(_Buffer, RecvAmmt - ReceivedBytes, SocketFlags.None);
            //            if (ThisReceivedBytes == 0) Client.Disconnect("SocketDataReceiver Not able to get the Remaining Data of the Packet? (Received 0 Data.)"); //Will never return 0 unless shutdown!
            //            System.Buffer.BlockCopy(_Buffer, 0, Data, ReceivedBytes, ThisReceivedBytes);
            //            ReceivedBytes += ThisReceivedBytes;
            //        }

            //        Packets.GenericPacket Output = new Packets.GenericPacket(Size.Concat(Data).ToArray());
            //        OnPacketReceivedEvent(Output);
            //        return Output;
            //    }
            //    catch (SocketException)
            //    {
            //        Connection.Client.Disconnect("Failed to receive data - Socket Closed Exception.");
            //        return Packets.NoPacket;
            //    }
            //    catch (ObjectDisposedException)
            //    {
            //        Connection.Client.Disconnect("Failed to receive data - Socket Disposed Exception.");
            //        return Packets.NoPacket;
            //    }
            //    catch (Exception e)
            //    {
            //        Log.Error(e);
            //        Client.Disconnect("SocketDataReceiver Generic Error - See the Error Log for Details!");
            //        return Packets.NoPacket;
            //    }
            //}
        }
    }
}
