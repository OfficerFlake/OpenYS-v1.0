using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_39_WeaponsOption : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_39_WeaponsOption(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 39;
                base.Data = DataPacket.Data;
            }

            public Packet_39_WeaponsOption(bool _Enabled)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 39;
                Enabled = _Enabled;
            }

            public bool Enabled
            {
                get
                {
                    if (BitConverter.ToUInt32(_GetDataBytes(0, 4), 0) == 0) return false;
                    else return true;
                }
                set
                {
                    if (value == false) _SetDataBytes(0, BitConverter.GetBytes((uint)0));
                    else _SetDataBytes(0, BitConverter.GetBytes((uint)1));
                }
            }
        }
    }
}
