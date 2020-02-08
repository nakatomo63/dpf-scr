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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace digital_photo_frame_screen_saver
{
	public partial class Setting : Form
	{
		private class InvalidInputValueException : System.Exception { }

		public Setting()
		{
			InitializeComponent();

			// initialize combo boxes
			InitMonitorList();
			LoadSettings();
		}

		void LoadSettings()
		{
			// loadは勝手にやってくれる
//			this.textPicturePath.Text = Properties.Settings.Default.PicturePath;
//			this.textFileNameFilter.Text = Properties.Settings.Default.FileNameFilter;
//			this.checkSearchSubDirectory.Checked = Properties.Settings.Default.IncludeSubDirectories;
//			this.checkResizeHighQuality.Checked = Properties.Settings.Default.ResizeHighQuality;
			this.cbRotMode.SelectedIndex = ( int ) Properties.Settings.Default.RotationMode;
			this.cbViewMode.SelectedIndex = ( int ) Properties.Settings.Default.ViewMode;
			this.textRotInterval.Text = Properties.Settings.Default.RotationInterval.ToString();
//			this.picBackgroundColor.BackColor = Properties.Settings.Default.BackgroundColor;
			this.cbMultiDisp.SelectedIndex = ( int ) Properties.Settings.Default.MultipleDisplayBehavior;
//			this.textExifFont.Font = Properties.Settings.Default.ExifFontFace;
//			this.textExifFont.ForeColor = Properties.Settings.Default.ExifFontColor;
			this.textExifFont.Text = Properties.Settings.Default.ExifFontFace.ToString();
			this.textExifFont.BackColor = Properties.Settings.Default.BackgroundColor;
			this.labelClockSize.Text = this.trackClockSize.Value.ToString() + "%";
			this.labelClockVerticalPosition.Text = this.trackClockVerticalPosition.Value.ToString() + "%";

			int targetDisplay = ( int ) Properties.Settings.Default.TargetDisplayNum;
			int monitorCount = this.cbTargetDisplay.Items.Count;

			// Single monitor
			if( monitorCount == 1 )
			{
				this.cbTargetDisplay.SelectedIndex = 0;
				this.cbTargetDisplay.Enabled = false;
			}
			else
			{
				if( targetDisplay >= monitorCount )
				{
					this.cbTargetDisplay.SelectedIndex = 0;
				}
				else
				{
					this.cbTargetDisplay.SelectedIndex = targetDisplay;
				}
			}

			this.cbExifMode.SelectedIndex = Math.Min( ( int ) Properties.Settings.Default.ExifMode, monitorCount );
			this.cbClockMode.SelectedIndex = Math.Min( ( int ) Properties.Settings.Default.ClockMode, monitorCount );

			this.btnClockColorChoose.Enabled = !this.checkClockColorAutomatic.Checked;
		}

		void SaveSettings()
		{
			// validate input value
			try
			{
				if( uint.Parse( this.textRotInterval.Text ) == 0 )
				{
					throw new OverflowException( "Timer Interval equals to 0." );
				}
			}
			catch( Exception )
			{
				MessageBox.Show( this, "'Rotation Interval' can only be positive integer value.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				this.textRotInterval.Focus();
				this.textRotInterval.SelectAll();
				throw new InvalidInputValueException();
			}

			try
			{
				if( this.textFileNameFilter.Text.Length != 0 )
				{
					// compile test
					System.Text.RegularExpressions.Regex r
						= new System.Text.RegularExpressions.Regex( this.textFileNameFilter.Text );
				}
				else
				{
					this.textFileNameFilter.Text = ".*";
				}
			}
			catch( Exception )
			{
				MessageBox.Show( this, "Regular Expression in 'File Name Filter' cannot be compiled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				this.textFileNameFilter.Focus();
				this.textFileNameFilter.SelectAll();
				throw new InvalidInputValueException();
			}

			// 詳細不明だが書き戻しは自分で書かないとダメ？なようだ
			Properties.Settings.Default.PicturePath = this.textPicturePath.Text;
			Properties.Settings.Default.FileNameFilter = this.textFileNameFilter.Text;
			Properties.Settings.Default.IncludeSubDirectories = this.checkSearchSubDirectory.Checked;
			Properties.Settings.Default.ViewMode = ( uint ) Math.Max( this.cbViewMode.SelectedIndex, 0 );
			Properties.Settings.Default.ResizeHighQuality = this.checkResizeHighQuality.Checked;
			Properties.Settings.Default.RotationMode = ( uint ) Math.Max( this.cbRotMode.SelectedIndex, 0 );
			Properties.Settings.Default.RotationInterval = uint.Parse( this.textRotInterval.Text );
			Properties.Settings.Default.BackgroundColor = this.picBackgroundColor.BackColor;
			Properties.Settings.Default.MultipleDisplayBehavior = ( uint ) Math.Max( this.cbMultiDisp.SelectedIndex, 0 );
			Properties.Settings.Default.TargetDisplayNum = ( uint ) this.cbTargetDisplay.SelectedIndex;
			Properties.Settings.Default.ExifMode = ( uint ) this.cbExifMode.SelectedIndex;
			Properties.Settings.Default.ExifFontFace = this.textExifFont.Font;
			Properties.Settings.Default.ExifFontColor = this.textExifFont.ForeColor;

			Properties.Settings.Default.ClockMode = ( uint ) this.cbClockMode.SelectedIndex;
			Properties.Settings.Default.ClockSize = this.trackClockSize.Value;
			Properties.Settings.Default.ClockColor = this.picClockColor.BackColor;
			Properties.Settings.Default.ClockColorAutomatic = this.checkClockColorAutomatic.Checked;
			Properties.Settings.Default.ClockVerticalPosition = this.trackClockVerticalPosition.Value;

			Properties.Settings.Default.Save();
		}

		void InitMonitorList()
		{
			int i = 1;
			cbTargetDisplay.Items.Clear();
			cbExifMode.Items.Clear();
			cbTargetDisplay.BeginUpdate();
			cbExifMode.BeginUpdate();

			cbExifMode.Items.Add( "None" );
			cbClockMode.Items.Add( "None" );

			foreach( Screen s in Screen.AllScreens )
			{
				string text = String.Format( "Display{0}: ({1},{2}) {3}x{4}"
					, i, s.Bounds.Left, s.Bounds.Top, s.Bounds.Width, s.Bounds.Height );
				cbTargetDisplay.Items.Add( text );
				cbExifMode.Items.Add( "Show on " + text );
				cbClockMode.Items.Add( "Show on " + text );
				++i;
			}

			cbTargetDisplay.EndUpdate();
			cbExifMode.EndUpdate();
		}

		private void btnBrowseFolder_Click( object sender, EventArgs e )
		{
			this.folderBrowserDialog.SelectedPath = this.textPicturePath.Text;
			if( this.folderBrowserDialog.ShowDialog() == DialogResult.OK )
			{
				this.textPicturePath.Text = this.folderBrowserDialog.SelectedPath;
			}
		}

		private void btnCancel_Click( object sender, EventArgs e )
		{
			// close dialog
			this.Close();
		}

		private void btnOK_Click( object sender, EventArgs e )
		{
			// save settings
			try
			{
				this.SaveSettings();
				this.Close();
			}
			catch( InvalidInputValueException )
			{
				return;
			}
		}

		private void cbMultiDisp_SelectedIndexChanged( object sender, EventArgs e )
		{
			// 'Single' => Enable
			this.cbTargetDisplay.Enabled = ( this.cbMultiDisp.SelectedIndex == 0 );
		}

		private void btnColorChoose_Click( object sender, EventArgs e )
		{
			this.colorBackGroundDialog.Color = this.picBackgroundColor.BackColor;
			this.colorBackGroundDialog.AllowFullOpen = true;
			this.colorBackGroundDialog.SolidColorOnly = false;

			if( this.colorBackGroundDialog.ShowDialog( this ) == DialogResult.OK )
			{
				this.picBackgroundColor.BackColor = this.textExifFont.BackColor = this.colorBackGroundDialog.Color;
			}
		}

		private void btnExifFontChoose_Click( object sender, EventArgs e )
		{
			this.fontDialog.Font = this.textExifFont.Font;
			this.fontDialog.Color = this.textExifFont.ForeColor;
			if( this.fontDialog.ShowDialog() != DialogResult.Cancel )
			{
				this.textExifFont.Font = this.fontDialog.Font;
				this.textExifFont.ForeColor = this.fontDialog.Color;
				this.textExifFont.Text = this.fontDialog.Font.ToString();
			}
		}

		private void cbExifMode_SelectedIndexChanged( object sender, EventArgs e )
		{
			this.textExifFont.Enabled = this.btnExifFontChoose.Enabled
				= ( this.cbExifMode.SelectedIndex > 0 );
		}

		private void trackClockSize_Scroll( object sender, EventArgs e )
		{
			this.labelClockSize.Text = this.trackClockSize.Value.ToString() + "%";
		}

		private void btnClockColorChoose_Click( object sender, EventArgs e )
		{
			this.colorBackGroundDialog.Color = this.picClockColor.BackColor;
			this.colorBackGroundDialog.AllowFullOpen = true;
			this.colorBackGroundDialog.SolidColorOnly = false;

			if( this.colorBackGroundDialog.ShowDialog( this ) == DialogResult.OK )
			{
				this.picClockColor.BackColor = this.colorBackGroundDialog.Color;
			}
		}

		private void checkClockColorAutomatic_CheckedChanged( object sender, EventArgs e )
		{
			// 時計色自動セレクトがON => Chooseできなくする
			this.btnClockColorChoose.Enabled = !this.checkClockColorAutomatic.Checked;
		}

		private void trackClockVerticalPosition_Scroll( object sender, EventArgs e )
		{
			this.labelClockVerticalPosition.Text = this.trackClockVerticalPosition.Value.ToString() + "%";
		}
	}
}
