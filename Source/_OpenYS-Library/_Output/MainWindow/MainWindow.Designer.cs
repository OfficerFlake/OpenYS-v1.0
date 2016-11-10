namespace OpenYS
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.ConsoleOutputPanel = new System.Windows.Forms.Panel();
            this.ConsoleOutput = new System.Windows.Forms.RichTextBox();
            this.ConsoleInputPanel = new System.Windows.Forms.Panel();
            this.ConsoleInputPrompt = new System.Windows.Forms.Label();
            this.ConsoleInput = new System.Windows.Forms.TextBox();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.MenuStrip_Server = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Field = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Field_CurrentField = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Field_AfterRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Server_JoinLock = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_ConnectionLock = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Seperator_01 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Server_Restart = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Restart_RestartNow = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Restart_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Server_Restart_RestartTimer = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Shutdown = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Shutdown_ShutdownNow = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Shutdown_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Server_Shutdown_ShutdownOnNextRestart = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_Seperator_02 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Server_Version = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_OwnerInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_OwnerInfo_OwnerName = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_OwnerInfo_OwnerEmail = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Debug = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Debug_DumpLog = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_ForceSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_ForceSetting_Fog = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_EnableSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_EnableSetting_Blackout = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_EnableSetting_Collisions = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_EnableSetting_Fog = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_SetTime = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_Wind = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_Wind_CurrentInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_Wind_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Settings_Weather_Wind_SetWindX = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_Wind_SetWindY = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_Wind_SetWindZ = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Weather_Wind_Change = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather_DayNightCycle = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather_FogColor = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather_GroundColor = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather_SkyColor = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather_Turbulence = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Replay = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Replay_Load = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Replay_Play = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Replay_Stop = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Replay_JumpTo = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_OpenYS = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_OpenYS_ConsoleName = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_OpenYS_BackgroundName = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_EnableMissiles = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_EnableWeapons = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_LoginPassword = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Settings_ReloadSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_ReviveGrounds = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_EnableSmoke = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_YSFDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_YSFNetcodeVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_OYSNetcodeVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_FieldName = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_AutoKickLoginBots = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_NotifyWhenBotConnects = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server_ConnectionLocked = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server_JoinLocked = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server_Seperator_00 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Settings_Server_ListenerPort = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server_Seperator_01 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Settings_Server_RedirectToPort = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server_RedirectToAddress = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip_Settings_Server_Seperator_02 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuStrip_Settings_Server_RestartTimer = new System.Windows.Forms.ToolStripMenuItem();
            this.ClientListPanel = new System.Windows.Forms.Panel();
            this.ClientsList = new System.Windows.Forms.RichTextBox();
            this.ConsoleOutputPanel.SuspendLayout();
            this.MenuStrip.SuspendLayout();
            this.ClientListPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConsoleOutputPanel
            // 
            this.ConsoleOutputPanel.Controls.Add(this.ConsoleOutput);
            this.ConsoleOutputPanel.Location = new System.Drawing.Point(0, 23);
            this.ConsoleOutputPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ConsoleOutputPanel.Name = "ConsoleOutputPanel";
            this.ConsoleOutputPanel.Size = new System.Drawing.Size(1000, 639);
            this.ConsoleOutputPanel.TabIndex = 0;
            // 
            // ConsoleOutput
            // 
            this.ConsoleOutput.BackColor = System.Drawing.Color.Black;
            this.ConsoleOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConsoleOutput.Cursor = System.Windows.Forms.Cursors.Default;
            this.ConsoleOutput.ForeColor = System.Drawing.Color.White;
            this.ConsoleOutput.Location = new System.Drawing.Point(0, 0);
            this.ConsoleOutput.Name = "ConsoleOutput";
            this.ConsoleOutput.ReadOnly = true;
            this.ConsoleOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.ConsoleOutput.Size = new System.Drawing.Size(1000, 639);
            this.ConsoleOutput.TabIndex = 3;
            this.ConsoleOutput.TabStop = false;
            this.ConsoleOutput.Text = "";
            // 
            // ConsoleInputPanel
            // 
            this.ConsoleInputPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConsoleInputPanel.BackColor = System.Drawing.Color.Black;
            this.ConsoleInputPanel.Location = new System.Drawing.Point(0, 639);
            this.ConsoleInputPanel.Name = "ConsoleInputPanel";
            this.ConsoleInputPanel.Size = new System.Drawing.Size(1200, 75);
            this.ConsoleInputPanel.TabIndex = 1;
            // 
            // ConsoleInputPrompt
            // 
            this.ConsoleInputPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConsoleInputPrompt.BackColor = System.Drawing.Color.Black;
            this.ConsoleInputPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConsoleInputPrompt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ConsoleInputPrompt.Location = new System.Drawing.Point(0, 662);
            this.ConsoleInputPrompt.Margin = new System.Windows.Forms.Padding(0);
            this.ConsoleInputPrompt.Name = "ConsoleInputPrompt";
            this.ConsoleInputPrompt.Size = new System.Drawing.Size(64, 13);
            this.ConsoleInputPrompt.TabIndex = 2;
            this.ConsoleInputPrompt.Text = "OpenYS->";
            this.ConsoleInputPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ConsoleInput
            // 
            this.ConsoleInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ConsoleInput.BackColor = System.Drawing.Color.Black;
            this.ConsoleInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ConsoleInput.ForeColor = System.Drawing.Color.White;
            this.ConsoleInput.Location = new System.Drawing.Point(64, 662);
            this.ConsoleInput.Margin = new System.Windows.Forms.Padding(0);
            this.ConsoleInput.Name = "ConsoleInput";
            this.ConsoleInput.Size = new System.Drawing.Size(1136, 13);
            this.ConsoleInput.TabIndex = 0;
            this.ConsoleInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConsoleInput_KeyPress);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Server,
            this.MenuStrip_Debug,
            this.MenuStrip_Settings});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(1200, 24);
            this.MenuStrip.TabIndex = 5;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // MenuStrip_Server
            // 
            this.MenuStrip_Server.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Server_Field,
            this.MenuStrip_Server_Seperator_00,
            this.MenuStrip_Server_JoinLock,
            this.MenuStrip_Server_ConnectionLock,
            this.MenuStrip_Server_Seperator_01,
            this.MenuStrip_Server_Restart,
            this.MenuStrip_Server_Shutdown,
            this.MenuStrip_Server_Seperator_02,
            this.MenuStrip_Server_Version,
            this.MenuStrip_Server_OwnerInfo});
            this.MenuStrip_Server.Name = "MenuStrip_Server";
            this.MenuStrip_Server.Size = new System.Drawing.Size(51, 20);
            this.MenuStrip_Server.Text = "Server";
            this.MenuStrip_Server.Click += new System.EventHandler(this.MenuStrip_Server_Click);
            // 
            // MenuStrip_Server_Field
            // 
            this.MenuStrip_Server_Field.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Server_Field_CurrentField,
            this.MenuStrip_Server_Field_AfterRestart});
            this.MenuStrip_Server_Field.Name = "MenuStrip_Server_Field";
            this.MenuStrip_Server_Field.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_Field.Text = "Field";
            // 
            // MenuStrip_Server_Field_CurrentField
            // 
            this.MenuStrip_Server_Field_CurrentField.Enabled = false;
            this.MenuStrip_Server_Field_CurrentField.Name = "MenuStrip_Server_Field_CurrentField";
            this.MenuStrip_Server_Field_CurrentField.Size = new System.Drawing.Size(155, 22);
            this.MenuStrip_Server_Field_CurrentField.Text = "<CurrentField>";
            // 
            // MenuStrip_Server_Field_AfterRestart
            // 
            this.MenuStrip_Server_Field_AfterRestart.Name = "MenuStrip_Server_Field_AfterRestart";
            this.MenuStrip_Server_Field_AfterRestart.Size = new System.Drawing.Size(155, 22);
            this.MenuStrip_Server_Field_AfterRestart.Text = "<AfterRestart>";
            this.MenuStrip_Server_Field_AfterRestart.Click += new System.EventHandler(this.MenuStrip_Server_Field_AfterRestart_Click);
            // 
            // MenuStrip_Server_Seperator_00
            // 
            this.MenuStrip_Server_Seperator_00.Name = "MenuStrip_Server_Seperator_00";
            this.MenuStrip_Server_Seperator_00.Size = new System.Drawing.Size(163, 6);
            // 
            // MenuStrip_Server_JoinLock
            // 
            this.MenuStrip_Server_JoinLock.Name = "MenuStrip_Server_JoinLock";
            this.MenuStrip_Server_JoinLock.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_JoinLock.Text = "JoinLock?";
            this.MenuStrip_Server_JoinLock.Click += new System.EventHandler(this.MenuStrip_Server_JoinLock_Click);
            // 
            // MenuStrip_Server_ConnectionLock
            // 
            this.MenuStrip_Server_ConnectionLock.Name = "MenuStrip_Server_ConnectionLock";
            this.MenuStrip_Server_ConnectionLock.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_ConnectionLock.Text = "ConnectionLock?";
            this.MenuStrip_Server_ConnectionLock.Click += new System.EventHandler(this.MenuStrip_Server_ConnectionLock_Click);
            // 
            // MenuStrip_Server_Seperator_01
            // 
            this.MenuStrip_Server_Seperator_01.Name = "MenuStrip_Server_Seperator_01";
            this.MenuStrip_Server_Seperator_01.Size = new System.Drawing.Size(163, 6);
            // 
            // MenuStrip_Server_Restart
            // 
            this.MenuStrip_Server_Restart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Server_Restart_RestartNow,
            this.MenuStrip_Server_Restart_Seperator_00,
            this.MenuStrip_Server_Restart_RestartTimer});
            this.MenuStrip_Server_Restart.Name = "MenuStrip_Server_Restart";
            this.MenuStrip_Server_Restart.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_Restart.Text = "Restart...";
            this.MenuStrip_Server_Restart.Click += new System.EventHandler(this.MenuStrip_Server_Restart_Click);
            // 
            // MenuStrip_Server_Restart_RestartNow
            // 
            this.MenuStrip_Server_Restart_RestartNow.Name = "MenuStrip_Server_Restart_RestartNow";
            this.MenuStrip_Server_Restart_RestartNow.Size = new System.Drawing.Size(141, 22);
            this.MenuStrip_Server_Restart_RestartNow.Text = "RestartNow";
            this.MenuStrip_Server_Restart_RestartNow.Click += new System.EventHandler(this.MenuStrip_Server_Restart_RestartNow_Click);
            // 
            // MenuStrip_Server_Restart_Seperator_00
            // 
            this.MenuStrip_Server_Restart_Seperator_00.Name = "MenuStrip_Server_Restart_Seperator_00";
            this.MenuStrip_Server_Restart_Seperator_00.Size = new System.Drawing.Size(138, 6);
            // 
            // MenuStrip_Server_Restart_RestartTimer
            // 
            this.MenuStrip_Server_Restart_RestartTimer.Name = "MenuStrip_Server_Restart_RestartTimer";
            this.MenuStrip_Server_Restart_RestartTimer.Size = new System.Drawing.Size(141, 22);
            this.MenuStrip_Server_Restart_RestartTimer.Text = "RestartTimer";
            this.MenuStrip_Server_Restart_RestartTimer.Click += new System.EventHandler(this.MenuStrip_Server_Restart_RestartTimer_Click);
            // 
            // MenuStrip_Server_Shutdown
            // 
            this.MenuStrip_Server_Shutdown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Server_Shutdown_ShutdownNow,
            this.MenuStrip_Server_Shutdown_Seperator_00,
            this.MenuStrip_Server_Shutdown_ShutdownOnNextRestart});
            this.MenuStrip_Server_Shutdown.Name = "MenuStrip_Server_Shutdown";
            this.MenuStrip_Server_Shutdown.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_Shutdown.Text = "Shutdown...";
            this.MenuStrip_Server_Shutdown.Click += new System.EventHandler(this.MenuStrip_Server_Shutdown_Click);
            // 
            // MenuStrip_Server_Shutdown_ShutdownNow
            // 
            this.MenuStrip_Server_Shutdown_ShutdownNow.Name = "MenuStrip_Server_Shutdown_ShutdownNow";
            this.MenuStrip_Server_Shutdown_ShutdownNow.Size = new System.Drawing.Size(209, 22);
            this.MenuStrip_Server_Shutdown_ShutdownNow.Text = "ShutdownNow";
            this.MenuStrip_Server_Shutdown_ShutdownNow.Click += new System.EventHandler(this.MenuStrip_Server_Shutdown_ShutdownNow_Click);
            // 
            // MenuStrip_Server_Shutdown_Seperator_00
            // 
            this.MenuStrip_Server_Shutdown_Seperator_00.Name = "MenuStrip_Server_Shutdown_Seperator_00";
            this.MenuStrip_Server_Shutdown_Seperator_00.Size = new System.Drawing.Size(206, 6);
            // 
            // MenuStrip_Server_Shutdown_ShutdownOnNextRestart
            // 
            this.MenuStrip_Server_Shutdown_ShutdownOnNextRestart.Name = "MenuStrip_Server_Shutdown_ShutdownOnNextRestart";
            this.MenuStrip_Server_Shutdown_ShutdownOnNextRestart.Size = new System.Drawing.Size(209, 22);
            this.MenuStrip_Server_Shutdown_ShutdownOnNextRestart.Text = "ShutdownOnNextRestart?";
            // 
            // MenuStrip_Server_Seperator_02
            // 
            this.MenuStrip_Server_Seperator_02.Name = "MenuStrip_Server_Seperator_02";
            this.MenuStrip_Server_Seperator_02.Size = new System.Drawing.Size(163, 6);
            // 
            // MenuStrip_Server_Version
            // 
            this.MenuStrip_Server_Version.Name = "MenuStrip_Server_Version";
            this.MenuStrip_Server_Version.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_Version.Text = "<Version>";
            this.MenuStrip_Server_Version.Click += new System.EventHandler(this.MenuStrip_Server_Version_Click);
            // 
            // MenuStrip_Server_OwnerInfo
            // 
            this.MenuStrip_Server_OwnerInfo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Server_OwnerInfo_OwnerName,
            this.MenuStrip_Server_OwnerInfo_OwnerEmail,
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild});
            this.MenuStrip_Server_OwnerInfo.Name = "MenuStrip_Server_OwnerInfo";
            this.MenuStrip_Server_OwnerInfo.Size = new System.Drawing.Size(166, 22);
            this.MenuStrip_Server_OwnerInfo.Text = "OwnerInfo";
            // 
            // MenuStrip_Server_OwnerInfo_OwnerName
            // 
            this.MenuStrip_Server_OwnerInfo_OwnerName.Name = "MenuStrip_Server_OwnerInfo_OwnerName";
            this.MenuStrip_Server_OwnerInfo_OwnerName.Size = new System.Drawing.Size(177, 22);
            this.MenuStrip_Server_OwnerInfo_OwnerName.Text = "OwnerName";
            this.MenuStrip_Server_OwnerInfo_OwnerName.Click += new System.EventHandler(this.MenuStrip_Server_OwnerInfo_OwnerName_Click);
            // 
            // MenuStrip_Server_OwnerInfo_OwnerEmail
            // 
            this.MenuStrip_Server_OwnerInfo_OwnerEmail.Name = "MenuStrip_Server_OwnerInfo_OwnerEmail";
            this.MenuStrip_Server_OwnerInfo_OwnerEmail.Size = new System.Drawing.Size(177, 22);
            this.MenuStrip_Server_OwnerInfo_OwnerEmail.Text = "OwnerEmail";
            this.MenuStrip_Server_OwnerInfo_OwnerEmail.Click += new System.EventHandler(this.MenuStrip_Server_OwnerInfo_OwnerEmail_Click);
            // 
            // MenuStrip_Server_OwnerInfo_IsCustomBuild
            // 
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild.Name = "MenuStrip_Server_OwnerInfo_IsCustomBuild";
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild.Size = new System.Drawing.Size(177, 22);
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild.Text = "IsCustomisedBuild?";
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild.ToolTipText = "Customised Builds do NOT submit bugs! Please enable if you are modifying code!";
            this.MenuStrip_Server_OwnerInfo_IsCustomBuild.Click += new System.EventHandler(this.MenuStrip_Server_OwnerInfo_IsCustomBuild_Click);
            // 
            // MenuStrip_Debug
            // 
            this.MenuStrip_Debug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Debug_DumpLog});
            this.MenuStrip_Debug.Name = "MenuStrip_Debug";
            this.MenuStrip_Debug.Size = new System.Drawing.Size(54, 20);
            this.MenuStrip_Debug.Text = "Debug";
            this.MenuStrip_Debug.Click += new System.EventHandler(this.MenuStrip_Debug_Click);
            // 
            // MenuStrip_Debug_DumpLog
            // 
            this.MenuStrip_Debug_DumpLog.Name = "MenuStrip_Debug_DumpLog";
            this.MenuStrip_Debug_DumpLog.Size = new System.Drawing.Size(152, 22);
            this.MenuStrip_Debug_DumpLog.Text = "DumpLog";
            this.MenuStrip_Debug_DumpLog.Click += new System.EventHandler(this.MenuStrip_Debug_DumpLog_Click);
            // 
            // MenuStrip_Settings
            // 
            this.MenuStrip_Settings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Weather,
            this.MenuStrip_Settings_AdvWeather,
            this.MenuStrip_Settings_Replay,
            this.MenuStrip_Settings_OpenYS,
            this.MenuStrip_Settings_EnableMissiles,
            this.MenuStrip_Settings_EnableWeapons,
            this.MenuStrip_Settings_LoginPassword,
            this.MenuStrip_Settings_Seperator_00,
            this.MenuStrip_Settings_ReloadSettings,
            this.MenuStrip_Settings_ReviveGrounds,
            this.MenuStrip_Settings_EnableSmoke,
            this.MenuStrip_Settings_Loading,
            this.MenuStrip_Settings_Server});
            this.MenuStrip_Settings.Name = "MenuStrip_Settings";
            this.MenuStrip_Settings.Size = new System.Drawing.Size(61, 20);
            this.MenuStrip_Settings.Text = "Settings";
            this.MenuStrip_Settings.Click += new System.EventHandler(this.MenuStrip_Settings_Click);
            // 
            // MenuStrip_Settings_Weather
            // 
            this.MenuStrip_Settings_Weather.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Weather_ForceSetting,
            this.MenuStrip_Settings_Weather_EnableSetting,
            this.MenuStrip_Settings_Weather_SetTime,
            this.MenuStrip_Settings_Weather_Wind});
            this.MenuStrip_Settings_Weather.Name = "MenuStrip_Settings_Weather";
            this.MenuStrip_Settings_Weather.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_Weather.Text = "Weather";
            // 
            // MenuStrip_Settings_Weather_ForceSetting
            // 
            this.MenuStrip_Settings_Weather_ForceSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout,
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions,
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere,
            this.MenuStrip_Settings_Weather_ForceSetting_Fog});
            this.MenuStrip_Settings_Weather_ForceSetting.Name = "MenuStrip_Settings_Weather_ForceSetting";
            this.MenuStrip_Settings_Weather_ForceSetting.Size = new System.Drawing.Size(155, 22);
            this.MenuStrip_Settings_Weather_ForceSetting.Text = "ForceSetting...";
            // 
            // MenuStrip_Settings_Weather_ForceSetting_Blackout
            // 
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout.Checked = true;
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout.Name = "MenuStrip_Settings_Weather_ForceSetting_Blackout";
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout.Text = "Blackout";
            this.MenuStrip_Settings_Weather_ForceSetting_Blackout.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_ForceSetting_Blackout_Click);
            // 
            // MenuStrip_Settings_Weather_ForceSetting_Collisions
            // 
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions.Checked = true;
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions.Name = "MenuStrip_Settings_Weather_ForceSetting_Collisions";
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions.Text = "Collisions";
            this.MenuStrip_Settings_Weather_ForceSetting_Collisions.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_ForceSetting_Collisions_Click);
            // 
            // MenuStrip_Settings_Weather_ForceSetting_LandEverywhere
            // 
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.Checked = true;
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.Name = "MenuStrip_Settings_Weather_ForceSetting_LandEverywhere";
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.Text = "LandEverywhere";
            this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_ForceSetting_LandEverywhere_Click);
            // 
            // MenuStrip_Settings_Weather_ForceSetting_Fog
            // 
            this.MenuStrip_Settings_Weather_ForceSetting_Fog.Checked = true;
            this.MenuStrip_Settings_Weather_ForceSetting_Fog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Weather_ForceSetting_Fog.Name = "MenuStrip_Settings_Weather_ForceSetting_Fog";
            this.MenuStrip_Settings_Weather_ForceSetting_Fog.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_ForceSetting_Fog.Text = "Fog";
            this.MenuStrip_Settings_Weather_ForceSetting_Fog.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_ForceSetting_Fog_Click);
            // 
            // MenuStrip_Settings_Weather_EnableSetting
            // 
            this.MenuStrip_Settings_Weather_EnableSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Weather_EnableSetting_Blackout,
            this.MenuStrip_Settings_Weather_EnableSetting_Collisions,
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere,
            this.MenuStrip_Settings_Weather_EnableSetting_Fog});
            this.MenuStrip_Settings_Weather_EnableSetting.Name = "MenuStrip_Settings_Weather_EnableSetting";
            this.MenuStrip_Settings_Weather_EnableSetting.Size = new System.Drawing.Size(155, 22);
            this.MenuStrip_Settings_Weather_EnableSetting.Text = "EnableSetting...";
            // 
            // MenuStrip_Settings_Weather_EnableSetting_Blackout
            // 
            this.MenuStrip_Settings_Weather_EnableSetting_Blackout.Name = "MenuStrip_Settings_Weather_EnableSetting_Blackout";
            this.MenuStrip_Settings_Weather_EnableSetting_Blackout.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_EnableSetting_Blackout.Text = "Blackout";
            this.MenuStrip_Settings_Weather_EnableSetting_Blackout.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_EnableSetting_Blackout_Click);
            // 
            // MenuStrip_Settings_Weather_EnableSetting_Collisions
            // 
            this.MenuStrip_Settings_Weather_EnableSetting_Collisions.Name = "MenuStrip_Settings_Weather_EnableSetting_Collisions";
            this.MenuStrip_Settings_Weather_EnableSetting_Collisions.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_EnableSetting_Collisions.Text = "Collisions";
            this.MenuStrip_Settings_Weather_EnableSetting_Collisions.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_EnableSetting_Collisions_Click);
            // 
            // MenuStrip_Settings_Weather_EnableSetting_LandEverywhere
            // 
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.Checked = true;
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.Name = "MenuStrip_Settings_Weather_EnableSetting_LandEverywhere";
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.Text = "LandEverywhere";
            this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_EnableSetting_LandEverywhere_Click);
            // 
            // MenuStrip_Settings_Weather_EnableSetting_Fog
            // 
            this.MenuStrip_Settings_Weather_EnableSetting_Fog.Checked = true;
            this.MenuStrip_Settings_Weather_EnableSetting_Fog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Weather_EnableSetting_Fog.Name = "MenuStrip_Settings_Weather_EnableSetting_Fog";
            this.MenuStrip_Settings_Weather_EnableSetting_Fog.Size = new System.Drawing.Size(160, 22);
            this.MenuStrip_Settings_Weather_EnableSetting_Fog.Text = "Fog";
            this.MenuStrip_Settings_Weather_EnableSetting_Fog.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_EnableSetting_Fog_Click);
            // 
            // MenuStrip_Settings_Weather_SetTime
            // 
            this.MenuStrip_Settings_Weather_SetTime.Name = "MenuStrip_Settings_Weather_SetTime";
            this.MenuStrip_Settings_Weather_SetTime.Size = new System.Drawing.Size(155, 22);
            this.MenuStrip_Settings_Weather_SetTime.Text = "SetTime";
            this.MenuStrip_Settings_Weather_SetTime.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_SetTime_Click);
            // 
            // MenuStrip_Settings_Weather_Wind
            // 
            this.MenuStrip_Settings_Weather_Wind.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Weather_Wind_CurrentInfo,
            this.MenuStrip_Settings_Weather_Wind_Seperator_00,
            this.MenuStrip_Settings_Weather_Wind_SetWindX,
            this.MenuStrip_Settings_Weather_Wind_SetWindY,
            this.MenuStrip_Settings_Weather_Wind_SetWindZ,
            this.MenuStrip_Settings_Weather_Wind_Change});
            this.MenuStrip_Settings_Weather_Wind.Name = "MenuStrip_Settings_Weather_Wind";
            this.MenuStrip_Settings_Weather_Wind.Size = new System.Drawing.Size(155, 22);
            this.MenuStrip_Settings_Weather_Wind.Text = "Wind...";
            // 
            // MenuStrip_Settings_Weather_Wind_CurrentInfo
            // 
            this.MenuStrip_Settings_Weather_Wind_CurrentInfo.Enabled = false;
            this.MenuStrip_Settings_Weather_Wind_CurrentInfo.Name = "MenuStrip_Settings_Weather_Wind_CurrentInfo";
            this.MenuStrip_Settings_Weather_Wind_CurrentInfo.Size = new System.Drawing.Size(152, 22);
            this.MenuStrip_Settings_Weather_Wind_CurrentInfo.Text = "<CurrentInfo>";
            // 
            // MenuStrip_Settings_Weather_Wind_Seperator_00
            // 
            this.MenuStrip_Settings_Weather_Wind_Seperator_00.Name = "MenuStrip_Settings_Weather_Wind_Seperator_00";
            this.MenuStrip_Settings_Weather_Wind_Seperator_00.Size = new System.Drawing.Size(149, 6);
            // 
            // MenuStrip_Settings_Weather_Wind_SetWindX
            // 
            this.MenuStrip_Settings_Weather_Wind_SetWindX.Name = "MenuStrip_Settings_Weather_Wind_SetWindX";
            this.MenuStrip_Settings_Weather_Wind_SetWindX.Size = new System.Drawing.Size(152, 22);
            this.MenuStrip_Settings_Weather_Wind_SetWindX.Text = "SetWindX";
            this.MenuStrip_Settings_Weather_Wind_SetWindX.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_Wind_SetWindX_Click);
            // 
            // MenuStrip_Settings_Weather_Wind_SetWindY
            // 
            this.MenuStrip_Settings_Weather_Wind_SetWindY.Name = "MenuStrip_Settings_Weather_Wind_SetWindY";
            this.MenuStrip_Settings_Weather_Wind_SetWindY.Size = new System.Drawing.Size(152, 22);
            this.MenuStrip_Settings_Weather_Wind_SetWindY.Text = "SetWindY";
            this.MenuStrip_Settings_Weather_Wind_SetWindY.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_Wind_SetWindY_Click);
            // 
            // MenuStrip_Settings_Weather_Wind_SetWindZ
            // 
            this.MenuStrip_Settings_Weather_Wind_SetWindZ.Name = "MenuStrip_Settings_Weather_Wind_SetWindZ";
            this.MenuStrip_Settings_Weather_Wind_SetWindZ.Size = new System.Drawing.Size(152, 22);
            this.MenuStrip_Settings_Weather_Wind_SetWindZ.Text = "SetWindZ";
            this.MenuStrip_Settings_Weather_Wind_SetWindZ.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_Wind_SetWindZ_Click);
            // 
            // MenuStrip_Settings_Weather_Wind_Change
            // 
            this.MenuStrip_Settings_Weather_Wind_Change.Name = "MenuStrip_Settings_Weather_Wind_Change";
            this.MenuStrip_Settings_Weather_Wind_Change.Size = new System.Drawing.Size(152, 22);
            this.MenuStrip_Settings_Weather_Wind_Change.Text = "Change...";
            this.MenuStrip_Settings_Weather_Wind_Change.Click += new System.EventHandler(this.MenuStrip_Settings_Weather_Wind_Change_Click);
            // 
            // MenuStrip_Settings_AdvWeather
            // 
            this.MenuStrip_Settings_AdvWeather.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_AdvWeather_DayNightCycle,
            this.MenuStrip_Settings_AdvWeather_FogColor,
            this.MenuStrip_Settings_AdvWeather_GroundColor,
            this.MenuStrip_Settings_AdvWeather_SkyColor,
            this.MenuStrip_Settings_AdvWeather_Turbulence,
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather});
            this.MenuStrip_Settings_AdvWeather.Name = "MenuStrip_Settings_AdvWeather";
            this.MenuStrip_Settings_AdvWeather.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_AdvWeather.Text = "Adv. Weather";
            // 
            // MenuStrip_Settings_AdvWeather_DayNightCycle
            // 
            this.MenuStrip_Settings_AdvWeather_DayNightCycle.Name = "MenuStrip_Settings_AdvWeather_DayNightCycle";
            this.MenuStrip_Settings_AdvWeather_DayNightCycle.Size = new System.Drawing.Size(183, 22);
            this.MenuStrip_Settings_AdvWeather_DayNightCycle.Text = "DayNightCycle";
            this.MenuStrip_Settings_AdvWeather_DayNightCycle.Click += new System.EventHandler(this.MenuStrip_Settings_AdvWeather_DayNightCycle_Click);
            // 
            // MenuStrip_Settings_AdvWeather_FogColor
            // 
            this.MenuStrip_Settings_AdvWeather_FogColor.Name = "MenuStrip_Settings_AdvWeather_FogColor";
            this.MenuStrip_Settings_AdvWeather_FogColor.Size = new System.Drawing.Size(183, 22);
            this.MenuStrip_Settings_AdvWeather_FogColor.Text = "FogColor";
            this.MenuStrip_Settings_AdvWeather_FogColor.Click += new System.EventHandler(this.MenuStrip_Settings_AdvWeather_FogColor_Click);
            // 
            // MenuStrip_Settings_AdvWeather_GroundColor
            // 
            this.MenuStrip_Settings_AdvWeather_GroundColor.Name = "MenuStrip_Settings_AdvWeather_GroundColor";
            this.MenuStrip_Settings_AdvWeather_GroundColor.Size = new System.Drawing.Size(183, 22);
            this.MenuStrip_Settings_AdvWeather_GroundColor.Text = "GroundColor";
            this.MenuStrip_Settings_AdvWeather_GroundColor.Click += new System.EventHandler(this.MenuStrip_Settings_AdvWeather_GroundColor_Click);
            // 
            // MenuStrip_Settings_AdvWeather_SkyColor
            // 
            this.MenuStrip_Settings_AdvWeather_SkyColor.Name = "MenuStrip_Settings_AdvWeather_SkyColor";
            this.MenuStrip_Settings_AdvWeather_SkyColor.Size = new System.Drawing.Size(183, 22);
            this.MenuStrip_Settings_AdvWeather_SkyColor.Text = "SkyColor";
            this.MenuStrip_Settings_AdvWeather_SkyColor.Click += new System.EventHandler(this.MenuStrip_Settings_AdvWeather_SkyColor_Click);
            // 
            // MenuStrip_Settings_AdvWeather_Turbulence
            // 
            this.MenuStrip_Settings_AdvWeather_Turbulence.Name = "MenuStrip_Settings_AdvWeather_Turbulence";
            this.MenuStrip_Settings_AdvWeather_Turbulence.Size = new System.Drawing.Size(183, 22);
            this.MenuStrip_Settings_AdvWeather_Turbulence.Text = "Turbulence";
            this.MenuStrip_Settings_AdvWeather_Turbulence.Click += new System.EventHandler(this.MenuStrip_Settings_AdvWeather_Turbulence_Click);
            // 
            // MenuStrip_Settings_AdvWeather_UseVariableWeather
            // 
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather.Checked = true;
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather.Name = "MenuStrip_Settings_AdvWeather_UseVariableWeather";
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather.Size = new System.Drawing.Size(183, 22);
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather.Text = "UseVariableWeather?";
            this.MenuStrip_Settings_AdvWeather_UseVariableWeather.Click += new System.EventHandler(this.MenuStrip_Settings_AdvWeather_UseVariableWeather_Click);
            // 
            // MenuStrip_Settings_Replay
            // 
            this.MenuStrip_Settings_Replay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Replay_Load,
            this.MenuStrip_Settings_Replay_Play,
            this.MenuStrip_Settings_Replay_Stop,
            this.MenuStrip_Settings_Replay_JumpTo});
            this.MenuStrip_Settings_Replay.Name = "MenuStrip_Settings_Replay";
            this.MenuStrip_Settings_Replay.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_Replay.Text = "Replay";
            // 
            // MenuStrip_Settings_Replay_Load
            // 
            this.MenuStrip_Settings_Replay_Load.Name = "MenuStrip_Settings_Replay_Load";
            this.MenuStrip_Settings_Replay_Load.Size = new System.Drawing.Size(116, 22);
            this.MenuStrip_Settings_Replay_Load.Text = "Load";
            this.MenuStrip_Settings_Replay_Load.Click += new System.EventHandler(this.MenuStrip_Settings_Replay_Load_Click);
            // 
            // MenuStrip_Settings_Replay_Play
            // 
            this.MenuStrip_Settings_Replay_Play.Name = "MenuStrip_Settings_Replay_Play";
            this.MenuStrip_Settings_Replay_Play.Size = new System.Drawing.Size(116, 22);
            this.MenuStrip_Settings_Replay_Play.Text = "Play";
            this.MenuStrip_Settings_Replay_Play.Click += new System.EventHandler(this.MenuStrip_Settings_Replay_Play_Click);
            // 
            // MenuStrip_Settings_Replay_Stop
            // 
            this.MenuStrip_Settings_Replay_Stop.Name = "MenuStrip_Settings_Replay_Stop";
            this.MenuStrip_Settings_Replay_Stop.Size = new System.Drawing.Size(116, 22);
            this.MenuStrip_Settings_Replay_Stop.Text = "Stop";
            this.MenuStrip_Settings_Replay_Stop.Click += new System.EventHandler(this.MenuStrip_Settings_Replay_Stop_Click);
            // 
            // MenuStrip_Settings_Replay_JumpTo
            // 
            this.MenuStrip_Settings_Replay_JumpTo.Enabled = false;
            this.MenuStrip_Settings_Replay_JumpTo.Name = "MenuStrip_Settings_Replay_JumpTo";
            this.MenuStrip_Settings_Replay_JumpTo.Size = new System.Drawing.Size(116, 22);
            this.MenuStrip_Settings_Replay_JumpTo.Text = "JumpTo";
            // 
            // MenuStrip_Settings_OpenYS
            // 
            this.MenuStrip_Settings_OpenYS.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_OpenYS_ConsoleName,
            this.MenuStrip_Settings_OpenYS_BackgroundName});
            this.MenuStrip_Settings_OpenYS.Name = "MenuStrip_Settings_OpenYS";
            this.MenuStrip_Settings_OpenYS.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_OpenYS.Text = "OpenYS";
            // 
            // MenuStrip_Settings_OpenYS_ConsoleName
            // 
            this.MenuStrip_Settings_OpenYS_ConsoleName.Name = "MenuStrip_Settings_OpenYS_ConsoleName";
            this.MenuStrip_Settings_OpenYS_ConsoleName.Size = new System.Drawing.Size(170, 22);
            this.MenuStrip_Settings_OpenYS_ConsoleName.Text = "ConsoleName";
            this.MenuStrip_Settings_OpenYS_ConsoleName.Click += new System.EventHandler(this.MenuStrip_Settings_OpenYS_ConsoleName_Click);
            // 
            // MenuStrip_Settings_OpenYS_BackgroundName
            // 
            this.MenuStrip_Settings_OpenYS_BackgroundName.Name = "MenuStrip_Settings_OpenYS_BackgroundName";
            this.MenuStrip_Settings_OpenYS_BackgroundName.Size = new System.Drawing.Size(170, 22);
            this.MenuStrip_Settings_OpenYS_BackgroundName.Text = "BackgroundName";
            this.MenuStrip_Settings_OpenYS_BackgroundName.Click += new System.EventHandler(this.MenuStrip_Settings_OpenYS_BackgroundName_Click);
            // 
            // MenuStrip_Settings_EnableMissiles
            // 
            this.MenuStrip_Settings_EnableMissiles.Checked = true;
            this.MenuStrip_Settings_EnableMissiles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_EnableMissiles.Name = "MenuStrip_Settings_EnableMissiles";
            this.MenuStrip_Settings_EnableMissiles.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_EnableMissiles.Text = "EnableMissiles?";
            this.MenuStrip_Settings_EnableMissiles.Click += new System.EventHandler(this.MenuStrip_Settings_EnableMissiles_Click);
            // 
            // MenuStrip_Settings_EnableWeapons
            // 
            this.MenuStrip_Settings_EnableWeapons.Checked = true;
            this.MenuStrip_Settings_EnableWeapons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_EnableWeapons.Name = "MenuStrip_Settings_EnableWeapons";
            this.MenuStrip_Settings_EnableWeapons.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_EnableWeapons.Text = "EnableWeapons?";
            this.MenuStrip_Settings_EnableWeapons.Click += new System.EventHandler(this.MenuStrip_Settings_EnableWeapons_Click);
            // 
            // MenuStrip_Settings_LoginPassword
            // 
            this.MenuStrip_Settings_LoginPassword.Name = "MenuStrip_Settings_LoginPassword";
            this.MenuStrip_Settings_LoginPassword.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_LoginPassword.Text = "LoginPassword...";
            this.MenuStrip_Settings_LoginPassword.Click += new System.EventHandler(this.MenuStrip_Settings_LoginPassword_Click);
            // 
            // MenuStrip_Settings_Seperator_00
            // 
            this.MenuStrip_Settings_Seperator_00.Name = "MenuStrip_Settings_Seperator_00";
            this.MenuStrip_Settings_Seperator_00.Size = new System.Drawing.Size(160, 6);
            // 
            // MenuStrip_Settings_ReloadSettings
            // 
            this.MenuStrip_Settings_ReloadSettings.Name = "MenuStrip_Settings_ReloadSettings";
            this.MenuStrip_Settings_ReloadSettings.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_ReloadSettings.Text = "Reload Settings";
            this.MenuStrip_Settings_ReloadSettings.Click += new System.EventHandler(this.MenuStrip_Settings_ReloadSettings_Click);
            // 
            // MenuStrip_Settings_ReviveGrounds
            // 
            this.MenuStrip_Settings_ReviveGrounds.Name = "MenuStrip_Settings_ReviveGrounds";
            this.MenuStrip_Settings_ReviveGrounds.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_ReviveGrounds.Text = "ReviveGrounds";
            this.MenuStrip_Settings_ReviveGrounds.Click += new System.EventHandler(this.MenuStrip_Settings_ReviveGrounds_Click);
            // 
            // MenuStrip_Settings_EnableSmoke
            // 
            this.MenuStrip_Settings_EnableSmoke.Checked = true;
            this.MenuStrip_Settings_EnableSmoke.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_EnableSmoke.Name = "MenuStrip_Settings_EnableSmoke";
            this.MenuStrip_Settings_EnableSmoke.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_EnableSmoke.Text = "EnableSmoke?";
            this.MenuStrip_Settings_EnableSmoke.Click += new System.EventHandler(this.MenuStrip_Settings_EnableSmoke_Click);
            // 
            // MenuStrip_Settings_Loading
            // 
            this.MenuStrip_Settings_Loading.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Loading_YSFDirectory,
            this.MenuStrip_Settings_Loading_YSFNetcodeVersion,
            this.MenuStrip_Settings_Loading_OYSNetcodeVersion,
            this.MenuStrip_Settings_Loading_FieldName,
            this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket,
            this.MenuStrip_Settings_Loading_Seperator_00,
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses,
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage,
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage,
            this.MenuStrip_Settings_Loading_AutoKickLoginBots,
            this.MenuStrip_Settings_Loading_NotifyWhenBotConnects,
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification,
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification});
            this.MenuStrip_Settings_Loading.Name = "MenuStrip_Settings_Loading";
            this.MenuStrip_Settings_Loading.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_Loading.Text = "Loading...";
            // 
            // MenuStrip_Settings_Loading_YSFDirectory
            // 
            this.MenuStrip_Settings_Loading_YSFDirectory.Name = "MenuStrip_Settings_Loading_YSFDirectory";
            this.MenuStrip_Settings_Loading_YSFDirectory.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_YSFDirectory.Text = "<YSFDirectory>";
            this.MenuStrip_Settings_Loading_YSFDirectory.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_YSFDirectory_Click);
            // 
            // MenuStrip_Settings_Loading_YSFNetcodeVersion
            // 
            this.MenuStrip_Settings_Loading_YSFNetcodeVersion.Name = "MenuStrip_Settings_Loading_YSFNetcodeVersion";
            this.MenuStrip_Settings_Loading_YSFNetcodeVersion.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_YSFNetcodeVersion.Text = "<YSFNetcodeVersion>";
            this.MenuStrip_Settings_Loading_YSFNetcodeVersion.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_YSFNetcodeVersion_Click);
            // 
            // MenuStrip_Settings_Loading_OYSNetcodeVersion
            // 
            this.MenuStrip_Settings_Loading_OYSNetcodeVersion.Name = "MenuStrip_Settings_Loading_OYSNetcodeVersion";
            this.MenuStrip_Settings_Loading_OYSNetcodeVersion.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_OYSNetcodeVersion.Text = "<OYSNetcodeVersion>";
            this.MenuStrip_Settings_Loading_OYSNetcodeVersion.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_OYSNetcodeVersion_Click);
            // 
            // MenuStrip_Settings_Loading_FieldName
            // 
            this.MenuStrip_Settings_Loading_FieldName.Name = "MenuStrip_Settings_Loading_FieldName";
            this.MenuStrip_Settings_Loading_FieldName.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_FieldName.Text = "<FieldName>";
            this.MenuStrip_Settings_Loading_FieldName.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_FieldName_Click);
            // 
            // MenuStrip_Settings_Loading_AircraftPerAircraftListPacket
            // 
            this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket.Name = "MenuStrip_Settings_Loading_AircraftPerAircraftListPacket";
            this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket.Text = "<AircraftPerAircraftListPacket>";
            this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_AircraftPerAircraftListPacket_Click);
            // 
            // MenuStrip_Settings_Loading_Seperator_00
            // 
            this.MenuStrip_Settings_Loading_Seperator_00.Name = "MenuStrip_Settings_Loading_Seperator_00";
            this.MenuStrip_Settings_Loading_Seperator_00.Size = new System.Drawing.Size(248, 6);
            // 
            // MenuStrip_Settings_Loading_AutoOPLocalAddresses
            // 
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses.Checked = true;
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses.Name = "MenuStrip_Settings_Loading_AutoOPLocalAddresses";
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses.Text = "AutoOPLocalAddresses?";
            this.MenuStrip_Settings_Loading_AutoOPLocalAddresses.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_AutoOPLocalAddresses_Click);
            // 
            // MenuStrip_Settings_Loading_SendLoginWelcomeMessage
            // 
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Checked = true;
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Name = "MenuStrip_Settings_Loading_SendLoginWelcomeMessage";
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Text = "SendLoginWelcomeMessage?";
            this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_SendLoginWelcomeMessage_Click);
            // 
            // MenuStrip_Settings_Loading_SendLoginCompleteMessage
            // 
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage.Checked = true;
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage.Name = "MenuStrip_Settings_Loading_SendLoginCompleteMessage";
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage.Text = "SendLoginCompleteMessage?";
            this.MenuStrip_Settings_Loading_SendLoginCompleteMessage.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_SendLoginCompleteMessage_Click);
            // 
            // MenuStrip_Settings_Loading_AutoKickLoginBots
            // 
            this.MenuStrip_Settings_Loading_AutoKickLoginBots.Checked = true;
            this.MenuStrip_Settings_Loading_AutoKickLoginBots.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Loading_AutoKickLoginBots.Name = "MenuStrip_Settings_Loading_AutoKickLoginBots";
            this.MenuStrip_Settings_Loading_AutoKickLoginBots.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_AutoKickLoginBots.Text = "AutoKickLoginBots?";
            this.MenuStrip_Settings_Loading_AutoKickLoginBots.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_AutoKickLoginBots_Click);
            // 
            // MenuStrip_Settings_Loading_NotifyWhenBotConnects
            // 
            this.MenuStrip_Settings_Loading_NotifyWhenBotConnects.Name = "MenuStrip_Settings_Loading_NotifyWhenBotConnects";
            this.MenuStrip_Settings_Loading_NotifyWhenBotConnects.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_NotifyWhenBotConnects.Text = "NotifyWhenBotConnects?";
            this.MenuStrip_Settings_Loading_NotifyWhenBotConnects.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_NotifyWhenBotConnects_Click);
            // 
            // MenuStrip_Settings_Loading_SendLoadingPercentNotification
            // 
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification.Checked = true;
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification.Name = "MenuStrip_Settings_Loading_SendLoadingPercentNotification";
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification.Text = "SendLoadingPercentNotification?";
            this.MenuStrip_Settings_Loading_SendLoadingPercentNotification.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_SendLoadingPercentNotification_Click);
            // 
            // MenuStrip_Settings_Loading_SendLoginCompleteNotification
            // 
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification.Checked = true;
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification.CheckState = System.Windows.Forms.CheckState.Checked;
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification.Name = "MenuStrip_Settings_Loading_SendLoginCompleteNotification";
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification.Size = new System.Drawing.Size(251, 22);
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification.Text = "SendLoginCompleteNotification?";
            this.MenuStrip_Settings_Loading_SendLoginCompleteNotification.Click += new System.EventHandler(this.MenuStrip_Settings_Loading_SendLoginCompleteNotification_Click);
            // 
            // MenuStrip_Settings_Server
            // 
            this.MenuStrip_Settings_Server.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip_Settings_Server_ConnectionLocked,
            this.MenuStrip_Settings_Server_JoinLocked,
            this.MenuStrip_Settings_Server_Seperator_00,
            this.MenuStrip_Settings_Server_ListenerPort,
            this.MenuStrip_Settings_Server_Seperator_01,
            this.MenuStrip_Settings_Server_RedirectToPort,
            this.MenuStrip_Settings_Server_RedirectToAddress,
            this.MenuStrip_Settings_Server_Seperator_02,
            this.MenuStrip_Settings_Server_RestartTimer});
            this.MenuStrip_Settings_Server.Name = "MenuStrip_Settings_Server";
            this.MenuStrip_Settings_Server.Size = new System.Drawing.Size(163, 22);
            this.MenuStrip_Settings_Server.Text = "Server...";
            // 
            // MenuStrip_Settings_Server_ConnectionLocked
            // 
            this.MenuStrip_Settings_Server_ConnectionLocked.Name = "MenuStrip_Settings_Server_ConnectionLocked";
            this.MenuStrip_Settings_Server_ConnectionLocked.Size = new System.Drawing.Size(179, 22);
            this.MenuStrip_Settings_Server_ConnectionLocked.Text = "ConnectionLocked?";
            this.MenuStrip_Settings_Server_ConnectionLocked.Click += new System.EventHandler(this.MenuStrip_Settings_Server_ConnectionLocked_Click);
            // 
            // MenuStrip_Settings_Server_JoinLocked
            // 
            this.MenuStrip_Settings_Server_JoinLocked.Name = "MenuStrip_Settings_Server_JoinLocked";
            this.MenuStrip_Settings_Server_JoinLocked.Size = new System.Drawing.Size(179, 22);
            this.MenuStrip_Settings_Server_JoinLocked.Text = "JoinLocked?";
            this.MenuStrip_Settings_Server_JoinLocked.Click += new System.EventHandler(this.MenuStrip_Settings_Server_JoinLocked_Click);
            // 
            // MenuStrip_Settings_Server_Seperator_00
            // 
            this.MenuStrip_Settings_Server_Seperator_00.Name = "MenuStrip_Settings_Server_Seperator_00";
            this.MenuStrip_Settings_Server_Seperator_00.Size = new System.Drawing.Size(176, 6);
            // 
            // MenuStrip_Settings_Server_ListenerPort
            // 
            this.MenuStrip_Settings_Server_ListenerPort.Name = "MenuStrip_Settings_Server_ListenerPort";
            this.MenuStrip_Settings_Server_ListenerPort.Size = new System.Drawing.Size(179, 22);
            this.MenuStrip_Settings_Server_ListenerPort.Text = "ListenerPort";
            this.MenuStrip_Settings_Server_ListenerPort.Click += new System.EventHandler(this.MenuStrip_Settings_Server_ListenerPort_Click);
            // 
            // MenuStrip_Settings_Server_Seperator_01
            // 
            this.MenuStrip_Settings_Server_Seperator_01.Name = "MenuStrip_Settings_Server_Seperator_01";
            this.MenuStrip_Settings_Server_Seperator_01.Size = new System.Drawing.Size(176, 6);
            // 
            // MenuStrip_Settings_Server_RedirectToPort
            // 
            this.MenuStrip_Settings_Server_RedirectToPort.Name = "MenuStrip_Settings_Server_RedirectToPort";
            this.MenuStrip_Settings_Server_RedirectToPort.Size = new System.Drawing.Size(179, 22);
            this.MenuStrip_Settings_Server_RedirectToPort.Text = "RedirectToPort";
            this.MenuStrip_Settings_Server_RedirectToPort.Click += new System.EventHandler(this.MenuStrip_Settings_Server_RedirectToPort_Click);
            // 
            // MenuStrip_Settings_Server_RedirectToAddress
            // 
            this.MenuStrip_Settings_Server_RedirectToAddress.Name = "MenuStrip_Settings_Server_RedirectToAddress";
            this.MenuStrip_Settings_Server_RedirectToAddress.Size = new System.Drawing.Size(179, 22);
            this.MenuStrip_Settings_Server_RedirectToAddress.Text = "RedirectToAddress";
            this.MenuStrip_Settings_Server_RedirectToAddress.Click += new System.EventHandler(this.MenuStrip_Settings_Server_RedirectToAddress_Click);
            // 
            // MenuStrip_Settings_Server_Seperator_02
            // 
            this.MenuStrip_Settings_Server_Seperator_02.Name = "MenuStrip_Settings_Server_Seperator_02";
            this.MenuStrip_Settings_Server_Seperator_02.Size = new System.Drawing.Size(176, 6);
            // 
            // MenuStrip_Settings_Server_RestartTimer
            // 
            this.MenuStrip_Settings_Server_RestartTimer.Name = "MenuStrip_Settings_Server_RestartTimer";
            this.MenuStrip_Settings_Server_RestartTimer.Size = new System.Drawing.Size(179, 22);
            this.MenuStrip_Settings_Server_RestartTimer.Text = "RestartTimer";
            this.MenuStrip_Settings_Server_RestartTimer.Click += new System.EventHandler(this.MenuStrip_Settings_Server_RestartTimer_Click);
            // 
            // ClientListPanel
            // 
            this.ClientListPanel.BackColor = System.Drawing.Color.Black;
            this.ClientListPanel.Controls.Add(this.ClientsList);
            this.ClientListPanel.ForeColor = System.Drawing.Color.White;
            this.ClientListPanel.Location = new System.Drawing.Point(1000, 23);
            this.ClientListPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ClientListPanel.Name = "ClientListPanel";
            this.ClientListPanel.Size = new System.Drawing.Size(200, 639);
            this.ClientListPanel.TabIndex = 6;
            // 
            // ClientsList
            // 
            this.ClientsList.BackColor = System.Drawing.Color.Black;
            this.ClientsList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ClientsList.ForeColor = System.Drawing.Color.White;
            this.ClientsList.Location = new System.Drawing.Point(0, 0);
            this.ClientsList.Margin = new System.Windows.Forms.Padding(0);
            this.ClientsList.Name = "ClientsList";
            this.ClientsList.ReadOnly = true;
            this.ClientsList.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.ClientsList.Size = new System.Drawing.Size(200, 639);
            this.ClientsList.TabIndex = 4;
            this.ClientsList.TabStop = false;
            this.ClientsList.Text = "";
            this.ClientsList.Enter += new System.EventHandler(this.ClientsList_Enter);
            // 
            // MainWindow
            // 
            this.AccessibleDescription = "OpenYS Testing Window";
            this.AccessibleName = "OpenYS Window";
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.ClientSize = new System.Drawing.Size(1200, 675);
            this.Controls.Add(this.ClientListPanel);
            this.Controls.Add(this.ConsoleInput);
            this.Controls.Add(this.ConsoleInputPrompt);
            this.Controls.Add(this.ConsoleOutputPanel);
            this.Controls.Add(this.MenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenYS Testing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.FormWindow_Load);
            this.ConsoleOutputPanel.ResumeLayout(false);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ClientListPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel ConsoleOutputPanel;
        private System.Windows.Forms.RichTextBox ConsoleOutput;
        private System.Windows.Forms.Panel ConsoleInputPanel;
        private System.Windows.Forms.Label ConsoleInputPrompt;
        private System.Windows.Forms.TextBox ConsoleInput;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Field;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Restart;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Shutdown;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Debug;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Debug_DumpLog;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather_DayNightCycle;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather_FogColor;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather_GroundColor;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather_SkyColor;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather_Turbulence;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_AdvWeather_UseVariableWeather;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Replay;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Server_Seperator_00;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_JoinLock;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_ConnectionLock;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Server_Seperator_01;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Restart_RestartNow;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Server_Restart_Seperator_00;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Restart_RestartTimer;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Shutdown_ShutdownNow;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Server_Shutdown_Seperator_00;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Shutdown_ShutdownOnNextRestart;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Server_Seperator_02;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_OwnerInfo;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_OwnerInfo_OwnerName;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_OwnerInfo_OwnerEmail;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_OwnerInfo_IsCustomBuild;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_ForceSetting;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_ForceSetting_Blackout;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_ForceSetting_Collisions;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_ForceSetting_LandEverywhere;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_ForceSetting_Fog;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_EnableSetting;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_EnableSetting_Blackout;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_EnableSetting_Collisions;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_EnableSetting_LandEverywhere;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_EnableSetting_Fog;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_SetTime;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_Wind;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Replay_Load;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Replay_Play;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Replay_Stop;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Replay_JumpTo;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_OpenYS;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_OpenYS_ConsoleName;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_OpenYS_BackgroundName;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_EnableMissiles;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_EnableWeapons;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_LoginPassword;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Settings_Seperator_00;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_ReloadSettings;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Version;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_ReviveGrounds;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_EnableSmoke;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_YSFDirectory;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_YSFNetcodeVersion;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_OYSNetcodeVersion;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_FieldName;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_AutoOPLocalAddresses;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_SendLoginWelcomeMessage;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_SendLoginCompleteMessage;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_AutoKickLoginBots;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_NotifyWhenBotConnects;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_SendLoadingPercentNotification;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_SendLoginCompleteNotification;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Loading_AircraftPerAircraftListPacket;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server_ConnectionLocked;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server_JoinLocked;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server_ListenerPort;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server_RedirectToPort;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server_RedirectToAddress;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Server_RestartTimer;
        private System.Windows.Forms.Panel ClientListPanel;
        private System.Windows.Forms.RichTextBox ClientsList;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Settings_Weather_Wind_Seperator_00;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Settings_Loading_Seperator_00;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Settings_Server_Seperator_00;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Settings_Server_Seperator_01;
        private System.Windows.Forms.ToolStripSeparator MenuStrip_Settings_Server_Seperator_02;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Field_CurrentField;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Server_Field_AfterRestart;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_Wind_CurrentInfo;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_Wind_SetWindX;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_Wind_SetWindY;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_Wind_SetWindZ;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip_Settings_Weather_Wind_Change;
    }
}