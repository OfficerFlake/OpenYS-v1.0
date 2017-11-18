using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenYS
{
    public static partial class Games
    {
        public static class Racing
        {
            public class CheckPointDescriptor
            {
                public float TimeStamp = 0;
                public World.Objects.Path._Point CheckPoint = null;
                public Vehicle Vehicle = Vehicles.NoVehicle;
                
                public CheckPointDescriptor(Vehicle _ThisVehicle, World.Objects.Path._Point _CheckPoint)
                {
                    CheckPoint = _CheckPoint;
                    Vehicle = _ThisVehicle;
                    World.Objects.Path._Point ThisPoint = _CheckPoint;
                    Math3D.Point3 Previous = new Math3D.Point3(_ThisVehicle.Prev_PosX, _ThisVehicle.Prev_PosY, _ThisVehicle.Prev_PosZ);
                    Math3D.Point3 Current = new Math3D.Point3(_ThisVehicle.PosX, _ThisVehicle.PosY, _ThisVehicle.PosZ);
                    Math3D.Segment3 Segment = new Math3D.Segment3(Previous, Current);
                    Math3D.Point3 Point = new Math3D.Point3(ThisPoint.X, ThisPoint.Y, ThisPoint.Z);
                    double Tolerance = Math3D.Distance(Previous, Current);
                    Tolerance /= (_ThisVehicle.TimeStamp - _ThisVehicle.Prev_TimeStamp);
                    float HitRadius = (float)_ThisVehicle.CachedAircraft.HTRADIUS;
                    if (Tolerance < HitRadius * 5) Tolerance = HitRadius * 5;
                    if (Math3D.DoesSegmentPassNearPoint(Segment, Point, Tolerance))
                    {
                        Math3D.Point3 ClosestPoint = Math3D.GetPointSegmentClosestPointIfIntersecting(Point, Segment);
                        if (ClosestPoint == null)
                        {
                            //Doesn't even intersect! Let's say it crossed this point at the last second!
                            TimeStamp = _ThisVehicle.TimeStamp;
                        }
                        else
                        {
                            //Does intersect! find the exact millisecond the intersection took place!
                            double Numerator = Math3D.Distance(Previous, ClosestPoint);
                            double Denominator = Math3D.Distance(Previous, Current);
                            float TimeSpanDifference = _ThisVehicle.TimeStamp - _ThisVehicle.Prev_TimeStamp;
                            TimeStamp = _ThisVehicle.Prev_TimeStamp + (float)((Numerator / Denominator) * TimeSpanDifference);
                        }
                    }
                }
            }

            public static List<CheckPointDescriptor> ActiveCheckPoints = new List<CheckPointDescriptor>();

            public static void CheckIntersections(Client ThisClient)
            {
                Vehicle ThisVehicle = ThisClient.Vehicle;
                if (ThisVehicle == Vehicles.NoVehicle) return;
                for (int j = 0; j < World.Objects.PathList.Count; j++)
                {
                    World.Objects.Path ThisPath = World.Objects.PathList[j];
                    for (int k = ThisPath.Points.Count - 1; k >= 0 ; k--)
                    {
                        #region Segment Testings Maths...
                        World.Objects.Path._Point ThisPoint = ThisPath.Points[k];
                        if (!ActiveCheckPoints.Select(x => x.CheckPoint).Contains(ThisPath.Points[0]) & ThisPoint != ThisPath.Points[0]) continue;
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
                            lock (ActiveCheckPoints)
                            {
                                List<World.Objects.Path._Point> Intersection = ActiveCheckPoints.Select(x => x.CheckPoint).Intersect(ThisPath.Points).ToList();
                                if (ThisPath.Points.Count > Intersection.Count)
                                {
                                    if (ThisPath.Points[Intersection.Count] == ThisPoint) //Allow to go forwards or backwards on the track!
                                    {
                                        #region Add the new point.
                                        ActiveCheckPoints.Add(new CheckPointDescriptor(ThisVehicle, ThisPoint));
                                        #endregion
                                        #region Update the data-set intersection
                                        Intersection = ActiveCheckPoints.Select(x => x.CheckPoint).Intersect(ThisPath.Points).ToList();
                                        #endregion
                                        #region GET SPLIT TIME
                                        string SplitTimeResult = "<ERROR>";
                                        try
                                        {
                                            float starttime = 0;
                                            float endtime = 0;

                                            if (ActiveCheckPoints.Count > 0)
                                            {
                                                starttime = ActiveCheckPoints.Where(x => x.Vehicle == ThisVehicle).Where(y => y.CheckPoint == Intersection[0]).ToArray()[0].TimeStamp;
                                                endtime = ActiveCheckPoints.Where(x => x.Vehicle == ThisVehicle).Where(y => y.CheckPoint == ThisPoint).ToArray()[0].TimeStamp;
                                            }
                                            else
                                            {
                                                starttime = ThisVehicle.TimeStamp;
                                                endtime = ThisVehicle.TimeStamp;
                                            }

                                            TimeSpan StartTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(starttime * 1000));
                                            TimeSpan EndTimeSpan = new TimeSpan(0, 0, 0, 0, (int)(endtime * 1000));
                                            TimeSpan LapTime = EndTimeSpan - StartTimeSpan;
                                            string Hours = LapTime.Hours.ToString();
                                            while (Hours.Length < 2) Hours = "0" + Hours;
                                            if (Hours.Length > 2) Hours = "99";

                                            string Minutes = LapTime.Minutes.ToString();
                                            while (Minutes.Length < 2) Minutes = "0" + Minutes;
                                            if (Minutes.Length > 2) Minutes = "99";

                                            string Seconds = LapTime.Seconds.ToString();
                                            while (Seconds.Length < 2) Seconds = "0" + Seconds;
                                            if (Seconds.Length > 2) Seconds = "99";

                                            string Milliseconds = LapTime.Milliseconds.ToString();
                                            while (Milliseconds.Length < 3) Milliseconds = "0" + Milliseconds;
                                            if (Milliseconds.Length > 3) Milliseconds = "999";

                                            SplitTimeResult = Hours + ":" + Minutes + ":" + Seconds + "." + Milliseconds;
                                        }
                                        catch
                                        {
                                        }
                                        #endregion
                                        if (SplitTimeResult == "00:00:00.000") continue; //ThisClient.SendMessage(ThisPath.Identify + " Split " + (k - 1) + " -  " + SplitTimeResult);
                                        else
                                        {
                                            if (ThisPoint == ThisPath.Points[ThisPath.Points.Count - 1] & Intersection.Count == ThisPath.Points.Count) //Finish Line!
                                            {
                                                ThisClient.SendMessage("[" + ThisPath.Identify + "]" + " Lap - " + SplitTimeResult);
                                            }
                                            else
                                            {
                                                ThisClient.SendMessage("[" + ThisPath.Identify + "]" + " Split " + k + " - " + SplitTimeResult);
                                            }
                                        }
                                        
                                    }
                                }
                                if (ThisPoint == ThisPath.Points[ThisPath.Points.Count - 1] & Intersection.Count == ThisPath.Points.Count)
                                {
                                    ActiveCheckPoints.OrderBy(x => x.TimeStamp);
                                    foreach (World.Objects.Path._Point ThisUnionPoint in Intersection)
                                    {
                                        CheckPointDescriptor CheckPointToRemove = ActiveCheckPoints.Where(x => x.CheckPoint == ThisUnionPoint).Where(y => y.Vehicle == ThisVehicle).ToArray()[0];
                                        ActiveCheckPoints.Remove(CheckPointToRemove);
                                    }
                                    Intersection = ActiveCheckPoints.Select(x => x.CheckPoint).Intersect(ThisPath.Points).ToList(); //need to update this!
                                }
                                if (ThisPoint == ThisPath.Points[0] & Intersection.Count == 1)
                                {
                                    //ThisClient.SendMessage("START LAP" + ActiveCheckPoints.Count.ToString());
                                    //ActiveCheckPoints.Add(new CheckPointDescriptor(ThisVehicle, ThisPoint));
                                    //continue;
                                    //k = 0;  //Restart the check, we may have passed another checkpoint a second ago!
                                }
                                
                            }
                        }
                    }
                }
            }

            public static void OnTenMinuteUpdateEvent()
            {
                lock (Clients.AllClients)
                {
                    lock (ActiveCheckPoints) {
                        foreach (CheckPointDescriptor ThisCheckpoint in ActiveCheckPoints)
                        {
                            if (Clients.AllClients.Where(x => x.Vehicle == ThisCheckpoint.Vehicle).Count() == 0)
                            {
                                ActiveCheckPoints.Remove(ThisCheckpoint);
                            }
                        }
                    }
                }
            }

            public static void Initialise()
            {
                //Console.WriteLine("&3[&6GAMES&3]&7Racing Initialised.");
                OpenYS.Events_OnMacroTickUpdate.Add(OnTenMinuteUpdateEvent);
            }
        }
    }
}
