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
        /// This packet, sent from a server or client will tell the receiver that the original sender supports OYS extended
        /// functionality. in other words, this is how OYS sees the difference between a vanilla ysf client of an OYS client.
        /// </summary>
        public class Packet_64_29_OpenYS_Handshake : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_64_29_OpenYS_Handshake(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 29;
                base.Data = DataPacket.Data;
                if (Data.Length < 4) throw new PacketCreationException("Data.Length < 4");
            }

            public Packet_64_29_OpenYS_Handshake(uint _Version)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 29;
                base.Data = Bits.Repeat("\0", 4);
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
