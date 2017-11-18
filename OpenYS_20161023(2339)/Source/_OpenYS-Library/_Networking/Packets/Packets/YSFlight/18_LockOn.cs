using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_18_LockOn : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_18_LockOn(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 18;
                base.Data = DataPacket.Data;
                if (Data.Length < 16) throw new PacketCreationException("Data.Length < 16");
            }

            public Packet_18_LockOn(uint _SenderID, uint _SenderType, uint _TargetID, uint _TargetType)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 18;
                base.Data = new byte[0];
                SenderID = _SenderID;
                SenderType = _SenderType;
                TargetID = _TargetID;
                TargetType = _TargetType;
            }

            public uint SenderID
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

            public uint SenderType
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(4,4), 0);
                }
                set
                {
                    _SetDataBytes(4, BitConverter.GetBytes(value));
                }
            }

            public uint TargetID
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(8, 4), 0);
                }
                set
                {
                    _SetDataBytes(8, BitConverter.GetBytes(value));
                }
            }

            public uint TargetType
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(12, 4), 0);
                }
                set
                {
                    _SetDataBytes(12, BitConverter.GetBytes(value));
                }
            }
        }
    }
}
