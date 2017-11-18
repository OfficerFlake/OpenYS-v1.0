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
        public static readonly CommandDescriptor OpenYS_Command_Schedueling_Sleep = new CommandDescriptor
        {
            _Name = "Sleep",
            _Version = 1.0,
            _Date = new DateTime(2015, 02, 11),
            _Author = "OfficerFlake",

            _Category = "Moderation",
            _Hidden = true,

            _Descrption = "Suspends the calling thread for the specified ammount of milliseconds. Only useful in schedueling agents.",
            _Usage = "/Sleep",
            _Commands = new string[] { "/Sleep", },

            #region Requirements
            _Requirements =
                //Requirement.Build_Client       |
                //Requirement.Build_Server       |
                //Requirement.Build_Release      |
                //Requirement.Build_Debug        |
                //Requirement.Permission_OP |
                Requirement.User_Background |
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
            _Handler = OpenYS_Command_Schedueling_Sleep_Method,
        };

        public static bool OpenYS_Command_Schedueling_Sleep_Method(Client ThisClient, CommandReader Command)
        {
            uint SleepTime = 0;
            if (Command._CmdArguments.Count() < 1)
            {
                if (ThisClient.FormationTarget == 0)
                {
                    ThisClient.SendMessage("&eError - Sleep timer not provided a sleep time.");
                }
                return false;
            }
            if (!UInt32.TryParse(Command._CmdArguments[0], out SleepTime))
            {
                ThisClient.SendMessage("&eError - Sleep time not provided an integer value.");
                return false;
            }
            Thread.Sleep((int)SleepTime);
            return true;
        }
    }
}