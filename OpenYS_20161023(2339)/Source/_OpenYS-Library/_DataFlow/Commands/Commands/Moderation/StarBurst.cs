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
        public static readonly CommandDescriptor OpenYS_Command_Particles_StarBurst = new CommandDescriptor
        {
            _Name = "StarBurst",
            _Version = 1.0,
            _Date = new DateTime(2015, 09, 03),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Creates a Fireworks StarBurst at the specified position, with the given size.",
            _Usage = "/StarBurst [X] [Y] [Z] [S]",
            _Commands = new string[] { "/StarBurst", },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                Requirement.Permission_OP |
                //Requirement.User_YSFlight      |
                //Requirement.Protocal_OpenYS    |
                //Requirement.Protocal_YSFlight  |
                //Requirement.Status_Connecting  |
                //Requirement.Status_Connected   |
                //Requirement.Status_Flying      |
                //Requirement.Status_NotFlying   |
                Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Particles_StarBurst_Method,
        };

        public static bool OpenYS_Command_Particles_StarBurst_Method(Client ThisClient, CommandReader Command)
        {
            if (Command._CmdArguments.Count() < 4)
            {
                ThisClient.SendMessage("&eNot Enough Arguments Received. Need PositionX, PositionY, PositionZ, Size, all in meters!");
                return false;
            }
            bool Failed = false;
            float X;
            float Y;
            float Z;
            float S;
            Failed |= !Single.TryParse(Command._CmdArguments[0], out X);
            Failed |= !Single.TryParse(Command._CmdArguments[1], out Y);
            Failed |= !Single.TryParse(Command._CmdArguments[2], out Z);
            Failed |= !Single.TryParse(Command._CmdArguments[3], out S);
            if (Failed)
            {
                ThisClient.SendMessage("&eFormat invalied - use numbers only!");
                return false;
            }

            if (S <= 0)
            {
                //don't want a star burst!
                ThisClient.SendMessage("&eA StarBurst of size 0?...");
                return false;
            }
            Math3D.Vector3[] Vectors = Math3D.GeneratePlatonicSolidVertecies(2);
            foreach (Math3D.Vector3 Vector in Vectors)
            {
                Packets.Packet_20_OrdinanceLaunch Firework = new Packets.Packet_20_OrdinanceLaunch();
                Firework.BurnoutDistance = S;
                Firework.HdgX = (float)Vector.XAngle();
                Firework.HdgY = (float)Vector.YAngle();
                Firework.HdgZ = 0;
                Firework.PosX = X;
                Firework.PosY = Y;
                Firework.PosZ = Z;
                Firework.SenderID = 0;
                Firework.SenderType = 1;
                Firework.InitVelocity = S*1.4f;
                Firework.MaximumDamage = 0;
                Firework.MaximumVelocity = S;
                Firework.OrdinanceType = Packets.Packet_20_OrdinanceLaunch.OrdinanceTypes.FLR;
                Clients.LoggedIn.SendPacket(Firework);
            }
            return true;
        }
    }
}