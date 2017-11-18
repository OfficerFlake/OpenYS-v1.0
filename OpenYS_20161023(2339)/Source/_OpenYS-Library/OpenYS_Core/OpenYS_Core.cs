using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace OpenYS
{
    public static partial class OpenYS
    {
        public static Client OpenYSConsole = new Client(null, null);
        public static Assembly Assembly = Assembly.GetEntryAssembly();

        public static Client OpenYSBackGround = new Client(null, null);

        public static Thread ResetTimerThread; //DEPRECATED

        public static DateTime TimeStarted = DateTime.Now;

        public static class YSFServer
        {
            public static IPEndPoint EndPoint
            {
                get
                {
                    return new IPEndPoint(Settings.Server.HostAddress, (int)Settings.Server.HostPort);
                }
            }
        }

        public static class YSFListener
        {
            public static IPEndPoint EndPoint
            {
                get
                {
                    return new IPEndPoint(IPAddress.Any, (int)Settings.Server.ListenerPort);
                }
            }
            public static ManualResetEvent ServerStarted = new ManualResetEvent(false);
        }

        [Flags]
        public enum _BuildType
        {
            None =          0,
            Client =        1 << 0,
            Server =        1 << 2,
            Tray =          1 << 3,
        }

        public static _BuildType BuildType = _BuildType.None;

        #region Weather
        public static Packets.Packet_33_Weather _Weather
        {
            get
            {
                uint LightingMode = 0;
                uint Time = CurrentTick/10;
                if (Time >= 0700 && Time < 1900) LightingMode = 0;
                else LightingMode = 65537;

                Packets.Packet_33_Weather WeatherPacket = new Packets.Packet_33_Weather(false)
                {
                    Lighting = LightingMode, //0 = Day, 65537 == Night.
                    Control_LandEverywhere = Settings.Options.Control.LandEverywhere, //Forces the client to obay the servers choice for land everywhere.
                    Control_Collisions = Settings.Options.Control.Collisions, //Forces the client to obay the servers choice for collsions.
                    Control_BlackOut = Settings.Options.Control.BlackOut, //Forces the client to obay the servers choice for blackout.
                    Control_Fog = Settings.Options.Control.Fog, //No idea... You don't have to enable this but it doesn't appear to make a difference.
                    Enable_LandEverywhere = Settings.Options.Enable.LandEverywhere, //Server enabled land everywhere. client may reject unless force option is set above.
                    Enable_Collisions = Settings.Options.Enable.Collisions, //Server enabled collisions. client may reject unless force option is set above.
                    Enable_BlackOut = Settings.Options.Enable.BlackOut, //Server enabled blackout. client may reject unless force option is set above.
                    Enable_Fog = Settings.Options.Enable.Fog, //Server enabled fog. appears to be always forced. Leave this on if you want the server to have fog enabled.
                    WindX = Settings.Weather.WindX, // Wind speed on X (West/East) axis in meters per second.
                    WindY = Settings.Weather.WindY, // Wind speed on Y (Up/Down) axis in meters per second.
                    WindZ = Settings.Weather.WindZ, // Wind speed on Z (North/South) axis in meters per second.
                    Fog = Settings.Weather.Fog, // Visibility in meters. Only effective if enable fog is set.
                };
                return WeatherPacket;
            }
            set
            {
                Settings.Weather.Time = GetServerTimeUint();
                Settings.Options.Control.LandEverywhere = value.Control_LandEverywhere;
                Settings.Options.Control.Collisions = value.Control_Collisions;
                Settings.Options.Control.BlackOut = value.Control_BlackOut;
                Settings.Options.Control.Fog = value.Control_Fog;
                Settings.Options.Enable.LandEverywhere = value.Enable_LandEverywhere;
                Settings.Options.Enable.Collisions = value.Enable_Collisions;
                Settings.Options.Enable.BlackOut = value.Enable_BlackOut;
                Settings.Options.Enable.Fog = value.Enable_Fog;
                Settings.Weather.WindX = value.WindX;
                Settings.Weather.WindY = value.WindY;
                Settings.Weather.WindZ = value.WindZ;
                Settings.Weather.Fog = value.Fog;
            }
        }

        public static Packets.Packet_33_Weather Weather
        {
            get
            {
                if (AdvancedWeatherOptions._Weather_Variable)
                {
                    return AdvancedWeatherOptions.GetUpdatedWeather();
                    //return the variable weather calculation.
                }
                return _Weather;
            }
            set
            {
                _Weather = value;
                //The current weather as represented in the weather packet.
                AdvancedWeatherOptions._WindX = value.WindX; //m/s
                AdvancedWeatherOptions._WindY = value.WindY; //m/s
                AdvancedWeatherOptions._WindZ = value.WindZ; //m/s
                AdvancedWeatherOptions._Fog__ = value.Fog; //m
                AdvancedWeatherOptions._WindX_DUDX1 = 0; //m/s
                AdvancedWeatherOptions._WindY_DUDX1 = 0; //m/s
                AdvancedWeatherOptions._WindZ_DUDX1 = 0; //m/s
                AdvancedWeatherOptions._Fog___DUDX1 = 0; //m
            }
        }

        public static void UpdateWeather()
        {
            foreach (Client ThisClient in Clients.AllClients.ToArray())
            {
                ThisClient.SendPacket(Weather);
            }
        }

        public static string GetWeatherTAF()
        {
            int Angle;
            int Speed;

            string _Angle;
            string _Speed;

            Angle = 90 - (int)(Math.Atan2(-OpenYS.Weather.WindZ, -OpenYS.Weather.WindX) / Math.PI * 180);
            while (Angle < 0) Angle += 360;
            while (Angle >= 360) Angle -= 360;

            Speed = (int)(Math.Sqrt(Math.Pow(OpenYS.Weather.WindZ, 2) + Math.Pow(OpenYS.Weather.WindX, 2)) * 1.943844492440605);

            if (Speed <= 0) Angle = 0;
            _Angle = Angle.ToString();
            if (_Angle.Length > 3) _Angle = "XXX";
            while (_Angle.Length < 3) _Angle = "0" + _Angle;
            _Speed = Speed.ToString();
            if (_Speed.Length > 2) _Speed = "99";
            while (_Speed.Length < 2) _Speed = "0" + _Speed;

            return _Angle + _Speed + "KT";
        }

        public static class AdvancedWeatherOptions
        {
            public static bool _Weather_Variable {
                get
                {
                    return Settings.Weather.Advanced.VariableWeather; //Off by default!
                }
                set
                {
                    Settings.Weather.Advanced.VariableWeather = value;
                }
            }
            public static int _DayNightCycleLength
            {
                get
                {
                    return Settings.Weather.Advanced.DayLength; //Off by default!
                }
                set
                {
                    Settings.Weather.Advanced.DayLength = value;
                }
            }
            public static DateTime _LastDayNightCycleRestart = DateTime.Now;

            public static Colors.XRGBColor SkyColor
            {
                get
                {
                    return Settings.Weather.Advanced.SkyColor; //Overwritten by the loading process
                }
                set
                {
                    Settings.Weather.Advanced.SkyColor = value;
                }
            }
            public static Colors.XRGBColor FogColor
            {
                get
                {
                    return Settings.Weather.Advanced.FogColor; //Overwritten by the loading process
                }
                set
                {
                    Settings.Weather.Advanced.FogColor = value;
                }
            }
            public static Colors.XRGBColor GndColor
            {
                get
                {
                    return Settings.Weather.Advanced.GndColor; //Overwritten by the loading process
                }
                set
                {
                    Settings.Weather.Advanced.GndColor = value;
                }
            }

            //NOTE - THE VARIABLE WEATHER DEPENDS ON THE BASE WEATHER! SET YOUR BASE WEATHER FIRST THEN CONFIGURE THE BELOW!
            //NOTE - THE UPDATED WEATHER TURNS THE CONTROL FLAGS OFF TO STOP THE SPAM OF "SERVER ENABLES LAND EVERYWHERE" ETC.\
            //NOTE -    THE USER SHOULD GET THE CONTROL AS PART OF THE ORIGINAL WEATHER PACKET WHEN LOGGING IN!

            //The current weather as represented in the weather packet.
            public static float _WindX = Weather.WindX; //m/s
            public static float _WindY = Weather.WindY; //m/s
            public static float _WindZ = Weather.WindZ; //m/s
            public static float _Fog__ = Weather.Fog; //m

            //The maximum variance from the base weather as %. 50% means vary by up to 50% away from the base value.
            public static int _Weather_Variance_Limit = 50;

            //The maximum rates the weather can change by per minute.
            public static float _WindX_DUDX1_MAX
            {
                get
                {
                    return Settings.Weather.Advanced.ChangeLimitAbsolute.WindX; //m/s
                }
                set
                {
                    Settings.Weather.Advanced.ChangeLimitAbsolute.WindX = value;
                }
            }
            public static float _WindY_DUDX1_MAX
            {
                get
                {
                    return Settings.Weather.Advanced.ChangeLimitAbsolute.WindY; //m/s
                }
                set
                {
                    Settings.Weather.Advanced.ChangeLimitAbsolute.WindY = value;
                }
            }
            public static float _WindZ_DUDX1_MAX
            {
                get
                {
                    return Settings.Weather.Advanced.ChangeLimitAbsolute.WindZ; //m/s
                }
                set
                {
                    Settings.Weather.Advanced.ChangeLimitAbsolute.WindZ = value;
                }
            }
            public static float _Fog___DUDX1_MAX {
                get
                {
                    return Settings.Weather.Advanced.ChangeLimitAbsolute.Fog; //m/s
                }
                set
                {
                    Settings.Weather.Advanced.ChangeLimitAbsolute.Fog = value;
                }
            }

            //The current rate at which the weather is changing.
            public static float _WindX_DUDX1 = 0; //m/s
            public static float _WindY_DUDX1 = 0; //m/s
            public static float _WindZ_DUDX1 = 0; //m/s
            public static float _Fog___DUDX1 = 0; //m


            //The maximum rates the weather change rate will vary by as % per tick. this is in BOTH directions (+/-)
            public static float _WindX_DUDX2_MAX
            {
                get {
                    return Settings.Weather.Advanced.AccelerationLimit.WindX; //%
                }
                set {
                    Settings.Weather.Advanced.AccelerationLimit.WindX = value;
                }
            }
            public static float _WindY_DUDX2_MAX
            {
                get {
                    return Settings.Weather.Advanced.AccelerationLimit.WindY; //%
                }
                set {
                    Settings.Weather.Advanced.AccelerationLimit.WindY = value;
                }
            }
            public static float _WindZ_DUDX2_MAX
            {
                get {
                    return Settings.Weather.Advanced.AccelerationLimit.WindZ; //%
                }
                set {
                    Settings.Weather.Advanced.AccelerationLimit.WindZ = value;
                }
            }
            public static float _Fog___DUDX2_MAX
            {
                get {
                    return Settings.Weather.Advanced.AccelerationLimit.Fog; //%
                }
                set {
                    Settings.Weather.Advanced.AccelerationLimit.Fog = value;
                }
            }

            //The maximum rate of random variance in the values as %. 0 is no change, 100 is the difference between the max/min and the base.
            public static int _Turbulence
            {
                get
                {
                    return Settings.Weather.Advanced.TurbulencePercent;
                }
                set
                {
                    Settings.Weather.Advanced.TurbulencePercent = value;
                }
            }

            public static Packets.Packet_33_Weather GetUpdatedWeather()
            {
                Packets.Packet_33_Weather OutWeather = new Packets.Packet_33_Weather(_Weather);
                if (!AdvancedWeatherOptions._Weather_Variable) return OutWeather;
                OutWeather.WindX = _WindX;
                OutWeather.WindY = _WindY;
                OutWeather.WindZ = _WindZ;
                OutWeather.Fog = _Fog__;

                //Get the baseline weather
                float BASEWINDX = _Weather.WindX;
                float BASEWINDY = _Weather.WindY;
                float BASEWINDZ = _Weather.WindZ;
                float BASEFOG__ = _Weather.Fog;

                //find the minimum weather limit
                float MINWINDX = BASEWINDX - ((BASEWINDX - 0) * (_Weather_Variance_Limit / (float)100));
                float MINWINDY = BASEWINDY - ((BASEWINDY - 0) * (_Weather_Variance_Limit / (float)100));
                float MINWINDZ = BASEWINDZ - ((BASEWINDZ - 0) * (_Weather_Variance_Limit / (float)100));
                float MINFOG__ = BASEFOG__ - ((BASEFOG__ - 0) * (_Weather_Variance_Limit / (float)100));

                //find the maximum weather limit
                float MAXWINDX = BASEWINDX + ((BASEWINDX - 0) * (_Weather_Variance_Limit / (float)100));
                float MAXWINDY = BASEWINDY + ((BASEWINDY - 0) * (_Weather_Variance_Limit / (float)100));
                float MAXWINDZ = BASEWINDZ + ((BASEWINDZ - 0) * (_Weather_Variance_Limit / (float)100));
                float MAXFOG__ = BASEFOG__ + ((BASEFOG__ - 0) * (_Weather_Variance_Limit / (float)100));

                //shouldn't need any of this crap...
                /*
                if (BASEWINDX < MINWINDX) BASEWINDX = MINWINDX;
                if (BASEWINDY < MINWINDY) BASEWINDY = MINWINDY;
                if (BASEWINDZ < MINWINDZ) BASEWINDZ = MINWINDZ;
                if (BASEFOG__ < MINFOG__) BASEFOG__ = MINFOG__;

                if (BASEWINDX > MAXWINDX) BASEWINDX = MAXWINDX;
                if (BASEWINDY > MAXWINDY) BASEWINDY = MAXWINDY;
                if (BASEWINDZ > MAXWINDZ) BASEWINDZ = MAXWINDZ;
                if (BASEFOG__ > MAXFOG__) BASEFOG__ = MAXFOG__;
                */

                if (MINFOG__ < 0) MINFOG__ = 0;
                if (MAXFOG__ > 20000) MAXFOG__ = 20000;

                Random RandomGenerator = new Random();

                //adjust the rate of change for the weather.

                //LIMITER * ([VALUE+([BASE-VALUE]/2)]/[BASE/(BASE*VARIANCELIMIT)]) ~= 0.00 +/- ACCEPTABLE VARIANCE.
                //LIMITER * ([VALUE+([BASE-VALUE]/2)]/[BASE/(BASE*VARIANCELIMIT)]) ~= 0.00 +/- ACCEPTABLE VARIANCE.
                //LIMITER * ([VALUE+([BASE-VALUE]/2)]/[(BASE*VARIANCELIMIT)/BASE]) ~= 0.00 +/- ACCEPTABLE VARIANCE.
                //4000 * 3750
                //((-3000 * 100),
                //
                //RATIO GETS STEEPER AS THE VALUES GET FURTHER AWAY FROM CENTER. IT'S LIKE A BALL IN A PARABOLA! :D
                float Limiter;
                if (_Weather_Variance_Limit == 0) Limiter = 1;
                else Limiter = (_Weather_Variance_Limit / (float)100);
                try
                {
                    if (BASEWINDX != 0) _WindX_DUDX1 += (_WindX_DUDX1_MAX / 60 / 10) * ((RandomGenerator.Next(-10000, 10000) / (float)10000) + (((float)BASEWINDX - _WindX) / ((float)BASEWINDX * Limiter)) / 2);
                    if (BASEWINDY != 0) _WindY_DUDX1 += (_WindY_DUDX1_MAX / 60 / 10) * ((RandomGenerator.Next(-10000, 10000) / (float)10000) + (((float)BASEWINDY - _WindY) / ((float)BASEWINDY * Limiter)) / 2);
                    if (BASEWINDZ != 0) _WindZ_DUDX1 += (_WindZ_DUDX1_MAX / 60 / 10) * ((RandomGenerator.Next(-10000, 10000) / (float)10000) + (((float)BASEWINDZ - _WindZ) / ((float)BASEWINDZ * Limiter)) / 2);
                    if (BASEFOG__ != 0) _Fog___DUDX1 += (_Fog___DUDX1_MAX / 60 / 10) * ((RandomGenerator.Next(-10000, 10000) / (float)10000) + (((float)BASEFOG__ - _Fog__) / ((float)BASEFOG__ * Limiter)) / 2);
                }
                catch
                {
                }

                //limit the rates of change to the minimum.
                if (_WindX_DUDX1 < -_WindX_DUDX1_MAX / 60 / 10) _WindX_DUDX1 = -_WindX_DUDX1_MAX / 60 / 10;
                if (_WindY_DUDX1 < -_WindY_DUDX1_MAX / 60 / 10) _WindY_DUDX1 = -_WindY_DUDX1_MAX / 60 / 10;
                if (_WindZ_DUDX1 < -_WindZ_DUDX1_MAX / 60 / 10) _WindZ_DUDX1 = -_WindZ_DUDX1_MAX / 60 / 10;
                if (_Fog___DUDX1 < -_Fog___DUDX1_MAX / 60 / 10) _Fog___DUDX1 = -_Fog___DUDX1_MAX / 60 / 10;

                //limit the rates of change to the maximum.
                if (_WindX_DUDX1 > _WindX_DUDX1_MAX / 60 / 10) _WindX_DUDX1 = _WindX_DUDX1_MAX / 60 / 10;
                if (_WindY_DUDX1 > _WindY_DUDX1_MAX / 60 / 10) _WindY_DUDX1 = _WindY_DUDX1_MAX / 60 / 10;
                if (_WindZ_DUDX1 > _WindZ_DUDX1_MAX / 60 / 10) _WindZ_DUDX1 = _WindZ_DUDX1_MAX / 60 / 10;
                if (_Fog___DUDX1 > _Fog___DUDX1_MAX / 60 / 10) _Fog___DUDX1 = _Fog___DUDX1_MAX / 60 / 10;

                //add the rate of change to the current weather.
                _WindX += _WindX_DUDX1;
                _WindY += _WindY_DUDX1;
                _WindZ += _WindZ_DUDX1;
                _Fog__ += _Fog___DUDX1;

                //limit the weather by the minimums.
                if (_WindX < MINWINDX) _WindX = MINWINDX;
                if (_WindY < MINWINDY) _WindY = MINWINDY;
                if (_WindZ < MINWINDZ) _WindZ = MINWINDZ;
                if (_Fog__ < MINFOG__) _Fog__ = MINFOG__;

                //limit the weather by the maximums.
                if (_WindX > MAXWINDX) _WindX = MAXWINDX;
                if (_WindY > MAXWINDY) _WindY = MAXWINDY;
                if (_WindZ > MAXWINDZ) _WindZ = MAXWINDZ;
                if (_Fog__ > MAXFOG__) _Fog__ = MAXFOG__;

                //prepare for turbulence
                float OutWindX = _WindX;
                float OutWindY = _WindY;
                float OutWindZ = _WindZ;
                float OutFog__ = _Fog__;

                OutWindX *= (RandomGenerator.Next(-_Turbulence, _Turbulence) / (float)100) + 1;
                OutWindY *= (RandomGenerator.Next(-_Turbulence, _Turbulence) / (float)100) + 1;
                OutWindZ *= (RandomGenerator.Next(-_Turbulence, _Turbulence) / (float)100) + 1;
                //OutFog__ *= (RandomGenerator.Next(-Turbulence, Turbulence) / (float)100) + 1;

                OutWeather.WindX = OutWindX;
                OutWeather.WindY = OutWindY;
                OutWeather.WindZ = OutWindZ;
                OutWeather.Fog = OutFog__;

                return OutWeather;
            }
        }
        #endregion
        #region Missiles
        public static Packets.Packet_31_MissilesOption Missiles
        {
            get {
                return new Packets.Packet_31_MissilesOption(Settings.Weapons.Missiles);
            }
            set
            {
                Settings.Weapons.Missiles = value.Enabled;
            }
        }
        #endregion
        #region Weapons
        public static Packets.Packet_39_WeaponsOption Weapons
        {
            get
            {
                return new Packets.Packet_39_WeaponsOption(Settings.Weapons.Unguided);
            }
            set
            {
                Settings.Weapons.Unguided = value.Enabled;
            }
        }
        #endregion
        #region NoExternalAirView
        public static Packets.Packet_43_MiscCommand NoExternalAirView
        {
            get
            {
                return new Packets.Packet_43_MiscCommand
                (
                "NOEXAIRVW",
                Settings.Options.NoExternalAirView.ToString().ToUpperInvariant()
                );
            }
            set
            {
                if (value.Argument.ToUpperInvariant() == "TRUE")
                {
                    Settings.Options.NoExternalAirView = true;
                }
                else Settings.Options.NoExternalAirView = false; 
            }
        }
        #endregion
        #region RadarAltitude
        public static Packets.Packet_43_MiscCommand RadarAltitude
        {
            get
            {
                return new Packets.Packet_43_MiscCommand
                (
                "RADARALTI",
                Settings.Options.RadarBaseAltitude + "m"
                );
            }
            set
            {
                uint Output = 0;
                UInt32.TryParse(value.Argument.ToUpperInvariant().Split('M')[0], out Output);
                Settings.Options.RadarBaseAltitude = Output;
            }
        }
        #endregion
        #region UsernameDistance
        public static Packets.Packet_41_UsernameDistance UsernameDistance
        {
            get
            {
                string UsernameDistance = Settings.Options.UsernameDistance;
                if (UsernameDistance.ToUpperInvariant() == "TRUE") return new Packets.Packet_41_UsernameDistance(true);
                if (UsernameDistance.ToUpperInvariant() == "FALSE") return new Packets.Packet_41_UsernameDistance(false);
                try
                {
                    uint Meters = 0;
                    Meters = UInt32.Parse(UsernameDistance);
                    return new Packets.Packet_41_UsernameDistance(Meters);
                }
                catch
                {
                    return new Packets.Packet_41_UsernameDistance(true);
                }
            }
            set
            {
                switch (value.UsernameDistance)
                {
                    case 0:
                        Settings.Options.UsernameDistance = value.UsernameDistance.ToString();
                        break;
                    case 1:
                        Settings.Options.UsernameDistance = "TRUE";
                        break;
                    case 2:
                        break;
                    default:
                        Settings.Options.UsernameDistance = "FALSE";
                        break;
                }
            }
        }
        #endregion
        #region Field
        //NOT A GET/SET METHOD! NEED TO KEEP THIS STATIC DURING RUNTIME!
        public static Packets.Packet_04_FieldName Field = new Packets.Packet_04_FieldName("HAWAII")
        {
            FieldName = Settings.Loading.FieldName, //This is overwritten by the loading process - this value is the DEFAULT!
        };
        #endregion

        public static bool ApplySettings()
        {
            Clients.AllClients.SendMessage("Settings Reloaded.");
            Clients.AllClients.SendPacket(Weather);
            Clients.AllClients.SendPacket(Missiles);
            Clients.AllClients.SendPacket(Weapons);
            //Clients.AllClients.SendPacket(Missiles);
            return true;
        }
    }
}
