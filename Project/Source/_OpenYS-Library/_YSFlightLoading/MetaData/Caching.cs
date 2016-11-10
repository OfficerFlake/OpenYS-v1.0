using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static class CachedData
    {
        public class __AircraftDetailed
        {
        //    //NOT USED

        //    //This information compiled with the assistance of YSFHQ.
        //    //
        //    //https://forum.ysfhq.com/viewtopic.php?t=1772
        //    //
        //    //

        //    //SET A DEFAULT VALUE, NO "NULL" PLEASE!
        //    string                  IDENTIFY = "NULL";
        //    string                  SUBSTNAM = "NULL";
        //    string                  CATEGORY = "AIRCRAFT";
        //    bool                    AFTBURNR = true;
        //    Numbers.Mass.Tonnes     THRAFTBN = 10.AsTonnes();
        //    Numbers.Mass.Tonnes     THRMILIT = 5.AsTonnes();
        //    double                  THRSTREV = 0.5;
        //    Numbers.Mass.Tonnes     WEIGHCLN = 1.AsTonnes();
        //    Numbers.Mass.Tonnes     WEIGFUEL = 1.AsTonnes();
        //    Numbers.Mass.Tonnes     WEIGLOAD = 1.AsTonnes();
        //    Numbers.Mass.Kilograms  FUELABRN = 4.AsKilograms();
        //    Numbers.Mass.Kilograms  FUELMILI = 1.AsKilograms();
        //    Math3D.Orientation3     COCKPITP = new Math3D.Orientation3(0,1,5,0,0,0);
        //    Dictionary<string, Math3D.Orientation3>
        //                            EXCAMERA = new Dictionary<string, Math3D.Orientation3>();
        //    string                  INSTPANL = "User/Null.IST";
        //    Math3D.Point3           ISPNLPOS = new Math3D.Point3(0,0,0);
        //    double                  ISPNLSCL = 1.0;
        //    Math3D.Attitude3        ISPNLATT = new Math3D.Attitude3(0,0,0);
        //    Math2D.Point2           SCRNCNTR = new Math2D.Point2(0,0);
        //    Math3D.Point3           LEFTGEAR = new Math3D.Point3(-10,-1,-5);
        //    Math3D.Point3           RIGHGEAR = new Math3D.Point3( 10,-1,-5);
        //    Math3D.Point3           WHELGEAR = new Math3D.Point3(  0,-1, 5);
        //    Math3D.Point3           ARRESTER = new Math3D.Point3(  0,-1,-10);

        //    Math3D.Point3           MACHNGUN = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN2 = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN3 = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN4 = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN5 = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN6 = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN7 = new Math3D.Point3(0, 0, 10);
        //    Math3D.Point3           MACHNGN8 = new Math3D.Point3(0, 0, 10);

        //    double                  GUNINTVL = 1.0;
        //    Math3D.Vector3          GUNDIREC = new Math3D.Vector3(0, 0, 1);

        //    Math3D.Point3           SMOKEGEN = new Math3D.Point3(0,0,-10);

        //    Math3D.Point3           VAPORPO0 = new Math3D.Point3(10,0,-10);
        //    Math3D.Point3           VAPORPO1 = new Math3D.Point3(10,0,-10);

        //    double                  HTRADIUS = 10.0;

        //    string                  TRIGGER1 = "SMK";
        //    string                  TRIGGER2 = "FLR";
        //    string                  TRIGGER3 = "GUN";
        //    string                  TRIGGER4 = "AAM";

        //    double                  STRENGTH = 10.0;

        //    double                  CRITAOAP = 30.0;
        //    double                  CRITAOAM = 15.0;

        //    double                  CRITSPED = 0;
        //    double                  MAXSPEED = 0;
        //    bool                    HASSPOIL = true;
        //    bool                    RETRGEAR = true;
        //    bool                    VARGEOMW = true;
        //    double                  CLVARGEO = 1.0;
        //    double                  CDVARGEO = 1.0;
        //    double                  CLBYFLAP = 1.0;
        //    double                  CDBYFLAP = 1.0;
        //    double                  CDBYGEAR = 1.0;
        //    double                  CDSPOILR = 1.0;

        //    bool                    TRSTVCTR = true;
        //    Math3D.Vector3          TRSTDIR0 = new Math3D.Vector3(0, 0, 1);
        //    Math3D.Vector3          TRSTDIR1 = new Math3D.Vector3(0, 1, 0);

        //    double                  PSTMPTCH = 10.0;
        //    double                  PSTMYAW_ = 10.0;
        //    double                  PSTMROLL = 10.0;

        //    double                  WINGAREA = 30.0;

        //    double                  MXIPTAOA = 10.0;
        //    double                  MXIPTSSA = 10.0;
        //    double                  MXIPTROL = 10.0;

        //    double                  MANESPD1 = 0.0;
        //    double                  MANESPD2 = 100.0;

        //    double                  CPITMANE = 10.0;
        //    double                  CPITSTAB = 10.0;
        //    double                  CYAWMANE = 10.0;
        //    double                  CYAWSTAB = 10.0;
        //    double                  CROLLMAN = 10.0;

        //    double                  INITFUEL = 10.0;
        //    double                  INITLOAD = 10.0;
        //    double                  INITSPED = 10.0;

        //    bool                    GUNSIGHT = true;
        //    double                  GUNPOWER = 0;
        //    int                     INITIGUN = 0;

        //    string                  WEAPONCH = "SMK";
        //    double                  SMOKEOIL = 100;

        //    #region WeaponDescriptClass
        //    public class WeaponDescription
        //    {
        //        public string Weapon = "NULL";
        //        public ushort Ammo = 0;

        //        public WeaponDescription(string _Weapon, ushort _Ammo)
        //        {
        //            Weapon = _Weapon;
        //            Ammo = _Ammo;
        //        }
        //    }
        //    #endregion
        //    List<WeaponDescription> WEAPONS_ = new List<WeaponDescription>();

        //    double                  BMBAYRCS = 1;
        //    double                  RADARCRS = 1;

        //    double                  REFVCRUS = 1.AsMachAtSeaLevel();
        //    double                  REFACRUS = 10000.AsFeet();
        //    double                  REFTCRUS = 0.5;

        //    double                  REFVLAND = 100.AsKnots();
        //    double                  REFAOALD = 7;
        //    double                  REFLNRWY = 1000;
        //    double                  REFTHRLD = 0.3;

        //    double                  MAXNMFLR = 60;



        }
        public class Aircraft
        {
            //This information compiled with the assistance of YSFHQ.
            //
            //https://forum.ysfhq.com/viewtopic.php?t=1772
            //
            //

            //SET A DEFAULT VALUE, NO "NULL" PLEASE!

            #region YSF
            public string IDENTIFY = "NULL";
            public string SUBSTNAM = "NULL";
            public Numbers.Mass.Kilograms WEIGHCLN = 0.AsKilograms();
            public Numbers.Mass.Kilograms WEIGFUEL = 0.AsKilograms();
            public Numbers.Mass.Kilograms WEIGLOAD = 0.AsKilograms();
            public Numbers.Mass.Kilograms FUELABRN = 0.AsKilograms();
            public Numbers.Mass.Kilograms FUELMILI = 0.AsKilograms();
            public Math3D.Point3 LEFTGEAR = new Math3D.Point3(-10, -1, -5);
            public Math3D.Point3 RIGHGEAR = new Math3D.Point3(10, -1, -5);
            public Math3D.Point3 WHELGEAR = new Math3D.Point3(0, -1, 5);

            public Numbers.Length.Meters HTRADIUS = 10.0;

            public double STRENGTH = 10.0;

            //public double INITFUEL = 10.0;

            public bool GUNSIGHT = true;
            public double GUNPOWER = 0;
            public int INITIGUN = 0;

            public Numbers.Mass.Kilograms SMOKEOIL = 100;

            public double BMBAYRCS = 1;
            public double RADARCRS = 1;

            public double MAXNMFLR = 60;
            #endregion
            #region OYS
            public string OYS_CARRIER = "NULL";
            #endregion
            #region Methods
            public double GetBaseHeight()
            {
                double MAINGEAR = (LEFTGEAR.Y < RIGHGEAR.Y) ? LEFTGEAR.Y : RIGHGEAR.Y;
                return Math.Abs((WHELGEAR.Y < MAINGEAR) ? WHELGEAR.Y : MAINGEAR);
            }
            #endregion
        }
        public static class _Aircraft
        {
            public static Aircraft None = new Aircraft();
        }
        public static Aircraft Cache(this MetaData.Aircraft ThisAircraft)
        {
            Aircraft Output = CachedData._Aircraft.None;
            #region DAT File Exists?
            if (!Files.FileExists(Settings.Loading.YSFlightDirectory + ThisAircraft.AircraftPath0Dat))
            {
                Log.Warning("Can't find .DAT File: \"" + Settings.Loading.YSFlightDirectory + ThisAircraft.AircraftPath0Dat + "\". Can't Cache!");
                return Output;
            }
            #endregion
            #region Load DAT File
            string[] DatFileContents = Files.FileReadAllLines(Settings.Loading.YSFlightDirectory + ThisAircraft.AircraftPath0Dat);
            List<string[]> SplitLines = new List<string[]>();
            #endregion
            #region Process DAT File into Arguments
            foreach (string ThisLine in DatFileContents)
            {
                string[] ReadyToAdd = ThisLine.SplitPreservingQuotes(' ');
                if (ReadyToAdd.Length < 1) continue;
                if (ReadyToAdd[0].ToUpperInvariant() == "REM")
                {
                    if (ReadyToAdd.Length < 2) continue;
                    if (ReadyToAdd[1].ToUpperInvariant() != "OPENYS") continue;
                    if (ReadyToAdd.Length < 4) continue;
                    ReadyToAdd = ReadyToAdd.Skip(2).ToArray();
                }
                List<string> Out = new List<string>();
                foreach (string ThisString in ReadyToAdd)
                {
                    if (ThisString.StartsWith("#"))
                    {
                        break;
                    }
                    if (ThisString.StartsWith(";"))
                    {
                        break;
                    }
                    Out.Add(ThisString);
                }
                SplitLines.Add(Out.ToArray());
            }
            #endregion
            #region Set Cached Values from Arguments
            Output = new Aircraft();
            foreach (FieldInfo ThisField in typeof(Aircraft).GetFields())
            {
                //Foreach Field in Class...
                foreach (string[] ThisArgumentStack in SplitLines)
                {
                    //compare against each line in dat file...
                    if (ThisArgumentStack[0].ToUpperInvariant() == ThisField.Name)
                    {
                        //If DAT file command = field name...

                        //2 VALUES:
                        if (ThisArgumentStack.Length < 2)
                        {
                            Log.Warning("Bad .DAT Line in .DAT File: " + Settings.Loading.YSFlightDirectory + ThisAircraft.AircraftPath0Dat + ". " + ThisArgumentStack.ToStringList());
                            Debug.WriteLine("Bad .DAT Line - Break here to investigate.");
                        }

                        #region String
                        if (ThisField.FieldType == typeof(string))
                        {
                            ThisField.SetValue(Output, ThisArgumentStack[1]);
                            continue;
                        }
                        #endregion
                        #region Bool
                        if (ThisField.FieldType == typeof(bool))
                        {
                            bool Value = false;
                            bool Failed = !Boolean.TryParse(ThisArgumentStack[1].ToUpperInvariant(), out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            } 
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Int
                        if (ThisField.FieldType == typeof(int))
                        {
                            int Value = 0;
                            bool Failed = !Int32.TryParse(ThisArgumentStack[1].ToUpperInvariant(), out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            } 
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Double
                        if (ThisField.FieldType == typeof(double))
                        {
                            double Value = 0;
                            bool Failed = !Double.TryParse(ThisArgumentStack[1].ToUpperInvariant(), out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion

                        #region Velocity.Knots
                        if (ThisField.FieldType == typeof(Numbers.Velocity.Knots))
                        {
                            Numbers.Velocity.Knots Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsKnots(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Velocity.MetersPerSecond
                        if (ThisField.FieldType == typeof(Numbers.Velocity.MetersPerSecond))
                        {
                            Numbers.Velocity.MetersPerSecond Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsMetersPerSecond(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Velocity.MilesPerHour
                        if (ThisField.FieldType == typeof(Numbers.Velocity.MilesPerHour))
                        {
                            Numbers.Velocity.MilesPerHour Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsMilesPerHour(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Velocity.KilometersPerHour
                        if (ThisField.FieldType == typeof(Numbers.Velocity.KilometersPerHour))
                        {
                            Numbers.Velocity.KilometersPerHour Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsKilometersPerHour(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Velocity.FeetPerSecond
                        if (ThisField.FieldType == typeof(Numbers.Velocity.FeetPerSecond))
                        {
                            Numbers.Velocity.FeetPerSecond Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsFeetPerSecond(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Velocity.MachAtSeaLevel
                        if (ThisField.FieldType == typeof(Numbers.Velocity.MachAtSeaLevel))
                        {
                            Numbers.Velocity.MachAtSeaLevel Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsMachAtSeaLevel(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion

                        #region Length.Kilometers
                        if (ThisField.FieldType == typeof(Numbers.Length.Kilometers))
                        {
                            Numbers.Length.Kilometers Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsKilometers(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.Meters
                        if (ThisField.FieldType == typeof(Numbers.Length.Meters))
                        {
                            Numbers.Length.Meters Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsMeters(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.Centimeters
                        if (ThisField.FieldType == typeof(Numbers.Length.Centimeters))
                        {
                            Numbers.Length.Centimeters Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsCentimeters(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.Miles
                        if (ThisField.FieldType == typeof(Numbers.Length.Miles))
                        {
                            Numbers.Length.Miles Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsMiles(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.NauticalMiles
                        if (ThisField.FieldType == typeof(Numbers.Length.NauticalMiles))
                        {
                            Numbers.Length.NauticalMiles Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsNauticalMiles(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.Yards
                        if (ThisField.FieldType == typeof(Numbers.Length.Yards))
                        {
                            Numbers.Length.Yards Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsYards(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.Feet
                        if (ThisField.FieldType == typeof(Numbers.Length.Feet))
                        {
                            Numbers.Length.Feet Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsFeet(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Length.Inches
                        if (ThisField.FieldType == typeof(Numbers.Length.Inches))
                        {
                            Numbers.Length.Inches Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsInches(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion

                        #region Mass.Tonnes
                        if (ThisField.FieldType == typeof(Numbers.Mass.Tonnes))
                        {
                            Numbers.Mass.Tonnes Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsTonnes(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Mass.Kilograms
                        if (ThisField.FieldType == typeof(Numbers.Mass.Kilograms))
                        {
                            Numbers.Mass.Kilograms Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsKilograms(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Mass.Grams
                        if (ThisField.FieldType == typeof(Numbers.Mass.Grams))
                        {
                            Numbers.Mass.Grams Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsGrams(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Mass.Pounds
                        if (ThisField.FieldType == typeof(Numbers.Mass.Pounds))
                        {
                            Numbers.Mass.Pounds Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsPounds(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Mass.Ounces
                        if (ThisField.FieldType == typeof(Numbers.Mass.Ounces))
                        {
                            Numbers.Mass.Ounces Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsOunces(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Mass.Stones
                        if (ThisField.FieldType == typeof(Numbers.Mass.Stones))
                        {
                            Numbers.Mass.Stones Value = 0;
                            bool Failed = !ThisArgumentStack[1].AsStones(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion

                        #region Angles.Degrees
                        if (ThisField.FieldType == typeof(Numbers.Angles.Degrees))
                        {
                            Numbers.Angles.Degrees Value = 0;
                            bool Failed = ThisArgumentStack[1].AsDegrees(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Angles.Radians
                        if (ThisField.FieldType == typeof(Numbers.Angles.Radians))
                        {
                            Numbers.Angles.Radians Value = 0;
                            bool Failed = ThisArgumentStack[1].AsRadians(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion
                        #region Angles.Gradians
                        if (ThisField.FieldType == typeof(Numbers.Angles.Gradians))
                        {
                            Numbers.Angles.Gradians Value = 0;
                            bool Failed = ThisArgumentStack[1].AsGradians(out Value);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ", " + ThisArgumentStack[0] + ": " + ThisArgumentStack[1]);
                                Debug.WriteLine("Failed to convert value.");
                            }
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion

                        //3 VALUES:
                        if (ThisArgumentStack.Length < 3)
                        {
                            Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ": " + ThisArgumentStack.ToArray().ToStringList());
                            Debug.WriteLine("Bad .DAT Line - Break here to investigate.");
                        }

                        //4 VALUES:
                        if (ThisArgumentStack.Length < 4)
                        {
                            Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ": " + ThisArgumentStack.ToArray().ToStringList());
                            Debug.WriteLine("Bad .DAT Line - Break here to investigate.");
                        }

                        #region Math3D.Point3
                        if (ThisField.FieldType == typeof(Math3D.Point3))
                        {
                            Math3D.Point3 Value = new Math3D.Point3(0,0,0);
                            Numbers.Length.Meters _X = 0;
                            Numbers.Length.Meters _Y = 0;
                            Numbers.Length.Meters _Z = 0;
                            bool Failed = !ThisArgumentStack[1].AsMeters(out _X);
                            Failed |= !ThisArgumentStack[2].AsMeters(out _Y);
                            Failed |= !ThisArgumentStack[3].AsMeters(out _Z);
                            if (Failed)
                            {
                                Log.Warning("Failed to Convert Dat Variable for " + ThisAircraft.Identify + ": " + ThisArgumentStack.Skip(1).ToArray().ToStringList());
                                Debug.WriteLine("Failed to convert value.");
                            }
                            Value.X = _X;
                            Value.Y = _Y;
                            Value.Z = _Z;
                            ThisField.SetValue(Output, Value);
                            continue;
                        }
                        #endregion

                        Log.Warning("Dat Variable Not Recognised for " + ThisAircraft.Identify + "?: " + ThisArgumentStack.ToArray().ToStringList());
                        Debug.WriteLine("Value not recognised.");
                    }
                }
                //Debug.WriteLine(ThisField.ToString());
            }
            #endregion
            #region Post-Conversion Integrity Checks
            if (Output.WEIGFUEL <= 0)
            {
                Log.Warning("Something went wrong in conversion of fuel? " + ThisAircraft.Identify);
            }
            #endregion
            return Output;
        }
    }
}