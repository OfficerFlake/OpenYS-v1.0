using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_05_EntityJoined : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_05_EntityJoined(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 5;
                base.Data = DataPacket.Data;
                if (Data.Length < 60) throw new PacketCreationException("Packet Length < 60");
            }

            public Packet_05_EntityJoined()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 5;
                Initialise();
            }

            public bool Initialise()
            {
                Data = Bits.Repeat("\0", 108);
                Data = Bits.ArrayCombine(Data, new byte[16]{ 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00});
                Data = Bits.ArrayCombine(Data, Bits.Repeat("\0", 48));
                return true;
            }

            public uint ObjectType //0:4
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(0,4), 0);
                }
                set
                {
                    _SetDataBytes(0, BitConverter.GetBytes(value));
                }
            }

                public bool IsAircraft
                {
                    get
                    {
                        return (ObjectType == 0);
                    }
                    set
                    {
                        if (value)
                        {
                            ObjectType = 0;
                        }
                        else
                        {
                            ObjectType = 65537;
                        }
                    }
                }
                public bool IsGround
                {
                    get
                    {
                        return (ObjectType == 65537);
                    }
                    set
                    {
                        if (value)
                        {
                            ObjectType = 65537;
                        }
                        else
                        {
                            ObjectType = 0;
                        }
                    }
                }

            public uint ID //4:8
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

            public uint IFF //8:12
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(8, 4), 0);
                }
                set
                {
                    _SetDataBytes(8, BitConverter.GetBytes(value));
                }
            }

            public float PosX //12:16
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

            public float PosY //16:20
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

            public float PosZ //20:24
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

            public float RotX //24:28
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(24, 4), 0);
                }
                set
                {
                    _SetDataBytes(24, BitConverter.GetBytes(value));
                }
            }

            public float RotY //28:32
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(28, 4), 0);
                }
                set
                {
                    _SetDataBytes(28, BitConverter.GetBytes(value));
                }
            }

            public float RotZ //32:36
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

            public string Identify //36:68
            {
                get
                {
                    return _GetDataBytes(36, 32).ToDataString();
                }
                set
                {
                    if (value == null) value = "\0";
                    _SetDataBytes(36, Strings.Resize(value, 32).ToByteArray());
                }
            }

            //32 Bytes??? //68:100

            //4 Bytes??? //100:104

            public uint Flags //104:108
            {
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(104, 4), 0);
                }
                set
                {
                    _SetDataBytes(104, BitConverter.GetBytes(value));
                }
            }

            public byte OwnerType //108:109
            {
                get
                {
                    return _GetDataBytes(108, 1)[0];
                }
                set
                {
                    _SetDataBytes(108, new byte[1]{value});
                }
            }

                public bool IsOwnedByThisPlayer
                {
                    get
                    {
                        return (OwnerType == 3);
                    }
                    set //set TRUE only!
                    {
                        if (value)
                        {
                            OwnerType = 3;
                        }
                    }
                }
                public bool IsOwnedByOtherPlayer
                {
                    get
                    {
                        return (OwnerType == 2);
                    }
                    set //set TRUE only!
                    {
                        if (value)
                        {
                            OwnerType = 2;
                        }
                    }
                }
                public bool IsOwnedByNobody
                {
                    get
                    {
                        return (OwnerType == 0);
                    }
                    set //set TRUE only!
                    {
                        if (value)
                        {
                            OwnerType = 0;
                        }
                    }
                }

            //15 Bytes??? //109:124

            public string OwnerName //124:((140):(180)?)
            {
                get
                {
                    return _GetDataBytes(124, 16).ToDataString();
                }
                set
                {
                    if (value == null) value = "\0";
                    _SetDataBytes(124, Strings.Resize(value, 16).ToByteArray());
                }
            }
        }
    }
}
