using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_33_Weather : GenericPacket
        {            

            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_33_Weather(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 33;
                base.Data = DataPacket.Data;
            }

            public Packet_33_Weather()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 33;
            }

            /// <summary>
            /// Special Constructor - Allows to say if is a request or not.
            /// </summary>
            /// <param name="DataPacket"></param>
            /// <param name="IsResponse"></param>
            public Packet_33_Weather(bool IsRequest)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 33;
                if (!IsRequest)
                {
                    InitResponse();
                }
            }

            public uint Lighting
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
            public uint _Options
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
                public bool Control_LandEverywhere
                {
                    get
                    {
                        return ((_Options & "1000 0000".ToByte()) == "1000 0000".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "1000 0000".ToByte();
                        else _Options &= "0111 1111".ToByte();
                    }
                }
                public bool Control_Collisions
                {
                    get
                    {
                        return ((_Options & "0010 0000".ToByte()) == "0010 0000".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0010 0000".ToByte();
                        else _Options &= "1101 1111".ToByte();
                    }
                }
                public bool Control_BlackOut
                {
                    get
                    {
                        return ((_Options & "0000 1000".ToByte()) == "0000 1000".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0000 1000".ToByte();
                        else _Options &= "1111 0111".ToByte();
                    }
                }
                public bool Control_Fog
                {
                    get
                    {
                        return ((_Options & "0000 0010".ToByte()) == "0000 0010".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0000 0010".ToByte();
                        else _Options &= "1111 1101".ToByte();
                    }
                }
                public bool Enable_LandEverywhere
                {
                    get
                    {
                        return ((_Options & "0100 0000".ToByte()) == "0100 0000".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0100 0000".ToByte();
                        else _Options &= "1011 1111".ToByte();
                    }
                }
                public bool Enable_Collisions
                {
                    get
                    {
                        return ((_Options & "0001 0000".ToByte()) == "0001 0000".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0001 0000".ToByte();
                        else _Options &= "1110 1111".ToByte();
                    }
                }
                public bool Enable_BlackOut
                {
                    get
                    {
                        return ((_Options & "0000 0100".ToByte()) == "0000 0100".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0000 0100".ToByte();
                        else _Options &= "1111 1011".ToByte();
                    }
                }
                public bool Enable_Fog
                {
                    get
                    {
                        return ((_Options & "0000 0001".ToByte()) == "0000 0001".ToByte());
                    }
                    set
                    {
                        if (value) _Options |= "0000 0001".ToByte();
                        else _Options &= "1111 1110".ToByte();
                    }
                }

            public float WindX
            {
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(8, 4), 0);
                }
                set
                {
                    _SetDataBytes(8, BitConverter.GetBytes(value));
                }
            }
            public float WindY
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
            public float WindZ
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
            public float Fog
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

            public bool InitResponse()
            {
                Data = Bits.Repeat("\0", 24);
                return true;
            }

        }
    }
}
