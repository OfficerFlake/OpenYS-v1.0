using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        /// <summary>
        /// Generic Packet Baseclass. Has get/set modifiers for easy conversion of data between human interfacing / bitwise data.
        /// 
        /// All specialised packets should inherit this class and use get/set modifiers on the type and data tags only. Size is always automatically re-calculated on modification of the data.
        /// </summary>
        public class GenericPacket
        {
            #region ACTUAL STORAGE OF DATA
            //Raw Data - the only REAL storage value for ANY packet. Use get/set's when modifying packets only!
            internal string _RawPacketStringDEPRECATED
            {
                get
                {
                    return _RawPacketBytes.ToDataString();
                }
                set
                {
                    _RawPacketBytes = value.ToByteArray();
                }
            }
            private byte[] __RawPacketBytes = new byte[] { 4, 0, 0, 0, 0, 0, 0, 0 }; //Default packet is size 4, type 0, data <EMTPY>.
            internal byte[] _RawPacketBytes
            {
                get
                {
                    __RawPacketBytes = Bits.ArrayCombine(BitConverter.GetBytes((uint)__RawPacketBytes.Length-4), __RawPacketBytes.Skip(4).ToArray());
                    return __RawPacketBytes;
                }
                set
                {
                    __RawPacketBytes = value;
                    __RawPacketBytes = Bits.ArrayCombine(BitConverter.GetBytes((uint)value.Length-4), __RawPacketBytes.Skip(4).ToArray());
                }
            }
            #endregion

            #region Get/Set on RAW PACKET
            /// <summary>
            /// Returns the selected bytes from the COMPLETE Packet, starting at Offset, that is Count bytes long.
            /// Similar to String.Substring() method.
            /// 
            /// if Offset is LONGER than the packet size, an array of nulls (/0) of length Count will be returned.
            /// 
            /// If the count is LONGER than the packet size, but the offset is IN the packet size, the data will be returned where is available, and then proceeded by trailing Nulls (/0).
            /// </summary>
            /// <param name="Offset"></param>
            /// <param name="Count"></param>
            /// <returns></returns>
            public byte[] _GetRawBytes(int Offset, int Count)
            {
                if (Count < 0)
                {
                    //give back nothing.
                    return new byte[0];
                }
                if (Offset < 0)
                {
                    //wrap the offset to account for negative values...
                    while (Offset < 0) Offset += _RawPacketBytes.Length;
                }
                if (Offset > _RawPacketBytes.Length)
                {
                    //trying to get data past the end of the string, return "nulls".
                    return Bits.EmptyBits(Count);
                }
                if (_RawPacketBytes.Length >= Offset + Count)
                {
                    //Enough data exists at this point to return the data in full.
                    return _RawPacketBytes.Skip(Offset).Take(Count).ToArray();
                }
                else
                {
                    //not quite enough data exists past this point to return data, get what you can, and then add nulls.
                    return Bits.ArrayCombine(_RawPacketBytes.Skip(Offset).ToArray(), new byte[Count-(_RawPacketBytes.Length - Offset)]);
                }
            }
            public byte GetRawByte(int index)
            {
                return _GetRawBytes(index, 1)[0];
            }

            /// <summary>
            /// Sets the selected bytes of the COMPLETE Packet, starting at Offset, to the Arrays contents.
            /// 
            /// If the result is LONGER than the original packet, the packet is resized.
            /// If the offset is LONGER than the packet, the packet is resized to fit.
            /// </summary>
            /// <param name="Offset"></param>
            /// <param name="Array"></param>
            /// <returns></returns>
            public bool _SetRawBytes(int Offset, byte[] Array)
            {
                if (Offset < 0)
                {
                    //wrap the offset to account for negative values...
                    while (Offset < 0) Offset += _RawPacketBytes.Length;
                }
                if (Offset > _RawPacketBytes.Length)
                {
                    //trying to append AFTER the length of the data! Add extra!
                    _RawPacketBytes = Bits.ArrayCombine(_RawPacketBytes.Take(Offset).ToArray(), Bits.EmptyBits(Offset - _RawPacketBytes.Length), Array);
                    return true;
                }
                if (_RawPacketBytes.Length >= Offset + Array.Length)
                {
                    //long enough to overwrite...
                    //return _RawDataBytes.Skip(Start).Take(Count).ToArray();
                    _RawPacketBytes = Bits.ArrayCombine(_RawPacketBytes.Take(Offset).ToArray(), Array, _RawPacketBytes.Skip(Offset).Skip(Array.Length).ToArray());
                    return true;
                }
                else
                {
                    //not long enough to overwrite, but long enough to start adding part way through...
                    _RawPacketBytes = Bits.ArrayCombine(_RawPacketBytes.Take(Offset).ToArray(), Array);
                    return true;
                }
            }
            public bool SetRawByte(int index, byte input)
            {
                _SetRawBytes(index, new byte[1] { input });
                return true;
            }
            #endregion

            #region Get/Set on DATA of packet.
            /// <summary>
            /// Returns the selected bytes from the Data segment of the Packet, starting at Offset, that is Count bytes long.
            /// Similar to String.Substring() method.
            /// 
            /// if Offset is LONGER than the data size, an array of nulls (/0) of length Count will be returned.
            /// 
            /// If the count is LONGER than the data size, but the offset is IN the data size, the data will be returned where is available, and then proceeded by trailing Nulls (/0).
            /// </summary>
            /// <param name="Offset"></param>
            /// <param name="Count"></param>
            /// <returns></returns>
            public byte[] _GetDataBytes(int Offset, int Count)
            {
                return _GetRawBytes(Offset + 8, Count);
            }

            /// <summary>
            /// Sets the Data segment of the Packet, starting at Offset, to the Arrays contents.
            /// 
            /// If the array is LONGER than the data, the packet is resized.
            /// If the offset is LONGER than the data, the packet is resized to fit.
            /// </summary>
            /// <param name="Offset"></param>
            /// <param name="Array"></param>
            /// <returns></returns>
            public bool _SetDataBytes(int Offset, byte[] Array)
            {
                return _SetRawBytes(Offset + 8, Array);
            }

            /// <summary>
            /// Gets the byte at the given index of the DATA.
            /// 
            /// If the index is LONGER than the packet, a Null (/0) is returned.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public byte GetDataByte(int index)
            {
                return _GetDataBytes(index, 1)[0];
            }

            /// <summary>
            /// Sets the byte at the given index of the DATA segment of the packet.
            /// 
            /// if the index is LONGER than the packet, the packet is resized to fit.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="input"></param>
            /// <returns></returns>
            public bool SetDataByte(int index, byte input)
            {
                _SetDataBytes(index, new byte[1] { input });
                return true;
            }
            #endregion

            //COMMON ELEMENTS OF ALL PACKETS.

            #region Size of Packet (0:4)
            /// <summary>
            /// Size of the packet - Automatically recalculated whenever packet size changes. Taken directly from the packet raw data (0:4).
            /// </summary>
            public uint Size
            {
                get
                {
                    try
                    {
                        return BitConverter.ToUInt32(_GetRawBytes(0,4), 0);
                    }
                    catch
                    {
                        return 4; //default packet size...
                    }
                }
                //no set!
            }
            #endregion
            #region Type of Packet (4:8)
            /// <summary>
            /// Type of the packet - Taken directly from the packet raw data (4:8).
            /// </summary>
            public uint Type
            {
                get
                {
                    try
                    {
                        return BitConverter.ToUInt32(_GetRawBytes(4, 4), 0);
                    }
                    catch
                    {
                        return 0; //default type...
                    }
                }
                set
                {
                    _SetRawBytes(4, BitConverter.GetBytes(value));
                }
            }
            #endregion
            #region Data of Packet (8:>)
            /// <summary>
            /// Data of the packet - Taken directly from the packet raw data (8:)
            /// </summary>
            public byte[] Data
            {
                get
                {
                    try
                    {
                        return _GetRawBytes(8, _RawPacketBytes.Length - 8);
                    }
                    catch
                    {
                        return new byte[0]; //Default empty data...
                    }
                }
                set
                {
                    _SetRawBytes(8, value);
                }
            }
            #endregion

            //Constructors
            #region Build from NOTHING (Empty Packet)
            /// <summary>
            /// Default generic packet. Type 0, Data: "";
            /// </summary>
            public GenericPacket()
            {
                Data = new byte[0];
                Type = 0;
            }
            #endregion
            #region Build from Raw Data String (Converts to Data Array, CPU Intensive)
            /// <summary>
            /// Generic Packet initialised with raw packet data from a socket - that is, size, type and data all as one data string.
            /// </summary>
            /// <param name="_Input"></param>
            public GenericPacket(string _Input)
            {
                _RawPacketBytes = _Input.ToByteArray();
            }
            #endregion
            #region Build from Raw Byte Array (BEST METHOD)
            /// <summary>
            /// Generic Packet initialised with raw packet data from a socket - that is, size, type and data all as one byte array.
            /// </summary>
            /// <param name="_Input"></param>
            public GenericPacket(byte[] _Input)
            {
                _RawPacketBytes = _Input;
            }
            #endregion


            //Functions
            /// <summary>
            /// Returns the full packet as a string - that is, size, type and data.
            /// </summary>
            /// <returns></returns>
            public string GetRawString()
            {
                return _RawPacketBytes.ToDataString();
            }

            /// <summary>
            /// Returns the full packet as a byte array - that is, size, type and data.
            /// </summary>
            /// <returns></returns>
            public byte[] GetRawBytes()
            {
                return _RawPacketBytes;
            }

            public bool SetRawBit(int index, byte bitnumber)
            {
                byte BitwiseRepresentation = 0;
                if (bitnumber  > 7) BitwiseRepresentation = 255;
                if (bitnumber == 7) BitwiseRepresentation = 128;
                if (bitnumber == 6) BitwiseRepresentation = 64;
                if (bitnumber == 5) BitwiseRepresentation = 32;
                if (bitnumber == 4) BitwiseRepresentation = 16;
                if (bitnumber == 3) BitwiseRepresentation = 8;
                if (bitnumber == 2) BitwiseRepresentation = 4;
                if (bitnumber == 1) BitwiseRepresentation = 2;
                if (bitnumber == 0) BitwiseRepresentation = 1;
                if (bitnumber  < 0) BitwiseRepresentation = 0;

                byte CurrentByte = GetDataByte(index);
                byte NewByte = (byte)(CurrentByte | BitwiseRepresentation);

                _SetRawBytes(index, new byte[1] { NewByte });
                return true;
            }
            public bool UnsetRawBit(int index, byte bitnumber)
            {
                byte BitwiseRepresentation = 0;
                if (bitnumber > 7) BitwiseRepresentation = 255;
                if (bitnumber == 7) BitwiseRepresentation = 128;
                if (bitnumber == 6) BitwiseRepresentation = 64;
                if (bitnumber == 5) BitwiseRepresentation = 32;
                if (bitnumber == 4) BitwiseRepresentation = 16;
                if (bitnumber == 3) BitwiseRepresentation = 8;
                if (bitnumber == 2) BitwiseRepresentation = 4;
                if (bitnumber == 1) BitwiseRepresentation = 2;
                if (bitnumber == 0) BitwiseRepresentation = 1;
                if (bitnumber < 0) BitwiseRepresentation = 0;

                byte CurrentByte = GetRawByte(index);
                byte NewByte = (byte)(CurrentByte & ~BitwiseRepresentation);

                SetRawByte(index, NewByte);
                return true;
            }

            public Packet_64_UserPacket ToCustomPacket()
            {
                return new Packet_64_UserPacket(this) { SubType = Type, SubData = Data };
            }
        }

        /* Deprecated
        public static void Analyse(this GenericPacket ThisPacket)
        {
            Console.Locked = true;
            System.Console.SetCursorPosition(0, 0);
            System.Console.BackgroundColor = ConsoleColor.Red;
            string output = "ANALYSING PACKET OF SIZE: " + ThisPacket.Size.ToString() + " AND TYPE: " + ThisPacket.Type + ".";
            System.Console.Write(output + Utilities.StringRepeat(" ", System.Console.WindowWidth - output.Length));
            System.Console.BackgroundColor = ConsoleColor.Black;
            Console.ConsoleHandler(ThisPacket.Data.ToColoredDebugHexString());
            int x = System.Console.CursorLeft;
            int y = System.Console.CursorTop;
            System.Console.WriteLine(Utilities.StringRepeat(" ", 1024));
            System.Console.CursorTop = y;
            System.Console.CursorLeft = x;
            Console.Locked = false;
        }
        */

        public static string Compare(GenericPacket Packet1, GenericPacket Packet2)
        {
            List<string> output = new List<string>();
            if (Packet1.Type != Packet2.Type)
            {
                output.Add("&ePackets are Different Types");
            }

            if (Packet1.Data.Length != Packet2.Data.Length)
            {
                output.Add("&ePackets are Different Lengths");
            }
            else
            {
                output.Add("&aBegin Comparison");
                int matching = 0;
                int error = 0;
                for (int i = 0; i < Packet1.Data.Length; i++)
                {
                    if (Packet1.Data[i] != Packet2.Data[i])
                    {
                        error++;
                        output.Add("&e-Packets differ at byte[" + i + "].");
                        output.Add("&b--Packet1: " + Packet1.Data[i].ToString());
                        output.Add("&d--Packet2: " + Packet2.Data[i].ToString());
                    }
                    else
                    {
                        matching++;
                    }
                }
                output.Add("&c-Total Matches: " + matching);
                output.Add("&c-Total Errors: " + error);
                output.Add("&aEnd Comparison");
            }
            string outputfinal = "";
            foreach (string ThisString in output)
            {
                if (outputfinal.Length > 0) outputfinal += "\n";
                outputfinal += ThisString;
            }
            return outputfinal;
        }

        [Serializable]
        public class PacketCreationException : Exception
        {
            public PacketCreationException() : base("The packet was created with invalid parameters.")
            {
            }

            public PacketCreationException(string message) : base(message)
            {
            }

            public PacketCreationException(string message, Exception inner): base(message, inner)
            {
            }
        }

        public static GenericPacket NoPacket = new GenericPacket();
    }
}