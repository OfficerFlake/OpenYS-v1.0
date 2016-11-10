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
        /// This is the base packet for all OYS custom packets. It uses YSF packet #64 which YSF ignores, but OYS accepts. 
        /// </summary>
        public class Packet_64_UserPacket : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_64_UserPacket(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 64;
                base.Data = DataPacket.Data;
                if (Data.Length < 4) throw new PacketCreationException("Data.Length < 4");
            }

            public uint SubType
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

            public byte[] SubData
            {
                get
                {
                    return _GetDataBytes(4, Data.Length - 4);
                }
                set
                {
                    _SetDataBytes(4, value);
                }
            }

            public GenericPacket ToYSFPacket()
            {
                GenericPacket OutPacket = new GenericPacket();
                OutPacket.Type = SubType;
                OutPacket.Data = SubData;
                return OutPacket;
            }
        }
    }
}
