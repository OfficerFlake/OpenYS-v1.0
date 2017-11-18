using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_47_ForceJoin : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_47_ForceJoin(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                    base.Type = 47;
                    base.Data = DataPacket.Data;
            }

            public Packet_47_ForceJoin()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 47;
                base.Data = new byte[0];
            }
        }
    }
}
