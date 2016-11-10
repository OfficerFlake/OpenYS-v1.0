using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_01_Login : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_01_Login(GenericPacket DataPacket) : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 1;
                base.Data = DataPacket.Data;
                if (Data.Length < 20) throw new PacketCreationException("Packet Length < 20");
            }

            public Packet_01_Login(string _Username, uint _Version)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 1;
                base.Data = Bits.Repeat("\0", 20);
                Username = _Username;
                Version = _Version;
            }

            public string Username
            {
                get
                {
                    if (Data.Length > 20) return Data.Skip(20).ToArray().ToDataString().Split('\0')[0]; //Version > 20110207
                    else return _GetDataBytes(0, 16).ToArray().ToDataString().Split('\0')[0]; //Version <= 20110207
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(0, value.Resize(16).ToByteArray());
                    _SetDataBytes(20, value.ToByteArray());
                }
            }
            public uint Version
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(16,4), 0);
                }
                set
                {
                    _SetDataBytes(16, BitConverter.GetBytes(value));
                }
            }

            public override string ToString()
            {
                return "<" + Username + ">,<" + Version.ToString() + ">";
            }
        }
    }
}
