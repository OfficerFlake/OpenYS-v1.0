using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenYS
{
    public static partial class Games
    {
        public static class Racing2
        {
            private static int CurrentRaceID = 0;
            public static int GetNextRaceID()
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    int Out = CurrentRaceID + 1;
                    CurrentRaceID++;
                    return Out;
                }

            }

            #region RaceDescriptor
            public abstract class RaceDescription
            {
                #region Positioning
                public List<_Racer> RacersSortedByPosition
                {
                    get
                    {
                        if (RaceType == _RaceType.TimeTrial)
                        {
                            if (Racers.Count < 1) return new List<_Racer>();
                            List<_Racer> Output = Racers.ToArray().ToList();
                            Output.OrderBy(w => w.TimeJoinedSession); //first to join gets the advantage!
                            List<_Racer> Temp = new List<_Racer>();
                            foreach (_Racer ThisRacer in Output)
                            {
                                if (ThisRacer.QualifyingPosition > 0) Temp.Add(ThisRacer);
                            }
                            Temp.OrderBy(v => v.QualifyingPosition); //lowest time comes first!
                            foreach (_Racer ThisRacer in Output)
                            {
                                if (ThisRacer.QualifyingPosition == 0) Temp.Add(ThisRacer); //no time set moves to back of list.
                            }
                            Output = Temp.ToArray().ToList();
                            return Output;
                        }
                        if (RaceType == _RaceType.Race)
                        {
                            if (Racers.Count < 1) return new List<_Racer>();
                            List<_Racer> Output = Racers.ToArray().ToList();
                            Output.OrderBy(u => u.TimeJoinedSession); //first to join gets the advantage!
                            List<_Racer> Temp = new List<_Racer>();
                            foreach (_Racer ThisRacer in Output)
                            {
                                if (ThisRacer.QualifyingPosition > 0) Temp.Add(ThisRacer);
                            }
                            Temp.OrderBy(v => v.QualifyingPosition); //furthest up the grid comes first!
                            foreach (_Racer ThisRacer in Output)
                            {
                                if (ThisRacer.QualifyingPosition == 0) Temp.Add(ThisRacer); //no qualifying position set moves to back of grid.
                            }
                            Output = Temp.ToArray().ToList();
                            Output.OrderBy(w => w.TimeAheadOfLastCheckpoint);
                            Output.Reverse(); //furtheset ahead of checkpoint first in array.
                            Output.OrderBy(x => x.CurrentCheckpoint);
                            Output.Reverse(); //furtheset forward checkpoints first in array.
                            Output.OrderBy(y => y.CurrentLap);
                            Output.Reverse(); //furthest forward lap followed by furthest forwards checkpoint, followed by furthest time ahead of that checkpoint, followed by the qualifying position.
                            return Output;
                        }

                        //undefined!
                        if (RaceType == _RaceType.Undefined)
                        {
                            return new List<_Racer>();
                        }

                        //logic fail
                        return new List<_Racer>();
                    }
                }
                public int GetPositionOfRacer(_Racer ThisRacer)
                {
                    int Position = 1;
                    try
                    {
                        Position += RacersSortedByPosition.IndexOf(ThisRacer);
                    }
                    catch
                    {
                        Position = 0;
                    }
                    return Position;
                }
                #endregion

                #region Descriptive Variables
                public int ID = 0;

                public World.Objects.Path Path = null;
                public bool ReverseCircuit = false;
                public Client Owner = Clients.NoClient; //person who created the event - has control of the event!
                public bool JoinLock = false; //can new users join the race event?
                public bool RespawnLock = false; //can current users respawn if they die or are killed?
                #endregion

                #region Race Parameters
                #region Penalties
                public double JumpStartPenalty = 30.00;
                public bool EnableJumpStartDetection = false;
                #endregion
                #region Timing
                public DateTime RaceStarted = DateTime.Now;
                public DateTime RaceEnds = DateTime.Now; //if 0, never expires...
                public float FastestLap = 0;
                public float[] FastestSectors = new float[] { };
                public float[] CumulativeSectors = new float[] { };
                #endregion
                #region RaceType
                [Flags]
                public enum _RaceType
                {
                    Undefined = 0 << 0,
                    TimeTrial = 1 << 0,
                    Race      = 1 << 1,
                }
                public _RaceType RaceType = _RaceType.Undefined;
                public void RaceTypeSetUndefined()
                {
                    if (Status.CurrentState.State == _Status._RaceStateFlags.NotStarted)
                    {
                        RaceType = _RaceType.Undefined;
                    }
                }
                public void RaceTypeSetTimeTrial()
                {
                    if (Status.CurrentState.State == _Status._RaceStateFlags.NotStarted)
                    {
                        RaceType = _RaceType.TimeTrial;
                    }
                }
                public void RaceTypeSetRace()
                {
                    if (Status.CurrentState.State == _Status._RaceStateFlags.NotStarted)
                    {
                        RaceType = _RaceType.Race;
                    }
                }
                public bool RaceTypeIsUndefined()
                {
                    return (RaceType == _RaceType.Undefined);
                }
                public bool RaceTypeIsTimeTrial()
                {
                    return (RaceType == _RaceType.TimeTrial);
                }
                public bool RaceTypeIsRace()
                {
                    return (RaceType == _RaceType.Race);
                }
                #endregion
                #region Laps
                public int TotalLapCount = 0;
                public int CurrentLapCount
                {
                    get
                    {
                        if (Racers.Count < 1) return 0;
                        return (Racers.OrderBy(x => x.CurrentLap)).Last().CurrentLap;
                    }
                }
                #endregion
                #region Status
                public class _Status
                {
                    #region RaceStateFlags
                    [Flags]
                    public enum _RaceStateFlags
                    {
                        NotStarted = 0 << 0,
                        InProgress = 1 << 1,
                        Finished   = 1 << 2,
                    }
                    #endregion
                    #region RaceStateMoments
                    public class RaceStateMoment
                    {

                        public _RaceStateFlags State = _RaceStateFlags.NotStarted;
                        public DateTime Time = DateTime.Now;

                        public RaceStateMoment(_RaceStateFlags _State)
                        {
                            State = _State;
                            Time = DateTime.Now;
                        }
                    }
                    #endregion
                    public List<RaceStateMoment> StateHistory = new List<RaceStateMoment>() { new RaceStateMoment(_RaceStateFlags.NotStarted) };
                    public void SetNotStarted()
                    {
                        StateHistory.Add(new RaceStateMoment(_RaceStateFlags.NotStarted));
                    }
                    public void SetInProgress()
                    {
                        StateHistory.Add(new RaceStateMoment(_RaceStateFlags.InProgress));
                    }
                    public void SetFinished()
                    {
                        StateHistory.Add(new RaceStateMoment(_RaceStateFlags.Finished));
                    }
                    public RaceStateMoment CurrentState
                    {
                        get
                        {
                            return StateHistory.Last();
                        }
                        set
                        {
                            StateHistory.Add(value);
                        }
                    }
                    public bool IsNotStarted()
                    {
                        return (CurrentState.State == _RaceStateFlags.NotStarted);
                    }
                    public bool IsInProgress()
                    {
                        return (CurrentState.State == _RaceStateFlags.InProgress);
                    }
                    public bool IsFinished()
                    {
                        return (CurrentState.State == _RaceStateFlags.Finished);
                    }
                }
                public _Status Status = new _Status();
                #endregion
                #endregion

                #region Participants
                public class _Racer
                {
                    public RaceDescription Race = null;
                    public Client Client = Clients.NoClient; //Client object of this racer
                    public float TimeStamp_RaceStarted = 0; //The timestamp of the client when the race was started!
                    public float TimeStamp_LastCheckpoint = 0; //the timestamp of the client when they passed the last checkpoint.
                    public float TimeStamp_LapStarted = 0; //the timestamp of the client when they passed the last checkpoint.
                    public DateTime TimeJoinedSession = DateTime.Now;
                    public float FastestLap = 0; //laptimes of 0 are invalid, will not count!
                    public int QualifyingPosition = 0;
                    public int CurrentCheckpoint = 0; //the incremental index of the checkpoint.
                    public int CurrentLap = 0; //The current lap number.
                    public bool JumpStart = false;
                    public float[] FastestSectors = new float[] { };
                    public float[] CumulativeSectors = new float[] { };

                    public float TimeAheadOfLastCheckpoint
                    {
                        get
                        {
                            if (Client.Vehicle == Vehicles.NoVehicle) return 0;
                            return (Client.Vehicle.TimeStamp - TimeStamp_LastCheckpoint);
                        }
                    }
                    public _Racer GetPosionPrev()
                    {
                        int CurrentPos = Race.GetPositionOfRacer(this);
                        int TotalRacers = Race.Racers.Count();
                        if (CurrentPos >= TotalRacers) //Last Place...
                        {
                            return this;
                        }
                        else
                        {
                            try
                            {
                                return Race.RacersSortedByPosition[CurrentPos + 1];
                            }
                            catch
                            {
                                return null;
                            }

                        }
                    }
                    public _Racer GetPosionNext()
                    {
                        int CurrentPos = Race.GetPositionOfRacer(this);
                        int TotalRacers = Race.Racers.Count();
                        if (CurrentPos <= 0) //First Place...
                        {
                            return this;
                        }
                        else
                        {
                            try
                            {
                                return Race.RacersSortedByPosition[CurrentPos - 1];
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }

                    public float RaceTimeAheadOfPrev()
                    {
                        _Racer RacerPrev = GetPosionPrev();
                        if (RacerPrev == this) return 0;
                        if (RacerPrev == null) return 0;

                        int RacerPrevSectorCount = RacerPrev.CumulativeSectorTimeStamps.Count();
                        if (CumulativeSectorTimeStamps.Count() < RacerPrevSectorCount)
                        {
                            return 0;
                        }
                        float RacerPrevTimestamp = RacerPrev.CumulativeSectorTimeStamps[RacerPrevSectorCount - 1];
                        float RacerThisTimestamp = CumulativeSectorTimeStamps[RacerPrevSectorCount - 1];
                        float TimeAhead = RacerThisTimestamp - RacerPrevTimestamp;
                        return TimeAhead;
                    }
                    public float RaceTimeBehindNext()
                    {
                        _Racer RacerNext = GetPosionNext();
                        if (RacerNext == this) return 0;
                        if (RacerNext == null) return 0;

                        int RacerThisSectorCount = CumulativeSectorTimeStamps.Count();
                        if (RacerThisSectorCount > RacerNext.CumulativeSectorTimeStamps.Count())
                        {
                            return 0;
                        }
                        float RacerNextTimestamp = RacerNext.CumulativeSectorTimeStamps[RacerThisSectorCount - 1];
                        float RacerThisTimestamp = CumulativeSectorTimeStamps[RacerThisSectorCount - 1];
                        float TimeBehind = RacerThisTimestamp - RacerNextTimestamp;
                        return TimeBehind;
                    }

                    public string RaceTimeGetTimeAheadString()
                    {
                        _Racer Prev = GetPosionPrev();
                        if (Prev == this | Prev == null) return "<ERROR>";

                        return "P" + Race.GetPositionOfRacer(Prev) + " " + Prev.Client.Username.Resize(16).Replace('\0', ' ') + " " + RaceTimeAheadOfPrev().AsFormattedTimeDifference();
                    }
                    public string RaceTimeGetTimeBehindString()
                    {
                        _Racer Next = GetPosionNext();
                        if (Next == this | Next == null) return "<ERROR>";

                        return "P" + Race.GetPositionOfRacer(Next) + " " + Next.Client.Username.Resize(16).Replace('\0', ' ') + " " + RaceTimeBehindNext().AsFormattedTimeDifference();
                    }

                    public float RaceTimeGetTotalTime()
                    {
                        //timestamp race started
                        //latest cumulative time
                        //end time.
                        if (CurrentCheckpoint / Race.Path.Points.Count() >= Race.TotalLapCount)
                        {
                            //already finished the race, get the race time!
                            return CumulativeSectorTimeStamps.Last();
                        }
                        else
                        {
                            //not finished the race, get the current vehicle timestamp...
                            if (Client.Vehicle != Vehicles.NoVehicle) return Client.Vehicle.TimeStamp - TimeStamp_RaceStarted;

                            //no vehicle, use a rough estimate.
                            else return (float)(DateTime.Now - Race.RaceStarted).TotalSeconds;
                        }
                    }

                    public List<float> CumulativeSectorTimeStamps = new List<float>(); //total time from race start to reach this sector...
                    public List<float> SectorTimeStamps = new List<float>(); //time taken to travel sector...

                    public _Racer(Client ThisClient, RaceDescription ThisRace)
                    {
                        Client = ThisClient;
                        Race = ThisRace;

                        FastestSectors = new float[ThisRace.FastestSectors.Count()];
                        CumulativeSectors = new float[ThisRace.CumulativeSectors.Count()];
                    }
                }
                public List<_Racer> Racers = new List<_Racer>();
                public _Racer AddRacer(Client ThisClient)
                {
                    lock (Racers)
                    {
                        _Racer ThisRacer = new _Racer(ThisClient, this);
                        Racers.Add(ThisRacer);
                        return ThisRacer;
                    }
                }
                #endregion

                #region Updating
                public void CheckIntersections(_Racer ThisRacer)
                {
                    List<Client> NONRACING = Clients.AllClients.Except(ThisRacer.Race.Racers.Select(x=>x.Client)).ToList();
                    List<Client> RACING = ThisRacer.Race.Racers.Select(x=>x.Client).ToList();

                    if (RaceEnds <= DateTime.Now & Status.IsInProgress() & RaceEnds > RaceStarted)
                    {
                        //The session has timed out!
                        lock (Threads.GenericThreadSafeLock)
                        {
                            if (!Status.IsFinished()) Status.SetFinished();
                            foreach (_Racer DNFRacer in Racers.Where(x => x.CurrentCheckpoint / (Path.Points.Count) < TotalLapCount))
                            {
                                DNFRacer.Client.SendMessage("RACE OVER ##" + " P" + GetPositionOfRacer(DNFRacer) + " ## RACE OVER");
                            }
                        }
                    }

                    if (ThisRacer.CurrentLap > TotalLapCount && TotalLapCount > 0)
                    {
                        //stop counting for this racer, they finished the race!
                        return;
                    }

                    Client ThisClient = ThisRacer.Client;
                    Vehicle ThisVehicle = ThisRacer.Client.Vehicle;
                    if (ThisVehicle == Vehicles.NoVehicle) return;
                    World.Objects.Path ThisPath = Path;
                    World.Objects.Path._Point ThisPoint;
                    try
                    {
                        ThisPoint = ThisPath.Points[(ThisRacer.CurrentCheckpoint + 1) % ThisPath.Points.Count];
                    }
                    catch
                    {
                        return;
                    }
                    //Target point will be the next checkpoint the vehicle has to pass!
                    #region Set up the 3D Maths required.
                    //if (!ActiveCheckPoints.Select(x => x.CheckPoint).Contains(ThisPath.Points[0]) & ThisPoint != ThisPath.Points[0]) continue;
                    Math3D.Point3 Previous = new Math3D.Point3(ThisVehicle.Prev_PosX, ThisVehicle.Prev_PosY, ThisVehicle.Prev_PosZ);
                    Math3D.Point3 Current = new Math3D.Point3(ThisVehicle.PosX, ThisVehicle.PosY, ThisVehicle.PosZ);
                    Math3D.Segment3 Segment = new Math3D.Segment3(Previous, Current);
                    Math3D.Point3 Point = new Math3D.Point3(ThisPoint.X, ThisPoint.Y, ThisPoint.Z);
                    double Tolerance = Math3D.Distance(Previous, Current);
                    Tolerance /= (ThisVehicle.TimeStamp - ThisVehicle.Prev_TimeStamp);
                    float HitRadius = (float)ThisVehicle.CachedAircraft.HTRADIUS;
                    if (Tolerance < HitRadius * 5) Tolerance = HitRadius * 5;
                    #endregion
                    if (Math3D.DoesSegmentPassNearPoint(Segment, Point, Tolerance))
                    {
                        int ModulatedCheckPointCount = (ThisRacer.CurrentCheckpoint + 1) % ThisPath.Points.Count;

                        //increment point count!
                        ThisRacer.CurrentCheckpoint++;
                        string CurrentLapTime = "<ERROR>";
                        string CurrentSplitTime = "<ERROR>";
                        #region GET SPLIT/LAP TIME
                        //float starttime = ThisRacer.TimeStamp_LastCheckpoint;
                        float endtime = ThisRacer.TimeStamp_LastCheckpoint;
                        float laptime = 0;
                        float splittime = 0;
                        try
                        {
                            double Distance = Math3D.GetPointSegmentDistanceIfIntersecting(Point, Segment);
                            if (Distance == Double.PositiveInfinity | Distance == Double.NegativeInfinity)
                            {
                                //no intersection???
                                return;
                            }
                            double FractionOfTime = Distance / Segment.Length();
                            endtime = (float)(FractionOfTime * (ThisVehicle.TimeStamp - ThisVehicle.Prev_TimeStamp)) + ThisVehicle.Prev_TimeStamp;

                            TimeSpan StartLapTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(ThisRacer.TimeStamp_LapStarted * 1000));
                            TimeSpan StartSplitTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(ThisRacer.TimeStamp_LastCheckpoint * 1000));
                            TimeSpan EndTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(endtime * 1000));
                            CurrentLapTime = StartLapTimeSpan.GetTimeDifference(EndTimeSpan);
                            laptime = (float)((EndTimeSpan - StartLapTimeSpan).TotalSeconds);
                            splittime = (float)((EndTimeSpan - StartSplitTimeSpan).TotalSeconds);
                            CurrentSplitTime = StartSplitTimeSpan.GetTimeDifference(EndTimeSpan);
                        }
                        catch
                        {
                        }
                        #endregion
                        if (ThisRacer.CumulativeSectorTimeStamps.Count > 0)
                            ThisRacer.CumulativeSectorTimeStamps.Add(splittime + ThisRacer.CumulativeSectorTimeStamps.Last());
                        else ThisRacer.CumulativeSectorTimeStamps.Add(splittime);
                        ThisRacer.SectorTimeStamps.Add(splittime);
                        if (ModulatedCheckPointCount == 0)
                        {
                            //just passed the start/finish line!
                            bool YellowLap = false;
                            bool RedLap = false;

                            if (CurrentLapTime == "00:00:00.000") return;
                            float ThisLapTime = laptime;
                            float LapTimeDifference = ThisLapTime - FastestLap;
                            
                            if (ThisRacer.FastestLap == 0)
                            {
                                ThisRacer.FastestLap = ThisLapTime;
                                if (FastestLap == 0)
                                {
                                    LapTimeDifference = 0;
                                }
                                else
                                {
                                    LapTimeDifference = ThisLapTime - FastestLap;
                                }
                            }

                            string PositionDifference = "<ERROR>";
                            string LapCountString = "<ERROR>";
                            if (FastestLap == 0)
                            {
                                //No fastest lap set yet!
                                FastestLap = ThisLapTime;
                                LapTimeDifference = 0;
                                RedLap = true;
                            }
                            if (LapTimeDifference < 0)
                            {
                                //New Circuit Record!
                                FastestLap = ThisLapTime;
                                RedLap = true;
                                NONRACING.SendMessage("&a" + ThisRacer.Client.Username + " Just set a new lap record on " + Path.Identify);
                                NONRACING.SendMessage("&a    Using a &3" + ThisRacer.Client.Vehicle.Identify);
                                NONRACING.SendMessage("&a    LapTime: &e" + ThisLapTime.AsFormattedTimeDifference().Substring(1) + "&c!");
                                RACING.Exclude(ThisRacer.Client).SendMessage("&a" + ThisRacer.Client.Username + " New lap record: &e" + ThisLapTime.AsFormattedTimeDifference().Substring(1) + "&c!");
                            }
                            if (ThisRacer.FastestLap > ThisLapTime)
                            {
                                //New Personal Best!
                                ThisRacer.FastestLap = ThisLapTime;
                                YellowLap = true;
                            }
                            string LapBonusMarker = "";
                            if (YellowLap) LapBonusMarker = "[&e+&f]";
                            if (RedLap) LapBonusMarker = "[&c#&f]";
                            if (RaceTypeIsTimeTrial())
                            {
                                LapCountString = "L" + ThisRacer.CurrentLap.ToString() + " P" + GetPositionOfRacer(ThisRacer);
                                PositionDifference = LapTimeDifference.AsFormattedTimeDifference();
                                ThisClient.SendMessage(LapCountString + " - " + CurrentLapTime + " (" + PositionDifference + ")" + LapBonusMarker);
                            }
                            if (RaceTypeIsRace())
                            {
                                string LapsRemaining = (TotalLapCount - ThisRacer.CurrentLap).ToString();                                
                                if (ThisRacer.CurrentLap < TotalLapCount)
                                {
                                    LapCountString = "L" + LapsRemaining + " P" + GetPositionOfRacer(ThisRacer);
                                }
                                else
                                {
                                    LapCountString = "P" + GetPositionOfRacer(ThisRacer);
                                }
                                int CurrentPosition = GetPositionOfRacer(ThisRacer);
                                string PosAhead = "";
                                string PosBehind = "";
                                if (CurrentPosition != Racers.Count()) //Not Last
                                {
                                    PosAhead = ThisRacer.RaceTimeGetTimeAheadString();
                                }
                                if (CurrentPosition != 1) //Not First
                                {
                                    PosBehind = ThisRacer.RaceTimeGetTimeBehindString();
                                }

                                if (ThisRacer.CurrentLap < TotalLapCount)
                                {
                                    ThisClient.SendMessage(LapCountString + " - " + CurrentLapTime + LapBonusMarker);
                                }
                                else
                                {
                                    if (GetPositionOfRacer(ThisRacer) == 1)
                                    {
                                        ThisClient.SendMessage("RACE OVER ## " + LapCountString + " - " + ThisRacer.RaceTimeGetTotalTime() + LapBonusMarker + " ## RACE OVER");
                                    }
                                    else
                                    {
                                        float Time = RacersSortedByPosition[0].RaceTimeGetTotalTime();
                                        float Difference = ThisRacer.RaceTimeGetTotalTime() - Time;
                                        string StrDiff = Difference.AsFormattedTimeDifference();

                                        ThisClient.SendMessage("RACE OVER ## " + LapCountString + " - " + ThisRacer.RaceTimeGetTotalTime() + LapBonusMarker + "(" + StrDiff + ")" + " ## RACE OVER");
                                    }
                                }
                                if (PosAhead != "") ThisClient.SendMessage("    " + PosAhead);
                                if (PosBehind != "") ThisClient.SendMessage("    " + PosBehind);
                            }
                            
                            ThisRacer.TimeStamp_LapStarted = endtime;
                            ThisRacer.TimeStamp_LastCheckpoint = endtime;
                            if (GetPositionOfRacer(ThisRacer) == 1 & ThisRacer.CurrentLap == TotalLapCount & TotalLapCount != 0)
                            {
                                //coming first
                                //completed the last lap
                                //not a 0 lap race...
                                //... Means this racer WON the race!

                                RaceEnds = DateTime.Now + new TimeSpan(0, 0, 0, 0, (int)(FastestLap * 1000 * 2)); //double the fastest lap time of the race should be plenty of time for the stragglers to catch up!
                            }
                            ThisRacer.CurrentLap++;
                        }
                        else
                        {
                            //normal checkpoint.

                            bool YellowSector = false;
                            bool RedSector = false;

                            #region FastestSectors
                            if (FastestSectors[ModulatedCheckPointCount] == 0)
                            {
                                //no time set for this sector, set it now!
                                FastestSectors[ModulatedCheckPointCount] = splittime;
                            }

                            if (splittime < FastestSectors[ModulatedCheckPointCount])
                            {
                                //New circuit record sector!
                                FastestSectors[ModulatedCheckPointCount] = splittime;
                            }

                            if (splittime < ThisRacer.FastestSectors[ModulatedCheckPointCount])
                            {
                                //New personal best sector!
                                ThisRacer.FastestSectors[ModulatedCheckPointCount] = splittime;
                            }
                            #endregion

                            #region CumulativeSectors
                            float Difference = laptime - CumulativeSectors[ModulatedCheckPointCount];

                            if (CumulativeSectors[ModulatedCheckPointCount] == 0)
                            {
                                //no time set for this sector, set it now!
                                CumulativeSectors[ModulatedCheckPointCount] = laptime;
                                RedSector = true;
                                Difference = 0;
                            }

                            if (laptime < CumulativeSectors[ModulatedCheckPointCount])
                            {
                                //New circuit record sector!
                                CumulativeSectors[ModulatedCheckPointCount] = laptime;
                                RedSector = true;
                            }

                            if (laptime < ThisRacer.CumulativeSectors[ModulatedCheckPointCount])
                            {
                                //New personal best sector!
                                ThisRacer.CumulativeSectors[ModulatedCheckPointCount] = laptime;
                                YellowSector = true;
                            }
                            #endregion

                            string StrDiff = Difference.AsFormattedTimeDifference();

                            string SectorBonusMarker = "";
                            if (YellowSector) SectorBonusMarker = "[&e+&f]";
                            if (RedSector) SectorBonusMarker = "[&c#&f]";

                            ThisClient.SendMessage("Split " + ModulatedCheckPointCount + " - " + CurrentSplitTime + "(" + StrDiff + ")" + SectorBonusMarker);
                            ThisRacer.TimeStamp_LastCheckpoint = endtime;
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region Race
            public class Race : RaceDescription
            {
                public Race()
                {
                    ID = GetNextRaceID();
                }

                public Race(List<_Racer> _SortedList)
                {
                    ID = GetNextRaceID();
                    int i = 1;
                    foreach (_Racer ThisRacer in _SortedList)
                    {
                        _Racer NewRacer = AddRacer(ThisRacer.Client);
                        if (NewRacer == null)
                        {
                            //race not in stopped state!
                            return;
                        }
                        NewRacer.QualifyingPosition = i;
                        i++;
                    }
                }
            }
            public static List<Race> Races = new List<Race>();
            #endregion

            public static bool Update(Client ThisClient)
            {
                Race[] _Races = new Race[]{ };
                lock (Races)
                {
                    _Races = Races.ToArray();
                }
                foreach (Race ThisRace in Races.Where(x=>x.Racers.Select(y=>y.Client).Contains(ThisClient)))
                {
                    foreach (RaceDescription._Racer ThisRacer in ThisRace.Racers.Where(x=>x.Client == ThisClient))
                    {
                        ThisRace.CheckIntersections(ThisRacer);
                    }
                }
                return true;
            }
        }
    }
}
