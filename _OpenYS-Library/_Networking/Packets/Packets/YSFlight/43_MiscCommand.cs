using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_43_MiscCommand : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_43_MiscCommand(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 43;
                base.Data = DataPacket.Data;
                if (Data.Length < 4) throw new PacketCreationException("Data.Length < 4");
            }

            public Packet_43_MiscCommand(string _Command, string _Argument)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 30;
                base.Data = new byte[0];
                Command = _Command;
                Argument = _Argument;
            }

            public string Command
            {
                get
                {
                    return _GetDataBytes(4, Data.Length-4).ToDataString().Split(' ')[0];
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(4, Bits.ArrayCombine(value.ToByteArray(), new byte[1]{(byte)' '}, Argument.ToByteArray()));
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
                    _SetDataBytes(4, Bits.ArrayCombine(Argument.ToByteArray(), new byte[1] { (byte)' ' }, value.ToByteArray()));
                }
            }
        }
    }
}
