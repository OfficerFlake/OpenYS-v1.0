using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static partial class Packets
    {
        public class Packet_36_WeaponsConfig: GenericPacket
        {
            /// <summary>
            /// Constructor. Common to all inheriting packet types.
            /// </summary>
            /// <param name="Creator"></param>
            /// <param name="DataPacket"></param>
            public Packet_36_WeaponsConfig(GenericPacket DataPacket)
                : base() //base == parent. ie: create the parent with this argument.
            {
                    base.Type = 36;
                    base.Data = DataPacket.Data;
            }

            public Packet_36_WeaponsConfig(uint _ID)
                : base() //base == parent. ie: create the parent with this argument.
            {
                base.Type = 36;
                base.Data = new byte[0];
                ID = _ID;
            }

            public uint ID
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

            public ushort Version
            {
                get
                {
                    return BitConverter.ToUInt16(_GetDataBytes(4, 2), 0);
                }
                set
                {
                    _SetDataBytes(4, BitConverter.GetBytes(value));
                }
            }

            public class WeaponDescription
            {
                public ushort Weapon = 0;
                public ushort Ammo = 0;

                public WeaponDescription(ushort _Weapon, ushort _Ammo)
                {
                    Weapon = _Weapon;
                    Ammo = _Ammo;
                }
            }

            public List<WeaponDescription> WeaponsInfo
            {
                get
                {
                    if (Data.Length < 6)
                    {
                        Data = Bits.Repeat("\0", 6);
                    }
                    List<WeaponDescription> _WeaponsInfoOutput = new List<WeaponDescription>();
                    string DataString = Data.ToDataString();
                    for (int i = 6; i <= Data.Length - 4; i += 4)
                    {
                        try
                        {
                            _WeaponsInfoOutput.Add(
                                new WeaponDescription(
                                    BitConverter.ToUInt16(DataString.Substring(i + 0).ToByteArray(), 0),
                                    BitConverter.ToUInt16(DataString.Substring(i + 2).ToByteArray(), 0)
                                    )
                                    );
                        }
                        catch
                        {
                            //failed to load weapon info.
                            Debug.WriteLine("Failed to parse weapons info. Break here to inspect...");
                        }
                    }
                    return _WeaponsInfoOutput;
                }
                set
                {
                    if (Data.Length < 6)
                    {
                        Data = Bits.Repeat("\0", 6);
                    }
                    Data = Data.Take(6).ToArray();
                    for (int i = 0; i < value.Count; i++)
                    {
                        Data = Bits.ArrayCombine(Data, BitConverter.GetBytes(value.ToList()[i].Weapon));
                        Data = Bits.ArrayCombine(Data, BitConverter.GetBytes(value.ToList()[i].Ammo));
                    }
                }
            }

            public bool _AddWeapon(ushort _WeaponType, ushort _Ammo)
            {
                List<WeaponDescription> UpdateInfo = WeaponsInfo;
                UpdateInfo.Add(new WeaponDescription(_WeaponType, _Ammo));
                WeaponsInfo = UpdateInfo;
                return true;
            }
            public bool _RemoveWeapon(ushort _WeaponType)
            {
                List<WeaponDescription> UpdateInfo = WeaponsInfo;
                List<WeaponDescription> WeaponsToRemove = WeaponsInfo.Where(x=>x.Weapon == _WeaponType).ToList();
                UpdateInfo.RemoveAll(x => x.Weapon == _WeaponType);
                for(int i = 0; i < WeaponsToRemove.Count - 1; i++)
                {
                    UpdateInfo.Add(WeaponsToRemove[i]);
                }
                WeaponsInfo = UpdateInfo;
                return true;
            }
            public bool _RemoveAllWeapon(ushort _WeaponType)
            {
                List<WeaponDescription> UpdateInfo = WeaponsInfo;
                UpdateInfo.RemoveAll(x => x.Weapon == _WeaponType);
                WeaponsInfo = UpdateInfo;
                return true;
            }

            public static class WeaponTypes
            {
                public const ushort Null = 0;
                public const ushort AAM_Short = 1;
                public const ushort AGM = 11;
                public const ushort B500 = 3;
                public const ushort FLRPOD = 5;
                public const ushort RKT = 4;
                public const ushort FLR = 200;
                public const ushort AAM_Mid = 6;
                public const ushort B250 = 7;
                public const ushort Unknown_8 = 8;
                public const ushort B500_HD = 9;
                public const ushort AAM_X = 10;
                //public const ushort Unknown_11 = 11;
                public const ushort FuelTank = 12;
            }
        }
    }
}
