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
        /// This packet tells the receiving server/client that the aircraft should be positioned RELATIVE to the target aircraft
        /// a lot of work went into this packet type, please don't rip it off!
        /// </summary>
        public class Packet_64_11_FormationData : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_64_11_FormationData(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 11;
                if (DataPacket.Data.Length == 0) DataPacket.Data = new byte[52];
                base.Data = DataPacket.Data;
                if (Data.Length < 16) throw new PacketCreationException("Data.Length < 16");
            }

            public Packet_64_11_FormationData(short _Version)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 11;
                switch (_Version)
                {
                    case 5:
                        Data = new byte[52];
                        Version = _Version;
                        break;
                    default:
                        throw new PacketCreationException("Version Number not recognised!");
                }
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
            } //0:4

            public uint SenderID
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(4, 4), 0);
                }
                set
                {
                    _SetDataBytes(4, BitConverter.GetBytes(value));
                }
            } //4:8
            public uint TargetID
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(8, 4), 0);
                }
                set
                {
                    _SetDataBytes(8, BitConverter.GetBytes(value));
                }
            } //8:12

            public short Version
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(12, 2), 0);
                }
                set
                {
                    _SetDataBytes(12, BitConverter.GetBytes(value));
                }
            } //12:14

            public float PosX
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(14, 4), 0);
                }
                set
                {
                    _SetDataBytes(14, BitConverter.GetBytes(value));
                }
            } //14:18
            public float PosY
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(18, 4), 0);
                }
                set
                {
                    _SetDataBytes(18, BitConverter.GetBytes(value));
                }
            } //18:22
            public float PosZ
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(22, 4), 0);
                }
                set
                {
                    _SetDataBytes(22, BitConverter.GetBytes(value));
                }
            } //22:26

            public short HdgX
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(26, 2), 0);
                }
                set
                {
                    _SetDataBytes(26, BitConverter.GetBytes(value));
                }
            } //26:28
            public short HdgY
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(28, 2), 0);
                }
                set
                {
                    _SetDataBytes(28, BitConverter.GetBytes(value));
                }
            } //28:30
            public short HdgZ
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(30, 2), 0);
                }
                set
                {
                    _SetDataBytes(30, BitConverter.GetBytes(value));
                }
            } //30:32

            public short V_HdgX
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(32, 2), 0);
                }
                set
                {
                    _SetDataBytes(32, BitConverter.GetBytes(value));
                }
            } //32:34
            public short V_HdgY
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(34, 2), 0);
                }
                set
                {
                    _SetDataBytes(34, BitConverter.GetBytes(value));
                }
            } //34:36
            public short V_HdgZ
            {
                get
                {
                    return BitConverter.ToInt16(_GetDataBytes(36, 2), 0);
                }
                set
                {
                    _SetDataBytes(36, BitConverter.GetBytes(value));
                }
            } //36:38

            public sbyte Anim_Aileron
            {
                get
                {
                    return (sbyte)GetDataByte(38);
                }
                set
                {
                    SetDataByte(38, (byte)value);
                }
            } //38:39
            public byte Anim_Boards
            {
                get
                {
                    return GetDataByte(39);
                }
                set
                {
                    SetDataByte(39, value);
                }
            } //39:40
            public byte Anim_BombBay
            {
                get
                {
                    return GetDataByte(40);
                }
                set
                {
                    SetDataByte(40, value);
                }
            } //40:41
            public byte Anim_Brake
            {
                get
                {
                    return GetDataByte(41);
                }
                set
                {
                    SetDataByte(41, value);
                }
            } //41:42
            public sbyte Anim_Elevator
            {
                get
                {
                    return (sbyte)GetDataByte(42);
                }
                set
                {
                    SetDataByte(42, (byte)value);
                }
            } //42:43
            public byte Anim_Flaps
            {
                get
                {
                    return GetDataByte(43);
                }
                set
                {
                    SetDataByte(43, value);
                }
            } //43:44
            public byte Anim_Gear
            {
                get
                {
                    return GetDataByte(44);
                }
                set
                {
                    SetDataByte(44, value);
                }
            } //44:45
            public byte Anim_Nozzle
            {
                get
                {
                    return GetDataByte(45);
                }
                set
                {
                    SetDataByte(45, value);
                }
            } //45:46
            public byte Anim_Reverse
            {
                get
                {
                    return GetDataByte(46);
                }
                set
                {
                    SetDataByte(46, value);
                }
            } //46:47
            public sbyte Anim_Rudder
            {
                get
                {
                    return (sbyte)GetDataByte(47);
                }
                set
                {
                    SetDataByte(47, (byte)value);
                }
            } //47:48
            public byte Anim_Throttle
            {
                get
                {
                    return GetDataByte(48);
                }
                set
                {
                    SetDataByte(48, value);
                }
            } //48:49
            public sbyte Anim_Trim
            {
                get
                {
                    return (sbyte)GetDataByte(49);
                }
                set
                {
                    SetDataByte(49, (byte)value);
                }
            } //49:50
            public byte Anim_VGW
            {
                get
                {
                    return GetDataByte(50);
                }
                set
                {
                    SetDataByte(50, value);
                }
            } //50:51

            public byte _Anim_Flags
            {
                get
                {
                    return GetDataByte(51);
                }
                set
                {
                    SetDataByte(51, value);
                }
            } //51:52
                public bool Anim_Light_Land
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "1000 0000".ToByte()) == "1000 0000".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "1000 0000".ToByte();
                            else _Anim_Flags &= "0111 1111".ToByte();
                        }
                    }
                }
                public bool Anim_Light_Strobe
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0100 0000".ToByte()) == "0100 0000".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0100 0000".ToByte();
                            else _Anim_Flags &= "1011 1111".ToByte();
                        }
                    }
                }
                public bool Anim_Light_Nav
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0010 0000".ToByte()) == "0010 0000".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0010 0000".ToByte();
                            else _Anim_Flags &= "1101 1111".ToByte();
                        }
                    }
                }
                public bool Anim_Light_Beacon
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0001 0000".ToByte()) == "0001 0000".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0001 0000".ToByte();
                            else _Anim_Flags &= "1110 1111".ToByte();
                        }
                    }
                }
                public bool Anim_Guns
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0000 1000".ToByte()) == "0000 1000".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0000 1000".ToByte();
                            else _Anim_Flags &= "1111 0111".ToByte();
                        }
                    }
                }
                public bool Anim_Contrails
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0000 0100".ToByte()) == "0000 0100".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0000 0100".ToByte();
                            else _Anim_Flags &= "1111 1011".ToByte();
                        }
                    }
                }
                public bool Anim_Smoke
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0000 0010".ToByte()) == "0000 0010".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0000 0010".ToByte();
                            else _Anim_Flags &= "1111 1101".ToByte();
                        }
                    }
                }
                public bool Anim_Burners
                {
                    get
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            return ((_Anim_Flags & "0000 0001".ToByte()) == "0000 0001".ToByte());
                        }
                        return false;
                    }
                    set
                    {
                        if (Version == 3 | Version == 4 | Version == 5)
                        {
                            if (value) _Anim_Flags |= "0000 0001".ToByte();
                            else _Anim_Flags &= "1111 1110".ToByte();
                        }
                    }
                }
        }
    }
}
