using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace OpenYS
{
    public static partial class Commands
    {
        public static readonly CommandDescriptor OpenYS_Command_Flight_Immortal = new CommandDescriptor
        {
            _Name = "Immortal",
            _Version = 1.0,
            _Date = new DateTime(2014, 06, 01),
            _Author = "OfficerFlake",

            _Category = "Flight",
            _Hidden = false,

            _Descrption = "Makes you invincible.",
            _Usage = "/Immortal",
            _Commands = new string[] { "/Flight.Immortal", "/Immortal", "/Flight.Invincible", "/Invincible", },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.User_Console       |
                Requirement.User_YSFlight |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                Requirement.Status_Connected |
                Requirement.Status_Flying |
                //Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Flight_Immortal_Method,
        };

        public static bool OpenYS_Command_Flight_Immortal_Method(Client ThisClient, CommandReader Command)
        {
            Packets.Packet_36_WeaponsConfig Loading = ThisClient.Vehicle.WeaponsLoading;
            bool HasWeapons = false;
            #region Determine if Armed.
            foreach (ushort WeaponID in Loading.WeaponsInfo.Select(x => x.Weapon))
            {
                switch (WeaponID)
                {
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.AAM_Mid:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.AAM_Short:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.AAM_X:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.AGM:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.B250:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.B500:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.B500_HD:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.RKT:
                        goto HasWeapons;
                    case Packets.Packet_36_WeaponsConfig.WeaponTypes.FuelTank:
                        //I am well aware a fuel tank isn't really a "weapon" but because it can be used as such we better disable it! Tough luck...
                        goto HasWeapons;
                    default:
                        continue;
                    HasWeapons:
                        HasWeapons = true;
                        continue;
                }
            }
            #endregion
            if (HasWeapons)
            {
                ThisClient.SendMessage("Can not make you immortal as you spawned with weapons loaded. Spawn again without weapons if you wish to be immortal!");
                return false;
            }
            //Not armed.
            ThisClient.Vehicle.Invincible = true;
            ThisClient.SendMessage("You are now immortal! Happy flying!");
            Packets.Packet_30_AirCommand DisableGuns = new Packets.Packet_30_AirCommand(ThisClient.Vehicle.ID, "INITIGUN", "0");
            Clients.LoggedIn.SendPacket(DisableGuns);
            return true;
        }
    }
}