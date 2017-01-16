using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace mhedit
{
    /// <summary>
    /// ZoomPicBox does what it says on the wrapper.
    /// </summary>
    /// <remarks>
    /// PictureBox doesn't lend itself well to overriding. Why not start with something basic and do the job properly?
    /// </remarks>
    public class ZoomPictureBox : ScrollableControl
    {
        Image _image;
        float _zoom = 1.0f;

        public ZoomPictureBox()
        {
            //Double buffer the control
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer |
              ControlStyles.Selectable |
              ControlStyles.StandardClick |
              ControlStyles.ContainerControl, true);

            this.AutoScroll = true;
            
        }

        [Category("Appearance"),
        Description("The image to be displayed")]
        public Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                _zoom = 1.0f;
                UpdateScaleFactor();
                Invalidate();
            }
        }

        
        [Category("Appearance"),
        Description("The zoom factor. Less than 1 to reduce. More than 1 to magnify.")]

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (value < 0 || value < 0.00001) value = 0.00001f;
                _zoom = value;
                UpdateScaleFactor();
                Invalidate();
            }
        }

        /// <summary>
        /// Calculates the effective size of the image
        ///after zooming and updates the AutoScrollSize accordingly
        /// </summary>

        private void UpdateScaleFactor()
        {
            if (_image == null)
                this.AutoScrollMinSize = this.Size;
            else
            {
                this.AutoScrollMinSize = new Size(
                  (int)(this._image.Width * _zoom + 0.5f),
                  (int)(this._image.Height * _zoom + 0.5f)
                  );
            }
        }

        InterpolationMode _interpolationMode = InterpolationMode.High;
        [Category("Appearance"),
        Description("The interpolation mode used to smooth the drawing")]
        public InterpolationMode InterpolationMode
        {
            get { return _interpolationMode; }
            set { _interpolationMode = value; }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // do nothing.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            //if no image, don't bother
            if (_image == null)
            {
                return;
            }
            //Set up a zoom matrix
            Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0);
            //now translate the matrix into position for the scrollbars
            mx.Translate(this.AutoScrollPosition.X / _zoom, this.AutoScrollPosition.Y / _zoom);
            //use the transform
            e.Graphics.Transform = mx;
            //and the desired interpolation mode
            e.Graphics.InterpolationMode = _interpolationMode;
            //Draw the image ignoring the images resolution settings.
            int newWidth = (int)(this._image.Width * _zoom);
            int newHeight = (int)(this._image.Height * _zoom);
            e.Graphics.DrawImage(_image, new Rectangle((this.Width - newWidth) / 2, (this.Height - newHeight) / 2, this._image.Width, this._image.Height), 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel);
            base.OnPaint(e);
        }


    }
}


