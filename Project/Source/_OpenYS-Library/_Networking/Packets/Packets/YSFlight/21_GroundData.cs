using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_21_GroundData : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_21_GroundData(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 21;
                base.Data = DataPacket.Data;
                if (Data.Length < 12) throw new PacketCreationException("Data Length < 12");
            }

            public Packet_21_GroundData(int _Version)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 21;
                base.Data = new byte[0];
                switch (_Version)
                {
                    case 1:
                        //all good, do nothing.
                        Initialise(_Version);
                        break;
                    default:
                        throw new PacketCreationException("Version Number not recognised!");
                }

                Initialise(_Version);
            }

            public bool Initialise(int _Version)
            {
                Version = (short)_Version;
                if (_Version == 1) _Init1();
                return true;
            }

            private bool _Init1()
            {
                Data = new byte[48];
                Version = 1;
                return true;
            }

            public float TimeStamp
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(0, 4), 0);
                }
                set
                {
                    _SetDataBytes(0, BitConverter.GetBytes(value));
                }
            }

            public uint ID
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(4, 4), 0);
                }
                set
                {
                    _SetDataBytes(4, BitConverter.GetBytes(value));
                }
            }

            public short Strength
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(8, 2), 0);
                }
                set
                {
                    _SetDataBytes(8, BitConverter.GetBytes(value));
                }
            }

            public short Version
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(10, 2), 0);
                }
                set
                {
                    _SetDataBytes(10, BitConverter.GetBytes(value));
                }
            }

            public float PosX
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(12, 4), 0);
                }
                set
                {
                    _SetDataBytes(12, BitConverter.GetBytes(value));
                }
            }
            public float PosY
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(16, 4), 0);
                }
                set
                {
                    _SetDataBytes(16, BitConverter.GetBytes(value));
                }
            }
            public float PosZ
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(20, 4), 0);
                }
                set
                {
                    _SetDataBytes(20, BitConverter.GetBytes(value));
                }
            }

            public short HdgX
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(24, 2), 0);
                }
                set
                {
                    _SetDataBytes(24, BitConverter.GetBytes(value));
                }
            }
            public short HdgY
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(26, 2), 0);
                }
                set
                {
                    _SetDataBytes(26, BitConverter.GetBytes(value));
                }
            }
            public short HdgZ
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(28, 2), 0);
                }
                set
                {
                    _SetDataBytes(28, BitConverter.GetBytes(value));
                }
            }

            public byte _Anim_Flags
            {
                get
                {
                    return GetDataByte(30);
                }
                set
                {
                    SetDataByte(30, value);
                }
            }
                public bool Anim_Guns
            {
                get
                {
                    if (Version == 1)
                    {
                        return ((_Anim_Flags & "0000 0001".ToByte()) == "0000 0001".ToByte());
                    }
                    return false;
                }
                set
                {
                    if (Version == 1)
                    {
                        if (value) _Anim_Flags |= "0000 0001".ToByte();
                        else _Anim_Flags &= "1111 1110".ToByte();
                    }
                }
            }

            public byte _CPU_Flags
            {
                get
                {
                    return GetDataByte(31);
                }
                set
                {
                    SetDataByte(31, value);
                }
            }

            public short V_PosX
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(32, 2), 0);
                }
                set
                {
                    _SetDataBytes(32, BitConverter.GetBytes(value));
                }
            }
            public short V_PosY
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(34, 2), 0);
                }
                set
                {
                    _SetDataBytes(34, BitConverter.GetBytes(value));
                }
            }
            public short V_PosZ
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(36, 2), 0);
                }
                set
                {
                    _SetDataBytes(36, BitConverter.GetBytes(value));
                }
            }

            public short V_Rotation
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(38, 2), 0);
                }
                set
                {
                    _SetDataBytes(38, BitConverter.GetBytes(value));
                }
            }
            //38-39-V_HdgX ??? Seems to adjust the V_HdgX but it doesn't really have a use...
        }
    }
}
