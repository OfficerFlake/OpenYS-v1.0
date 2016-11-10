using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace OpenYS
{
    public partial class MainWindow : Form
    {
        public ManualResetEvent IsLoaded = new ManualResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
            Console.Parent = this;
            IsLoaded.Set();
        }

        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        public static class Console
        {
            public static MainWindow Parent = null;

            #region Colors
            private static Color UnderColor = ConsoleColor.Color_0;
            private static Color DefaultColor = ConsoleColor.Color_F;
            private static class ConsoleColor
            {
                public static Color Color_0 = Color.FromArgb(0, 0, 0);
                public static Color Color_1 = Color.FromArgb(0, 0, 170);
                public static Color Color_2 = Color.FromArgb(0, 170, 0);
                public static Color Color_3 = Color.FromArgb(0, 170, 170);
                public static Color Color_4 = Color.FromArgb(170, 0, 0);
                public static Color Color_5 = Color.FromArgb(170, 0, 170);
                public static Color Color_6 = Color.FromArgb(255, 170, 0);
                public static Color Color_7 = Color.FromArgb(170, 170, 170);
                public static Color Color_8 = Color.FromArgb(85, 85, 85);
                public static Color Color_9 = Color.FromArgb(85, 85, 255);
                public static Color Color_A = Color.FromArgb(85, 255, 85);
                public static Color Color_B = Color.FromArgb(85, 255, 255);
                public static Color Color_C = Color.FromArgb(255, 85, 85);
                public static Color Color_D = Color.FromArgb(255, 85, 255);
                public static Color Color_E = Color.FromArgb(255, 255, 85);
                public static Color Color_F = Color.FromArgb(255, 255, 255);
            }
            #endregion
            #region Styles
            private static FontStyle DefaultStyle = FontStyle.Regular;
            private static class ConsoleFontStyles
            {
                //public static FontStyle Style_K = FontStyle.Strikeout; //Not Supported!
                public static FontStyle Style_L = FontStyle.Bold;
                public static FontStyle Style_M = FontStyle.Strikeout;
                public static FontStyle Style_N = FontStyle.Underline;
                public static FontStyle Style_R = FontStyle.Regular;
            }
            #endregion
            #region Properties
            private static char[] SplittingChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'K', 'L', 'M', 'N', 'R' };

            public class MessageSegment
            {
                public Color Color = DefaultColor;
                public FontStyle Style = DefaultStyle;
                public string String = "";

                public MessageSegment(string _String, Color _Color, FontStyle _Style)
                {
                    String = _String;
                    Color = _Color;
                    Style = _Style;
                }
            }
            public static ConcurrentQueue<List<MessageSegment>> MessageQueue = new ConcurrentQueue<List<MessageSegment>>();
            #endregion
            #region Writing...
            private static void AddText(List<MessageSegment> Segments)
            {
                foreach (MessageSegment ThisSegment in Segments)
                {
                    Parent.ConsoleOutput.SelectionStart = Parent.ConsoleOutput.TextLength;
                    Parent.ConsoleOutput.SelectionLength = 0;

                    Parent.ConsoleOutput.SelectionColor = ThisSegment.Color;
                    Parent.ConsoleOutput.SelectionFont = new Font(Parent.ConsoleOutput.Font, ThisSegment.Style);
                    Parent.ConsoleOutput.AppendText(ThisSegment.String);
                    Parent.ConsoleOutput.SelectionColor = DefaultColor;
                    Parent.ConsoleOutput.SelectionFont = new Font(Parent.ConsoleOutput.Font, DefaultStyle);
                    Parent.ConsoleOutput.ScrollToCaret();
                }
            }
            private delegate void AddTextDelegate(List<MessageSegment> Segments);

            private static void PrepareText(string RawInput)
            {
                List<MessageSegment> Segments = new List<MessageSegment>();

                Color CurrentColor = ConsoleColor.Color_F;
                FontStyle CurrentStyle = FontStyle.Regular;
                string CurrentString = "";
                bool SplitReady = false;

                foreach (char ThisChar in RawInput)
                {
                    #region SplitReady
                    if (SplitReady)
                    {
                        SplitReady = false;
                        if (ThisChar == '&')
                        {
                            SplitReady = true;
                            CurrentString += "&";
                            continue;
                        }
                        if (SplittingChars.Contains(ThisChar.ToString().ToUpperInvariant()[0]))
                        {
                            Color PreviousColor = CurrentColor;
                            FontStyle PreviousStyle = CurrentStyle;
                            #region Switch
                            switch (ThisChar.ToString().ToUpperInvariant()[0])
                            {
                                case '0': CurrentColor = ConsoleColor.Color_0; goto BuildPacket;
                                case '1': CurrentColor = ConsoleColor.Color_1; goto BuildPacket;
                                case '2': CurrentColor = ConsoleColor.Color_2; goto BuildPacket;
                                case '3': CurrentColor = ConsoleColor.Color_3; goto BuildPacket;
                                case '4': CurrentColor = ConsoleColor.Color_4; goto BuildPacket;
                                case '5': CurrentColor = ConsoleColor.Color_5; goto BuildPacket;
                                case '6': CurrentColor = ConsoleColor.Color_6; goto BuildPacket;
                                case '7': CurrentColor = ConsoleColor.Color_7; goto BuildPacket;
                                case '8': CurrentColor = ConsoleColor.Color_8; goto BuildPacket;
                                case '9': CurrentColor = ConsoleColor.Color_9; goto BuildPacket;
                                case 'A': CurrentColor = ConsoleColor.Color_A; goto BuildPacket;
                                case 'B': CurrentColor = ConsoleColor.Color_B; goto BuildPacket;
                                case 'C': CurrentColor = ConsoleColor.Color_C; goto BuildPacket;
                                case 'D': CurrentColor = ConsoleColor.Color_D; goto BuildPacket;
                                case 'E': CurrentColor = ConsoleColor.Color_E; goto BuildPacket;
                                case 'F': CurrentColor = ConsoleColor.Color_F; goto BuildPacket;

                                case 'K': goto default;
                                case 'L': CurrentStyle |= ConsoleFontStyles.Style_L; CurrentStyle &= ~FontStyle.Regular; goto BuildPacket;
                                case 'M': CurrentStyle |= ConsoleFontStyles.Style_M; CurrentStyle &= ~FontStyle.Regular; goto BuildPacket;
                                case 'N': CurrentStyle |= ConsoleFontStyles.Style_N; CurrentStyle &= ~FontStyle.Regular; goto BuildPacket;
                                case 'R': CurrentStyle = ConsoleFontStyles.Style_R; goto BuildPacket;
                                default:
                                    CurrentString += "&" + ThisChar;
                                    break;

                                BuildPacket:
                                    Segments.Add(new MessageSegment(CurrentString, PreviousColor, PreviousStyle));
                                    CurrentString = "";
                                    break;
                            }
                            #endregion
                            continue;
                        }
                    }
                    #endregion

                    #region NotSplitReady
                    else
                    {
                        if (ThisChar == '&')
                        {
                            SplitReady = true;
                            continue;
                        }
                        else
                        {
                            CurrentString += ThisChar;
                        }
                    }
                    #endregion
                }

                //wrap up the end.
                if (SplitReady)
                {
                    CurrentString += "&";
                }
                Segments.Add(new MessageSegment(CurrentString, CurrentColor, CurrentStyle));

                AddTextDelegate d = new AddTextDelegate(AddText);
                try
                {
                    Parent.ConsoleOutput.Invoke(d, new object[] { Segments });
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            public static void WriteLine(object Input, params object[] Args)
            {
                if (Parent == null) return;
                //Format Input...
                if (Input == null) return; //DO NOTHING.
                if (Args == null) Args = new object[0];
                string Formatted = Input.ToString();
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

                int i = 1;
                while (!Parent.ConsoleOutput.IsHandleCreated)
                {

                    Thread.Sleep(i);
                    i *= 2; //Double the sleep time...
                    Debug.WriteLine("Waiting for a handle for the MainWindow...");
                }
                PrepareText(Formatted);
            }
            #endregion
        }

        private void FormWindow_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ClientSize = new Size(1200, 675);
            this.Size = new Size(1200, 675);
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
        }

        private delegate void ConsoleInput_KeyPressDelegate(object sender, KeyPressEventArgs e);
        private void ConsoleInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ConsoleInput.InvokeRequired)
            {
                ConsoleInput.Invoke(new ConsoleInput_KeyPressDelegate(ConsoleInput_KeyPress), sender, e);
                return;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                ConsoleInput.Enabled = false;
                CommandManager.ProcessConsole(ConsoleInput.Text);
                ConsoleInput.Text = "";
                ConsoleInput.Enabled = true;
                ConsoleInput.Focus();
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConsoleInput.Enabled = false;
            MenuStrip.Enabled = false;
            new Thread(() => CommandManager.ProcessConsole("/ShutdownNow")).Start();
            e.Cancel = true;
        }

        private delegate void FocusOnWindowDelegate();
        public void FocusOnWindow()
        {
            if (InvokeRequired)
                Invoke(new FocusOnWindowDelegate(FocusOnWindow));
            else
                Activate();
        }

        private delegate void FocusOnConsoleInputDelegate();
        public void FocusOnConsoleInput()
        {
            if (ConsoleInput.InvokeRequired)
                ConsoleInput.Invoke(new FocusOnConsoleInputDelegate(FocusOnConsoleInput));
            else
                ConsoleInput.Focus();
        }

        private delegate void ClearOutputWindowDelegate();
        public void ClearOutputWindow()
        {
            if (ConsoleOutput.InvokeRequired)
                ConsoleOutput.Invoke(new ClearOutputWindowDelegate(ClearOutputWindow));
            else
                ConsoleOutput.Text = "";
        }

        private delegate void DisableMenuStripDelegate();
        public void DisableMenuStrip()
        {
            if (MenuStrip.InvokeRequired)
                MenuStrip.Invoke(new DisableMenuStripDelegate(DisableMenuStrip));
            else
                MenuStrip.Enabled = false;
        }

        private delegate void EnableMenuStripDelegate();
        public void EnableMenuStrip()
        {
            if (MenuStrip.InvokeRequired)
                MenuStrip.Invoke(new EnableMenuStripDelegate(EnableMenuStrip));
            else
                MenuStrip.Enabled = true;
        }

        private delegate void DisableConsoleInputDelegate();
        public void DisableConsoleInput()
        {
            if (ConsoleInput.InvokeRequired)
                ConsoleInput.Invoke(new DisableConsoleInputDelegate(DisableConsoleInput));
            else
                ConsoleInput.Enabled = false;
        }

        private delegate void EnableConsoleInputDelegate();
        public void EnableConsoleInput()
        {
            if (ConsoleInput.InvokeRequired)
                ConsoleInput.Invoke(new EnableConsoleInputDelegate(EnableConsoleInput));
            else
                ConsoleInput.Enabled = true;
        }

        private void MenuStrip_Server_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_JoinLock.Checked = Settings.Server.JoinLocked;
            MenuStrip_Settings_Server_JoinLocked.Checked = Settings.Server.JoinLocked;
            MenuStrip_Server_ConnectionLock.Checked = Settings.Server.ConnectionLocked;
            MenuStrip_Settings_Server_ConnectionLocked.Checked = Settings.Server.ConnectionLocked;
            MenuStrip_Server_Version.Text = "Version: " + Environment.GetCompilationDate();

            MenuStrip_Server_Field_CurrentField.Text = "CurrentField: " + Strings.Resize(OpenYS.Field.FieldName, 32);
            MenuStrip_Server_Field_AfterRestart.Text = "AfterRestart: " + Strings.Resize(Settings.Loading.FieldName, 32);

            MenuStrip_Server_Restart_RestartTimer.Text = "Restart in " + OpenYS.GetTimeLeftUntilReset().ToString();
            if (OpenYS.GetTimeLeftUntilReset() == TimeSpan.MaxValue)
                MenuStrip_Server_Restart_RestartTimer.Text = "Server will not auto-restart.";

            MenuStrip_Server_Field_CurrentField.Text = "CurrentField: " + Strings.Resize(OpenYS.Field.FieldName, 32);
            MenuStrip_Server_Field_AfterRestart.Text = "AfterRestart: " + Strings.Resize(Settings.Loading.FieldName, 32);
            MenuStrip_Server_OwnerInfo_OwnerName.Text = "OwnerName: " + Strings.Resize(Settings.Options.OwnerName, 32);
            MenuStrip_Server_OwnerInfo_OwnerEmail.Text = "OwnerEmail: " + Strings.Resize(Settings.Options.OwnerEmail, 32);

            MenuStrip_Settings_Weather_Wind_CurrentInfo.Text = OpenYS.GetWeatherTAF();
        }

        private void MenuStrip_Server_JoinLock_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_JoinLock.Checked = !Settings.Server.JoinLocked;
            MenuStrip_Settings_Server_JoinLocked.Checked = !Settings.Server.JoinLocked;
            if (Settings.Server.JoinLocked)
                new Thread(() => CommandManager.ProcessConsole("/JoinLock OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/JoinLock ON")).Start();
        }

        private void MenuStrip_Server_ConnectionLock_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_ConnectionLock.Checked = !Settings.Server.ConnectionLocked;
            MenuStrip_Settings_Server_ConnectionLocked.Checked = !Settings.Server.ConnectionLocked;
            if (Settings.Server.ConnectionLocked)
                new Thread(() => CommandManager.ProcessConsole("/ServerLock OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/ServerLock ON")).Start();
        }

        private void MenuStrip_Server_Field_AfterRestart_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Field", //Prompt
                "Change Field", //Title
                Settings.Loading.FieldName.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/Field " + InputString)).Start();
        }

        private void MenuStrip_Server_Restart_RestartNow_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/RestartNow")).Start();
        }

        private void MenuStrip_Server_Restart_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/Restart")).Start();
        }

        private void MenuStrip_Server_Shutdown_ShutdownNow_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/ShutdownNow")).Start();
        }

        private void MenuStrip_Server_Shutdown_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/Shutdown")).Start();
        }

        private void MenuStrip_Server_Version_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/Version")).Start();
        }

        private void MenuStrip_Debug_DumpLog_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/Dump")).Start();
        }

        private void MenuStrip_Settings_Weather_ForceSetting_Blackout_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_ForceSetting_Blackout.Checked = !Settings.Options.Control.BlackOut;
            if (Settings.Options.Control.BlackOut)
                new Thread(() => CommandManager.ProcessConsole("/ForceBlackout OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/ForceBlackout ON")).Start();
        }

        private void MenuStrip_Settings_Weather_ForceSetting_Collisions_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_ForceSetting_Collisions.Checked = !Settings.Options.Control.Collisions;
            if (Settings.Options.Control.Collisions)
                new Thread(() => CommandManager.ProcessConsole("/ForceCollisions OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/ForceCollisions ON")).Start();
        }

        private void MenuStrip_Settings_Weather_ForceSetting_LandEverywhere_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.Checked = !Settings.Options.Control.LandEverywhere;
            if (Settings.Options.Control.LandEverywhere)
                new Thread(() => CommandManager.ProcessConsole("/ForceLandEverywhere OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/ForceLandEverywhere ON")).Start();
        }

        private void MenuStrip_Settings_Weather_ForceSetting_Fog_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_ForceSetting_Fog.Checked = !Settings.Options.Control.Fog;
            if (Settings.Options.Control.Fog)
                new Thread(() => CommandManager.ProcessConsole("/ForceFog OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/ForceFog ON")).Start();
        }

        private void MenuStrip_Settings_Weather_EnableSetting_Blackout_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_EnableSetting_Blackout.Checked = !Settings.Options.Enable.BlackOut;
            if (Settings.Options.Enable.BlackOut)
                new Thread(() => CommandManager.ProcessConsole("/EnableBlackout OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableBlackout ON")).Start();
        }

        private void MenuStrip_Settings_Weather_EnableSetting_Collisions_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_EnableSetting_Collisions.Checked = !Settings.Options.Enable.Collisions;
            if (Settings.Options.Enable.Collisions)
                new Thread(() => CommandManager.ProcessConsole("/EnableCollisions OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableCollisions ON")).Start();
        }

        private void MenuStrip_Settings_Weather_EnableSetting_LandEverywhere_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.Checked = !Settings.Options.Enable.LandEverywhere;
            if (Settings.Options.Enable.LandEverywhere)
                new Thread(() => CommandManager.ProcessConsole("/EnableLandEverywhere OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableLandEverywhere ON")).Start();
        }

        private void MenuStrip_Settings_Weather_EnableSetting_Fog_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Weather_EnableSetting_Fog.Checked = !Settings.Options.Enable.Fog;
            if (Settings.Options.Enable.Fog)
                new Thread(() => CommandManager.ProcessConsole("/EnableFog OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableFog ON")).Start();
        }

        private void MenuStrip_Settings_Weather_SetTime_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Server Time", //Prompt
                "Change Server Time", //Title
                Settings.Weather.Time.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/SetTime " + InputString)).Start();
        }

        private void MenuStrip_Settings_Weather_Wind_SetWindX_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Wind X-Axis Speed", //Prompt
                "Change Wind X-Axis Speed", //Title
                Settings.Weather.WindX.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/WindX " + InputString)).Start();
        }

        private void MenuStrip_Settings_Weather_Wind_SetWindY_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Wind Y-Axis Speed", //Prompt
                "Change Wind Y-Axis Speed", //Title
                Settings.Weather.WindY.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/WindY " + InputString)).Start();
        }

        private void MenuStrip_Settings_Weather_Wind_SetWindZ_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Wind Z-Axis Speed", //Prompt
                "Change Wind Z-Axis Speed", //Title
                Settings.Weather.WindZ.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/WindZ " + InputString)).Start();
        }

        private void MenuStrip_Settings_Weather_Wind_Change_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change wind (HHHSSKT), where HHH is Heading, SS is speed, and KT is knots. Follows standard TAF format.", //Prompt
                "Change Wind", //Title
                OpenYS.GetWeatherTAF(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/Wind " + InputString)).Start();
        }

        private void ClientsList_Enter(object sender, EventArgs e)
        {
            FocusOnConsoleInput();
        }

        private void MenuStrip_Settings_AdvWeather_DayNightCycle_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Day Length", //Prompt
                "Change Day Length", //Title
                Settings.Weather.Advanced.DayLength.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/DayLength " + InputString)).Start();
        }

        private void MenuStrip_Settings_AdvWeather_FogColor_Click(object sender, EventArgs e)
        {
            Colors.XRGBColor SettingsColor = Settings.Weather.Advanced.FogColor;
            ColorDialog ColorInput = new ColorDialog();
            ColorInput.AllowFullOpen = true;
            ColorInput.AnyColor = true;
            ColorInput.SolidColorOnly = false;
            ColorInput.Color = Color.FromArgb(SettingsColor.Red, SettingsColor.Green, SettingsColor.Blue);

            if (ColorInput.ShowDialog() == DialogResult.OK)
            {
                new Thread(() => CommandManager.ProcessConsole
                (
                    "/FogColor " + 
                    ColorInput.Color.R.ToString() + " " +
                    ColorInput.Color.G.ToString() + " " +
                    ColorInput.Color.B.ToString())
                ).Start();
            }
        }

        private void MenuStrip_Settings_AdvWeather_GroundColor_Click(object sender, EventArgs e)
        {
            Colors.XRGBColor SettingsColor = Settings.Weather.Advanced.GndColor;
            ColorDialog ColorInput = new ColorDialog();
            ColorInput.AllowFullOpen = true;
            ColorInput.AnyColor = true;
            ColorInput.SolidColorOnly = false;
            ColorInput.Color = Color.FromArgb(SettingsColor.Red, SettingsColor.Green, SettingsColor.Blue);

            if (ColorInput.ShowDialog() == DialogResult.OK)
            {
                new Thread(() => CommandManager.ProcessConsole
                (
                    "/GroundColor " +
                    ColorInput.Color.R.ToString() + " " +
                    ColorInput.Color.G.ToString() + " " +
                    ColorInput.Color.B.ToString())
                ).Start();
            }
        }

        private void MenuStrip_Settings_AdvWeather_SkyColor_Click(object sender, EventArgs e)
        {
            Colors.XRGBColor SettingsColor = Settings.Weather.Advanced.SkyColor;
            ColorDialog ColorInput = new ColorDialog();
            ColorInput.AllowFullOpen = true;
            ColorInput.AnyColor = true;
            ColorInput.SolidColorOnly = false;
            ColorInput.Color = Color.FromArgb(SettingsColor.Red, SettingsColor.Green, SettingsColor.Blue);

            if (ColorInput.ShowDialog() == DialogResult.OK)
            {
                new Thread(() => CommandManager.ProcessConsole
                (
                    "/SkyColor " +
                    ColorInput.Color.R.ToString() + " " +
                    ColorInput.Color.G.ToString() + " " +
                    ColorInput.Color.B.ToString())
                ).Start();
            }
        }

        private void MenuStrip_Settings_AdvWeather_Turbulence_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Turbulence %", //Prompt
                "Change Turbulence %", //Title
                Settings.Weather.Advanced.TurbulencePercent.ToString(), //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/Turbulence " + InputString)).Start();
        }

        private void MenuStrip_Settings_AdvWeather_UseVariableWeather_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_AdvWeather_UseVariableWeather.Checked = !Settings.Weather.Advanced.VariableWeather;
            if (Settings.Weather.Advanced.VariableWeather)
                new Thread(() => CommandManager.ProcessConsole("/VariableWeather OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/VariableWeather ON")).Start();
        }

        private void MenuStrip_Settings_Replay_Load_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Load Replay", //Prompt
                "Load Replay", //Title
                "./Replay.YFS", //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/LoadReplay " + InputString)).Start();
        }

        private void MenuStrip_Settings_Replay_Play_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/PlayReplay")).Start();
        }

        private void MenuStrip_Settings_Replay_Stop_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/StopReplay")).Start();
        }

        private void MenuStrip_Settings_OpenYS_ConsoleName_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Console Name", //Prompt
                "Change Console Name", //Title
                Settings.Options.ConsoleName, //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/ConsoleName " + InputString)).Start();
        }

        private void MenuStrip_Settings_OpenYS_BackgroundName_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Background Name", //Prompt
                "Change Background Name", //Title
                Settings.Options.SchedulerName, //Default Response
                -1, -1);
            new Thread(() => CommandManager.ProcessConsole("/BackgroundName " + InputString)).Start();
        }

        private void MenuStrip_Settings_EnableMissiles_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_EnableMissiles.Checked = !Settings.Weapons.Missiles;
            if (Settings.Weapons.Missiles)
                new Thread(() => CommandManager.ProcessConsole("/EnableMissiles OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableMissiles ON")).Start();
        }

        private void MenuStrip_Settings_EnableWeapons_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_EnableWeapons.Checked = !Settings.Weapons.Unguided;
            if (Settings.Weapons.Unguided)
                new Thread(() => CommandManager.ProcessConsole("/EnableWeapons OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableWeapons ON")).Start();
        }

        private void MenuStrip_Settings_LoginPassword_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Remote Login Password", //Prompt
                "Change Remote Login Password", //Title
                "", //Default Response
                -1, -1);

            if (InputString.Length <= 0) return;

            new Thread(() =>
            {
                if (InputString.Length < 8)
                {
                    Console.WriteLine("&6WARNING&f: New password is not long enough, it is advised you use a minimum of 8 chars!\n");
                }
                Settings.Administration.AdminPassword = InputString;
                Console.WriteLine("&aRemote login password has been updated!\n");
            }).Start();
        }

        private void MenuStrip_Settings_ReloadSettings_Click(object sender, EventArgs e)
        {
            new Thread(() => SettingsHandler.LoadAll()).Start();
        }

        private void MenuStrip_Settings_ReviveGrounds_Click(object sender, EventArgs e)
        {
            new Thread(() => CommandManager.ProcessConsole("/ReviveGrounds")).Start();
        }

        private void MenuStrip_Settings_EnableSmoke_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_EnableSmoke.Checked = !Settings.Flight.EnableSmoke;
            if (Settings.Flight.EnableSmoke)
                new Thread(() => CommandManager.ProcessConsole("/EnableSmoke OFF")).Start();
            else
                new Thread(() => CommandManager.ProcessConsole("/EnableSmoke ON")).Start();
        }

        private void MenuStrip_Settings_Loading_YSFDirectory_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change YSFlight Install Directory Location", //Prompt
                "Change YSFlight Install Directory Location", //Title
                Settings.Loading.YSFlightDirectory, //Default Response
                -1, -1);

            if (InputString.Length <= 0) return;

            if (!Directories.DirectoryExists(InputString))
            {
                Console.WriteLine("&6WARNING&f: YSFlight Directory provided doesn't appear to exist. Please check your input!\n");
            }

            new Thread(() =>
            {
                Settings.Loading.YSFlightDirectory = InputString;
                Console.WriteLine("&aYSFlight Install Directory location has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Loading_YSFNetcodeVersion_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change YSFlight Netcode Version", //Prompt
                "Change YSFlight Netcode Version", //Title
                Settings.Loading.YSFNetcodeVersion.ToString(), //Default Response
                -1, -1);

            int InputInt = 0;
            bool Failed = !Int32.TryParse(InputString, out InputInt);
            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: Netcode version provided not an Integer. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Loading.YSFNetcodeVersion = (uint)InputInt;
                Console.WriteLine("&aYSFlight Netcode version has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Loading_OYSNetcodeVersion_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change OpenYS Framework Version", //Prompt
                "Change OpenYS Framework Version", //Title
                Settings.Loading.OYSNetcodeVersion.ToString(), //Default Response
                -1, -1);

            int InputInt = 0;
            bool Failed = !Int32.TryParse(InputString, out InputInt);
            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: Netcode version provided not an Integer. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Loading.OYSNetcodeVersion = (uint)InputInt;
                Console.WriteLine("&aOpenYS Framework version number has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Loading_FieldName_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_Field_AfterRestart_Click(sender, e);
        }

        private void MenuStrip_Settings_Loading_AircraftPerAircraftListPacket_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Number of Aircraft per Aircraft List Packet", //Prompt
                "Change Number of Aircraft per Aircraft List Packet", //Title
                Settings.Loading.AircraftListPacketSize.ToString(), //Default Response
                -1, -1);

            int InputInt = 0;
            bool Failed = !Int32.TryParse(InputString, out InputInt);
            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: AircraftListPacketSize provided not an Integer. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Loading.AircraftListPacketSize = (uint)InputInt;
                Console.WriteLine("&aAircraftListPacketSize has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Loading_AutoOPLocalAddresses_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_AutoOPLocalAddresses.Checked = !Settings.Loading.AutoOPs;
            if (Settings.Loading.AutoOPs)
                new Thread(() =>
                {
                    Settings.Loading.AutoOPs = false;
                    Console.WriteLine("&aAutoOPLocalAddresses now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.AutoOPs = true;
                    Console.WriteLine("&aAutoOPLocalAddresses now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Loading_SendLoginWelcomeMessage_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Checked = !Settings.Loading.SendConnectedWelcomeMessage;
            if (Settings.Loading.SendConnectedWelcomeMessage)
                new Thread(() =>
                {
                    Settings.Loading.SendConnectedWelcomeMessage = false;
                    Console.WriteLine("&aSendConnectedWelcomeMessage now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.SendConnectedWelcomeMessage = true;
                    Console.WriteLine("&aSendConnectedWelcomeMessage now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Loading_SendLoginCompleteMessage_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Checked = !Settings.Loading.SendLogInCompleteWelcomeMessage;
            if (Settings.Loading.SendLogInCompleteWelcomeMessage)
                new Thread(() =>
                {
                    Settings.Loading.SendLogInCompleteWelcomeMessage = false;
                    Console.WriteLine("&aSendLogInCompleteWelcomeMessage now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.SendLogInCompleteWelcomeMessage = true;
                    Console.WriteLine("&aSendLogInCompleteWelcomeMessage now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Loading_AutoKickLoginBots_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_AutoKickLoginBots.Checked = !Settings.Loading.KickBots;
            if (Settings.Loading.KickBots)
                new Thread(() =>
                {
                    Settings.Loading.KickBots = false;
                    Console.WriteLine("&aKickBots now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.KickBots = true;
                    Console.WriteLine("&aKickBots now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Loading_NotifyWhenBotConnects_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_NotifyWhenBotConnects.Checked = !Settings.Loading.BotPingMessages;
            if (Settings.Loading.BotPingMessages)
                new Thread(() =>
                {
                    Settings.Loading.BotPingMessages = false;
                    Console.WriteLine("&aBotPingMessages now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.BotPingMessages = true;
                    Console.WriteLine("&aBotPingMessages now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Loading_SendLoadingPercentNotification_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_SendLoadingPercentNotification.Checked = !Settings.Loading.SendLoadingPercentNotification;
            if (Settings.Loading.SendLoadingPercentNotification)
                new Thread(() =>
                {
                    Settings.Loading.SendLoadingPercentNotification = false;
                    Console.WriteLine("&aSendLoadingPercentNotification now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.SendLoadingPercentNotification = true;
                    Console.WriteLine("&aSendLoadingPercentNotification now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Loading_SendLoginCompleteNotification_Click(object sender, EventArgs e)
        {
            MenuStrip_Settings_Loading_SendLoginCompleteNotification.Checked = !Settings.Loading.SendLoginCompleteNotification;
            if (Settings.Loading.SendLoginCompleteNotification)
                new Thread(() =>
                {
                    Settings.Loading.SendLoginCompleteNotification = false;
                    Console.WriteLine("&aSendLoginCompleteNotification now \"&cOFF&a\"!\n");
                }).Start();
            else
                new Thread(() =>
                {
                    Settings.Loading.SendLoginCompleteNotification = true;
                    Console.WriteLine("&aSendLoadingPercentNotification now \"ON\"!\n");
                }).Start();
        }

        private void MenuStrip_Settings_Server_ConnectionLocked_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_ConnectionLock_Click(sender, e);
        }

        private void MenuStrip_Settings_Server_JoinLocked_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_JoinLock_Click(sender, e);
        }

        private void MenuStrip_Settings_Server_ListenerPort_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Incoming Connection Port", //Prompt
                "Change Incoming Connection Port", //Title
                Settings.Server.ListenerPort.ToString(), //Default Response
                -1, -1);

            int InputInt = 0;
            bool Failed = !Int32.TryParse(InputString, out InputInt);
            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: Incoming connection port number provided not an Integer. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Server.ListenerPort = (uint)InputInt;
                Console.WriteLine("&aIncoming connection port number has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Server_RedirectToPort_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change ClientMode Destination Port", //Prompt
                "Change ClientMode Destination Port", //Title
                Settings.Server.ListenerPort.ToString(), //Default Response
                -1, -1);

            int InputInt = 0;
            bool Failed = !Int32.TryParse(InputString, out InputInt);
            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: ClientMode destination port number provided not an Integer. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Server.ListenerPort = (uint)InputInt;
                Console.WriteLine("&aClientMode destination port number has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Server_RedirectToAddress_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change YSFlight Install Directory Location", //Prompt
                "Change YSFlight Install Directory Location", //Title
                Settings.Server.HostAddress.ToString(), //Default Response
                -1, -1);

            if (InputString.Length <= 0) return;

            IPAddress InputAddress = IPAddress.Loopback;

            bool Failed = !IPAddress.TryParse(InputString, out InputAddress);

            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: ClientMode destination address provided not an IPAddress. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Server.HostAddress = InputAddress;
                Console.WriteLine("&aClientMode destination address has been updated!\n");
                Console.WriteLine("&cCHANGES WILL TAKE EFFECT AFTER NEXT RESTART!\n");
            }).Start();
        }

        private void MenuStrip_Settings_Server_RestartTimer_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_Restart_RestartTimer_Click(sender, e);
        }

        private void MenuStrip_Server_Restart_RestartTimer_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change RestartTimer", //Prompt
                "Change RestartTimer", //Title
                Settings.Server.RestartTimer.ToString(), //Default Response
                -1, -1);

            int InputInt = 0;
            bool Failed = !Int32.TryParse(InputString, out InputInt);
            if (Failed)
            {
                Console.WriteLine("&6ERROR&f: Restart Timer duration provided not an Integer. Please check your input!\n");
                return;
            }

            new Thread(() =>
            {
                Settings.Server.RestartTimer = InputInt;
                Console.WriteLine("&aRestart Timer has been updated!\n");
            }).Start();
        }

        private void MenuStrip_Server_OwnerInfo_OwnerName_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Owner Name", //Prompt
                "Change Owner Name", //Title
                Settings.Options.OwnerName, //Default Response
                -1, -1);
            if (InputString.Length > 0) Settings.Options.OwnerName = InputString;
        }

        private void MenuStrip_Server_OwnerInfo_OwnerEmail_Click(object sender, EventArgs e)
        {
            string InputString = Microsoft.VisualBasic.Interaction.InputBox(
                "Change Owner Email", //Prompt
                "Change Owner Email", //Title
                Settings.Options.OwnerEmail, //Default Response
                -1, -1);
            if (InputString.Length > 0) Settings.Options.OwnerEmail = InputString;
        }

        private void MenuStrip_Server_OwnerInfo_IsCustomBuild_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_OwnerInfo_IsCustomBuild.Checked = !Settings.Options.IsCustomBuild;
            Settings.Options.IsCustomBuild = !Settings.Options.IsCustomBuild;
        }

        private void MenuStrip_Debug_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_Click(sender, e);
        }

        private void MenuStrip_Settings_Click(object sender, EventArgs e)
        {
            MenuStrip_Server_Click(sender, e);
        }
    }
}
