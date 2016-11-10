using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_32_ChatMessage : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_32_ChatMessage(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 32;
                base.Data = DataPacket.Data;
                if (Data.Length < 8) throw new PacketCreationException("Data.Length < 8");
            }

            public Packet_32_ChatMessage(GenericPacket DataPacket, string ThisClientUsername)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 32;
                base.Data = DataPacket.Data;
                _Username = ThisClientUsername;
                if (Data.Length < 8) throw new PacketCreationException("Data.Length < 8");
            }

            /// <summary>
            /// Special Constructor - Allows for the setting of the clients username!
            /// </summary>
            /// <param name="DataPacket"></param>
            /// <param name="ThisClient"></param>
            public Packet_32_ChatMessage(string ThisClientUsername, string _Message)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 32;
                _Username = ThisClientUsername;
                FullMessage = "(" + _Username + ")" + _Message + "\0";
            }

            public Packet_32_ChatMessage(string _FullMessage)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 32;
                FullMessage = _FullMessage + "\0";
            }

            private string _Username = "";

            public string FullMessage
            {
                get
                {
                    return _GetDataBytes(8, Data.Length - 8).ToDataString().Split('\0')[0];
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(8, value.ToByteArray());
                }
            }

            public string Message
            {
                get
                {
                    return FullMessage.Substring(1 + _Username.Length + 1);
                }
                set
                {
                    FullMessage = FullMessage.Substring(0, 1 + _Username.Length + 1) + value + "\0";
                }
            }
        }
    }
}
