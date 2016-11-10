using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_04_FieldName : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_04_FieldName(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 4;
                base.Data = DataPacket.Data;
                if (Data.Length < 60) throw new PacketCreationException("Packet Length < 60");
            }

            public Packet_04_FieldName(string _FieldName)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 4;
                base.Data = Bits.Repeat("\0", 60);
                if (_FieldName == "") throw new PacketCreationException("Blank Field Name.");
                FieldName = _FieldName;
            }

            public Packet_04_FieldName(string _FieldName, float _PosX, float _PosY, float _PosZ, float _RotX, float _RotY, float _RotZ)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 4;
                base.Data = Bits.Repeat("\0", 60);
                if (_FieldName == "") throw new PacketCreationException("Blank Field Name.");
                FieldName = _FieldName;
                PosX = _PosX;
                PosY = _PosY;
                PosZ = _PosZ;
                RotX = _RotX;
                RotY = _RotY;
                RotZ = _RotZ;
            }

            public string FieldName
            {
                get
                {
                    return _GetDataBytes(0, 32).ToDataString().Split('\0')[0];
                }
                set
                {
                    if (value == null) value = "";
                    _SetDataBytes(0, value.Resize(32).ToByteArray());
                }
            }

            public float PosX
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(32, 4), 0);
                }
                set
                {
                    _SetDataBytes(32, BitConverter.GetBytes(value));
                }
            }
            public float PosY
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(36, 4), 0);
                }
                set
                {
                    _SetDataBytes(36, BitConverter.GetBytes(value));
                }
            }
            public float PosZ
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(40, 4), 0);
                }
                set
                {
                    _SetDataBytes(40, BitConverter.GetBytes(value));
                }
            }

            public float RotX
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(44, 4), 0);
                }
                set
                {
                    _SetDataBytes(44, BitConverter.GetBytes(value));
                }
            }
            public float RotY
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(48, 4), 0);
                }
                set
                {
                    _SetDataBytes(48, BitConverter.GetBytes(value));
                }
            }
            public float RotZ
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(52, 4), 0);
                }
                set
                {
                    _SetDataBytes(52, BitConverter.GetBytes(value));
                }
            }

            public float Unknown
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(56, 4), 0);
                }
                set
                {
                    _SetDataBytes(56, BitConverter.GetBytes(value));
                }
            }
        }
    }
}
