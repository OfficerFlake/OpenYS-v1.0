using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static class MetaData
    {
        #region Aircraft
        public class Aircraft
        {
            public string AircraftPath0Dat;
            public string AircraftPath1Model;
            public string AircraftPath2Collision;
            public string AircraftPath3Cockpit;
            public string AircraftPath4Coarse;

            public string Identify;
            //DO NOT ADD MORE INFO! METADATA IS A CACHE ONLY!
        }

        public static class _Aircraft
        {
            #region NoAircraft
            /// <summary>
            /// Psuedo-Object to represent Null.
            /// </summary>
            public static Aircraft None = new Aircraft() { Identify = "NULL" };
            #endregion
            #region Load All
            /// <summary>
            /// Searches the YSFlightDirectory for the Aircraft Folder, and loads all Aircraft Lists from it.
            /// </summary>
            /// <returns></returns>
            public static bool LoadAll()
            {
                //Invalidate the old aircraft list!
                List.Clear();

                //if the YSFlight Aircraft directory doesn't exist, return false.
                string YSFlightAircraftDirectory = Settings.Loading.YSFlightDirectory + "/Aircraft/";
                if (!Directories.DirectoryExists(YSFlightAircraftDirectory)) return false;
                string[] Filenames = Directories.DirectoryGetFilenames(YSFlightAircraftDirectory);
                string[] AircraftLists = Filenames.Where(x => x.ToUpperInvariant().StartsWith("AIR") && x.ToUpperInvariant().EndsWith(".LST")).ToArray();
                foreach (string AircraftList in AircraftLists)
                {
                    if (!Files.FileExists(YSFlightAircraftDirectory + AircraftList)) return false;
                    string[] AircraftListContents = Files.FileReadAllLines(YSFlightAircraftDirectory + AircraftList);
                    AircraftListContents = AircraftListContents.Where(x => x.ToUpperInvariant().Contains(".DAT")).ToArray();
                    foreach (string Line in AircraftListContents)
                    {
                        string ProcessedLine = Line.ReplaceAll("\\", "/");
                        string[] SplitString = Strings.SplitPreservingQuotes(ProcessedLine, ' ');
                        string AircraftPath0Dat = "";
                        string AircraftPath1Model = "";
                        string AircraftPath2Collision = "";
                        string AircraftPath3Cockpit = "";
                        string AircraftPath4Coarse = "";

                        if (SplitString.Length > 4) SplitString = new string[] { SplitString[0], SplitString[1], SplitString[2], SplitString[3], SplitString[4] };
                        switch (SplitString.Length - 1)
                        {
                            case 4:
                                AircraftPath4Coarse = SplitString[4];
                                goto case 3;
                            case 3:
                                AircraftPath3Cockpit = SplitString[3];
                                goto case 2;
                            case 2:
                                AircraftPath2Collision = SplitString[2];
                                goto case 1;
                            case 1:
                                AircraftPath1Model = SplitString[1];
                                goto case 0;
                            case 0:
                                AircraftPath0Dat = SplitString[0];
                                break;
                        }

                        Aircraft NewMetaAircraft = new Aircraft();
                        NewMetaAircraft.AircraftPath0Dat = AircraftPath0Dat;
                        NewMetaAircraft.AircraftPath1Model = AircraftPath1Model;
                        NewMetaAircraft.AircraftPath2Collision = AircraftPath2Collision;
                        NewMetaAircraft.AircraftPath3Cockpit = AircraftPath3Cockpit;
                        NewMetaAircraft.AircraftPath4Coarse = AircraftPath4Coarse;

                        if (NewMetaAircraft.AircraftPath0Dat.Length < 4) Console.WriteLine(Line);

                        List.Add(NewMetaAircraft);
                    }
                }

                //AT THIS POINT, ALL YSFLIGHT AIRCRAFT LST's ARE FULLY LOADED. NOW WE CACHE THE AIRCRAFT NAMES.

                foreach (Aircraft ThisMetaAircraft in List)
                {
                    if (!Files.FileExists(Settings.Loading.YSFlightDirectory + "/" + ThisMetaAircraft.AircraftPath0Dat))
                    {
                        Console.WriteLine("Failed To Load: " + Settings.Loading.YSFlightDirectory + "/" + ThisMetaAircraft.AircraftPath0Dat);
                        continue; //Couldn't find the aircraft DAT file, we'll leave it blank!
                    }
                    string[] DatFileContents = Files.FileReadAllLines(Settings.Loading.YSFlightDirectory + "/" + ThisMetaAircraft.AircraftPath0Dat);
                    foreach (string DatFileLine in DatFileContents)
                    {
                        #region Identify
                        if (DatFileLine.ToUpperInvariant().Contains("IDENTIFY"))
                        {
                            string AircraftName = Strings.SplitPreservingQuotes(DatFileLine, ' ')[1];
                            AircraftName = AircraftName.ReplaceAll(" ", "_");
                            AircraftName = AircraftName.Split('�')[0];
                            ThisMetaAircraft.Identify = AircraftName.ToUpperInvariant();
                        }
                        #endregion
                    }
                }

                //Cache Complete. All aircraft are loaded, ready for use.
                return true;
            }
            #endregion
            #region Find By Name
            /// <summary>
            /// Finds the desired MetaObject by name. If no meta object is found, NULL is returned.
            /// </summary>
            /// <param name="Name">Aircraft name to search for.</param>
            /// <returns>
            /// Match: Last Matching MetaAircraft Object
            /// Else:  "NoMetaAircraft" Psuedo-Object.
            /// </returns>
            public static Aircraft FindByName(string Name)
            {
                Aircraft Output = None;
                if (Name == null) return Output;

                foreach (Aircraft ThisMetaAircraft in List)
                {
                    if (ThisMetaAircraft == null) continue;
                    if (ThisMetaAircraft.Identify == null) continue;
                    if (ThisMetaAircraft.Identify.ToUpperInvariant().Resize(31) == Name.ToUpperInvariant().Resize(31))
                    {
                        Output = ThisMetaAircraft;
                    }
                }
                if (Output == None)
                {
                    Log.Warning("Failed to find MetaData for aircraft: " + Name + ".");
                }
                return Output;
            }
            #endregion
            #region List
            public static List<Aircraft> List = new List<Aircraft>();
            #endregion
        }
        #endregion

        #region Ground
        public class Ground
        {
            public string GroundPath0Dat;
            public string GroundPath1Model;
            public string GroundPath2Collision;
            public string GroundPath3Cockpit;
            public string GroundPath4Coarse;

            public string Identify;
            //DO NOT ADD MORE INFO! METADATA IS A CACHE ONLY!
        }

        public static class _Ground
        {
            #region NoGround
            /// <summary>
            /// Psuedo-Object to represent Null.
            /// </summary>
            public static Ground None = new Ground() { Identify = "NULL" };
            #endregion
            #region Load All
            /// <summary>
            /// Searches the YSFlightDirectory for the Ground Folder, and loads all Ground Lists from it.
            /// </summary>
            /// <returns>True if Loading OK!</returns>
            public static bool LoadAll()
            {
                //Invalidate the old grounds list!
                List.Clear();

                //if the YSFlight Ground directory doesn't exist, return false.

                string YSFlightGroundDirectory = Settings.Loading.YSFlightDirectory + "/Ground/";
                if (!Directories.DirectoryExists(YSFlightGroundDirectory)) return false;
                string[] Filenames = Directories.DirectoryGetFilenames(YSFlightGroundDirectory);
                string[] GroundLists = Filenames.Where(x => x.ToUpperInvariant().StartsWith("GRO") && x.ToUpperInvariant().EndsWith(".LST")).ToArray();
                foreach (string GroundList in GroundLists)
                {
                    if (!Files.FileExists(YSFlightGroundDirectory + GroundList)) return false;
                    string[] GroundListContents = Files.FileReadAllLines(YSFlightGroundDirectory + GroundList);
                    GroundListContents = GroundListContents.Where(x => x.ToUpperInvariant().Contains(".DAT")).ToArray();
                    foreach (string Line in GroundListContents)
                    {
                        string ProcessedLine = Line.ReplaceAll("\\", "/"); //escape code. actually replacing a single blackslash with a single forwardslash. :)
                        //ProcessedLine = Utilities.StringCompress(ProcessedLine);
                        //string[] SplitString = ProcessedLine.Split(' ');
                        string[] SplitString = Strings.SplitPreservingQuotes(ProcessedLine, ' ');
                        string GroundPath0Dat = "";
                        string GroundPath1Model = "";
                        string GroundPath2Collision = "";
                        string GroundPath3Cockpit = "";
                        string GroundPath4Coarse = "";

                        switch (SplitString.Length - 1)
                        {
                            case 4:
                                GroundPath4Coarse = SplitString[4];
                                goto case 3;
                            case 3:
                                GroundPath3Cockpit = SplitString[3];
                                goto case 2;
                            case 2:
                                GroundPath2Collision = SplitString[2];
                                goto case 1;
                            case 1:
                                GroundPath1Model = SplitString[1];
                                goto case 0;
                            case 0:
                                GroundPath0Dat = SplitString[0];
                                break;
                        }

                        Ground NewMetaGround = new Ground();
                        NewMetaGround.GroundPath0Dat = GroundPath0Dat;
                        NewMetaGround.GroundPath1Model = GroundPath1Model;
                        NewMetaGround.GroundPath2Collision = GroundPath2Collision;
                        NewMetaGround.GroundPath3Cockpit = GroundPath3Cockpit;
                        NewMetaGround.GroundPath4Coarse = GroundPath4Coarse;

                        List.Add(NewMetaGround);
                    }
                }

                //AT THIS POINT, ALL YSFLIGHT Ground LST's ARE FULLY LOADED. NOW WE CACHE THE Ground data.

                foreach (Ground ThisMetaGround in List)
                {
                    if (!Files.FileExists(Settings.Loading.YSFlightDirectory + "/" + ThisMetaGround.GroundPath0Dat)) continue; //Couldn't find the Ground DAT file, we'll leave it blank!
                    string[] DatFileContents = Files.FileReadAllLines(Settings.Loading.YSFlightDirectory + "/" + ThisMetaGround.GroundPath0Dat);
                    foreach (string DatFileLine in DatFileContents)
                    {
                        if (DatFileLine.ToUpperInvariant().Contains("IDENTIFY"))
                        {
                            string GroundName = Strings.SplitPreservingQuotes(DatFileLine, ' ')[1];
                            ThisMetaGround.Identify = GroundName;
                        }
                    }
                }

                //Cache Complete. All Ground are loaded, ready for use.
                return true;
            }
            #endregion
            #region Find By Name
            /// <summary>
            /// Finds the desired MetaObject by name. If no meta object is found, NULL is returned.
            /// </summary>
            /// <param name="Name">Ground name to search for.</param>
            /// <returns>
            /// Match: Last Matching MetaGround Object
            /// Else:  "NoMetaGround" Psuedo-Object.
            /// </returns>
            public static Ground FindByName(string Name)
            {
                Ground Output = None;
                if (Name == null) return Output;
                foreach (Ground ThisMetaGround in List)
                {
                    if (ThisMetaGround == null) continue;
                    if (ThisMetaGround.Identify == null) continue;
                    if (ThisMetaGround.Identify.ToUpperInvariant() == Name.ToUpperInvariant())
                    {
                        Output = ThisMetaGround;
                    }
                }
                if (Output == None)
                {
                    Log.Warning("Failed to find MetaData for ground: " + Name + ".");
                }
                return Output;
            }
            #endregion
            #region List
            public static List<Ground> List = new List<Ground>();
            #endregion
        }
        #endregion

        #region Scenery
        public class Scenery
        {
            public string SceneryPath1Fld;
            public string SceneryPath2Stp;
            public string SceneryPath3Yfs;

            public string Identify;
            //DO NOT ADD MORE INFO! METADATA IS A CACHE ONLY!
        }

        public static class _Scenery
        {
            #region NoScenery
            /// <summary>
            /// Psuedo-Object to represent Null.
            /// </summary>
            public static Scenery None = new Scenery() { Identify = "" };
            #endregion
            #region Load All
            /// <summary>
            /// Searches the YSFlightDirectory for the Scenery Folder, and loads all Scenery Lists from it.
            /// </summary>
            /// <returns>True if Loading OK!</returns>
            public static bool LoadAll()
            {
                //Invalidate the old scenery list!
                List.Clear();


                //if the YSFlight Scenery directory doesn't exist, return false.

                string YSFlightSceneryDirectory = Settings.Loading.YSFlightDirectory + "/Scenery/";
                if (!Directories.DirectoryExists(YSFlightSceneryDirectory)) return false;
                string[] Filenames = Directories.DirectoryGetFilenames(YSFlightSceneryDirectory);
                foreach (string ThisFileName in Filenames)
                {
                }
                string[] SceneryLists = Filenames.Where(x => x.ToUpperInvariant().StartsWith("SCE") && x.ToUpperInvariant().EndsWith(".LST")).ToArray();
                foreach (string ThisFileName in SceneryLists)
                {
                }
                foreach (string SceneryList in SceneryLists)
                {
                    if (!Files.FileExists(YSFlightSceneryDirectory + SceneryList)) return false;
                    string[] SceneryListContents = Files.FileReadAllLines(YSFlightSceneryDirectory + SceneryList);
                    SceneryListContents = SceneryListContents.Where(x => x.ToUpperInvariant().Contains(".FLD")).ToArray();
                    foreach (string Line in SceneryListContents)
                    {
                        string ProcessedLine = Line.ReplaceAll("\\", "/"); //escape code. actually replacing a single backslash with a single forwardslash. :)
                        //ProcessedLine = Utilities.StringCompress(ProcessedLine);
                        //string[] SplitString = ProcessedLine.Split(' ');
                        string[] SplitString = Strings.SplitPreservingQuotes(ProcessedLine, ' ');
                        string Identify = "";
                        string SceneryPath1Fld = "";
                        string SceneryPath2Stp = "";
                        string SceneryPath3Yfs = "";

                        switch (SplitString.Length - 1)
                        {
                            case 3:
                                SceneryPath3Yfs = SplitString[3];
                                goto case 2;
                            case 2:
                                SceneryPath2Stp = SplitString[2];
                                goto case 1;
                            case 1:
                                SceneryPath1Fld = SplitString[1];
                                goto case 0;
                            case 0:
                                Identify = SplitString[0];
                                break;
                        }

                        Scenery NewMetaScenery = new Scenery();
                        NewMetaScenery.Identify = Identify;
                        NewMetaScenery.SceneryPath1Fld = SceneryPath1Fld;
                        NewMetaScenery.SceneryPath2Stp = SceneryPath2Stp;
                        NewMetaScenery.SceneryPath3Yfs = SceneryPath3Yfs;

                        List.Add(NewMetaScenery);
                    }
                }

                //AT THIS POINT, ALL YSFLIGHT SCENERY LST's ARE FULLY LOADED.
                return true;
            }
            #endregion
            #region Find By Name
            /// <summary>
            /// Finds the desired MetaObject by name. If no meta object is found, NULL is returned.
            /// </summary>
            /// <param name="Name">Scenery name to search for.</param>
            /// <returns>
            /// Match: Last Matching MetaScenery Object
            /// Else:  "NoMetaScenery" Psuedo-Object.
            /// </returns>
            public static Scenery FindByName(string Name)
            {
                Scenery Output = None;
                if (Name == null) return Output;

                foreach (Scenery ThisMetaScenery in List)
                {
                    if (ThisMetaScenery == null) continue;
                    if (ThisMetaScenery.Identify == null) continue;
                    if (ThisMetaScenery.Identify.ToUpperInvariant() == Name.ToUpperInvariant())
                    {
                        Output = ThisMetaScenery;
                    }
                }
                if (Output == None)
                {
                    Log.Warning("Failed to find MetaData for scenery: " + Name + ".");
                }
                return Output;
            }
            #endregion
            #region List
            public static List<Scenery> List = new List<Scenery>();
            #endregion
        }
        #endregion
    }
}