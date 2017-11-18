using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_41_UsernameDistance : GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_41_UsernameDistance(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 41;
                base.Data = DataPacket.Data;
            }

            public Packet_41_UsernameDistance(uint _UsernameDistance)
                : base() //base == parent. ie: create the parent with this argument.
            {
                _Packet_41_UsernameDistance(_UsernameDistance, true);
            }

            public Packet_41_UsernameDistance(uint _UsernameDistance, bool ShowWarning)
                : base() //base == parent. ie: create the parent with this argument.
            {
                _Packet_41_UsernameDistance(_UsernameDistance, ShowWarning);
            }

            public void _Packet_41_UsernameDistance(uint _UsernameDistance, bool ShowWarning)
            {
                base.Type = 41;
                UsernameDistance = _UsernameDistance;
                #region Incorrect Usage Style Warning...
#if DEBUG
                if (_UsernameDistance <= 2 & _UsernameDistance > 0 & ShowWarning)
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("WARNING: UsernameDistance Packet created with argument 0 < X <= 2");
                    System.Console.WriteLine("");
                    System.Console.WriteLine("These values are special to YS and do not set the range, but functionality!");
                    System.Console.WriteLine("Use only if you know what you are doing!");
                    System.Console.WriteLine("You should use instead the boolean setters for this packet!");
                }
#endif
                #endregion
            }

            public Packet_41_UsernameDistance(bool _ShowUsernames)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 41;
                if (_ShowUsernames) Always = true;
                else Never = true;
            }

            public Packet_41_UsernameDistance(bool _ShowUsernames, bool ShowWarning)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 41;
                if (_ShowUsernames) Always = true;
                else Never = true;
            }

            public uint UsernameDistance
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

            public bool Always
            {
                get
                {
                    return (UsernameDistance == 1);
                }
                set
                {
                    if (value) UsernameDistance = 1;
                    else Never = true;
                }
            }

            public bool Never
            {
                get
                {
                    return (UsernameDistance == 2);
                }
                set
                {
                    if (value) UsernameDistance = 2;
                    else Always = true;
                }
            }
        }
    }
}
