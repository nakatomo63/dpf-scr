using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace digital_photo_frame_screen_saver
{
	public partial class Saver : Form
	{
        internal static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SystemParametersInfoA( uint uiAction, uint uiParam, out uint pvParam, uint fWinIni );
            // ^^ SystemParametersInfo**A**()をつけないとダメ。なぜか今までは動いていた。
            public const uint SPI_GETSCREENSAVETIMEOUT = 0x000E;
        }

		private Size drawArea;
		private Point origin;

		// マウス操作で終了する
		private Point mouseLocation;

		// 現在のframe. Update()時になる早で描画される
		private Bitmap currentFrame;
		// 次のframe. Background Workerが作っておいてくれる
		private bool backFrameReady;
		private Bitmap backFrame;

		private List< string > fileList;
		private List< string > fileQueue;
		private int fileQueuePos;

		// 時計用のframe（現在時刻）
		private Bitmap clockFrame;
		// 時計用のframe（次の時刻・秒）
		private Bitmap backClockFrame;
		// 時計用 処理待ち時間履歴
		private List<long> clockRefleshTime;
		// 時計の桁を描くもの
		private Segment7Clock clockDrawer;
		// clockFrameに描かれている現在時刻
		private DateTime clockTargetTime;
		// backClockFrameに描かれている次の時刻
		private DateTime backClockTime;
		// 時計の描画位置
		private Rectangle clockTargetRect;
		// 時計の色
		private Color clockColor1, clockColor2, clockBackColor1, clockBackColor2;

		public Saver()
		{
			this.InitializeComponent();

			this.backFrameReady = false;

			// get system parameter
			uint timeout = 0;
			if( NativeMethods.SystemParametersInfoA( NativeMethods.SPI_GETSCREENSAVETIMEOUT, 0, out timeout, 0 ) && timeout != 0 )
			{
				// succeeded
				this.timerSuspendTimeout.Interval = ( int ) timeout * 1000;
			}
			else
			{
				// failed -> 1 minute
				this.timerSuspendTimeout.Interval = 1 * 60 * 1000;
			}

			// Current settings are empty => load from previous version
			if( Properties.Settings.Default.PicturePath == "" )
			{
				Properties.Settings.Default.Upgrade();
			}

			this.clockFrame = this.backClockFrame = null;
			this.clockRefleshTime = new List<long>();
			this.clockDrawer = new Segment7Clock();
			this.clockTargetTime = this.backClockTime = DateTime.Now;

			// initialize
			this.mouseLocation.X = this.mouseLocation.Y = -1;
		}


		List<string> EnumAllFiles( string dir, string pattern, bool subdir )
		{
			// compile to regex
			Regex r = new Regex( pattern );
			List<string> list = new List<string>();

			foreach( string file in
				System.IO.Directory.GetFiles
				( dir
				, "*"
				, subdir ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly ) )
			{
				if( r.IsMatch( file ) )
				{
					list.Add( file );
				}
			}

			return list;
		}

		// generate file queue
		void GenerateQueue()
		{
			List< string > temp = new List< string >( this.fileList );
			switch( Properties.Settings.Default.RotationMode )
			{
			// rotate by file name order
			case 0:
				temp.Sort();
				break;
			// rotate by date
			case 1:
				temp.Sort( delegate ( string a, string b )
					{
						return DateTime.Compare( System.IO.File.GetCreationTime( a ), System.IO.File.GetCreationTime( b ) );
					}
				);
				break;
			// random
			case 2:
				// don't need to do anything
				break;
			// shuffle
			case 3:
				Random rand = new Random();
				for( int i = 0; i < temp.Count; ++i )
				{
					// 上手く混ざらない気がするため左辺は必ず全itemを対象にする。
					//int x = rand.Next( temp.Count );
					int y = rand.Next( temp.Count );
					//string t = temp[ x ];
					//temp[ x ] = temp[ y ];
					string t = temp[ i ];
					temp[ i ] = temp[ y ];
					temp[ y ] = t;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException( "Properties.Settings.Default.RotationMode" );
			}

			this.fileQueue = temp;
			this.fileQueuePos = -1;
		}

		// get a file from queue
		string GetNextFile()
		{
			// random
			if( Properties.Settings.Default.RotationMode == 2 )
			{
				Random rand = new Random();
				return this.fileList[ rand.Next( this.fileList.Count ) ];
			}
			else
			{
				// reload
				if( ++this.fileQueuePos >= this.fileQueue.Count )
				{
					this.fileQueuePos = 0;
				}

				return this.fileQueue[ this.fileQueuePos ];
			}
		}

		// previous file
		string GetPrevFile()
		{
			// random
			if( Properties.Settings.Default.RotationMode == 2 )
			{
				return this.GetNextFile();
			}
			else
			{
				// reload
				if( this.fileQueuePos <= 0 )
				{
					this.fileQueuePos = this.fileQueue.Count;
				}

				return this.fileQueue[ --this.fileQueuePos ];
			}
		}

		void StartSlideShow()
		{
			// set form position
			Rectangle wholeScreen = new Rectangle( 0, 0, 0, 0 );
			foreach( Screen s in Screen.AllScreens )
			{
				wholeScreen = Rectangle.Union( wholeScreen, s.Bounds );
			}

			// formの位置を原点に移動
			this.Location = this.origin = new Point( wholeScreen.Left, wholeScreen.Top );
			this.Size = this.drawArea = new Size( wholeScreen.Width, wholeScreen.Height );
			this.SetStyle( ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true );

			// マウスキャプチャ
			this.Capture = true;
			Cursor.Hide();
			
			// search all files in the directory
			this.fileList
				= EnumAllFiles
				( Properties.Settings.Default.PicturePath
				, Properties.Settings.Default.FileNameFilter
				, Properties.Settings.Default.IncludeSubDirectories );

			GenerateQueue();

			// 時計表示の設定
			uint clockmode = Properties.Settings.Default.ClockMode;
			if( clockmode >= 1 )
			{
				// 対象モニタのRect
				var monRect = Screen.AllScreens[ clockmode - 1 ].Bounds;
				// 時計のサイズ
				var size = this.clockDrawer.Size;
				// 描画サイズ（モニタの横幅に対する割合）
				double ratio = Properties.Settings.Default.ClockSize / 100.0;

				int width = ( int ) ( monRect.Width * ratio );
				int height = ( int ) ( width * size.Y );

				// centering
				this.clockTargetRect = new Rectangle(
					monRect.X + ( monRect.Width - width ) / 2 - this.origin.X,
					monRect.Y + ( monRect.Height - height ) * Properties.Settings.Default.ClockVerticalPosition / 100 - this.origin.Y,
					width, height );

				// 既定の色にしておく
				this.clockColor1 = this.clockColor2 = this.clockBackColor1 = this.clockBackColor2
					= Properties.Settings.Default.ClockColor;

				// start
				// 初回はすぐに
				this.timerClock.Interval = 1;
				this.timerClock.Start();
			}

			// load first file
			this.LoadPicture( this.GetNextFile() );
			this.backFrameReady = true;

			// do first drawing
			this.OnTimer( this.GetNextFile() );
			// start timer
			this.timerPicChange.Interval = ( int ) Properties.Settings.Default.RotationInterval;
			this.timerPicChange.Start();
		}

		// 画像の左右のborderの幅を検出して有効領域を返す
		Rectangle DetectValidArea( Bitmap image )
		{
			// 24bppの場合のみ対応ということにしておく
			if( image.PixelFormat != PixelFormat.Format24bppRgb )
			{
				// 元画像そのもの
				return new Rectangle( new Point( 0, 0 ), image.Size );
			}

			// 左端の一列の色がすべて一致するかを調査する
			BitmapData bm = image.LockBits
				( new Rectangle( 0, 0, image.Width, image.Height )
				, ImageLockMode.ReadOnly, image.PixelFormat );
			int stride = Math.Abs( bm.Stride );
			int ps = 0;
			int eps = 5;    // 圧縮のせい？で微妙に画素値が異なることがある

			switch( image.PixelFormat )
			{
			case PixelFormat.Format24bppRgb: ps = 3; break;
			default: throw new Exception( "Unknown pixel format" );
			}

			int border_left;
			int border_right;

			unsafe
			{
				byte* p0 = ( byte* ) bm.Scan0.ToPointer();

				// border color
				byte[] px0 = new byte[ ps ];
				// 左端上の色がborder colorだとする
				for( int i = 0; i < ps; ++i ) { px0[ i ] = p0[ i ]; }

				int x;
				for( x = 0; x < image.Width; ++x )
				{
					// 縦方向に見ているので計算が遅い・・・。
					int y;
					for( y = 0; y < image.Height; ++y )
					{
						byte* p = p0 + stride * y + ps * x;
						int i;
						for( i = 0; i < ps; ++i )
						{
							if( Math.Abs( ( int ) p[ i ] - px0[ i ] ) > eps )
							{
								// 色が違う
								break;
							}
						}
						if( i != ps ) { break; }
					}

					// 最後の行まで見られたらOK
					if( y != image.Height )
					{
						break;
					}
				}

				// 0でborderなし
				border_left = x;

				// 逆からも調べる
				for( x = 0; x < image.Width; ++x )
				{
					int y;
					for( y = 0; y < image.Height; ++y )
					{
						byte* p = p0 + stride * y + ps * ( image.Width - x - 1 );
						int i;
						for( i = 0; i < ps; ++i )
						{
							if( Math.Abs( ( int ) p[ i ] - px0[ i ] ) > eps )
							{
								break;
							}
						}
						if( i != ps ) { break; }
					}

					// 最後の行まで見られたらOK
					if( y != image.Height )
					{
						break;
					}
				}

				// == image.Widthでborderなし
				border_right = image.Width - x;
			}

			// 調査終了
			image.UnlockBits( bm );

			return new Rectangle( border_left, 0, border_right - border_left, image.Height );
		}

		// 画像全体の代表色を検出する
		( Color primaryColor, Color secondaryColor ) DetectRepresentingColor( Bitmap image )
		{
			// RGB -> HSV
			Func<int, int, int, (float h, float s, float v)> RGB2HSV = ( int r, int g, int b ) =>
			{
				float h, s, v;
				int max, min;

				// hue
				if( r == g && g == b )
				{
					h = float.NaN;
					max = min = r;
				}
				else if( r >= g && r >= b )
				{
					max = r;
					min = Math.Min( g, b );
					h = 60.0f * ( g - b ) / ( max - min );
				}
				else if( g >= r && g >= b )
				{
					max = g;
					min = Math.Min( r, b );
					h = 60.0f * ( b - r ) / ( max - min ) + 120;
				}
				else
				{
					max = b;
					min = Math.Min( r, g );
					h = 60.0f * ( r - g ) / ( max - min ) + 240;
				}

				if( h < 0 ) { h += 360; }
				if( h >= 360 ) { h %= 360; }

				// saturation
				if( max == 0 )
				{
					s = 0;
				}
				else
				{
					s = ( ( float ) max - min ) / max;
				}

				// value
				v = max;

				return (h, s, v);
			};

			// HSV -> RGB
			Func<float, float, float, (int r, int g, int b)> HSV2RGB = ( float hue, float sat, float val ) =>
			{
				if( float.IsNaN( hue ) ) { hue = 0f; }

				var dh = Math.Floor( ( hue % 360 ) / 60 );
				var p = ( int ) Math.Round( 255 * val * ( 1 - sat ) );
				var q = ( int ) Math.Round( 255 * val * ( 1 - sat * ( hue / 60 - dh ) ) );
				var t = ( int ) Math.Round( 255 * val * ( 1 - sat * ( 1 - ( hue / 60 - dh ) ) ) );
				var v = ( int ) Math.Round( 255 * val );

				int r, g, b;

				switch( dh )
				{
				case 0: r = v; g = t; b = p; break;
				case 1: r = q; g = v; b = p; break;
				case 2: r = p; g = v; b = t; break;
				case 3: r = p; g = q; b = v; break;
				case 4: r = t; g = p; b = v; break;
				case 5: r = v; g = p; b = q; break;
				default: r = g = b = 0; break;
				}

				return (r, g, b);
			};

			// 24bppの場合のみ対応ということにしておく
			if( image.PixelFormat != PixelFormat.Format24bppRgb )
			{
				// 元画像の(0,0)をそのまま採用
				var c = image.GetPixel( 0, 0 );
				return (c, c); 
			}

			// 長辺240pxくらいにリサイズする（そんなに大きくても意味がないため）
			int span = 240;
			var procSize = new Rectangle( 0, 0, 0, 0 );
			if( image.Width >= image.Height )
			{
				procSize.Width = span;
				procSize.Height = span * image.Height / image.Width;
			}
			else
			{
				procSize.Width = span * image.Width / image.Height;
				procSize.Height = span;
			}

			// ここに縮小して描画
			var imgSmall = new Bitmap( procSize.Width, procSize.Height, image.PixelFormat );
			var gSmall = Graphics.FromImage( imgSmall );
			gSmall.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
			gSmall.DrawImage( image, procSize );

			// pixel access
			var bm = imgSmall.LockBits
				( new Rectangle( 0, 0, imgSmall.Width, imgSmall.Height )
				, ImageLockMode.ReadOnly, imgSmall.PixelFormat );
			int stride = Math.Abs( bm.Stride );
			byte[] pixel = new byte[ stride * imgSmall.Height ];
			Marshal.Copy( bm.Scan0, pixel, 0, pixel.Length );
			// ここで使い終わり
			imgSmall.UnlockBits( bm );

			// Hueは中央値を使う
			// Saturationは平均値を使う
			//   =>画像全体をscanして計算する
			// Valueは固定値とする
			var hue_l = new List<float>();
			var sat_l = new List<float>();

			var hue_hist = new List<int>();
			for( int i = 0; i < 360; ++i ) { hue_hist.Add( 0 ); }

			for( int y = 0; y < imgSmall.Height; ++y )
			{
				for( int x = 0; x < imgSmall.Width; ++x )
				{
					// RGB -> HSV convert
					int offset = stride * y + 3 * x;
					int b = pixel[ offset ];
					int g = pixel[ offset + 1 ];
					int r = pixel[ offset + 2 ];

					(float h, float s, float v) = RGB2HSV( r, g, b );

					Debug.Assert( !float.IsNaN( s ) );

					sat_l.Add( s );

					if( !float.IsNaN( h ) )
					{
						hue_l.Add( h );
						// 彩度に応じて重みづけをする
						hue_hist[ ( int ) h ] += ( int ) Math.Pow( s * 10, 2 );
					}
				}
			}

#if false
			// Hueの中央値は？
			//float hue_med = QuickSelect.Median( hue_l );

			// Hueの場合、値の大小そのものに意味はない。Hue=0とHue=359は殆ど同じ。
			// ところが「中央値」として抽出すると中々選ばれない値になってしまう。
			// （当然画像の頻度分布にもよるが、相手が自然画像なら含まれるHueの範囲の幅は
			//   そんなに変わらない（と思われる）わけで、結果として真ん中＝180付近が選ばれやすくなる（はず））
			// よって、最頻値を採用してみる
			//
			// ただし、当然だが空の色など背景の地の色を拾いやすくなる。。。
			// =>saturationによって重みづけをした
			float hue_med = 0;
			int count = 0;
			for( int i = 0; i < hue_hist.Count; ++i )
			{
				if( count <= hue_hist[ i ] )
				{
					count = hue_hist[ i ];
					hue_med = i;
				}
			}

			// ★やはり空の色とDD51のキャブの色が競合して「青」になってしまう。
			// 二色以上選ぶ必要がありそう。二色選んでグラデーションにする？うざいか？
			// Otsu法で2クラスタに分割してそれぞれの最頻値を取ればよい。
			// 三色以上選びたければ、Otsu法を再帰的に適用するか、k-means法を使う
			// k-means法は初期値をランダムに設定するので収束が若干不安定。よってk-means++法がよさそう。
			// グラデーションは文字そのものをグラデーションで描くのではなく、
			// グラデ背景を文字で切り抜くようにする。透明色（A=0xFF）で描画する？
#endif
			// ヒストグラムの谷を見つけてそこを境界とする
			// Hueは回転しても同じ意味になる。赤っぽい画像だと0度付近で山が切られてしまうおそれがある。
			int min_count = int.MaxValue;
			int phase = 0;
			for( int i = 0; i < hue_hist.Count; ++i )
			{
				if( min_count > hue_hist[ i ] )
				{
					min_count = hue_hist[ i ];
					phase = i;

					if( min_count == 0 )
					{
						break;
					}
				}
			}

			// 回転したヒストグラム
			var hue_hist_rot = new List<int>();
			for( int n = 0; n < hue_hist.Count; ++n )
			{
				int idx = n + phase;
				if( idx >= hue_hist.Count ) { idx -= hue_hist.Count; }
				hue_hist_rot.Add( hue_hist[ idx ] );
			}

			// Otsu法でthresholdを求める
			int thresh = ListUtility.OtsuThresh( hue_hist_rot, 0, hue_hist_rot.Count );

			// 前半の最大値と後半の最大値（の位置）を求める
			int first_max = ListUtility.MaxPos( hue_hist_rot, 0, thresh );
			int second_max = ListUtility.MaxPos( hue_hist_rot, thresh, hue_hist_rot.Count );

			// 最頻値をそれぞれ求める
			int hue_1st = ( first_max + phase ) % hue_hist.Count;
			int hue_2nd = ( second_max + phase ) % hue_hist.Count;
			int hue_pri, hue_sec;

			if( hue_hist[ hue_1st ] > hue_hist[ hue_2nd ] )
			{
				hue_pri = hue_1st;
				hue_sec = hue_2nd;
			}
			else
			{
				hue_pri = hue_2nd;
				hue_sec = hue_1st;
			}

			// Saturationは平均値をもらう
			// あんまり低いと味気ないので少し下駄をはかせる
			float sat_avg = 0.3f + 0.7f * sat_l.Average();

			// valueは一定値にする（ある程度明るくないと見えないと思われるため）
			float value_target = 0.8f;

			Debug.WriteLine( "Representing Color HSV = (" + hue_1st + "," + sat_avg + "," + value_target + ")" );

			// 目標色を作る
			Color primary, secondary;

			if( sat_avg > 0 )
			{
				var (r1, g1, b1) = HSV2RGB( hue_pri, sat_avg, value_target );
				var (r2, g2, b2) = HSV2RGB( hue_sec, sat_avg, value_target );
				primary = Color.FromArgb( r1, g1, b1 );
				secondary = Color.FromArgb( r2, g2, b2 );
			}
			else
			{
				int value = ( int ) ( value_target * 255 );
				primary = secondary = Color.FromArgb( value, value, value );
			}

			Debug.WriteLine( "Representing Color RGB = (" + primary.ToString() + "),(" + secondary.ToString() + ")" );

			return (primary, secondary);
		}

		// 次のframeに次のpictureを読み込む
		// 基本的にはbgwPictureLoader_DoWork()のcontextで呼ばれる
		void LoadPicture( string nextFile )
		{
			// build next frame
			// read a file can be read
			Bitmap image = null;
			Bitmap inputimage = null;

			try
			{
				System.Diagnostics.Debug.WriteLine( String.Format( "File: '{0}'", nextFile ) );
				inputimage = new Bitmap( nextFile );
			}
			catch( Exception )
			{
				// おそらく非対応ファイル形式
				return;
			}

			// imageが額縁の場合、消去する
			if( Properties.Settings.Default.RemoveSideBorder )
			{
				var srcRect = DetectValidArea( inputimage );

				// 額縁検出？
				if( srcRect.X != 0 || srcRect.Width != inputimage.Width )
				{
					image = inputimage.Clone( srcRect, inputimage.PixelFormat );

					// metadataをコピーする
					foreach( int id in inputimage.PropertyIdList )
					{
						try { image.SetPropertyItem( inputimage.GetPropertyItem( id ) ); }
						catch( ArgumentException ) { /* ignore errors */ }
					}

					inputimage.Dispose();

					Debug.WriteLine( "Border detected: cropping rect=" + srcRect.ToString() );
				}
				else
				{
					image = inputimage;
				}
			}
			else
			{
				image = inputimage;
			}

			// frame buffer
			Bitmap nextFrame = new Bitmap( this.drawArea.Width, this.drawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb );
			using( Graphics gFrame = Graphics.FromImage( nextFrame ) )
			{
				// set resize quality
				if( Properties.Settings.Default.ResizeHighQuality )
				{
					gFrame.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				}

				// Fill
				gFrame.Clear( Properties.Settings.Default.BackgroundColor );

				// draw
				switch( Properties.Settings.Default.MultipleDisplayBehavior )
				{
				// Single - draw only on specified display
				case 0:
					DrawToArea( gFrame, image, Screen.AllScreens[ Properties.Settings.Default.TargetDisplayNum ].Bounds );
					break;
				// Clone - draw to all existing displays
				case 1:
					foreach( Screen s in Screen.AllScreens )
					{
						DrawToArea( gFrame, image, s.Bounds );
					}
					break;
				// Span
				case 2:
					DrawToArea( gFrame, image, new Rectangle( this.origin, this.drawArea ) );
					break;
				default:
					throw new ArgumentOutOfRangeException( "Properties.Settings.Default.MultipleDisplayBehavior" );
				}

				// show Exif Information
				if( Properties.Settings.Default.ExifMode > 0 )
				{
					DrawExifInfo( gFrame, image, Screen.AllScreens[ Properties.Settings.Default.ExifMode - 1 ].Bounds );
				}
			}

			// 代表色を求める
			if( Properties.Settings.Default.ClockMode >= 1 &&
				Properties.Settings.Default.ClockColorAutomatic )
			{
				( this.clockBackColor1, this.clockBackColor2 ) = DetectRepresentingColor( image );
			}

			image.Dispose();
			this.backFrame = nextFrame;
		}

		private void Saver_Load( object sender, EventArgs e )
		{
			StartSlideShow();
		}

		void DrawToArea( Graphics g, Image img, Rectangle targetRect )
		{
			Rectangle srcRect;
			Rectangle destRect;
            switch( Properties.Settings.Default.ViewMode )
            {
            // Center
            case 0:
                destRect = new Rectangle
                    ( Math.Max( 0, ( targetRect.Width - img.Width ) / 2 ) + targetRect.Left - this.origin.X
                    , Math.Max( 0, ( targetRect.Height - img.Height ) / 2 ) + targetRect.Top - this.origin.Y
                    , Math.Min( img.Width, targetRect.Width )
                    , Math.Min( img.Height, targetRect.Height )
                );

                srcRect = new Rectangle
                    ( Math.Max( 0, ( img.Width - targetRect.Width ) / 2 )
                    , Math.Max( 0, ( img.Height - targetRect.Height ) / 2 )
                    , Math.Min( img.Width, targetRect.Width )
                    , Math.Min( img.Height, targetRect.Height )
                );

                g.DrawImage( img, destRect, srcRect, GraphicsUnit.Pixel );
                break;
            // Tile
            case 1:
                for( int row = 0; row <= targetRect.Height / img.Height; ++row )
                {
                    for( int col = 0; col <= targetRect.Width / img.Width; ++col )
                    {
                        int width, height;
                        width = Math.Min
                                ( Math.Max( 0, targetRect.Width - img.Width * col ) // remaining area
                                , img.Width );
                        height = Math.Min
                                ( Math.Max( 0, targetRect.Height - img.Height * row )
                                , img.Height );

                        destRect = new Rectangle
                            ( img.Width * col + targetRect.Left - this.origin.X
                            , img.Height * row + targetRect.Top - this.origin.Y
                            , width
                            , height
                        );
                        srcRect = new Rectangle( 0, 0, width, height );

                        g.DrawImage( img, destRect, srcRect, GraphicsUnit.Pixel );
                    }
                }
                break;
            // Fit To Screen
            case 2:
                g.DrawImage( img
                    , targetRect.Left - this.origin.X
                    , targetRect.Top - this.origin.Y
                    , targetRect.Width
                    , targetRect.Height );
                break;
            // Fit To Screen maintaining Aspect Ratio
            case 3:
                // img is wide against the screen
                if( ( double ) img.Width / img.Height > ( double ) targetRect.Width / targetRect.Height )
                {
                    int width, height;
                    width = targetRect.Width;
                    height = ( int ) ( img.Height * ( ( double ) targetRect.Width / img.Width ) );

                    destRect = new Rectangle
                        ( 0 + targetRect.Left - this.origin.X
                        , ( targetRect.Height - height ) / 2 + targetRect.Top - this.origin.Y
                        , width, height );
                }
                // img is toll
                else
                {
                    int width, height;
                    width = ( int ) ( img.Width * ( ( double ) targetRect.Height / img.Height ) );
                    height = targetRect.Height;

                    destRect = new Rectangle
                        ( ( targetRect.Width - width ) / 2 + targetRect.Left - this.origin.X
                        , 0 + targetRect.Top - this.origin.Y
                        , width, height );
                }

                g.DrawImage( img, destRect );

                Debug.WriteLine( destRect.ToString() );
                break;
            // Crop
            // Aspect ratioが同じ場合はそのまま縮小
            // 異なる場合は画面内で最大化する。
            // 例えば16:10の画像を16:9の画面に表示するとき、画面の長辺側に合わせてresize。はみ出す部分はcropする。
            // ただし縦長の画像を横長の画面の横幅に合わせてresizeすると、元画像が大幅にcropされることになる。
            // これを防ぐため、縦長画像→横長画面の場合は画面の縦を目標にresize。逆に横長画像→縦長画面の場合は画面の横を目標にresize
            // 一般化すると、画像のアスペクト比ri = width_image / height_image、画面のアスペクト比rm = width_mon / height_monとして、
            //   ri < rm => width_monに合わせる
            //   rm > ri => height_monに合わせる
            case 4:
                // aspect ratio
                double r_img = img.Width / ( double ) img.Height;
                double r_mon = targetRect.Width / ( double ) targetRect.Height;

				// 画像も画面も横長or縦長の場合
				if( ( r_img >= 1.0 && r_mon >= 1.0 ) || ( r_img < 1.0 && r_mon < 1.0 ) )
				{
					// 画像の方が背が高い … 幅は100%使われ、上下がcropされる
					if( r_img < r_mon )
					{
						// 拡大縮小比
						double scale = img.Width / ( double ) targetRect.Width;
						// 実際に使われる元画像の縦の長さ
						int available_height = ( int ) ( targetRect.Height * scale );
						// はみ出す部分
						int cropped_tall = img.Height - available_height;

						// 当然これが正の値になるはず
						Debug.Assert( cropped_tall >= 0 );

						// 転送元Rect
						srcRect = new Rectangle( 0, cropped_tall / 2, img.Width, available_height );
					}
					// 画像の方が幅が広い … 高さは100%使われ、左右がcropされる
					else
					{
						double scale = img.Height / ( double ) targetRect.Height;
						int available_width = ( int ) ( targetRect.Width * scale );
						int cropped_wide = img.Width - available_width;

						Debug.Assert( cropped_wide >= 0 );

						srcRect = new Rectangle( cropped_wide / 2, 0, available_width, img.Height );
					}

					// 転送先は全面になる
					destRect = targetRect;
					destRect.X -= this.origin.X;
					destRect.Y -= this.origin.Y;
				}
				else
				{
					// 転送元は全面になる
					srcRect = new Rectangle( 0, 0, img.Width, img.Height );

					// 画像が横長なのに画面が縦長
					if( r_img >= 1.0 && r_mon < 1.0 )
					{
						// 幅は100%使われ、上下に余りが出る
						double scale = img.Width / ( double ) targetRect.Width;
						// リサイズ後の画像の高さ
						int available_height = ( int ) ( img.Height / scale );
						// 余る部分
						int remained_tall = targetRect.Height - available_height;

						// 当然これが正の値になるはず
						Debug.Assert( remained_tall >= 0 );

						// 転送先Rect
						destRect = new Rectangle(
							 0 + targetRect.Left - this.origin.X,
							 remained_tall / 2 + targetRect.Top - this.origin.Y,
							 targetRect.Width,
							 available_height );
					}
					// 画像が縦長なのに画面が横長の場合
					else
					{
						// 上下は100%使われ、左右に余りが出る
						double scale = img.Height / ( double ) targetRect.Height;
						int available_width = ( int ) ( img.Width / scale );
						int remained_wide = targetRect.Width - available_width;
						
						Debug.Assert( remained_wide >= 0 );

						destRect = new Rectangle(
							remained_wide / 2 + targetRect.Left - this.origin.X,
							0 + targetRect.Top - this.origin.Y,
							available_width,
							targetRect.Height );
					}
				}

				Debug.WriteLine( "dest=" + destRect.ToString() + ", src=" + srcRect.ToString() + ", target=" + targetRect.ToString() + ", origin=" + this.origin.ToString() );
				g.DrawImage( img, destRect, srcRect, GraphicsUnit.Pixel );

				break;
            default:
                throw new ArgumentOutOfRangeException( "Properties.Settings.Default.ViewMode" );
            }
		}

		void DrawExifInfo( Graphics g, Image img, Rectangle targetRect )
		{
			// Get Exif Information
			int[] IdList = img.PropertyIdList;

			Func<int, byte[]> GetValue = delegate( int id )
			{
				int idx = Array.IndexOf( IdList, id );
				if( idx == -1 )
				{
					return null;
				}
				else
				{
					return img.PropertyItems[ idx ].Value;
				}
			};

			Func<uint, uint, uint> GCD = delegate( uint a, uint b )
			{
				while( b != 0 )
				{
					uint r = a % b;
					a = b;
					b = r;
				}

				return a;
			};

			Func<byte[], string> AsString = delegate( byte[] binary )
			{
				return System.Text.Encoding.ASCII.GetString( binary ).TrimEnd( new char[] { '\0' } );
			};

			Func<byte[], double> AsRational = delegate( byte[] binary )
			{
				return ( double ) BitConverter.ToUInt32( binary, 0 ) / BitConverter.ToUInt32( binary, 4 );
			};

			List<string> InfoList = new List<string>();

			// Size
			InfoList.Add( string.Format( "Size: {0}x{1}", img.Width, img.Height ) );

			// ImageWidth, Height
			{
				byte[] ImageWidth = GetValue( 0x0100 );
				byte[] ImageHeight = GetValue( 0x0101 );
				if( ImageWidth != null && ImageHeight != null )
				{
					InfoList.Add( string.Format( "Original Size: {0}x{1}"
						, BitConverter.ToUInt32( ImageWidth, 0 ), BitConverter.ToUInt32( ImageHeight, 0 ) ) );
				}
			}

			// DateTimeOriginal
			{
				byte[] DateTimeOriginal = GetValue( 0x9003 );
				if( DateTimeOriginal != null )
				{
					InfoList.Add( "Date: " + DateTime.ParseExact( AsString( DateTimeOriginal ), "yyyy:MM:dd HH:mm:ss", null ).ToString() );
				}
			}

			// Make (maker)
			{
				byte[] Make = GetValue( 0x010f );
				if( Make != null )
				{
					InfoList.Add( "Maker: " + AsString( Make ) );
				}
			}

			// Model
			{
				byte[] Model = GetValue( 0x0110 );
				if( Model != null )
				{
					InfoList.Add( "Model: " + AsString( Model ) );
				}
			}

			// FNumber
			{
				byte[] FNumber = GetValue( 0x829d );
				if( FNumber != null )
				{
					InfoList.Add( String.Format( "Aperture Value: f/{0:F1}", AsRational( FNumber ) ) );
				}
			}

			// MaxApertureValue
			{
				byte[] MaxApertureValue = GetValue( 0x9205 );
				if( MaxApertureValue != null )
				{
					InfoList.Add( String.Format( "Maximum Aperture Value: f/{0:F1}", AsRational( MaxApertureValue ) ) );
				}
			}

			// ExposureTime
			{
				byte[] ExposureTime = GetValue( 0x829a );
				if( ExposureTime != null )
				{
					uint dividend = BitConverter.ToUInt32( ExposureTime, 0 );
					uint divisor = BitConverter.ToUInt32( ExposureTime, 4 );

					// over 1sec
					if( dividend >= divisor )
					{
						InfoList.Add( String.Format( "Shutter Speed: {0} sec", ( double ) dividend / divisor ) );
					}
					// less than 1sec
					// 2/3sec とかだとおかしくなる。
					else
					{
						uint upper, lower;
						// 分母の小数点以下2桁まで0なら切り捨てて1/xとする
						if( ( divisor / dividend ) * 100.0 % 100 == 0 )
						{
							upper = 1;
							lower = ( uint ) ( ( double ) divisor / dividend );
						}
						else
						{
							uint gcd = GCD( dividend, divisor );

							upper = dividend / gcd;
							lower = divisor / gcd;
						}
						InfoList.Add( String.Format( "Shutter Speed: {0}/{1} sec", upper, lower ) );
					}
				}
			}

			// ISO
			{
				byte[] ISOSpeedRatings = GetValue( 0x8827 );
				if( ISOSpeedRatings != null )
				{
					InfoList.Add( String.Format( "Film Speed: ISO {0}", BitConverter.ToUInt16( ISOSpeedRatings, 0 ) ) );
				}
			}

			// ExposureBiasValue
			{
				byte[] ExposureBiasValue = GetValue( 0x9204 );
				if( ExposureBiasValue != null )
				{
					int dividend = BitConverter.ToInt32( ExposureBiasValue, 0 );
					uint divisor = BitConverter.ToUInt32( ExposureBiasValue, 4 );

					if( dividend == 0 )
					{
						InfoList.Add( "Exposure Bias: +/- 0 EV" );
					}
					else
					{
						uint gcd = GCD( ( uint ) Math.Abs( dividend ), divisor );
						if( gcd != 1 ) { dividend /= ( int ) gcd; divisor /= gcd; }

						if( Math.Abs( dividend ) == divisor )
						{
							InfoList.Add( String.Format( "Exposure Bias: {0}EV", dividend ) );
						}
						else
						{
							InfoList.Add( String.Format( "Exposure Bias: {0}/{1} EV", dividend, divisor ) );
						}
					}
				}
			}

			// FocalLength
			double focal_len = 0.0;
			{
				byte[] FocalLength = GetValue( 0x920a );
				if( FocalLength != null )
				{
					focal_len = AsRational( FocalLength );
					InfoList.Add( String.Format( "Focal Length: {0:F1} mm", focal_len ) );
				}
			}

			// FocalLengthIn35mmFormat
			{
				byte[] FocalLengthIn35mmFormat = GetValue( 0xa405 );
				if( FocalLengthIn35mmFormat != null )
				{
					UInt16 focal_len_in35mm = BitConverter.ToUInt16( FocalLengthIn35mmFormat, 0 );
					if( Math.Abs( focal_len_in35mm - focal_len ) >= 1.0 )
					{
						InfoList.Add( String.Format( "35mm Equivalent Focal Length: {0} mm", focal_len_in35mm ) );
					}
				}
			}

			// ColorSpace
			{
				byte[] ColorSpace = GetValue( 0xa001 );
				if( ColorSpace != null )
				{
					int flag = BitConverter.ToUInt16( ColorSpace, 0 );
					if( flag == 1 ) { InfoList.Add( "Color Space: sRGB" ); }
					else if( flag == 2 ) { InfoList.Add( "Color Space: Adobe RGB" ); }
				}
			}

			//	0x9209,	// Flash int16u

			string InfoString = "";
			foreach( string s in InfoList )
			{
				InfoString += s + "\n";
			}

			InfoString = InfoString.TrimEnd( new char[] { ',', ' ' } );

			// Measure drawing area
			// 5% padding
			int paddingX = ( int ) ( targetRect.Width * 0.05 );
			int paddingY = ( int ) ( targetRect.Height * 0.05 );

			Rectangle destRect = new Rectangle
				( targetRect.Left + paddingX - this.origin.X
				, targetRect.Top + paddingY - this.origin.Y
				, targetRect.Width - paddingX * 2
				, targetRect.Height - paddingY * 2 );

			StringFormat formatOptions = new StringFormat();

			formatOptions.Alignment = StringAlignment.Far;
			formatOptions.LineAlignment = StringAlignment.Far;

			SizeF area = g.MeasureString( InfoString, Properties.Settings.Default.ExifFontFace, destRect.Size, formatOptions );

			// Draw Base Rect
			// Expand region by 2%
			Color fontBG = Color.FromArgb( 128, Properties.Settings.Default.BackgroundColor );
			g.FillRectangle
				( new SolidBrush( fontBG )
				, targetRect.Left + targetRect.Width - paddingX - area.Width * 1.02F - this.origin.X
				, targetRect.Top + targetRect.Height - paddingY - area.Height * 1.02F - this.origin.Y
				, area.Width * 1.04F, area.Height * 1.04F );

			// Draw
			g.DrawString
				( InfoString
				, Properties.Settings.Default.ExifFontFace
				, new SolidBrush( Properties.Settings.Default.ExifFontColor )
				, destRect
				, formatOptions );
		}

		void OnTimer( string nextFile )
		{
			// 次のframe（以前に準備済み）があるか？
			while( !this.backFrameReady || this.backFrame == null )
			{
				// 作業完了まで待ってあげる
				Debug.Assert( this.bgwPictureLoader.IsBusy );

				// ...
				Application.DoEvents();

				Debug.WriteLine( "Waiting for PictureLoader..." );
			}

			// 現在のframeは要らない
			if( this.currentFrame != null )
			{
				this.currentFrame.Dispose();
				this.currentFrame = null;
			}

			// あったら更新
			this.currentFrame = this.backFrame;

			// 時計も更新
			if( Properties.Settings.Default.ClockMode >= 1 )
			{
				// 色を更新
				this.clockColor1 = this.clockBackColor1;
				this.clockColor2 = this.clockBackColor2;

				// 新しい色で描き直す
				if( this.clockFrame != null )
				{
					this.clockFrame.Dispose(); this.clockFrame = null;
				}
				this.clockFrame = DrawClock( this.clockTargetTime );
			}

			// refresh
			this.Invalidate();
			this.Update();

			// 作業中でなければ次のframeを準備してもらう
			if( !this.bgwPictureLoader.IsBusy )
			{
				// 次のframeをworkerに準備させる
				this.backFrameReady = false;
				this.bgwPictureLoader.RunWorkerAsync( nextFile );
			}

			// 次の秒の時計も更新する
			if( Properties.Settings.Default.ClockMode >= 1 )
			{
				if( this.backClockFrame != null )
				{
					this.backClockFrame.Dispose(); this.backClockFrame = null;
				}
				this.backClockFrame = DrawClock( this.backClockTime );
			}
		}

		private void timerPicChange_Tick( object sender, EventArgs e )
		{
			this.OnTimer( this.GetNextFile() );
		}

		private void Saver_Paint( object sender, PaintEventArgs e )
		{
			// draw current frame
			if( this.currentFrame != null )
			{
				// 壁紙を描画する
				e.Graphics.DrawImageUnscaled( this.currentFrame, 0, 0, this.currentFrame.Width, this.currentFrame.Height );
			}

			// 時計を描画する
			if( this.clockFrame != null )
			{
				e.Graphics.DrawImageUnscaled( this.clockFrame, new Point( this.clockTargetRect.X, this.clockTargetRect.Y ) );
			}

			//Debug.WriteLine( "OnPaint(): now=" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss.fffffff" ) );
		}

		private void Saver_MouseMove( object sender, MouseEventArgs e )
		{
			if( this.mouseLocation.X >= 0 )
			{
				if( ( Math.Abs( MousePosition.X - mouseLocation.X ) > 10 ) ||
					( Math.Abs( MousePosition.Y - mouseLocation.Y ) > 10 ) )
				{
#if !DEBUG
					// exit
					this.Close();
#endif
				}
			}

			mouseLocation = MousePosition;
		}

		private void Saver_MouseDown( object sender, MouseEventArgs e )
		{
			this.Close();
		}

		private void Saver_Deactivate( object sender, EventArgs e )
		{
#if !DEBUG
			this.Close();
#endif
		}

		private void Saver_KeyDown( object sender, KeyEventArgs e )
		{
			Debug.WriteLine( String.Format( "KeyChar is {0}", e.KeyCode ) );
			switch( e.KeyCode )
			{
			case Keys.Right:
			case Keys.PageDown:
				this.timerPicChange.Stop();
				this.OnTimer( this.GetNextFile() );
				this.timerSuspendTimeout.Start();
				break;
			case Keys.Left:
			case Keys.PageUp:
				this.timerPicChange.Stop();
				this.OnTimer( this.GetPrevFile() );
				this.timerSuspendTimeout.Start();
				break;
			case Keys.Enter:
				if( !this.timerPicChange.Enabled )
				{
					this.timerPicChange.Start();
				}
				break;
			default:
				this.Close();
				break;
			}
		}

		private void timerSuspendTimeout_Tick( object sender, EventArgs e )
		{
			this.timerSuspendTimeout.Stop();
			this.timerPicChange.Start();
		}

		// 時計の処理

		// 時計を描画する
		Bitmap DrawClock( DateTime d )
		{
			var newClockFrame = new Bitmap( this.clockTargetRect.Width, this.clockTargetRect.Height, PixelFormat.Format32bppArgb );
			this.clockDrawer.BuildClock
				( Graphics.FromImage( newClockFrame )
				, new Rectangle( 0, 0, this.clockTargetRect.Width, this.clockTargetRect.Height )
				, d
				, this.clockColor1, this.clockColor2 );
			return newClockFrame;
		}

		// 「今の秒」になった瞬間に呼ばれる
		// 呼ばれたら置いてある時計画像を描画する（というか画面をRefreshして描画させる）
		//   ^^これに掛かる時間を測定しておく。
		// 「次の秒の」時計画像を生成し、置いておく。
		// 次の秒までの時間を測定し、timerClockの次の呼び出し時刻を決定する。
		//   ^^このとき、以前に再描画に要した時間を参考に、描画完了＝次の秒開始 となるように制御する。
		private void timerClock_Tick( object sender, EventArgs e )
		{
			// 一旦止める
			this.timerClock.Stop();

			// 再描画所要時間 測定開始
			Stopwatch sw = Stopwatch.StartNew();

			// 用意していたものを、なる早で再描画する
			if( this.clockFrame != null )
			{
				// 現在のものは破棄
				this.clockFrame.Dispose();
				this.clockFrame = null;
			}
			this.clockFrame = this.backClockFrame;
			this.clockTargetTime = this.backClockTime;
			// これで再描画される
			this.Refresh();

			// 測定終了
			sw.Stop();

			// 過去N個の移動平均とする
			// 追加
			this.clockRefleshTime.Add( sw.ElapsedMilliseconds );
			// 最大N個保持する
			if( this.clockRefleshTime.Count > 10 )
			{
				// 一番前のものを削除
				this.clockRefleshTime.RemoveAt( 0 );
			}
			// 平均を取る
			int delay_avg = ( int ) this.clockRefleshTime.Average();

			//debug
			var format = "yyyy-MM-dd HH:mm:ss.fffffff";

			// 時計を作りたい時刻
			DateTime next = DateTime.Now;

			// 描画したい時間が過ぎていなければ、フレームを作り直しても意味がない
			// ので「次の秒」を目標とする
			var timeAhead = next - this.backClockTime;
			if( timeAhead.Ticks < 0 )
			{
				// 多分ほんの数ms
				Debug.WriteLine( "Time ahead=" + -timeAhead.Milliseconds );
				next = this.backClockTime.AddSeconds( 1.0 );
			}
			else
			{
				// 既にほしい時間が過ぎていたら
				// 秒以下を切り捨てる
				next = next.AddTicks( -( next.Ticks % TimeSpan.FromSeconds( 1 ).Ticks ) ).AddSeconds( 1.0 );
			}

			// 時計フレームを作成
			this.backClockFrame = DrawClock( next );

			// 初回か？
			if( this.clockFrame == null )
			{
				this.timerClock.Interval = 1;

				Debug.WriteLine( "initial call" );
			}
			else
			{
				// 次回までの待ち時間
				DateTime now = DateTime.Now;
				var diff = ( next - now ).TotalMilliseconds - delay_avg;
				this.timerClock.Interval = Math.Max( ( int ) diff, 1 );

				Debug.WriteLine
					( "target time=" + next.ToString( format )
					+ ",now=" + now.ToString( format )
					+ ",diff=" + ( next - now ).ToString()
					+ ",delay_avg=" + delay_avg.ToString()
					+ ",interval=" + diff.ToString() );
			}

			// 目標時刻を覚える
			this.backClockTime = next;

			// 再開
			this.timerClock.Start();
		}

		// 次のpictureをloadするworker
		private void bgwPictureLoader_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
		{
			// load完了！
			// この関数はメインスレッドのcontextで呼ばれる（はず。そうでないと意味がない）
			if( this.backFrame != null )
			{
				this.backFrameReady = true;
			}
		}

		// e.Argument = (string) file to load
		private void bgwPictureLoader_DoWork( object sender, DoWorkEventArgs e )
		{
			string nextFile = ( string ) e.Argument;

			this.LoadPicture( nextFile );
		}
	}
}
