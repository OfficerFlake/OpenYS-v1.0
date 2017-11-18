using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
 
namespace OpenYS
{
    public class OpenYS_Client_App : Form
    {
        public NotifyIcon trayIcon;
        private TextBox Username_TextBox;
        private TextBox Password_TextBox;
        private Label Heading_Label;
        private Label Username_Label;
        private Label Password_Label;
        private CheckBox ShowPassword_CheckBox;
        private Button Authentication_Button;
        private Label Instructions_Label_1;
        private Label AuthenticationStatus_Label;
        private Label Instructions_Label_2;
        public ContextMenu trayMenu;

        public bool PasswordIsEncrypted = true;

        public OpenYS_Client_App()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show", OnShow);
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "OpenYS-Tray";
            trayIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            trayIcon.DoubleClick += new EventHandler(OnShow);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
            SettingsHandler.LoadAll();
            Username_TextBox.Text = Settings.YSFHQ.Username;
            Password_TextBox.Text = Settings.YSFHQ.EncryptedPassword;
            Instructions_Label_1.Text = "Connect your YSFlight client to 127.0.0.1:" + Settings.Server.ListenerPort.ToString();
            Instructions_Label_2.Text = "To begin!";
            PasswordIsEncrypted = true;
            if (Username_TextBox.Text == "" | Password_TextBox.Text == "")
            {
                Password_TextBox.Text = "";
                PasswordIsEncrypted = false;
            }
            Application.DoEvents();

            if (Username_TextBox.Text != "" & Password_TextBox.Text != "") Authenticate();
            Application.DoEvents();


            //////////////////////////////////////////////
            // DO CLIENTSIDE STUFF
            //////////////////////////////////////////////

            //Initialisation
            #region Initialisation
            Thread.CurrentThread.Name = "OpenYS Client Core";
            Threads.List.Add(Thread.CurrentThread);

            Console._LogOutput = Log.ConsoleOutput;
            Console._Process = CommandManager.ProcessConsole;
            Console.ConsolePrompt = "&cOpenYS&f->&f";
            Console.Locked = true;

            if (Console.console_present)
            {
                System.Console.CursorVisible = false;
                System.Console.Clear();
                System.Console.Title = "OpenYS - YSFlight Open Source Client";
            }

            #region OYS_Console
            OpenYS.OpenYSConsole.SetConsole();
            OpenYS.OpenYSConsole.SetFakeClient();
            OpenYS.OpenYSConsole.Username = Settings.Options.ConsoleName;
            OpenYS.OpenYSConsole.OP();
            #endregion
            #region OYS_BackgroundHandler
            OpenYS.OpenYSBackGround.SetController();
            OpenYS.OpenYSBackGround.SetFakeClient();
            OpenYS.OpenYSBackGround.Username = Settings.Options.SchedulerName;
            OpenYS.OpenYSBackGround.OP();
            #endregion


            Sequencers.Process = CommandManager.ProcessScheduler;
            Schedulers.Process = CommandManager.ProcessScheduler;

            OpenYS.BuildType = OpenYS._BuildType.Client;
            Environment.EntryAssembly = Assembly.GetExecutingAssembly();
            #endregion

            Server.Listener.Start();
            OpenYS.YSFListener.ServerStarted.WaitOne();
            while (true)
            {
                Application.DoEvents();
            }

            //Try and start a proxy service!
        }

        private void OnExit(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("Are you sure you want to exit OpenYS Tray?\n\nYou will no longer be able to \"quick connect\" or use advanced OpenYS Features!", "Exit OpenYS Tray?", MessageBoxButtons.YesNo);
            if (Result == DialogResult.Yes)
            {
                System.Environment.Exit(0);
            }
        }

