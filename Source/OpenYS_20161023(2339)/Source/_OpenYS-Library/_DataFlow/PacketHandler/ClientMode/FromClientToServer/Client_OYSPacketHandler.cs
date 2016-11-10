using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OpenYS
{
    public static partial class PacketHandler
    {
        public static partial class ClientMode
        {
            public static partial class FromClientToServer
            {
                #region CLIENT
                public static bool OYSHandle(Client ThisClient, Packets.GenericPacket InPacket)
                {
                    #region PacketSwitch
                    if (InPacket == null | InPacket == Packets.NoPacket)
                    {
                        return false;
                    }
                    switch (InPacket.Type)
                    {
                        default:
#if DEBUG
                        //Console.WriteLine("Unknown Custom Packet From " + ThisClient.Username);
                        //Console.WriteLine("-Type: " +  InPacket.Type.ToString());
                        //Console.WriteLine("-Data: " +  InPacket.Data.ToString());
#endif
                            break;
                    }
                    #endregion
                    return true;
                }
                #endregion
            }
        }
    }
}
