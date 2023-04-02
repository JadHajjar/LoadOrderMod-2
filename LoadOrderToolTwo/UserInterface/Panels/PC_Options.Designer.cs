﻿using LoadOrderToolTwo.UserInterface.Dropdowns;

namespace LoadOrderToolTwo.UserInterface.Panels;

partial class PC_Options
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

	#region Component Designer generated code

	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PC_Options));
			this.TLP_Main = new System.Windows.Forms.TableLayoutPanel();
			this.TLP_HelpLogs = new SlickControls.RoundedGroupTableLayoutPanel();
			this.slickButton1 = new SlickControls.SlickButton();
			this.TLP_Advanced = new SlickControls.RoundedGroupTableLayoutPanel();
			this.slickCheckbox9 = new SlickControls.SlickCheckbox();
			this.slickCheckbox3 = new SlickControls.SlickCheckbox();
			this.TLP_Settings = new SlickControls.RoundedGroupTableLayoutPanel();
			this.slickCheckbox10 = new SlickControls.SlickCheckbox();
			this.slickCheckbox6 = new SlickControls.SlickCheckbox();
			this.CB_LinkModAssets = new SlickControls.SlickCheckbox();
			this.TLP_Folders = new SlickControls.RoundedGroupTableLayoutPanel();
			this.TB_VirtualAppDataPath = new SlickControls.SlickPathTextBox();
			this.TB_VirtualGamePath = new SlickControls.SlickPathTextBox();
			this.TB_SteamPath = new SlickControls.SlickPathTextBox();
			this.TB_AppDataPath = new SlickControls.SlickPathTextBox();
			this.TB_GamePath = new SlickControls.SlickPathTextBox();
			this.TLP_Preferences = new SlickControls.RoundedGroupTableLayoutPanel();
			this.slickCheckbox8 = new SlickControls.SlickCheckbox();
			this.slickCheckbox7 = new SlickControls.SlickCheckbox();
			this.slickCheckbox5 = new SlickControls.SlickCheckbox();
			this.slickCheckbox2 = new SlickControls.SlickCheckbox();
			this.slickCheckbox1 = new SlickControls.SlickCheckbox();
			this.slickCheckbox4 = new SlickControls.SlickCheckbox();
			this.TLP_UI = new SlickControls.RoundedGroupTableLayoutPanel();
			this.B_HelpTranslate = new SlickControls.SlickButton();
			this.DD_Language = new LoadOrderToolTwo.UserInterface.Dropdowns.LanguageDropDown();
			this.B_Theme = new SlickControls.SlickButton();
			this.slickSpacer1 = new SlickControls.SlickSpacer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.slickScroll1 = new SlickControls.SlickScroll();
			this.TLP_Main.SuspendLayout();
			this.TLP_HelpLogs.SuspendLayout();
			this.TLP_Advanced.SuspendLayout();
			this.TLP_Settings.SuspendLayout();
			this.TLP_Folders.SuspendLayout();
			this.TLP_Preferences.SuspendLayout();
			this.TLP_UI.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// base_Text
			// 
			this.base_Text.Location = new System.Drawing.Point(-2, 3);
			this.base_Text.Size = new System.Drawing.Size(77, 26);
			this.base_Text.Text = "Language";
			// 
			// TLP_Main
			// 
			this.TLP_Main.AutoSize = true;
			this.TLP_Main.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_Main.ColumnCount = 4;
			this.TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.TLP_Main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.TLP_Main.Controls.Add(this.TLP_HelpLogs, 3, 0);
			this.TLP_Main.Controls.Add(this.TLP_Advanced, 2, 1);
			this.TLP_Main.Controls.Add(this.TLP_Settings, 0, 1);
			this.TLP_Main.Controls.Add(this.TLP_Folders, 0, 3);
			this.TLP_Main.Controls.Add(this.TLP_Preferences, 0, 0);
			this.TLP_Main.Controls.Add(this.TLP_UI, 2, 0);
			this.TLP_Main.Location = new System.Drawing.Point(0, 66);
			this.TLP_Main.MaximumSize = new System.Drawing.Size(1100, 0);
			this.TLP_Main.Name = "TLP_Main";
			this.TLP_Main.RowCount = 4;
			this.TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.TLP_Main.Size = new System.Drawing.Size(1100, 749);
			this.TLP_Main.TabIndex = 13;
			// 
			// TLP_HelpLogs
			// 
			this.TLP_HelpLogs.AddOutline = true;
			this.TLP_HelpLogs.AutoSize = true;
			this.TLP_HelpLogs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_HelpLogs.ColumnCount = 1;
			this.TLP_HelpLogs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_HelpLogs.Controls.Add(this.slickButton1, 0, 1);
			this.TLP_HelpLogs.Dock = System.Windows.Forms.DockStyle.Top;
			this.TLP_HelpLogs.Location = new System.Drawing.Point(828, 3);
			this.TLP_HelpLogs.Name = "TLP_HelpLogs";
			this.TLP_HelpLogs.Padding = new System.Windows.Forms.Padding(7, 38, 7, 7);
			this.TLP_HelpLogs.RowCount = 2;
			this.TLP_HelpLogs.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_HelpLogs.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_HelpLogs.Size = new System.Drawing.Size(269, 81);
			this.TLP_HelpLogs.TabIndex = 23;
			this.TLP_HelpLogs.Text = "HelpLogs";
			this.TLP_HelpLogs.Visible = false;
			// 
			// slickButton1
			// 
			this.slickButton1.ColorShade = null;
			this.slickButton1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickButton1.Dock = System.Windows.Forms.DockStyle.Top;
			this.slickButton1.Image = global::LoadOrderToolTwo.Properties.Resources.I_Theme;
			this.slickButton1.Location = new System.Drawing.Point(10, 41);
			this.slickButton1.Name = "slickButton1";
			this.slickButton1.Size = new System.Drawing.Size(249, 30);
			this.slickButton1.SpaceTriggersClick = true;
			this.slickButton1.TabIndex = 15;
			this.slickButton1.Text = "ThemeUIScale";
			// 
			// TLP_Advanced
			// 
			this.TLP_Advanced.AddOutline = true;
			this.TLP_Advanced.AutoSize = true;
			this.TLP_Advanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_Advanced.ColorStyle = Extensions.ColorStyle.Red;
			this.TLP_Advanced.ColumnCount = 1;
			this.TLP_Main.SetColumnSpan(this.TLP_Advanced, 2);
			this.TLP_Advanced.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_Advanced.Controls.Add(this.slickCheckbox9, 0, 1);
			this.TLP_Advanced.Controls.Add(this.slickCheckbox3, 0, 0);
			this.TLP_Advanced.Dock = System.Windows.Forms.DockStyle.Top;
			this.TLP_Advanced.Location = new System.Drawing.Point(553, 294);
			this.TLP_Advanced.Name = "TLP_Advanced";
			this.TLP_Advanced.Padding = new System.Windows.Forms.Padding(7, 38, 7, 7);
			this.TLP_Advanced.RowCount = 2;
			this.TLP_Advanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Advanced.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Advanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.TLP_Advanced.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.TLP_Advanced.Size = new System.Drawing.Size(544, 121);
			this.TLP_Advanced.TabIndex = 22;
			this.TLP_Advanced.Text = "AdvancedSettings";
			// 
			// slickCheckbox9
			// 
			this.slickCheckbox9.AutoSize = true;
			this.slickCheckbox9.Checked = false;
			this.slickCheckbox9.CheckedText = null;
			this.slickCheckbox9.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox9.DefaultValue = false;
			this.slickCheckbox9.EnterTriggersClick = false;
			this.slickCheckbox9.Location = new System.Drawing.Point(10, 79);
			this.slickCheckbox9.Name = "slickCheckbox9";
			this.slickCheckbox9.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox9.Size = new System.Drawing.Size(309, 32);
			this.slickCheckbox9.SpaceTriggersClick = true;
			this.slickCheckbox9.TabIndex = 20;
			this.slickCheckbox9.Tag = "AdvancedLaunchOptions";
			this.slickCheckbox9.Text = "AdvancedLaunchOptions";
			this.slickCheckbox9.UncheckedText = null;
			this.slickCheckbox9.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox3
			// 
			this.slickCheckbox3.AutoSize = true;
			this.slickCheckbox3.Checked = false;
			this.slickCheckbox3.CheckedText = null;
			this.slickCheckbox3.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox3.DefaultValue = false;
			this.slickCheckbox3.EnterTriggersClick = false;
			this.slickCheckbox3.Location = new System.Drawing.Point(10, 41);
			this.slickCheckbox3.Name = "slickCheckbox3";
			this.slickCheckbox3.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox3.Size = new System.Drawing.Size(524, 32);
			this.slickCheckbox3.SpaceTriggersClick = true;
			this.slickCheckbox3.TabIndex = 15;
			this.slickCheckbox3.Tag = "AdvancedIncludeEnable";
			this.slickCheckbox3.Text = "AdvancedIncludeEnable";
			this.slickCheckbox3.UncheckedText = null;
			this.slickCheckbox3.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// TLP_Settings
			// 
			this.TLP_Settings.AddOutline = true;
			this.TLP_Settings.AutoSize = true;
			this.TLP_Settings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_Settings.ColorStyle = Extensions.ColorStyle.Yellow;
			this.TLP_Settings.ColumnCount = 1;
			this.TLP_Main.SetColumnSpan(this.TLP_Settings, 2);
			this.TLP_Settings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_Settings.Controls.Add(this.slickCheckbox10, 0, 2);
			this.TLP_Settings.Controls.Add(this.slickCheckbox6, 0, 1);
			this.TLP_Settings.Controls.Add(this.CB_LinkModAssets, 0, 0);
			this.TLP_Settings.Dock = System.Windows.Forms.DockStyle.Top;
			this.TLP_Settings.Location = new System.Drawing.Point(3, 294);
			this.TLP_Settings.Name = "TLP_Settings";
			this.TLP_Settings.Padding = new System.Windows.Forms.Padding(7, 38, 7, 7);
			this.TLP_Settings.RowCount = 3;
			this.TLP_Settings.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Settings.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Settings.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Settings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.TLP_Settings.Size = new System.Drawing.Size(544, 166);
			this.TLP_Settings.TabIndex = 21;
			this.TLP_Settings.Text = "Settings";
			// 
			// slickCheckbox10
			// 
			this.slickCheckbox10.AutoSize = true;
			this.slickCheckbox10.Checked = false;
			this.slickCheckbox10.CheckedText = null;
			this.slickCheckbox10.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox10.DefaultValue = false;
			this.slickCheckbox10.EnterTriggersClick = false;
			this.slickCheckbox10.Location = new System.Drawing.Point(10, 124);
			this.slickCheckbox10.Name = "slickCheckbox10";
			this.slickCheckbox10.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox10.Size = new System.Drawing.Size(335, 32);
			this.slickCheckbox10.SpaceTriggersClick = true;
			this.slickCheckbox10.TabIndex = 20;
			this.slickCheckbox10.Tag = "HidePseudoMods";
			this.slickCheckbox10.Text = "HidePseudoMods";
			this.slickCheckbox10.UncheckedText = null;
			this.slickCheckbox10.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox6
			// 
			this.slickCheckbox6.AutoSize = true;
			this.slickCheckbox6.Checked = false;
			this.slickCheckbox6.CheckedText = null;
			this.slickCheckbox6.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox6.DefaultValue = false;
			this.slickCheckbox6.EnterTriggersClick = false;
			this.slickCheckbox6.Location = new System.Drawing.Point(10, 86);
			this.slickCheckbox6.Name = "slickCheckbox6";
			this.slickCheckbox6.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox6.Size = new System.Drawing.Size(524, 32);
			this.slickCheckbox6.SpaceTriggersClick = true;
			this.slickCheckbox6.TabIndex = 18;
			this.slickCheckbox6.Tag = "OverrideGameChanges";
			this.slickCheckbox6.Text = "OverrideGameChanges";
			this.slickCheckbox6.UncheckedText = null;
			this.slickCheckbox6.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// CB_LinkModAssets
			// 
			this.CB_LinkModAssets.AutoSize = true;
			this.CB_LinkModAssets.Checked = false;
			this.CB_LinkModAssets.CheckedText = null;
			this.CB_LinkModAssets.Cursor = System.Windows.Forms.Cursors.Hand;
			this.CB_LinkModAssets.DefaultValue = false;
			this.CB_LinkModAssets.EnterTriggersClick = false;
			this.CB_LinkModAssets.Location = new System.Drawing.Point(10, 48);
			this.CB_LinkModAssets.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
			this.CB_LinkModAssets.Name = "CB_LinkModAssets";
			this.CB_LinkModAssets.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.CB_LinkModAssets.Size = new System.Drawing.Size(457, 32);
			this.CB_LinkModAssets.SpaceTriggersClick = true;
			this.CB_LinkModAssets.TabIndex = 0;
			this.CB_LinkModAssets.Tag = "LinkModAssets";
			this.CB_LinkModAssets.Text = "LinkModAssets";
			this.CB_LinkModAssets.UncheckedText = null;
			this.CB_LinkModAssets.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// TLP_Folders
			// 
			this.TLP_Folders.AddOutline = true;
			this.TLP_Folders.AutoSize = true;
			this.TLP_Folders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_Folders.ColumnCount = 1;
			this.TLP_Main.SetColumnSpan(this.TLP_Folders, 4);
			this.TLP_Folders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_Folders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.TLP_Folders.Controls.Add(this.TB_VirtualAppDataPath, 0, 4);
			this.TLP_Folders.Controls.Add(this.TB_VirtualGamePath, 0, 3);
			this.TLP_Folders.Controls.Add(this.TB_SteamPath, 0, 2);
			this.TLP_Folders.Controls.Add(this.TB_AppDataPath, 0, 1);
			this.TLP_Folders.Controls.Add(this.TB_GamePath, 0, 0);
			this.TLP_Folders.Dock = System.Windows.Forms.DockStyle.Top;
			this.TLP_Folders.Image = ((System.Drawing.Image)(resources.GetObject("TLP_Folders.Image")));
			this.TLP_Folders.Location = new System.Drawing.Point(3, 466);
			this.TLP_Folders.Name = "TLP_Folders";
			this.TLP_Folders.Padding = new System.Windows.Forms.Padding(7, 43, 7, 7);
			this.TLP_Folders.RowCount = 5;
			this.TLP_Folders.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Folders.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Folders.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Folders.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Folders.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Folders.Size = new System.Drawing.Size(1094, 280);
			this.TLP_Folders.TabIndex = 19;
			this.TLP_Folders.Text = "FolderSettings";
			// 
			// TB_VirtualAppDataPath
			// 
			this.TB_VirtualAppDataPath.Dock = System.Windows.Forms.DockStyle.Top;
			this.TB_VirtualAppDataPath.FileExtensions = new string[0];
			this.TB_VirtualAppDataPath.Folder = true;
			this.TB_VirtualAppDataPath.Image = ((System.Drawing.Image)(resources.GetObject("TB_VirtualAppDataPath.Image")));
			this.TB_VirtualAppDataPath.LabelText = "VirtualAppDataPath";
			this.TB_VirtualAppDataPath.Location = new System.Drawing.Point(12, 233);
			this.TB_VirtualAppDataPath.Margin = new System.Windows.Forms.Padding(5);
			this.TB_VirtualAppDataPath.MinimumSize = new System.Drawing.Size(50, 35);
			this.TB_VirtualAppDataPath.Name = "TB_VirtualAppDataPath";
			this.TB_VirtualAppDataPath.Placeholder = "/home/user/.local/share/Colossal Order/Cities_Skylines";
			this.TB_VirtualAppDataPath.SelectedText = "";
			this.TB_VirtualAppDataPath.SelectionLength = 0;
			this.TB_VirtualAppDataPath.SelectionStart = 0;
			this.TB_VirtualAppDataPath.Size = new System.Drawing.Size(1070, 35);
			this.TB_VirtualAppDataPath.TabIndex = 4;
			this.TB_VirtualAppDataPath.TextChanged += new System.EventHandler(this.TB_FolderPath_TextChanged);
			// 
			// TB_VirtualGamePath
			// 
			this.TB_VirtualGamePath.Dock = System.Windows.Forms.DockStyle.Top;
			this.TB_VirtualGamePath.FileExtensions = new string[0];
			this.TB_VirtualGamePath.Folder = true;
			this.TB_VirtualGamePath.Image = ((System.Drawing.Image)(resources.GetObject("TB_VirtualGamePath.Image")));
			this.TB_VirtualGamePath.LabelText = "VirtualGamePath";
			this.TB_VirtualGamePath.Location = new System.Drawing.Point(12, 188);
			this.TB_VirtualGamePath.Margin = new System.Windows.Forms.Padding(5);
			this.TB_VirtualGamePath.MinimumSize = new System.Drawing.Size(50, 35);
			this.TB_VirtualGamePath.Name = "TB_VirtualGamePath";
			this.TB_VirtualGamePath.Placeholder = "/home/user/.steam/steam/steamapps/common/Cities_Skylines";
			this.TB_VirtualGamePath.SelectedText = "";
			this.TB_VirtualGamePath.SelectionLength = 0;
			this.TB_VirtualGamePath.SelectionStart = 0;
			this.TB_VirtualGamePath.Size = new System.Drawing.Size(1070, 35);
			this.TB_VirtualGamePath.TabIndex = 3;
			this.TB_VirtualGamePath.TextChanged += new System.EventHandler(this.TB_FolderPath_TextChanged);
			// 
			// TB_SteamPath
			// 
			this.TB_SteamPath.Dock = System.Windows.Forms.DockStyle.Top;
			this.TB_SteamPath.FileExtensions = new string[0];
			this.TB_SteamPath.Folder = true;
			this.TB_SteamPath.Image = ((System.Drawing.Image)(resources.GetObject("TB_SteamPath.Image")));
			this.TB_SteamPath.LabelText = "SteamPath";
			this.TB_SteamPath.Location = new System.Drawing.Point(12, 143);
			this.TB_SteamPath.Margin = new System.Windows.Forms.Padding(5);
			this.TB_SteamPath.MinimumSize = new System.Drawing.Size(50, 35);
			this.TB_SteamPath.Name = "TB_SteamPath";
			this.TB_SteamPath.Placeholder = "C:\\Program Files (x86)\\Steam";
			this.TB_SteamPath.SelectedText = "";
			this.TB_SteamPath.SelectionLength = 0;
			this.TB_SteamPath.SelectionStart = 0;
			this.TB_SteamPath.Size = new System.Drawing.Size(1070, 35);
			this.TB_SteamPath.TabIndex = 2;
			this.TB_SteamPath.TextChanged += new System.EventHandler(this.TB_FolderPath_TextChanged);
			// 
			// TB_AppDataPath
			// 
			this.TB_AppDataPath.Dock = System.Windows.Forms.DockStyle.Top;
			this.TB_AppDataPath.FileExtensions = new string[0];
			this.TB_AppDataPath.Folder = true;
			this.TB_AppDataPath.Image = ((System.Drawing.Image)(resources.GetObject("TB_AppDataPath.Image")));
			this.TB_AppDataPath.LabelText = "AppDataPath";
			this.TB_AppDataPath.Location = new System.Drawing.Point(12, 98);
			this.TB_AppDataPath.Margin = new System.Windows.Forms.Padding(5);
			this.TB_AppDataPath.MinimumSize = new System.Drawing.Size(50, 35);
			this.TB_AppDataPath.Name = "TB_AppDataPath";
			this.TB_AppDataPath.Placeholder = "%LocalAppData%\\Colossal Order\\Cities_Skylines";
			this.TB_AppDataPath.SelectedText = "";
			this.TB_AppDataPath.SelectionLength = 0;
			this.TB_AppDataPath.SelectionStart = 0;
			this.TB_AppDataPath.Size = new System.Drawing.Size(1070, 35);
			this.TB_AppDataPath.TabIndex = 1;
			this.TB_AppDataPath.TextChanged += new System.EventHandler(this.TB_FolderPath_TextChanged);
			// 
			// TB_GamePath
			// 
			this.TB_GamePath.Dock = System.Windows.Forms.DockStyle.Top;
			this.TB_GamePath.FileExtensions = new string[0];
			this.TB_GamePath.Folder = true;
			this.TB_GamePath.Image = ((System.Drawing.Image)(resources.GetObject("TB_GamePath.Image")));
			this.TB_GamePath.LabelText = "GamePath";
			this.TB_GamePath.Location = new System.Drawing.Point(12, 53);
			this.TB_GamePath.Margin = new System.Windows.Forms.Padding(5, 10, 5, 5);
			this.TB_GamePath.MinimumSize = new System.Drawing.Size(50, 35);
			this.TB_GamePath.Name = "TB_GamePath";
			this.TB_GamePath.Placeholder = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Cities_Skylines";
			this.TB_GamePath.SelectedText = "";
			this.TB_GamePath.SelectionLength = 0;
			this.TB_GamePath.SelectionStart = 0;
			this.TB_GamePath.Size = new System.Drawing.Size(1070, 35);
			this.TB_GamePath.TabIndex = 0;
			this.TB_GamePath.TextChanged += new System.EventHandler(this.TB_FolderPath_TextChanged);
			// 
			// TLP_Preferences
			// 
			this.TLP_Preferences.AddOutline = true;
			this.TLP_Preferences.AutoSize = true;
			this.TLP_Preferences.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_Preferences.ColumnCount = 2;
			this.TLP_Main.SetColumnSpan(this.TLP_Preferences, 2);
			this.TLP_Preferences.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_Preferences.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.TLP_Preferences.Controls.Add(this.slickCheckbox8, 0, 8);
			this.TLP_Preferences.Controls.Add(this.slickCheckbox7, 0, 7);
			this.TLP_Preferences.Controls.Add(this.slickCheckbox5, 0, 4);
			this.TLP_Preferences.Controls.Add(this.slickCheckbox2, 0, 2);
			this.TLP_Preferences.Controls.Add(this.slickCheckbox1, 0, 1);
			this.TLP_Preferences.Controls.Add(this.slickCheckbox4, 0, 5);
			this.TLP_Preferences.Dock = System.Windows.Forms.DockStyle.Top;
			this.TLP_Preferences.Image = ((System.Drawing.Image)(resources.GetObject("TLP_Preferences.Image")));
			this.TLP_Preferences.Location = new System.Drawing.Point(3, 3);
			this.TLP_Preferences.Name = "TLP_Preferences";
			this.TLP_Preferences.Padding = new System.Windows.Forms.Padding(7, 43, 7, 7);
			this.TLP_Preferences.RowCount = 10;
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_Preferences.Size = new System.Drawing.Size(544, 285);
			this.TLP_Preferences.TabIndex = 18;
			this.TLP_Preferences.Text = "Preferences";
			// 
			// slickCheckbox8
			// 
			this.slickCheckbox8.AutoSize = true;
			this.slickCheckbox8.Checked = false;
			this.slickCheckbox8.CheckedText = null;
			this.slickCheckbox8.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox8.DefaultValue = false;
			this.slickCheckbox8.EnterTriggersClick = false;
			this.slickCheckbox8.Location = new System.Drawing.Point(10, 243);
			this.slickCheckbox8.Name = "slickCheckbox8";
			this.slickCheckbox8.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox8.Size = new System.Drawing.Size(306, 32);
			this.slickCheckbox8.SpaceTriggersClick = true;
			this.slickCheckbox8.TabIndex = 20;
			this.slickCheckbox8.Tag = "FilterOutPackagesWithMods";
			this.slickCheckbox8.Text = "FilterOutPackagesWithMods";
			this.slickCheckbox8.UncheckedText = null;
			this.slickCheckbox8.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox7
			// 
			this.slickCheckbox7.AutoSize = true;
			this.slickCheckbox7.Checked = false;
			this.slickCheckbox7.CheckedText = null;
			this.slickCheckbox7.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox7.DefaultValue = false;
			this.slickCheckbox7.EnterTriggersClick = false;
			this.slickCheckbox7.Location = new System.Drawing.Point(10, 205);
			this.slickCheckbox7.Name = "slickCheckbox7";
			this.slickCheckbox7.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox7.Size = new System.Drawing.Size(453, 32);
			this.slickCheckbox7.SpaceTriggersClick = true;
			this.slickCheckbox7.TabIndex = 19;
			this.slickCheckbox7.Tag = "FilterOutPackagesWithOneAsset";
			this.slickCheckbox7.Text = "FilterOutPackagesWithOneAsset";
			this.slickCheckbox7.UncheckedText = null;
			this.slickCheckbox7.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox5
			// 
			this.slickCheckbox5.AutoSize = true;
			this.slickCheckbox5.Checked = false;
			this.slickCheckbox5.CheckedText = null;
			this.slickCheckbox5.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox5.DefaultValue = false;
			this.slickCheckbox5.EnterTriggersClick = false;
			this.slickCheckbox5.Location = new System.Drawing.Point(10, 129);
			this.slickCheckbox5.Name = "slickCheckbox5";
			this.slickCheckbox5.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox5.Size = new System.Drawing.Size(377, 32);
			this.slickCheckbox5.SpaceTriggersClick = true;
			this.slickCheckbox5.TabIndex = 17;
			this.slickCheckbox5.Tag = "DisableNewModsByDefault";
			this.slickCheckbox5.Text = "DisableNewModsByDefault";
			this.slickCheckbox5.UncheckedText = null;
			this.slickCheckbox5.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox2
			// 
			this.slickCheckbox2.AutoSize = true;
			this.slickCheckbox2.Checked = false;
			this.slickCheckbox2.CheckedText = null;
			this.slickCheckbox2.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox2.DefaultValue = false;
			this.slickCheckbox2.EnterTriggersClick = false;
			this.slickCheckbox2.Location = new System.Drawing.Point(10, 91);
			this.slickCheckbox2.Name = "slickCheckbox2";
			this.slickCheckbox2.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox2.Size = new System.Drawing.Size(365, 32);
			this.slickCheckbox2.SpaceTriggersClick = true;
			this.slickCheckbox2.TabIndex = 2;
			this.slickCheckbox2.Tag = "ShowDatesRelatively";
			this.slickCheckbox2.Text = "ShowDatesRelatively";
			this.slickCheckbox2.UncheckedText = null;
			this.slickCheckbox2.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox1
			// 
			this.slickCheckbox1.AutoSize = true;
			this.slickCheckbox1.Checked = false;
			this.slickCheckbox1.CheckedText = null;
			this.slickCheckbox1.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox1.DefaultValue = false;
			this.slickCheckbox1.EnterTriggersClick = false;
			this.slickCheckbox1.Location = new System.Drawing.Point(10, 53);
			this.slickCheckbox1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
			this.slickCheckbox1.Name = "slickCheckbox1";
			this.slickCheckbox1.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox1.Size = new System.Drawing.Size(301, 32);
			this.slickCheckbox1.SpaceTriggersClick = true;
			this.slickCheckbox1.TabIndex = 1;
			this.slickCheckbox1.Tag = "LargeItemOnHover";
			this.slickCheckbox1.Text = "IncreaseItemSizeOnHover";
			this.slickCheckbox1.UncheckedText = null;
			this.slickCheckbox1.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// slickCheckbox4
			// 
			this.slickCheckbox4.AutoSize = true;
			this.slickCheckbox4.Checked = false;
			this.slickCheckbox4.CheckedText = null;
			this.slickCheckbox4.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slickCheckbox4.DefaultValue = false;
			this.slickCheckbox4.EnterTriggersClick = false;
			this.slickCheckbox4.Location = new System.Drawing.Point(10, 167);
			this.slickCheckbox4.Name = "slickCheckbox4";
			this.slickCheckbox4.Padding = new System.Windows.Forms.Padding(7, 5, 7, 5);
			this.slickCheckbox4.Size = new System.Drawing.Size(380, 32);
			this.slickCheckbox4.SpaceTriggersClick = true;
			this.slickCheckbox4.TabIndex = 16;
			this.slickCheckbox4.Tag = "DisableNewAssetsByDefault";
			this.slickCheckbox4.Text = "DisableNewAssetsByDefault";
			this.slickCheckbox4.UncheckedText = null;
			this.slickCheckbox4.CheckChanged += new System.EventHandler(this.CB_CheckChanged);
			// 
			// TLP_UI
			// 
			this.TLP_UI.AddOutline = true;
			this.TLP_UI.AutoSize = true;
			this.TLP_UI.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.TLP_UI.ColumnCount = 1;
			this.TLP_UI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.TLP_UI.Controls.Add(this.B_HelpTranslate, 0, 1);
			this.TLP_UI.Controls.Add(this.DD_Language, 0, 0);
			this.TLP_UI.Controls.Add(this.B_Theme, 0, 3);
			this.TLP_UI.Controls.Add(this.slickSpacer1, 0, 2);
			this.TLP_UI.Dock = System.Windows.Forms.DockStyle.Top;
			this.TLP_UI.Location = new System.Drawing.Point(553, 3);
			this.TLP_UI.Name = "TLP_UI";
			this.TLP_UI.Padding = new System.Windows.Forms.Padding(7, 38, 7, 7);
			this.TLP_UI.RowCount = 4;
			this.TLP_UI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_UI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_UI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_UI.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.TLP_UI.Size = new System.Drawing.Size(269, 220);
			this.TLP_UI.TabIndex = 20;
			this.TLP_UI.Text = "UserInterface";
			// 
			// B_HelpTranslate
			// 
			this.B_HelpTranslate.ColorShade = null;
			this.B_HelpTranslate.Cursor = System.Windows.Forms.Cursors.Hand;
			this.B_HelpTranslate.Dock = System.Windows.Forms.DockStyle.Top;
			this.B_HelpTranslate.Image = global::LoadOrderToolTwo.Properties.Resources.I_Translate;
			this.B_HelpTranslate.Location = new System.Drawing.Point(10, 115);
			this.B_HelpTranslate.Name = "B_HelpTranslate";
			this.B_HelpTranslate.Size = new System.Drawing.Size(249, 30);
			this.B_HelpTranslate.SpaceTriggersClick = true;
			this.B_HelpTranslate.TabIndex = 16;
			this.B_HelpTranslate.Text = "HelpTranslate";
			this.B_HelpTranslate.Click += new System.EventHandler(this.B_HelpTranslate_Click);
			// 
			// DD_Language
			// 
			this.DD_Language.Cursor = System.Windows.Forms.Cursors.Hand;
			this.DD_Language.Dock = System.Windows.Forms.DockStyle.Top;
			this.DD_Language.Font = new System.Drawing.Font("Segoe UI", 15F);
			this.DD_Language.Location = new System.Drawing.Point(14, 45);
			this.DD_Language.Margin = new System.Windows.Forms.Padding(7);
			this.DD_Language.Name = "DD_Language";
			this.DD_Language.Padding = new System.Windows.Forms.Padding(7);
			this.DD_Language.Size = new System.Drawing.Size(241, 60);
			this.DD_Language.TabIndex = 14;
			this.DD_Language.Text = "Language";
			// 
			// B_Theme
			// 
			this.B_Theme.ColorShade = null;
			this.B_Theme.Cursor = System.Windows.Forms.Cursors.Hand;
			this.B_Theme.Dock = System.Windows.Forms.DockStyle.Top;
			this.B_Theme.Image = global::LoadOrderToolTwo.Properties.Resources.I_Theme;
			this.B_Theme.Location = new System.Drawing.Point(10, 180);
			this.B_Theme.Name = "B_Theme";
			this.B_Theme.Size = new System.Drawing.Size(249, 30);
			this.B_Theme.SpaceTriggersClick = true;
			this.B_Theme.TabIndex = 15;
			this.B_Theme.Text = "ThemeUIScale";
			this.B_Theme.Click += new System.EventHandler(this.B_Theme_Click);
			// 
			// slickSpacer1
			// 
			this.slickSpacer1.Dock = System.Windows.Forms.DockStyle.Top;
			this.slickSpacer1.Location = new System.Drawing.Point(10, 151);
			this.slickSpacer1.Name = "slickSpacer1";
			this.slickSpacer1.Size = new System.Drawing.Size(249, 23);
			this.slickSpacer1.TabIndex = 17;
			this.slickSpacer1.TabStop = false;
			this.slickSpacer1.Text = "slickSpacer1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.TLP_Main);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 30);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1106, 813);
			this.panel1.TabIndex = 14;
			// 
			// slickScroll1
			// 
			this.slickScroll1.Dock = System.Windows.Forms.DockStyle.Right;
			this.slickScroll1.LinkedControl = this.TLP_Main;
			this.slickScroll1.Location = new System.Drawing.Point(1098, 30);
			this.slickScroll1.Name = "slickScroll1";
			this.slickScroll1.Size = new System.Drawing.Size(8, 813);
			this.slickScroll1.Style = SlickControls.StyleType.Vertical;
			this.slickScroll1.TabIndex = 15;
			this.slickScroll1.TabStop = false;
			this.slickScroll1.Text = "slickScroll1";
			// 
			// PC_Options
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.slickScroll1);
			this.Controls.Add(this.panel1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(58)))), ((int)(((byte)(69)))));
			this.LabelBounds = new System.Drawing.Point(-2, 3);
			this.Name = "PC_Options";
			this.Padding = new System.Windows.Forms.Padding(0, 30, 0, 0);
			this.Size = new System.Drawing.Size(1106, 843);
			this.Text = "Language";
			this.Controls.SetChildIndex(this.base_Text, 0);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.slickScroll1, 0);
			this.TLP_Main.ResumeLayout(false);
			this.TLP_Main.PerformLayout();
			this.TLP_HelpLogs.ResumeLayout(false);
			this.TLP_Advanced.ResumeLayout(false);
			this.TLP_Advanced.PerformLayout();
			this.TLP_Settings.ResumeLayout(false);
			this.TLP_Settings.PerformLayout();
			this.TLP_Folders.ResumeLayout(false);
			this.TLP_Preferences.ResumeLayout(false);
			this.TLP_Preferences.PerformLayout();
			this.TLP_UI.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.TableLayoutPanel TLP_Main;
	private SlickControls.RoundedGroupTableLayoutPanel TLP_Preferences;
	private SlickControls.SlickCheckbox CB_LinkModAssets;
	private SlickControls.RoundedGroupTableLayoutPanel TLP_Folders;
	private SlickControls.SlickPathTextBox TB_VirtualAppDataPath;
	private SlickControls.SlickPathTextBox TB_VirtualGamePath;
	private SlickControls.SlickPathTextBox TB_SteamPath;
	private SlickControls.SlickPathTextBox TB_AppDataPath;
	private SlickControls.SlickPathTextBox TB_GamePath;
	private SlickControls.SlickCheckbox slickCheckbox1;
	private SlickControls.SlickCheckbox slickCheckbox2;
	private LanguageDropDown DD_Language;
	private SlickControls.SlickCheckbox slickCheckbox3;
	private SlickControls.SlickCheckbox slickCheckbox5;
	private SlickControls.SlickCheckbox slickCheckbox4;
	private SlickControls.SlickCheckbox slickCheckbox6;
	private SlickControls.SlickCheckbox slickCheckbox7;
	private SlickControls.SlickCheckbox slickCheckbox8;
	private SlickControls.SlickCheckbox slickCheckbox9;
	private SlickControls.SlickCheckbox slickCheckbox10;
	private System.Windows.Forms.Panel panel1;
	private SlickControls.SlickScroll slickScroll1;
	private SlickControls.RoundedGroupTableLayoutPanel TLP_UI;
	private SlickControls.SlickButton B_Theme;
	private SlickControls.RoundedGroupTableLayoutPanel TLP_Advanced;
	private SlickControls.RoundedGroupTableLayoutPanel TLP_Settings;
	private SlickControls.RoundedGroupTableLayoutPanel TLP_HelpLogs;
	private SlickControls.SlickButton slickButton1;
	private SlickControls.SlickButton B_HelpTranslate;
	private SlickControls.SlickSpacer slickSpacer1;
}
