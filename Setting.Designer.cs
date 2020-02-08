// Copyright 2010-2020 Nakagawa Tomoya
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace digital_photo_frame_screen_saver
{
	partial class Setting
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.cbRotMode = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.cbViewMode = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.cbMultiDisp = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.cbTargetDisplay = new System.Windows.Forms.ComboBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnBrowseFolder = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.textRotInterval = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.colorBackGroundDialog = new System.Windows.Forms.ColorDialog();
			this.label9 = new System.Windows.Forms.Label();
			this.btnColorChoose = new System.Windows.Forms.Button();
			this.cbExifMode = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.label11 = new System.Windows.Forms.Label();
			this.btnExifFontChoose = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.cbClockMode = new System.Windows.Forms.ComboBox();
			this.label16 = new System.Windows.Forms.Label();
			this.labelClockColor = new System.Windows.Forms.Label();
			this.btnClockColorChoose = new System.Windows.Forms.Button();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.labelClockSize = new System.Windows.Forms.Label();
			this.labelClockVerticalPosition = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.trackClockVerticalPosition = new System.Windows.Forms.TrackBar();
			this.trackClockSize = new System.Windows.Forms.TrackBar();
			this.checkClockColorAutomatic = new System.Windows.Forms.CheckBox();
			this.checkRemoveSideBorder = new System.Windows.Forms.CheckBox();
			this.textExifFont = new System.Windows.Forms.TextBox();
			this.picClockColor = new System.Windows.Forms.PictureBox();
			this.picBackgroundColor = new System.Windows.Forms.PictureBox();
			this.checkResizeHighQuality = new System.Windows.Forms.CheckBox();
			this.checkSearchSubDirectory = new System.Windows.Forms.CheckBox();
			this.textFileNameFilter = new System.Windows.Forms.TextBox();
			this.textPicturePath = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.trackClockVerticalPosition)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackClockSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picClockColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picBackgroundColor)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "&Directory to Show:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.ShowNewFolderButton = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 120);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 24);
			this.label2.TabIndex = 7;
			this.label2.Text = "&Rotation Mode:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbRotMode
			// 
			this.cbRotMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbRotMode.FormattingEnabled = true;
			this.cbRotMode.Items.AddRange(new object[] {
            "Rotate by name order",
            "Rotate by date",
            "Random",
            "Shuffle"});
			this.cbRotMode.Location = new System.Drawing.Point(152, 120);
			this.cbRotMode.Margin = new System.Windows.Forms.Padding(4);
			this.cbRotMode.Name = "cbRotMode";
			this.cbRotMode.Size = new System.Drawing.Size(456, 23);
			this.cbRotMode.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 184);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 24);
			this.label3.TabIndex = 13;
			this.label3.Text = "&View Mode:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbViewMode
			// 
			this.cbViewMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbViewMode.FormattingEnabled = true;
			this.cbViewMode.Items.AddRange(new object[] {
            "Center",
            "Tile",
            "Fit to screen",
            "Fit to screen keeping image\'s aspect ratio",
            "Crop"});
			this.cbViewMode.Location = new System.Drawing.Point(152, 184);
			this.cbViewMode.Margin = new System.Windows.Forms.Padding(4);
			this.cbViewMode.Name = "cbViewMode";
			this.cbViewMode.Size = new System.Drawing.Size(456, 23);
			this.cbViewMode.TabIndex = 14;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 328);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 24);
			this.label4.TabIndex = 20;
			this.label4.Text = "&Multiple Display:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbMultiDisp
			// 
			this.cbMultiDisp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMultiDisp.FormattingEnabled = true;
			this.cbMultiDisp.Items.AddRange(new object[] {
            "Single",
            "Clone",
            "Span"});
			this.cbMultiDisp.Location = new System.Drawing.Point(152, 328);
			this.cbMultiDisp.Margin = new System.Windows.Forms.Padding(4);
			this.cbMultiDisp.Name = "cbMultiDisp";
			this.cbMultiDisp.Size = new System.Drawing.Size(456, 23);
			this.cbMultiDisp.TabIndex = 21;
			this.cbMultiDisp.SelectedIndexChanged += new System.EventHandler(this.cbMultiDisp_SelectedIndexChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 360);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(128, 24);
			this.label5.TabIndex = 22;
			this.label5.Text = "&Target Display:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cbTargetDisplay
			// 
			this.cbTargetDisplay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTargetDisplay.FormattingEnabled = true;
			this.cbTargetDisplay.Items.AddRange(new object[] {
            "(example)",
            "Screen1 - RDT241WEX (1920x1200)",
            "Screen2 - Generic PnP Display (1280x1024)"});
			this.cbTargetDisplay.Location = new System.Drawing.Point(152, 360);
			this.cbTargetDisplay.Margin = new System.Windows.Forms.Padding(4);
			this.cbTargetDisplay.Name = "cbTargetDisplay";
			this.cbTargetDisplay.Size = new System.Drawing.Size(456, 23);
			this.cbTargetDisplay.TabIndex = 23;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(336, 696);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(128, 32);
			this.btnOK.TabIndex = 43;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(480, 696);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(128, 32);
			this.btnCancel.TabIndex = 44;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnBrowseFolder
			// 
			this.btnBrowseFolder.Location = new System.Drawing.Point(569, 8);
			this.btnBrowseFolder.Margin = new System.Windows.Forms.Padding(4);
			this.btnBrowseFolder.Name = "btnBrowseFolder";
			this.btnBrowseFolder.Size = new System.Drawing.Size(43, 24);
			this.btnBrowseFolder.TabIndex = 2;
			this.btnBrowseFolder.Text = "...";
			this.btnBrowseFolder.UseVisualStyleBackColor = true;
			this.btnBrowseFolder.Click += new System.EventHandler(this.btnBrowseFolder_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 152);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(128, 24);
			this.label6.TabIndex = 9;
			this.label6.Text = "Rotation &Interval:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRotInterval
			// 
			this.textRotInterval.Location = new System.Drawing.Point(152, 152);
			this.textRotInterval.Margin = new System.Windows.Forms.Padding(4);
			this.textRotInterval.Name = "textRotInterval";
			this.textRotInterval.Size = new System.Drawing.Size(405, 22);
			this.textRotInterval.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(576, 152);
			this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(32, 24);
			this.label7.TabIndex = 12;
			this.label7.Text = "ms";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(16, 40);
			this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(128, 24);
			this.label8.TabIndex = 3;
			this.label8.Text = "File Name &Filter:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 248);
			this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(128, 24);
			this.label9.TabIndex = 16;
			this.label9.Text = "Background Color:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnColorChoose
			// 
			this.btnColorChoose.Location = new System.Drawing.Point(480, 248);
			this.btnColorChoose.Margin = new System.Windows.Forms.Padding(4);
			this.btnColorChoose.Name = "btnColorChoose";
			this.btnColorChoose.Size = new System.Drawing.Size(128, 24);
			this.btnColorChoose.TabIndex = 17;
			this.btnColorChoose.Text = "&Choose...";
			this.btnColorChoose.UseVisualStyleBackColor = true;
			this.btnColorChoose.Click += new System.EventHandler(this.btnColorChoose_Click);
			// 
			// cbExifMode
			// 
			this.cbExifMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbExifMode.FormattingEnabled = true;
			this.cbExifMode.Location = new System.Drawing.Point(152, 416);
			this.cbExifMode.Margin = new System.Windows.Forms.Padding(4);
			this.cbExifMode.Name = "cbExifMode";
			this.cbExifMode.Size = new System.Drawing.Size(456, 23);
			this.cbExifMode.TabIndex = 26;
			this.cbExifMode.SelectedIndexChanged += new System.EventHandler(this.cbExifMode_SelectedIndexChanged);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(16, 416);
			this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(128, 24);
			this.label10.TabIndex = 25;
			this.label10.Text = "&Exif Information:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// fontDialog
			// 
			this.fontDialog.AllowVerticalFonts = false;
			this.fontDialog.FontMustExist = true;
			this.fontDialog.ShowColor = true;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(16, 448);
			this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(128, 24);
			this.label11.TabIndex = 27;
			this.label11.Text = "Font Face:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnExifFontChoose
			// 
			this.btnExifFontChoose.Location = new System.Drawing.Point(480, 448);
			this.btnExifFontChoose.Margin = new System.Windows.Forms.Padding(4);
			this.btnExifFontChoose.Name = "btnExifFontChoose";
			this.btnExifFontChoose.Size = new System.Drawing.Size(128, 24);
			this.btnExifFontChoose.TabIndex = 29;
			this.btnExifFontChoose.Text = "C&hoose...";
			this.btnExifFontChoose.UseVisualStyleBackColor = true;
			this.btnExifFontChoose.Click += new System.EventHandler(this.btnExifFontChoose_Click);
			// 
			// label12
			// 
			this.label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label12.Location = new System.Drawing.Point(16, 104);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(584, 2);
			this.label12.TabIndex = 6;
			// 
			// label13
			// 
			this.label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label13.Location = new System.Drawing.Point(19, 312);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(584, 2);
			this.label13.TabIndex = 19;
			// 
			// label14
			// 
			this.label14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label14.Location = new System.Drawing.Point(19, 400);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(584, 2);
			this.label14.TabIndex = 24;
			// 
			// label15
			// 
			this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label15.Location = new System.Drawing.Point(19, 488);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(584, 2);
			this.label15.TabIndex = 30;
			// 
			// cbClockMode
			// 
			this.cbClockMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbClockMode.FormattingEnabled = true;
			this.cbClockMode.Location = new System.Drawing.Point(152, 504);
			this.cbClockMode.Margin = new System.Windows.Forms.Padding(4);
			this.cbClockMode.Name = "cbClockMode";
			this.cbClockMode.Size = new System.Drawing.Size(456, 23);
			this.cbClockMode.TabIndex = 32;
			this.cbClockMode.SelectedIndexChanged += new System.EventHandler(this.cbExifMode_SelectedIndexChanged);
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(16, 504);
			this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(128, 24);
			this.label16.TabIndex = 31;
			this.label16.Text = "Digital C&lock:";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClockColor
			// 
			this.labelClockColor.Location = new System.Drawing.Point(16, 640);
			this.labelClockColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelClockColor.Name = "labelClockColor";
			this.labelClockColor.Size = new System.Drawing.Size(128, 24);
			this.labelClockColor.TabIndex = 39;
			this.labelClockColor.Text = "Color:";
			this.labelClockColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnClockColorChoose
			// 
			this.btnClockColorChoose.Location = new System.Drawing.Point(336, 640);
			this.btnClockColorChoose.Margin = new System.Windows.Forms.Padding(4);
			this.btnClockColorChoose.Name = "btnClockColorChoose";
			this.btnClockColorChoose.Size = new System.Drawing.Size(128, 24);
			this.btnClockColorChoose.TabIndex = 40;
			this.btnClockColorChoose.Text = "Choose (&K)...";
			this.btnClockColorChoose.UseVisualStyleBackColor = true;
			this.btnClockColorChoose.Click += new System.EventHandler(this.btnClockColorChoose_Click);
			// 
			// label18
			// 
			this.label18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label18.Location = new System.Drawing.Point(19, 680);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(584, 2);
			this.label18.TabIndex = 42;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(16, 544);
			this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(128, 32);
			this.label19.TabIndex = 33;
			this.label19.Text = "Si&ze:";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClockSize
			// 
			this.labelClockSize.Location = new System.Drawing.Point(560, 536);
			this.labelClockSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelClockSize.Name = "labelClockSize";
			this.labelClockSize.Size = new System.Drawing.Size(48, 48);
			this.labelClockSize.TabIndex = 35;
			this.labelClockSize.Text = "60%";
			this.labelClockSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClockVerticalPosition
			// 
			this.labelClockVerticalPosition.Location = new System.Drawing.Point(560, 584);
			this.labelClockVerticalPosition.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelClockVerticalPosition.Name = "labelClockVerticalPosition";
			this.labelClockVerticalPosition.Size = new System.Drawing.Size(48, 48);
			this.labelClockVerticalPosition.TabIndex = 38;
			this.labelClockVerticalPosition.Text = "50%";
			this.labelClockVerticalPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(16, 592);
			this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(128, 32);
			this.label20.TabIndex = 36;
			this.label20.Text = "&Vertical Position:";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// trackClockVerticalPosition
			// 
			this.trackClockVerticalPosition.AutoSize = false;
			this.trackClockVerticalPosition.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ClockVerticalPosition", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackClockVerticalPosition.Location = new System.Drawing.Point(152, 584);
			this.trackClockVerticalPosition.Maximum = 100;
			this.trackClockVerticalPosition.Name = "trackClockVerticalPosition";
			this.trackClockVerticalPosition.Size = new System.Drawing.Size(400, 48);
			this.trackClockVerticalPosition.SmallChange = 5;
			this.trackClockVerticalPosition.TabIndex = 37;
			this.trackClockVerticalPosition.TickFrequency = 10;
			this.trackClockVerticalPosition.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackClockVerticalPosition.Value = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ClockVerticalPosition;
			this.trackClockVerticalPosition.Scroll += new System.EventHandler(this.trackClockVerticalPosition_Scroll);
			// 
			// trackClockSize
			// 
			this.trackClockSize.AutoSize = false;
			this.trackClockSize.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ClockSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.trackClockSize.Location = new System.Drawing.Point(152, 536);
			this.trackClockSize.Maximum = 100;
			this.trackClockSize.Name = "trackClockSize";
			this.trackClockSize.Size = new System.Drawing.Size(400, 48);
			this.trackClockSize.SmallChange = 5;
			this.trackClockSize.TabIndex = 34;
			this.trackClockSize.TickFrequency = 10;
			this.trackClockSize.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackClockSize.Value = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ClockSize;
			this.trackClockSize.Scroll += new System.EventHandler(this.trackClockSize_Scroll);
			// 
			// checkClockColorAutomatic
			// 
			this.checkClockColorAutomatic.Checked = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ClockColorAutomatic;
			this.checkClockColorAutomatic.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ClockColorAutomatic", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkClockColorAutomatic.Location = new System.Drawing.Point(480, 640);
			this.checkClockColorAutomatic.Name = "checkClockColorAutomatic";
			this.checkClockColorAutomatic.Size = new System.Drawing.Size(128, 24);
			this.checkClockColorAutomatic.TabIndex = 41;
			this.checkClockColorAutomatic.Text = "&Automatic";
			this.checkClockColorAutomatic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.checkClockColorAutomatic.UseVisualStyleBackColor = true;
			this.checkClockColorAutomatic.CheckedChanged += new System.EventHandler(this.checkClockColorAutomatic_CheckedChanged);
			// 
			// checkRemoveSideBorder
			// 
			this.checkRemoveSideBorder.Checked = global::digital_photo_frame_screen_saver.Properties.Settings.Default.RemoveSideBorder;
			this.checkRemoveSideBorder.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "RemoveSideBorder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkRemoveSideBorder.Location = new System.Drawing.Point(152, 280);
			this.checkRemoveSideBorder.Name = "checkRemoveSideBorder";
			this.checkRemoveSideBorder.Size = new System.Drawing.Size(456, 24);
			this.checkRemoveSideBorder.TabIndex = 18;
			this.checkRemoveSideBorder.Text = "Remove Image &Border";
			this.checkRemoveSideBorder.UseVisualStyleBackColor = true;
			// 
			// textExifFont
			// 
			this.textExifFont.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ExifFontFace", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textExifFont.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ExifFontColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textExifFont.Font = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ExifFontFace;
			this.textExifFont.ForeColor = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ExifFontColor;
			this.textExifFont.Location = new System.Drawing.Point(152, 448);
			this.textExifFont.Margin = new System.Windows.Forms.Padding(4);
			this.textExifFont.Name = "textExifFont";
			this.textExifFont.ReadOnly = true;
			this.textExifFont.Size = new System.Drawing.Size(315, 25);
			this.textExifFont.TabIndex = 28;
			// 
			// picClockColor
			// 
			this.picClockColor.BackColor = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ClockColor;
			this.picClockColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picClockColor.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ClockColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.picClockColor.Location = new System.Drawing.Point(152, 640);
			this.picClockColor.Margin = new System.Windows.Forms.Padding(4);
			this.picClockColor.Name = "picClockColor";
			this.picClockColor.Size = new System.Drawing.Size(176, 24);
			this.picClockColor.TabIndex = 21;
			this.picClockColor.TabStop = false;
			// 
			// picBackgroundColor
			// 
			this.picBackgroundColor.BackColor = global::digital_photo_frame_screen_saver.Properties.Settings.Default.BackgroundColor;
			this.picBackgroundColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.picBackgroundColor.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "BackgroundColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.picBackgroundColor.Location = new System.Drawing.Point(152, 248);
			this.picBackgroundColor.Margin = new System.Windows.Forms.Padding(4);
			this.picBackgroundColor.Name = "picBackgroundColor";
			this.picBackgroundColor.Size = new System.Drawing.Size(320, 24);
			this.picBackgroundColor.TabIndex = 21;
			this.picBackgroundColor.TabStop = false;
			// 
			// checkResizeHighQuality
			// 
			this.checkResizeHighQuality.Checked = global::digital_photo_frame_screen_saver.Properties.Settings.Default.ResizeHighQuality;
			this.checkResizeHighQuality.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkResizeHighQuality.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "ResizeHighQuality", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkResizeHighQuality.Location = new System.Drawing.Point(152, 216);
			this.checkResizeHighQuality.Margin = new System.Windows.Forms.Padding(4);
			this.checkResizeHighQuality.Name = "checkResizeHighQuality";
			this.checkResizeHighQuality.Size = new System.Drawing.Size(456, 24);
			this.checkResizeHighQuality.TabIndex = 15;
			this.checkResizeHighQuality.Text = "Use High &Quality Resize Method";
			this.checkResizeHighQuality.UseVisualStyleBackColor = true;
			// 
			// checkSearchSubDirectory
			// 
			this.checkSearchSubDirectory.Checked = global::digital_photo_frame_screen_saver.Properties.Settings.Default.IncludeSubDirectories;
			this.checkSearchSubDirectory.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "IncludeSubDirectories", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.checkSearchSubDirectory.Location = new System.Drawing.Point(152, 72);
			this.checkSearchSubDirectory.Margin = new System.Windows.Forms.Padding(4);
			this.checkSearchSubDirectory.Name = "checkSearchSubDirectory";
			this.checkSearchSubDirectory.Size = new System.Drawing.Size(456, 24);
			this.checkSearchSubDirectory.TabIndex = 5;
			this.checkSearchSubDirectory.Text = "Include &Sub Directories";
			this.checkSearchSubDirectory.UseVisualStyleBackColor = true;
			// 
			// textFileNameFilter
			// 
			this.textFileNameFilter.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::digital_photo_frame_screen_saver.Properties.Settings.Default, "FileNameFilter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textFileNameFilter.Location = new System.Drawing.Point(152, 40);
			this.textFileNameFilter.Margin = new System.Windows.Forms.Padding(4);
			this.textFileNameFilter.Name = "textFileNameFilter";
			this.textFileNameFilter.Size = new System.Drawing.Size(456, 22);
			this.textFileNameFilter.TabIndex = 4;
			this.textFileNameFilter.Text = global::digital_photo_frame_screen_saver.Properties.Settings.Default.FileNameFilter;
			// 
			// textPicturePath
			// 
			this.textPicturePath.Location = new System.Drawing.Point(152, 8);
			this.textPicturePath.Margin = new System.Windows.Forms.Padding(4);
			this.textPicturePath.Name = "textPicturePath";
			this.textPicturePath.Size = new System.Drawing.Size(408, 22);
			this.textPicturePath.TabIndex = 1;
			this.textPicturePath.Text = global::digital_photo_frame_screen_saver.Properties.Settings.Default.PicturePath;
			// 
			// Setting
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(622, 737);
			this.Controls.Add(this.trackClockVerticalPosition);
			this.Controls.Add(this.trackClockSize);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.checkClockColorAutomatic);
			this.Controls.Add(this.checkRemoveSideBorder);
			this.Controls.Add(this.btnExifFontChoose);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textExifFont);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.cbClockMode);
			this.Controls.Add(this.cbExifMode);
			this.Controls.Add(this.btnClockColorChoose);
			this.Controls.Add(this.btnColorChoose);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.labelClockVerticalPosition);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.labelClockSize);
			this.Controls.Add(this.labelClockColor);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.picClockColor);
			this.Controls.Add(this.picBackgroundColor);
			this.Controls.Add(this.checkResizeHighQuality);
			this.Controls.Add(this.checkSearchSubDirectory);
			this.Controls.Add(this.textFileNameFilter);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textRotInterval);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btnBrowseFolder);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.cbTargetDisplay);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cbMultiDisp);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.cbViewMode);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.cbRotMode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPicturePath);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Setting";
			this.Text = "Configuration - Digital Photo Frame Screen Saver";
			((System.ComponentModel.ISupportInitialize)(this.trackClockVerticalPosition)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackClockSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picClockColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picBackgroundColor)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.TextBox textPicturePath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbRotMode;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbViewMode;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cbMultiDisp;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox cbTargetDisplay;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnBrowseFolder;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textRotInterval;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textFileNameFilter;
		private System.Windows.Forms.CheckBox checkSearchSubDirectory;
		private System.Windows.Forms.CheckBox checkResizeHighQuality;
		private System.Windows.Forms.ColorDialog colorBackGroundDialog;
		private System.Windows.Forms.PictureBox picBackgroundColor;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btnColorChoose;
		private System.Windows.Forms.ComboBox cbExifMode;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.FontDialog fontDialog;
		private System.Windows.Forms.TextBox textExifFont;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button btnExifFontChoose;
		private System.Windows.Forms.CheckBox checkRemoveSideBorder;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.ComboBox cbClockMode;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.CheckBox checkClockColorAutomatic;
		private System.Windows.Forms.PictureBox picClockColor;
		private System.Windows.Forms.Label labelClockColor;
		private System.Windows.Forms.Button btnClockColorChoose;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TrackBar trackClockSize;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label labelClockSize;
		private System.Windows.Forms.Label labelClockVerticalPosition;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TrackBar trackClockVerticalPosition;
	}
}