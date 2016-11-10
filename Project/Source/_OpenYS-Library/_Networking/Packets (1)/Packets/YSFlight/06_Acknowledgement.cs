using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_06_Acknowledgement : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_06_Acknowledgement(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                    base.Type = 6;
                    base.Data = DataPacket.Data;
            }

            public Packet_06_Acknowledgement(params uint[] _Arguments)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 6;
                base.Data = new byte[0];
                Arguments = _Arguments;
            }

            public uint[] Arguments
            {
                get
                {
                    List<uint> ArgumentsOut = new List<uint>();
                    for (int i = 0; i <= Data.Length - 4; i += 4)
                    {
                        ArgumentsOut.Add(BitConverter.ToUInt32(_GetDataBytes(i, 4), 0));
                    }
                    return ArgumentsOut.ToArray();
                }
                set
                {
                    Data = new byte[0];
                    foreach (uint ThisUInt in value)
                    {
                        Data = Bits.ArrayCombine(Data, BitConverter.GetBytes(ThisUInt));
                    }
                }
            }
        }
    }
}
