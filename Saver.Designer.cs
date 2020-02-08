namespace digital_photo_frame_screen_saver
{
	partial class Saver
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
			this.components = new System.ComponentModel.Container();
			this.timerPicChange = new System.Windows.Forms.Timer(this.components);
			this.timerSuspendTimeout = new System.Windows.Forms.Timer(this.components);
			this.timerClock = new System.Windows.Forms.Timer(this.components);
			this.bgwPictureLoader = new System.ComponentModel.BackgroundWorker();
			this.SuspendLayout();
			// 
			// timerPicChange
			// 
			this.timerPicChange.Tick += new System.EventHandler(this.timerPicChange_Tick);
			// 
			// timerSuspendTimeout
			// 
			this.timerSuspendTimeout.Tick += new System.EventHandler(this.timerSuspendTimeout_Tick);
			// 
			// timerClock
			// 
			this.timerClock.Interval = 1;
			this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
			// 
			// bgwPictureLoader
			// 
			this.bgwPictureLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPictureLoader_DoWork);
			this.bgwPictureLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPictureLoader_RunWorkerCompleted);
			// 
			// Saver
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(389, 338);
			this.ControlBox = false;
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "Saver";
			this.ShowInTaskbar = false;
			this.Text = "Digital Photo Frame Screen Saver";
			this.Deactivate += new System.EventHandler(this.Saver_Deactivate);
			this.Load += new System.EventHandler(this.Saver_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Saver_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Saver_KeyDown);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Saver_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Saver_MouseMove);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerPicChange;
		private System.Windows.Forms.Timer timerSuspendTimeout;
		private System.Windows.Forms.Timer timerClock;
		private System.ComponentModel.BackgroundWorker bgwPictureLoader;
	}
}