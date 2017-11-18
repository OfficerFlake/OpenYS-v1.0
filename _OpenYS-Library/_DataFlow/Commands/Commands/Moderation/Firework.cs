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
        public static readonly CommandDescriptor OpenYS_Command_Particles_Firework = new CommandDescriptor
        {
            _Name = "FireWork",
            _Version = 1.0,
            _Date = new DateTime(2015, 09, 03),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = false,

            _Descrption = "Launches a Firework at the specified ground position, that will reach the specified height.",
            _Usage = "/FireWork ([X] [Z] [H])",
            _Commands = new string[] { "/FireWork", },

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
            _Handler = OpenYS_Command_Particles_Firework_Method,
        };

        public static bool OpenYS_Command_Particles_Firework_Method(Client ThisClient, CommandReader Command)
        {
            Threads.Add(() =>
            {
                if (Command._CmdArguments.Count() < 4)
                {
                    ThisClient.SendMessage("&eNot Enough Arguments Received. Need GroundX, GroundZ, Height, Size, all in meters!");
                    return;
                }
                bool Failed = false;
                float X;
                float Z;
                float H;
                float S;
                Failed |= !Single.TryParse(Command._CmdArguments[0], out X);
                Failed |= !Single.TryParse(Command._CmdArguments[1], out Z);
                Failed |= !Single.TryParse(Command._CmdArguments[2], out H);
                Failed |= !Single.TryParse(Command._CmdArguments[3], out S);
                if (Failed)
                {
                    ThisClient.SendMessage("&eFormat invalied - use numbers only!");
                    return;
                }
                if (H == 0)
                {
                    ThisClient.SendMessage("&eThat firework would explode on the ground!");
                    return;
                }

                Packets.Packet_20_OrdinanceLaunch Flare = new Packets.Packet_20_OrdinanceLaunch();
                Flare.BurnoutDistance = H;
                Flare.HdgX = 0;
                Flare.HdgY = 90.ToYSRadians();
                Flare.HdgZ = 0;
                Flare.PosX = X;
                Flare.PosY = 0;
                Flare.PosZ = Z;
                Flare.SenderID = 0;
                Flare.SenderType = 1;
                Flare.InitVelocity = H;
                Flare.MaximumDamage = 0;
                Flare.MaximumVelocity = H * 2;
                Flare.OrdinanceType = Packets.Packet_20_OrdinanceLaunch.OrdinanceTypes.FLR;
                Clients.LoggedIn.SendPacket(Flare);

                Thread.Sleep(2000);

                if (S <= 0)
                {
                    //don't want a star burst!
                    return;
                }
                Math3D.Vector3[] Vectors = Math3D.GeneratePlatonicSolidVertecies(2);
                foreach (Math3D.Vector3 Vector in Vectors)
                {
                    Packets.Packet_20_OrdinanceLaunch Firework = new Packets.Packet_20_OrdinanceLaunch();
                    Firework.BurnoutDistance = S/3;
                    Firework.HdgX = (float)Vector.XAngle();
                    Firework.HdgY = (float)Vector.YAngle();
                    Firework.HdgZ = 0;
                    Firework.PosX = X;
                    Firework.PosY = H;
                    Firework.PosZ = Z;
                    Firework.SenderID = 0;
                    Firework.SenderType = 1;
                    Firework.InitVelocity = S / 1.4f;
                    Firework.MaximumDamage = 0;
                    Firework.MaximumVelocity = S/2;
                    Firework.OrdinanceType = Packets.Packet_20_OrdinanceLaunch.OrdinanceTypes.FLR;
                    Clients.LoggedIn.SendPacket(Firework);
                }
            }, "Flare Launch");
            return true;
        }
    }
}