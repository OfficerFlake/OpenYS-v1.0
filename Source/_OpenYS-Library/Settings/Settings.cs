using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace OpenYS
{
    public static class Settings
    {
        public static class Loading
        {
            public static string YSFlightDirectory = "C:/Program Files/YSFLIGHT.COM/YSFLIGHT/";
            public static uint YSFNetcodeVersion = 20110207;
            public static uint OYSNetcodeVersion = 20150207;
            public static string FieldName = "HAWAII";
            public static bool AutoOPs = true;
            public static bool SendConnectedWelcomeMessage = true;
            public static bool SendLogInCompleteWelcomeMessage = true;
            public static bool KickBots = true;
            public static bool BotPingMessages = true;
            public static bool SendLoadingPercentNotification = true;
            public static bool SendLoginCompleteNotification = true;
            public static uint AircraftListPacketSize = 32;
        }

        public static class Server
        {
            public static bool ConnectionLocked = false;
            public static bool JoinLocked = false;
            public static uint HostPort = 7914;
            public static IPAddress HostAddress = IPAddress.Parse("127.0.0.1");
            public static uint ListenerPort = 7915;
            public static int RestartTimer = 120;
        }

        public static class Flight
        {
            public static bool EnableSmoke = true;
            public static bool EnableMidAirRefueling = true;
            public static bool JoinFlightNotification = true;
            public static bool LeaveFlightNotification = true;
            public static bool SpawnChocks = true;
        }

        public static class Weather
        {
            //public static string Lighting = "DAY";
            public static uint Time = 1200;
            public static float WindX = 0;
            public static float WindY = 0;
            public static float WindZ = 0;
            public static float Fog = 20000;

            public static class Advanced
            {
                public static bool VariableWeather = false;
                public static int DayLength = 0;
                public static Colors.XRGBColor SkyColor = new Colors.XRGBColor(180, 184, 186);
                public static Colors.XRGBColor FogColor = new Colors.XRGBColor(120, 140, 160);
                public static Colors.XRGBColor GndColor = new Colors.XRGBColor(94, 117, 109);
                public static bool EnableSkyColor = false;
                public static bool EnableFogColor = false;
                public static bool EnableGndColor = false;

                public static class EnvironmentColors
                {
                    public static Colors.XRGBColor NightSkyColor = new Colors.XRGBColor(48, 0, 96);
                    public static Colors.XRGBColor NightHorizonColor = new Colors.XRGBColor(48, 0, 96);

                    public static Colors.XRGBColor DawnSkyColor = new Colors.XRGBColor(200, 85, 200);
                    public static Colors.XRGBColor DawnHorizonColor = new Colors.XRGBColor(240, 200, 90);

                    public static Colors.XRGBColor DaySkyColor = new Colors.XRGBColor(23, 106, 189);
                    public static Colors.XRGBColor DayHorizonColor = new Colors.XRGBColor(120, 140, 160);

                    public static Colors.XRGBColor DuskSkyColor = new Colors.XRGBColor(128, 0, 32);
                    public static Colors.XRGBColor DuskHorizonColor = new Colors.XRGBColor(255, 160, 0);

                    public static Colors.XRGBColor MaxAltitudeSkyColor = new Colors.XRGBColor(48, 0, 96);
                    public static Colors.XRGBColor MaxAltitudeHorizonColor = new Colors.XRGBColor(23, 106, 189);

                    public static Colors.XRGBColor NoAtmosphereSkyColor = new Colors.XRGBColor(0, 0, 0);
                    public static Colors.XRGBColor NoAtmosphereHorizonColor = new Colors.XRGBColor(0, 0, 5);

                    public static Colors.XRGBColor WhiteFogColor = new Colors.XRGBColor(160, 160, 160);

                    public static double OverallBlendFactor = 1.0;
                    public static double NightColorFactor = 0.12;


                }

                public static class AtmosphericFading
                {
                    public static double MinimumAltitudeInMeters = 12000;
                    public static double MaximumAltitudeInMeters = 30000;
                }

                public static class ChangeLimitAbsolute
                {
                    public static float WindX = 10;
                    public static float WindY = 10;
                    public static float WindZ = 10;
                    public static float Fog = 400;
                }

                public static class AccelerationLimit
                {
                    public static float WindX = 5;
                    public static float WindY = 5;
                    public static float WindZ = 5;
                    public static float Fog = 1000;
                }

                public static int TurbulencePercent = 50;
            }
        }

        public static class Weapons
        {
            public static bool Missiles = true;
            public static bool Unguided = true;
        }

        public static class Options
        {
            public static bool NoExternalAirView = false;
            public static bool AllowOYSFramework = true;
            public static bool AllowGrounds = true;
            public static bool AllowOPs = true;
            public static bool AllowListUsers = true;
            public static uint RadarBaseAltitude = 0;
            public static string UsernameDistance = "TRUE";
            public static string ConsoleName = "OpenYS Console";
            public static string SchedulerName = "OpenYS Scheduler";
            public static string OwnerName = "???";
            public static string OwnerEmail = "???";
            public static string ServerName = "OpenYS Server";
            public static bool IsCustomBuild = false;

            public static class Control
            {
                public static bool LandEverywhere = false;
                public static bool Collisions = false;
                public static bool BlackOut = false;
                public static bool Fog = false;
            }

            public static class Enable
            {
                public static bool LandEverywhere = true;
                public static bool Collisions = false;
                public static bool BlackOut = false;
                public static bool Fog = true;
            }

        }

        public static class Novelties
        {
            public static bool LaunchBombsFromGuns = false;
        }

        public static class Administration
        {
            public static string AdminPassword = "";
        }

        public static class IRC
        {
            public static bool Enabled = false;
            public static string Name = "OpenYS";
            public static IPAddress HostIP = IPAddress.Parse("127.0.0.1");
            public static int HostPort = 6667;
            public static string Channels = "None";
            public static bool ForwardIRCToServer = true;
            public static bool ForwardsServerToIRC = true;
            public static bool ShowEvents = true;
            public static bool RegisteredNick = false;
            public static string RegistrationServiceName = "NickServ";
            public static string RegistrationAuthPass = "PASSWORD";
            public static string IRCMessagesColor = "&d";
            public static bool IRCToServerStripFormatting = false;
            public static bool ServerToIRCStripFormatting = false;
            public static int DelayTime = 1000;
            public static int BotCount = 1;
        }

        public static class YSFHQ
        {
            public static string Username = "";
            public static string EncryptedPassword = "";
        }
    }
}
