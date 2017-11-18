using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenYS
{
    public static class Vehicles
    {
        private static ReaderWriterLockSlim _ListLock = new ReaderWriterLockSlim();
        private static List<Vehicle> _List = new List<Vehicle>();
        public static List<Vehicle> List
        {
            get
            {
                try
                {
                    _ListLock.EnterReadLock();
                    return _List;
                }
                finally
                {
                    _ListLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _ListLock.EnterWriteLock();
                    _List = value;
                }
                finally
                {
                    _ListLock.ExitWriteLock();
                }
            }
        }

        public static Vehicle NoVehicle = new Vehicle();

        public static uint GetDistance(Vehicle Vehicle1, Vehicle Vehicle2)
        {
            uint Distance = (uint)
            (
                Math.Sqrt
                (
                    Math.Pow((Vehicle1.PosX - Vehicle2.PosX), 2) + 
                    Math.Pow((Vehicle1.PosY - Vehicle2.PosY), 2) + 
                    Math.Pow((Vehicle1.PosZ - Vehicle2.PosZ), 2)
                )
            );
            return Distance;
        }

        public static Vehicle Find(uint ID)
        {
            Vehicle Output = NoVehicle;
            if (List.Select(x => x.ID).Contains(ID))
            {
                Output = List.FindLast(x => x.ID == ID);
            }
            return Output;
        }
    }

    public static class VehiclesHistory
    {
        //maintains the history of ALL Vehicles - used in the "/ResumeFlight " command!

        private static ReaderWriterLockSlim _ListLock = new ReaderWriterLockSlim();
        private static List<Vehicle> _List = new List<Vehicle>();
        public static List<Vehicle> List
        {
            get
            {
                try
                {
                    _ListLock.EnterReadLock();
                    return _List;
                }
                finally
                {
                    _ListLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _ListLock.EnterWriteLock();
                    _List = value;
                }
                finally
                {
                    _ListLock.ExitWriteLock();
                }
            }
        }

        public static Vehicle Find(uint ID)
        {
            Vehicle Output = Vehicles.NoVehicle;
            if (List.Select(x => x.ID).Contains(ID))
            {
                Output = List.FindLast(x => x.ID == ID);
            }
            return Output;
        }
    }

    public class Vehicle
    {
        #region Variables
        public uint Type = 0;
        public string OwnerName = "nameless";
        public uint ID = 0;
        public float TimeStamp = 0;
        //public float LastClientTimeStamp = 0;
        public string Identify = "";
        public uint IFF = 0;
        public float PosX = 0;
        public float PosY = 0;
        public float PosZ = 0;
        public short HdgX = 0;
        public short HdgY = 0;
        public short HdgZ = 0;
        public short V_PosX = 0;
        public short V_PosY = 0;
        public short V_PosZ = 0;
        public float V_HdgX = 0;
        public float V_HdgY = 0;
        public float V_HdgZ = 0;
        public float Weight_Fuel = 0;
        public float Weight_SmokeOil = 0;

        public float Prev_TimeStamp = 0;
        public float Prev_PosX = 0;
        public float Prev_PosY = 0;
        public float Prev_PosZ = 0;

        public byte Anim_Gear = 0;
        public byte Anim_Throttle = 0;
        public byte Anim_Boards = 0;
        public byte Anim_VGW = 0;
        public byte Anim_BombBay = 0;
        public byte Anim_Nozzle = 0;
        public byte _Anim_Flags = 0;
        
        public short Strength = 0;

        public MetaData.Aircraft MetaAircraft = MetaData._Aircraft.None;
        public CachedData.Aircraft CachedAircraft = CachedData._Aircraft.None;
        public Packets.Packet_36_WeaponsConfig WeaponsLoading = new Packets.Packet_36_WeaponsConfig(0);
        public bool Refuelling = false;
        public DateTime LastUpdated = DateTime.Now;
        public bool Invincible = false;
        public bool HasAddedWeapons = false;
        public bool IsVirtualVehicle = false; //virtual vehicles are not owned by a YSFClient - they are managed by the server!
        public MetaData.Ground VirtualCarrierObject_MetaData = MetaData._Ground.None;
        public uint VirtualCarrierObject_ID = 0;
        #endregion

        public Packets.Packet_05_EntityJoined GetJoinPacket(bool _OwnedByPlayerRequesting)
        {
            Packets.Packet_05_EntityJoined EntityJoined = new Packets.Packet_05_EntityJoined();
            EntityJoined.ObjectType = Type;
            EntityJoined.ID = ID;
            EntityJoined.IFF = IFF;
            EntityJoined.PosX = PosX;
            EntityJoined.PosY = PosY;
            EntityJoined.PosZ = PosZ;
            EntityJoined.RotX = HdgX;
            EntityJoined.RotY = HdgY;
            EntityJoined.RotZ = HdgZ;
            EntityJoined.Identify = Identify;
            EntityJoined.OwnerName = OwnerName;
            if (_OwnedByPlayerRequesting)
            {
                EntityJoined.IsOwnedByThisPlayer = true;
            }
            else
            {
                EntityJoined.IsOwnedByOtherPlayer = true;
            }
            return EntityJoined;
        }

        public bool Update(Packets.Packet_05_EntityJoined EntityJoined)
        {
            Type = EntityJoined.ObjectType;
            ID = EntityJoined.ID;
            OwnerName = EntityJoined.OwnerName;
            //Timestamp = (float)(DateTime.Now - Server.Info.TimeStarted).TotalSeconds;
            Prev_PosX = EntityJoined.PosX;
            Prev_PosY = EntityJoined.PosY;
            Prev_PosZ = EntityJoined.PosZ;
            PosX = EntityJoined.PosX;
            PosY = EntityJoined.PosY;
            PosZ = EntityJoined.PosZ;
            HdgX = (short)(EntityJoined.RotX / Math.PI * 32767);
            HdgY = (short)(EntityJoined.RotY / Math.PI * 32767);
            HdgZ = (short)(EntityJoined.RotZ / Math.PI * 32767);
            LastUpdated = DateTime.Now;
            Identify = EntityJoined.Identify;
            IFF = EntityJoined.IFF;
            return true;
        }

        public bool Update(Packets.Packet_11_FlightData FlightData)
        {
            //Debug.WriteLine(TimeStamp + ", " + FlightData.TimeStamp);
            if (TimeStamp <= FlightData.TimeStamp && ID == FlightData.ID)
            {
                Prev_TimeStamp = TimeStamp;
                TimeStamp = FlightData.TimeStamp;
                Prev_PosX = PosX;
                Prev_PosY = PosY;
                Prev_PosZ = PosZ;
                PosX = FlightData.PosX;
                PosY = FlightData.PosY;
                PosZ = FlightData.PosZ;
                HdgX = FlightData.HdgX;
                HdgY = FlightData.HdgY;
                HdgZ = FlightData.HdgZ;
                V_PosX = FlightData.V_PosX;
                V_PosY = FlightData.V_PosY;
                V_PosZ = FlightData.V_PosZ;
                V_HdgX = FlightData.V_HdgX;
                V_HdgY = FlightData.V_HdgY;
                V_HdgZ = FlightData.V_HdgZ;
                Weight_Fuel = FlightData.Weight_Fuel;
                Weight_SmokeOil = FlightData.Weight_SmokeOil;
                Anim_Gear = (byte)FlightData.Anim_Gear;
                Anim_Throttle = (byte)FlightData.Anim_Throttle;
                Anim_Boards = (byte)FlightData.Anim_Boards;
                Anim_VGW = (byte)FlightData.Anim_VGW;
                Anim_BombBay = (byte)FlightData.Anim_BombBay;
                Anim_Nozzle = (byte)FlightData.Anim_Nozzle;
                _Anim_Flags = (byte)FlightData._Anim_Flags;
                Strength = FlightData.Strength;

                LastUpdated = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool Update(Packets.Packet_64_11_FormationData FormationData)
        {
            if (TimeStamp < FormationData.TimeStamp && ID == FormationData.SenderID)
            {
                if (Vehicles.List.ToArray().Where(x => x.ID == FormationData.TargetID).Count() <= 0) return false;
                Vehicle TargetVehicle = Vehicles.List.ToArray().Where(x => x.ID == FormationData.TargetID).ToArray()[0];
                Prev_TimeStamp = TimeStamp;
                TimeStamp = FormationData.TimeStamp;
                Prev_PosX = PosX;
                Prev_PosY = PosY;
                Prev_PosZ = PosZ;
                PosX = TargetVehicle.PosX + FormationData.PosX;
                PosY = TargetVehicle.PosY + FormationData.PosY;
                PosZ = TargetVehicle.PosZ + FormationData.PosZ;
                HdgX = FormationData.HdgX;
                HdgY = FormationData.HdgY;
                HdgZ = FormationData.HdgZ;
                V_HdgX = FormationData.V_HdgX;
                V_HdgY = FormationData.V_HdgY;
                V_HdgZ = FormationData.V_HdgZ;
                LastUpdated = DateTime.Now;
                return true;
            }
            return false;
        }

        public Packets.Packet_11_FlightData GetFlightData()
        {
            Packets.Packet_11_FlightData Output = new Packets.Packet_11_FlightData(3);
            Output.TimeStamp = TimeStamp;
            Output.ID = ID;
            Output.PosX = PosX;
            Output.PosY = PosY;
            Output.PosZ = PosZ;
            Output.HdgX = (short)HdgX;
            Output.HdgY = (short)HdgY;
            Output.HdgZ = (short)HdgZ;
            Output.V_PosX = V_PosX;
            Output.V_PosY = V_PosY;
            Output.V_PosZ = V_PosZ;
            Output.Weight_Fuel = Weight_Fuel;
            Output.Weight_SmokeOil = (short)Weight_SmokeOil;
            Output.Anim_Gear = Anim_Gear;
            Output.Anim_Throttle = Anim_Throttle;
            Output.Anim_Boards = Anim_Boards;
            Output.Anim_VGW = Anim_VGW;
            Output.Anim_BombBay = Anim_BombBay;
            Output.Anim_Nozzle = Anim_Nozzle;
            Output._Anim_Flags = _Anim_Flags;

            return Output;
        }

        public Packets.Packet_21_GroundData GetGroundData()
        {
            Packets.Packet_21_GroundData Output = new Packets.Packet_21_GroundData(1);
            Output.TimeStamp = TimeStamp;
            Output.ID = ID;
            Output.Strength = Strength;
            Output.PosX = PosX;
            Output.PosY = PosY;
            Output.PosZ = PosZ;
            Output.HdgX = HdgX;
            Output.HdgY = HdgY;
            Output.HdgZ = HdgZ;
            Output.V_PosX = V_PosX;
            Output.V_PosY = V_PosY;
            Output.V_PosZ = V_PosZ;
            return Output;
        }

        public override string ToString()
        {
            return "[" + ID + "]" + Identify + ", " + OwnerName;
        }
    }
}
