using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_16_PrepareSimulation : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_16_PrepareSimulation(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 16;
                base.Data = DataPacket.Data;
            }

            public Packet_16_PrepareSimulation()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 16;
                base.Data = new byte[0];
            }
        }
    }
}
