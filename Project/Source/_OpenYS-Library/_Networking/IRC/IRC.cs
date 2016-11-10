//Based of Fragma's IRC Class for fCraft Minecraft Client.
//
//Original release licence and notes are below!

#region Original Licence
/* Copyright 2009-2012 Matvei Stefarov <me@matvei.org>
 * 
 * Based, in part, on SmartIrc4net code. Original license is reproduced below.
 * 
 *
 *
 * SmartIrc4net - the IRC library for .NET/C# <http://smartirc4net.sf.net>
 *
 * Copyright (c) 2003-2005 Mirco Bauer <meebey@meebey.net> <http://www.meebey.net>
 *
 * Full LGPL License: <http://www.gnu.org/licenses/lgpl.txt>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace OpenYS
{

    #region IRC
    /// <summary> IRC control class. </summary>
    public static class IRC
    {

        #region IRC Thread
        /// <summary> Class represents an IRC connection/thread.
        /// There is an undocumented option (IRCThreads) to "load balance" the outgoing
        /// messages between multiple bots. If that's the case, several IRCThread objects
        /// are created. The bots grab messages from IRC.outputQueue whenever they are
        /// not on cooldown (a bit of an intentional race condition). </summary>
        sealed class IRCThread : IDisposable
        {
            TcpClient client;
            StreamReader reader;
            StreamWriter writer;
            Thread thread;
            bool isConnected;
            public bool IsReady;
            bool reconnect;
            public bool ResponsibleForInputParsing;
            public string ActualBotNick;
            string desiredBotNick;
            DateTime lastMessageSent;
            readonly ConcurrentQueue<string> localQueue = new ConcurrentQueue<string>();


            public bool Start(string botNick, bool parseInput)
            {
                if (botNick == null) throw new ArgumentNullException("botNick");
                desiredBotNick = botNick;
                ResponsibleForInputParsing = parseInput;
                try
                {
                    // start the machinery!
                    thread = new Thread(IoThread)
                    {
                        Name = "OpenYS.IRC",
                        IsBackground = true,
                        CurrentCulture = new CultureInfo("en-US")
                    };
                    thread.Start();
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Warning(
                                Settings.IRC.IRCMessagesColor + "[IRC] Could not start the bot: "+ ex.ToString());
                    return false;
                }
            }


            void Connect()
            {
                // initialize the client
                IPAddress ipToBindTo = IPAddress.Any;
                IPEndPoint localEndPoint = new IPEndPoint(ipToBindTo, 0);
                client = new TcpClient(localEndPoint)
                {
                    NoDelay = true,
                    ReceiveTimeout = Timeout,
                    SendTimeout = Timeout
                };
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);

                // connect
                try
                {
                    client.Connect(Settings.IRC.HostIP, Settings.IRC.HostPort);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] {0}", e);
                }

                // prepare to read/write
                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());
                isConnected = true;
            }


            void Send(string msg)
            {
                if (msg == null) throw new ArgumentNullException("msg");
                localQueue.Enqueue(msg);
            }


            // runs in its own thread, started from Connect()
            void IoThread()
            {
                string outputLine = "";
                lastMessageSent = DateTime.UtcNow;

                do
                {
                    try
                    {
                        ActualBotNick = desiredBotNick;
                        reconnect = false;
                        Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                    "Connecting to {0}:{1} as {2}",
                                    hostName, port, ActualBotNick);
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                    "Connecting to {0}:{1} as {2}",
                                    hostName, port, ActualBotNick);
                        Connect();

                        // register
                        Send(IRCCommands.User(ActualBotNick, 8, Settings.Options.ServerName));
                        Send(IRCCommands.Nick(ActualBotNick));

                        while (isConnected && !reconnect)
                        {
                            Thread.Sleep(10);

                            if (localQueue.Length > 0 &&
                                DateTime.UtcNow.Subtract(lastMessageSent).TotalMilliseconds >= SendDelay &&
                                localQueue.Dequeue(ref outputLine))
                            {

                                writer.Write(outputLine + "\r\n");
                                lastMessageSent = DateTime.UtcNow;
                                writer.Flush();
                            }

                            if (OutputQueue.Length > 0 &&
                                DateTime.UtcNow.Subtract(lastMessageSent).TotalMilliseconds >= SendDelay &&
                                OutputQueue.Dequeue(ref outputLine))
                            {

                                writer.Write(outputLine + "\r\n");
                                lastMessageSent = DateTime.UtcNow;
                                writer.Flush();
                            }

                            if (client.Client.Available > 0)
                            {
                                string line = reader.ReadLine();
                                if (line == null) break;
                                HandleMessage(line);
                            }
                        }

                    }
                    catch (SocketException)
                    {
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] &cDisconnected. Will retry in {0} seconds.",
                                    ReconnectDelay / 1000);
                        Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] &cDisconnected. Will retry in {0} seconds.",
                                    ReconnectDelay / 1000);
                        reconnect = true;

                    }
                    catch (IOException)
                    {
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] &cDisconnected. Will retry in {0} seconds.",
                                    ReconnectDelay / 1000);
                        Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] &cDisconnected. Will retry in {0} seconds.",
                                    ReconnectDelay / 1000);
                        reconnect = true;
#if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] &c{0}", ex);
                        Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] &c<General Fail - See logs for details!>\n     Will retry in {0} seconds.",
                                    ReconnectDelay / 1000);
                        reconnect = true;
#endif
                    }

                    if (reconnect) Thread.Sleep(ReconnectDelay);
                } while (reconnect);
            }


            void HandleMessage(string message)
            {
                if (message == null) throw new ArgumentNullException("message");

                IRCMessage msg = MessageParser(message, ActualBotNick);
#if DEBUG_IRC
                Logger.Log( LogType.IRC,
                            "[{0}]: {1}",
                            msg.Type, msg.RawMessage );
#endif

                switch (msg.Type)
                {
                    case IRCMessageType.Login:
                        if (Settings.IRC.RegisteredNick)
                        {
                            Send(IRCCommands.Privmsg(Settings.IRC.RegistrationServiceName,
                                                       "IDENTIFY " + Settings.IRC.RegistrationAuthPass));
                        }
                        foreach (string channel in channelNames)
                        {
                            Send(IRCCommands.Join(channel));
                        }
                        IsReady = true;
                        AssignBotForInputParsing(); // bot should be ready to receive input after joining
                        return;


                    case IRCMessageType.Ping:
                        // ping-pong
                        Send(IRCCommands.Pong(msg.RawMessageArray[1].Substring(1)));
                        return;


                    case IRCMessageType.ChannelAction:
                    case IRCMessageType.ChannelMessage:
                        // channel chat
                        if (!ResponsibleForInputParsing) return;
                        if (!IsBotNick(msg.Nick))
                        {
                            string processedMessage = msg.Message;
                            if (msg.Type == IRCMessageType.ChannelAction)
                            {
                                if (processedMessage.StartsWith("\u0001ACTION"))
                                {
                                    processedMessage = processedMessage.Substring(8);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            processedMessage = Colors.ToOYSColorCodes(processedMessage);
                            processedMessage = NonPrintableChars.Replace(processedMessage, "");
                            if (Settings.IRC.IRCToServerStripFormatting)
                            {
                                processedMessage = processedMessage.StripFormatting();
                            }
                            if (processedMessage.Length > 0)
                            {
                                if (Settings.IRC.ForwardIRCToServer)
                                {
                                    if (msg.Type == IRCMessageType.ChannelAction)
                                    {
                                        if (msg.Nick.Contains("OpenYSIRC"))
                                        {
                                            if (processedMessage.Remove(0, 2).StartsWith("* ")) //ColorCode then "* "
                                            {
                                                processedMessage = processedMessage.Remove(0, 4);
                                                Clients.YSFClients.SendMessage("[#" + msg.Channel + "] " + processedMessage);
                                                Console.WriteLine("[#" + msg.Channel + "] " + processedMessage);
                                                Log.Chat(
                                                    "[#" + msg.Channel + "] " + processedMessage);
                                            }
                                            else if (processedMessage.StartsWith("* ")) //NOColorCode then "* "
                                            {
                                                processedMessage = processedMessage.Remove(0, 2);
                                                Clients.YSFClients.SendMessage("[#" + msg.Channel + "] &f"+ processedMessage);
                                                Console.WriteLine("[#" + msg.Channel + "] &f" + processedMessage);
                                                Log.Chat(
                                                    "[#" + msg.Channel + "] &f"+ processedMessage);
                                            }
                                            else
                                            {
                                                Clients.YSFClients.SendMessage("[#" + msg.Channel + "] "+ processedMessage);
                                                Console.WriteLine("[#" + msg.Channel + "] " + processedMessage);
                                                Log.Chat(
                                                    "[#" + msg.Channel + "] "+ processedMessage);
                                            }
                                        }
                                        else
                                        {
                                            Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] "+msg.Nick+" "+processedMessage);
                                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " + msg.Nick + " " + processedMessage);
                                            Log.Chat(
                                                    Settings.IRC.IRCMessagesColor + "[IRC] "+msg.Channel+": * "+msg.Nick+" "+processedMessage);
                                        }
                                    }
                                    else
                                    {
                                        if (msg.Nick == "Xertion")
                                        {
                                            Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC]{0} {1}", Colors.White, processedMessage);
                                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC]{0} {1}", Colors.White, processedMessage);
                                        }
                                        if (msg.Nick == Settings.IRC.Name)
                                        {
                                            int colonpos = processedMessage.IndexOf(":");
                                            if (colonpos == -1)
                                            {
                                                //Ignore all messages except for player joined, kicked etc.
                                                if (!processedMessage.ToLower().StartsWith("player"))
                                                {
                                                    return;
                                                }
                                            }

                                            if (colonpos >= 1)
                                            {
                                                if (processedMessage.Remove(colonpos).Contains(' '))
                                                {
                                                    return;
                                                }
                                            }

                                            processedMessage = processedMessage.Remove(processedMessage.IndexOf(":")) + "&f" + processedMessage.Remove(0, processedMessage.IndexOf(":"));
                                            Clients.YSFClients.SendMessage("[#" + msg.Channel + "]{0} {1}", Colors.White, processedMessage);
                                            Console.WriteLine("[#" + msg.Channel + "]{0} {1}", Colors.White, processedMessage);
                                            Log.Chat("[#" + msg.Channel + "]{0} {1}", Colors.White, processedMessage);
                                        }
                                        else
                                        {
                                            //Ignore Bot Commands.
                                            if (processedMessage.StartsWith(","))
                                            {
                                                return;
                                            }
                                            Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] " + Settings.IRC.IRCMessagesColor + "(" + Settings.IRC.IRCMessagesColor + "{0}" + Settings.IRC.IRCMessagesColor + ")&f{1}", msg.Nick, processedMessage);
                                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " + Settings.IRC.IRCMessagesColor + "(" + Settings.IRC.IRCMessagesColor + "{0}" + Settings.IRC.IRCMessagesColor + ")&f{1}", msg.Nick, processedMessage);
                                            Log.Chat(Settings.IRC.IRCMessagesColor + "[IRC] " + Settings.IRC.IRCMessagesColor + "(" + Settings.IRC.IRCMessagesColor + "{0}" + Settings.IRC.IRCMessagesColor + ")&f{1}", msg.Nick, processedMessage);
                                        }
                                    }
                                }
                                else if (msg.Message.StartsWith("#"))
                                {
                                    Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] {0}{1}: {2}",
                                                    msg.Nick, Colors.White, processedMessage.Substring(1));
                                    Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] {0}{1}: {2}",
                                                    msg.Nick, Colors.White, processedMessage.Substring(1));
                                    Log.Chat(
                                                Settings.IRC.IRCMessagesColor + "[IRC] {0&f}: {1}&f: {2}", msg.Channel, msg.Nick, processedMessage);
                                }
                            }
                        }
                        return;


                    case IRCMessageType.Join:
                        if (!ResponsibleForInputParsing) return;
                        if (Settings.IRC.ShowEvents)
                        {
                            Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] {0} joined {1}",
                                            msg.Nick, msg.Channel);
                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] {0} joined {1}",
                                            msg.Nick, msg.Channel);
                            Log.Chat(
                                        Settings.IRC.IRCMessagesColor + "[IRC] {0} joined {1}", msg.Nick, msg.Channel);
                        }
                        return;


                    case IRCMessageType.Kick:
                        string kicked = msg.RawMessageArray[3];
                        if (kicked == ActualBotNick)
                        {
                            Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                        "&cIRC Bot was kicked from {0} by {1} ({2}), rejoining.",
                                        msg.Channel, msg.Nick, msg.Message);
                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                        "&cIRC Bot was kicked from {0} by {1} ({2}), rejoining.",
                                        msg.Channel, msg.Nick, msg.Message);
                            Thread.Sleep(ReconnectDelay);
                            Send(IRCCommands.Join(msg.Channel));
                        }
                        else
                        {
                            if (!ResponsibleForInputParsing) return;
                            string kickMessage = NonPrintableChars.Replace(msg.Message, "");
                            if (Settings.IRC.IRCToServerStripFormatting)
                            {
                                kickMessage = kickMessage.StripFormatting();
                            }
                            Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] {0} kicked {1} from {2} ({3})",
                                            msg.Nick, kicked, msg.Channel, kickMessage);
                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] {0} kicked {1} from {2} ({3})",
                                            msg.Nick, kicked, msg.Channel, kickMessage);
                            Log.Chat(
                                        Settings.IRC.IRCMessagesColor + "[IRC] {0} kicked {1} from {2} ({3})",
                                        msg.Nick, kicked, msg.Channel, kickMessage);
                        }
                        return;


                    case IRCMessageType.Part:
                    case IRCMessageType.Quit:
                        if (!ResponsibleForInputParsing) return;
                        if (Settings.IRC.ShowEvents)
                        {
                            Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] {0} left {1}",
                                            msg.Nick, msg.Channel);
                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] {0} left {1}",
                                            msg.Nick, msg.Channel);
                            Log.Chat(
                                        Settings.IRC.IRCMessagesColor + "[IRC] {0} left {1}",
                                            msg.Nick, msg.Channel);
                        }
                        return;


                    case IRCMessageType.NickChange:
                        if (!ResponsibleForInputParsing) return;
                        Clients.YSFClients.SendMessage(Settings.IRC.IRCMessagesColor + "[IRC] {0} is now known as {1}",
                                        msg.Nick, msg.Message);
                        Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] {0} is now known as {1}",
                                        msg.Nick, msg.Message);
                        return;


                    case IRCMessageType.ErrorMessage:
                    case IRCMessageType.Error:
                        bool die = false;
                        switch (msg.ReplyCode)
                        {
                            case IRCReplyCode.ErrorNicknameInUse:
                            case IRCReplyCode.ErrorNicknameCollision:
                                Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                            "&cError: Nickname \"{0}\" is already in use. Trying \"{0}_\"",
                                            ActualBotNick);
                                Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                            "&cError: Nickname \"{0}\" is already in use. Trying \"{0}_\"",
                                            ActualBotNick);
                                ActualBotNick += "_";
                                Send(IRCCommands.Nick(ActualBotNick));
                                break;

                            case IRCReplyCode.ErrorBannedFromChannel:
                            case IRCReplyCode.ErrorNoSuchChannel:
                                Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                            "&cError: {0} ({1})",
                                            msg.ReplyCode, msg.Channel);
                                Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                            "&cError: {0} ({1})",
                                            msg.ReplyCode, msg.Channel);
                                die = true;
                                break;

                            case IRCReplyCode.ErrorBadChannelKey:
                                Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                            "&cError: Channel password required for {0}. fCraft does not currently support passworded channels.",
                                            msg.Channel);
                                Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                            "&cError: Channel password required for {0}. fCraft does not currently support passworded channels.",
                                            msg.Channel);
                                die = true;
                                break;

                            default:
                                Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                            "&cError ({0}): {1}",
                                            msg.ReplyCode, msg.RawMessage);
                                Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " +
                                            "&cError ({0}): {1}",
                                            msg.ReplyCode, msg.RawMessage);
                                break;
                        }

                        if (die)
                        {
                            Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " +  "&cError: Disconnecting.");
                            Console.WriteLine(Settings.IRC.IRCMessagesColor + "[IRC] " + "&cError: Disconnecting.");
                            reconnect = false;
                            DisconnectThread();
                        }

                        return;


                    case IRCMessageType.QueryAction:
                        // TODO: PMs
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                    "Query: {0}", msg.RawMessage);
                        break;


                    case IRCMessageType.Kill:
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                    "Bot was killed from {0} by {1} ({2}), reconnecting.",
                                    hostName, msg.Nick, msg.Message);
                        reconnect = true;
                        isConnected = false;
                        return;
                }
            }


            public void DisconnectThread()
            {
                IsReady = false;
                AssignBotForInputParsing();
                isConnected = false;
                if (thread != null && thread.IsAlive)
                {
                    thread.Join(1000);
                    if (thread.IsAlive)
                    {
                        thread.Abort();
                    }
                }
                try
                {
                    if (reader != null) reader.Close();
                }
                catch (ObjectDisposedException) { }
                try
                {
                    if (writer != null) writer.Close();
                }
                catch (ObjectDisposedException) { }
                try
                {
                    if (client != null) client.Close();
                }
                catch (ObjectDisposedException) { }
            }


            #region IDisposable members

            public void Dispose()
            {
                try
                {
                    if (reader != null) reader.Dispose();
                }
                catch (ObjectDisposedException) { }

                try
                {
                    if (reader != null) writer.Dispose();
                }
                catch (ObjectDisposedException) { }

                try
                {
                    if (client != null && client.Connected)
                    {
                        client.Close();
                    }
                }
                catch (ObjectDisposedException) { }
            }

            #endregion
        }
        #endregion

        #region Variables
        static IRCThread[] threads;

        const int Timeout = 10000; // socket timeout (ms)
        internal static int SendDelay = 1000; // set by ApplyConfig
        const int ReconnectDelay = 15000;

        static string hostName;
        static int port;
        static string[] channelNames;
        static string botNick;

        static readonly ConcurrentQueue<string> OutputQueue = new ConcurrentQueue<string>();


        static void AssignBotForInputParsing()
        {
            bool needReassignment = false;
            for (int i = 0; i < threads.Length; i++)
            {
                if (threads[i].ResponsibleForInputParsing && !threads[i].IsReady)
                {
                    threads[i].ResponsibleForInputParsing = false;
                    needReassignment = true;
                }
            }
            if (needReassignment)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    if (threads[i].IsReady)
                    {
                        threads[i].ResponsibleForInputParsing = true;
                        Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " + 
                                    "Bot \"{0}\" is now responsible for parsing input.",
                                    threads[i].ActualBotNick);
                        return;
                    }
                }
                Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " +  "All IRC bots have disconnected.");
            }
        }

        // includes IRC color codes and non-printable ASCII
        static readonly Regex NonPrintableChars = new Regex("\x03\\d{1,2}(,\\d{1,2})?|[\x00-\x1F\x7E-\xFF]", RegexOptions.Compiled);
        #endregion

        #region Methods
        public static void Init()
        {
            if (!Settings.IRC.Enabled) return;

            hostName = Settings.IRC.HostIP.ToString();
            port = Settings.IRC.HostPort;
            channelNames = Settings.IRC.Channels.Split(',');
            for (int i = 0; i < channelNames.Length; i++)
            {
                channelNames[i] = channelNames[i].Trim();
                if (!channelNames[i].StartsWith("#"))
                {
                    channelNames[i] = '#' + channelNames[i].Trim();
                }
            }
            botNick = Settings.IRC.Name;
        }


        public static bool Start()
        {
            if (!Settings.IRC.Enabled) return false;

            int threadCount = Settings.IRC.BotCount;

            if (threadCount == 1)
            {
                IRCThread thread = new IRCThread();
                if (thread.Start(botNick, true))
                {
                    threads = new[] { thread };
                }

            }
            else
            {
                List<IRCThread> threadTemp = new List<IRCThread>();
                for (int i = 0; i < threadCount; i++)
                {
                    IRCThread temp = new IRCThread();
                    if (temp.Start(botNick + (i + 1), (threadTemp.Count == 0)))
                    {
                        threadTemp.Add(temp);
                    }
                }
                threads = threadTemp.ToArray();
            }

            if (threads.Length > 0)
            {
                HookUpHandlers();
                return true;
            }
            else
            {
                Log.Warning(Settings.IRC.IRCMessagesColor + "[IRC] " +  "IRC functionality disabled.");
                return false;
            }
        }


        public static void SendChannelMessage(string line)
        {
            if (line == null) throw new ArgumentNullException("line");
            if (channelNames == null) return; // in case IRC bot is disabled.
            if (!Settings.IRC.ServerToIRCStripFormatting)
            {
                line = Colors.ToIRCColorCodes(line);
            }
            else
            {
                line = NonPrintableChars.Replace(line, "").Trim();
            }
            for (int i = 0; i < channelNames.Length; i++)
            {
                SendRawMessage(IRCCommands.Privmsg(channelNames[i], line));
            }
        }


        public static void SendAction(string line)
        {
            if (line == null) throw new ArgumentNullException("line");
            SendChannelMessage(String.Format("\u0001ACTION {0}\u0001", line));
        }


        public static void SendNotice(string line)
        {
            if (line == null) throw new ArgumentNullException("line");
            if (channelNames == null) return; // in case IRC bot is disabled.
            if (!Settings.IRC.ServerToIRCStripFormatting)
            {
                line = Colors.ToIRCColorCodes(line);
            }
            else
            {
                line = NonPrintableChars.Replace(line, "").Trim();
            }
            for (int i = 0; i < channelNames.Length; i++)
            {
                SendRawMessage(IRCCommands.Notice(channelNames[i], line));
            }
        }


        const int MaxMessageSize = 512;
        public static void SendRawMessage(string line)
        {
            if (line == null) throw new ArgumentNullException("line");
            // handle newlines
            if (line.Contains('\n'))
            {
                string prefix = line.Substring(0, line.IndexOf(':') + 1);
                string[] segments = line.Substring(prefix.Length).Split('\n');
                SendRawMessage(prefix + segments[0]);
                for (int i = 1; i < segments.Length; i++)
                {
                    SendRawMessage(prefix + "> " + segments[i]);
                }
                return;
            }

            // handle line wrapping
            if (line.Length > MaxMessageSize)
            {
                string prefix = line.Substring(0, line.IndexOf(':') + 1);
                SendRawMessage(line.Substring(0, MaxMessageSize));
                int offset = MaxMessageSize;
                while (offset < line.Length)
                {
                    int length = Math.Min(line.Length - offset, MaxMessageSize - prefix.Length);
                    SendRawMessage(prefix + "> " + line.Substring(offset, length - 2));
                    offset += length;
                }
                return;
            }

            // actually send
            OutputQueue.Enqueue(line);
        }


        static bool IsBotNick(string str)
        {
            if (str == null) throw new ArgumentNullException("str");
            return threads.Any(t => t.ActualBotNick == str);
        }


        public static void Disconnect()
        {
            if (threads != null && threads.Length > 0)
            {
                foreach (IRCThread thread in threads)
                {
                    thread.DisconnectThread();
                }
            }
        }
        #endregion

        #region Server Event Handlers

        static void HookUpHandlers()
        {
            Clients._IRCOutputs.Add(ChatMessageOutputEventHandler);
            //Player.Ready += PlayerReadyHandler;
            //Player.HideChanged += OnPlayerHideChanged;
            //Player.Disconnected += PlayerDisconnectedHandler;
            //Player.Kicked += PlayerKickedHandler;
            //PlayerInfo.BanChanged += PlayerInfoBanChangedHandler;
            //PlayerInfo.RankChanged += PlayerInfoRankChangedHandler;
        }

        static void ChatMessageOutputEventHandler(string Message)
        {
            bool enabled = Settings.IRC.ForwardsServerToIRC;
            Message = Message.Replace("&f", "§WHITE§");
            Message = Message.Replace("&0", "&f");
            Message = Message.Replace("§WHITE§", "&0");
            if (enabled)
            {
                if (Message.StartsWith("*")) return; //Message bounce back, do NOT log!
                else SendChannelMessage(Message);
            }
        }

        #region Unused code from fCraft...
        //static void OnPlayerHideChanged(object sender, PlayerHideChangedEventArgs e)
        //{
        //    if (!ConfigKey.IRCBotAnnounceServerJoins.Enabled() || e.Silent)
        //    {
        //        return;
        //    }
        //    if (e.IsNowHidden)
        //    {
        //        if (ConfigKey.IRCBotAnnounceServerJoins.Enabled())
        //        {
        //            ShowPlayerDisconnectedMsg(e.Player, LeaveReason.ClientQuit);
        //        }
        //    }
        //    else
        //    {
        //        PlayerReadyHandler(null, new PlayerEventArgs(e.Player));
        //    }
        //}


        //static void PlayerReadyHandler(object sender, IPlayerEvent e)
        //{
        //    if (ConfigKey.IRCBotAnnounceServerJoins.Enabled() && !e.Player.Info.IsHidden)
        //    {
        //        string message = String.Format("\u0001ACTION {0} {1} connected.\u0001",
        //                                        Colors.IRCBold,
        //                                        e.Player.ClassyName);
        //        SendChannelMessage(message);
        //    }
        //}


        //static void PlayerDisconnectedHandler(object sender, PlayerDisconnectedEventArgs e)
        //{
        //    if (e.Player.HasFullyConnected && ConfigKey.IRCBotAnnounceServerJoins.Enabled() && !e.Player.Info.IsHidden)
        //    {
        //        ShowPlayerDisconnectedMsg(e.Player, e.LeaveReason);
        //    }
        //}


        //static void ShowPlayerDisconnectedMsg(Player player, LeaveReason leaveReason)
        //{
        //    string message = String.Format("{0} {1} left the server ({2})",
        //                     Colors.IRCBold,
        //                     player.ClassyName,
        //                     leaveReason);
        //    SendAction(message);
        //}


        //static void PlayerKickedHandler(object sender, PlayerKickedEventArgs e)
        //{
        //    if (e.Announce && e.Context == LeaveReason.Kick)
        //    {
        //        PlayerSomethingMessage(e.Kicker, "kicked", e.Player.Info, e.Reason);
        //    }
        //}


        //static void PlayerInfoBanChangedHandler(object sender, PlayerInfoBanChangedEventArgs e)
        //{
        //    if (e.Announce)
        //    {
        //        if (e.WasUnbanned)
        //        {
        //            PlayerSomethingMessage(e.Banner, "unbanned", e.PlayerInfo, e.Reason);
        //        }
        //        else
        //        {
        //            PlayerSomethingMessage(e.Banner, "banned", e.PlayerInfo, e.Reason);
        //        }
        //    }
        //}


        //static void PlayerInfoRankChangedHandler(object sender, PlayerInfoRankChangedEventArgs e)
        //{
        //    if (e.Announce)
        //    {
        //        string actionString = String.Format("{0} from {1} to {2}",
        //                                             e.RankChangeType,
        //                                             e.OldRank.ClassyName,
        //                                             e.NewRank.ClassyName);
        //        PlayerSomethingMessage(e.RankChanger, actionString, e.PlayerInfo, e.Reason);
        //    }
        //}


        //static void PlayerSomethingMessage(IClassy player, string action, IClassy target, [CanBeNull] string reason)
        //{
        //    if (player == null) throw new ArgumentNullException("player");
        //    if (action == null) throw new ArgumentNullException("action");
        //    if (target == null) throw new ArgumentNullException("target");
        //    string message = String.Format("{0}* {1} was {2} by {3}",
        //            Colors.IRCBold,
        //            target.ClassyName,
        //            action,
        //            player.ClassyName);
        //    if (!String.IsNullOrEmpty(reason))
        //    {
        //        message += " Reason: " + reason;
        //    }
        //    if (ConfigKey.IRCBotAnnounceServerEvents.Enabled())
        //    {
        //        SendAction(message);
        //    }
        //}
        #endregion

        #endregion

        #region Parsing

        static readonly IRCReplyCode[] ReplyCodes = (IRCReplyCode[])Enum.GetValues(typeof(IRCReplyCode));


        static IRCMessageType GetMessageType(string rawline, string actualBotNick)
        {
            if (rawline == null) throw new ArgumentNullException("rawline");
            if (actualBotNick == null) throw new ArgumentNullException("actualBotNick");

            Match found = ReplyCodeRegex.Match(rawline);
            if (found.Success)
            {
                string code = found.Groups[1].Value;
                IRCReplyCode replycode = (IRCReplyCode)int.Parse(code);

                // check if this replycode is known in the RFC
                if (Array.IndexOf(ReplyCodes, replycode) == -1)
                {
                    return IRCMessageType.Unknown;
                }

                switch (replycode)
                {
                    case IRCReplyCode.Welcome:
                    case IRCReplyCode.YourHost:
                    case IRCReplyCode.Created:
                    case IRCReplyCode.MyInfo:
                    case IRCReplyCode.Bounce:
                        return IRCMessageType.Login;
                    case IRCReplyCode.LuserClient:
                    case IRCReplyCode.LuserOp:
                    case IRCReplyCode.LuserUnknown:
                    case IRCReplyCode.LuserMe:
                    case IRCReplyCode.LuserChannels:
                        return IRCMessageType.Info;
                    case IRCReplyCode.MotdStart:
                    case IRCReplyCode.Motd:
                    case IRCReplyCode.EndOfMotd:
                        return IRCMessageType.Motd;
                    case IRCReplyCode.NamesReply:
                    case IRCReplyCode.EndOfNames:
                        return IRCMessageType.Name;
                    case IRCReplyCode.WhoReply:
                    case IRCReplyCode.EndOfWho:
                        return IRCMessageType.Who;
                    case IRCReplyCode.ListStart:
                    case IRCReplyCode.List:
                    case IRCReplyCode.ListEnd:
                        return IRCMessageType.List;
                    case IRCReplyCode.BanList:
                    case IRCReplyCode.EndOfBanList:
                        return IRCMessageType.BanList;
                    case IRCReplyCode.Topic:
                    case IRCReplyCode.TopicSetBy:
                    case IRCReplyCode.NoTopic:
                        return IRCMessageType.Topic;
                    case IRCReplyCode.WhoIsUser:
                    case IRCReplyCode.WhoIsServer:
                    case IRCReplyCode.WhoIsOperator:
                    case IRCReplyCode.WhoIsIdle:
                    case IRCReplyCode.WhoIsChannels:
                    case IRCReplyCode.EndOfWhoIs:
                        return IRCMessageType.WhoIs;
                    case IRCReplyCode.WhoWasUser:
                    case IRCReplyCode.EndOfWhoWas:
                        return IRCMessageType.WhoWas;
                    case IRCReplyCode.UserModeIs:
                        return IRCMessageType.UserMode;
                    case IRCReplyCode.ChannelModeIs:
                        return IRCMessageType.ChannelMode;
                    default:
                        if (((int)replycode >= 400) &&
                            ((int)replycode <= 599))
                        {
                            return IRCMessageType.ErrorMessage;
                        }
                        else
                        {
                            return IRCMessageType.Unknown;
                        }
                }
            }

            found = PingRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Ping;
            }

            found = ErrorRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Error;
            }

            found = ActionRegex.Match(rawline);
            if (found.Success)
            {
                switch (found.Groups[1].Value)
                {
                    case "#":
                    case "!":
                    case "&":
                    case "+":
                        return IRCMessageType.ChannelAction;
                    default:
                        return IRCMessageType.QueryAction;
                }
            }

            found = CtcpRequestRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.CtcpRequest;
            }

            found = MessageRegex.Match(rawline);
            if (found.Success)
            {
                switch (found.Groups[1].Value)
                {
                    case "#":
                    case "!":
                    case "&":
                    case "+":
                        return IRCMessageType.ChannelMessage;
                    default:
                        return IRCMessageType.QueryMessage;
                }
            }

            found = CtcpReplyRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.CtcpReply;
            }

            found = NoticeRegex.Match(rawline);
            if (found.Success)
            {
                switch (found.Groups[1].Value)
                {
                    case "#":
                    case "!":
                    case "&":
                    case "+":
                        return IRCMessageType.ChannelNotice;
                    default:
                        return IRCMessageType.QueryNotice;
                }
            }

            found = InviteRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Invite;
            }

            found = JoinRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Join;
            }

            found = TopicRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.TopicChange;
            }

            found = NickRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.NickChange;
            }

            found = KickRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Kick;
            }

            found = PartRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Part;
            }

            found = ModeRegex.Match(rawline);
            if (found.Success)
            {
                if (found.Groups[1].Value == actualBotNick)
                {
                    return IRCMessageType.UserModeChange;
                }
                else
                {
                    return IRCMessageType.ChannelModeChange;
                }
            }

            found = QuitRegex.Match(rawline);
            if (found.Success)
            {
                return IRCMessageType.Quit;
            }

            found = KillRegex.Match(rawline);
            return found.Success ? IRCMessageType.Kill : IRCMessageType.Unknown;
        }


        static IRCMessage MessageParser(string rawline, string actualBotNick)
        {
            if (rawline == null) throw new ArgumentNullException("rawline");
            if (actualBotNick == null) throw new ArgumentNullException("actualBotNick");

            string line;
            string nick = null;
            string ident = null;
            string host = null;
            string channel = null;
            string message = null;
            IRCReplyCode replycode;

            if (rawline[0] == ':')
            {
                line = rawline.Substring(1);
            }
            else
            {
                line = rawline;
            }

            string[] linear = line.Split(new[] { ' ' });

            // conform to RFC 2812
            string from = linear[0];
            string messagecode = linear[1];
            int exclamationpos = from.IndexOf('!');
            int atpos = from.IndexOf('@');
            int colonpos = line.IndexOfOrdinal(" :");
            if (colonpos != -1)
            {
                // we want the exact position of ":" not beginning from the space
                colonpos += 1;
            }
            if (exclamationpos != -1)
            {
                nick = from.Substring(0, exclamationpos);
            }
            if ((atpos != -1) &&
                (exclamationpos != -1))
            {
                ident = from.Substring(exclamationpos + 1, (atpos - exclamationpos) - 1);
            }
            if (atpos != -1)
            {
                host = from.Substring(atpos + 1);
            }

            try
            {
                replycode = (IRCReplyCode)int.Parse(messagecode);
            }
            catch (FormatException)
            {
                replycode = IRCReplyCode.Null;
            }
            IRCMessageType type = GetMessageType(rawline, actualBotNick);
            if (colonpos != -1)
            {
                message = line.Substring(colonpos + 1);
            }

            switch (type)
            {
                case IRCMessageType.Join:
                case IRCMessageType.Kick:
                case IRCMessageType.Part:
                case IRCMessageType.TopicChange:
                case IRCMessageType.ChannelModeChange:
                case IRCMessageType.ChannelMessage:
                case IRCMessageType.ChannelAction:
                case IRCMessageType.ChannelNotice:
                    channel = linear[2];
                    break;
                case IRCMessageType.Who:
                case IRCMessageType.Topic:
                case IRCMessageType.Invite:
                case IRCMessageType.BanList:
                case IRCMessageType.ChannelMode:
                    channel = linear[3];
                    break;
                case IRCMessageType.Name:
                    channel = linear[4];
                    break;
            }

            if ((channel != null) &&
                (channel[0] == ':'))
            {
                channel = channel.Substring(1);
            }

            return new IRCMessage(from, nick, ident, host, channel, message, rawline, type, replycode);
        }


        static readonly Regex ReplyCodeRegex = new Regex("^:[^ ]+? ([0-9]{3}) .+$", RegexOptions.Compiled);
        static readonly Regex PingRegex = new Regex("^PING :.*", RegexOptions.Compiled);
        static readonly Regex ErrorRegex = new Regex("^ERROR :.*", RegexOptions.Compiled);
        static readonly Regex ActionRegex = new Regex("^:.*? PRIVMSG (.).* :" + "\x1" + "ACTION .*" + "\x1" + "$", RegexOptions.Compiled);
        static readonly Regex CtcpRequestRegex = new Regex("^:.*? PRIVMSG .* :" + "\x1" + ".*" + "\x1" + "$", RegexOptions.Compiled);
        static readonly Regex MessageRegex = new Regex("^:.*? PRIVMSG (.).* :.*$", RegexOptions.Compiled);
        static readonly Regex CtcpReplyRegex = new Regex("^:.*? NOTICE .* :" + "\x1" + ".*" + "\x1" + "$", RegexOptions.Compiled);
        static readonly Regex NoticeRegex = new Regex("^:.*? NOTICE (.).* :.*$", RegexOptions.Compiled);
        static readonly Regex InviteRegex = new Regex("^:.*? INVITE .* .*$", RegexOptions.Compiled);
        static readonly Regex JoinRegex = new Regex("^:.*? JOIN .*$", RegexOptions.Compiled);
        static readonly Regex TopicRegex = new Regex("^:.*? TOPIC .* :.*$", RegexOptions.Compiled);
        static readonly Regex NickRegex = new Regex("^:.*? NICK .*$", RegexOptions.Compiled);
        static readonly Regex KickRegex = new Regex("^:.*? KICK .* .*$", RegexOptions.Compiled);
        static readonly Regex PartRegex = new Regex("^:.*? PART .*$", RegexOptions.Compiled);
        static readonly Regex ModeRegex = new Regex("^:.*? MODE (.*) .*$", RegexOptions.Compiled);
        static readonly Regex QuitRegex = new Regex("^:.*? QUIT :.*$", RegexOptions.Compiled);
        static readonly Regex KillRegex = new Regex("^:.*? KILL (.*) :.*$", RegexOptions.Compiled);

        #endregion
    }
    #endregion

    #region IRC_Extras
    #pragma warning disable 1591
    /// <summary> IRC protocol reply codes. </summary>
    public enum IRCReplyCode
    {
        Null = 000,
        Welcome = 001,
        YourHost = 002,
        Created = 003,
        MyInfo = 004,
        Bounce = 005,
        TraceLink = 200,
        TraceConnecting = 201,
        TraceHandshake = 202,
        TraceUnknown = 203,
        TraceOperator = 204,
        TraceUser = 205,
        TraceServer = 206,
        TraceService = 207,
        TraceNewType = 208,
        TraceClass = 209,
        TraceReconnect = 210,
        StatsLinkInfo = 211,
        StatsCommands = 212,
        EndOfStats = 219,
        UserModeIs = 221,
        ServiceList = 234,
        ServiceListEnd = 235,
        StatsUptime = 242,
        StatsOLine = 243,
        LuserClient = 251,
        LuserOp = 252,
        LuserUnknown = 253,
        LuserChannels = 254,
        LuserMe = 255,
        AdminMe = 256,
        AdminLocation1 = 257,
        AdminLocation2 = 258,
        AdminEmail = 259,
        TraceLog = 261,
        TraceEnd = 262,
        TryAgain = 263,
        Away = 301,
        UserHost = 302,
        IsOn = 303,
        UnAway = 305,
        NowAway = 306,
        WhoIsUser = 311,
        WhoIsServer = 312,
        WhoIsOperator = 313,
        WhoWasUser = 314,
        EndOfWho = 315,
        WhoIsIdle = 317,
        EndOfWhoIs = 318,
        WhoIsChannels = 319,
        ListStart = 321,
        List = 322,
        ListEnd = 323,
        ChannelModeIs = 324,
        UniqueOpIs = 325,
        NoTopic = 331,
        Topic = 332,
        TopicSetBy = 333,
        Inviting = 341,
        Summoning = 342,
        InviteList = 346,
        EndOfInviteList = 347,
        ExceptionList = 348,
        EndOfExceptionList = 349,
        Version = 351,
        WhoReply = 352,
        NamesReply = 353,
        Links = 364,
        EndOfLinks = 365,
        EndOfNames = 366,
        BanList = 367,
        EndOfBanList = 368,
        EndOfWhoWas = 369,
        Info = 371,
        Motd = 372,
        EndOfInfo = 374,
        MotdStart = 375,
        EndOfMotd = 376,
        YouAreOper = 381,
        Rehashing = 382,
        YouAreService = 383,
        Time = 391,
        UsersStart = 392,
        Users = 393,
        EndOfUsers = 394,
        NoUsers = 395,
        ErrorNoSuchNickname = 401,
        ErrorNoSuchServer = 402,
        ErrorNoSuchChannel = 403,
        ErrorCannotSendToChannel = 404,
        ErrorTooManyChannels = 405,
        ErrorWasNoSuchNickname = 406,
        ErrorTooManyTargets = 407,
        ErrorNoSuchService = 408,
        ErrorNoOrigin = 409,
        ErrorNoRecipient = 411,
        ErrorNoTextToSend = 412,
        ErrorNoTopLevel = 413,
        ErrorWildTopLevel = 414,
        ErrorBadMask = 415,
        ErrorUnknownCommand = 421,
        ErrorNoMotd = 422,
        ErrorNoAdminInfo = 423,
        ErrorFileError = 424,
        ErrorNoNicknameGiven = 431,
        ErrorErroneusNickname = 432,
        ErrorNicknameInUse = 433,
        ErrorNicknameCollision = 436,
        ErrorUnavailableResource = 437,
        ErrorUserNotInChannel = 441,
        ErrorNotOnChannel = 442,
        ErrorUserOnChannel = 443,
        ErrorNoLogin = 444,
        ErrorSummonDisabled = 445,
        ErrorUsersDisabled = 446,
        ErrorNotRegistered = 451,
        ErrorNeedMoreParams = 461,
        ErrorAlreadyRegistered = 462,
        ErrorNoPermissionForHost = 463,
        ErrorPasswordMismatch = 464,
        ErrorYouAreBannedCreep = 465,
        ErrorYouWillBeBanned = 466,
        ErrorKeySet = 467,
        ErrorChannelIsFull = 471,
        ErrorUnknownMode = 472,
        ErrorInviteOnlyChannel = 473,
        ErrorBannedFromChannel = 474,
        ErrorBadChannelKey = 475,
        ErrorBadChannelMask = 476,
        ErrorNoChannelModes = 477,
        ErrorBanListFull = 478,
        ErrorNoPrivileges = 481,
        ErrorChannelOpPrivilegesNeeded = 482,
        ErrorCannotKillServer = 483,
        ErrorRestricted = 484,
        ErrorUniqueOpPrivilegesNeeded = 485,
        ErrorNoOperHost = 491,
        ErrorUserModeUnknownFlag = 501,
        ErrorUsersDoNotMatch = 502
    }


    /// <summary> IRC message types. </summary>
    public enum IRCMessageType
    {
        Ping,
        Info,
        Login,
        Motd,
        List,
        Join,
        Kick,
        Part,
        Invite,
        Quit,
        Kill,
        Who,
        WhoIs,
        WhoWas,
        Name,
        Topic,
        BanList,
        NickChange,
        TopicChange,
        UserMode,
        UserModeChange,
        ChannelMode,
        ChannelModeChange,
        ChannelMessage,
        ChannelAction,
        ChannelNotice,
        QueryMessage,
        QueryAction,
        QueryNotice,
        CtcpReply,
        CtcpRequest,
        Error,
        ErrorMessage,
        Unknown
    }
    #pragma warning restore 1591
    #endregion
}