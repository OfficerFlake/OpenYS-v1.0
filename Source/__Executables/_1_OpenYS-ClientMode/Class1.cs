using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpenYS
{
    class ClientContext
    {
        public TcpClient Client;
        public Stream Stream;
        public byte[] Buffer = new byte[4];
        public MemoryStream Message = new MemoryStream();
    }

    class Program2
    {
        #region Connecting
        static void OnClientAccepted(IAsyncResult ar)
        {
            TcpListener listener = ar.AsyncState as TcpListener;
            if (listener == null)
                return;

            try
            {
                ClientContext context = new ClientContext();
                context.Client = listener.EndAcceptTcpClient(ar);
                context.Stream = context.Client.GetStream();
                context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, OnClientRead, context);
            }
            finally
            {
                listener.BeginAcceptTcpClient(OnClientAccepted, listener);
            }
        }
        #endregion

        #region Recieving
        static void OnMessageReceived(ClientContext context)
        {
            // process the message here

            Packets.GenericPacket ThisPacket = new Packets.GenericPacket(context.Message.ToArray());
            System.Console.WriteLine(ThisPacket.Type);
        }

        static void OnClientRead(IAsyncResult ar)
        {
            ClientContext context = ar.AsyncState as ClientContext;
            if (context == null)
                return;

            try
            {
                int read = context.Stream.EndRead(ar);
                context.Message.Write(context.Buffer, 0, read);

                int length = BitConverter.ToInt32(context.Buffer, 0);
                byte[] buffer = new byte[1024];
                while (length > 0)
                {
                    read = context.Stream.Read(buffer, 0, Math.Min(buffer.Length, length));
                    context.Message.Write(buffer, 0, read);
                    length -= read;
                }

                OnMessageReceived(context);
            }
            catch (System.Exception)
            {
                context.Client.Close();
                context.Stream.Dispose();
                context.Message.Dispose();
                context = null;
            }
            finally
            {
                if (context != null)
                    context.Stream.BeginRead(context.Buffer, 0, context.Buffer.Length, OnClientRead, context);
            }
        }
        #endregion

        #region Sending
        private static void Send(ClientContext context, Packets.GenericPacket OutPacket)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = OutPacket.GetRawBytes();

            // Begin sending the data to the remote device.
            context.Stream.BeginWrite(byteData, 0, byteData.Length, new AsyncCallback(SendCallback), context.Client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                //sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion

        public static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 7915));
            listener.Start();

            listener.BeginAcceptTcpClient(OnClientAccepted, listener);

            Console.Write("Press enter to exit...");
            Console.ReadLine();
            listener.Stop();
        }
    }
}