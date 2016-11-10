using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenYS
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class ConsoleOutput
        {
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

            private static FontStyle DefaultStyle = FontStyle.Regular;
            private static class ConsoleFontStyles
            {
                //public static FontStyle Style_K = FontStyle.Strikeout; //Not Supported!
                public static FontStyle Style_L = FontStyle.Bold;
                public static FontStyle Style_M = FontStyle.Strikeout;
                public static FontStyle Style_N = FontStyle.Underline;
                public static FontStyle Style_R = FontStyle.Regular;
            }

            private char[] SplittingChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'K', 'L', 'M', 'N', 'R' };

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
            public ConcurrentQueue<List<MessageSegment>> MessageQueue = new ConcurrentQueue<List<MessageSegment>>();

            private void AddText(List<MessageSegment> Segments)
            {
                foreach (MessageSegment ThisSegment in Segments)
                {
                    this.MenuStrip.SelectionStart = this.MenuStrip.TextLength;
                    this.MenuStrip.SelectionLength = 0;

                    this.MenuStrip.SelectionColor = ThisSegment.Color;
                    this.MenuStrip.SelectionFont = new Font(this.MenuStrip.Font, ThisSegment.Style);
                    this.MenuStrip.AppendText(ThisSegment.String);
                    this.MenuStrip.SelectionColor = DefaultColor;
                    this.MenuStrip.SelectionFont = new Font(this.MenuStrip.Font, DefaultStyle);
                }
            }
            private delegate void AddTextDelegate(List<MessageSegment> Segments);

            private void PrepareText(string RawInput)
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
                this.MenuStrip.Invoke(d, new object[] { Segments });
            }

            public void WriteLine(string Input)
            {
                PrepareText(Input);
            }
        }


        private void FormWindow_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(1200, 675);
            this.ClientSize = new Size(1200, 675);
        }

        private void ConsoleInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CommandManager.ProcessConsole(this.Text);
                ConsoleInput.Text = "";
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
