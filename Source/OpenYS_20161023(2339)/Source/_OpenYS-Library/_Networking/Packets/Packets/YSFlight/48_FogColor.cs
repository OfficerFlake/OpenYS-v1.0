using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_48_FogColor : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_48_FogColor(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                    base.Type = 48;
                    base.Data = DataPacket.Data;
            }

            public Packet_48_FogColor(byte _Red, byte _Green, byte _Blue)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 48;
                base.Data = new byte[0];
                Red = _Red;
                Green = _Green;
                Blue = _Blue;
            }

            public Packet_48_FogColor(Colors.XRGBColor _Color)
            {
                base.Type = 48;
                base.Data = new byte[0];
                Red = _Color.Red;
                Green = _Color.Green;
                Blue = _Color.Blue;
            }

            public byte Red
            {
                get
                {
                    return GetDataByte(0);
                }
                set
                {
                    SetDataByte(0, value);
                }
            }
            public byte Green
            {
                get
                {
                    return GetDataByte(1);
                }
                set
                {
                    SetDataByte(1, value);
                }
            }
            public byte Blue
            {
                get
                {
                    return GetDataByte(2);
                }
                set
                {
                    SetDataByte(2, value);
                }
            }
            public Colors.XRGBColor Color
            {
                get
                {
                    return new Colors.XRGBColor(Red, Green, Blue);
                }
                set
                {
                    Red = value.Red;
                    Green = value.Green;
                    Blue = value.Blue;
                }
            }
        }
    }
}
