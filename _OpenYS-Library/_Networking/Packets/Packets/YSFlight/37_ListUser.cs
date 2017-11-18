using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_37_ListUser : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_37_ListUser(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 37;
                base.Data = DataPacket.Data;
            }

            public Packet_37_ListUser()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 37;
            }

            public Packet_37_ListUser(short _ClientType, short _IFF, uint _ID, string _Identify)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 37;
                ClientType = _ClientType;
                IFF = _IFF;
                ID = _ID;
                Identify = _Identify;
            }

            public short ClientType
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(0, 2), 0);
                }
                set
                {
                    _SetDataBytes(0, BitConverter.GetBytes(value));
                }
            }

            public short IFF
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(2, 2), 0);
                }
                set
                {
                    _SetDataBytes(2, BitConverter.GetBytes(value));
                }
            }

            public uint ID
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(4, 4), 0);
                }
                set
                {
                    _SetDataBytes(4, BitConverter.GetBytes(value));
                }
            }

            public string Identify
            {
                get
                {
                    return _GetDataBytes(12, Data.Length - 12).ToDataString().Split('\0')[0]; //13 == cull trailing \0.
                }
                set
                {
                    Data = Bits.ArrayCombine(_GetDataBytes(0,12), value.ToByteArray(), new byte[1]{0});
                }
            }
        }
    }
}
