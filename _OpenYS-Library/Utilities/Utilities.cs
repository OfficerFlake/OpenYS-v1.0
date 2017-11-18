using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace OpenYS
{
    #region Threads
    public static partial class Threads
    {
        #region _List (Non Thread Safe)
        /// <summary>
        /// Non-Thread-Safe list of all OYS Threads.
        /// </summary>
        private static List<Thread> _List = new List<Thread>();
        #endregion
        #region Thread Locker
        /// <summary>
        /// Thread-Safe Lock for manging the threads list.
        /// </summary>
        private static ReaderWriterLockSlim _ThreadListLock = new ReaderWriterLockSlim();
        #endregion
        #region List (Thread Safe)
        /// <summary>
        /// Thread-Safe list of OYS Threads.
        /// </summary>
        public static List<Thread> List
        {
            get
            {
                try
                {
                    _ThreadListLock.EnterReadLock();
                    return _List;
                }
                finally {
                    _ThreadListLock.ExitReadLock();
                }
            }
            set
            {
                try
                {
                    _ThreadListLock.EnterWriteLock();
                    _List = value;
                }
                finally
                {
                    _ThreadListLock.ExitWriteLock();
                }
            }
        }
        #endregion

        #region Run
        /// <summary>
        /// Starts the given method on another thread and monitors thread for safety.
        /// </summary>
        /// <param name="Input">Thread to run. Use "() => MethodToRun(Args)"</param>
        /// <returns>True if thread started ok.</returns>
        private static bool Run(Thread Input)
        {
            try
            {
                //Console.WriteLine("RUN: " + Input.Name);
                Input.Start();
                return true;
            }
            catch
            {
                //Console.WriteLine("FAIL: " + Input.Name);
                return false;
            }
        }
        #endregion
        #region Add
        /// <summary>
        /// Starts the given method on another thread and monitors thread for safety.
        /// </summary>
        /// <param name="Input">Thread to run. Use "() => MethodToRun(Args)"</param>
        /// <param name="Name">Name of the new thread.</param>
        /// <returns>ID of new thread spawned.</returns>
        public static Thread Add(Action funcToRun, string Name)
        {
            Thread NewThread = new Thread(() =>
                {
                    #region TRY:
#if RELEASE
                    try
                    {
#endif
                    #endregion
                        funcToRun();
                    #region CATCH
#if RELEASE
                    }
                    catch (Exception e)
                    {
                        Emailing.SendBugReportEmail(e);
                    }
#endif
                    #endregion
                });
            NewThread.Name = Name;
            List.Add(NewThread);
            if (!Run(NewThread))
            {
                Debug.WriteLine("Failed to Spawn Thread: " + Name);
                return Thread.CurrentThread;
            }
            else return NewThread;
        }
        #endregion
        #region Prepare
        /// <summary>
        /// Prepares the given method on another thread to be run.
        /// </summary>
        /// <param name="Input">Thread to run. Use "() => MethodToRun(Args)"</param>
        /// <param name="Name">Name of the new thread.</param>
        /// <returns>ID of new thread spawned.</returns>
        public static Thread Prepare(Action funcToRun, string Name)
        {
            Thread NewThread = new Thread(() => funcToRun());
            NewThread.Name = Name;
            List.Add(NewThread);
            return NewThread;
        }
        #endregion

        #region GenericThreadSafeLock
        /// <summary>
        /// This object can be used to lock any method for use of only one thread at a time.
        /// </summary>
        public static object GenericThreadSafeLock = new object();
        #endregion
    }
    #endregion

    #region Math2D
    public static partial class Math2D
    {
        public class Point2
        {
            //Properties
            #region public double X
            /// <summary>
            /// X value of the point.
            /// </summary>
            public double X = 0;
            #endregion
            #region public double Y
            /// <summary>
            /// Y value of the point.
            /// </summary>
            public double Y = 0;
            #endregion

            //Contructors
            #region Point2 (X, Y)
            /// <summary>
            /// Constructs a point based on the 3 scalar positions.
            /// </summary>
            /// <param name="_X">Magnitude of Position on X Axis.</param>
            /// <param name="_Y">Magnitude of Position on Y Axis.</param>
            public Point2(double _X, double _Y)
            {
                X = _X;
                Y = _Y;
            }
            #endregion
            #region Point2 ("X Y")
            /// <summary>
            /// Constructs a Point from a string representation of the Points base axis positional components.
            /// </summary>
            /// <param name="_XY">"X Y"</param>
            public Point2(string _XY)
            {
                //"X Y Z"
                X = Byte.Parse(_XY.Split(' ')[0]);
                Y = Byte.Parse(_XY.Split(' ')[1]);
            }
            #endregion

            //Conversions
            #region ToString (Override)
            /// <summary>
            /// Represents the Point as a string
            /// </summary>
            /// <returns>A string of the format "X Y".</returns>
            public override string ToString()
            {
                return X.ToString() + " " + Y.ToString();
            }
            #endregion

            //Operators
            #region ~ Operators ~
            #endregion
        }
    }
    #endregion

    #region Math3D
    public static partial class Math3D
    {
        public class Point3
        {
            //Properties
            #region public double X
            /// <summary>
            /// X value of the point.
            /// </summary>
            public double X = 0;
            #endregion
            #region public double Y
            /// <summary>
            /// Y value of the point.
            /// </summary>
            public double Y = 0;
            #endregion
            #region public double Z
            /// <summary>
            /// Z value of the point.
            /// </summary>
            public double Z = 0;
            #endregion

            //Contructors
            #region Point3 (X, Y, Z)
            /// <summary>
            /// Constructs a point based on the 3 scalar positions.
            /// </summary>
            /// <param name="_X">Magnitude of Position on X Axis.</param>
            /// <param name="_Y">Magnitude of Position on Y Axis.</param>
            /// <param name="_Z">Magnitude of Position on Z Axis.</param>
            public Point3(double _X, double _Y, double _Z)
            {
                X = _X;
                Y = _Y;
                Z = _Z;
            }
            #endregion
            #region Point3 ("X Y Z")
            /// <summary>
            /// Constructs a Point from a string representation of the Points base axis positional components.
            /// </summary>
            /// <param name="_XYZ">"X Y Z"</param>
            public Point3(string _XYZ)
            {
                //"X Y Z"
                X = Byte.Parse(_XYZ.Split(' ')[0]);
                Y = Byte.Parse(_XYZ.Split(' ')[1]);
                Z = Byte.Parse(_XYZ.Split(' ')[2]);
            }
            #endregion

            //Conversions
            #region ToString (Override)
            /// <summary>
            /// Represents the Vector as a string
            /// </summary>
            /// <returns>A string of the format "X Y Z".</returns>
            public override string ToString()
            {
                return X.ToString() + " " + Y.ToString() + " " + Z.ToString();
            }
            #endregion
            #region ToVector3
            /// <summary>
            /// Represents the Point as a Vector
            /// </summary>
            /// <returns>A Vector where X is the Point X component, Y is the Point Y component, and Z is the Point Z component.</returns>
            public Vector3 ToVector3()
            {
                return new Vector3(this.X, this.Y, this.Z);
            }
            #endregion

            //Operators
            #region ~ Operators ~
            public static Point3 operator +(Point3 V0, Point3 V1)
            {
                return new Point3(V0.X + V1.X, V0.Y + V1.Y, V0.Z + V1.Z);
            }
            public static Point3 operator -(Point3 V0, Point3 V1)
            {
                return new Point3(V0.X - V1.X, V0.Y - V1.Y, V0.Z - V1.Z);
            }
            public static Point3 operator +(Point3 P0, Vector3 V1)
            {
                return new Point3(P0.X + V1.X, P0.Y + V1.Y, P0.Z + V1.Z);
            }
            public static Point3 operator -(Point3 P0, Vector3 V1)
            {
                return new Point3(P0.X - V1.X, P0.Y - V1.Y, P0.Z - V1.Z);
            }
            #endregion
        }
        public class Vector3
        {
            //Properties
            #region public double X
            /// <summary>
            /// X value of the vector.
            /// </summary>
            public double X = 0;
            #endregion
            #region public double Y
            /// <summary>
            /// Y value of the vector.
            /// </summary>
            public double Y = 0;
            #endregion
            #region public double Z
            /// <summary>
            /// Z value of the vector.
            /// </summary>
            public double Z = 0;
            #endregion
            
            //Constructors
            #region Vector3 (X, Y, Z)
            /// <summary>
            /// Constructs a vector based on the 3 scalar axes.
            /// </summary>
            /// <param name="_X">Magnitude of Vector on X Axis.</param>
            /// <param name="_Y">Magnitude of Vector on Y Axis.</param>
            /// <param name="_Z">Magnitude of Vector on Z Axis.</param>
            public Vector3(double _X, double _Y, double _Z)
            {
                X = _X;
                Y = _Y;
                Z = _Z;
            }
            #endregion
            #region Vector3 ("X Y Z")
            /// <summary>
            /// Constructs a vector from a string representation of the vectors base axis scalar components.
            /// </summary>
            /// <param name="_XYZ">"X Y Z"</param>
            public Vector3(string _XYZ)
            {
                if (_XYZ.Split(' ').Count() < 3)
                {
                    throw new ArgumentException("Vector not in \"X Y Z\" format!");
                }
                //"X Y Z"
                X = Double.Parse(_XYZ.Split(' ')[0]);
                Y = Double.Parse(_XYZ.Split(' ')[1]);
                Z = Double.Parse(_XYZ.Split(' ')[2]);
            }
            #endregion

            //Methods
            #region XAngle
            /// <summary>
            /// Returns the angle of the vector along the X axis in radians.
            /// </summary>
            /// <returns>The Angle from the X Axis, in Radians.</returns>
            public double XAngle()
            {
                //X/Z
                return Math.Atan2(this.X, this.Z);
            }
            #endregion
            #region YAngle
            /// <summary>
            /// Returns the angle of the vector along the Y axis in radians.
            /// </summary>
            /// <returns>The Angle from the Y Axis, in Radians.
            public double YAngle()
            {
                //X/Z
                return Math.Atan2(this.Y, (new Vector3(this.X, 0, this.Z)).Length());
            }
            #endregion
            #region Length
            /// <summary>
            /// Returns the Scalar component of the Vector - that is, the Magnitude of the vector.
            /// </summary>
            /// <returns>Magnitude of the Vector.</returns>
            public double Length()
            {
                return Math.Sqrt
                    (
                        Math.Pow((X), 2) +
                        Math.Pow((Y), 2) +
                        Math.Pow((Z), 2)
                    );
            }
            #endregion
            #region Normalise
            /// <summary>
            /// Normalises the vector
            /// </summary>
            /// <returns>The same directional vector where the length is now 1.00</returns>
            public Vector3 Normalise()
            {
                //return the same vector direction but of total unit length 1;
                double Length = this.Length();
                return new Vector3
                    (
                        this.X / Length,
                        this.Y / Length,
                        this.Z / Length
                    );
            }
            #endregion

            //Conversions
            #region ToString (Override)
            /// <summary>
            /// Represents the Vector as a string
            /// </summary>
            /// <returns>A string of the format "X Y Z".</returns>
            public override string ToString()
            {
                return X.ToString() + " " + Y.ToString() + " " + Z.ToString();
            }
            #endregion
            #region ToPoint3
            /// <summary>
            /// Represents the Vector as a point
            /// </summary>
            /// <returns>A point where X is the vectors X component, Y is the vectors Y component, and Z is the vectors Z component.</returns>
            public Point3 ToPoint3()
            {
                return new Point3(this.X, this.Y, this.Z);
            }
            #endregion

            //Operators
            #region ~ Operators ~
            public static Vector3 operator +(Vector3 V0, Vector3 V1)
            {
                return new Vector3(V0.X + V1.X, V0.Y + V1.Y, V0.Z + V1.Z);
            }
            public static Vector3 operator -(Vector3 V0, Vector3 V1)
            {
                return new Vector3(V0.X - V1.X, V0.Y - V1.Y, V0.Z - V1.Z);
            }
            public static Vector3 operator *(double Base, Vector3 Input)
            {
                return new Vector3(Input.X * Base, Input.Y * Base, Input.Z * Base);
            }
            public static Vector3 operator /(double Base, Vector3 Input)
            {
                return new Vector3(Input.X / Base, Input.Y / Base, Input.Z / Base);
            }
            #endregion
        }
        public class Attitude3
        {
            //Properties
            #region public double H
            /// <summary>
            /// H value of the Attitude.
            /// </summary>
            public double H = 0;
            #endregion
            #region public double P
            /// <summary>
            /// P value of the Attitude.
            /// </summary>
            public double P = 0;
            #endregion
            #region public double B
            /// <summary>
            /// B value of the Attitude.
            /// </summary>
            public double B = 0;
            #endregion

            //Contructors
            #region Attitude3 (H, P, B)
            /// <summary>
            /// Constructs a point based on the 3 scalar positions.
            /// </summary>
            /// <param name="_H">Magnitude of Angle on H Axis.</param>
            /// <param name="_P">Magnitude of Angle on P Axis.</param>
            /// <param name="_B">Magnitude of Angle on B Axis.</param>
            public Attitude3(double _H, double _P, double _B)
            {
                H = _H;
                P = _P;
                B = _B;
            }
            #endregion
            #region Attitude3 ("H P B")
            /// <summary>
            /// Constructs an Attitude from a string representation of the Attitudes base angle scalar components.
            /// </summary>
            /// <param name="_HPB">"H P B"</param>
            public Attitude3(string _HPB)
            {
                //"X Y Z"
                H = Byte.Parse(_HPB.Split(' ')[0]);
                P = Byte.Parse(_HPB.Split(' ')[1]);
                B = Byte.Parse(_HPB.Split(' ')[2]);
            }
            #endregion

            //Conversions
            #region ToString (Override)
            /// <summary>
            /// Represents the Vector as a string
            /// </summary>
            /// <returns>A string of the format "H P B".</returns>
            public override string ToString()
            {
                return H.ToString() + " " + P.ToString() + " " + B.ToString();
            }
            #endregion

            //Operators
            #region ~ Operators ~
            //Add angular operators here.
            #endregion
        }
        public class Orientation3
        {
            //Properties
            public Point3 Position;
            public Attitude3 Attitude;

            //Constructors
            #region Orientation3 (X, Y, Z, H, P, B)
            /// <summary>
            /// Constructs an orientation based on the 3 scalar axes and 3 scalar angles.
            /// </summary>
            /// <param name="_X">Position of Orientation on X Axis.</param>
            /// <param name="_Y">Position of Orientation on Y Axis.</param>
            /// <param name="_Z">Position of Orientation on Z Axis.</param>
            public Orientation3(double _X, double _Y, double _Z, double _H, double _P, double _B)
            {
                Position = new Point3(_X, _Y, _Z);
                Attitude = new Attitude3(_H, _P, _B);
            }
            #endregion
            #region Orientation3 ("X Y Z H P B")
            /// <summary>
            /// Constructs an orientation from a string representation of the orientations base axis/angle scalar components.
            /// </summary>
            /// <param name="_XYZ">"X Y Z H P B"</param>
            public Orientation3(string _XYZHPB)
            {
                if (_XYZHPB.Split(' ').Count() < 6)
                {
                    throw new ArgumentException("Vector not in \"X Y Z H P B\" format!");
                }
                //"X Y Z H P B"
                Position = new Point3(0, 0, 0);
                Attitude = new Attitude3(0, 0, 0);
                Position.X = Double.Parse(_XYZHPB.Split(' ')[0]);
                Position.Y = Double.Parse(_XYZHPB.Split(' ')[1]);
                Position.Z = Double.Parse(_XYZHPB.Split(' ')[2]);
                Attitude.H = Double.Parse(_XYZHPB.Split(' ')[3]);
                Attitude.P = Double.Parse(_XYZHPB.Split(' ')[4]);
                Attitude.B = Double.Parse(_XYZHPB.Split(' ')[5]);
            }
            #endregion

            //Conversions
            #region ToString (Override)
            /// <summary>
            /// Represents the Vector as a string
            /// </summary>
            /// <returns>A string of the format "X Y Z H P B".</returns>
            public override string ToString()
            {
                return Position.X.ToString() + " " + Position.Y.ToString() + " " + Position.Z.ToString() + Attitude.H.ToString() + " " + Attitude.P.ToString() + " " + Attitude.B.ToString();
            }
            #endregion

            //Operators
            #region ~ Operators ~
            //Add Angular and Scalar operators here.
            #endregion
        }
        public class Segment3
        {
            //Properties
            #region public Point3 P0
            /// <summary>
            /// The start point of the segment.
            /// </summary>
            public Point3 P0 = new Point3(0, 0, 0);
            #endregion
            #region public Point3 P1
            /// <summary>
            /// The end point of the segment.
            /// </summary>
            public Point3 P1 = new Point3(0, 0, 0);
            #endregion
            

            //Constructors
            #region Segment3 (P0, P1)
            /// <summary>
            /// Constructs a Line Segment where P0 is the start point of the line segment and P1 is the end point of the line segment.
            /// </summary>
            /// <param name="_P0">The Start of the Line Segment.</param>
            /// <param name="_P1">The End of the Line Segment.</param>
            public Segment3(Point3 _P0, Point3 _P1)
            {
                P0 = _P0;
                P1 = _P1;
            }
            #endregion

            //Conversions
            #region ToString (Override)
            /// <summary>
            /// Represents the Segment as a string
            /// </summary>
            /// <returns>A string of the format "(P0) => (P1)".</returns>
            public override string ToString()
            {
                return "(" + P0.ToString() + ")" + " => " + "(" + P1.ToString() + ")";
            }
            #endregion

            //Methods
            #region Length
            public double Length()
            {
                return Distance(P0, P1);
            }
            #endregion
        }

        #region GeneratePlatonicSolidVertecies
        /// <summary>
        /// Creates an array of vectors radiating away from a centerpoint.
        /// 
        /// Works by creating a basic isometric cube, then interpolating the edges over each pass.
        /// The points on the edges are then normalised before the process is repeated.
        /// 
        /// The end result would be an exponentially complex ball of points radiating away from the centre at equal distance (a sphere)
        /// </summary>
        /// <param name="_Complexity">How many subdivisions the generated polyhedron will have
        /// 
        /// Warning: Exponential! Reccommend 2 units!</param>
        /// <returns></returns>
        public static Vector3[] GeneratePlatonicSolidVertecies(int _Complexity)
        {
            //generate a primite icosphere, then subdivide for complexity
            List<Point3> Points = new List<Point3>()
            {
                //Generic OctoHedron (complexity 1)
                new Point3(-1, 0, 0),
                new Point3( 1, 0, 0),
                new Point3( 0,-1, 0),
                new Point3( 0, 1, 0),
                new Point3( 0, 0,-1),
                new Point3( 0, 0, 1)
            };
            for (int i = 0; i < _Complexity-1; i++)
            {
                //repeat for complexity
                Point3[] OriginalPoints = Points.ToArray();
                double EdgeLength = Math.Sqrt(Math.Pow((6 / OriginalPoints.Count()),2)*2);
                for (int j = 0; j < OriginalPoints.Count(); j++)
                {
                    //for each vertex
                    for (int k = 0; k < OriginalPoints.Count(); k++)
                    {
                        if (OriginalPoints[k] == OriginalPoints[j]) continue; //if the same vertex, ignore.
                        //test against every other vertex.
                        if (Distance(OriginalPoints[k], OriginalPoints[j]) > EdgeLength*1.1) continue; //Edge length is NOT within spec tolerance.
                        if (Distance(OriginalPoints[k], OriginalPoints[j]) < EdgeLength*0.9) continue; //Edge length is NOT within spec tolerance.

                        //Edge length WILL be within spec tolerance, Add the point.
                        Points.Add
                            (
                            new Point3
                                (
                                    (OriginalPoints[k].X + OriginalPoints[j].X)/2,
                                    (OriginalPoints[k].Y + OriginalPoints[j].Y)/2,
                                    (OriginalPoints[k].Z + OriginalPoints[j].Z)/2
                                )
                            );
                    }
                    //That vertex has passed the test and has been processed.
                }
                //All verticies have been subdivded at this point.
                
                //remove all duplicates.
                Points = Points.Distinct().ToList();

                List<Point3> temporaryPoints = new List<Point3>();
                //normalise all the vertecies
                for (int j = 0; j < Points.Count(); j++)
                {
                    Vector3 AsVector = new Vector3(Points[j].X, Points[j].Y, Points[j].Z);
                    AsVector = AsVector.Normalise();
                    temporaryPoints.Add
                        (
                            new Point3(AsVector.X, AsVector.Y, AsVector.Z)
                        );
                }
                Points = temporaryPoints;
            }
            //finally done! Convert to Vectors!
            List<Vector3> VectorOutput = new List<Vector3>();
            //normalise all the vertecies
            for (int j = 0; j < Points.Count(); j++)
            {
                Vector3 AsVector = new Vector3(Points[j].X, Points[j].Y, Points[j].Z);
                VectorOutput.Add(AsVector);
            }
            return VectorOutput.ToArray();
        }
        #endregion

        #region GetPointSegmentDistance
        /// <summary>
        /// Uses dot products to find the closest point along Segment S to point P.
        /// 
        /// Unlike the method "GetPointSegmentDistanceIfIntersecting",
        ///     this method will return the end points of the line
        ///     if the closest computed segment is NOT on the line.
        /// </summary>
        /// <param name="P">The point of interest, P</param>
        /// <param name="S">The line segment to test, S</param>
        /// <returns>the closest point along the segment to P</returns>
        public static double GetPointSegmentDistance(Point3 P, Segment3 S)
        {
            Vector3 v = (S.P1 - S.P0).ToVector3();
            Vector3 w = (P - S.P0).ToVector3();

            double c1 = Dot(w, v);
            if (c1 <= 0)
                return Distance(P, S.P0);

            double c2 = Dot(v, v);
            if (c2 <= c1)
                return Distance(P, S.P1);

            double b = c1 / c2;
            Point3 Pb = S.P0 + b * v;
            return Distance(P, Pb);
        }
        #endregion

        #region DoesSegmentPassNearPoint
        /// <summary>
        /// Returns true if the line segment S passes within tolerance T meters of point P
        /// </summary>
        /// <param name="S">The line segment to measure from</param>
        /// <param name="P">The point to measure to</param>
        /// <param name="Tolerance">The tolerance in meters</param>
        /// <returns>True if D less then or equal to T.</returns>
        public static bool DoesSegmentPassNearPoint(Segment3 S, Point3 P, double Tolerance)
        {
            double TestResult = GetPointSegmentDistanceIfIntersecting(P, S);
            if (TestResult == Double.NegativeInfinity | TestResult == Double.PositiveInfinity) return false; //no intersection.
            return (TestResult <= Tolerance); //there IS an intersection on the plane, however, is it close enough?
        }
        #endregion

        #region GetPointSegmentDistanceIfIntersecting
        /// <summary>
        /// Uses dot products to find the closest point along Segment S to point P.
        /// 
        /// if the closest point along the infinte line extended along S is not located on S,
        /// Positive/Negative infinity is thrown.
        /// </summary>
        /// <param name="P">The point to measure to.</param>
        /// <param name="S">The line segment to test.</param>
        /// <returns></returns>
        public static double GetPointSegmentDistanceIfIntersecting(Point3 P, Segment3 S)
        {
            #region Vector3 V = Reverse Vector S.
            Vector3 v = (S.P1 - S.P0).ToVector3();
            #endregion
            #region Vector3 W = Vector P to S0
            Vector3 w = (P - S.P0).ToVector3();
            #endregion

            #region void Pb = Closest point on Segment S to P.
            #endregion

            #region double C1 = Distance from S0 to Pb.
            //If C1 is less then 0, then the intercept is NOT on the line segment, it is before it.
            //in that case we say the line segment is does not pass near P, and return Neg. Inf.
            double c1 = Dot(w, v);
            if (c1 <= 0)
                return Double.NegativeInfinity;
            #endregion
            #region double C2 = Length of S.
            //if C2 < C1, then the know that the closest point is FARTHER along the line segment,
            //in this case, we return Pos. Inf.
            double c2 = Dot(v, v);
            if (c2 <= c1)
                return Double.PositiveInfinity;
            #endregion

            #region B = Distance along S from S0 the point Pb is located.
            double b = c1 / c2;
            #endregion
            #region Distance to point is given the length of vector Pb to P.
            Point3 Pb = S.P0 + b * v;
            #endregion
            return Distance(P, Pb);
        }
        #endregion

        #region GetPointSegmentClosestPointIfIntersecting
        /// <summary>
        /// This method is just like "GetPointSegmentDistanceIfIntersecting"
        ///     but it returns the closest point instead of the distance between the points.
        ///     
        /// In this case, if the point is NOT on the segment, null is thrown.
        /// </summary>
        /// <param name="P">Point of interest, P.</param>
        /// <param name="S">Segment to test, S.</param>
        /// <returns>Closest point if intercept exists, else null</returns>
        public static Point3 GetPointSegmentClosestPointIfIntersecting(Point3 P, Segment3 S)
        {
            Vector3 v = (S.P1 - S.P0).ToVector3();
            Vector3 w = (P - S.P0).ToVector3();

            double c1 = Dot(w, v);
            if (c1 <= 0)
                return null;

            double c2 = Dot(v, v);
            if (c2 <= c1)
                return null;

            double b = c1 / c2;
            Point3 Pb = S.P0 + b * v;
            return Pb;
        }
        #endregion

        #region Dot Product
        /// <summary>
        /// Returns the dot product of the two vectors.
        /// </summary>
        /// <param name="U">Vector 1, U</param>
        /// <param name="V">Vector 2, V</param>
        /// <returns>Dot product of both Vectors.</returns>
        public static double Dot(Vector3 U, Vector3 V)
        {
            return ((U).X * (V).X + (U).Y * (V).Y + (U).Z * (V).Z);
        }
        #endregion
        #region Point Distance
        /// <summary>
        /// Returns the distance between two points
        /// </summary>
        /// <param name="U">Point 1</param>
        /// <param name="V">Point 2</param>
        /// <returns>Distance between the two points.</returns>
        public static double Distance(Point3 U, Point3 V)
        {
            return Math.Sqrt
                (
                    Math.Pow((U.X - V.X), 2) +
                    Math.Pow((U.Y - V.Y), 2) +
                    Math.Pow((U.Z - V.Z), 2)
                );
        }
        #endregion
    }
    #endregion

    #region Environment
    public static partial class Environment
    {
        #region Command Line Arguments
        /// <summary>
        /// The relevent arguments passed to OYS / to pass to OYS if it is restarted.
        /// </summary>
        public static string[] CommandLineArguments = { };
        #endregion
        #region Entry Assembly
        /// <summary>
        /// Used in calculation of other variables - this is the .exe running OYS at the moment!
        /// </summary>
        public static Assembly EntryAssembly = Assembly.GetEntryAssembly();
        #endregion
        #region Executable Name
        /// <summary>
        /// This is the filename of the .exe running OYS at the moment - used when calling restarts!
        /// </summary>
        public static string ExecutableName
        {
            get
            {
                return Path.GetFileNameWithoutExtension(EntryAssembly.Location);
            }
        }
        #endregion
        #region Get Build Version
        /// <summary>
        /// Gets the build version of OYS.
        /// </summary>
        /// <returns>Returns Buildversion or "MISSINGNO." if that fails.</returns>
        public static string GetCompilationDate()
        {
            try
            {
                var resourceName = "OpenYS.BuildVersion.txt";

                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd().ReplaceAll("\r", "").ReplaceAll("\n", "");
                        return result;
                    }
                }
            }
            catch
            {
                return "MISSINGNO.";
            }
        }
        #endregion
        #region Get API Key
        /// <summary>
        /// Gets the API Key of this release of OYS.
        /// </summary>
        /// <returns>Returns APIKey or "MISSINGNO." if that fails.</returns>
        public static string GetAPIKey()
        {
            try
            {
                var resourceName = "OpenYS.APIKey.txt";

                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd().ReplaceAll("\r", "").ReplaceAll("\n", "");
                        return result;
                    }
                }
            }
            catch
            {
                return "MISSINGNO.";
            }
        }
        #endregion
        #region External IP Address
        private static IPAddress _IP = IPAddress.Parse("127.0.0.1");
        public static IPAddress ExternalIP
        {
            get
            {
                if (_IP.ToString() != "127.0.0.1") return _IP;
                else
                {
                    try
                    {
                        string url = "http://checkip.dyndns.org";
                        System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                        System.Net.WebResponse resp = req.GetResponse();
                        System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                        string response = sr.ReadToEnd().Trim();
                        string[] a = response.Split(':');
                        string a2 = a[1].Substring(1);
                        string[] a3 = a2.Split('<');
                        string a4 = a3[0];
                        _IP = IPAddress.Parse(a4);
                    }
                    catch
                    {
                        //Failed...
                    }
                }
                return _IP;
            }
        }
        #endregion

        #region External Variables
        //These Variables all set when loading...
        public static string OwnerName = "???";
        public static string OwnerEmail = "???";

        public static string ServerName = "???";
        #endregion

        #region [!] Terminate Signal
        /// <summary>
        /// This signal will probably be deprecated soon.
        /// 
        /// It served the purpose of being a common signal to all threads that the program was ending/restarting.
        /// It may get moved elsewhere.
        /// </summary>
        public static ManualResetEvent TerminateSignal = new ManualResetEvent(false);
        #endregion
        #region [!] Restart Now
        /// <summary>
        /// This method will probably be deprecated soon.
        /// 
        /// It served the purpose of being a commonly accessible method to all threads
        /// to restart the program. It may get moved elsewhere.
        /// </summary>
        public static void RestartNow()
        {
            var fileName = Assembly.GetEntryAssembly().Location;
            string Outargs = "";
            foreach (string argument in Environment.CommandLineArguments)
            {
                if (Outargs.Length != 0) Outargs += " ";
                if (argument.Contains(' ')) Outargs += "\"";
                Outargs += argument;
                if (argument.Contains(' ')) Outargs += "\"";
            }
            System.Diagnostics.Process.Start(fileName, Outargs);
            System.Environment.Exit(0);
        }
        #endregion
        #region [!] Quit Now
        /// <summary>
        /// This method will probably be deprecated soon.
        /// 
        /// It served the purpose of being a commonly accessible method to all threads
        /// to end the program. It may get moved elsewhere.
        /// </summary>
        public static void QuitNow()
        {
            System.Environment.Exit(0);
        }
        #endregion
    }
    #endregion

    #region [!] OwnerInfo
    public static partial class OwnerInfo
    {
        //useless garbage at the moment. Need to grab this info from settings!

        public static string OwnerName = "";
        public static string OwnerContact = "";
    }
    #endregion

    #region Colors
    public static partial class Colors
    {
        #region XRGB Color
        /// <summary>
        /// A Color represented by R G B values.
        /// </summary>
        public class XRGBColor
        {
            public byte Red = 0;
            public byte Green = 0;
            public byte Blue = 0;

            public XRGBColor(byte _Red, byte _Green, byte _Blue)
            {
                Red = _Red;
                Green = _Green;
                Blue = _Blue;
            }

            public XRGBColor(string _INPUT)
            {
                if (_INPUT.CountOccurances(" ") >= 2)
                {
                    //"R G B"
                    Red = Byte.Parse(_INPUT.Split(' ')[0]);
                    Green = Byte.Parse(_INPUT.Split(' ')[1]);
                    Blue = Byte.Parse(_INPUT.Split(' ')[2]);
                    return;
                }
                if (_INPUT.StartsWith("#") && _INPUT.Length == 7)
                {
                    byte[] Arr = Enumerable.Range(0, _INPUT.Substring(1).Length / 2) .Select(x => Convert.ToByte(_INPUT.Substring(1).Substring(x * 2, 2), 16)) .ToArray();
                    //"#RRGGBB"
                    Red = Arr[0];
                    Green = Arr[1];
                    Blue = Arr[2];
                    return;
                }
            }

            public override string ToString()
            {
                return Red.ToString() + " " + Green.ToString() + " " + Blue.ToString();
            }

            public string ToHexString()
            {
                byte[] RGB = new byte[3]{ Red, Green, Blue };
                return "#" + BitConverter.ToString(RGB).Replace("-", "");
            }

            public XRGBColor LinearBlend(XRGBColor BlendWith, double Percent)
            {
                int OldRed = this.Red;
                int OldGreen = this.Green;
                int OldBlue = this.Blue;
                int NewRed = BlendWith.Red;
                int NewGreen = BlendWith.Green;
                int NewBlue = BlendWith.Blue;

                XRGBColor Output = new XRGBColor(
                    (byte)(OldRed + (int)(Percent * (NewRed - OldRed))),
                    (byte)(OldGreen + (int)(Percent * (NewGreen - OldGreen))),
                    (byte)(OldBlue + (int)(Percent * (NewBlue - OldBlue)))
                    );

                return Output;
            }

            public XRGBColor AlphaBlend(XRGBColor BlendWith, double Percent)
            {
                if (Percent < 0) Percent = 0;
                if (Percent > 1) Percent = 1;

                int OldRed = this.Red;
                int OldGreen = this.Green;
                int OldBlue = this.Blue;
                int NewRed = BlendWith.Red;
                int NewGreen = BlendWith.Green;
                int NewBlue = BlendWith.Blue;

                XRGBColor Output = new XRGBColor(
                    (byte)((int)(NewRed * Percent) + (int)(OldRed * (1 - Percent))),
                    (byte)((int)(NewGreen * Percent) + (int)(OldGreen * (1 - Percent))),
                    (byte)((int)(NewBlue * Percent) + (int)(OldBlue * (1 - Percent)))
                    );

                return Output;
            }

            public ARGBColor ToARGBColor()
            {
                return new ARGBColor(255, Red, Green, Blue);
            }
        }
        #endregion
        #region ARGB Color
        /// <summary>
        /// A Color represented by A R G B values.
        /// </summary>
        public class ARGBColor
        {
            public byte Alpha = 0;
            public byte Red = 0;
            public byte Green = 0;
            public byte Blue = 0;

            public ARGBColor(byte _Alpha, byte _Red, byte _Green, byte _Blue)
            {
                Alpha = _Alpha;
                Red = _Red;
                Green = _Green;
                Blue = _Blue;
            }

            public ARGBColor(string _RGB)
            {
                //"A R G B"
                Alpha = Byte.Parse(_RGB.Split(' ')[0]);
                Red = Byte.Parse(_RGB.Split(' ')[1]);
                Green = Byte.Parse(_RGB.Split(' ')[2]);
                Blue = Byte.Parse(_RGB.Split(' ')[3]);
            }

            public override string ToString()
            {
                return Alpha.ToString() + " " + Red.ToString() + " " + Green.ToString() + " " + Blue.ToString();
            }

            public string ToHexString()
            {
                byte[] ARGB = new byte[4]{ Alpha, Red, Green, Blue };
                return "#" + BitConverter.ToString(ARGB).Replace("-", "");
            }

            public XRGBColor ToXRGBColor()
            {
                return new XRGBColor(Red, Green, Blue);
            }

            public ARGBColor LinearBlend(ARGBColor BlendWith, double Percent)
            {
                int OldAlpha = this.Alpha;
                int OldRed = this.Red;
                int OldGreen = this.Green;
                int OldBlue = this.Blue;
                int NewAlpha = BlendWith.Alpha;
                int NewRed = BlendWith.Red;
                int NewGreen = BlendWith.Green;
                int NewBlue = BlendWith.Blue;

                ARGBColor Output = new ARGBColor(
                    (byte)(OldAlpha + (int)(Percent * (NewAlpha - OldAlpha))),
                    (byte)(OldRed + (int)(Percent * (NewRed - OldRed))),
                    (byte)(OldGreen + (int)(Percent * (NewGreen - OldGreen))),
                    (byte)(OldBlue + (int)(Percent * (NewBlue - OldBlue)))
                    );

                return Output;
            }

            public ARGBColor AlphaBlend(ARGBColor BlendWith, double Percent)
            {
                if (Percent < 0) Percent = 0;
                if (Percent > 1) Percent = 1;

                int OldAlpha = this.Alpha;
                int OldRed = this.Red;
                int OldGreen = this.Green;
                int OldBlue = this.Blue;
                int NewAlpha = BlendWith.Alpha;
                int NewRed = BlendWith.Red;
                int NewGreen = BlendWith.Green;
                int NewBlue = BlendWith.Blue;

                ARGBColor Output = new ARGBColor(
                    (byte)((int)(NewAlpha * Percent * (NewAlpha / 255d)) + (int)(OldAlpha * (OldAlpha / 255d) * (1 - Percent * (NewAlpha / 255d)))),
                    (byte)((int)(NewRed * Percent * (NewAlpha / 255d)) + (int)(OldRed * (OldAlpha / 255d) * (1 - Percent * (NewAlpha / 255d)))),
                    (byte)((int)(NewGreen * Percent * (NewAlpha / 255d)) + (int)(OldGreen * (OldAlpha / 255d) * (1 - Percent * (NewAlpha / 255d)))),
                    (byte)((int)(NewBlue * Percent * (NewAlpha / 255d)) + (int)(OldBlue * (OldAlpha / 255d) * (1 - Percent * (NewAlpha / 255d))))
                    );

                return Output;
            }
        }
        #endregion

        #region System Colors
        public static XRGBColor Foreground_0_DarkBlack   = new XRGBColor(  0,   0,   0);
        public static XRGBColor Foreground_1_DarkBlue    = new XRGBColor(  0,   0, 170);
        public static XRGBColor Foreground_2_DarkGreen   = new XRGBColor(  0, 170,   0);
        public static XRGBColor Foreground_3_DarkCyan    = new XRGBColor(  0, 170, 170);
        public static XRGBColor Foreground_4_DarkRed     = new XRGBColor(170,   0,   0);
        public static XRGBColor Foreground_5_DarkPurple  = new XRGBColor(170,   0, 170);
        public static XRGBColor Foreground_6_DarkYellow  = new XRGBColor(255, 170,   0);
        public static XRGBColor Foreground_7_DarkWhite   = new XRGBColor(170, 170, 170);
        public static XRGBColor Foreground_8_LightBlack  = new XRGBColor( 85,  85,  85);
        public static XRGBColor Foreground_9_LightBlue   = new XRGBColor(  0,   0, 255);
        public static XRGBColor Foreground_A_LightGreen  = new XRGBColor( 85, 255,  85);
        public static XRGBColor Foreground_B_LightCyan   = new XRGBColor( 85, 255, 255);
        public static XRGBColor Foreground_C_LightRed    = new XRGBColor(255,  85,  85);
        public static XRGBColor Foreground_D_LightPurple = new XRGBColor(255,  85, 255);
        public static XRGBColor Foreground_E_LightYellow = new XRGBColor(255, 255,  85);
        public static XRGBColor Foreground_F_LightWhite  = new XRGBColor(255, 255, 255);

        public static XRGBColor Background_0_DarkBlack   = new XRGBColor(  0,   0,   0);
        public static XRGBColor Background_1_DarkBlue    = new XRGBColor(  0,   0,  42);
        public static XRGBColor Background_2_DarkGreen   = new XRGBColor(  0,  42,   0);
        public static XRGBColor Background_3_DarkCyan    = new XRGBColor(  0,  42,  42);
        public static XRGBColor Background_4_DarkRed     = new XRGBColor( 42,   0,   0);
        public static XRGBColor Background_5_DarkPurple  = new XRGBColor( 42,   0,  42);
        public static XRGBColor Background_6_DarkYellow  = new XRGBColor( 42,  42,   0);
        public static XRGBColor Background_7_DarkWhite   = new XRGBColor( 42,  42,  42);
        public static XRGBColor Background_8_LightBlack  = new XRGBColor( 21,  21,  21);
        public static XRGBColor Background_9_LightBlue   = new XRGBColor( 21,  21,  63);
        public static XRGBColor Background_A_LightGreen  = new XRGBColor( 21,  63,  21);
        public static XRGBColor Background_B_LightCyan   = new XRGBColor( 21,  63,  63);
        public static XRGBColor Background_C_LightRed    = new XRGBColor( 63,  21,  21);
        public static XRGBColor Background_D_LightPurple = new XRGBColor( 63,  21,  63);
        public static XRGBColor Background_E_LightYellow = new XRGBColor( 63,  63,  21);
        public static XRGBColor Background_F_LightWhite  = new XRGBColor( 63,  63,  63);
        #endregion
        #region IRC Stuff
        public const string Black = "&0",
                            Navy = "&1",
                            Green = "&2",
                            Teal = "&3",
                            Maroon = "&4",
                            Purple = "&5",
                            Olive = "&6",
                            Silver = "&7",
                            Gray = "&8",
                            Blue = "&9",
                            Lime = "&a",
                            Aqua = "&b",
                            Red = "&c",
                            Magenta = "&d",
                            Yellow = "&e",
                            White = "&f";

        public static readonly Dictionary<string, IRCColor> OYSToIRCColors = new Dictionary<string, IRCColor> {
            { White, IRCColor.White },
            { Black, IRCColor.Black },
            { Navy, IRCColor.Navy },
            { Green, IRCColor.Green },
            { Red, IRCColor.Red },
            { Maroon, IRCColor.Maroon },
            { Purple, IRCColor.Purple },
            { Olive, IRCColor.Olive },
            { Yellow, IRCColor.Yellow },
            { Lime, IRCColor.Lime },
            { Teal, IRCColor.Teal },
            { Aqua, IRCColor.Aqua },
            { Blue, IRCColor.Blue },
            { Magenta, IRCColor.Magenta },
            { Gray, IRCColor.Gray },
            { Silver, IRCColor.Silver },
        };
        public enum IRCColor
        {
            White = 0,
            Black,
            Navy,
            Green,
            Red,
            Maroon,
            Purple,
            Olive,
            Yellow,
            Lime,
            Teal,
            Aqua,
            Blue,
            Magenta,
            Gray,
            Silver
        }
        public static string ToOYSColorCodes(string input)
        {
            if (input == null) throw new ArgumentNullException("input");
            StringBuilder sb = new StringBuilder(input);
            ToOYSColorCodes(sb);
            return sb.ToString();
        }
        public static string ToIRCColorCodes(string input)
        {
            if (input == null) throw new ArgumentNullException("input");
            StringBuilder sb = new StringBuilder(input);
            ToIRCColorCodes(sb);
            return sb.ToString();
        }

        public static void ToOYSColorCodes(StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException("sb");
            foreach (KeyValuePair<string, IRCColor> code in OYSToIRCColors)
            {
                string replacement = code.Key;
                sb.Replace('\u0003' + ((int)code.Value).ToString().PadLeft(2, '0'), replacement);
            }
        }
        public static void ToIRCColorCodes(StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException("sb");
            foreach (KeyValuePair<string, IRCColor> code in OYSToIRCColors)
            {
                string replacement = '\u0003' + ((int)code.Value).ToString().PadLeft(2, '0');
                sb.Replace(code.Key, replacement);
            }
        }
        public static int IndexOfOrdinal(this string haystack, string needle)
        {
            return haystack.IndexOf(needle, StringComparison.Ordinal);
        }
        #endregion
    }
    #endregion

    //NARRATE BELOW...

    #region Files
    public static partial class Files
    {
        #region File Exists?
        /// <summary>
        /// Check to see if a file exists.
        /// </summary>
        /// <param name="FileName">
        /// Path to the file.
        /// </param>
        /// <returns>True if the file exists.</returns>
        public static bool FileExists(string FileName)
        {
            try
            {
                if (File.Exists(FileName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region Read All Lines
        /// <summary>
        /// Read all the lines from the filename specified.
        /// </summary>
        /// <param name="Filename">Path to file.</param>
        /// <returns>string array seperated by line</returns>
        public static string[] FileReadAllLines(string Filename)
        {
            if (!FileExists(Filename)) return new string[] { "" };
            return File.ReadAllLines(Filename);
        }
        #endregion
        #region Prepare File
        /// <summary>
        /// Prepares the file for writing to.
        /// </summary>
        /// <param name="Filename">Path to the file.</param>
        /// <returns>True if it needed to create the file.</returns>
        public static bool FilePrepare(string Filename)
        {
            if (!(File.Exists(Filename)))
            {
                //.Dispose releases the file for use by the OS again.
                try
                {
                    File.Create(Filename).Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        #endregion            
        #region Delete File
        /// <summary>
        /// Deletes a file from the disk.
        /// </summary>
        /// <param name="Filename">Path to the file.</param>
        /// <returns>True if the file was deleted successfully.</returns>
        public static bool FileDelete(string Filename)
        {
            if ((File.Exists(Filename)))
            {
                //.Dispose releases the file for use by the OS again.
                try
                {
                    File.Delete(Filename);
                    return true;
                }
                catch
                {
                }
                return false;
            }
            return false;
        }
        #endregion
        #region Append
        /// <summary>
        /// Appends the strings to the file specified.
        /// </summary>
        /// <param name="Filename">Path to the file.</param>
        /// <param name="Strings">List of lines to add to the file</param>
        public static void FileAppend(string Filename, string[] Strings)
        {
            try
            {
                lock (Threads.GenericThreadSafeLock) File.AppendAllLines(Filename, Strings);
            }
            catch (Exception e)
            {
                Debug.WriteLine("File Write Failed!");
                Debug.WriteLine(e.Message);
                return;
            }
        }
        #endregion
        #region HTMLAppend
        /// <summary>
        /// Appends the strings to the file specified.
        /// </summary>
        /// <param name="Filename">Path to the file.</param>
        /// <param name="Strings">List of lines to add to the file</param>
        public static void FileHTMLAppend(string Filename, string[] Strings)
        {
            if (!File.Exists(Filename))
            {
                FileHTMLWrite(Filename, Strings);
            }
            try
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    
                    string[] Contents = File.ReadAllLines(Filename);
                    if (Contents.Count() <= 2)
                    {
                        FileHTMLWrite(Filename, Strings); //OVERWRITE...
                    }
                    List<string> Output = Contents.Take(Contents.Count() - 2).ToList();
                    foreach (string ThisString in Strings)
                    {
                        Output.Add(ThisString.ConvertToHTMLOutput());
                    }
                    string[] Finalise = Contents.Skip(Contents.Count() - 2).ToArray();
                    foreach (string ThisString in Finalise)
                    {
                        Output.Add(ThisString);
                    }
                    File.WriteAllLines(Filename, Output.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("File Append HTML Failed!");
                Debug.WriteLine(e.Message);
                return;
            }
        }
        #endregion
        #region Write
        /// <summary>
        /// Writes the strings to the file specified... if the file exists, it is overwritten.
        /// </summary>
        /// <param name="Filename">Path to the file.</param>
        /// <param name="Message">Strings to write to the file.</param>
        public static void FileWrite(string Filename, string[] Message)
        {
            try
            {
                lock (Threads.GenericThreadSafeLock) File.WriteAllLines(Filename, Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine("File Write Failed!");
                Debug.WriteLine(e.Message);
                return;
            }
        }
        #endregion
        #region HTMLWrite
        public static void FileHTMLWrite(string Filename, string[] Message)
        {
            try
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    List<string> Output = new List<string>();
                    Output.Add("<html>");
                    Output.Add("<body bgcolor=\"#000000\">");
                    foreach (string ThisString in Message)
                    {
                        Output.Add(ThisString.ConvertToHTMLOutput());
                    }
                    Output.Add("</body>");
                    Output.Add("</html>");
                    File.WriteAllLines(Filename, Output.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("File Write HTML Failed!");
                Debug.WriteLine(e.Message);
                return;
            }
        }
        #endregion
        #region Count Occurances In Files
        /// <summary>
        /// Searches all the filenames specified for occurances of string "Match" (case insensitive) and counts the results.
        /// </summary>
        /// <param name="Filenames">list of filepaths to search.</param>
        /// <param name="Match">the string to search for</param>
        /// <returns>total number of occurances in all files.</returns>
        public static int CountOccurances(string[] Filenames, string Match)
        {
            int Out = 0;
            foreach (string ThisFile in Filenames)
            {
                string[] Contents = Files.FileReadAllLines(ThisFile);
                foreach (string ThisLine in Contents)
                {
                    if (ThisLine.ToUpperInvariant().Contains(Match.ToUpperInvariant())) Out++;
                }
            }
            return Out;
        }
        #endregion
        #region Prepare Log File
        /// <summary>
        /// Just like "PrepareFile" method, but also adds a file header to the top of the file if the file is blank.
        /// </summary>
        /// <param name="Filename">path to the file.</param>
        public static void PrepareLog(string Filename)
        {
            try
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    string[] Message = { String.Format("--- Log Created {0} ---", DateTime.Now.ToOYSLongDateTime()) };
                    if (FilePrepare(Filename)) FileAppend(Filename, Message);
                }
            }
            catch
            {
                //Can't prepare the log, it's being written by something else!
            }

        }
        #endregion
        #region Prepare Log File HTML
        /// <summary>
        /// Just like "PrepareFile" method, but also adds a file header to the top of the file if the file is blank.
        /// </summary>
        /// <param name="Filename">path to the file.</param>
        public static void PrepareHTMLLog(string Filename)
        {
            try
            {
                lock (Threads.GenericThreadSafeLock)
                {
                    string[] Message = { String.Format("&7{0}: --- Log Created ---\n", DateTime.Now.ToOYSLongDateTime()) };
                    if (FilePrepare(Filename)) FileHTMLWrite(Filename, Message);
                }
            }
            catch
            {
                //Can't prepare the log, it's being written by something else!
            }

        }
        #endregion
    }
    #endregion

    #region Directories
    public static partial class Directories
    {
        #region Copy All
        /// <summary>
        /// Copies all subdirectories and files from Source to Destination.
        /// </summary>
        /// <param name="Source">Source directory to copy from.</param>
        /// <param name="Destination">Destindation directory to copy to.</param>
        public static void CopyAll(string Source, string Destination)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(Source, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(Source, Destination));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(Source, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(Source, Destination), true);
        }
        #endregion
        #region Remove Matching
        /// <summary>
        /// All subdirectories that match the expression will be deleted.
        /// </summary>
        /// <param name="Match">Expression to match against, can use wildcars etc.</param>
        /// <param name="Location">Top level directory in which to search.</param>
        public static void RemoveMatching(string Match, string Location)
        {
            if (Directory.Exists(Location))
            {
                //Now destroy all of the directories
                foreach (string dirPath in Directory.GetDirectories(Location, Match,
                    SearchOption.AllDirectories))
                {
                    DirectoryDeleteAll(dirPath);
                }
            }
        }
        #endregion
        #region Directory Exists?
        /// <summary>
        /// Check to see if a directory exists.
        /// </summary>
        /// <param name="DirectoryName">
        /// Path to the directory.
        /// </param>
        /// <returns>true if the directory exists.</returns>
        public static bool DirectoryExists(string DirectoryName)
        {
            if (Directory.Exists(DirectoryName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region Get Files
        /// <summary>
        /// Get all the filenames in the specified directory.
        /// 
        /// !!! THIS ASSUMES THE DIRECTORY EXISTS !!!
        /// </summary>
        /// <param name="DirectoryName">path to search in.</param>
        /// <returns>list of files in the directory.</returns>
        public static string[] DirectoryGetFilenames(string DirectoryName)
        {
            return new DirectoryInfo(DirectoryName).GetFiles().Select(x => x.Name).ToArray();
        }
        #endregion
        #region Prepare Directory
        /// <summary>
        /// Prepares the directory for operations by creating the directory if it doesn't yet exist.
        /// </summary>
        /// <param name="DirectoryName">Path to search for.</param>
        /// <returns>Returns True if it needed to create the directory.</returns>
        public static void DirectoryPrepare(string DirectoryName)
        {
            if (!(Directory.Exists(DirectoryName)))
            {
                try
                {
                    Directory.CreateDirectory(DirectoryName);
                }
                catch
                {
                }
            }
        }
        #endregion
        #region Remove All
        /// <summary>
        /// Will remove the directory specified, plus all subdirectories and files.
        /// </summary>
        /// <param name="DirectoryPath">path to remove.</param>
        public static void DirectoryDeleteAll(string DirectoryPath)
        {
            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(DirectoryPath);

            if (Directory.Exists(DirectoryPath))
            {
                bool FilesExisted = false;
                foreach (FileInfo file in downloadedMessageInfo.GetFiles())
                {
                    file.Delete();
                    FilesExisted = true;
                }
                if (FilesExisted) Thread.Sleep(1000);
                foreach (DirectoryInfo dir in downloadedMessageInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                downloadedMessageInfo.Delete(true);
                //Directory.Delete(DirectoryPath);
            }
        }
        #endregion
    }
    #endregion

    #region [!] Console
    /// <summary>
    /// Will most likely to away with this entire class in favour of a new console.
    /// </summary>
    public static partial class Console2
    {
        /// <summary>
        /// Writes to the console with colored text.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="text"></param>
        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.Write(text);
            System.Console.ForegroundColor = originalColor;
        }

        public static string AsString(this System.ConsoleColor Input)
        {
            switch (Input)
            {
                case ConsoleColor.Black: return "&0";
                case ConsoleColor.DarkBlue: return "&1";
                case ConsoleColor.DarkGreen: return "&2";
                case ConsoleColor.DarkCyan: return "&3";
                case ConsoleColor.DarkRed: return "&4";
                case ConsoleColor.DarkMagenta: return "&5";
                case ConsoleColor.DarkYellow: return "&6";
                case ConsoleColor.Gray: return "&7";
                case ConsoleColor.DarkGray: return "&8";
                case ConsoleColor.Blue: return "&9";
                case ConsoleColor.Green: return "&A";
                case ConsoleColor.Cyan: return "&B";
                case ConsoleColor.Red: return "&C";
                case ConsoleColor.Magenta: return "&D";
                case ConsoleColor.Yellow: return "&E";
                case ConsoleColor.White: return "&F";
                default: return "&F";
            }
        }
    }
    #endregion

    #region Emailing
    public static partial class Emailing
    {
        public static void SendEmail(string Subject, string Output)
        {
            //name: OpenYSProject@gmail.com
            //pass: YSFlightHeadquarters
            //DOB: 15/01/1990

            var fromAddress = new MailAddress("openysproject@gmail.com", "OpenYS");
            var toAddress = new MailAddress("OfficerFlake@gmail.com", "OfficerFlake");
            const string fromPassword = "YSFlightHeadquarters";
            string subject = Subject;
            string body = Output;

            var smtp = new SmtpClient
            {
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                try
                {
                    smtp.Send(message);
                    //Console.WriteLine("Email Send Success.");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Email Send Failed... You're on your own! Good luck!");
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public static string[] PrepareBugReportEmail(this Exception e)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            string Subject = "OpenYS Bug Report - " + Environment.GetCompilationDate() + ".";
            Subject = Subject.ReplaceAll("\n", "").ReplaceAll("\r", "");

            string Body = "";
            Body += "OpenYS Bug Report\n";
            Body += "Version: " + Environment.GetCompilationDate() + "\n";
            Body += "Server Name: " + Environment.ServerName + "\n";
            Body += "Server Address: " + Environment.ExternalIP + "\n";
            Body += "Owner Name: " + Environment.OwnerName + "\n";
            Body += "Owner Email: " + Environment.OwnerEmail + "\n";
            Body += "\n";
            Body += "OpenYS Client Class has the following error:\n";
            Body += "\n";
            Body += Strings.StripFormatting(Debug.GetStackTrace(e));

            return new string[] { Subject, Body };
        }

        public static bool SendBugReportEmail(Exception e)
        {
            try
            {
                string[] Email = Emailing.PrepareBugReportEmail(e);
                Emailing.SendEmail(Email[0], Email[1]);
                return true;
            }
            catch (Exception e2)
            {
                Debug.WriteLine("Email Send Failed... You're on your own! Good luck!");
                Debug.WriteLine(e2.Message);
                return false;
            }
        }

        public static string[] PrepareCrashReportEmail(this Exception e)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            var line = frame.GetFileLineNumber();

            string Subject = "OpenYS Crash Report - " + Environment.GetCompilationDate() + ".";
            Subject = Subject.ReplaceAll("\n", "").ReplaceAll("\r", "");

            string Body = "";
            Body += "OpenYS Bug Report\n";
            Body += "Version: " + Environment.GetCompilationDate() + "\n";
            Body += "Server Name: " + Environment.ServerName + "\n";
            Body += "Server Address: " + Environment.ExternalIP + "\n";
            Body += "Owner Name: " + Environment.OwnerName + "\n";
            Body += "Owner Email: " + Environment.OwnerEmail + "\n";
            Body += "\n";
            Body += "OpenYS crashed with the following error:\n";
            Body += "\n";
            Body += Strings.StripFormatting(Debug.GetStackTrace(e));

            return new string[] { Subject, Body };
        }

        public static bool SendCrashReportEmail(Exception e)
        {
            try
            {
                string[] Email = Emailing.PrepareCrashReportEmail(e);
                Emailing.SendEmail(Email[0], Email[1]);
                return true;
            }
            catch (Exception e2)
            {
                Debug.WriteLine("Email Send Failed... You're on your own! Good luck!");
                Debug.WriteLine(e2.Message);
                return false;
            }
        }
    }
    #endregion

    #region OYS_DateTime
    public static partial class OYS_DateTime
    {
        /// <summary>
        /// Returns a string array of the specified datetime in OYS standard type values.
        /// </summary>
        /// <param name="ThisDateTime"></param>
        /// <returns>YYYY, MM, DD, HH, mm, ss</returns>
        public static string[] ToOYSFormat(this DateTime ThisDateTime)
        {
            #region Year
            string Year = ThisDateTime.Year.ToString();
            if (ThisDateTime.Year.ToString().Length > 4) Year = Year.Substring(0, 4);
            while (Year.Length < 4) Year = "0" + Year;
            #endregion
            #region Month
            string Month = ThisDateTime.Month.ToString();
            if (ThisDateTime.Month.ToString().Length > 2) Month = Month.Substring(0, 2);
            while (Month.Length < 2) Month = "0" + Month;
            #endregion
            #region Day
            string Day = ThisDateTime.Day.ToString();
            if (ThisDateTime.Day.ToString().Length > 2) Day = Day.Substring(0, 2);
            while (Day.Length < 2) Day = "0" + Day;
            #endregion
            #region Hour
            string Hour = ThisDateTime.Hour.ToString();
            if (ThisDateTime.Hour.ToString().Length > 2) Hour = Hour.Substring(0, 2);
            while (Hour.Length < 2) Hour = "0" + Hour;
            #endregion
            #region Minute
            string Minute = ThisDateTime.Minute.ToString();
            if (ThisDateTime.Minute.ToString().Length > 2) Minute = Minute.Substring(0, 2);
            while (Minute.Length < 2) Minute = "0" + Minute;
            #endregion
            #region Second
            string Second = ThisDateTime.Second.ToString();
            if (ThisDateTime.Second.ToString().Length > 2) Second = Second.Substring(0, 2);
            while (Second.Length < 2) Second = "0" + Second;
            #endregion
            string[] Output = { Year, Month, Day, Hour, Minute, Second };
            return Output;
        }
        public static string ToOYSLongDateTime(this DateTime CurrentTime)
        {
            string[] FormattedTime = ToOYSFormat(CurrentTime);
            return String.Format("{0}{1}{2}({3}{4}{5})", FormattedTime[0], FormattedTime[1], FormattedTime[2], FormattedTime[3], FormattedTime[4], FormattedTime[5]);
        }
        public static string ToOYSShortDateTime(this DateTime CurrentTime)
        {
            string[] FormattedTime = ToOYSFormat(CurrentTime);
            return String.Format("{0}{1}{2}({3}{4})", FormattedTime[0], FormattedTime[1], FormattedTime[2], FormattedTime[3], FormattedTime[4]);
        }
        public static string ToOYSDate(this DateTime CurrentTime)
        {
            string[] FormattedTime = ToOYSFormat(CurrentTime);
            return String.Format("{0}{1}{2}", FormattedTime[0], FormattedTime[1], FormattedTime[2]);
        }
        public static string ToOYSLongTime(this DateTime CurrentTime)
        {
            string[] FormattedTime = ToOYSFormat(CurrentTime);
            return String.Format("{0}{1}{2}", FormattedTime[3], FormattedTime[4], FormattedTime[5]);
        }
        public static string ToOYSShortTime(this DateTime CurrentTime)
        {
            string[] FormattedTime = ToOYSFormat(CurrentTime);
            return String.Format("{0}{1}", FormattedTime[3], FormattedTime[4]);
        }

        public static string GetTimeDifference(this TimeSpan T0, TimeSpan T1)
        {
            TimeSpan TDelta = T1 - T0;
            string SplitHours = TDelta.Hours.ToString();
            while (SplitHours.Length < 2) SplitHours = "0" + SplitHours;
            if (SplitHours.Length > 2) SplitHours = "99";

            string SplitMinutes = TDelta.Minutes.ToString();
            while (SplitMinutes.Length < 2) SplitMinutes = "0" + SplitMinutes;
            if (SplitMinutes.Length > 2) SplitMinutes = "99";

            string SplitSeconds = TDelta.Seconds.ToString();
            while (SplitSeconds.Length < 2) SplitSeconds = "0" + SplitSeconds;
            if (SplitSeconds.Length > 2) SplitSeconds = "99";

            string SplitMilliseconds = TDelta.Milliseconds.ToString();
            while (SplitMilliseconds.Length < 3) SplitMilliseconds = "0" + SplitMilliseconds;
            if (SplitMilliseconds.Length > 3) SplitMilliseconds = "999";

            return SplitHours + ":" + SplitMinutes + ":" + SplitSeconds + "." + SplitMilliseconds;
        }
    }
    #endregion

    #region Strings
    public static partial class Strings
    {
        /// <summary>
        /// Splits a string by Minecraft-style formatting codes.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static string[] _SplitByFormatCodes(this string Input)
        {
            List<string> Out = new List<string>();
            string Builder = "&f";
            for (int i = 0; i < Input.Length; i++)
            {
                if (i < Input.Length - 1)
                {
                    //More characters follow.
                    if (Input[i] == '&')
                    {
                        //Format delimiter... check against format codes to split!
                        switch (Input.ToUpperInvariant()[i + 1])
                        {
                            case '0':
                                goto Split;
                            case '1':
                                goto Split;
                            case '2':
                                goto Split;
                            case '3':
                                goto Split;
                            case '4':
                                goto Split;
                            case '5':
                                goto Split;
                            case '6':
                                goto Split;
                            case '7':
                                goto Split;
                            case '8':
                                goto Split;
                            case '9':
                                goto Split;
                            case 'A':
                                goto Split;
                            case 'B':
                                goto Split;
                            case 'C':
                                goto Split;
                            case 'D':
                                goto Split;
                            case 'E':
                                goto Split;
                            case 'F':
                                goto Split;
                            default:
                                //Not a splitting char, do NOT split!
                                Builder += Input[i];
                                continue;
                            Split:
                                if (Builder.Length >= 2) Out.Add(Builder);
                                Builder = "";
                                Builder += Input[i];
                                Builder += Input[i+1];
                                i++; //To skip the next char...
                                continue;
                        }
                    }
                    //Not an ampersand, add as normal.
                    Builder += Input[i];
                    continue;
                }
                else
                {
                    //Last char in string.
                    Builder += Input[i];
                    Out.Add(Builder);
                    Builder = "";
                }
            }
            return Out.ToArray();
        }

        private static string _GetColorCodeByChar(this char Input)
        {
            switch (Input.ToString().ToUpperInvariant())
            {
                case "0":
                    return Colors.Foreground_0_DarkBlack.ToHexString();
                case "1":
                    return Colors.Foreground_1_DarkBlue.ToHexString();
                case "2":
                    return Colors.Foreground_2_DarkGreen.ToHexString();
                case "3":
                    return Colors.Foreground_3_DarkCyan.ToHexString();
                case "4":
                    return Colors.Foreground_4_DarkRed.ToHexString();
                case "5":
                    return Colors.Foreground_5_DarkPurple.ToHexString();
                case "6":
                    return Colors.Foreground_6_DarkYellow.ToHexString();
                case "7":
                    return Colors.Foreground_7_DarkWhite.ToHexString();
                case "8":
                    return Colors.Foreground_8_LightBlack.ToHexString();
                case "9":
                    return Colors.Foreground_9_LightBlue.ToHexString();
                case "A":
                    return Colors.Foreground_A_LightGreen.ToHexString();
                case "B":
                    return Colors.Foreground_B_LightCyan.ToHexString();
                case "C":
                    return Colors.Foreground_C_LightRed.ToHexString();
                case "D":
                    return Colors.Foreground_D_LightPurple.ToHexString();
                case "E":
                    return Colors.Foreground_E_LightYellow.ToHexString();
                case "F":
                    return Colors.Foreground_F_LightWhite.ToHexString();
                default:
                    return Colors.Foreground_F_LightWhite.ToHexString();
            }
        }

        private static string _ConvertToHTMLOutput(this string Input)
        {
            if (Input.Length < 2) return "ConvertToHTMLOutput Failure [Length]: \"" + Input + "\"";
            if (Input[0] != '&') return "ConvertToHTMLOutput Failure [Delimiter]: \"" + Input + "\"";
            return "</font><font face=\"Courier New\" color=\"" + Input[1]._GetColorCodeByChar() + "\">" + Input.Substring(2);
        }

        public static string ConvertToHTMLOutput(this string Input)
        {
            string HTML = Input;
            string[] SPLIT = HTML._SplitByFormatCodes();
            List<string> HTMLReady = new List<string>();
            foreach (string ThisString in SPLIT)
            {
                HTMLReady.Add(ThisString.Substring(0,2) + WebUtility.HtmlEncode(ThisString.Substring(2)));
            }
            string Output = "";
            Output += "<font face=\"Courier New\" color=\"" + Colors.Foreground_F_LightWhite.ToHexString() + "\">"; //open the first font entry... this is often always an empty string!
            foreach (string ThisString in HTMLReady)
            {
                Output += ThisString._ConvertToHTMLOutput();
            }
            Output += "</font>"; //terminate the last font entry
            Output += "<br>";
            return Output;
        }

        /// <summary>
        /// Simplification method: Compresses all tabs and whitespaces into just one space each.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string CompressWhiteSpace(this string Input)
        {
            while (Input.Contains("  ")) Input = Input.ReplaceAll("  ", " ");
            while (Input.Contains("\t\t")) Input = Input.ReplaceAll("\t\t", "\t");
            Input = Input.ReplaceAll("\t", " ");
            return Input;
        }

        /// <summary>
        /// Uses a RegEx to split a string by white spaces - preserving quoted blocks.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string[] SplitPreservingQuotes(this string Input, char splittingchar)
        {
            if (Input == null) Input = "";
            Input = Input.ReplaceAll("\t\t", "\t");
            Input = Input.ReplaceAll("\t", " ");
            List<string> Strings = Regex.Matches(Input, @"[\""].+?[\""]|[^" + splittingchar + "]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();

            List<string> Output = new List<string>();
            for (int i = 0; i < Strings.Count; i++)
            {
                Output.Add(Strings[i].Split('\t')[0].Split(';')[0].ReplaceAll("\"", ""));
            }

            return Output.ToArray();
        }

        /// <summary>
        /// Replaces all occurences of oldstr in string Input to newstr. WARNING: DO NOT REPLACE oldstr WHERE newstr CONTAINS oldstr!
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string ReplaceAll(this string Input, string oldstr, string newstr)
        {
            while (Input.Contains(oldstr))
            {
                if (newstr.Contains(oldstr)) break;
                if (oldstr == "") break;
                if (oldstr == newstr) break;
                Input = Input.Replace(oldstr, newstr);
            }
            return Input;
        }

        /// <summary>
        /// Makes a string N chars long by trimming excess or adding trailing nulls(\0).
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Resize(this string Input, int limit)
        {
            string output = "";
            if (Input == null) Input = "";
            if (limit < 0) limit = 0;
            foreach (char ThisChar in Input)
            {
                //compresses the input to a max of N.
                if (output.Length < limit)
                {
                    output += ThisChar;
                }
            }
            //extends the input to n if it is under the limit.
            while (output.Length < limit)
            {
                output += '\0';
            }
            //output = output.Substring(0, limit-1) + '\0'; //don't need to terminate with a null anymore!
            return output;
        }

        /// <summary>
        /// Returns a new string where Input is repeated amount times.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Ammount"></param>
        /// <returns></returns>
        public static string Repeat(this string Input, int Ammount)
        {
            return String.Concat(Enumerable.Repeat(Input, Ammount));
        }

        /// <summary>
        /// Converts an array of bytes to a standard string. Each byte in the input array is converted to it's character representation.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string ToDataString(this byte[] Input)
        {
            string output = "";
            foreach (byte ThisByte in Input)
            {
                output += Convert.ToChar(ThisByte);
            }
            return output;
        }

        /// <summary>
        /// Converts an array of chars to a standard string.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string ToDataString(this char[] Input)
        {
            string output = "";
            foreach (char ThisChar in Input)
            {
                output += ThisChar;
            }
            return output;
        }

        /// <summary>
        /// Converts a standard string into an array of bytes. Each character in the string is converted to it's byte value.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string Input)
        {
            List<byte> Outbytes = new List<byte>();
            foreach (char ThisChar in Input)
            {
                if (ThisChar > 255 | ThisChar < 0)
                {
                    Outbytes.Add((byte)' '); //avoids unicode char issues.
                }
                else
                {
                    Outbytes.Add(Convert.ToByte(ThisChar));
                }
            }
            return Outbytes.ToArray();
        }

        /// <summary>
        /// Removes the final comma and space from a building string list, and adds the finalising period.
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        public static string FinaliseStringList(this string InputString)
        {
            if (InputString.Length < 2) return InputString;
            if (!InputString.EndsWith(", ")) return InputString + "."; //it wasn't a list!
            return InputString.Remove(InputString.Length - 2) + ".";
        }

        /// <summary>
        /// Converts a List of Strings to a comma seperated, period finalised list.
        /// </summary>
        /// <param name="ThisStringList"></param>
        /// <returns></returns>
        public static string ToStringList(this List<String> ThisStringList)
        {
            string Output = "";
            foreach (string ThisString in ThisStringList)
            {
                Output += ThisString + ", ";
            }
            if (Output == "") return "None.";
            return Output.FinaliseStringList();
        }

        /// <summary>
        /// Converts a List of Strings to a comma seperated, period finalised list.
        /// </summary>
        /// <param name="ThisStringList"></param>
        /// <returns></returns>
        public static string ToStringList(this string[] ThisStringArray)
        {
            return ToStringList(ThisStringArray.ToList());
        }


        public static string ToTitleCaseInvariant(this string Input)
        {
            string output = "";
            if (Input == null) return "";
            foreach (string ThisString in Input.Split(' '))
            {
                if (output.Length > 0) output += " ";
                if (ThisString.Length > 0) output += ThisString.Substring(0, 1).ToUpperInvariant();
                if (ThisString.Length > 1) output += ThisString.Substring(1).ToLowerInvariant();
            }
            return output;
        }

        public static string[] BreakDownFormattedString(this string input)
        {

            List<string> output = new List<string>();
            string nextoutput = "";
            char nextcolor = 'f';
            if (input == null) return output.ToArray();
            if (input.IndexOf('&') == -1)
            {
                output.Add("&f" + input);
                return output.ToArray();
            }
            else
            {
                for (int i = 0; i < input.Length; i++)
                {
                    if (i > 0)
                    {
                        if (input[i] == '\\')
                        {
                            i++;
                            nextoutput += input[i];
                            continue;
                        }
                    }
                    if (input[i] == '&')
                    {
                        if (input.Length > i + 1)
                        {
                            foreach (char colorid in "0123456789abcdef")
                            {
                                if (colorid == input.ToLowerInvariant()[i + 1])
                                {
                                    output.Add("&" + nextcolor + nextoutput);
                                    nextoutput = "";
                                    nextcolor = colorid;
                                    continue;
                                }
                            }
                            foreach (char colorid in "syprhwmi")
                            {
                                if (colorid == input.ToLowerInvariant()[i + 1])
                                {
                                    output.Add("&" + nextcolor + nextoutput);
                                    nextoutput = "";
                                    nextcolor = colorid;
                                    continue;
                                }
                            }
                            continue;
                        }
                    }
                    else if (i > 0)
                    {
                        if (input[i - 1] == '&')
                        {
                            if (i > 1)
                            {
                                if (input[i - 2] == '\\')
                                {
                                    nextoutput += input[i];
                                    continue;
                                }
                            }
                            foreach (char colorid in "0123456789abcdef")
                            {
                                if (colorid == input[i])
                                {
                                    continue;
                                }
                            }
                            foreach (char colorid in "syprhwmi")
                            {
                                if (colorid == input.ToLowerInvariant()[i])
                                {
                                    output.Add("&" + nextcolor + nextoutput);
                                    nextoutput = "";
                                    nextcolor = colorid;
                                    continue;
                                }
                            }
                            continue;
                        }
                    }
                    if (input[i] == '&') nextoutput += "\\";
                    nextoutput += input[i];
                }
                output.Add("&" + nextcolor + nextoutput);
                return output.ToArray();
            }
        }

        public static string StripFormatting(this string input)
        {
            if (input == null) return "";
            if (input.IndexOf('&') == -1)
            {
                return input;
            }
            else
            {
                StringBuilder output = new StringBuilder(input.Length);
                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '&')
                    {
                        if (i == input.Length - 1)
                        {
                            break;
                        }
                        i++;
                        if (input[i] == 'n' || input[i] == 'N')
                        {
                            output.Append('\n');
                        }
                    }
                    else
                    {
                        output.Append(input[i]);
                    }
                }
                return output.ToString();
            }
        }

        public static int CountOccurances(this string Input, string SubStr)
        {
            return Input.Length - Input.Replace(SubStr, "").Length;
        }
    }
    #endregion

    #region Encryption
    public static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
    public static class OYS_Cryptography
    {
        public static string EncryptPassword(string RawPass)
        {
            return StringCipher.Encrypt(RawPass, Environment.GetAPIKey());
        }

        public static string DecryptPassword(string EncryptedPass)
        {
            return StringCipher.Decrypt(EncryptedPass, Environment.GetAPIKey());
        }
    }
    #endregion

    #region Bits
    public static partial class Bits
    {

        /// <summary>
        /// returns the UNITS column of a Hex number between 0 and 255, as a value from 0 to 15.
        /// </summary>
        /// <param name="ThisByte"></param>
        /// <returns></returns>
        public static byte GetUnits(this byte ThisByte)
        {
            return (byte)(ThisByte & 15);
        }

        /// <summary>
        /// returns the TENS column of a Hex number between 0 and 255, as a value from 0 to 15.
        /// </summary>
        /// <param name="ThisByte"></param>
        /// <returns></returns>
        public static byte GetTens(this byte ThisByte)
        {
            return (byte)((ThisByte & 240) / 16);
        }

        /// <summary>
        /// returns a value from 0 to 255, where the byte given is the units only. (overflow bits not supported.) [Bitwise it shifts the numbers 4 binary to the left]
        /// </summary>
        /// <param name="ThisByte"></param>
        /// <returns></returns>
        public static byte ToTens(this byte ThisByte)
        {
            return (byte)((int)ThisByte * 16);
        }

        /// <summary>
        /// returns a value from 0 to 15, where the byte given is the tens only. (underflow bits not supported.) [Bitwise it shifts the numbers 4 binary to the right]
        /// </summary>
        /// <param name="ThisByte"></param>
        /// <returns></returns>
        public static byte ToUnits(this byte ThisByte)
        {
            return (byte)((int)ThisByte / 16);
        }

        /// <summary>
        /// Sets the sepecific bit of a byte to 1.
        /// </summary>
        /// <param name="ThisByte"></param>
        /// <param name="Cell"></param>
        /// <returns></returns>
        public static byte SetBit(this byte ThisByte, int Cell)
        {
            if (Cell >= 8 || Cell < 0)
            {
                return ThisByte;
            }
            byte SetTo = (byte)(Math.Pow((double)2, (double)Cell));
            return (byte)(ThisByte & SetTo);
        }

        /// <summary>
        /// UnSets the sepecific bit of a byte to 1.
        /// </summary>
        /// <param name="ThisByte"></param>
        /// <param name="Cell"></param>
        /// <returns></returns>
        public static byte UnSetBit(this byte ThisByte, int Cell)
        {
            if (Cell >= 8 || Cell < 0)
            {
                return ThisByte;
            }
            byte SetTo = (byte)(Math.Pow((double)2, (double)Cell));
            return (byte)(ThisByte & ~SetTo);
        }

        /// <summary>
        /// Converts a string representation of a binary number to a byte
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static byte ToByte(this string Input)
        {
            Input = Input.ReplaceAll(" ", "");
            if (Input.Length != 8) return 0;
            string temp = Input;
            byte output = 0;
            if (temp.ReplaceAll("1", "0").ReplaceAll("0", "") != "") return 0;
            for (int i = 0; i <= 7; i++)
            {
                if (Input.Substring(7 - i, 1) == "1") output |= (byte)(Math.Pow((double)2, (double)i));
            }
            return output;
        }

        public static byte[] ArrayCombine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        public static byte[] EmptyBits(int Count)
        {
            return Strings.Repeat("\0", Count).ToByteArray();
        }

        /// <summary>
        /// Returns a new byte array where Input is repeated amount times.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Ammount"></param>
        /// <returns></returns>
        public static byte[] Repeat(this string Input, int Ammount)
        {
            return String.Concat(Enumerable.Repeat(Input, Ammount)).ToByteArray();
        }
    }
    #endregion

    #region Numbers
    public static partial class Numbers
    {
        #region Velocity
        public static class Velocity
        {
            public static class Conversions
            {
                public static double Knots_ConversionFactor = 1.943844492440605;
                public static double MetersPerSecond_ConversionFactor = 1;
                public static double MilesPerHour_ConversionFactor = 2.236936292054402;
                public static double KilometersPerHour_ConversionFactor = 3.6;
                public static double FeetPerSecond_ConversionFactor = 3.280839895013123;
                public static double MachAtSeaLevel_ConversionFactor = 0.0029386414601757;
            }

            public class Common
            {
                public double OwnConversionFactor = 0;
                public double Value = 0;

                public Knots ToKnots()
                {
                    return new Knots(this.Value / OwnConversionFactor * Conversions.Knots_ConversionFactor);
                }
                public MetersPerSecond ToMetersPerSecond()
                {
                    return new MetersPerSecond(this.Value / OwnConversionFactor * Conversions.MetersPerSecond_ConversionFactor);
                }
                public MilesPerHour ToMilesPerHour()
                {
                    return new MilesPerHour(this.Value / OwnConversionFactor * Conversions.MilesPerHour_ConversionFactor);
                }
                public KilometersPerHour ToKilometersPerHour()
                {
                    return new KilometersPerHour(this.Value / OwnConversionFactor * Conversions.KilometersPerHour_ConversionFactor);
                }
                public FeetPerSecond ToFeetPerSecond()
                {
                    return new FeetPerSecond(this.Value / OwnConversionFactor * Conversions.FeetPerSecond_ConversionFactor);
                }
                public MachAtSeaLevel ToMachAtSeaLevel()
                {
                    return new MachAtSeaLevel(this.Value / OwnConversionFactor * Conversions.MachAtSeaLevel_ConversionFactor);
                }
            }

            public class Knots : Common
            {
                public Knots(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Knots_ConversionFactor;
                }

                public static implicit operator Double(Knots Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Knots(Double Input)
                {
                    return new Knots(Input);
                }

                public override string ToString()
                {
                    return base.Value + "KT";
                }
            }
            public class MetersPerSecond : Common
            {
                public MetersPerSecond(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.MetersPerSecond_ConversionFactor;
                }

                public static implicit operator Double(MetersPerSecond Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator MetersPerSecond(Double Input)
                {
                    return new MetersPerSecond(Input);
                }

                public override string ToString()
                {
                    return base.Value + "M/S";
                }
            }
            public class MilesPerHour : Common
            {
                public MilesPerHour(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.MilesPerHour_ConversionFactor;
                }

                public static implicit operator Double(MilesPerHour Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator MilesPerHour(Double Input)
                {
                    return new MilesPerHour(Input);
                }

                public override string ToString()
                {
                    return base.Value + "M/H";
                }
            }
            public class KilometersPerHour : Common
            {
                public KilometersPerHour(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.KilometersPerHour_ConversionFactor;
                }

                public static implicit operator KilometersPerHour(Double Input)
                {
                    return new KilometersPerHour(Input);
                }

                public static implicit operator Double(KilometersPerHour Input)
                {
                    return (Double)Input.Value;
                }

                public override string ToString()
                {
                    return base.Value + "K/H";
                }
            }
            public class FeetPerSecond : Common
            {
                public FeetPerSecond(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.FeetPerSecond_ConversionFactor;
                }

                public static implicit operator Double(FeetPerSecond Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator FeetPerSecond(Double Input)
                {
                    return new FeetPerSecond(Input);
                }

                public override string ToString()
                {
                    return base.Value + "F/S";
                }
            }
            public class MachAtSeaLevel : Common
            {
                public MachAtSeaLevel(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.MachAtSeaLevel_ConversionFactor;
                }

                public static implicit operator Double(MachAtSeaLevel Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator MachAtSeaLevel(Double Input)
                {
                    return new MachAtSeaLevel(Input);
                }

                public override string ToString()
                {
                    return base.Value + "MACH";
                }
            }
        }
        #region As... (Double)
        public static Velocity.Knots AsKnots(this double ThisNumber)
        {
            return new Velocity.Knots(ThisNumber);
        }
        public static Velocity.MetersPerSecond AsMetersPerSecond(this double ThisNumber)
        {
            return new Velocity.MetersPerSecond(ThisNumber);
        }
        public static Velocity.MilesPerHour AsMilesPerHour(this double ThisNumber)
        {
            return new Velocity.MilesPerHour(ThisNumber);
        }
        public static Velocity.KilometersPerHour AsKilometersPerHour(this double ThisNumber)
        {
            return new Velocity.KilometersPerHour(ThisNumber);
        }
        public static Velocity.FeetPerSecond AsFeetPerSecond(this double ThisNumber)
        {
            return new Velocity.FeetPerSecond(ThisNumber);
        }
        public static Velocity.MachAtSeaLevel AsMachAtSeaLevel(this double ThisNumber)
        {
            return new Velocity.MachAtSeaLevel(ThisNumber);
        }
        #endregion
        #region As... (Int32)
        public static Velocity.Knots AsKnots(this int ThisNumber)
        {
            return new Velocity.Knots(ThisNumber);
        }
        public static Velocity.MetersPerSecond AsMetersPerSecond(this int ThisNumber)
        {
            return new Velocity.MetersPerSecond(ThisNumber);
        }
        public static Velocity.MilesPerHour AsMilesPerHour(this int ThisNumber)
        {
            return new Velocity.MilesPerHour(ThisNumber);
        }
        public static Velocity.KilometersPerHour AsKilometersPerHour(this int ThisNumber)
        {
            return new Velocity.KilometersPerHour(ThisNumber);
        }
        public static Velocity.FeetPerSecond AsFeetPerSecond(this int ThisNumber)
        {
            return new Velocity.FeetPerSecond(ThisNumber);
        }
        public static Velocity.MachAtSeaLevel AsMachAtSeaLevel(this int ThisNumber)
        {
            return new Velocity.MachAtSeaLevel(ThisNumber);
        }
        #endregion
        #region As... (String)
        private static bool _ConvertVelocity(this string Input, double ConversionFactor, out double Output)
        {
            Output = 0;
            double OutputConversionFactor = ConversionFactor;

            double Value = 0;
            string WorkingString = Input.ToUpperInvariant();
            bool Failed = false;

            #region From KT
            if (WorkingString.EndsWith("KT"))
            {
                WorkingString = WorkingString.Replace("KT", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.Knots_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion

            #region From MPS
            if (WorkingString.EndsWith("MPS"))
            {
                WorkingString = WorkingString.Replace("MPS", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MetersPerSecond_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From M/S
            if (WorkingString.EndsWith("M/S"))
            {
                WorkingString = WorkingString.Replace("M/S", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MetersPerSecond_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From MS
            if (WorkingString.EndsWith("MS"))
            {
                WorkingString = WorkingString.Replace("MS", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MetersPerSecond_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion

            #region From MPH
            if (WorkingString.EndsWith("MPH"))
            {
                WorkingString = WorkingString.Replace("MPH", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MilesPerHour_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From M/H
            if (WorkingString.EndsWith("M/H"))
            {
                WorkingString = WorkingString.Replace("M/H", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MilesPerHour_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From MH
            if (WorkingString.EndsWith("MH"))
            {
                WorkingString = WorkingString.Replace("MH", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MilesPerHour_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion

            #region From KPH
            if (WorkingString.EndsWith("KPH"))
            {
                WorkingString = WorkingString.Replace("KPH", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.KilometersPerHour_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From K/H
            if (WorkingString.EndsWith("K/H"))
            {
                WorkingString = WorkingString.Replace("K/H", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.KilometersPerHour_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From KH
            if (WorkingString.EndsWith("KH"))
            {
                WorkingString = WorkingString.Replace("KH", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.KilometersPerHour_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion

            #region From FPS
            if (WorkingString.EndsWith("FPS"))
            {
                WorkingString = WorkingString.Replace("FPS", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.FeetPerSecond_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From F/S
            if (WorkingString.EndsWith("F/S"))
            {
                WorkingString = WorkingString.Replace("F/S", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.FeetPerSecond_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From FS
            if (WorkingString.EndsWith("FS"))
            {
                WorkingString = WorkingString.Replace("FS", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.FeetPerSecond_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion

            #region From MACH
            if (WorkingString.EndsWith("MACH"))
            {
                WorkingString = WorkingString.Replace("MACH", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Velocity.Conversions.MachAtSeaLevel_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion

            //failed to find an end tag...

            #region <NO TAG>
            Failed = !Double.TryParse(WorkingString, out Value);
            if (Failed) return false;
            Output = Value;
            return true;
            #endregion
        }
        public static bool AsKnots(this string Input, out Velocity.Knots Output)
        {
            double WorkingValue = 0;
            Output = new Velocity.Knots(0);
            bool Failed = !_ConvertVelocity(Input, Velocity.Conversions.Knots_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsKnots();
            return true;
        }
        public static bool AsMetersPerSecond(this string Input, out Velocity.MetersPerSecond Output)
        {
            double WorkingValue = 0;
            Output = new Velocity.MetersPerSecond(0);
            bool Failed = !_ConvertVelocity(Input, Velocity.Conversions.MetersPerSecond_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsMetersPerSecond();
            return true;
        }
        public static bool AsMilesPerHour(this string Input, out Velocity.MilesPerHour Output)
        {
            double WorkingValue = 0;
            Output = new Velocity.MilesPerHour(0);
            bool Failed = !_ConvertVelocity(Input, Velocity.Conversions.MilesPerHour_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsMilesPerHour();
            return true;
        }
        public static bool AsKilometersPerHour(this string Input, out Velocity.KilometersPerHour Output)
        {
            double WorkingValue = 0;
            Output = new Velocity.KilometersPerHour(0);
            bool Failed = !_ConvertVelocity(Input, Velocity.Conversions.KilometersPerHour_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsKilometersPerHour();
            return true;
        }
        public static bool AsFeetPerSecond(this string Input, out Velocity.FeetPerSecond Output)
        {
            double WorkingValue = 0;
            Output = new Velocity.FeetPerSecond(0);
            bool Failed = !_ConvertVelocity(Input, Velocity.Conversions.FeetPerSecond_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsFeetPerSecond();
            return true;
        }
        public static bool AsMachAtSeaLevel(this string Input, out Velocity.MachAtSeaLevel Output)
        {
            double WorkingValue = 0;
            Output = new Velocity.MachAtSeaLevel(0);
            bool Failed = !_ConvertVelocity(Input, Velocity.Conversions.MachAtSeaLevel_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsMachAtSeaLevel();
            return true;
        }
        #endregion
        #endregion
        #region Length
        public static class Length
        {
            public static class Conversions
            {
                public static double Kilometers_ConversionFactor = 0.001;
                public static double Meters_ConversionFactor = 1;
                public static double Centimeters_ConversionFactor = 100;

                public static double Miles_ConversionFactor = 6.21371192237334e-4;
                public static double NauticalMiles_ConversionFactor = 5.399568034557235e-4;

                public static double Yards_ConversionFactor = 1.093613298337708;
                public static double Feet_ConversionFactor = 3.280839895013123;
                public static double Inches_ConversionFactor = 39.37007874015748;
            }

            public class Common
            {
                public double OwnConversionFactor = 0;
                public double Value = 0;

                public Kilometers ToKilometers()
                {
                    return new Kilometers(this.Value / OwnConversionFactor * Conversions.Kilometers_ConversionFactor);
                }
                public Meters ToMeters()
                {
                    return new Meters(this.Value / OwnConversionFactor * Conversions.Meters_ConversionFactor);
                }
                public Centimeters ToCentimeters()
                {
                    return new Centimeters(this.Value / OwnConversionFactor * Conversions.Centimeters_ConversionFactor);
                }
                public Miles ToMiles()
                {
                    return new Miles(this.Value / OwnConversionFactor * Conversions.Miles_ConversionFactor);
                }
                public NauticalMiles ToNauticalMiles()
                {
                    return new NauticalMiles(this.Value / OwnConversionFactor * Conversions.NauticalMiles_ConversionFactor);
                }
                public Yards ToYards()
                {
                    return new Yards(this.Value / OwnConversionFactor * Conversions.Yards_ConversionFactor);
                }
                public Feet ToFeet()
                {
                    return new Feet(this.Value / OwnConversionFactor * Conversions.Feet_ConversionFactor);
                }
                public Inches ToInches()
                {
                    return new Inches(this.Value / OwnConversionFactor * Conversions.Inches_ConversionFactor);
                }
            }

            public class Kilometers : Common
            {
                public Kilometers(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Kilometers_ConversionFactor;
                }

                public static implicit operator Double(Kilometers Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Kilometers(Double Input)
                {
                    return new Kilometers(Input);
                }

                public override string ToString()
                {
                    return base.Value + "KM";
                }
            }
            public class Meters : Common
            {
                public Meters(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Meters_ConversionFactor;
                }

                public static implicit operator Double(Meters Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Meters(Double Input)
                {
                    return new Meters(Input);
                }

                public override string ToString()
                {
                    return base.Value + "M";
                }
            }
            public class Centimeters : Common
            {
                public Centimeters(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Centimeters_ConversionFactor;
                }

                public static implicit operator Double(Centimeters Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Centimeters(Double Input)
                {
                    return new Centimeters(Input);
                }

                public override string ToString()
                {
                    return base.Value + "CM";
                }
            }
            public class Miles : Common
            {
                public Miles(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Miles_ConversionFactor;
                }

                public static implicit operator Double(Miles Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Miles(Double Input)
                {
                    return new Miles(Input);
                }

                public override string ToString()
                {
                    return base.Value + "MI";
                }
            }
            public class NauticalMiles : Common
            {
                public NauticalMiles(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.NauticalMiles_ConversionFactor;
                }

                public static implicit operator Double(NauticalMiles Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator NauticalMiles(Double Input)
                {
                    return new NauticalMiles(Input);
                }

                public override string ToString()
                {
                    return base.Value + "NM";
                }
            }
            public class Yards : Common
            {
                public Yards(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Yards_ConversionFactor;
                }

                public static implicit operator Double(Yards Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Yards(Double Input)
                {
                    return new Yards(Input);
                }

                public override string ToString()
                {
                    return base.Value + "YD";
                }
            }
            public class Feet : Common
            {
                public Feet(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Feet_ConversionFactor;
                }

                public static implicit operator Double(Feet Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Feet(Double Input)
                {
                    return new Feet(Input);
                }

                public override string ToString()
                {
                    return base.Value + "FT";
                }
            }
            public class Inches : Common
            {
                public Inches(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Inches_ConversionFactor;
                }

                public static implicit operator Double(Inches Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Inches(Double Input)
                {
                    return new Inches(Input);
                }

                public override string ToString()
                {
                    return base.Value + "IN";
                }
            }
        }

        #region As... (Double)
        public static Length.Kilometers AsKilometers(this double ThisNumber)
        {
            return new Length.Kilometers(ThisNumber);
        }
        public static Length.Meters AsMeters(this double ThisNumber)
        {
            return new Length.Meters(ThisNumber);
        }
        public static Length.Centimeters AsCentimeters(this double ThisNumber)
        {
            return new Length.Centimeters(ThisNumber);
        }
        public static Length.Miles AsMiles(this double ThisNumber)
        {
            return new Length.Miles(ThisNumber);
        }
        public static Length.NauticalMiles AsNauticalMiles(this double ThisNumber)
        {
            return new Length.NauticalMiles(ThisNumber);
        }
        public static Length.Yards AsYards(this double ThisNumber)
        {
            return new Length.Yards(ThisNumber);
        }
        public static Length.Feet AsFeet(this double ThisNumber)
        {
            return new Length.Feet(ThisNumber);
        }
        public static Length.Inches AsInches(this double ThisNumber)
        {
            return new Length.Inches(ThisNumber);
        }
        #endregion
        #region As... (Int32)
        public static Length.Kilometers AsKilometers(this int ThisNumber)
        {
            return new Length.Kilometers(ThisNumber);
        }
        public static Length.Meters AsMeters(this int ThisNumber)
        {
            return new Length.Meters(ThisNumber);
        }
        public static Length.Centimeters AsCentimeters(this int ThisNumber)
        {
            return new Length.Centimeters(ThisNumber);
        }
        public static Length.Miles AsMiles(this int ThisNumber)
        {
            return new Length.Miles(ThisNumber);
        }
        public static Length.NauticalMiles AsNauticalMiles(this int ThisNumber)
        {
            return new Length.NauticalMiles(ThisNumber);
        }
        public static Length.Yards AsYards(this int ThisNumber)
        {
            return new Length.Yards(ThisNumber);
        }
        public static Length.Feet AsFeet(this int ThisNumber)
        {
            return new Length.Feet(ThisNumber);
        }
        public static Length.Inches AsInches(this int ThisNumber)
        {
            return new Length.Inches(ThisNumber);
        }
        #endregion
        #region As... (String)
        private static bool _ConvertLength(this string Input, double ConversionFactor, out double Output)
        {
            Output = 0;
            double OutputConversionFactor = ConversionFactor;

            double Value = 0;
            string WorkingString = Input.ToUpperInvariant();
            bool Failed = false;

            #region From KM
            if (WorkingString.EndsWith("KM"))
            {
                WorkingString = WorkingString.Replace("KM", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Kilometers_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From M
            if (WorkingString.EndsWith("M"))
            {
                WorkingString = WorkingString.Replace("M", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Meters_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From CM
            if (WorkingString.EndsWith("CM"))
            {
                WorkingString = WorkingString.Replace("CM", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Centimeters_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From MI
            if (WorkingString.EndsWith("MI"))
            {
                WorkingString = WorkingString.Replace("MI", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Miles_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From NM
            if (WorkingString.EndsWith("NM"))
            {
                WorkingString = WorkingString.Replace("NM", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.NauticalMiles_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From YD
            if (WorkingString.EndsWith("YD"))
            {
                WorkingString = WorkingString.Replace("YD", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Yards_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From Y
            if (WorkingString.EndsWith("Y"))
            {
                WorkingString = WorkingString.Replace("Y", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Yards_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From FT
            if (WorkingString.EndsWith("FT"))
            {
                WorkingString = WorkingString.Replace("FT", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Feet_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From IN
            if (WorkingString.EndsWith("IN"))
            {
                WorkingString = WorkingString.Replace("IN", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Length.Conversions.Inches_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            //failed to find an end tag...

            #region <NO TAG>
            Failed = !Double.TryParse(WorkingString, out Value);
            if (Failed) return false;
            Output = Value;
            return true;
            #endregion
        }
        public static bool AsKilometers(this string Input, out Length.Kilometers Output)
        {
            double WorkingValue = 0;
            Output = new Length.Kilometers(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Kilometers_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsKilometers();
            return true;
        }
        public static bool AsMeters(this string Input, out Length.Meters Output)
        {
            double WorkingValue = 0;
            Output = new Length.Meters(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Meters_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsMeters();
            return true;
        }
        public static bool AsCentimeters(this string Input, out Length.Centimeters Output)
        {
            double WorkingValue = 0;
            Output = new Length.Centimeters(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Centimeters_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsCentimeters();
            return true;
        }
        public static bool AsMiles(this string Input, out Length.Miles Output)
        {
            double WorkingValue = 0;
            Output = new Length.Miles(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Miles_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsMiles();
            return true;
        }
        public static bool AsNauticalMiles(this string Input, out Length.NauticalMiles Output)
        {
            double WorkingValue = 0;
            Output = new Length.NauticalMiles(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.NauticalMiles_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsNauticalMiles();
            return true;
        }
        public static bool AsYards(this string Input, out Length.Yards Output)
        {
            double WorkingValue = 0;
            Output = new Length.Yards(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Yards_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsYards();
            return true;
        }
        public static bool AsFeet(this string Input, out Length.Feet Output)
        {
            double WorkingValue = 0;
            Output = new Length.Feet(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Feet_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsFeet();
            return true;
        }
        public static bool AsInches(this string Input, out Length.Inches Output)
        {
            double WorkingValue = 0;
            Output = new Length.Inches(0);
            bool Failed = !_ConvertLength(Input, Length.Conversions.Inches_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsInches();
            return true;
        }
        #endregion
        #endregion
        #region Mass
        public static class Mass
        {
            public static class Conversions
            {
                public static double Tonnes_ConversionFactor = 0.001;
                public static double Kilograms_ConversionFactor = 1;
                public static double Grams_ConversionFactor = 1000;
                public static double Pounds_ConversionFactor = 2.204622621848776;
                public static double Ounces_ConversionFactor = 35.27396194958041;
                public static double Stones_ConversionFactor = 0.1574730444177697;
            }

            public class Common
            {
                public double OwnConversionFactor = 0;
                public double Value = 0;

                public Tonnes ToTonnes()
                {
                    return new Tonnes(this.Value / OwnConversionFactor * Conversions.Tonnes_ConversionFactor);
                }
                public Kilograms ToKilograms()
                {
                    return new Kilograms(this.Value / OwnConversionFactor * Conversions.Kilograms_ConversionFactor);
                }
                public Grams ToGrams()
                {
                    return new Grams(this.Value / OwnConversionFactor * Conversions.Grams_ConversionFactor);
                }
                public Pounds ToPounds()
                {
                    return new Pounds(this.Value / OwnConversionFactor * Conversions.Pounds_ConversionFactor);
                }
                public Ounces ToOunces()
                {
                    return new Ounces(this.Value / OwnConversionFactor * Conversions.Ounces_ConversionFactor);
                }
                public Stones ToStones()
                {
                    return new Stones(this.Value / OwnConversionFactor * Conversions.Stones_ConversionFactor);
                }
            }

            public class Tonnes : Common
            {
                public Tonnes(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Tonnes_ConversionFactor;
                }

                public static implicit operator Double(Tonnes Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Tonnes(Double Input)
                {
                    return new Tonnes(Input);
                }

                public override string ToString()
                {
                    return base.Value + "T";
                }
            }
            public class Kilograms : Common
            {
                public Kilograms(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Kilograms_ConversionFactor;
                }

                public static implicit operator Double(Kilograms Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Kilograms(Double Input)
                {
                    return new Kilograms(Input);
                }

                public override string ToString()
                {
                    return base.Value + "KG";
                }
            }
            public class Grams : Common
            {
                public Grams(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Grams_ConversionFactor;
                }

                public static implicit operator Double(Grams Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Grams(Double Input)
                {
                    return new Grams(Input);
                }

                public override string ToString()
                {
                    return base.Value + "G";
                }
            }
            public class Pounds : Common
            {
                public Pounds(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Pounds_ConversionFactor;
                }

                public static implicit operator Double(Pounds Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Pounds(Double Input)
                {
                    return new Pounds(Input);
                }

                public override string ToString()
                {
                    return base.Value + "LB";
                }
            }
            public class Ounces : Common
            {
                public Ounces(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Ounces_ConversionFactor;
                }

                public static implicit operator Double(Ounces Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Ounces(Double Input)
                {
                    return new Ounces(Input);
                }

                public override string ToString()
                {
                    return base.Value + "OZ";
                }
            }
            public class Stones : Common
            {
                public Stones(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Stones_ConversionFactor;
                }

                public static implicit operator Double(Stones Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Stones(Double Input)
                {
                    return new Stones(Input);
                }

                public override string ToString()
                {
                    return base.Value + "ST";
                }
            }
        }
        #region As... (Double)
        public static Mass.Tonnes AsTonnes(this double ThisNumber)
        {
            return new Mass.Tonnes(ThisNumber);
        }
        public static Mass.Kilograms AsKilograms(this double ThisNumber)
        {
            return new Mass.Kilograms(ThisNumber);
        }
        public static Mass.Grams AsGrams(this double ThisNumber)
        {
            return new Mass.Grams(ThisNumber);
        }
        public static Mass.Pounds AsPounds(this double ThisNumber)
        {
            return new Mass.Pounds(ThisNumber);
        }
        public static Mass.Ounces AsOunces(this double ThisNumber)
        {
            return new Mass.Ounces(ThisNumber);
        }
        public static Mass.Stones AsStones(this double ThisNumber)
        {
            return new Mass.Stones(ThisNumber);
        }
        #endregion
        #region As... (Int32)
        public static Mass.Tonnes AsTonnes(this int ThisNumber)
        {
            return new Mass.Tonnes(ThisNumber);
        }
        public static Mass.Kilograms AsKilograms(this int ThisNumber)
        {
            return new Mass.Kilograms(ThisNumber);
        }
        public static Mass.Grams AsGrams(this int ThisNumber)
        {
            return new Mass.Grams(ThisNumber);
        }
        public static Mass.Pounds AsPounds(this int ThisNumber)
        {
            return new Mass.Pounds(ThisNumber);
        }
        public static Mass.Ounces AsOunces(this int ThisNumber)
        {
            return new Mass.Ounces(ThisNumber);
        }
        public static Mass.Stones AsStones(this int ThisNumber)
        {
            return new Mass.Stones(ThisNumber);
        }
        #endregion
        #region As... (String)
        private static bool _ConvertMass(this string Input, double ConversionFactor, out double Output)
        {
            Output = 0;
            double OutputConversionFactor = ConversionFactor;

            double Value = 0;
            string WorkingString = Input.ToUpperInvariant();
            bool Failed = false;

            #region From T
            if (WorkingString.EndsWith("T"))
            {
                WorkingString = WorkingString.Replace("T", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Mass.Conversions.Tonnes_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From KG
            if (WorkingString.EndsWith("KG"))
            {
                WorkingString = WorkingString.Replace("KG", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Mass.Conversions.Kilograms_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From G
            if (WorkingString.EndsWith("G"))
            {
                WorkingString = WorkingString.Replace("G", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Mass.Conversions.Grams_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From LB
            if (WorkingString.EndsWith("LB"))
            {
                WorkingString = WorkingString.Replace("LB", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Mass.Conversions.Pounds_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From OZ
            if (WorkingString.EndsWith("OZ"))
            {
                WorkingString = WorkingString.Replace("OZ", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Mass.Conversions.Ounces_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From ST
            if (WorkingString.EndsWith("ST"))
            {
                WorkingString = WorkingString.Replace("ST", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Mass.Conversions.Stones_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            //failed to find an end tag...

            #region <NO TAG>
            Failed = !Double.TryParse(WorkingString, out Value);
            if (Failed) return false;
            Output = Value;
            return true;
            #endregion
        }
        public static bool AsTonnes(this string Input, out Mass.Tonnes Output)
        {
            double WorkingValue = 0;
            Output = new Mass.Tonnes(0);
            bool Failed = !_ConvertMass(Input, Mass.Conversions.Tonnes_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsTonnes();
            return true;
        }
        public static bool AsKilograms(this string Input, out Mass.Kilograms Output)
        {
            double WorkingValue = 0;
            Output = new Mass.Kilograms(0);
            bool Failed = !_ConvertMass(Input, Mass.Conversions.Kilograms_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsKilograms();
            return true;
        }
        public static bool AsGrams(this string Input, out Mass.Grams Output)
        {
            double WorkingValue = 0;
            Output = new Mass.Grams(0);
            bool Failed = !_ConvertMass(Input, Mass.Conversions.Grams_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsGrams();
            return true;
        }
        public static bool AsPounds(this string Input, out Mass.Pounds Output)
        {
            double WorkingValue = 0;
            Output = new Mass.Pounds(0);
            bool Failed = !_ConvertMass(Input, Mass.Conversions.Pounds_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsPounds();
            return true;
        }
        public static bool AsOunces(this string Input, out Mass.Ounces Output)
        {
            double WorkingValue = 0;
            Output = new Mass.Ounces(0);
            bool Failed = !_ConvertMass(Input, Mass.Conversions.Ounces_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsOunces();
            return true;
        }
        public static bool AsStones(this string Input, out Mass.Stones Output)
        {
            double WorkingValue = 0;
            Output = new Mass.Stones(0);
            bool Failed = !_ConvertMass(Input, Mass.Conversions.Stones_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsStones();
            return true;
        }

        #endregion
        #endregion
        #region Angles
        public static class Angles
        {
            public static class Conversions
            {
                public static double Degrees_ConversionFactor = 1;
                public static double Radians_ConversionFactor = 0.0174532925199433;
                public static double Gradians_ConversionFactor = 1.111111111111111;
            }

            public class Common
            {
                public double OwnConversionFactor = 0;
                public double Value = 0;

                public Degrees ToDegrees()
                {
                    return new Degrees(this.Value / OwnConversionFactor * Conversions.Degrees_ConversionFactor);
                }
                public Radians ToRadians()
                {
                    return new Radians(this.Value / OwnConversionFactor * Conversions.Radians_ConversionFactor);
                }
                public Gradians ToGradians()
                {
                    return new Gradians(this.Value / OwnConversionFactor * Conversions.Gradians_ConversionFactor);
                }
            }

            public class Degrees : Common
            {
                public Degrees(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Degrees_ConversionFactor;
                }

                public static implicit operator Double(Degrees Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Degrees(Double Input)
                {
                    return new Degrees(Input);
                }

                public override string ToString()
                {
                    return base.Value + "DEG";
                }
            }
            public class Radians : Common
            {
                public Radians(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Radians_ConversionFactor;
                }

                public static implicit operator Double(Radians Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Radians(Double Input)
                {
                    return new Radians(Input);
                }

                public override string ToString()
                {
                    return base.Value + "RAD";
                }
            }
            public class Gradians : Common
            {
                public Gradians(double Input)
                {
                    base.Value = Input;
                    base.OwnConversionFactor = Conversions.Gradians_ConversionFactor;
                }

                public static implicit operator Double(Gradians Input)
                {
                    return (Double)Input.Value;
                }

                public static implicit operator Gradians(Double Input)
                {
                    return new Gradians(Input);
                }

                public override string ToString()
                {
                    return base.Value + "GRAD";
                }
            }
        }
        #region As... (Double)
        public static Angles.Degrees AsDegrees(this double ThisNumber)
        {
            return new Angles.Degrees(ThisNumber);
        }
        public static Angles.Radians AsRadians(this double ThisNumber)
        {
            return new Angles.Radians(ThisNumber);
        }
        public static Angles.Gradians AsGradians(this double ThisNumber)
        {
            return new Angles.Gradians(ThisNumber);
        }
        #endregion
        #region As... (Int32)
        public static Angles.Degrees AsDegrees(this int ThisNumber)
        {
            return new Angles.Degrees(ThisNumber);
        }
        public static Angles.Radians AsRadians(this int ThisNumber)
        {
            return new Angles.Radians(ThisNumber);
        }
        public static Angles.Gradians AsGradians(this int ThisNumber)
        {
            return new Angles.Gradians(ThisNumber);
        }
        #endregion
        #region As... (String)
        private static bool _ConvertAngle(this string Input, double ConversionFactor, out double Output)
        {
            Output = 0;
            double OutputConversionFactor = ConversionFactor;

            double Value = 0;
            string WorkingString = Input.ToUpperInvariant();
            bool Failed = false;

            #region From DEG
            if (WorkingString.EndsWith("DEG"))
            {
                WorkingString = WorkingString.Replace("DEG", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Angles.Conversions.Degrees_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From D
            if (WorkingString.EndsWith("D"))
            {
                WorkingString = WorkingString.Replace("D", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Angles.Conversions.Degrees_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From RAD
            if (WorkingString.EndsWith("RAD"))
            {
                WorkingString = WorkingString.Replace("RAD", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Angles.Conversions.Radians_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From R
            if (WorkingString.EndsWith("R"))
            {
                WorkingString = WorkingString.Replace("R", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Angles.Conversions.Radians_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From GRAD
            if (WorkingString.EndsWith("GRAD"))
            {
                WorkingString = WorkingString.Replace("GRAD", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Angles.Conversions.Gradians_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            #region From G
            if (WorkingString.EndsWith("G"))
            {
                WorkingString = WorkingString.Replace("G", "");
                Failed = !Double.TryParse(WorkingString, out Value);
                if (Failed) return false;
                Value =
                    Value /
                    Numbers.Angles.Conversions.Gradians_ConversionFactor *
                    OutputConversionFactor;
                Output = Value;
                return true;
            }
            #endregion
            //failed to find an end tag...

            #region <NO TAG>
            Failed = !Double.TryParse(WorkingString, out Value);
            if (Failed) return false;
            Output = Value;
            return true;
            #endregion
        }
        public static bool AsDegrees(this string Input, out Angles.Degrees Output)
        {
            double WorkingValue = 0;
            Output = new Angles.Degrees(0);
            bool Failed = !_ConvertAngle(Input, Angles.Conversions.Degrees_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsDegrees();
            return true;
        }
        public static bool AsRadians(this string Input, out Angles.Radians Output)
        {
            double WorkingValue = 0;
            Output = new Angles.Radians(0);
            bool Failed = !_ConvertAngle(Input, Angles.Conversions.Radians_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsRadians();
            return true;
        }
        public static bool AsGradians(this string Input, out Angles.Gradians Output)
        {
            double WorkingValue = 0;
            Output = new Angles.Gradians(0);
            bool Failed = !_ConvertAngle(Input, Angles.Conversions.Gradians_ConversionFactor, out WorkingValue);
            if (Failed) return false;
            Output = WorkingValue.AsGradians();
            return true;
        }

        #endregion
        #endregion


        #region OYSTime
        public static TimeSpan AsDays(this double ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 24 * 60 * 60 * 1000));
        }
        public static TimeSpan AsHour(this double ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 60 * 60 * 1000));
        }
        public static TimeSpan AsMinutes(this double ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 60 * 1000));
        }
        public static TimeSpan AsSeconds(this double ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 1000));
        }
        public static TimeSpan AsMicroseconds(this double ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber));
        }

        public static TimeSpan AsDays(this int ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 24 * 60 * 60 * 1000));
        }
        public static TimeSpan AsHour(this int ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 60 * 60 * 1000));
        }
        public static TimeSpan AsMinutes(this int ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 60 * 1000));
        }
        public static TimeSpan AsSeconds(this int ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber * 1000));
        }
        public static TimeSpan AsMicroseconds(this int ThisNumber)
        {
            return new TimeSpan(0, 0, 0, 0, (int)(ThisNumber));
        }

        public static string AsFormattedTimeDifference(this float Difference)
        {
            TimeSpan TS = new TimeSpan(0, 0, 0, 0, (int)(Math.Abs(Difference) * 1000));
            string Out = "";
            if (TS.TotalDays >= 1)
            {
                Out += "N/A";
            }
            Out += Difference >= 0 ? "+" : "-";
            if (TS.Hours >= 1)
            {
                string Hours = ((int)(TS.Hours)).ToString();
                if (Hours.Length > 2) Hours = "99";
                while (Hours.Length < 2) Hours = "0" + Hours;
                Out += Hours + ":";
            }
            if (TS.Minutes >= 1)
            {
                string Minutes = ((int)(TS.Minutes)).ToString();
                if (Minutes.Length > 2) Minutes = "99";
                while (Minutes.Length < 2) Minutes = "0" + Minutes;
                Out += Minutes + ":";
            }
            string Seconds = ((int)(TS.Seconds)).ToString();
            if (Seconds.Length > 2) Seconds = "99";
            if (TS.TotalMinutes >= 1)
            {
                while (Seconds.Length < 2) Seconds = "0" + Seconds;
            }
            Out += Seconds + ".";
            string Milliseconds = ((int)(TS.Milliseconds)).ToString();
            if (Milliseconds.Length > 3) Milliseconds = "999";
            while (Milliseconds.Length < 3) Milliseconds = "0" + Milliseconds;
            Out += Milliseconds;
            if (TS.TotalMilliseconds < 0)
            {
                Out = "N/A";
            }
            return Out;
        }
        #endregion
        #region YSAngles
        //YSSHORTDEG
        //DEGREES
        //YSFLOATRAD
        public static float ToDegrees(this short Input)
        {
            float Out = ((ushort)Input / 65536f * 360);
            if (Out > 180) Out -= 360;
            return Out;
        }

        public static short ToYSDegrees(this Int32 Input)
        {
            int Out = (int)((float)Input / 360f * 65536f);
            if (Out > 32767) Out -= 32767;
            return (short)Out;
        }

        public static float ToYSRadians(this Int32 Input)
        {
            double Out = ((double)Input / 180f * Math.PI);
            if (Out > Math.PI) Out -= Math.PI;
            return (float)Out;
        }

        public static float ToDegrees(this ushort Input)
        {
            return (Input / 65536f * 360);
        }

        public static float AngleAcuteDifference(float a1, float a2)
        {
            float difference = a1 - a2;
            while (difference < -180) difference += 360;
            while (difference > 180) difference -= 360;
            return difference;
        }
        #endregion

    }
    #endregion

    #region Debug
    public static partial class Debug
    {
        public static string GetStackTrace(Exception e)
        {
            System.Environment.SetEnvironmentVariable("_NT_SYMBOL_PATH", "./Debug/");
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            string output = "";
            output += "&cMESSAGE:    &e\n    " + e.Message + "\n";
            output += "&cMETHOD:     &e\n    " + frame.GetMethod() + "\n";
            output += "&cLINENUMBER: &e\n    " + frame.GetFileLineNumber() + "\n";
            output += "&cCOLUMNNUMBER: &e\n    " + frame.GetFileColumnNumber() + "\n";
            output += "&cSTACKTRACE: &e\n" + st.ToString().ReplaceAll("   at ", "    ") + "\n";
            output += "\n";
            return output;
        }

        public static string GetSourceCodePosition()
        {
            int depth = 1;
            return GetSourceCodePosition(depth);
        }

        public static string GetSourceCodePosition(int depth)
        {
            System.Environment.SetEnvironmentVariable("_NT_SYMBOL_PATH", "./Debug/");
            StackFrame SF = new StackFrame(depth + 1, true);
            return "&c" + SF.GetFileColumnNumber() + ":" + SF.GetFileLineNumber();
            //return "&e" + SF.GetFileName() + "\n    &c" + SF.GetFileColumnNumber() + ":" + SF.GetFileLineNumber();
        }

        /// <summary>
        /// Returns a position indexed list of each byte in the array for debugging purposes.
        /// </summary>
        /// <param name="ByteString"></param>
        /// <returns></returns>
        public static string _ToDebugHexString(this byte[] ByteString)
        {
            string output = "";
            int i = 0;
            foreach (byte ThisByte in ByteString)
            {
                string number = i.ToString();
                while (number.Length <= 3)
                {
                    number = "0" + number;
                }
                output += "" + number + "(" + "";
                output += BitConverter.ToString(new byte[] { ThisByte });
                output += ")-";
                i++;
            }
            return output;
        }


        public static string ToDebugHexString(this byte[] ByteString)
        {
            string output = "";
            int i = 0;
            foreach (byte ThisByte in ByteString)
            {
                string Letter = "f";
                if (i % 8 == 0) output += "\n";
                if (i % 16 == 15) Letter = "a";
                if (i % 16 == 14) Letter = "a";
                if (i % 16 == 13) Letter = "a";
                if (i % 16 == 12) Letter = "a";
                if (i % 16 == 11) Letter = "b";
                if (i % 16 == 10) Letter = "b";
                if (i % 16 == 09) Letter = "b";
                if (i % 16 == 08) Letter = "b";
                if (i % 16 == 07) Letter = "c";
                if (i % 16 == 06) Letter = "c";
                if (i % 16 == 05) Letter = "c";
                if (i % 16 == 04) Letter = "c";
                if (i % 16 == 03) Letter = "d";
                if (i % 16 == 02) Letter = "d";
                if (i % 16 == 01) Letter = "d";
                if (i % 16 == 00) Letter = "d";
                string number = i.ToString();
                while (number.Length < 4)
                {
                    number = "0" + number;
                }
                output += "&8" + number + "&7(" + "&" + Letter;
                output += BitConverter.ToString(new byte[] { ThisByte });
                output += "&7)-";
                i++;
                //if (i > 15) i = 0;
            }
            return output;
        }

        /// <summary>
        /// Returns a position indexed list of each byte in the array for debugging purposes.
        /// </summary>
        /// <param name="ByteString"></param>
        /// <returns></returns>
        public static string ToColoredDebugHexString(this byte[] ByteString)
        {
            string output = "";
            int i = 0;
            foreach (byte ThisByte in ByteString)
            {
                string Letter = "f";
                if (i % 8 == 0 && i > 0) output += "\n";
                if (ThisByte.GetTens() == 0x0F) Letter = "F";
                if (ThisByte.GetTens() == 0x0E) Letter = "E";
                if (ThisByte.GetTens() == 0x0D) Letter = "D";
                if (ThisByte.GetTens() == 0x0C) Letter = "C";
                if (ThisByte.GetTens() == 0x0B) Letter = "B";
                if (ThisByte.GetTens() == 0x0A) Letter = "A";
                if (ThisByte.GetTens() == 0x09) Letter = "9";
                if (ThisByte.GetTens() == 0x08) Letter = "8";
                if (ThisByte.GetTens() == 0x07) Letter = "7";
                if (ThisByte.GetTens() == 0x06) Letter = "6";
                if (ThisByte.GetTens() == 0x05) Letter = "5";
                if (ThisByte.GetTens() == 0x04) Letter = "4";
                if (ThisByte.GetTens() == 0x03) Letter = "3";
                if (ThisByte.GetTens() == 0x02) Letter = "2";
                if (ThisByte.GetTens() == 0x01) Letter = "1";
                if (ThisByte.GetTens() == 0x00) Letter = "0";
                string number = i.ToString();
                while (number.Length < 4)
                {
                    number = "0" + number;
                }
                output += "&8" + number + "&7(" + "&" + Letter;
                output += BitConverter.ToString(new byte[] { ThisByte })[0].ToString();

                Letter = "F";
                //if (i % 8 == 0) output += "\n";
                if (ThisByte.GetUnits() == 0x0F) Letter = "F";
                if (ThisByte.GetUnits() == 0x0E) Letter = "E";
                if (ThisByte.GetUnits() == 0x0D) Letter = "D";
                if (ThisByte.GetUnits() == 0x0C) Letter = "C";
                if (ThisByte.GetUnits() == 0x0B) Letter = "B";
                if (ThisByte.GetUnits() == 0x0A) Letter = "A";
                if (ThisByte.GetUnits() == 0x09) Letter = "9";
                if (ThisByte.GetUnits() == 0x08) Letter = "8";
                if (ThisByte.GetUnits() == 0x07) Letter = "7";
                if (ThisByte.GetUnits() == 0x06) Letter = "6";
                if (ThisByte.GetUnits() == 0x05) Letter = "5";
                if (ThisByte.GetUnits() == 0x04) Letter = "4";
                if (ThisByte.GetUnits() == 0x03) Letter = "3";
                if (ThisByte.GetUnits() == 0x02) Letter = "2";
                if (ThisByte.GetUnits() == 0x01) Letter = "1";
                if (ThisByte.GetUnits() == 0x00) Letter = "0";
                output += "&" + Letter;
                output += BitConverter.ToString(new byte[] { ThisByte })[1].ToString();

                output += "&7)-";
                i++;
                //if (i > 15) i = 0;
            }
            return output;
        }

        private static DateTime LastPoll = DateTime.Now;
        public static void PollEfficiency()
        {
            System.Diagnostics.Debug.WriteLine((DateTime.Now - LastPoll).TotalSeconds);
            LastPoll = DateTime.Now;
        }

        public static void StartEfficiency()
        {
            LastPoll = DateTime.Now;
        }

        /// <summary>
        /// Declares a Debug Test Point at this location in the code - no effect, but searching DTP() in the release will show these test points in order to make sure there are no useless DTP's in the code when releasing!
        /// </summary>
        public static void TestPoint()
        {
            //do nothing, declare a debug test point - makes easy to find debugging code to remove in future
            return;
        }

        public static void WriteLine(object Message)
        {
#if !DEBUG
            return;
#endif
#if DEBUG
            System.Environment.SetEnvironmentVariable("_NT_SYMBOL_PATH", "./Debug/");
            StackFrame SF = new StackFrame(1, true);
            string MethodName = SF.GetMethod().Name;
            System.Diagnostics.Debug.WriteLine(MethodName + "> " + Strings.StripFormatting(Message.ToString()));
#endif
        }

        public static void BugReport(object Message)
        {

        }

        public static void ShowPosition()
        {
            System.Environment.SetEnvironmentVariable("_NT_SYMBOL_PATH", "./Debug/");
            StackFrame SF = new StackFrame(1, true);
            string MethodName = SF.GetMethod().Name;
            System.Diagnostics.Debug.WriteLine(MethodName + "> " + SF.GetFileLineNumber() + ":" + SF.GetFileColumnNumber());
        }

        public static void SimulateLag(int MicroSeconds)
        {
            Random RandomNumberGenerator = new Random();
            const int Precision = 100000;
            double Factor = RandomNumberGenerator.Next(-Precision, Precision) / Precision / 2;
            Thread.Sleep(MicroSeconds + (int)(Factor * MicroSeconds));
        }

        public static void ShowStackTrace()
        {
            System.Environment.SetEnvironmentVariable("_NT_SYMBOL_PATH", "./Debug/");
            StackFrame SF = new StackFrame(1, true);
            StackTrace ST = new StackTrace(true);
            string MethodName = SF.GetMethod().Name;
            System.Diagnostics.Debug.WriteLine(MethodName + "> " + SF.GetFileLineNumber() + ":" + SF.GetFileColumnNumber());
            foreach (string Line in ST.ToString().Split('\n'))
            {
                System.Diagnostics.Debug.WriteLine("    " + Line.Replace('\n', ' '));
            }
        }

        public static void Console_ShowAllDebugTestPoints()
        {
            List<string> Output = new List<string>();
            string ProjectPath = Directory.GetCurrentDirectory() + "/" + "../../../..";
            string[] AllSourceFiles = Directory.GetFiles(ProjectPath, "*.cs", System.IO.SearchOption.AllDirectories);
            for (int i = 0; i < AllSourceFiles.Count(); i++)
            {
                //for each file...

                string[] Contents = Files.FileReadAllLines(AllSourceFiles[i]);
                for (int j = 0; j < Contents.Count(); j++)
                {
                    //for each line in that file...
                    if (Contents[j].ToUpperInvariant().Contains("DEBUG" + "." + "TESTPOINT"))
                    {
                        //Add that to the list.
                        Output.Add("    " + Path.GetFileName(AllSourceFiles[i]) + ": Line " + j);
                    }
                }
            }
            for (int i = 0; i < Output.Count; i++)
            {
                if (i == 0)
                {
                    System.Console.ForegroundColor = ConsoleColor.Yellow;
                    System.Console.WriteLine("There are a total of " + Output.Count + " active testpoints in the code:");
                }
                System.Console.WriteLine(Output[i]);
            }
            if (Output.Count == 0)
            {
                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine("There are 0 active testpoints in the code.");
            }
            System.Console.ForegroundColor = ConsoleColor.White;
        }
    }
    #endregion
}
