using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_29_VersionNotify : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_29_VersionNotify(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 29;
                base.Data = DataPacket.Data;
            }

            public Packet_29_VersionNotify(uint _Version)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 29;
                base.Data = new byte[0];
                Version = _Version;
            }

            public uint Version
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
        }
    }
}
