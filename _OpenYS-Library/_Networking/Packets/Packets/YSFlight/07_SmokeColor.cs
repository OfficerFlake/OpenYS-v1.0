using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_07_SmokeColor : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_07_SmokeColor(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                    base.Type = 7;
                    base.Data = DataPacket.Data;
            }

            public Packet_07_SmokeColor(uint _ID, byte _Index, byte _Red, byte _Green, byte _Blue)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 7;
                base.Data = new byte[0];
                ID = _ID;
                Index = _Index;
                Red = _Red;
                Green = _Green;
                Blue = _Blue;
            }

            public uint ID
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(0, 4), 0);
                }
                set
                {
                    _SetDataBytes(0, BitConverter.GetBytes(value));
                }
            }

            public byte Index
            {
                get
                {
                    return GetDataByte(4);
                }
                set
                {
                    SetDataByte(4, value);
                }
            }
            public byte Red
            {
                get
                {
                    return GetDataByte(5);
                }
                set
                {
                    SetDataByte(5, value);
                }
            }
            public byte Green
            {
                get
                {
                    return GetDataByte(6);
                }
                set
                {
                    SetDataByte(6, value);
                }
            }
            public byte Blue
            {
                get
                {
                    return GetDataByte(6);
                }
                set
                {
                    SetDataByte(6, value);
                }
            }
        }
    }
}