        private void OnTest(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["username"] = "OfficerFlake";
                values["password"] = "OfficerFlake378503";
                try
                {
                    //Key Verification:
                    //https://forum.ysfhq.com/api-check.php?apikey=

                    string APIKey = Environment.GetAPIKey();

                    if (APIKey == "MISSINGNO." | APIKey == "")
                    {
                        MessageBox.Show("APIKey not supplied for this custom build of OYS!\n\nTo get your own API Key, contact YSFHQ Webmasters, then place that key inside APIKey.txt, inside OpenYS-Library/Utilities!", "API Key Not Found!");
                        Application.Exit();
                    }

                    var response = client.UploadValues("https://forum.ysfhq.com/api-login.php?apikey=" + APIKey, values);
                    var responseString = Encoding.Default.GetString(response);
                    System.Console.WriteLine(responseString);
                }
                catch (System.Net.WebException WebError)
                {
                    if (WebError.Status == WebExceptionStatus.ProtocolError)
                    {
                        MessageBox.Show("APIKey not accepted for this release of OYS!\n\nPlease confirm your API Key with YSFHQ Webmasters, then update OpenYS-Library/Utilities/APIKey.txt!", "API Key Rejected!");
                    }
                    System.Console.WriteLine(WebError);
                }
            }
        }

        private void Authenticate()
        {
            //Disable all inputs!
            Username_TextBox.Enabled = false;
            Password_TextBox.Enabled = false;
            Authentication_Button.Enabled = false;
            ShowPassword_CheckBox.Enabled = false;
            AuthenticationStatus_Label.Text = "AUTHENTICATING...";
            AuthenticationStatus_Label.ForeColor = Color.FromArgb(240, 200, 0);
            AuthenticationStatus_Label.BackColor = Color.Transparent;
            OpenYS_Link.Stats._id = 0;
            Application.DoEvents();

            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["username"] = Username_TextBox.Text;

                if (PasswordIsEncrypted)
                {
                    values["password"] = OYS_Cryptography.DecryptPassword(Password_TextBox.Text);
                }
                else
                {
                    values["password"] = Password_TextBox.Text;
                }
                try
                {
                    //Key Verification:
                    //https://forum.ysfhq.com/api-check.php?apikey=

                    string APIKey = Environment.GetAPIKey();

                    if (APIKey == "MISSINGNO." | APIKey == "")
                    {
                        MessageBox.Show("APIKey not supplied for this custom build of OYS!\n\nTo get your own API Key, contact YSFHQ Webmasters, then place that key inside APIKey.txt, inside OpenYS-Library/Utilities!", "API Key Not Found!");
                        Username_TextBox.Enabled = false;
                        Password_TextBox.Enabled = false;
                        ShowPassword_CheckBox.Enabled = false;
                        Authentication_Button.Enabled = false; //leave this false, unable to authenticate without reloading!
                        AuthenticationStatus_Label.Text = "API KEY MISSING";
                        AuthenticationStatus_Label.ForeColor = Color.FromArgb(128, 0, 0);
                        AuthenticationStatus_Label.BackColor = Color.FromArgb(240, 200, 0);
                        OpenYS_Link.Stats._id = 0;
                        Application.DoEvents();
                        return;
                    }

                    var response = client.UploadValues("https://forum.ysfhq.com/api-login.php?apikey=" + APIKey, values);
                    var responseString = Encoding.Default.GetString(response);
                    System.Console.WriteLine(responseString);
                    bool Failed = !Int32.TryParse(responseString.Replace("\"", ""), out OpenYS_Link.Stats._id);

                    if (Failed | OpenYS_Link.Stats._id <= 0)
                    {
                        //NO GOOD!
                        Username_TextBox.Enabled = true;
                        Password_TextBox.Enabled = true;
                        ShowPassword_CheckBox.Enabled = true;
                        Authentication_Button.Enabled = true;
                        AuthenticationStatus_Label.Text = "AUTHENTICATION FAIL!";
                        AuthenticationStatus_Label.ForeColor = Color.FromArgb(240, 0, 0);
                        AuthenticationStatus_Label.BackColor = Color.Transparent;
                        Application.DoEvents();
                        return;
                    }
                    else
                    {
                        //ALL GOOD!
                        if (!PasswordIsEncrypted)
                        {
                            //Only update if the password has been modified!

                            SettingsHandler.LoadAll();
                            Settings.YSFHQ.Username = Username_TextBox.Text;
                            Settings.YSFHQ.EncryptedPassword = OYS_Cryptography.EncryptPassword(Password_TextBox.Text);
                            SettingsHandler.SaveAll();
                        }
                        Username_TextBox.Enabled = true;
                        Password_TextBox.Enabled = true;
                        ShowPassword_CheckBox.Enabled = true;
                        Authentication_Button.Enabled = true;
                        AuthenticationStatus_Label.Text = "AUTHENTICATED";
                        AuthenticationStatus_Label.ForeColor = Color.FromArgb(0, 128, 0);
                        AuthenticationStatus_Label.BackColor = Color.Transparent;
                        Application.DoEvents();

                        OpenYS_Link.GetAllStats();

                        //OpenYS_Link.OYS_Link_Response Response = OpenYS_Link.Get(YSFHQ_ID, "/stats_total_flight_seconds");
                        //MessageBox.Show(Response.Response, Response.Reason);

                        //^^ WORKING!
                        return;
                    }
                }
                catch (System.Net.WebException WebError)
                {
                    HttpStatusCode ErrorCode = ((HttpWebResponse)WebError.Response).StatusCode;

                    if (ErrorCode == HttpStatusCode.BadRequest) //400
                    {
                        //Missing Username / Password
                        Username_TextBox.Enabled = true;
                        Password_TextBox.Enabled = true;
                        ShowPassword_CheckBox.Enabled = true;
                        Authentication_Button.Enabled = true;
                        AuthenticationStatus_Label.Text = "NO USER OR PASS?";
                        AuthenticationStatus_Label.ForeColor = Color.FromArgb(240, 0, 0);
                        AuthenticationStatus_Label.BackColor = Color.Transparent;
                        Application.DoEvents();
                        return;
                    }
                    if (ErrorCode == HttpStatusCode.Unauthorized) //401
                    {
                        //Bad API Key
                        MessageBox.Show("APIKey not accepted for this release of OYS!\n\nPlease confirm your API Key with YSFHQ Webmasters, then update OpenYS-Library/Utilities/APIKey.txt!", "API Key Rejected!");
                        Username_TextBox.Enabled = false;
                        Password_TextBox.Enabled = false;
                        ShowPassword_CheckBox.Enabled = false;
                        Authentication_Button.Enabled = false; //leave this false, unable to authenticate without reloading!
                        AuthenticationStatus_Label.Text = "API KEY REJECTED";
                        AuthenticationStatus_Label.ForeColor = Color.FromArgb(128, 0, 0);
                        AuthenticationStatus_Label.BackColor = Color.FromArgb(240, 200, 0);
                        Application.DoEvents();
                        return;
                    }
                    if (ErrorCode == HttpStatusCode.Forbidden) //403
                    {
                        //Bad Username / Password
                        Username_TextBox.Enabled = true;
                        Password_TextBox.Enabled = true;
                        ShowPassword_CheckBox.Enabled = true;
                        Authentication_Button.Enabled = true;
                        AuthenticationStatus_Label.Text = "AUTHENTICATION FAIL!";
                        AuthenticationStatus_Label.ForeColor = Color.FromArgb(240, 0, 0);
                        AuthenticationStatus_Label.BackColor = Color.Transparent;
                        Application.DoEvents();
                        return;
                    }
                    //Generic Error?
                    MessageBox.Show("Generic Error Occured:\n\n" + WebError.Message, "Genereric Error!");
                    Username_TextBox.Enabled = true;
                    Password_TextBox.Enabled = true;
                    ShowPassword_CheckBox.Enabled = true;
                    Authentication_Button.Enabled = true;
                    AuthenticationStatus_Label.Text = "GENERIC ERROR!";
                    AuthenticationStatus_Label.ForeColor = Color.FromArgb(240, 0, 0);
                    AuthenticationStatus_Label.BackColor = Color.Transparent;
                    Application.DoEvents();
                    return;
                    //System.Console.WriteLine(WebError);
                }
            }
        }

        private void OnShow(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            ShowInTaskbar = true;
            Show();
            Focus();
            trayIcon.Visible = false;
        }

        private void OnHide(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Minimized;
            }
            ShowInTaskbar = false;
            Hide();
            //Focus();
            trayIcon.Visible = true;


        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void OpenYS_Tray_App_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; //To be closed using "Exit" SysTray Option!

            OnHide(sender, e);
        }

        private void InitializeComponent()
        {
            this.Username_TextBox = new System.Windows.Forms.TextBox();
            this.Password_TextBox = new System.Windows.Forms.TextBox();
            this.Heading_Label = new System.Windows.Forms.Label();
            this.Username_Label = new System.Windows.Forms.Label();
            this.Password_Label = new System.Windows.Forms.Label();
            this.ShowPassword_CheckBox = new System.Windows.Forms.CheckBox();
            this.Authentication_Button = new System.Windows.Forms.Button();
            this.Instructions_Label_1 = new System.Windows.Forms.Label();
            this.AuthenticationStatus_Label = new System.Windows.Forms.Label();
            this.Instructions_Label_2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Username_TextBox
            // 
            this.Username_TextBox.Location = new System.Drawing.Point(88, 45);
            this.Username_TextBox.Name = "Username_TextBox";
            this.Username_TextBox.Size = new System.Drawing.Size(364, 20);
            this.Username_TextBox.TabIndex = 0;
            this.Username_TextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.Username_TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // Password_TextBox
            // 
            this.Password_TextBox.Location = new System.Drawing.Point(88, 72);
            this.Password_TextBox.Name = "Password_TextBox";
            this.Password_TextBox.PasswordChar = '*';
            this.Password_TextBox.Size = new System.Drawing.Size(364, 20);
            this.Password_TextBox.TabIndex = 1;
            this.Password_TextBox.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.Password_TextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // Heading_Label
            // 
            this.Heading_Label.AutoSize = true;
            this.Heading_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Heading_Label.Location = new System.Drawing.Point(12, 9);
            this.Heading_Label.Name = "Heading_Label";
            this.Heading_Label.Size = new System.Drawing.Size(134, 13);
            this.Heading_Label.TabIndex = 2;
            this.Heading_Label.Text = "YSFHQ Authentication";
            // 
            // Username_Label
            // 
            this.Username_Label.AutoSize = true;
            this.Username_Label.Location = new System.Drawing.Point(12, 47);
            this.Username_Label.Name = "Username_Label";
            this.Username_Label.Size = new System.Drawing.Size(58, 13);
            this.Username_Label.TabIndex = 3;
            this.Username_Label.Text = "Username:";
            // 
            // Password_Label
            // 
            this.Password_Label.AutoSize = true;
            this.Password_Label.Location = new System.Drawing.Point(12, 75);
            this.Password_Label.Name = "Password_Label";
            this.Password_Label.Size = new System.Drawing.Size(56, 13);
            this.Password_Label.TabIndex = 4;
            this.Password_Label.Text = "Password:";
            // 
            // ShowPassword_CheckBox
            // 
            this.ShowPassword_CheckBox.AutoSize = true;
            this.ShowPassword_CheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ShowPassword_CheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowPassword_CheckBox.Location = new System.Drawing.Point(92, 102);
            this.ShowPassword_CheckBox.Name = "ShowPassword_CheckBox";
            this.ShowPassword_CheckBox.Size = new System.Drawing.Size(108, 17);
            this.ShowPassword_CheckBox.TabIndex = 6;
            this.ShowPassword_CheckBox.Text = "Show Password?";
            this.ShowPassword_CheckBox.UseVisualStyleBackColor = true;
            this.ShowPassword_CheckBox.CheckedChanged += new System.EventHandler(this.ShowPassword_CheckBox_CheckedChanged);
            // 
            // Authentication_Button
            // 
            this.Authentication_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Authentication_Button.Location = new System.Drawing.Point(353, 98);
            this.Authentication_Button.Name = "Authentication_Button";
            this.Authentication_Button.Size = new System.Drawing.Size(99, 23);
            this.Authentication_Button.TabIndex = 7;
            this.Authentication_Button.Text = "Authenticate!";
            this.Authentication_Button.UseVisualStyleBackColor = true;
            this.Authentication_Button.Click += new System.EventHandler(this.Authentication_Button_Click);
            // 
            // Instructions_Label_1
            // 
            this.Instructions_Label_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Instructions_Label_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Instructions_Label_1.Location = new System.Drawing.Point(152, 9);
            this.Instructions_Label_1.Name = "Instructions_Label_1";
            this.Instructions_Label_1.Size = new System.Drawing.Size(300, 13);
            this.Instructions_Label_1.TabIndex = 8;
            this.Instructions_Label_1.Text = "Loading internal server...";
            this.Instructions_Label_1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Instructions_Label_1.Click += new System.EventHandler(this.Instructions_Label_1_Click);
            // 
            // AuthenticationStatus_Label
            // 
            this.AuthenticationStatus_Label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AuthenticationStatus_Label.AutoSize = true;
            this.AuthenticationStatus_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AuthenticationStatus_Label.ForeColor = System.Drawing.Color.Red;
            this.AuthenticationStatus_Label.Location = new System.Drawing.Point(206, 103);
            this.AuthenticationStatus_Label.Name = "AuthenticationStatus_Label";
            this.AuthenticationStatus_Label.Size = new System.Drawing.Size(141, 13);
            this.AuthenticationStatus_Label.TabIndex = 9;
            this.AuthenticationStatus_Label.Text = "NOT AUTHENTICATED";
            this.AuthenticationStatus_Label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Instructions_Label_2
            // 
            this.Instructions_Label_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Instructions_Label_2.Location = new System.Drawing.Point(152, 22);
            this.Instructions_Label_2.Name = "Instructions_Label_2";
            this.Instructions_Label_2.Size = new System.Drawing.Size(300, 13);
            this.Instructions_Label_2.TabIndex = 10;
            this.Instructions_Label_2.Text = "Please wait...";
            this.Instructions_Label_2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OpenYS_Client_App
            // 
            this.ClientSize = new System.Drawing.Size(464, 133);
            this.Controls.Add(this.Instructions_Label_2);
            this.Controls.Add(this.AuthenticationStatus_Label);
            this.Controls.Add(this.Instructions_Label_1);
            this.Controls.Add(this.Authentication_Button);
            this.Controls.Add(this.ShowPassword_CheckBox);
            this.Controls.Add(this.Password_Label);
            this.Controls.Add(this.Username_Label);
            this.Controls.Add(this.Heading_Label);
            this.Controls.Add(this.Password_TextBox);
            this.Controls.Add(this.Username_TextBox);
            this.Name = "OpenYS_Client_App";
            this.Text = "OpenYS Tray";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenYS_Tray_App_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ShowPassword_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowPassword_CheckBox.CheckState == CheckState.Checked)
            {
                Password_TextBox.PasswordChar = '\0';
                if (PasswordIsEncrypted) Password_TextBox.Text = "";
            }
            else
            {
                Password_TextBox.PasswordChar = '*';
            }
        }

        private void Authentication_Button_Click(object sender, EventArgs e)
        {
            Authenticate();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            PasswordIsEncrypted = false;
            if (Password_TextBox.Text.Length <= 0)
            {
                Authentication_Button.Enabled = false;
                return;
            }
            else Authentication_Button.Enabled = true;

            if (Username_TextBox.Text.Length <= 0)
            {
                Authentication_Button.Enabled = false;
                return;
            }
            else Authentication_Button.Enabled = true;
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                Authenticate();
            }
        }

        private void Instructions_Label_1_Click(object sender, EventArgs e)
        {

        }


    }

    public static partial class OpenYS_Client
    {
        public static class Program 
        {
            #region DLL Functions
            [DllImport("kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            const int SW_HIDE = 0;
            const int SW_SHOW = 5;
            #endregion

            public static void Launcher(string[] args)
            {
                //workaround for the development launcher.
                #region DEBUG
    #if DEBUG
                Directory.SetCurrentDirectory("./Debug/");
    #endif
                #endregion
                #region RELEASE
    #if RELEASE
                    Directory.SetCurrentDirectory("./Release/");
    #endif
                #endregion
                Main(args);
            }

            public static void Main(string[] args)
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);

                //System.Console.Title = "OpenYS Startup";
                //System.Console.ForegroundColor = ConsoleColor.Red;
                //System.Console.WriteLine("About to Launch OYS!");
                //System.Console.WriteLine();
                //System.Console.WriteLine("If this doesn't succeed, there may be an issue with your .Net 4.0+ Install!");
                //System.Console.WriteLine(".Net 4.0+ can always be uninstalled and re-installed to fix it!");
                //System.Console.WriteLine();
                //System.Console.WriteLine("... That said, it is VERY rare that .Net has a problem!");
                //System.Console.WriteLine();
                //it is necessary to seperate Main and Run because of DLL loading...
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromLibrariesFolder);
                Run(args);
            }

            #region LoadFromLibrariesFolder
            private static Assembly LoadFromLibrariesFolder(object sender, ResolveEventArgs args)
            {
                //unblock all the files first!
                string[] FileNames = Directory.GetFiles("./Libraries/");
                foreach (string ThisFileName in FileNames)
                {
                    try
                    {
                        Unblock(ThisFileName);
                    }
                    catch
                    {
                    }
                }

                //This handler is called only when the common language runtime tries to bind to the assembly and fails.

                //Retrieve the list of referenced assemblies in an array of AssemblyName.
                Assembly MyAssembly, objExecutingAssembly;
                string strTempAssmbPath = "";

                try
                {
                    objExecutingAssembly = Assembly.GetExecutingAssembly();
                    if (args.RequestingAssembly != null) objExecutingAssembly = args.RequestingAssembly;
                    AssemblyName[] arrReferencedAssmbNames = objExecutingAssembly.GetReferencedAssemblies();

                    //Loop through the array of referenced assembly names.
                    foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
                    {
                        //Check for the assembly names that have raised the "AssemblyResolve" event.
                        if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
                        {
                            //Build the path of the assembly from where it has to be loaded.                
                            strTempAssmbPath = "./Libraries/" + args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
                            break;
                        }

                    }


                    //Load the assembly from the specified path.     
                    byte[] RawBytes = File.ReadAllBytes(strTempAssmbPath);
                    MyAssembly = Assembly.Load(RawBytes);
                }
                catch (Exception e)
                {
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.Clear();
                    System.Console.WriteLine("Failed to Launch OYS!");
                    System.Console.WriteLine();
                    System.Console.WriteLine("Specifically, Failed to load DLL: " + args.Name.Substring(0, args.Name.IndexOf(",")));
                    System.Console.WriteLine("Are you sure ALL the .DLL's are in the ./Libraries/ Folder?");
                    System.Console.WriteLine("");
                    System.Console.WriteLine("(Exception Info Follows)");
                    System.Console.WriteLine(e.ToString());
                    System.Console.WriteLine("");
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("[SCROLL UP TO SEE FULL DETAILS!]");
                    while (true)
                    {
                        System.Console.ReadKey(true);
                    }
                }
                //Return the loaded assembly.
                return MyAssembly;
            }

            #region Unblock File
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool DeleteFile(string name);

            private static bool Unblock(string fileName)
            {
                return DeleteFile(fileName + ":Zone.Identifier");
            }
            #endregion
            #endregion

            [STAThread]
            public static void Run(params string[] args)
            {
                OpenYS_Client_App MainTray = new OpenYS_Client_App();
                Application.Run(MainTray);
            }

        }
    }
}
