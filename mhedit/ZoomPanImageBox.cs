/* 
 * Developed by Shannon Young.  http://www.smallwisdom.com
 * Copyright 2005
 * 
 * You are welcome to use, edit, and redistribute this code.
 * 
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Smallwisdom.Windows.Forms
{
	/// <summary>
	/// ZoomPanImageBox is a specialized ImageBox with Pan and Zoom control.
	/// </summary>
	public class ZoomPanImageBox : System.Windows.Forms.UserControl
	{
        private const float ZOOM_MIN = 0.0f;
        private const float ZOOM_MAX = 3.0f;
        private const int EDGE_BUFFER = 100;
        private float zoom;
        private System.Windows.Forms.Panel imagePanel;
        private mhedit.PictureBoxEx pictureBox;


		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region Construct, Dispose

		public ZoomPanImageBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            ///this.imagePanel.MouseClick += new MouseEventHandler(ControlMouseClick);
            //this.pictureBox.MouseClick += new MouseEventHandler(ControlMouseClick);
            //this.imagePanel.KeyDown += new KeyEventHandler(ControlKeyDown);
            this.pictureBox.KeyDown += new KeyEventHandler(ControlKeyDown);
            this.SetStyle(ControlStyles.Selectable | ControlStyles.StandardClick, true);
		}

        /// <summary>
        /// Ensure that it is obvious when this control is focused.
        /// </summary>
        protected override bool ShowFocusCues
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Change the backgound colour to highlight the focus on this control
        /// </summary>
        /// <param name="e">Contains the event data</param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            this.imagePanel.BackColor = Color.Red;
            this.pictureBox.BackColor = SystemColors.Highlight;
        }

        /// <summary>
        /// Remove the Focus Indication
        /// </summary>
        /// <param name="e">Contains the event data</param>
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.imagePanel.BackColor = Color.Blue;
            this.pictureBox.BackColor = this.BackColor;
        }

        /// <summary>
        /// Ensure that this control focuses when clicked
        /// </summary>
        /// <param name="e">Contains the event data</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Focus();
        }

        /// <summary>
        /// Ensure that this control focuses when clicked
        /// </summary>
        /// <param name="e">Contains the event data</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.Focus();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Ensure that when the contained controls are clicked that this control gets the focus
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Contains the event data</param>
        private void control_Click(object sender, System.EventArgs e)
        {
            base.OnClick(e);
            this.Focus();
        }

        /// <summary>
        /// Ensure that when the contained controls are clicked that this control gets the focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Contains the event data</param>
        private void control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

                base.OnMouseDown(e);
                //this.Focus();

        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.imagePanel = new System.Windows.Forms.Panel();
            this.pictureBox = new mhedit.PictureBoxEx();
            this.imagePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imagePanel
            // 
            this.imagePanel.AutoScroll = true;
            this.imagePanel.Controls.Add(this.pictureBox);
            this.imagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(824, 608);
            this.imagePanel.TabIndex = 7;
            this.imagePanel.Click += new System.EventHandler(this.control_Click);
            this.imagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(3, 3);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(352, 162);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.DragOver += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragOver);
            this.pictureBox.Click += new System.EventHandler(this.control_Click);
            this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.control_MouseDown);
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.control_MouseClick);
            // 
            // ZoomPanImageBox
            // 
            this.Controls.Add(this.imagePanel);
            this.Name = "ZoomPanImageBox";
            this.Size = new System.Drawing.Size(824, 608);
            this.imagePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion


        private void ControlMouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        private void ControlKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

		/// <summary>
		/// Image loaded into the box.
		/// </summary>
		[Browsable(true),
		Description("Image loaded into the box.")]
		public Image Image
		{
			get
			{
				return pictureBox.Image;
			}
			set
			{
				// Set the image value
                zoom = 1.0f;
                pictureBox.Image = value;
                
                if (value != null)
				{
					// Initially, the zoom factor is 100% so set the
					// ImageBox size equal to the Image size.
                    pictureBox.Width = value.Size.Width;
                    pictureBox.Height = value.Size.Height;
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;

                    // Now, center the image in the panel
                    pictureBox.Left = (imagePanel.Width - pictureBox.Width) / 2;
                    pictureBox.Top = (imagePanel.Height - pictureBox.Height) / 2;
				}
				else
				{
					// If null image, then reset the imgBox size
					// to the size of the panel so that there are no
					// scroll bars.
                    pictureBox.BorderStyle = BorderStyle.None;
					pictureBox.Size = imagePanel.Size;
				}
			}
		}

        public bool PointInImage(Point loc)
        {
            if ((loc.X >= pictureBox.Left) && (loc.X <= (pictureBox.Left + pictureBox.Width))
                && (loc.Y >= pictureBox.Top) && (loc.Y <= (pictureBox.Top + pictureBox.Height)))
            {
                return true;
            }
            return false;
        }

        public Point PointToImageClient(Point loc)
        {
            return new Point(loc.X - pictureBox.Left, loc.Y - pictureBox.Top);
        }

        public float Zoom
        {
            get { return zoom; }
            set
            {
                if (zoom <= ZOOM_MAX && zoom > ZOOM_MIN)
                {
                    zoom = value;
                    setZoom();
                }
            }
        }

		private void scrollZoom_Scroll(object sender, System.EventArgs e)
		{
			setZoom();
		}

        private void setZoom()
		{
            if (pictureBox.Image != null)
            {
                // Set the ImageBox width and height to the new zoom
                // factor by multiplying the Image inside the Imagebox
                // by the new zoom factor.
                pictureBox.Width = Convert.ToInt32(pictureBox.Image.Width * zoom);
                pictureBox.Height = Convert.ToInt32(pictureBox.Image.Height * zoom);

                // Now, center the image in the panel
                pictureBox.Left = (imagePanel.Width - pictureBox.Width) / 2;
                pictureBox.Top = (imagePanel.Height - pictureBox.Height) / 2;
            }
		}

        private void control_MouseClick(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            this.Focus();
        }

        private void pictureBox_DragOver(object sender, DragEventArgs e)
        {
            base.OnDragOver(e);
        }



	}// end class
}// end namespace
