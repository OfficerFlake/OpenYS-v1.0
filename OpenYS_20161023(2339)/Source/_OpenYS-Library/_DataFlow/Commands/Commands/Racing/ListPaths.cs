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
        public static readonly CommandDescriptor OpenYS_Command_Racing_ListPaths = new CommandDescriptor
        {
            _Name = "ListRacePaths",
            _Version = 1.0,
            _Date = new DateTime(2015, 12, 23),
            _Author = "OfficerFlake",

            _Category = "Racing",
            _Hidden = false,

            _Descrption = "Lists paths to race on.",
            _Usage = "/ListPaths",
            _Commands = new string[] { "/ListPaths", "/ListRacePaths" },

            #region Requirements
            _Requirements =
            //Requirement.Build_Client       |
            //Requirement.Build_Server       |
            //Requirement.Build_Release      |
            //Requirement.Build_Debug        |
            //Requirement.User_Console       |
            //Requirement.User_YSFlight |
            //Requirement.Protocal_OpenYS    |
            //Requirement.Protocal_YSFlight  |
            //Requirement.Status_Connecting  |
            //Requirement.Status_Connected |
            //Requirement.Status_Flying |
            //Requirement.Status_NotFlying   |
            Requirement._EndList,
            #endregion

            //The method naming format should follow the standard packaging protocal!
            //This is to ensure no methods are overwritten by other users!
            //Please use a Namespace like method!
            //Namespace: <YourName/Repository>_<MethodType>_<MethodName>
            //The Handler should be similar, but end in "_Method"!
            _Handler = OpenYS_Command_Racing_ListPaths_Method,
        };

        public static bool OpenYS_Command_Racing_ListPaths_Method(Client ThisClient, CommandReader Command)
        {
            string[] Paths = World.Objects.PathList.Where(y => y.Type == 15).Select(x=>x.Identify).ToArray();
            string Output = Paths.ToStringList();
            if (Paths.Count() <= 0)
            {
                if (ThisClient.IsOP())
                {
                    ThisClient.SendMessage("No Racing Paths Available.\n\nTo add a path, use Scenery Editor and create an area/motion path with ID 15, and give it a name!");
                }
                else
                {
                    ThisClient.SendMessage("No Racing Paths Available. Ask an admin to set some up for you!");
                }
                return false;
            }
            else
            {
                ThisClient.SendMessage(Paths.Count() + " Racing Paths Available: " + Output);
                return true;
            }
        }
    }
}