using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_11_FlightData : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_11_FlightData(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 11;
                base.Data = DataPacket.Data;
                if (Data.Length < 12) throw new PacketCreationException("Data Length < 12");
            }

            public Packet_11_FlightData(int _Version)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 11;
                base.Data = new byte[0];
                switch (_Version)
                {
                    case 5:
                        goto case 3;
                    case 4:
                        goto case 3;
                    case 3:
                        //all good, version recognised.
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
                if (_Version == 3) _Init3(); //Long
                if (_Version == 4) _Init4(); //Short
                if (_Version == 5) _Init5(); //Short w/ B/Bay
                return true;
            }

            private bool _Init3()
            {
                Data = Bits.Repeat("\0", 95);
                Version = 3;
                return true;
            }

            private bool _Init4()
            {
                Data = Bits.Repeat("\0", 70);
                Version = 4;
                return true;
            }
            
            private bool _Init5()
            {
                Data = Bits.Repeat("\0", 70);
                Version = 5;
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

            public short Version 
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

            public float PosX
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(12, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(10, 4), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(12, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(10,  BitConverter.GetBytes(value));
                    }
                }
            }
            public float PosY
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(16, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(14, 4), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(16, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(14, BitConverter.GetBytes(value));
                    }
                }
            }
            public float PosZ
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(20, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(18, 4), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(20, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(18, BitConverter.GetBytes(value));
                    }
                }
            }

            public short HdgX
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(24, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(22, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(24, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(22, BitConverter.GetBytes(value));
                    }
                }
            }
            public short HdgY
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(26, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(24, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(26, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(24, BitConverter.GetBytes(value));
                    }
                }
            }
            public short HdgZ
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(28, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(26, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(28, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(26, BitConverter.GetBytes(value));
                    }
                }
            }

            public short V_PosX
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(30, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(28, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(30, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(28, BitConverter.GetBytes(value));
                    }
                }
            }
            public short V_PosY
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(32, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(30, 4), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(32, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(30, BitConverter.GetBytes(value));
                    }
                }
            }
            public short V_PosZ
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(34, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(32, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(34, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(32, BitConverter.GetBytes(value));
                    }
                }
            }

            public float V_HdgX
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(79, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(34, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(79, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(34, BitConverter.GetBytes((short)value));
                    }
                }
            }
            public float V_HdgY
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(83, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(36, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(83, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(36, BitConverter.GetBytes((short)value));
                    }
                }
            }
            public float V_HdgZ
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(87, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(38, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(87, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(38, BitConverter.GetBytes((short)value));
                    }
                }
            }

            public short LoadFactor
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(42, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(62);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(42, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(62, (byte)value);
                    }
                }
            }

            public ushort Ammo_GUN
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToUInt16(_GetDataBytes(44, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToUInt16(_GetDataBytes(54, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(44, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(54, BitConverter.GetBytes(value));
                    }
                }
            }
            public short Ammo_AAM
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(46, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(58);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(46, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(58, (byte)value);
                    }
                }
            }
            public short Ammo_AGM
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(48, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(59);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(48, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(59, (byte)value);
                    }
                }
            }
            public short Ammo_B500
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(50, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(60);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(50, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(60, (byte)value);
                    }
                }
            }
            
            public short Weight_SmokeOil
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(52, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(40, 2), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(52, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(40, BitConverter.GetBytes(value));
                    }
                }
            }

            public float Weight_Fuel
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(54, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt32(_GetDataBytes(42, 4), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(54, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(42, BitConverter.GetBytes((int)value));
                    }
                }
            }
            public float Weight_Payload
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToSingle(_GetDataBytes(58, 4), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt32(_GetDataBytes(46, 4), 0);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(58, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(46, BitConverter.GetBytes((int)value));
                    }
                }
            }

            public short Strength
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(_GetDataBytes(62, 2), 0);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(61);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(62, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(61, (byte)value);
                    }
                }
            }

            public byte FlightState
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(64);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(48);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(64, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(48, value);
                    }
                }
            }

            public byte Anim_VGW
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(65);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(49);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(65, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(49, value);
                    }
                }
            }
            public byte Anim_Boards
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(66);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(50) & "11110000".ToByte() //Select the TENS column of this Hex Digit.
                                )
                                // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(66, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(value & "11110000".ToByte());
                        byte Units = (byte)(GetDataByte(50) & "00001111".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(50, Combined);
                    }
                }
            }
            public byte Anim_Gear
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(67);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(50) & "00001111".ToByte() //Select the UNITS column of this Hex Digit.
                                )
                            // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(67, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(GetDataByte(50) & "00001111".ToByte());
                        byte Units = (byte)((byte)(value/255d*15) & "11110000".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(50, Combined);
                    }
                }
            }

            public byte Anim_Flaps
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(68);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(51) & "11110000".ToByte() //Select the TENS column of this Hex Digit.
                                )
                            // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(68, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(value & "11110000".ToByte());
                        byte Units = (byte)(GetDataByte(51) & "00001111".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(51, Combined);
                    }
                }
            }
            public byte Anim_Brake
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(69);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(51) & "00001111".ToByte() //Select the UNITS column of this Hex Digit.
                                )
                            // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(69, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(GetDataByte(51) & "11110000".ToByte());
                        byte Units = (byte)(value & "00001111".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(51, Combined);
                    }
                }
            }

            public byte _Anim_Flags
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(70);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(52);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(70, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(52, value);
                    }
                }
            }
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
                            if (value) _Anim_Flags  |=  "1000 0000".ToByte();
                            else _Anim_Flags        &=  "0111 1111".ToByte();
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
                            if (value) _Anim_Flags  |=  "0100 0000".ToByte();
                            else _Anim_Flags        &=  "1011 1111".ToByte();
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
                            if (value) _Anim_Flags  |=  "0010 0000".ToByte();
                            else _Anim_Flags        &=  "1101 1111".ToByte();
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
                            if (value) _Anim_Flags  |=  "0001 0000".ToByte();
                            else _Anim_Flags        &=  "1110 1111".ToByte();
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
                            if (value) _Anim_Flags  |=  "0000 1000".ToByte();
                            else _Anim_Flags        &=  "1111 0111".ToByte();
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

            public byte CPU_Flags
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(71);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(53);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(71, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(53, value);
                    }
                }
            }

            public byte Anim_Throttle
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(72);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return GetDataByte(63);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(72, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(63, value);
                    }
                }
            }

            public sbyte Anim_Elevator
            {
                get
                {
                    if (Version == 3)
                    {
                        return (sbyte)GetDataByte(73);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return (sbyte)GetDataByte(64);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(73, (byte)value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(64, (byte)value);
                    }
                }
            }
            public sbyte Anim_Aileron
            {
                get
                {
                    if (Version == 3)
                    {
                        return (sbyte)GetDataByte(74);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return (sbyte)GetDataByte(65);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(74, (byte)value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(65, (byte)value);
                    }
                }
            }
            public sbyte Anim_Rudder
            {
                get
                {
                    if (Version == 3)
                    {
                        return (sbyte)GetDataByte(75);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return (sbyte)GetDataByte(66);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(75, (byte)value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(66, (byte)value);
                    }
                }
            }
            public sbyte Anim_Trim
            {
                get
                {
                    if (Version == 3)
                    {
                        return (sbyte)GetDataByte(76);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return (sbyte)GetDataByte(67);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(76, (byte)value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        SetDataByte(67, (byte)value);
                    }
                }
            }

            public short Ammo_RKT
            {
                get
                {
                    if (Version == 3)
                    {
                        return BitConverter.ToInt16(Data, 77);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return BitConverter.ToInt16(Data, 56);
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        _SetDataBytes(77, BitConverter.GetBytes(value));
                    }
                    if (Version == 4 | Version == 5)
                    {
                        _SetDataBytes(56, BitConverter.GetBytes(value));
                    }
                }
            }


            public byte Anim_Nozzle
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(91);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(68) & "11110000".ToByte() //Select the TENS column of this Hex Digit.
                                )
                            // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(91, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(value & "11110000".ToByte());
                        byte Units = (byte)(GetDataByte(68) & "00001111".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(68, Combined);
                    }
                }
            }
            public byte Anim_Reverse
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(92);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(68) & "00001111".ToByte() //Select the UNITS column of this Hex Digit.
                                )
                            // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(92, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(GetDataByte(68) & "11110000".ToByte());
                        byte Units = (byte)(value & "00001111".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(68, Combined);
                    }
                }
            }

            public byte Anim_BombBay
            {
                get
                {
                    if (Version == 3)
                    {
                        return GetDataByte(93);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        return
                            (byte)
                            (
                                (byte)
                                (
                                    GetDataByte(69) & "11110000".ToByte() //Select the TENS column of this Hex Digit.
                                )
                            // >> 4 //No bit shift required...
                            );
                    }
                    return 0;
                }
                set
                {
                    if (Version == 3)
                    {
                        SetDataByte(93, value);
                    }
                    if (Version == 4 | Version == 5)
                    {
                        byte Tens = (byte)(value & "11110000".ToByte());
                        byte Units = (byte)(GetDataByte(69) & "00001111".ToByte());
                        byte Combined = (byte)(Tens | Units);
                        SetDataByte(69, Combined);
                    }
                }
            }
        }
    }
}
