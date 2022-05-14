using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace mhedit.Containers
{

    public static class ResourceFactory
    {
        [ Flags() ]
        public enum ImageEffect : int
        {
            Buttonize = 1,
            BackgroundBlack = 2
        }

        public static Image ApplyImageEffect( Image img, ImageEffect effect )
        {
            if ( img != null )
            {
                Graphics g = Graphics.FromImage( img );

                if ( ( ImageEffect.Buttonize & effect ) > 0 )
                {
                    Pen borderPen = new Pen( Color.White, 2 );
                    g.DrawLine( borderPen, 0, 0, 0, img.Height );
                    g.DrawLine( borderPen, 0, 0, img.Width, 0 );
                    g.DrawLine( borderPen, img.Width - 3, 0, img.Width - 3, img.Height - 3 );
                    g.DrawLine( borderPen, 0, img.Height - 3, img.Width - 3, img.Height - 3 );
                }

                if ( ( ImageEffect.BackgroundBlack & effect ) > 0 )
                {

                }
            }

            return img;
        }

        public static Image GetResourceImage( string resourceName )
        {
            Image bmp = null;
            Assembly a = Assembly.GetCallingAssembly(); // GetExecutingAssembly();
            string[] resNames = a.GetManifestResourceNames();
            Stream imgStream = a.GetManifestResourceStream( resourceName );

            if ( imgStream != null )
            {
                bmp = new Bitmap( imgStream );
                bmp.RotateFlip( RotateFlipType.RotateNoneFlipX );
                bmp.RotateFlip( RotateFlipType.RotateNoneFlipX );
            }

            return bmp;
        }

        public static Image ReplaceColor( Image img, Color originalColor, Color newColor )
        {
            if ( originalColor != newColor )
            {
                //Create bitmap
                Bitmap myImage = (Bitmap) img.Clone();
                ImageAttributes imageAttributes = new ImageAttributes();

                // Create mappings
                ColorMap colorMap = new ColorMap();
                colorMap.OldColor = originalColor;
                colorMap.NewColor = newColor;
                ColorMap[] remapTable = { colorMap };
                imageAttributes.SetRemapTable( remapTable, ColorAdjustType.Bitmap );

                // Draw new picture
                Graphics screenGraphics = Graphics.FromImage( img );
                screenGraphics.Clear( Color.Transparent );

                screenGraphics.DrawImage( myImage,
                    new Rectangle( 0, 0, myImage.Width,
                        myImage.Height ), //Set the detination Position
                    0, // x-coordinate of the portion of the source image to draw.
                    0, // y-coordinate of the portion of the source image to draw.
                    myImage.Width, // Watermark Width
                    myImage.Height, // Watermark Height
                    GraphicsUnit.Pixel, // Unit of measurment
                    imageAttributes ); //ImageAttributes Object

                return img;
            }

            return img;

        }

    }
}