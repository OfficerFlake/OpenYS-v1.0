using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_09_JoinApproved : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_09_JoinApproved(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 9;
                base.Data = DataPacket.Data;
            }

            public Packet_09_JoinApproved()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 9;
                base.Data = new byte[0];
            }
        }
    }
}
