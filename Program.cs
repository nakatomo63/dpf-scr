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
using System.Linq;
using System.Windows.Forms;

namespace digital_photo_frame_screen_saver
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] args )
		{
#if !DEBUG
			try
			{
#endif
			// check for multiple instances
			System.Threading.Mutex mu = new System.Threading.Mutex( false, "digital_photo_frame_screen_saver" );
			if( mu.WaitOne( 0, false ) == false )
			{
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );

			// 前バージョンからの設定更新
			if( Properties.Settings.Default._UpgradeRequired )
			{
				Properties.Settings.Default.Upgrade();
				// 次の同じバージョンで起動したときはUpgradeしない
				Properties.Settings.Default._UpgradeRequired = false;
				Properties.Settings.Default.Save();
			}

			if( args.Length > 0 )
			{
				string arg = args[ 0 ].ToLower( System.Globalization.CultureInfo.InvariantCulture ).Trim().Substring( 0, 2 );
				switch( arg )
				{
					// show options dialog
					case "/c":
						ShowOptions();
						break;
					// show preview
					case "/p":
						break;
					// password?
					case "/a":
						break;
					// show screen saver
					case "/s":
						ShowScreenSaver();
						break;
					default:
						MessageBox.Show( "Invalid Command Line Argument: " + arg, "Invalid Parameter", MessageBoxButtons.OK, MessageBoxIcon.Error );
						break;
				}
			}
			else
			{
				ShowOptions();
			}

			mu.ReleaseMutex();
#if !DEBUG
			}
			catch( Exception e )
			{
				MessageBox.Show
					( string.Format( "Unknown Error Occurred: {0}\nfrom {1}\n{2}", e.Message, e.Source, e.StackTrace )
					, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
#endif
		}

		static void ShowOptions()
		{
			// show options dialog
			Application.Run( new Setting() );
		}

		static void ShowScreenSaver()
		{
			// show screen saver
			Application.Run( new Saver() );
		}
	}
}
