using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_10_JoinDenied : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_10_JoinDenied(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 10;
                base.Data = DataPacket.Data;
            }

            public Packet_10_JoinDenied()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 10;
                base.Data = new byte[0];
            }
        }
    }
}
