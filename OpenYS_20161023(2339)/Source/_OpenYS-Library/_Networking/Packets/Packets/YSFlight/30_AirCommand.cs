using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_30_AirCommand : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_30_AirCommand(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 30;
                base.Data = DataPacket.Data;
                if (Data.Length < 4) throw new PacketCreationException("Data.Length < 4");
            }

            public Packet_30_AirCommand(uint _ID, string _Command, string _Argument)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 30;
                base.Data = new byte[0];
                ID = _ID;
                Command = _Command;
                Argument = _Argument;
            }

            public uint ID
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
            public string Command
            {
                get
                {
                    return _GetDataBytes(4, Data.Length - 4).ToDataString().Split(' ')[0];
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(4, Bits.ArrayCombine(value.ToByteArray(), new byte[1] { (byte)' ' }, Argument.ToByteArray(), new byte[1] { (byte)'\0' }));
                }
            }

            public string Argument
            {
                get
                {
                    return _GetDataBytes(4, Data.Length - 4).ToDataString().Split(' ')[0];
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(4, Bits.ArrayCombine(Argument.ToByteArray(), new byte[1] { (byte)' ' }, value.ToByteArray(), new byte[1] { (byte)'\0' }));
                }
            }
        }
    }
}
