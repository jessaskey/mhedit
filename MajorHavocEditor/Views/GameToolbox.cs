using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeEnemies.IonCannon;
using mhedit.Containers.MazeObjects;

namespace MajorHavocEditor.Views
{

    public partial class GameToolbox : UserControl, IUserInterface
    {
        private static readonly ImageList ImageList;

        static GameToolbox()
        {
            ImageList =
                new ImageList
                    {
                        ImageSize = new Size( 32, 32 )
                    }
                    .AddImages( new[]
                                {
                                    "arrow_32.png",
                                    "arrow_out_32.png",
                                    "booties_32.png",
                                    "cannon_32.png",
                                    "clock_32.png",
                                    "hand_32.png",
                                    "keypouch_32.png",
                                    "key_32.png",
                                    "lightning_32.png",
                                    "lightning_v_32.png",
                                    "lock_32.png",
                                    "oneway_32.png",
                                    "oxoid_32.png",
                                    "perkoid_32.png",
                                    "pod_32.png",
                                    "pyroid_32.png",
                                    "reactoid_32.png",
                                    "roboid_32.png",
                                    "spikes_32.png",
                                    "token_32.png",
                                    "transporter_32.png",
                                    "trippad_32.png",
                                    "wall_empty_32.png",
                                    "wall_horizontal_32.png",
                                    "wall_leftdown_32.png",
                                    "wall_leftup_32.png",
                                    "wall_rightdown_32.png",
                                    "wall_rightup_32.png",
                                    "wall_vertical_32.png",
                                } )
                    .WithResourcePath( "Resources/Images/Toolbox" )
                    .Load();
        }

        public GameToolbox()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );

            toolBox.AllowSwappingByDragDrop = false;

            toolBox.SmallImageList = ImageList;

            int tabIndex;

            tabIndex = toolBox.AddTab( "Maze Walls", -1 );
            toolBox[ tabIndex ].AddItem( "Horizontal", 23, true, new MazeWall( MazeWallType.Horizontal ) );
            toolBox[ tabIndex ].AddItem( "LeftUp", 25, true, new MazeWall( MazeWallType.LeftUp ) );
            toolBox[ tabIndex ].AddItem( "RightUp", 27, true, new MazeWall( MazeWallType.RightUp ) );
            toolBox[ tabIndex ].AddItem( "LeftDown", 24, true, new MazeWall( MazeWallType.LeftDown ) );
            toolBox[ tabIndex ].AddItem( "RightDown", 26, true, new MazeWall( MazeWallType.RightDown ) );
            toolBox[ tabIndex ].AddItem( "Vertical", 28, true, new MazeWall( MazeWallType.Vertical ) );
            toolBox[ tabIndex ].AddItem( "Empty", 22, true, new MazeWall( MazeWallType.Empty ) );
 
            tabIndex = toolBox.AddTab( "Maze Enemies", -1 );
            toolBox[ tabIndex ].AddItem( "Pyroid", 15, true, new Pyroid() );
            toolBox[ tabIndex ].AddItem( "Perkoid", 13, true, new Perkoid() );
            toolBox[ tabIndex ].AddItem( "Maxoid", 17, true, new Maxoid() );
            toolBox[ tabIndex ].AddItem( "Force Field", 8, true, new LightningH() );
            toolBox[ tabIndex ].AddItem( "Force Field", 9, true, new LightningV() );
            toolBox[ tabIndex ].AddItem( "Ion IonCannon", 3, true, new IonCannon() );
            toolBox[ tabIndex ].AddItem( "Trip Pad", 21, true, new TripPad() );

            tabIndex = toolBox.AddTab( "Maze Objects", -1 );
            toolBox[ tabIndex ].AddItem( "Reactoid", 16, true, new Reactoid() );
            toolBox[ tabIndex ].AddItem( "Arrow", 0, true, new Arrow() );
            toolBox[ tabIndex ].AddItem( "Out Arrow", 1, true, new ArrowOut() );
            toolBox[ tabIndex ].AddItem( "Oxoid", 12, true, new Oxoid() );
            toolBox[ tabIndex ].AddItem( "One Way", 11, true, new OneWay() );
            toolBox[ tabIndex ].AddItem( "Stalactites", 18, true, new Spikes() );
            toolBox[ tabIndex ].AddItem( "Transporter", 20, true, new Transporter() );
            toolBox[ tabIndex ].AddItem( "Booties", 2, true, new Boots() );
            toolBox[ tabIndex ].AddItem( "KeyPouch", 6, true, new KeyPouch() );
            toolBox[ tabIndex ].AddItem( "Lock", 10, true, new Lock() );
            toolBox[ tabIndex ].AddItem( "Key", 7, true, new Key() );
            toolBox[ tabIndex ].AddItem( "De Hand", 5, true, new Hand() );
            toolBox[ tabIndex ].AddItem( "Clock", 4, true, new Clock() );
            toolBox[ tabIndex ].AddItem( "Escape Pod", 14, true, new EscapePod() );
            toolBox[ tabIndex ].AddItem( "Hidden Level Token", 19, true, new HiddenLevelToken() );

            toolBox.SelectedTabIndex = 2;
        }

#region Overrides of Control

        public override Size GetPreferredSize( Size proposedSize )
        {
            return this.Size;
        }

#endregion

#region Implementation of IUserInterface

        public DockingState DockingState
        {
            get { return DockingState.DockRight; }
        }

        public DockingPosition DockingPositions
        {
            get
            {
                return DockingPosition.Left |
                    DockingPosition.Right |
                    DockingPosition.Top |
                    DockingPosition.Bottom;
            }
        }

        public bool HideOnClose
        {
            get { return true; }
        }

        public string Caption
        {
            get { return "Toolbox"; }
        }

        public object Icon
        {
            get { return null; }
        }

#endregion

    }

}
