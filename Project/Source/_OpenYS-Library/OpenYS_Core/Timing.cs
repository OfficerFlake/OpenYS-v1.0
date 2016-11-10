using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Net;
using System.Diagnostics;

namespace OpenYS
{
    public static partial class OpenYS
    {        

        #region Reset Timer
        public static float ServerTimestamp()
        {
            return (float)(DateTime.Now - OpenYS.TimeStarted).TotalSeconds;
        }
        public static DateTime Time_ResetTimerStarted = DateTime.Now;
        public static AutoResetEvent Signal_ResetServer = new AutoResetEvent(false);

        public static bool StartResetTimer()
        {
            Time_ResetTimerStarted = DateTime.Now;
            return true;
        }

        public static bool StartResetTimer(double Duration)
        {
            Settings.Server.RestartTimer = (int)Math.Ceiling(Duration);
            StartResetTimer();
            return true;
        }

        public static bool PollResetTimer()
        {
            if (Settings.Server.RestartTimer > 0)
            {
                #region ResetTimer
                TimeSpan TimeRemaining = new TimeSpan(0, Settings.Server.RestartTimer, 0) - (DateTime.Now - Time_ResetTimerStarted);
                TimeSpan LastUpdateTimeRemaining = new TimeSpan(0, Settings.Server.RestartTimer, 0) - (Time_LastMicroTick - Time_ResetTimerStarted); //should be always less then TimeRemaining, if not equal to.

                if (TimeRemaining.TotalMinutes <= 30 && LastUpdateTimeRemaining.TotalMinutes > 30)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 30 minutes! ***");
                }
                if (TimeRemaining.TotalMinutes <= 20 && LastUpdateTimeRemaining.TotalMinutes > 20)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 20 minutes! ***");
                }
                if (TimeRemaining.TotalMinutes <= 10 && LastUpdateTimeRemaining.TotalMinutes > 10)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 10 minutes! ***");
                }
                if (TimeRemaining.TotalMinutes <= 5 && LastUpdateTimeRemaining.TotalMinutes > 5)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 5 minutes! ***");
                }
                if (TimeRemaining.TotalMinutes <= 1 && LastUpdateTimeRemaining.TotalMinutes > 1)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 1 minute! ***");
                }
                if (TimeRemaining.TotalSeconds <= 30 && LastUpdateTimeRemaining.TotalSeconds > 30)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 30 Seconds! ***");
                }
                if (TimeRemaining.TotalSeconds <= 20 && LastUpdateTimeRemaining.TotalSeconds > 20)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 20 Seconds! ***");
                }
                if (TimeRemaining.TotalSeconds <= 10 && LastUpdateTimeRemaining.TotalSeconds > 10)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 10 Seconds! ***");
                }
                if (TimeRemaining.TotalSeconds <= 3 && LastUpdateTimeRemaining.TotalSeconds > 3)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 3 Seconds! ***");
                }
                if (TimeRemaining.TotalSeconds <= 2 && LastUpdateTimeRemaining.TotalSeconds > 2)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 2 Seconds! ***");
                }
                if (TimeRemaining.TotalSeconds <= 1 && LastUpdateTimeRemaining.TotalSeconds > 1)
                {
                    //Within the last update, the time remaining until reset ticked over this mark.
                    Clients.AllClients.SendMessage("&6*** Server will be reset in 1 Second! ***");
                }
                if (TimeRemaining.TotalSeconds <= 0)
                {
                    //Reset Now!!!
                    ResetNow();
                }
            }
            return true;
            #endregion
        }

        public static bool StopResetTimer()
        {
            Settings.Server.RestartTimer = 0;
            return true;
        }

        public static bool ResetNow()
        {
            Clients.AllClients.SendMessage("&6*** Server resetting! ***");
            Thread.Sleep(1.AsSeconds()); //sleep for 1 seconds.
            Clients.AllClients.SendMessage("&6*** Server resetting! ***");
            Thread.Sleep(1.AsSeconds()); //sleep for 1 seconds.
            Clients.AllClients.SendMessage("&6*** Server resetting! ***");
            Thread.Sleep(1.AsSeconds()); //sleep for 1 seconds.
            Signal_ResetServer.Set();
            return true;
        }
        #endregion

        #region Mirco Tick
        public static DateTime Time_LastMicroTick = DateTime.Now;
        public delegate void OnMicroTickUpdateEvent();
        public static List<OnMicroTickUpdateEvent> Events_OnMicroTickUpdate = new List<OnMicroTickUpdateEvent>();
        public static bool MicroTick()
        {
            #region DoWeather
            if (AdvancedWeatherOptions._Weather_Variable)
            {
                Packets.Packet_33_Weather TempWeather = AdvancedWeatherOptions.GetUpdatedWeather();
                TempWeather.Control_BlackOut = false;
                TempWeather.Control_Collisions = false;
                TempWeather.Control_LandEverywhere = false;
                Clients.AllClients.SendPacket(TempWeather);
            }
            #endregion
            #region DoHeartbeatMonitor
            Clients.DoHeartbeatMonitor();
            #endregion
            #region DoOnTickUpdateEvent
            lock (Events_OnMicroTickUpdate)
            {
                foreach (OnMicroTickUpdateEvent ThisMethod in Events_OnMicroTickUpdate)
                {
                    ThisMethod();
                }
            }
            #endregion

            #region CountThreads
            Thread[] _ThreadsArray;
            lock (Threads.List) _ThreadsArray = Threads.List.ToArray();
            foreach (Thread ThisThread in _ThreadsArray)
            {
                if (!ThisThread.IsAlive)
                {
                    //Log.Error("Thread killed as part of maintenance: " + ThisThread.Name);
                    Threads.List.RemoveAll(x => x == ThisThread);
                }
            }
            #endregion
            #region ManageVehicles
            Vehicle[] _VehiclesArray;
            lock (Vehicles.List) _VehiclesArray = Vehicles.List.ToArray();
            foreach (Vehicle ThisVehicle in _VehiclesArray)
            {
                if (Clients.Flying.Where(x=>x.Vehicle == ThisVehicle).Count() < 1
                    & !ThisVehicle.IsVirtualVehicle)
                {
                    Vehicles.List.RemoveAll(x => x == ThisVehicle);
                    Packets.Packet_13_RemoveAirplane RemoveAirplane = new Packets.Packet_13_RemoveAirplane(ThisVehicle.ID);
                    foreach (Client OtherClient in Clients.AllClients)
                    {
                        Packets.Packet_06_Acknowledgement AcknowledgeLeave = new Packets.Packet_06_Acknowledgement(2, RemoveAirplane.ID);
                        //OtherClient.SendPacketGetPacket(RemoveAirplane, AcknowledgeLeave);
                        OtherClient.SendPacket(RemoveAirplane);
                    }
                    Console.WriteLine("&3Found a lost Vehicle... Cleaned it up!");
                    continue;
                }
                if (ThisVehicle.Invincible)
                {
                    Clients.LoggedIn.SendPacket(new Packets.Packet_30_AirCommand(ThisVehicle.ID, "STRENGTH", "255"));
                }
            }
            #endregion
            return true;
        }
        #endregion

        #region Macro Tick
        public static DateTime Time_LastMacroTick = DateTime.Now;
        public delegate void OnMacroTickUpdateEvent();
        public static List<OnMacroTickUpdateEvent> Events_OnMacroTickUpdate = new List<OnMacroTickUpdateEvent>();
        public static bool MacroTick()
        {
            TimeSpan TimeSinceLastMacroTick = (DateTime.Now - Time_LastMacroTick);
            if (TimeSinceLastMacroTick.TotalMinutes >= 10)
            {
                foreach (OnMacroTickUpdateEvent ThisEvent in Events_OnMacroTickUpdate)
                {
                    ThisEvent();
                }
                Time_LastMacroTick = DateTime.Now;
            }
            return true;
        }
        #endregion

        #region Time Of Day
        public static uint PreviousTick = (OpenYS.Weather.Lighting <= 65536) ? 12000u : 0u;
        public static uint CurrentTick = (OpenYS.Weather.Lighting <= 65536) ? 12000u : 0u;
        public static uint GetServerTimeTicks()
        {
            if (AdvancedWeatherOptions._DayNightCycleLength <= 0)
            {
                return CurrentTick;
            }
            uint Hours = (OpenYS.Weather.Lighting <= 65536) ? 12u : 0u;
            if (OpenYS.AdvancedWeatherOptions._DayNightCycleLength > 0)
            {
                Hours = (uint)Math.Floor((((DateTime.Now - OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart).TotalMinutes / OpenYS.AdvancedWeatherOptions._DayNightCycleLength) * 24));
            }
            uint Minutes = 0;
            if (OpenYS.AdvancedWeatherOptions._DayNightCycleLength > 0)
            {
                Minutes = (uint)(((DateTime.Now - OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart).TotalMinutes / OpenYS.AdvancedWeatherOptions._DayNightCycleLength) * 24 * 60) % 60;
            }
            uint Output = Hours * 1000u + (uint)Math.Round(Minutes / 60d * 1000d);
            return Output;
        }

        public static bool UpdateTimeOfDay()
        {
            while ((DateTime.Now - AdvancedWeatherOptions._LastDayNightCycleRestart).TotalMinutes > AdvancedWeatherOptions._DayNightCycleLength & AdvancedWeatherOptions._DayNightCycleLength > 0)
            {
                AdvancedWeatherOptions._LastDayNightCycleRestart += new TimeSpan(0, AdvancedWeatherOptions._DayNightCycleLength, 0);
            }
            CurrentTick = GetServerTimeTicks();
            if (AdvancedWeatherOptions._DayNightCycleLength <= 0)
            {
                CurrentTick = PreviousTick;
                //return false;
            }

            //These packets ultimately sent to the clients.
            Packets.Packet_49_SkyColor SkyColorUpdate = new Packets.Packet_49_SkyColor(Settings.Weather.Advanced.SkyColor);
            Packets.Packet_48_FogColor FogColorUpdate = new Packets.Packet_48_FogColor(Settings.Weather.Advanced.FogColor);

            Colors.XRGBColor BlendedSkyColor = Settings.Weather.Advanced.SkyColor;
            Colors.XRGBColor BlendedFogColor = Settings.Weather.Advanced.FogColor;
            #region TimeOfDayBlending
            //Night 1
            if (CurrentTick < 6000)
            {
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.NightSkyColor;
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.NightHorizonColor;
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
                //FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(new Utilities.RGBColor(0, 0, 0), 1 - Settings.Weather.Advanced.EnvironmentColors.NightHorizonColorChangeFactor);
            }
            //Dawn 1
            if (CurrentTick >= 6000 && CurrentTick < 7000)
            {
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.NightSkyColor;
                BlendedSkyColor = BlendedSkyColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.DawnSkyColor, (CurrentTick - 6000) / 1000d);
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.NightHorizonColor;
                BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.DawnHorizonColor, (CurrentTick - 6000) / 1000d);
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
                //FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(new Utilities.RGBColor(0, 0, 0), 1 - Settings.Weather.Advanced.EnvironmentColors.NightHorizonColorChangeFactor);
            }
            //Dawn 2
            if (CurrentTick >= 7000 && CurrentTick < 8000)
            {
                if (PreviousTick < 7000)
                {
                    Weather = new Packets.Packet_33_Weather(_Weather) { Lighting = 0 }; //Set To Day
                    Clients.AllClients.SendPacket(Weather);
                }
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.DawnSkyColor;
                BlendedSkyColor = BlendedSkyColor.AlphaBlend(new Colors.XRGBColor(255, 255, 255), (CurrentTick - 7000) / 1000d);
                BlendedSkyColor = BlendedSkyColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.DaySkyColor, (CurrentTick - 7000) / 1000d);
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.DawnHorizonColor;
                BlendedFogColor = BlendedFogColor.AlphaBlend(new Colors.XRGBColor(255, 255, 255), (CurrentTick - 7000) / 1000d);
                BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.DayHorizonColor, (CurrentTick - 7000) / 1000d);
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
            }
            //Day
            if (CurrentTick >= 8000 && CurrentTick < 18000)
            {
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.DaySkyColor;
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.DayHorizonColor;
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
            }
            //Dusk 1
            if (CurrentTick >= 18000 && CurrentTick < 19000)
            {
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.DaySkyColor;
                BlendedSkyColor = BlendedSkyColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.DuskSkyColor, (CurrentTick - 18000) / 1000d);
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.DayHorizonColor;
                BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.DuskHorizonColor, (CurrentTick - 18000) / 1000d);
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
            }
            //Dusk 2
            if (CurrentTick >= 19000 && CurrentTick < 20000)
            {
                if (PreviousTick < 19000)
                {
                    Weather = new Packets.Packet_33_Weather(_Weather) { Lighting = 65537 }; //Set To Night
                    Clients.AllClients.SendPacket(Weather);
                }
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.DuskSkyColor;
                BlendedSkyColor = BlendedSkyColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.NightSkyColor, (CurrentTick - 19000) / 1000d);
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.DuskHorizonColor;
                BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.NightHorizonColor, (CurrentTick - 19000) / 1000d);
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
                //FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(new Utilities.RGBColor(0, 0, 0), 1 - Settings.Weather.Advanced.EnvironmentColors.NightHorizonColorChangeFactor);
            }
            //Night 2
            if (CurrentTick >= 20000)
            {
                BlendedSkyColor = Settings.Weather.Advanced.EnvironmentColors.NightSkyColor;
                SkyColorUpdate.Color = SkyColorUpdate.Color.AlphaBlend(BlendedSkyColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);

                BlendedFogColor = Settings.Weather.Advanced.EnvironmentColors.NightHorizonColor;
                FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(BlendedFogColor, Settings.Weather.Advanced.EnvironmentColors.OverallBlendFactor);
                //FogColorUpdate.Color = FogColorUpdate.Color.AlphaBlend(new Utilities.RGBColor(0, 0, 0), 1 - Settings.Weather.Advanced.EnvironmentColors.NightHorizonColorChangeFactor);
            }
            #endregion

            #region Fog Distance Blending
            BlendedFogColor = FogColorUpdate.Color;
            BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.WhiteFogColor, 1 - (OpenYS.Weather.Fog / 20000));
            FogColorUpdate.Color = BlendedFogColor;
            #endregion

            foreach (Client ThisClient in Clients.LoggedIn)
            {
                #region Altitude Blending
                //Now filter the sky and fog based on altitude.
                Packets.Packet_48_FogColor ThisClientFogUpdate = FogColorUpdate;
                Packets.Packet_49_SkyColor ThisClientSkyUpdate = SkyColorUpdate;

                Vehicle ThisVehicle = ThisClient.Vehicle;
                if (ThisVehicle != Vehicles.NoVehicle)
                {
                    if (ThisVehicle.PosY >= Settings.Weather.Advanced.AtmosphericFading.MaximumAltitudeInMeters) //Above 100,000ft
                    {
                        ThisClientSkyUpdate.Color = ThisClientSkyUpdate.Color.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.NoAtmosphereSkyColor, 1.0);
                        ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.NoAtmosphereHorizonColor, 1.0);
                        //ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Utilities.RGBColor(0, 0, 0), 1 - NightFogFilter);
                    }
                    if (ThisVehicle.PosY >= Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters && ThisVehicle.PosY < Settings.Weather.Advanced.AtmosphericFading.MaximumAltitudeInMeters) //Roughly 40,000ft to 100,000ft
                    {
                        BlendedSkyColor = ThisClientSkyUpdate.Color;
                        BlendedSkyColor = BlendedSkyColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.MaxAltitudeSkyColor, (ThisVehicle.PosY - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters) / (Settings.Weather.Advanced.AtmosphericFading.MaximumAltitudeInMeters - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters)); //fade to the sky color.
                        BlendedSkyColor = BlendedSkyColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.NoAtmosphereSkyColor, (ThisVehicle.PosY - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters) / (Settings.Weather.Advanced.AtmosphericFading.MaximumAltitudeInMeters - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters)); //fade to the noatmosphere color.
                        ThisClientSkyUpdate.Color = BlendedSkyColor;

                        BlendedFogColor = ThisClientFogUpdate.Color;
                        BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.MaxAltitudeHorizonColor, (ThisVehicle.PosY - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters) / (Settings.Weather.Advanced.AtmosphericFading.MaximumAltitudeInMeters - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters)); //fade to the Fog color.
                        BlendedFogColor = BlendedFogColor.AlphaBlend(Settings.Weather.Advanced.EnvironmentColors.NoAtmosphereHorizonColor, (ThisVehicle.PosY - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters) / (Settings.Weather.Advanced.AtmosphericFading.MaximumAltitudeInMeters - Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters)); //fade to the noatmosphere color.
                        ThisClientFogUpdate.Color = BlendedFogColor;
                        //ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Utilities.RGBColor(0, 0, 0), 1 - NightFogFilter);
                    }
                    if (ThisVehicle.PosY < Settings.Weather.Advanced.AtmosphericFading.MinimumAltitudeInMeters) //Less then 40,000 ft.
                    {
                        //Do nothing.
                    }
                    #region SetNightColors
                    if (CurrentTick < 6000 | CurrentTick >= 20000) ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), 1 - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor); //7000 = 0, 6000 = 1
                    if (CurrentTick >= 6000 & CurrentTick < 7000)
                    {
                        ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (1.0d - ((CurrentTick - 6000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    if (CurrentTick >= 7000 & CurrentTick < 8000)
                    {
                        ThisClientSkyUpdate.Color = ThisClientSkyUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (1.0d - ((CurrentTick - 7000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    if (CurrentTick >= 18000 & CurrentTick < 19000)
                    {
                        ThisClientSkyUpdate.Color = ThisClientSkyUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (((CurrentTick - 18000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    if (CurrentTick >= 19000 & CurrentTick < 20000)
                    {
                        ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (((CurrentTick - 19000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    #endregion
                }
                else
                {
                    #region SetNightColors
                    if (CurrentTick < 6000 | CurrentTick >= 20000) ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), 1 - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor); //7000 = 0, 6000 = 1
                    if (CurrentTick >= 6000 & CurrentTick < 7000)
                    {
                        ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (1.0d - ((CurrentTick - 6000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    if (CurrentTick >= 7000 & CurrentTick < 8000)
                    {
                        ThisClientSkyUpdate.Color = ThisClientSkyUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (1.0d - ((CurrentTick - 7000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    if (CurrentTick >= 18000 & CurrentTick < 19000)
                    {
                        ThisClientSkyUpdate.Color = ThisClientSkyUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (((CurrentTick - 18000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    if (CurrentTick >= 19000 & CurrentTick < 20000)
                    {
                        ThisClientFogUpdate.Color = ThisClientFogUpdate.Color.AlphaBlend(new Colors.XRGBColor(0, 0, 0), (((CurrentTick - 19000d) / 1000d)) * (1.0d - Settings.Weather.Advanced.EnvironmentColors.NightColorFactor));
                    }
                    #endregion
                }
                #endregion

                #region Set Definied Colors
                if (ThisClient.Version > 20110207)
                {
                    if (!Settings.Weather.Advanced.EnableSkyColor) ThisClient.SendPacket(ThisClientSkyUpdate);
                    if (!Settings.Weather.Advanced.EnableFogColor) ThisClient.SendPacket(ThisClientFogUpdate);
                }
                #endregion
            }
            PreviousTick = CurrentTick;
            return true;
        }

        public static string GetServerTimeString()
        {
            string DayNight = (OpenYS.Weather.Lighting <= 65536) ? "1200" : "0000";
            DayNight = "";
            uint Hours = GetServerTimeHours();
            uint Minutes = GetServerTimeMinutes();
            string _Hours = Hours.ToString();
            string _Minutes = Minutes.ToString();
            while (_Hours.Length < 2) _Hours = "0" + _Hours;
            while (_Minutes.Length < 2) _Minutes = "0" + _Minutes;
            DayNight = _Hours + _Minutes + "";
            if (DayNight.Length > 4) DayNight = "XXXX";

            return DayNight;
        }

        public static uint GetServerTimeHours()
        {
            uint Output = GetServerTimeTicks() / 1000u;
            return Output;
        }

        public static uint GetServerTimeUint()
        {
            uint Output = GetServerTimeHours() * 100 + GetServerTimeMinutes();
            return Output;
        }

        public static uint GetServerTimeMinutes()
        {
            return (uint)Math.Round((GetServerTimeTicks() % 1000)/1000d*60d);
        }

        public static bool SetServerTimeTicks(uint Ticks)
        {
            TimeSpan Adjustment = new TimeSpan(0, 0, 0, 0, (int)(Ticks / 24000d * OpenYS.AdvancedWeatherOptions._DayNightCycleLength * 60 * 1000d));
            OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now - Adjustment;
            CurrentTick = Ticks;
            PreviousTick = CurrentTick;
            Settings.Weather.Time = GetServerTimeUint();
            SettingsHandler.SaveAll();
            if (Ticks >= 7000 && Ticks < 19000)
            {
                OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { Lighting = 0 };
                Clients.AllClients.SendPacket(OpenYS.Weather);
            }
            else
            {
                OpenYS.Weather = new Packets.Packet_33_Weather(OpenYS._Weather) { Lighting = 65537 };
                Clients.AllClients.SendPacket(OpenYS.Weather);
            }
            return true;
        }

        #endregion

        public static bool DayNightCycler()
        {
            try
            {
                if (AdvancedWeatherOptions._DayNightCycleLength <= 0)
                {
                    return false;
                }

                //Synchronise the time.
                if (Weather.Lighting <= 65536)
                {
                    //Set time to 1200J
                    AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now - new TimeSpan(0, 0, (int)((float)AdvancedWeatherOptions._DayNightCycleLength / 2 * 60)); //mid day is half a day AFTER midnight! adjust the start day time accordingly!
                }
                else
                {
                    //Set time to 0000J
                    AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now; //mid day is half a day AFTER midnight! adjust the start day time accordingly!
                }

                while (!Environment.TerminateSignal.WaitOne(0))
                {
                    if (AdvancedWeatherOptions._DayNightCycleLength <= 0)
                    {
                        return false;
                    }
                    Thread.Sleep((int)((((AdvancedWeatherOptions._DayNightCycleLength / (float)4))*1000*60)-3000)); //sleep to 0600 or 1800.
                    if (AdvancedWeatherOptions._DayNightCycleLength != 0)
                    {
                        if (_Weather.Lighting >= 65537)
                        {
                            Clients.AllClients.SendMessage("&6The sun is about to rise...");
                            Thread.Sleep(3000);
                            Clients.AllClients.SendMessage("&6The sun rises...");
                            Weather = new Packets.Packet_33_Weather(_Weather) { Lighting = 0 };
                            Clients.AllClients.SendPacket(Weather);
                            Thread.Sleep((int)(((AdvancedWeatherOptions._DayNightCycleLength / (float)4)) * 1000 * 60)); //sleep to 1200
                        }
                        else
                        {
                            Clients.AllClients.SendMessage("&5The sun is about to set...");
                            Thread.Sleep(3000);
                            Clients.AllClients.SendMessage("&5The sun sets...");
                            Weather = new Packets.Packet_33_Weather(_Weather) { Lighting = 65537 };
                            Clients.AllClients.SendPacket(Weather);
                            Thread.Sleep((int)(((AdvancedWeatherOptions._DayNightCycleLength / (float)4)) * 1000 * 60)); //sleep until 0000
                            AdvancedWeatherOptions._LastDayNightCycleRestart = DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ThreadAbortException)
                {
                    return false;
                }
                else
                {
                    Log.Error(e);
                    Console.TerminateConsole(e);
                    return false;
                }
            }
            return true;
        }

        public static string GetWeatherTime()
        {
            string DayNight = (OpenYS.Weather.Lighting <= 65536) ? "1200" : "0000";
            //10
            //5
            //5/10 = 0.5
            //0.5 * 24 = 12*60 = minutes
            if (OpenYS.AdvancedWeatherOptions._DayNightCycleLength > 0)
            {
                DayNight = "";
                int Hours = (int)Math.Floor((((DateTime.Now - OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart).TotalMinutes / OpenYS.AdvancedWeatherOptions._DayNightCycleLength) * 24));
                int Minutes = (int)(((DateTime.Now - OpenYS.AdvancedWeatherOptions._LastDayNightCycleRestart).TotalMinutes / OpenYS.AdvancedWeatherOptions._DayNightCycleLength) * 24 * 60) % 60;
                string _Hours = Hours.ToString();
                string _Minutes = Minutes.ToString();
                while (_Hours.Length < 2) _Hours = "0" + _Hours;
                while (_Minutes.Length < 2) _Minutes = "0" + _Minutes;
                DayNight = _Hours + _Minutes;
            }
            return DayNight;
        }
    }
}
