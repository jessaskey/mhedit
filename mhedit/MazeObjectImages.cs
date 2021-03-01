using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.MazeObjects;

namespace mhedit
{
    public static class MazeObjectImages
    {
        public static ImageList List { get; } = new ImageList();

        static MazeObjectImages()
        {
            List.ImageSize = new Size( 32, 32 );

            List.Images.Add( typeof(MazeWall).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_horizontal_32.png" ) );
            List.Images.Add( ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_leftup_32.png" ) );
            List.Images.Add( ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_rightup_32.png" ) );
            List.Images.Add( ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_leftdown_32.png" ) );
            List.Images.Add( ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_rightdown_32.png" ) );
            List.Images.Add( ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_vertical_32.png" ) );
            List.Images.Add( ResourceFactory.GetResourceImage( "mhedit.images.buttons.wall_empty_32.png" ) );
            List.Images.Add( typeof( Pyroid ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.pyroid_32.png" ) );
            List.Images.Add( typeof( Reactoid ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.reactoid_32.png" ) );
            List.Images.Add( typeof( Perkoid ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.perkoid_32.png" ) );
            List.Images.Add( typeof( Maxoid ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.roboid_32.png" ) );
            List.Images.Add( typeof( Oxoid ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.oxoid_32.png" ) );
            List.Images.Add( typeof( Key ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.key_32.png" ) );
            List.Images.Add( typeof( Arrow ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.arrow_32.png" ) );
            List.Images.Add( typeof( LightningH ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.lightning_32.png" ) );
            List.Images.Add( typeof( LightningV ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.lightning_v_32.png" ) );
            List.Images.Add( typeof( Clock ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.clock_32.png" ) );
            List.Images.Add( typeof( IonCannon ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.cannon_32.png" ) );
            List.Images.Add( typeof( OneWay ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.oneway_32.png" ) );
            List.Images.Add( typeof( TripPad ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.trippad_32.png" ) );
            List.Images.Add( typeof( Lock ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.lock_32.png" ) );
            List.Images.Add( typeof( Hand ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.hand_32.png" ) );
            List.Images.Add( typeof( Spikes ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.spikes_32.png" ) );
            List.Images.Add( typeof( Boots ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.booties_32.png" ) );
            List.Images.Add( typeof( Transporter ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.transporter_32.png" ) );
            List.Images.Add( typeof( EscapePod ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.pod_32.png" ) );
            List.Images.Add( typeof( Maxoid ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.roboid_32.png" ) );
            List.Images.Add( typeof( ArrowOut ).Name,
                ResourceFactory.GetResourceImage( "mhedit.images.buttons.arrow_out_32.png" ) );
        }
    }
}
