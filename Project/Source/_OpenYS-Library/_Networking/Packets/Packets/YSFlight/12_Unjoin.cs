using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_12_Unjoin : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_12_Unjoin(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 12;
                base.Data = DataPacket.Data;
                if (Data.Length < 4) throw new PacketCreationException("Data.Length < 4");
            }

            public Packet_12_Unjoin(uint _ID)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 12;
                base.Data = new byte[0];
                ID = _ID;
            }

            public uint ID
            {
                get
                {
                    return BitConverter.ToUInt32(Data, 0);
                }
                set
                {
                    _SetDataBytes(0, BitConverter.GetBytes(value));
                }
            }
        }
    }
}
