using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenYS
{
    public static class World
    {


        public static bool Load(string Name)
        {
            MetaData.Scenery Target = MetaData._Scenery.FindByName(Name);
            if (Target == MetaData._Scenery.None)
            {
                return false;
            }
            else
            {
                Load(Target);
                return true;
            }
        }

        public static bool Load(MetaData.Scenery InputScenery)
        {
            Objects.GroundList.Clear();
            Objects.PathList.Clear();
            Objects.StartPositionList.Clear();

            Console.Write(ConsoleColor.DarkYellow, "Loading Scenery: ");
            Console.WriteLine(ConsoleColor.Cyan, InputScenery.Identify.ReplaceAll("\0", ""));
            Console.ForegroundColor = ConsoleColor.White;

            LoadFLD(InputScenery);
            Console.Write(ConsoleColor.White, "    Loaded ");
            if (Objects.GroundList.Count == 0) Console.Write(ConsoleColor.Red, Objects.GroundList.Count.ToString());
            else Console.Write(ConsoleColor.Green, Objects.GroundList.Count.ToString());
            Console.Write(ConsoleColor.White, " Ground Objects from FLD.");
            Console.WriteLine("");

            Console.Write(ConsoleColor.White, "    Loaded ");
            if (Objects.PathList.Count == 0) Console.Write(ConsoleColor.Red, Objects.PathList.Count.ToString());
            else Console.Write(ConsoleColor.Green, Objects.PathList.Count.ToString());
            Console.Write(ConsoleColor.White, " Motion Paths from FLD.");
            Console.WriteLine("");

            LoadSTP(InputScenery);
            Console.Write(ConsoleColor.White, "    Loaded ");
            if (Objects.StartPositionList.Count == 0) Console.Write(ConsoleColor.Red, Objects.StartPositionList.Count.ToString());
            else Console.Write(ConsoleColor.Green, Objects.StartPositionList.Count.ToString());
            Console.Write(ConsoleColor.White, " Start Positions from STP.");
            Console.WriteLine("");

            int PreviousGrounds = Objects.GroundList.Count;

            LoadYFS(InputScenery);
            Console.Write(ConsoleColor.White, "    Loaded ");
            if (Objects.GroundList.Count - PreviousGrounds == 0) Console.Write(ConsoleColor.Red, (Objects.GroundList.Count - PreviousGrounds).ToString());
            else Console.Write(ConsoleColor.Green, (Objects.GroundList.Count - PreviousGrounds).ToString());
            Console.Write(ConsoleColor.White, " Ground Objects from YFS.");
            Console.WriteLine("");
            return true;
        }

        private static bool LoadFLD(MetaData.Scenery InputScenery)
        {
            if (!Files.FileExists(Settings.Loading.YSFlightDirectory + "/" + InputScenery.SceneryPath1Fld)) return false;
            //get the grounds from this fld file.
            string[] FLDContents = Files.FileReadAllLines(Settings.Loading.YSFlightDirectory + "/" + InputScenery.SceneryPath1Fld);
            Objects.RootScenery = new Objects.Scenery();
            Objects.Ground CurrentGround = Objects.NULL_Ground;
            Objects.Path CurrentPath = Objects.NULL_Path;
            Objects.RootScenery.Parent = Objects.RootScenery;
            Objects.Scenery CurrentScenery = Objects.RootScenery;
            Objects.Scenery TargetScenery = Objects.RootScenery;
            int ExpectPos = 0;
            int Indent = 0;
            int GOBsHandled = 0;
            int PSTsHandled = 0;
            List<Objects.Scenery> AllSceneries = new List<Objects.Scenery>();
            for (int i = 0; i < FLDContents.Length; i++)
            {
                string ThisLine = FLDContents[i];
	            if (ThisLine == "") continue;

                while (i > CurrentScenery.EndLine && CurrentScenery != Objects.RootScenery)
                {
                    Indent--;
                    CurrentScenery = CurrentScenery.Parent;
                    if (Indent < 0) Indent = 0;
                }

                if (CurrentGround != Objects.NULL_Ground | CurrentPath != Objects.NULL_Path)
                {
                    if (CurrentGround != Objects.NULL_Ground)
                    {
                        #region POS
                        if (ThisLine.ToUpperInvariant().StartsWith("POS"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 7) continue; //couldn't read the position line - error?
                            bool failed = false;
                            failed |= !Single.TryParse(Split[1], out CurrentGround.Position.X);
                            failed |= !Single.TryParse(Split[2], out CurrentGround.Position.Y);
                            failed |= !Single.TryParse(Split[3], out CurrentGround.Position.Z);
                            failed |= !Single.TryParse(Split[4], out CurrentGround.Attitude.X);
                            failed |= !Single.TryParse(Split[5], out CurrentGround.Attitude.Y);
                            failed |= !Single.TryParse(Split[6], out CurrentGround.Attitude.Z);
                            CurrentGround.Attitude.X = (CurrentGround.Attitude.X / 65535 * 360);
                            CurrentGround.Attitude.Y = (CurrentGround.Attitude.Y / 65535 * 360);
                            CurrentGround.Attitude.Z = (CurrentGround.Attitude.Z / 65535 * 360);

                            if (failed)
                            {
                                //Console.WriteLine("POS GRO: " + ThisLine);
                                CurrentGround.Position.X = 0;
                                CurrentGround.Position.Y = 0;
                                CurrentGround.Position.Z = 0;
                                CurrentGround.Attitude.X = 0;
                                CurrentGround.Position.Y = 0;
                                CurrentGround.Position.Z = 0;
                            }
                            continue;
                        }
                        #endregion
                        #region NAM
                        if (ThisLine.ToUpperInvariant().StartsWith("NAM"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 2) continue; //couldn't read the name line - error?
                            CurrentGround.Identify = Split[1];
                            CurrentGround.MetaGroundObject = MetaData._Ground.FindByName(Split[1]);
                            continue;
                        }
                        #endregion
                        #region TAG
                        if (ThisLine.ToUpperInvariant().StartsWith("TAG"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(new char[] { ' ' }, 2);
                            if (Split.Length < 2) continue; //couldn't read the name line - error?
                            CurrentGround.Tag = Split[1].ReplaceAll("\"", "").ReplaceAll(" ", "_");
                            //Console.WriteLine("TAG: " + CurrentGround.Tag);
                            continue;
                        }
                        #endregion
                        #region IFF
                        if (ThisLine.ToUpperInvariant().StartsWith("IFF"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 2) continue; //couldn't read the iff line - error?
                            bool failed = false;
                            failed |= !UInt32.TryParse(Split[1], out CurrentGround.IFF);
                            if (failed)
                            {
                                CurrentGround.IFF = 0;
                            }
                            continue;
                        }
                        #endregion
                        #region END
                        if (ThisLine.ToUpperInvariant().StartsWith("END"))
                        {
                            CurrentScenery.GroundObjects.Add(CurrentGround);
                            //Console.WriteLine("GOB: " + CurrentGround.Identify);
                            CurrentGround = Objects.NULL_Ground;
                            GOBsHandled++;
                            continue;
                        }
                        #endregion
                    }
                    if (CurrentPath != Objects.NULL_Path)
                    {
                        #region POS
                        if (ThisLine.ToUpperInvariant().StartsWith("POS"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 4) continue; //couldn't read the position line - error?
                            bool failed = false;
                            failed |= !Single.TryParse(Split[1], out CurrentPath.Position.X);
                            failed |= !Single.TryParse(Split[2], out CurrentPath.Position.Y);
                            failed |= !Single.TryParse(Split[3], out CurrentPath.Position.Z);


                            if (failed)
                            {
                                //Console.WriteLine("POS PST: " + ThisLine);
                                CurrentPath.Position.X = 0;
                                CurrentPath.Position.Y = 0;
                                CurrentPath.Position.Z = 0;
                            }
                            continue;
                        }
                        #endregion
                        #region PNT
                        if (ThisLine.ToUpperInvariant().StartsWith("PNT"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 4) continue; //couldn't read the position line - error?
                            Objects.Path._Point ThisPoint = new Objects.Path._Point();
                            bool failed = false;
                            failed |= !Single.TryParse(Split[1], out ThisPoint.X);
                            failed |= !Single.TryParse(Split[2], out ThisPoint.Y);
                            failed |= !Single.TryParse(Split[3], out ThisPoint.Z);

                            if (failed)
                            {
                                ThisPoint.X = 0;
                                ThisPoint.Y = 0;
                                ThisPoint.Z = 0;
                            }
                            CurrentPath.Points.Add(ThisPoint);
                            continue;
                        }
                        #endregion
                        #region TAG
                        if (ThisLine.ToUpperInvariant().StartsWith("TAG"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(new char[] { ' ' }, 2);
                            if (Split.Length < 2) continue; //couldn't read the name line - error?
                            CurrentPath.Identify = Split[1].ReplaceAll("\"", "").ReplaceAll(" ", "_");
                            continue;
                        }
                        #endregion
                        #region AREA
                        if (ThisLine.ToUpperInvariant().StartsWith("AREA"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(new char[] { ' ' }, 2);
                            if (Split.Length < 2) continue; //couldn't read the name line - error?
                            CurrentPath.AreaType = Split[1].ReplaceAll("\"", "").ReplaceAll(" ", "_");
                            continue;
                        }
                        #endregion
                        #region ISLOOP
                        if (ThisLine.ToUpperInvariant().StartsWith("ISLOOP"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(new char[] { ' ' }, 2);
                            if (Split.Length < 2) continue; //couldn't read the isloop line - error?
                            string option = Split[1].ReplaceAll("\"", "").ReplaceAll(" ", "_");
                            Boolean.TryParse(option, out CurrentPath.IsLooping);
                            continue;
                        }
                        #endregion
                        #region ID
                        if (ThisLine.ToUpperInvariant().StartsWith("ID"))
                        {
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 2) continue; //couldn't read the iff line - error?
                            bool failed = false;
                            failed |= !UInt32.TryParse(Split[1], out CurrentPath.Type);
                            if (failed)
                            {
                                CurrentPath.Type = 0;
                            }
                            continue;
                        }
                        #endregion
                        #region FIL
                        if (ThisLine.ToUpperInvariant().StartsWith("FIL"))
                        {
                            //Dont even know WHAT to do here...
                            continue;
                            /*
                            string[] Split = Utilities.StringCompress(ThisLine).Split(new char[] { ' ' }, 2);
                            if (Split.Length < 2) continue; //couldn't read the name line - error?
                            CurrentPath.Identify = Split[1].ReplaceAll("\"", "").ReplaceAll(" ", "_");
                            //Console.WriteLine("TAG: " + CurrentGround.Tag);
                            continue;
                            */ 
                        }
                        #endregion

                        #region END
                        if (ThisLine.ToUpperInvariant().StartsWith("END"))
                        {
                            foreach (Objects.Path._Point ThisPoint in CurrentPath.Points.ToArray())
                            {
                                ThisPoint.X += CurrentPath.Position.X;
                                ThisPoint.Y += CurrentPath.Position.Y;
                                ThisPoint.Z += CurrentPath.Position.Z;
                            }
                            CurrentPath.Position.X = 0;
                            CurrentPath.Position.Y = 0;
                            CurrentPath.Position.Z = 0;
                            //if (CurrentPath.IsLooping)
                            //{
                            //    Objects.Path._Point NewPoint = new Objects.Path._Point();
                            //    NewPoint.X = CurrentPath.Points.ToArray()[0].X;
                            //    NewPoint.Y = CurrentPath.Points.ToArray()[0].Y;
                            //    NewPoint.Z = CurrentPath.Points.ToArray()[0].Z;
                            //    CurrentPath.Points.Add(NewPoint);
                            //}
                            CurrentScenery.MotionPaths.Add(CurrentPath);
                            CurrentPath = Objects.NULL_Path;
                            PSTsHandled++;
                            continue;
                        }
                        #endregion
                    }
                }
                else
                {
                        #region PCK
                        if (ThisLine.ToUpperInvariant().StartsWith("PCK"))
                        {
                            if (!ThisLine.Split(' ')[1].ToUpperInvariant().Contains(".FLD")) continue;
                            Objects.Scenery ChildScenery = new Objects.Scenery();
                            ChildScenery.Parent = CurrentScenery;
                            CurrentScenery.AddChild(ChildScenery);
                            AllSceneries.Add(ChildScenery);
                            CurrentScenery = ChildScenery;
                            CurrentScenery.Identify = ThisLine.Split(' ')[1].ToUpperInvariant();
                            int Output = 0;
                            Int32.TryParse(ThisLine.Split(' ')[2].ToUpperInvariant(), out Output);
                            CurrentScenery.EndLine = i + Output;
                            Indent++;
                            GOBsHandled = 0;
                            PSTsHandled = 0;
                            //Console.WriteLine(CurrentScenery.Identify);
                            continue;
                        }
                        #endregion
                        #region FIL
                        if (ThisLine.ToUpperInvariant().ReplaceAll("\t", " ").Split(' ')[0] == "FIL")
                        {
                            if (!ThisLine.Split(' ')[1].ToUpperInvariant().Contains(".FLD")) continue;
                            //Console.WriteLine(StringRepeat("-", Indent) + i.ToString() + " FIL: " + ThisLine.ToUpperInvariant().ReplaceAll("\t", " ").Split(' ')[1]);
                            //Console.WriteLine(ThisLine.ToUpperInvariant().ReplaceAll("\t", " ").Split(' ')[1]);
                            if (AllSceneries.Select(x => x.Identify).Contains(ThisLine.ToUpperInvariant().ReplaceAll("\t", " ").Split(' ')[1]))
                            {
                                //Console.WriteLine("FIL FOUND: " + ThisLine);
                                TargetScenery = AllSceneries.Where(x => x.Identify == ThisLine.ToUpperInvariant().ReplaceAll("\t", " ").Split(' ')[1]).ToArray()[0];
                                ExpectPos = i + 1;
                            }
                            continue;
                        }
                        #endregion
                        #region POS
                        if (ThisLine.ToUpperInvariant().StartsWith("POS") && ExpectPos == i)
                        {
                            //Console.WriteLine("POS SCE: " + ThisLine);
                            string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                            if (Split.Length < 7)
                            {
                                continue; //couldn't read the position line - error?
                            }
                            bool failed = false;
                            failed |= !Single.TryParse(Split[1], out TargetScenery.Position.X);
                            failed |= !Single.TryParse(Split[2], out TargetScenery.Position.Y);
                            failed |= !Single.TryParse(Split[3], out TargetScenery.Position.Z);
                            failed |= !Single.TryParse(Split[4], out TargetScenery.Attitude.X);
                            failed |= !Single.TryParse(Split[5], out TargetScenery.Attitude.Y);
                            failed |= !Single.TryParse(Split[6], out TargetScenery.Attitude.Z);
                            TargetScenery.Attitude.X = (TargetScenery.Attitude.X / 65535 * 360);
                            TargetScenery.Attitude.Y = (TargetScenery.Attitude.Y / 65535 * 360);
                            TargetScenery.Attitude.Z = (TargetScenery.Attitude.Z / 65535 * 360);


                            if (failed)
                            {
                                //Console.WriteLine("ERROR POS SCE: " + ThisLine);
                                TargetScenery.Position.X = 0;
                                TargetScenery.Position.Y = 0;
                                TargetScenery.Position.Z = 0;
                                TargetScenery.Attitude.X = 0;
                                TargetScenery.Position.Y = 0;
                                TargetScenery.Position.Z = 0;
                            }
                            continue;
                        }
                        #endregion

                        #region GND
                        if (ThisLine.ToUpperInvariant().StartsWith("GND") & CurrentScenery == Objects.RootScenery)
                        {
                            try
                            {
                                GroundColor = new Colors.XRGBColor(ThisLine.Substring(4));
                            }
                            catch
                            {
                            }
                        }
                        #endregion
                        #region SKY
                        if (ThisLine.ToUpperInvariant().StartsWith("SKY") & CurrentScenery == Objects.RootScenery)
                        {
                            try
                            {
                                SkyColor = new Colors.XRGBColor(ThisLine.Substring(4));
                            }
                            catch
                            {
                            }
                        }
                        #endregion

                        #region GOB
                        if (ThisLine.ToUpperInvariant().StartsWith("GOB"))
                        {
                            CurrentGround = new Objects.Ground();
                            CurrentPath = Objects.NULL_Path;
                            continue;
                        }
                        #endregion
                        #region PST
                        if (ThisLine.ToUpperInvariant().StartsWith("PST"))
                        {
                            if (i + 1 < FLDContents.Length)
                            {
                                string NextLine = FLDContents[i+1];
                                if (NextLine.ToUpperInvariant().StartsWith("ISLOOP"))
                                {
                                    CurrentGround = Objects.NULL_Ground;
                                    CurrentPath = new Objects.Path();
                                }
                            }
                            continue;
                        }
                        #endregion
                }
            }

            foreach (Objects.Scenery ThisScenery in Objects.RootScenery.Children)
            {
                ThisScenery.ProcessGrounds();
                ThisScenery.ProcessPaths();
            }
            foreach (Objects.Ground ThisGround in Objects.RootScenery.GroundObjects)
            {
                Objects.GroundList.Add(ThisGround);
            }
            foreach (Objects.Path ThisPath in Objects.RootScenery.MotionPaths.ToArray())
            {
                if (ThisPath.Identify != "" & ThisPath.Type == 15) Objects.PathList.Add(ThisPath);
            }
            return true;
        }

        private static bool LoadSTP(MetaData.Scenery InputScenery)
        {
            if (!Files.FileExists(Settings.Loading.YSFlightDirectory + "/" + InputScenery.SceneryPath2Stp)) return false;
            //get the stp's from this stp file.
            string[] STPContents = Files.FileReadAllLines(Settings.Loading.YSFlightDirectory + "/" + InputScenery.SceneryPath2Stp);
            foreach (string ThisLine in STPContents)
            {
            }
            Objects.StartPosition CurrentStartPosition = Objects.NULL_StartPosition;
            foreach (string ThisLine in STPContents)
            {
                if (CurrentStartPosition == Objects.NULL_StartPosition)
                {
                    #region N
                    if (ThisLine.ToUpperInvariant().StartsWith("N"))
                    {
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2) continue; //couldn't read the name line - error?
                        CurrentStartPosition = new Objects.StartPosition();
                        CurrentStartPosition.Identify = Split[1];
                        continue;
                    }
                    #endregion
                }
                else
                {
                    #region N
                    if (ThisLine.ToUpperInvariant().StartsWith("N"))
                    {
                        if (CurrentStartPosition != Objects.NULL_StartPosition) Objects.StartPositionList.Add(CurrentStartPosition);
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2)
                        {
                            CurrentStartPosition = Objects.NULL_StartPosition;
                            continue; //couldn't read the name line - error?
                        }
                        CurrentStartPosition = new Objects.StartPosition();
                        CurrentStartPosition.Identify = Split[1];
                        continue;
                    }
                    #endregion

                    #region C
                    if (ThisLine.ToUpperInvariant().StartsWith("C"))
                    {
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2) continue; //couldn't read the C line - error?
                        bool failed = false;
                        switch (Split[1].ToUpperInvariant())
                        {

                            case "POSITION":
                                #region POSITION
                                if (Split.Length < 5)
                                {
                                    continue; //couldn't read the position line - error?
                                }
                                failed = false;
                                if (Split[2].ToUpperInvariant().EndsWith("FT"))
                                {
                                    double output = 0;
                                    Double.TryParse(Split[2].ToUpperInvariant().ReplaceAll("FT", ""), out output);
                                    output *= 0.3048;
                                    Split[2] = output.ToString() + "M";
                                }
                                if (Split[3].ToUpperInvariant().EndsWith("FT"))
                                {
                                    double output = 0;
                                    Double.TryParse(Split[3].ToUpperInvariant().ReplaceAll("FT", ""), out output);
                                    output *= 0.3048;
                                    Split[3] = output.ToString() + "M";
                                }
                                if (Split[4].ToUpperInvariant().EndsWith("FT"))
                                {
                                    double output = 0;
                                    Double.TryParse(Split[4].ToUpperInvariant().ReplaceAll("FT", ""), out output);
                                    output *= 0.3048;
                                    Split[4] = output.ToString() + "M";
                                }

                                failed |= !Double.TryParse(Split[2].ToUpperInvariant().ReplaceAll("M", ""), out CurrentStartPosition.Position.X);
                                failed |= !Double.TryParse(Split[3].ToUpperInvariant().ReplaceAll("M", ""), out CurrentStartPosition.Position.Y);
                                failed |= !Double.TryParse(Split[4].ToUpperInvariant().ReplaceAll("M", ""), out CurrentStartPosition.Position.Z);

                                if (failed)
                                {
                                    CurrentStartPosition.Position.X = 0;
                                    CurrentStartPosition.Position.Y = 0;
                                    CurrentStartPosition.Position.Z = 0;
                                }
                                continue;
                                #endregion
                            case "ATTITUDE":
                                #region ATTITUDE
                                if (Split.Length < 5) continue; //couldn't read the attitude line - error?
                                failed = false;
                                double outx = 0;
                                double outy = 0;
                                double outz = 0;
                                failed |= !Double.TryParse(Split[2].ToUpperInvariant().ReplaceAll("DEG", ""), out outx);
                                failed |= !Double.TryParse(Split[3].ToUpperInvariant().ReplaceAll("DEG", ""), out outy);
                                failed |= !Double.TryParse(Split[4].ToUpperInvariant().ReplaceAll("DEG", ""), out outz);
                                CurrentStartPosition.Attitude.X += outx;
                                CurrentStartPosition.Attitude.Y += outy;
                                CurrentStartPosition.Attitude.Z += outz;

                                while (CurrentStartPosition.Attitude.X <= -180)
                                {
                                    CurrentStartPosition.Attitude.X += 360;
                                }
                                while (CurrentStartPosition.Attitude.Y <= -180)
                                {
                                    CurrentStartPosition.Attitude.Y += 360;
                                }
                                while (CurrentStartPosition.Attitude.Z <= -180)
                                {
                                    CurrentStartPosition.Attitude.Z += 360;
                                }

                                while (CurrentStartPosition.Attitude.X > 180)
                                {
                                    CurrentStartPosition.Attitude.X -= 360;
                                }
                                while (CurrentStartPosition.Attitude.Y > 180)
                                {
                                    CurrentStartPosition.Attitude.Y -= 360;
                                }
                                while (CurrentStartPosition.Attitude.Z > 180)
                                {
                                    CurrentStartPosition.Attitude.Z -= 360;
                                }

                                if (failed)
                                {
                                    CurrentStartPosition.Attitude.X = 0;
                                    CurrentStartPosition.Attitude.Y = 0;
                                    CurrentStartPosition.Attitude.Z = 0;
                                }
                                continue;
                                #endregion
                            case "INITSPED":
                                #region INITSPED
                                if (Split.Length < 3) continue; //couldn't read the initsped line - error?
                                failed = false;

                                if (Split[2].ToUpperInvariant().EndsWith("MACH"))
                                {
                                    double output = 0;
                                    Double.TryParse(Split[2].ToUpperInvariant().ReplaceAll("MACH", ""), out output);
                                    output *= 340.29;
                                    Split[2] = output.ToString() + "M/S";
                                }

                                failed |= !Double.TryParse(Split[2].ToUpperInvariant().ReplaceAll("M/S", ""), out CurrentStartPosition.Speed);

                                if (failed)
                                {
                                    CurrentStartPosition.Speed = 0;
                                }
                                continue;
                                #endregion
                            case "CTLTHROT":
                                #region CTLTHROT
                                if (Split.Length < 3) continue; //couldn't read the ctlthrot line - error?
                                failed = false;
                                failed |= !Double.TryParse(Split[2], out CurrentStartPosition.Throttle);

                                if (failed)
                                {
                                    CurrentStartPosition.Throttle = 0;
                                }
                                continue;
                                #endregion
                            case "CTLLDGEA":
                                #region CTLLDGEA
                                if (Split.Length < 3) continue; //couldn't read the ctlldgea line - error?
                                failed = false;
                                failed |= !Boolean.TryParse(Split[2], out CurrentStartPosition.Gear);

                                if (failed)
                                {
                                    CurrentStartPosition.Gear = true;
                                }
                                continue;
                                #endregion
                        }
                    }
                    #endregion

                    #region P
                    if (ThisLine.ToUpperInvariant().StartsWith("P"))
                    {

                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2) continue; //couldn't read the P line - error?
                        bool failed = false;
                        switch (Split[1].ToUpperInvariant())
                        {
                            case "CARRIER":
                                double AdjustAngleX = 0;
                                double AdjustAngleY = 0;
                                double AdjustAngleZ = 0;
                                failed |= !Double.TryParse(Split[6].ToUpperInvariant().ReplaceAll("DEG", ""), out AdjustAngleX);
                                failed |= !Double.TryParse(Split[7].ToUpperInvariant().ReplaceAll("DEG", ""), out AdjustAngleY);
                                failed |= !Double.TryParse(Split[8].ToUpperInvariant().ReplaceAll("DEG", ""), out AdjustAngleZ);

                                CurrentStartPosition.Attitude.X -= AdjustAngleX;
                                CurrentStartPosition.Attitude.Y -= AdjustAngleY;
                                CurrentStartPosition.Attitude.Z -= AdjustAngleZ;


                                if (failed)
                                {
                                    //CurrentStartPosition.Attitude.X = 0;
                                    //CurrentStartPosition.Attitude.Y = 0;
                                    //CurrentStartPosition.Attitude.Z = 0;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }
            }
            if (CurrentStartPosition != Objects.NULL_StartPosition)
            {
                Objects.StartPositionList.Add(CurrentStartPosition);
                //Since the declarations do not terminate, we need to add the last one at the end of the file.
            }

            return true;
        }

        private static bool LoadYFS(MetaData.Scenery InputScenery)
        {
            if (!Files.FileExists(Settings.Loading.YSFlightDirectory + "/" + InputScenery.SceneryPath3Yfs)) return false;
            //get the grounds from this fld file.
            string[] YFSContents = Files.FileReadAllLines(Settings.Loading.YSFlightDirectory + "/" + InputScenery.SceneryPath3Yfs);
            Objects.Ground CurrentGround = Objects.NULL_Ground;
            foreach (string ThisLine in YFSContents)
            {
                if (CurrentGround == Objects.NULL_Ground)
                {
                    #region GOB
                    if (ThisLine.ToUpperInvariant().StartsWith("GROUNDOB"))
                    {
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2)
                        {
                            CurrentGround = Objects.NULL_Ground;
                            continue; //couldn't read the name line - error?
                        }
                        CurrentGround = new Objects.Ground();
                        CurrentGround.Identify = Split[1];
                        CurrentGround.MetaGroundObject = MetaData._Ground.FindByName(Split[1]);
                        continue;
                    }
                    #endregion
                }
                else
                {
                    #region GOB
                    if (ThisLine.ToUpperInvariant().StartsWith("GROUNDOB"))
                    {
                        if (CurrentGround != Objects.NULL_Ground) Objects.GroundList.Add(CurrentGround);
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2) if (Split.Length < 2)
                            {
                                CurrentGround = Objects.NULL_Ground;
                                continue; //couldn't read the name line - error?
                            }
                        CurrentGround = new Objects.Ground();
                        CurrentGround.Identify = Split[1];
                        CurrentGround.MetaGroundObject = MetaData._Ground.FindByName(Split[1]);
                        continue;
                    }
                    #endregion
                    #region POS
                    if (ThisLine.ToUpperInvariant().StartsWith("GNDPOSIT"))
                    {
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 4) if (Split.Length < 2)
                            {
                                CurrentGround = Objects.NULL_Ground;
                                continue; //couldn't read the name line - error?
                            }
                        bool failed = false;

                        if (Split[1].ToUpperInvariant().EndsWith("FT"))
                        {
                            double output = 0;
                            failed |= !Double.TryParse(Split[1].ToUpperInvariant().ReplaceAll("FT", ""), out output);
                            output *= 0.3048;
                            Split[1] = output.ToString() + "M";
                        }
                        if (Split[2].ToUpperInvariant().EndsWith("FT"))
                        {
                            double output = 0;
                            failed |= !Double.TryParse(Split[2].ToUpperInvariant().ReplaceAll("FT", ""), out output);
                            output *= 0.3048;
                            Split[2] = output.ToString() + "M";
                        }
                        if (Split[3].ToUpperInvariant().EndsWith("FT"))
                        {
                            double output = 0;
                            failed |= !Double.TryParse(Split[3].ToUpperInvariant().ReplaceAll("FT", ""), out output);
                            output *= 0.3048;
                            Split[3] = output.ToString() + "M";
                        }

                        failed |= !Single.TryParse(Split[1].ToUpperInvariant().ReplaceAll("M", ""), out CurrentGround.Position.X);
                        failed |= !Single.TryParse(Split[2].ToUpperInvariant().ReplaceAll("M", ""), out CurrentGround.Position.Y);
                        failed |= !Single.TryParse(Split[3].ToUpperInvariant().ReplaceAll("M", ""), out CurrentGround.Position.Z);

                        if (failed)
                        {
                            CurrentGround.Position.X = 0;
                            CurrentGround.Position.Y = 0;
                            CurrentGround.Position.Z = 0;
                        }
                        continue;
                    }
                    #endregion
                    #region ATT
                    if (ThisLine.ToUpperInvariant().StartsWith("GNDATTIT"))
                    {
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 4)
                        {
                            CurrentGround = Objects.NULL_Ground;
                            continue; //couldn't read the name line - error?
                        }
                        bool failed = false;
                        failed |= !Single.TryParse(Split[1], out CurrentGround.Attitude.X);
                        failed |= !Single.TryParse(Split[2], out CurrentGround.Attitude.Y);
                        failed |= !Single.TryParse(Split[3], out CurrentGround.Attitude.Z);

                        if (failed)
                        {
                            CurrentGround.Attitude.X = 0;
                            CurrentGround.Attitude.Y = 0;
                            CurrentGround.Attitude.Z = 0;
                        }
                        continue;
                    }
                    #endregion
                    #region IFF
                    if (ThisLine.ToUpperInvariant().StartsWith("IDENTIFY"))
                    {
                        string[] Split = Strings.CompressWhiteSpace(ThisLine).Split(' ');
                        if (Split.Length < 2)
                        {
                            CurrentGround = Objects.NULL_Ground;
                            continue; //couldn't read the name line - error?
                        }
                        bool failed = false;
                        failed |= !UInt32.TryParse(Split[1], out CurrentGround.IFF);
                        if (failed)
                        {
                            CurrentGround.IFF = 0;
                        }
                        continue;
                    }
                    #endregion
                }
            }
            if (CurrentGround != Objects.NULL_Ground)
            {
                Objects.GroundList.Add(CurrentGround);
                //Since the declarations do not terminate, we need to add the last one at the end of the file.
            }

            return true;
        }

        public static Colors.XRGBColor GroundColor = Settings.Weather.Advanced.GndColor;

        public static Colors.XRGBColor SkyColor = Settings.Weather.Advanced.SkyColor;

        public static Colors.XRGBColor FogColor = Settings.Weather.Advanced.FogColor;

        public static class Objects
        {
            private static uint NextID = 1085;
            public static uint GetNextID()
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    //Locked otherwise we get race condition errors in the ID numbers! Do not want!
                    //NextID += 3;
                    NextID++;
                    return NextID;
                }
            }

            public class Aircraft
            {
                public string Identify;
                public uint IFF;

                public class _Position
                {
                    public double X;
                    public double Y;
                    public double Z;
                }
                public _Position Position = new _Position();

                public class _Attitude
                {
                    public double X;
                    public double Y;
                    public double Z;
                }
                public _Attitude Attitude = new _Attitude();
            }
            public static List<Aircraft> AircraftList = new List<Aircraft>();
            public static Aircraft NULL_Aircraft;

            public class StartPosition
            {
                public string Identify = "NULL";
                public double Speed = 0.00;
                public double Throttle = 0.00;
                public bool Gear = true;

                public class _Position
                {
                    public double X = 0.00;
                    public double Y = 0.00;
                    public double Z = 0.00;
                }
                public _Position Position = new _Position();

                public class _Attitude
                {
                    public double X = 0.00;
                    public double Y = 0.00;
                    public double Z = 0.00;
                }
                public _Attitude Attitude = new _Attitude();
            }
            public static List<StartPosition> StartPositionList = new List<StartPosition>();
            public static StartPosition NULL_StartPosition;

            public class Ground
            {
                public string Identify = "";
                public string Tag = "";
                public uint Strength = 240;
                public uint IFF = 0;
                public uint ID = World.Objects.GetNextID();
                public MetaData.Ground MetaGroundObject = MetaData._Ground.None;

                public class _Position
                {
                    public float X = 0;
                    public float Y = 0;
                    public float Z = 0;
                }
                public _Position Position = new _Position();

                public class _Attitude
                {
                    public float X = 0;
                    public float Y = 0;
                    public float Z = 0;
                }
                public _Attitude Attitude = new _Attitude();
            }
            public static List<Ground> GroundList = new List<Ground>();
            public static Ground NULL_Ground;

            public class Path
            {
                public string Identify = "";
                public uint Type = 0;
                public bool IsLooping = false;

                public string AreaType = "";

                public class _Position
                {
                    public float X = 0;
                    public float Y = 0;
                    public float Z = 0;
                }
                public _Position Position = new _Position();

                public class _Point
                {
                    public float X = 0;
                    public float Y = 0;
                    public float Z = 0;
                }
                public List<_Point> Points = new List<_Point>();

                public List<_Point> Interpolate()
                {
                    List<Objects.Path._Point> OriginalList = this.Points;
                    List<Objects.Path._Point> NewPath = new List<Objects.Path._Point>();
                    for (int i = 0; i < OriginalList.Count - 1; i++)
                    {
                        NewPath.Add(OriginalList.ToArray()[i]);
                        Objects.Path._Point NewPoint = new Objects.Path._Point();
                        NewPoint.X = (OriginalList.ToArray()[i].X + OriginalList.ToArray()[i + 1].X) / 2f;
                        NewPoint.Y = (OriginalList.ToArray()[i].Y + OriginalList.ToArray()[i + 1].Y) / 2f;
                        NewPoint.Z = (OriginalList.ToArray()[i].Z + OriginalList.ToArray()[i + 1].Z) / 2f;
                        NewPath.Add(NewPoint);
                        //if (i == OriginalList.Count - 2) NewPath.Add(OriginalList.ToArray()[i + 1]);
                        
                        /*
                        if (i == OriginalList.Count - 2 & this.IsLooping)
                        {
                            NewPoint = new Objects.Path._Point();
                            NewPoint.X = (OriginalList.ToArray()[i+1].X + OriginalList.ToArray()[0].X) / 2f;
                            NewPoint.Y = (OriginalList.ToArray()[i+1].Y + OriginalList.ToArray()[0].Y) / 2f;
                            NewPoint.Z = (OriginalList.ToArray()[i+1].Z + OriginalList.ToArray()[0].Z) / 2f;
                            NewPath.Add(NewPoint);
                        }
                        */
                        
                    }
                    NewPath.Add(OriginalList.ToArray()[OriginalList.Count - 1]);
                    return NewPath;
                }

                public List<_Point> Decimate()
                {
                    List<Objects.Path._Point> OriginalList = this.Points;
                    List<Objects.Path._Point> NewPath = new List<Objects.Path._Point>();
                    for (int i = 0; i < OriginalList.Count - 1; i++)
                    {
                        if (i == 0)
                        {
                            //add the start of the path.
                            NewPath.Add(OriginalList.ToArray()[0]);
                            continue;
                        }
                        if ((OriginalList.Count - 2) % 2 == 0)
                        {
                            //even no.
                            Objects.Path._Point NewPoint = new Objects.Path._Point();
                            NewPoint.X = (OriginalList.ToArray()[i].X + OriginalList.ToArray()[i + 1].X) / 2;
                            NewPoint.Y = (OriginalList.ToArray()[i].Y + OriginalList.ToArray()[i + 1].Y) / 2;
                            NewPoint.Z = (OriginalList.ToArray()[i].Z + OriginalList.ToArray()[i + 1].Z) / 2;
                            NewPath.Add(NewPoint);
                            i++; //skip the next point!
                        }
                        else
                        {
                            //odd no.
                            i++;
                            if (i < OriginalList.Count - 1) NewPath.Add(OriginalList.ToArray()[i]);
                        }
                    }
                    NewPath.Add(OriginalList.ToArray()[OriginalList.Count - 1]);
                    return NewPath;
                }

            }
            public static List<Path> PathList = new List<Path>();
            public static Path._Point NULL_Point = new Path._Point();
            public static Path NULL_Path = new Path();

            public class Scenery
            {
                public Scenery Parent = RootScenery;
                public string Identify = "";
                public int EndLine = 0;
                public List<Scenery> Children = new List<Scenery>();
                public List<Ground> GroundObjects = new List<Ground>();
                public List<Path> MotionPaths = new List<Path>();

                public class _Position
                {
                    public float X;
                    public float Y;
                    public float Z;
                }
                public _Position Position = new _Position();

                public class _Attitude
                {
                    public float X;
                    public float Y;
                    public float Z;
                }
                public _Attitude Attitude = new _Attitude();

                public bool AddChild(Scenery ThisChild)
                {
                    if (ThisChild == this) return false;
                    Children.Add(ThisChild);
                    return true;
                }

                public bool ProcessGrounds()
                {
                    int Indent = 0;
                    Scenery Target = this;
                    while (Target.Parent != RootScenery)
                    {
                        Indent++;
                        Target = Target.Parent;
                    }
                    foreach (Scenery ThisScenery in this.Children)
                    {
                        //Console.WriteLine("SUBSCENERY: " + Identify);
                        ThisScenery.ProcessGrounds();
                    }
                    //Console.WriteLine("END SUBSCENERY: " + Identify);
                    foreach (Ground ThisGround in this.GroundObjects)
                    {

                        float PosX = ThisGround.Position.X;
                        float PosY = ThisGround.Position.Y;
                        float PosZ = ThisGround.Position.Z;
                        ThisGround.Position.X = (float)(this.Position.X + (Math.Cos(-this.Attitude.X / 180 * Math.PI) * PosX) + (Math.Sin(-this.Attitude.X / 180 * Math.PI) * PosZ));
                        ThisGround.Position.Y = (float)(this.Position.Y + ThisGround.Position.Y);
                        ThisGround.Position.Z = (float)(this.Position.Z + (Math.Sin(-this.Attitude.X / 180 * Math.PI) * -PosX) + (Math.Cos(-this.Attitude.X / 180 * Math.PI) * PosZ));
                        ThisGround.Attitude.X += this.Attitude.X;
                        ThisGround.Attitude.Y += this.Attitude.Y;
                        ThisGround.Attitude.Z += this.Attitude.Z;
                        Parent.GroundObjects.Add(ThisGround);
                    }
                    GroundObjects.Clear();
                    return true;
                }

                public bool ProcessPaths()
                {
                    int Indent = 0;
                    Scenery Target = this;
                    while (Target.Parent != RootScenery)
                    {
                        Indent++;
                        Target = Target.Parent;
                    }
                    foreach (Scenery ThisScenery in this.Children)
                    {
                        //Console.WriteLine("SUBSCENERY: " + Identify);
                        ThisScenery.ProcessPaths();
                    }
                    //Console.WriteLine("END SUBSCENERY: " + Identify);
                    foreach (Path ThisPath in this.MotionPaths)
                    {
                        foreach (Path._Point ThisPoint in ThisPath.Points.ToArray())
                        {
                            float _PosX = ThisPoint.X + ThisPath.Position.X;
                            float _PosY = ThisPoint.Y + ThisPath.Position.Y;
                            float _PosZ = ThisPoint.Z + ThisPath.Position.Z;
                            ThisPoint.X = (float)(this.Position.X + (Math.Cos(-this.Attitude.X / 180 * Math.PI) * _PosX) + (Math.Sin(-this.Attitude.X / 180 * Math.PI) * _PosZ));
                            ThisPoint.Y = (float)(this.Position.Y + ThisPoint.Y);
                            ThisPoint.Z = (float)(this.Position.Z + (Math.Sin(-this.Attitude.X / 180 * Math.PI) * -_PosX) + (Math.Cos(-this.Attitude.X / 180 * Math.PI) * _PosZ));
                        }
                        float PosX = ThisPath.Position.X;
                        float PosY = ThisPath.Position.Y;
                        float PosZ = ThisPath.Position.Z;
                        ThisPath.Position.X = (float)(this.Position.X + (Math.Cos(-this.Attitude.X / 180 * Math.PI) * PosX) + (Math.Sin(-this.Attitude.X / 180 * Math.PI) * PosZ));
                        ThisPath.Position.Y = (float)(this.Position.Y + ThisPath.Position.Y);
                        ThisPath.Position.Z = (float)(this.Position.Z + (Math.Sin(-this.Attitude.X / 180 * Math.PI) * -PosX) + (Math.Cos(-this.Attitude.X / 180 * Math.PI) * PosZ));
                        Parent.MotionPaths.Add(ThisPath);
                    }
                    MotionPaths.Clear();
                    return true;
                }
            }

            public static Scenery RootScenery = new Scenery();
        }
    }
}