using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_20_OrdinanceLaunch : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_20_OrdinanceLaunch(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 20;
                base.Data = DataPacket.Data;
                if (Data.Length < 44) throw new PacketCreationException("Data.Length < 44");
            }

            public Packet_20_OrdinanceLaunch()
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 20;
                base.Data = Bits.Repeat("\0", 48);
                //SenderID = _SenderID;
            }

            public static class OrdinanceTypes
            {
                public const ushort Null = 0;
                public const ushort AAM_Short = 1;
                public const ushort AGM = 2;
                public const ushort B500 = 3;
                public const ushort RKT = 4;
                public const ushort FLR = 5;
                public const ushort AAM_Mid = 6;
                public const ushort B250 = 7;
                public const ushort Unknown_8 = 8;
                public const ushort B500_HD = 9;
                public const ushort AAM_X = 10;
                public const ushort Unknown_11 = 11;
                public const ushort FuelTank = 12;
            }

            public ushort OrdinanceType
            {
                //0:2 - OrdinanceType (USHORT)
                //1	    AAM(S)
                //2	    AGM
                //3	    B500
                //4	    RKT
                //5	    FLR
                //6	    AAM(M)
                //7	    B250
                //8	    ???
                //9	    B500HD
                //10	A-AAM
                //11	???
                //12	FUEL
                get
                {
                    return BitConverter.ToUInt16(_GetDataBytes(0,4), 0);
                }
                set
                {
                    _SetDataBytes(0, BitConverter.GetBytes(value));
                }
            }
            public float PosX
            {
                //2:6 - PosX (Meters) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(2, 4), 0);
                }
                set
                {
                    _SetDataBytes(2, BitConverter.GetBytes(value));
                }
            }
            public float PosY
            {
                //6:10 - PosY (Meters) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(6, 4), 0);
                }
                set
                {
                    _SetDataBytes(6, BitConverter.GetBytes(value));
                }
            }
            public float PosZ
            {
                //10:14- PosZ (Meters) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(10, 4), 0);
                }
                set
                {
                    _SetDataBytes(10, BitConverter.GetBytes(value));
                }
            }

            public float HdgX
            {
                //14:18 - HdgX (Radians) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(14, 4), 0);
                }
                set
                {
                    _SetDataBytes(14, BitConverter.GetBytes(value));
                }
            }
            public float HdgY
            {
                //18:22 - HdgY (Radians) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(18, 4), 0);
                }
                set
                {
                    _SetDataBytes(18, BitConverter.GetBytes(value));
                }
            }
            public float HdgZ
            {
                //22:26 - HdgZ (Radians) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(22, 4), 0);
                }
                set
                {
                    _SetDataBytes(22, BitConverter.GetBytes(value));
                }
            }

            public float InitVelocity
            {
                //26:30 - InitVelocity (M/S) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(26, 4), 0);
                }
                set
                {
                    _SetDataBytes(26, BitConverter.GetBytes(value));
                }
            }

            public float BurnoutDistance
            {
                //30:34 - Burnout Distance (FLOAT)
                //1 = No Burnout.
                //0 = Instant Burnout??? (Shrug?)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(30, 4), 0);
                }
                set
                {
                    _SetDataBytes(30, BitConverter.GetBytes(value));
                }
            }

            public uint MaximumDamage
            {
                //34:38 - MaximumDamage (UINT)
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(34, 4), 0);
                }
                set
                {
                    _SetDataBytes(34, BitConverter.GetBytes(value));
                }
            }

            public ushort SenderType
            {
                //38:40 - SenderType (USHORT)
                get
                {
                    return BitConverter.ToUInt16(_GetDataBytes(38, 2), 0);
                }
                set
                {
                    _SetDataBytes(38, BitConverter.GetBytes(value));
                }
            }
            public uint SenderID
            {
                //40:44 - Sender ID. (UINT)
                get
                {
                    return BitConverter.ToUInt32(_GetDataBytes(40, 4), 0);
                }
                set
                {
                    _SetDataBytes(40, BitConverter.GetBytes(value));
                }
            }

            public float MaximumVelocity
            {
                //44:48 - Maximum Velocity. (Optional!) (FLOAT)
                get
                {
                    return BitConverter.ToSingle(_GetDataBytes(44, 4), 0);
                }
                set
                {
                    _SetDataBytes(44, BitConverter.GetBytes(value));
                }
            }
        }
    }
}
