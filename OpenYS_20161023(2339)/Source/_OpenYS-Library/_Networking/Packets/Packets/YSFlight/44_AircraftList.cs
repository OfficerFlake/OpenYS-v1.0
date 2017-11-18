using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_44_AircraftList : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_44_AircraftList(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 44;
                base.Data = DataPacket.Data;
                if (Data.Length < 4) throw new PacketCreationException("Data.Length < 4");
            }

            public Packet_44_AircraftList(byte _Version, byte _Count, string[] _List)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 44;
                Data = Bits.Repeat("\0", 4);
                Version = _Version;
                Count = _Count;
                List = _List;
            }

            public byte Version
            {
                get
                {
                    return GetDataByte(0);
                }
                set
                {
                    SetDataByte(0, value);
                }
            }
            public byte Count
            {
                get
                {
                    return GetDataByte(1);
                }
                set
                {
                    SetDataByte(1, value);
                }
            }
            public string[] List
            {
                get
                {
                    return _GetDataBytes(0, Data.Length-4).ToDataString().Split('\0');
                }
                set
                {
                    string ACList = "";
                    foreach (string ThisString in value)
                    {
                        ACList += ThisString + "\0";
                    }
                    Data = Bits.ArrayCombine(_GetDataBytes(0,4), ACList.ToByteArray());
                    Count = (byte)(ACList.Split('\0').Length - 1);
                }
            }
        }
    }
}
