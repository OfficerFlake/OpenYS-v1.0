using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_35_ReviveGrounds : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_35_ReviveGrounds(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 35;
                base.Data = DataPacket.Data;
            }

            public Packet_35_ReviveGrounds()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 35;               
            }
        }
    }
}
