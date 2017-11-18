using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_08_JoinRequest : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_08_JoinRequest(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 8;
                base.Data = DataPacket.Data;
                if (Data.Length < 71) throw new PacketCreationException("Data Length < 71");
            }

            public Packet_08_JoinRequest(uint _IFF, string _AircraftName, string _StartPositionName, byte _FuelPercent)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 8;
                base.Data = Bits.Repeat("\0", 72);
                IFF = _IFF;
                AircraftName = _AircraftName;
                StartPositionName = _StartPositionName;
                FuelPercent = _FuelPercent;
            }

            public uint IFF
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

            public string AircraftName
            {
                get
                {
                    return _GetDataBytes(4, 32).ToDataString().Split('\0')[0];
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(4, value.Resize(32).ToByteArray());
                }
            }

            public string StartPositionName
            {
                get
                {
                    return _GetDataBytes(36, 32).ToDataString();
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(36, value.Resize(32).ToByteArray());
                }
            }

            public byte FuelPercent
            {
                get
                {
                    return GetDataByte(70);
                }
                set
                {
                    SetDataByte(70, value);
                }
            }
        }
    }
}
