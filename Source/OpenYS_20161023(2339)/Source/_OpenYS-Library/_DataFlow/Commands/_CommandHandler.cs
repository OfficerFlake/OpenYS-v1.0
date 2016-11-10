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
            public delegate bool CommandHandler(Client source, CommandReader cmd); //return true if command worked, false if it failed.
            public static List<CommandDescriptor> List = new List<CommandDescriptor>();

            #region CommandDescriptor
            public class CommandDescriptor
            {
                public string _Name = "";
                public double _Version = 0;
                public DateTime _Date = new DateTime();
                public string _Author = "";
                public string _Descrption = "";
                public string _Usage = "";
                public bool _Hidden = false;
                public bool _Disabled = false;
                public string _Category = "";
                public Requirement _Requirements = Requirement._Null;
                public string[] _Commands = {}; //multiple commands, supporting aliases.
                public CommandHandler _Handler = delegate { return false; };

                public bool _PreProcess(Client ThisClient, CommandReader Command)
                {
                    //Verify the caller meets all the requirements to run the command!
                    //If the client does NOT meet requirements, tell the client it does not meet the requirements and then reject the command!.
                    #region Build_Client
#if !CLIENT
                    if ((_Requirements & Requirement.Build_Client) > 0)
                    {
                        ThisClient.SendMessage(
                            "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +
                            
                            "can only be used on OpenYS Clients."
                            
                            );
                        return false;
                    }
#endif
                    #endregion
                    #region Build_Server
#if CLIENT
                    if ((_Requirements & Requirement.Build_Server) > 0)
                    {
                        ThisClient.SendMessage(
                            "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                            "can only be used on OpenYS Servers."

                            );
                        return false;
                    }
#endif
                    #endregion
                    #region Build_Release
#if !RELEASE
                    if ((_Requirements & Requirement.Build_Release) > 0)
                    {
                        ThisClient.SendMessage(
                            "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                            "can only be used on OpenYS Release Builds."

                            );
                        return false;
                    }
#endif
                    #endregion
                    #region Build_Debug
#if !DEBUG
                    if ((_Requirements & Requirement.Build_Debug) > 0)
                    {
                        ThisClient.SendMessage(
                            "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                            "can only be used on OpenYS Debug Builds."

                            );
                        return false;
                    }
#endif
                    #endregion
                    #region Permission_OP
                    if (!ThisClient.IsOP())
                    {
                        if ((_Requirements & Requirement.Permission_OP) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used by server owners."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region User_Virtual
                    if (!ThisClient.IsFakeClient())
                    {
                        if ((_Requirements & Requirement.User_Virtual) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used by Virtual Clients."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region User_Console
                    if (!ThisClient.IsConsole())
                    {
                        if ((_Requirements & Requirement.User_Console) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used by OpenYS Consoles."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region User_Background
                    if (!ThisClient.IsController())
                    {
                        if ((_Requirements & Requirement.User_Background) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used by OpenYS Background Processes."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region User_YSFlight
                    if (!ThisClient.IsYSFlightClient())
                    {
                        if ((_Requirements & Requirement.User_YSFlight) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used by YSFlight Clients."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region Protocal_OpenYS
                    if (!ThisClient.YSFClient.OpenYSSupport)
                    {
                        if ((_Requirements & Requirement.Protocal_OpenYS) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can not be used by Clients not supporting the OpenYS Extended Packet Protocal."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region Protocal_YSFlight
                    if (!true) //ALL clients will support the YSF Protocal!
                    {
                        /*
                        if ((_Requirements & Requirement.Protocal_YSFlight) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can not be used by Clients not supporting the YSFlight Vanilla Protocal."

                                );
                            return false;
                        }
                        */
                    }
                    #endregion
                    #region Status_LoggingIn
                    if (!ThisClient.IsLoggingIn())
                    {
                        if ((_Requirements & Requirement.Status_Connecting) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used while logging in."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region Status_LoggedIn
                    if (!ThisClient.IsLoggedIn())
                    {
                        if ((_Requirements & Requirement.Status_Connected) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used after completely logging in."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region Status_Flying
                    if (ThisClient.Vehicle == Vehicles.NoVehicle)
                    {
                        if ((_Requirements & Requirement.Status_Flying) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can only be used when flying."

                                );
                            return false;
                        }
                    }
                    #endregion
                    #region Status_NotFlying
                    if (ThisClient.Vehicle != Vehicles.NoVehicle)
                    {
                        if ((_Requirements & Requirement.Status_NotFlying) > 0)
                        {
                            ThisClient.SendMessage(
                                "&e\"" + Command._CmdComplete.Split(' ')[0] + "\" " +

                                "can not be used when flying."

                                );
                            return false;
                        }
                    }
                    #endregion

                    //All Good! Run The Command!
                    _Handler(ThisClient, Command);
                    return true;
                }
            }
            #endregion

            #region DebuggingCommandDescriptor
            public class DebuggingCommandDescriptor : CommandDescriptor
            {
            }
            #endregion

            #region Requirements
            [Flags]
            public enum Requirement
            {
                _EndList =                  1 << 00, //No effect on the final outcome - used to end the context list only.
                _Null =                     1 << 00, //No effect on the final outcome.
                Build_Client =              1 << 01, //Requires OpenYS Client.
                Build_Server =              1 << 02, //Requires OpenYS OpenYS. 
                Build_Release =             1 << 03, //Requires OpenYS is built in Release mode.
                Build_Debug =               1 << 04, //Requires OpenYS is built in Debug mode.
                Permission_OP =      1 << 05, //Requires Console to grant the user OP status.
                User_Console =              1 << 06, //Requires OpenYS Console.
                User_Background =           1 << 07, //Requires OpenYS Background Processor.
                User_Virtual =              1 << 08, //Requires A Fake Client to Use.
                User_YSFlight =             1 << 09, //Requires YSFlight Client.
                Protocal_OpenYS =           1 << 10, //Requires OpenYS Network Protocal.
                Protocal_YSFlight =         1 << 11, //Requires YSF Network Protocal.
                Status_Connecting =         1 << 12, //Requires logging in.
                Status_Connected =          1 << 13, //Requires logged in.
                Status_Flying =             1 << 14, //Requires flying.
                Status_NotFlying =          1 << 15, //Requires NOT flying.
            }
            #endregion

            public static string[] CmdWildCards = { "*" };
            public static string[] CmdAliasesReserver = { "/Help" };

            public static bool Register(CommandDescriptor ThisCmdDescriptor)
            {
                //This is called WHEN initialised, NOT automatically! hooray for static class huh? :/
                //if (_Name == null | _Version == 0 | _Date == null | _Author == null | _Descrption == null | _Usage == null | _Commands == null | _Handler == null)
                //{
                //    return false;
                //}
                foreach (var ThisFieldVar in ThisCmdDescriptor.GetType().GetFields(System.Reflection.BindingFlags.Public).ToArray())
                {
                    if (ThisFieldVar.GetValue(ThisCmdDescriptor) == null)
                    {
                        Console.WriteLine("&cERROR: &eFailure to register command: \"" + ThisCmdDescriptor._Name + "\". The Field " + ThisFieldVar.Name + " is left Null.");
                        return false;
                    }
                }
                if (ThisCmdDescriptor._Disabled) return false;
                foreach (string ThisString in ThisCmdDescriptor._Commands.ToArray())
                {
                    if (ThisString.StartsWith("//") | ThisString.StartsWith("@") | ThisString.StartsWith("@@") | ThisString.EndsWith("."))
                    {
                        Console.WriteLine("&cERROR: &eFailure to register command: \"" + ThisCmdDescriptor._Name + "\". There is an alias that uses illegal characters, that would conflict with the way OpenYS handles user chat (for example, stating with \"@\").");
                        return false;
                    }
                    if (!ThisString.StartsWith("/"))
                    {
                        Console.WriteLine("&cERROR: &eFailure to register command: \"" + ThisCmdDescriptor._Name + "\". There is an alias that does not start with the Command designator character: \"/\".");
                        return false;
                    }
                }

                if (Commands.List.Select(x => x._Name).Contains(ThisCmdDescriptor._Name))
                {
                    foreach (double ThisDouble in Commands.List.Where(x => x._Name == ThisCmdDescriptor._Name).Select(y => y._Version).ToArray())
                    {
                        if (ThisDouble > ThisCmdDescriptor._Version)
                        {
                            Console.WriteLine("&cERROR: &eFailure to register command: \"" + ThisCmdDescriptor._Name + "\". There is already a command by this name and it is of a newer version.");
                            return false;
                        }
                    }
                    //Console.WriteLine("&aSucessfully updated registered command: \"" + ThisCmdDescriptor._Name + "\".");
                }
                //Console.WriteLine("&eSucessfully registered command: \"" + ThisCmdDescriptor._Name + "\".");
                Commands.List.RemoveAll(x => x._Name == ThisCmdDescriptor._Name);
                Commands.List.Add(ThisCmdDescriptor);
                return true;
            }

            public static List<CommandDescriptor> FindCommand(string Cmd)
            {
                List<Commands.CommandDescriptor> MatchingCommands = new List<Commands.CommandDescriptor>();
                #region GetMatchingCommands
                foreach (Commands.CommandDescriptor ThisCmd in Commands.List.ToArray())
                {
                    //get all the command descriptors, and find matches one by one.
                    foreach (string CmdString in ThisCmd._Commands.ToArray())
                    {
                        //get all the aliases for this indiviual cmd, and check if it's length == length of command typed.
                        if (CmdString.Split('.').Count() != Cmd.Split('.').Count()) continue;
                        else
                        {
                            //iterate over each cmd part, find matches.
                            //USE THE "FOR" LUKE
                            bool CmdMatches = true;
                            for (int i = 0; i <= (CmdString.Split('.').Count() - 1); i++)
                            {
                                if (!(CmdString.Split('.')[i].ToUpperInvariant() == Cmd.Split('.')[i].ToUpperInvariant() || CmdWildCards.Contains(CmdString.Split('.')[i].ToUpperInvariant())))
                                {
                                    CmdMatches = false;
                                    break;
                                }
                            }
                            if (CmdMatches)
                            {
                                if (!(MatchingCommands.Contains(ThisCmd))) MatchingCommands.Add(ThisCmd);
                            }
                        }
                    }
                }
                #endregion

                return MatchingCommands;
            }

            #region CommandReader
            public class CommandReader
            {
                public string _CmdComplete = "";
                public string _CmdString
                {
                    get
                    {
                        //Get the first part of the command string...
                        return _CmdComplete.Split(new string[] { " " }, 2, StringSplitOptions.None)[0];
                    }
                    set
                    {
                        if (_CmdComplete.Contains(' '))
                        {
                            _CmdComplete = value + " " + _CmdComplete.Split(new string[] { " " }, 2, StringSplitOptions.None)[1];
                        }
                        else
                        {
                            _CmdComplete = value;
                        }
                    }
                }
                public string _CmdRawArguments
                {
                    get
                    {
                        if (_CmdComplete.Contains(' '))
                        {
                            return _CmdComplete.Split(new string[] { " " }, 2, StringSplitOptions.None)[1];
                        }
                        else
                        {
                            return null;
                        }
                    }
                    set
                    {
                        _CmdComplete = _CmdComplete.Split(new string[] { " " }, 2, StringSplitOptions.None)[0] + " " + value;
                    }
                }
                public string[] _CmdArguments
                {
                    get
                    {
                        if (_CmdRawArguments != null)
                        {
                            return _CmdRawArguments.Split(' ');
                        }
                        else return new string[]{};
                    }
                }

                public int _CmdPosition = 0;
                public string _CmdCurrent()
                {
                    if ((_CmdString.Replace(',', '.').Split('.').Count() - 1) < _CmdPosition || _CmdPosition < 0)
                    {
                        return "NULL";
                    }
                    else
                    {
                        return _CmdString.Replace(',', '.').Split('.')[_CmdPosition];
                    }
                }
                public string _CmdPrev()
                {
                    if ((_CmdString.Replace(',', '.').Split('.').Count() - 2) < _CmdPosition - 1 || _CmdPosition - 1 < 0)
                    {
                        return "NULL";
                    }
                    else
                    {
                        return _CmdString.Replace(',', '.').Split('.')[_CmdPosition - 1];
                    }
                }
                public string _CmdNext()
                {
                    if ((_CmdString.Replace(',', '.').Split('.').Count()) < _CmdPosition + 1 || _CmdPosition + 1 < 0)
                    {
                        return "NULL";
                    }
                    else
                    {
                        return _CmdString.Replace(',', '.').Split('.')[_CmdPosition + 1];
                    }
                }
                public string[] _CmdElements()
                {
                    return _CmdString.Remove(0, 1).Replace(',', '.').Split('.');
                }

                public void _CmdGotoPrev()
                {
                    if ((_CmdString.Replace(',', '.').Split('.').Count() - 2) < _CmdPosition - 1 || _CmdPosition - 1 < 0)
                    {
                        //Do Nothing
                    }
                    else
                    {
                        _CmdPosition--;
                    }
                }
                public void _CmdGotoNext()
                {
                    if ((_CmdString.Replace(',', '.').Split('.').Count()) < _CmdPosition + 1 || _CmdPosition + 1 < 0)
                    {
                        //Do Nothing
                    }
                    else
                    {
                        _CmdPosition++;
                    }
                }

                public CommandReader(string RawCommand)
                {
                    _CmdComplete = RawCommand;
                    if (RawCommand.Split(new string[] { " " }, 2, StringSplitOptions.None).Count() < 2)
                    {
                        RawCommand += " ";
                    }
                }
            }
            #endregion
        }

        public static partial class CommandManager
        {
            public static void ProcessConsole(object Input, params object[] Args)
            {
                string Formatted = "";
                if (Args == null) Args = new object[0];
                if (Args.Length > 0)
                {
                    try
                    {
                        Formatted = String.Format(Input.ToString(), Args);
                    }
                    catch (Exception e)
                    {
                        Formatted = e.StackTrace + "\n" + Input.ToString();
                    }
                }
                ProcessConsole(Formatted);
            }
            public static void ProcessConsole(string RawCommand)
            {
                #region TRY:
#if RELEASE
                try
                {
#endif
                            #endregion
                    Process(OpenYS.OpenYSConsole, RawCommand);
                #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    OpenYS.OpenYSConsole.BugReport(e);
                    Log.Error(e);
                }
#endif
                #endregion
            }
            public static void ProcessScheduler(object Input, params object[] Args)
            {
                string Formatted = "";
                if (Args == null) Args = new object[0];
                if (Args.Length > 0)
                {
                    try
                    {
                        Formatted = String.Format(Input.ToString(), Args);
                    }
                    catch (Exception e)
                    {
                        Formatted = e.StackTrace + "\n" + Input.ToString();
                    }
                }
                ProcessScheduler(Formatted);
            }
            public static void ProcessScheduler(string RawCommand)
            {
                #region TRY:
#if RELEASE
                try
                {
#endif
                #endregion
                    Process(OpenYS.OpenYSBackGround, RawCommand);
                #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    OpenYS.OpenYSBackGround.BugReport(e);
                    Log.Error(e);
                }
#endif
                #endregion
            }
            public static void Process(Client ThisClient, object Input, params object[] Args)
            {
                string Formatted = "";
                if (Args == null) Args = new object[0];
                if (Args.Length > 0)
                {
                    try
                    {
                        Formatted = String.Format(Input.ToString(), Args);
                    }
                    catch (Exception e)
                    {
                        Formatted = e.StackTrace + "\n" + Input.ToString();
                    }
                }
                ProcessConsole(ThisClient, Formatted);
            }
            public static void Process(Client ThisClient, string RawCommand)
            {
                #region TRY:
#if RELEASE
                try
                {
#endif
                #endregion
                    if (RawCommand == "/")
                    {
                        //Repeat Last Command.
                        RawCommand = ThisClient.MessagesTyped.Last().Message;
                    }

                    Commands.CommandReader Cmd = new Commands.CommandReader(RawCommand);

                    #region Private Messaging
                    /*
                    if (Cmd._CmdString.ToUpperInvariant().StartsWith("@@"))
                    {
                        Cmd._CmdString = Cmd._CmdString.Remove(0, 1);
                        Messaging.GroupChatMessage(ThisClient, Cmd);
                        return;
                    }
                    if (Cmd._CmdString.ToUpperInvariant().StartsWith("@"))
                    {
                        Cmd._CmdString = Cmd._CmdString.Remove(0, 1);
                        Messaging.PrivateChatMessage(ThisClient, Cmd);
                        return;
                    }
                    */
                    #endregion
                    #region // (Escape-Key'd Commands)
                    if (Cmd._CmdString.ToUpperInvariant().StartsWith("//"))
                    {
                        Cmd._CmdComplete = Cmd._CmdComplete.Remove(0, 1);
                        Clients.AllClients.SendMessage("&2(" + ThisClient.Username + ")&f" + Cmd._CmdComplete);
                        //Messaging.StandardChatMessage(ThisClient, Cmd);
                        return;
                    }
                    #endregion
                    #region /Help
                    if (Cmd._CmdString.ToUpperInvariant() == "/HELP")
                    {
                        #region "/Help"
                        if (Cmd._CmdArguments.Count() < 1)
                        {
                            //show help usage
                            ThisClient.SendMessage("&eWelcome To OpenYS!");
                            ThisClient.SendMessage("&e    For more information on commands available, type \"&a/Help Commands&e\".");
                            return;
                        }
                        #endregion
                        #region "/Help /..."
                        if (Cmd._CmdArguments[0].StartsWith("/"))
                        {
                            Cmd._CmdRawArguments = Cmd._CmdRawArguments.Substring(1, Cmd._CmdRawArguments.Length - 1);
                        }
                        if (Cmd._CmdArguments[0].ToUpperInvariant() == "COMMANDS")
                        {
                            ThisClient.SendMessage("&eUsage: &a" + "/Commands [Category]");
                            ThisClient.SendMessage("&e" + "    " + "Lists the Command categories available\n\n    If a category is specified, lists the commands in that category.");
                            ThisClient.SendMessage("&e" + "    Aliases: " + "&cNone.");
                            return;
                        }
                        List<Commands.CommandDescriptor> MatchingCommands = Commands.FindCommand("/" + Cmd._CmdArguments[0]);
                        if (MatchingCommands.Count() == 0)
                        {
                            if (OpenYS.BuildType == OpenYS._BuildType.Server)
                            {
                                ThisClient.SendMessage("&cCommand not found: \"" + Cmd._CmdArguments[0] + "\".");
                                return;
                            }
                            //ThisClient.YSFServer.SendMessage("&2(" + ThisClient.Username + ")&f" + Cmd._CmdComplete);
                            return;
                        }
                        if (MatchingCommands.OneMatchingCommand())
                        {
                            ThisClient.SendMessage("&eUsage: &a" + MatchingCommands[0]._Usage);
                            ThisClient.SendMessage("&e" + "    " + MatchingCommands[0]._Descrption);
                            ThisClient.SendMessage("&e" + "    Aliases: " + MatchingCommands[0]._Commands.Take(3).ToList().ToStringList());
                        }
                        else
                        {
                            ThisClient.SendMessage("&cERROR: &eMore then one matching command found: \"" + Cmd._CmdArguments[0] + "\".");
                        }
                        return;
                        #endregion
                    }
                    #endregion
                    #region /Commands
                    if (Cmd._CmdString.ToUpperInvariant() == "/COMMANDS")
                    {
                        #region "/Commands"
                        if (Cmd._CmdArguments.Count() < 1)
                        {
                            //show help usage
                            List<string> Categories = new List<string>();
                            foreach (Commands.CommandDescriptor ThisCommand in Commands.List)
                            {
                                if (ThisCommand._Category == null) continue;
                                if (ThisCommand._Hidden) continue;
                                if (Categories.Contains(ThisCommand._Category.ToTitleCaseInvariant())) continue;
                                Categories.Add(ThisCommand._Category.ToTitleCaseInvariant());
                            }
                            Categories.Sort((x, y) => string.Compare(x, y));
                            string output = "";
                            foreach (string ThisString in Categories)
                            {
                                string Category = ThisString;
                                if (ThisString.Length > 32) Category = ThisString.Substring(0,32);

                                output += "\n";
                                output += "    &a" + Category + Strings.Repeat(" ", 32 - Category.Length);
                            }
                            if (output == "") output = "\n    &cNo commands categories found.";
                            ThisClient.SendMessage("&eCommand Categories: " + output);
                            return;
                        }
                        #endregion
                        #region "/Commands ..."
                        List<Commands.CommandDescriptor> MatchingCommands = Commands.List.Where(x => x._Category.ToTitleCaseInvariant().Contains(Cmd._CmdArguments[0].ToTitleCaseInvariant())).ToList();
                        if (MatchingCommands.Count() == 0)
                        {
                            if (OpenYS.BuildType == OpenYS._BuildType.Server)
                            {
                                ThisClient.SendMessage("&cCommand Category not found: \"" + Cmd._CmdArguments[0] + "\".");
                                return;
                            }
                            //ThisClient.YSFServer.SendMessage("&2(" + ThisClient.Username + ")&f" + Cmd._CmdComplete);
                            return;
                        }
                        else
                        {
                            string output = "";
                            MatchingCommands.Sort((x, y) => string.Compare(x._Name, y._Name));
                            foreach (Commands.CommandDescriptor ThisCmd in MatchingCommands)
                            {
                                if (ThisCmd._Hidden) continue;

                                string Name = ThisCmd._Name;
                                if (Name.Length > 24) Name = ThisCmd._Name.Substring(0, 21) + "...";

                                output += "\n";
                                output += "    &a" + Name + Strings.Repeat(" ", 24 - Name.Length);

                                string Description = ThisCmd._Descrption;
                                if (Description.Length > (System.Console.WindowWidth - 24 - 4 - 1 - 3 - 1)) Description = ThisCmd._Descrption.Substring(0, (System.Console.WindowWidth - 24 - 4 - 1 - 3 - 1)) + "...";

                                output += " &e" + Description + Strings.Repeat(" ", (System.Console.WindowWidth - 24 - 4 - 1 - 1) - Description.Length);
                            }
                            if (output == "") output = "\n    &cNo commands found.";
                            ThisClient.SendMessage("&e" + MatchingCommands[0]._Category + " Commands: " + output);
                            return;
                        }
                        #endregion
                    }
                    #endregion
                    #region /...
                    if (Cmd._CmdString.ToUpperInvariant().StartsWith("/"))
                    {
                        List<Commands.CommandDescriptor> MatchingCommands = Commands.FindCommand(Cmd._CmdString);
                        if (MatchingCommands.Count() == 0)
                        {
                            if (OpenYS.BuildType == OpenYS._BuildType.Server)
                            {
                                ThisClient.SendMessage("&cCommand not found: \"" + Cmd._CmdComplete.Split(' ')[0] + "\".");
                                return;
                            }
                            //ThisClient.YSFServer.SendMessage("&2(" + ThisClient.Username + ")&f" + Cmd._CmdComplete);
                            return;
                        }
                        if (MatchingCommands.OneMatchingCommand())
                        {
                            MatchingCommands[0]._PreProcess(ThisClient, Cmd);
                        }
                        else
                        {
                            ThisClient.SendMessage("&cERROR: &eMore then one matching command found: \"" + Cmd._CmdString + "\".");
                        }
                        return;
                    }
                    #endregion
                    #region "BLAH BLAH BLAH"
                    if (OpenYS.BuildType == OpenYS._BuildType.Server)
                    {
                        if (ThisClient.IsOP())
                        {
                            Clients.AllClients.SendMessage("&c(&6" + ThisClient.Username + "&c)&f" + Cmd._CmdComplete);
                            Log.Chat("&c(&6" + ThisClient.Username + "&c)&f" + Cmd._CmdComplete);
                        }
                        else
                        {
                            Clients.AllClients.SendMessage("&2(" + ThisClient.Username + "&2)&f" + Cmd._CmdComplete);
                            Log.Chat("&2(" + ThisClient.Username + "&2)&f" + Cmd._CmdComplete);
                        }
                    }
                    else
                    {
                        //ThisClient.YSFServer.SendMessage("(" + ThisClient.Username + ")" + Cmd._CmdComplete);
                    }
                    ////ThisClient.YSFServer.SendMessage("&2(" + ThisClient.Username + ")&f" + Cmd._CmdComplete);
                    return;
                    #endregion
                #region CATCH
#if RELEASE
                }
                catch (Exception e)
                {
                    ThisClient.BugReport(e);
                    Log.Error(e);
                }
#endif
                #endregion
            }


            public static bool OneMatchingCommand(this List<Commands.CommandDescriptor> MatchingCommands)
            {
                if (MatchingCommands.Count() < 1)
                {
                    return false;
                }
                if (MatchingCommands.Count() == 1)
                {
                    return true;
                }
                if (MatchingCommands.Count() > 1)
                {
                    return false;
                }
                return false;
            }
        }
}