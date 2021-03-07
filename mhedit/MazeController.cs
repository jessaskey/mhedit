using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Silver.UI;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers;

namespace mhedit
{
	[DefaultPropertyAttribute("Name")]
	[Serializable]
	public partial class MazeController : UserControl, ITreeObject, ICustomTypeDescriptor, IChangeTracking
	{
#region Declarations

		public const int MAXWALLS = 209;

		/// This is the "padding" around the border of the entire maze.
		//private const int STAMPS_TRIM_LEFT = 3;
		//private Point objectOffset = new Point(-16, 16);

		private Maze _maze;
		private decimal _zoom = 1;
        private Point _mouseDownLocation;
		private bool _repainted = false;
		private string _fileName = String.Empty;
		private PropertyGrid _propertyGrid;
		private ComboBox _comboBoxObjects;
		private bool _gridLines = false;
		private string _lastError = string.Empty;

        private ObservableCollection<MazeObject> _selectedObjects =
            new ObservableCollection<MazeObject>();

#endregion

		#region PropertyIncludes

		private string[] NamesToInclude =
		{
          "Name",
          "MazeType",
          "FileName",
          "Hint",
          "Hint2"
		};

		#endregion

		#region Constructors

		public MazeController()
			: this( NameFactory.Create( "Maze" ) )
		{ }

		public MazeController( string name )
			: this( new Maze( name ) )
		{ }

		public MazeController( MazeType type, string name )
			: this( new Maze( type, name ) )
		{ }

		public MazeController( Maze maze )
		{
            this.InitializeComponent();

            this.ContextMenuStrip = this.mazeControllerContextMenu;

            this.selectAllToolStripMenuItem.DropDown.ImageList = MazeObjectImages.List;

			this._maze = maze;

			this._maze.PropertyChanged += this.OnMazePropertyChanged;

            this._selectedObjects.CollectionChanged += this.OnSelectedObjectsChanged;

			DoubleBuffered = true;
			SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true );
			UpdateStyles();

			base.Height = ( DataConverter.CanvasGridSize * _maze.MazeStampsY ) + ( DataConverter.PADDING * 2 );
			base.Width = ( DataConverter.CanvasGridSize * _maze.MazeStampsX ) + ( DataConverter.PADDING * 2 );

			//event methods
			//base.AllowDrop = true;
			//base.TabStop = true;

			//DragOver += new DragEventHandler(Maze_DragOver);
		}

        private void OnSelectedObjectsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (MazeObject mazeObject in this.Maze.MazeObjects)
                {
                    mazeObject.Selected = false;
                }
            }
			else if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (MazeObject mazeObject in e.NewItems)
                {
                    mazeObject.Selected = true;
                }
            }
			else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (MazeObject mazeObject in e.OldItems)
                {
                    mazeObject.Selected = false;
                }
            }
			else if (e.Action != NotifyCollectionChangedAction.Move)
            {
                throw new NotSupportedException(e.Action.ToString());
            }

			this.Invalidate();
		}

#endregion

		/// <summary>
		/// Because the base class control also has a Name property its easy to
		/// mistakenly call the wrong property... Return the Maze name during
		/// runtime but throw during debug.
		/// </summary>
		public new string Name
		{
			get
			{
#if DEBUG
				throw new NotSupportedException(
					"MazeController.Maze.Name is what you are looking for!" );
#else
				return this._maze.Name;
#endif
			}
		}

#region Public Properties

		[BrowsableAttribute(false)]
		public Maze Maze
		{
			get { return _maze; }
		}

		[BrowsableAttribute(false)]
		public bool ShowGridReferences { get; set; }

		[ReadOnly(true)]
		[DescriptionAttribute("The filename of this maze on disk.")]
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		[BrowsableAttribute(false)]
		public bool GridLines
		{
			get { return _gridLines; }
			set { _gridLines = value; }
		}

		[BrowsableAttribute(false)]
		public PropertyGrid PropertyGrid
		{
			get { return _propertyGrid; }
			set { _propertyGrid = value; }
		}

		[BrowsableAttribute( false )]
		public ComboBox ComboBoxObjects
		{
			get { return _comboBoxObjects; }
			set
			{
				if ( _comboBoxObjects != null )
					_comboBoxObjects.SelectedIndexChanged -= comboBoxObjects_SelectedIndexChanged;

				_comboBoxObjects = value;

				BindComboBoxObjects( null );

				if ( _comboBoxObjects != null )
					_comboBoxObjects.SelectedIndexChanged += comboBoxObjects_SelectedIndexChanged;
			}
		}

		[BrowsableAttribute(false)]
		public decimal Zoom
		{
			get { return _zoom; }
			set { _zoom = value; }
		}

#endregion

#region ITreeObject

		public void SetGridlines(bool grid)
		{
			_gridLines = grid;
		}

#endregion

#region ICustomTypeDescriptor

		private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection pdc)
		{
			ArrayList toInclude = new ArrayList();
			foreach (string s in NamesToInclude)
				toInclude.Add(s); 

			PropertyDescriptorCollection adjustedProps = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			foreach (PropertyDescriptor pd in pdc)
				if (toInclude.Contains(pd.Name))
					adjustedProps.Add(pd);

			return adjustedProps;
		} 

		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this, attributes, true);
			return FilterProperties(pdc);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(this,true);
			return FilterProperties(pdc);
		}

		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

        #endregion

#region Implementation of IChangeTracking

        public void AcceptChanges()
        {
            this._maze.AcceptChanges();
        }

        public bool IsChanged
        {
            get { return this._maze.IsChanged; }
        }

		#endregion

#region Overrides of Control

		protected override void OnPaint(PaintEventArgs e)
		{
			//Stopwatch stopwatch = new Stopwatch();
			//long time = stopwatch.ElapsedMilliseconds;
			//stopwatch.Start();
			//Console.Write("OnPaint Begin\n");

			Pen bigGridPen = new Pen(Color.DimGray, 1);
			Brush referenceBrush = Brushes.Yellow;

			int mazeWidth;
			int mazeHeight;
			int currentStamp;

			//base.DisplayRectangle = new Rectangle(base.Left, base.Top,(int)(base.Width * zoom),(int)(base.Height * zoom));
			base.Height = (int)( DataConverter.CanvasGridSize * _maze.MazeStampsY * _zoom) + (DataConverter.PADDING *2);
			base.Width = (int)(( DataConverter.CanvasGridSize * _maze.MazeStampsX * _zoom) + (DataConverter.PADDING * 2)); // - (GRIDUNITS * STAMPS_TRIM_LEFT * GRIDUNITSTAMPS * _zoom));

			mazeHeight = (int)(base.Height);
			mazeWidth = (int)(base.Width);

			//Console.Write("Init Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
			//stopwatch.Reset();
			//stopwatch.Start();

			Graphics g = e.Graphics;
			//g.RenderingOrigin = objectOffset;
			g.Clear(Color.Black);

			if (GridLines)
			{
				//now draw the major grid lines
				//vertical
				for (int i = 0; i <= mazeWidth; i += (int)( DataConverter.CanvasGridSize * _zoom))
				{
					g.DrawLine(bigGridPen, (int)(i+DataConverter.PADDING), 0, (int)(i+DataConverter.PADDING), mazeHeight);
				}
				//horizontal, start at zero 
				for (int i = 0; i <= mazeHeight; i += (int)( DataConverter.CanvasGridSize * _zoom))
				{
					g.DrawLine(bigGridPen, 0, (int)(i+ DataConverter.PADDING), mazeWidth, (int)(i+DataConverter.PADDING));
				}
			}

			if (ShowGridReferences)
			{
				//now draw the major grid lines
				//X
				int xOffset = -3;
				for (int i = Math.Abs(xOffset); i <= _maze.MazeStampsX; i++)
				{
					int gridValue = i + xOffset;
					g.DrawString(gridValue.ToString("X"), Font, referenceBrush, new Point((i * (int)( DataConverter.CanvasGridSize * _zoom)) + DataConverter.PADDING, 1));
				}
				//Y
				for (int i = 0; i <= _maze.MazeStampsY; i ++)
				{
					int gridValue = ((-i) + 12);
					g.DrawString(gridValue.ToString("X"), Font, referenceBrush, new Point(1, (i * (int)( DataConverter.CanvasGridSize * _zoom)) + DataConverter.PADDING));
				}
			}

			//Console.Write("Grid Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
			//stopwatch.Reset();
			//stopwatch.Start();

			//in order for our user walls to work we need to not draw
			//the base walls if a user wall exists in that stamp
			//we will use the base wall 'selected' property to denote
			//if there is not a wall in that stamp location
			//first reset all 'selected properties to 0
			for (int i = 0; i < _maze.MazeWallBase.Count ; i++)
			{
				if (_maze.MazeWallBase[i] != null)
				{
					_maze.MazeWallBase[i].Selected = false;
				}
			}
			//set wall to 'selected' of there is a user defined wall at that location
			for (int i = 0; i < _maze.MazeObjects.Count; i++)
			{
				MazeObject mazeObject = (MazeObject)_maze.MazeObjects[i];
				if (mazeObject.GetType() == typeof(MazeWall))
				{
					currentStamp = this._maze.PointToStamp(mazeObject.Position);
					if (currentStamp >= 0 && currentStamp < _maze.MazeWallBase.Count)
					{
						if (_maze.MazeWallBase[currentStamp] != null)
						{
							_maze.MazeWallBase[currentStamp].Selected = true;
						}
					}
				}
			}
			//now draw all walls that don't have a user defined wall at that location
			for (int rows = 0; rows < _maze.MazeStampsY; rows++)
			{
				for (int cols = 0; cols < _maze.MazeStampsX; cols++)
				{
					currentStamp = (rows * _maze.MazeStampsX) + cols;
					if (currentStamp < _maze.MazeWallBase.Count)
					{
						if (_maze.MazeWallBase[currentStamp] != null)
						{
							if (_maze.MazeWallBase[currentStamp].Selected == false)
							{
								if (_maze.MazeWallBase[currentStamp].WallType != MazeWallType.Empty)
								{
									Image currentImage = _maze.MazeWallBase[currentStamp].Image;
									if (currentImage != null)
									{
										Image scaledImage = currentImage.GetThumbnailImage((int)(currentImage.Width * _zoom), (int)(currentImage.Height * _zoom), null, System.IntPtr.Zero);
										if (scaledImage != null)
										{
											g.DrawImage(scaledImage, new Point((int)((cols * DataConverter.CanvasGridSize * _zoom)+ DataConverter.PADDING), (int)((rows * DataConverter.CanvasGridSize * _zoom)+ DataConverter.PADDING)));
										}
									}
								}
							}
						}
					}
				}
			}



			//Console.Write("Walls #1 Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
		   // stopwatch.Reset();
			//stopwatch.Start();

			//draw all wall objects
			for (int i = 0; i < _maze.MazeObjects.Count; i++)
			{
				MazeObject mazeObject = (MazeObject)_maze.MazeObjects[i];
				if (mazeObject.GetType() == typeof(MazeWall))
				{
					Image scaledImage = mazeObject.Image.GetThumbnailImage((int)(mazeObject.Image.Width * _zoom), (int)(mazeObject.Image.Height * _zoom), null, System.IntPtr.Zero);
					g.DrawImage(scaledImage, new Point((int)((mazeObject.Position.X * _zoom)+ DataConverter.PADDING), (int)((mazeObject.Position.Y * _zoom)+ DataConverter.PADDING)));
				}
			}

			//Console.Write("Walls #2 Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
			//stopwatch.Reset();
			//stopwatch.Start();

			//draw all non-wall objects
			for (int i = 0; i < _maze.MazeObjects.Count; i++)
			{
				MazeObject mazeObject = (MazeObject)_maze.MazeObjects[i];
				if (mazeObject.GetType() != typeof(MazeWall))
				{
					Image scaledImage = mazeObject.Image.GetThumbnailImage((int)(mazeObject.Image.Width * _zoom), (int)(mazeObject.Image.Height * _zoom), null, System.IntPtr.Zero);
					g.DrawImage( scaledImage, new Point( (int)( ( mazeObject.RenderPosition.X * _zoom ) + DataConverter.PADDING ), (int)( mazeObject.RenderPosition.Y * _zoom ) + DataConverter.PADDING ) );
				}
			}

			_repainted = true;
			//Console.Write("Objects Complete - " + stopwatch.ElapsedMilliseconds.ToString() + "\n");
			//stopwatch.Stop();

			//if (propertyGrid != null) propertyGrid.Refresh();
		}

		/// When inheriting from UserControl we must override this method to forward certain
		/// keystrokes to the maze actions.
		/// Inheriting from UserControl makes editing the context menu MUCH easier in the designer.
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Left:
                case Keys.Tab:
                    return true;
                    break;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Tab:
					//int selectedIndex = GetSelectedObjectIndex();
					//selectedIndex++;
					//selectedIndex %= _maze.MazeObjects.Count;
					//SetSelectedObject(selectedIndex);
					//Invalidate();
					break;
				case Keys.Escape:
                    this._selectedObjects.Clear();
                    this.Invalidate();
					break;
				case Keys.Delete:
                    this.DeleteSelectedObjects();
                    break;
				case Keys.Up:
                    this.TranslateObject(this._selectedObjects.FirstOrDefault(), 0, -1);
					Invalidate();
					break;
				case Keys.Down:
                    this.TranslateObject(this._selectedObjects.FirstOrDefault(), 0, 1);
					Invalidate();
					break;
				case Keys.Left:
                    this.TranslateObject(this._selectedObjects.FirstOrDefault(), -1, 0);
					Invalidate();
					break;
				case Keys.Right:
                    this.TranslateObject(this._selectedObjects.FirstOrDefault(), 1, 0);
					Invalidate();
					break;

			}
			base.OnKeyDown(e);
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
            base.OnMouseDown(e);

            /// Apparently the Panel that MazeController inherits from doesn't naturally get focus
            /// on click.
            //this.Focus();

			//         if ( e.Button == MouseButtons.Left &&
			//              ( ModifierKeys & Keys.Control ) != Keys.Control
			//              && this.ComboBoxObjects != null )
			//         {
			//             this._mouseDownLocation = e.Location;

			//             this.ComboBoxObjects.SelectedItem = this.ObtainObjectAt(e.Location);
			//}

			if (e.Button == MouseButtons.Left)
            {
                this._mouseDownLocation = e.Location;

                this.SelectObject(this.ObtainObjectAt(e.Location),
                    (ModifierKeys & Keys.Control) == Keys.Control);
            }


			//this._mode = (MultiSelectMode)( ModifierKeys & Keys.Control );

			///// Always need to know what, if any, node is clicked on.
			//TreeNode selected = this.GetNodeAt( e.Location );

			//this._currentSelection = selected != null &&
			//                         e.Button == MouseButtons.Left &&
			//                         e.Location.X >= selected.Bounds.Left &&
			//                         e.Location.X <= selected.Bounds.Right ?
			//                             selected : null;
		}

		private MazeObject SelectObject( MazeObject mazeObject, bool isMultiSelect )
        {
            if ( !isMultiSelect )
            {
				/// Avoid re-selecting a single node. (Do we care about extra events?)
                //if ( !mazeObject.Selected || this._selectedObjects.Count > 1 )
                {
                    this._selectedObjects.Clear();
                }
			}

			/// If someone clicks on "nothing" mazeObject will be null and nothing will be added.
			/// When not multiselect and nothing selected then it results in clearing selection.
			/// Selecting the same object 2x removes it.
            if ( !this._selectedObjects.Remove(mazeObject) && mazeObject != null )
            {
                this._selectedObjects.Add(mazeObject);
            }

			return mazeObject;
        }

        //private MazeObject SelectObject(IEnumerable<MazeObject> mazeObjects, bool isMultiSelect)
        //{
        //    if (!isMultiSelect)
        //    {
        //        /// Avoid re-selecting a single node. (Do we care about extra events?)
        //        //if ( !mazeObject.Selected || this._selectedObjects.Count > 1 )
        //        {
        //            this._selectedObjects.Clear();
        //        }
        //    }

        //    /// If someone clicks on "nothing" mazeObject will be null and nothing will be added.
        //    /// When not multiselect and nothing selected then it results in clearing selection.
        //    /// Selecting the same object 2x removes it.
        //    if (!this._selectedObjects.Remove(mazeObject) && mazeObject != null)
        //    {
        //        this._selectedObjects.Add(mazeObject);
        //    }

        //    return mazeObject;
        //}

		protected override void OnMouseMove( MouseEventArgs e )
        {
            base.OnMouseMove( e );

            if ( e.Button == MouseButtons.Left &&
                 (ModifierKeys & Keys.Control) != Keys.Control &&
                 this._mouseDownLocation != e.Location )
            {
                MazeObject obj = this._selectedObjects.FirstOrDefault();
                if ( obj != null )
                {
                    DoDragDrop( obj,
                        obj is EscapePod ? DragDropEffects.None : DragDropEffects.Copy );
                }
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(typeof(Silver.UI.ToolBoxItem)))
			{
				ToolBoxItem item = (ToolBoxItem)drgevent.Data.GetData(typeof(Silver.UI.ToolBoxItem));
				if (item.Object != null)
				{
					if (item.Object is MazeWall)
					{
						MazeObject mObject = this.ObtainObjectAt(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
						
						if (mObject != null && mObject is MazeWall)
						{
							drgevent.Effect = DragDropEffects.None;
						}
						else
						{
							drgevent.Effect = DragDropEffects.Copy;
						}
					}
					else
					{
						drgevent.Effect = DragDropEffects.Copy;
					}
				}
			}
			else 
			{
				drgevent.Effect = DragDropEffects.None;

				Point panelXY = PointToClient( new Point( drgevent.X, drgevent.Y ) );

				if ( !( panelXY.X < DataConverter.PADDING ||
						panelXY.Y < DataConverter.PADDING ||
						panelXY.X > this.Width - DataConverter.PADDING ||
						panelXY.Y > this.Height - DataConverter.PADDING ) )
				{
					drgevent.Effect = DragDropEffects.Copy;
				}

				//MazeObject mazeObject = (MazeObject)drgevent.Data.GetData( typeof( MazeObject ) );

				//if ( mazeObject != null )
				//{
				//    Point panelXY = PointToClient( new Point( drgevent.X, drgevent.Y ) );

				//    Debug.WriteLine( panelXY.ToString() );

				//    int widthMask = DataConverter.PADDING + ( mazeObject.Image.Size.Width / 2 );
				//    int heightMask = DataConverter.PADDING + ( mazeObject.Image.Size.Height / 2 );

				//    if ( !( panelXY.X < widthMask ||
				//            panelXY.Y < heightMask ||
				//            panelXY.X > this.Width - widthMask ||
				//            panelXY.Y > this.Height - heightMask ) )
				//    {
				//        drgevent.Effect = DragDropEffects.Move;
				//    }
				//}
			}

			base.OnDragOver(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			//this.Select();
			ToolBoxItem dragItem = null;
			Point panelXY = PointToClient(new Point(drgevent.X, drgevent.Y));

			if (drgevent.Data.GetDataPresent(typeof(Silver.UI.ToolBoxItem)))
			{
				dragItem = drgevent.Data.GetData(typeof(Silver.UI.ToolBoxItem)) as ToolBoxItem;

				if (null != dragItem && null != dragItem.Object)
				{
					if (_maze.MazeType != MazeType.TypeB && dragItem.Object.GetType() == typeof(EscapePod))
					{
						MessageBox.Show("The Escape pod can only be added to 'B' Type mazes.");
					}
					else
					{
						MazeObject clonedObject = AddObjectClone(dragItem.Object, panelXY);
						if (clonedObject != null)
						{
							Invalidate();
						}
					}
				}
			}
			else
			{
				this.MoveObject( this._selectedObjects.FirstOrDefault(), panelXY );
				Invalidate();
			}
			this.Focus();   //this is required so that when dropping an object, the maze panel gets focus so keys work immediately with out clicking on the maze first.
			base.OnDragDrop(drgevent);
		}

#endregion

#region Public Methods


		public Image GetImage()
		{
			bool gridLineState = GridLines;
			GridLines = false;
			_repainted = false;
			Invalidate(true);

			while(!_repainted)
			{
				Application.DoEvents();
			}

			SuspendLayout();
			// reverse control z-index
			ReverseControlZIndex(this);

			int xTrim = 200;
			Bitmap bitmap = new Bitmap(this.Width, this.Height);

			this.DrawToBitmap(bitmap, new Rectangle(0, 0, this.Width, this.Height));

			Bitmap trimmedBitmap = new Bitmap(bitmap.Width - xTrim, bitmap.Height);
			Rectangle cropRect = new Rectangle(xTrim, 0, bitmap.Width - xTrim, bitmap.Height);
			using (Graphics g = Graphics.FromImage(trimmedBitmap))
			{
				g.DrawImage(bitmap, new Rectangle(0, 0, trimmedBitmap.Width, trimmedBitmap.Height),cropRect,GraphicsUnit.Pixel);
			}

			// reverse control z-index back
			ReverseControlZIndex(this);
			ResumeLayout(true);

			GridLines = gridLineState;
			Invalidate(true);

			return AdjustContrast(trimmedBitmap, (ushort) 100);
		}

		public static Bitmap AdjustContrast(Bitmap Image, ushort increase)
		{
			Bitmap NewBitmap = (Bitmap)Image.Clone();
			BitmapData data = NewBitmap.LockBits(
				new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
				ImageLockMode.ReadWrite,
				NewBitmap.PixelFormat);
			int Height = NewBitmap.Height;
			int Width = NewBitmap.Width;

			unsafe
			{
				for (int y = 0; y < Height; ++y)
				{
					byte* row = (byte*)data.Scan0 + (y * data.Stride);
					int columnOffset = 0;
					for (int x = 0; x < Width; ++x)
					{
						int B = row[columnOffset];
						int G = row[columnOffset + 1];
						int R = row[columnOffset + 2];

						if (R > 0) R += increase;
						if (G > 0) G += increase;
						if (B > 0) B += increase;

						if (R > 255) R = 255;
						if (G > 255) G = 255;
						if (B > 255) B = 255;

						row[columnOffset] = (byte)B;
						row[columnOffset + 1] = (byte)G;
						row[columnOffset + 2] = (byte)R;

						columnOffset += 4;
					}
				}
			}
			NewBitmap.UnlockBits(data);
			return NewBitmap;
		}

		private void ReverseControlZIndex(Control parentControl)
		{
			var list = new List<Control>();
			foreach (Control i in parentControl.Controls)
			{
				list.Add(i);
			}
			var total = list.Count;
			for (int i = 0; i < total / 2; i++)
			{
				var left = parentControl.Controls.GetChildIndex(list[i]);
				var right = parentControl.Controls.GetChildIndex(list[total - 1 - i]);

				parentControl.Controls.SetChildIndex(list[i], right);
				parentControl.Controls.SetChildIndex(list[total - 1 - i], left);
			}
		}

		public TreeNode TreeRender(TreeView treeView, TreeNode currentNode, bool gridLines)
		{
			_gridLines = gridLines;
			TreeNode parentNode = null;
			TreeNode mazeNode = null;
			if (treeView != null)
			{
				try
				{
					treeView.BeginUpdate();

					//see if it has a parent node...
					if ( ( parentNode = currentNode?.Parent ) != null )
					{
						treeView.Nodes.Remove( currentNode );
					}

					/// Add node after selected
					mazeNode = new TreeNode( this._maze.Name )
					{
						Tag = this
					};

					/// if Add maze occurs inside a MazeCollection
					if ( treeView.SelectedNode?.Parent?.Tag is MazeCollectionController collectionController )
					{
						/// Insert After
						int mazeIndex = 
							treeView.SelectedNode.Parent.Nodes.IndexOf( treeView.SelectedNode ) + 1;

						treeView.SelectedNode.Parent.Nodes.Insert( mazeIndex, mazeNode );

						collectionController.MazeCollection.Mazes.Insert( mazeIndex, this._maze );
					}
					else
					{
						/// top level maze.
						treeView.Nodes.Insert(
							treeView.Nodes.IndexOf( treeView.SelectedNode?.Parent ?? treeView.SelectedNode ) + 1,
							mazeNode );
					}

					mazeNode.ImageIndex = ((int)_maze.MazeType) + 1;
					mazeNode.SelectedImageIndex = mazeNode.ImageIndex;
				}
				catch (Exception ex)
				{
					_lastError = ex.Message;
				}
				finally
				{
					treeView.EndUpdate();
				}
			}
			else
			{
				_lastError = "Tree not defined.";
			}
			return mazeNode;
		}

#endregion

#region Private Methods

		public bool AddObject(object obj)
		{
			bool wasAdded = false;

			if ( obj is MazeObject mazeObject )
			{
				if ( mazeObject.MaxObjects > _maze.GetObjectTypeCount( obj.GetType() ) )
				{
					this._selectedObjects.Clear();

					mazeObject.Name = NameFactory.Create( obj.GetType().Name );

					if ( obj is MazeWall wall )
					{
						wall.Position = wall.GetAdjustedPosition(wall.Position);
					}

					_maze.MazeObjects.Add( mazeObject );
					BindComboBoxObjects( mazeObject );

					wasAdded = true;
				}
				else
				{
					MessageBox.Show( $"You can't add any more {obj.GetType().Name} objects.",
						"The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}

			return wasAdded;
		}

		public MazeObject AddObjectClone(object obj, Point point)
		{
			MazeObject clonedObject = null;

			if ( obj is MazeObject mazeObject )
			{
				if ( mazeObject.MaxObjects > _maze.GetObjectTypeCount( obj.GetType() ) )
				{
					this._selectedObjects.Clear();

					if ( obj is MazeWall mazeWall )
					{
						clonedObject = new MazeWall( mazeWall.WallType )
						{
							UserWall = true,
							WallIndex = this._maze.PointToStamp( point ),
						};
					}
					else
					{
						clonedObject = (MazeObject)Activator.CreateInstance( obj.GetType(), true );

						if ( clonedObject is TripPad tripPad )
						{
							//special case for Trip Pads, must create a pyroid too
							TripPadPyroid tripPyroid = new TripPadPyroid
							{
								Name = NameFactory.Create( typeof( TripPadPyroid ).Name ),
							};

							tripPyroid.Position = tripPyroid.GetAdjustedPosition( tripPad.Position );
							tripPad.Pyroid = tripPyroid;
							_maze.MazeObjects.Add( tripPyroid );
						}
					}

					clonedObject.Name = NameFactory.Create( clonedObject.GetType().Name );
					clonedObject.Position = clonedObject.GetAdjustedPosition( point );
					clonedObject.Selected = true;
					_maze.MazeObjects.Add( clonedObject );

					BindComboBoxObjects( clonedObject );
				}
				else
				{
					MessageBox.Show( $"You can't add any more {obj.GetType().Name} objects.",
						"The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning );
				}
			}

			return clonedObject;
		}

		private void BindComboBoxObjects(MazeObject obj)
		{
			if (_comboBoxObjects != null)
			{
                BindingList<IName> dataSource = new BindingList<IName>(
					_maze.MazeObjects.OrderBy( o => o.GetType() == typeof(MazeWall) ).
                          ThenBy( o => o.GetType().Name ).ToList().ConvertAll( m => (IName)m ) );

				/// Add the maze so user can select and edit elements.
				dataSource.Insert( 0, this._maze );

				_comboBoxObjects.DataSource = dataSource;
				_comboBoxObjects.DisplayMember = "Name";
				_comboBoxObjects.ValueMember = "Name";

				if (obj != null)
				{
					_comboBoxObjects.SelectedItem = obj;
				}
			}
		}

		//private void MoveSelectedObject(Point newpos)
		//{
		//	MazeObject obj = GetSelectedObject();
		//	if (obj != null)
		//	{
		//		obj.Position = obj.GetAdjustedPosition( newpos);
		//	}
		//}

		//private void MoveSelectedObject(int x, int y)
		//{
		//	MazeObject obj = GetSelectedObject();
		//	if (obj != null)
		//	{
		//		int new_x = obj.Position.X + (x * obj.SnapSize.X);
		//		int new_y = obj.Position.Y + (y * obj.SnapSize.Y);

		//		if (new_x >= 0)
		//		{
		//			if (new_y >= 0)
		//			{
		//				if (new_x <= base.Width - obj.Size.Width)
		//				{
		//					if (new_y <= base.Height - obj.Size.Height)
		//					{
		//						if (new_x != obj.Position.X || new_y != obj.Position.Y)
		//						{
		//							obj.Position = new Point(new_x, new_y);
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//}

        private void MoveObject(MazeObject mazeObject, Point newpos)
        {
            if (mazeObject != null)
            {
                mazeObject.Position = mazeObject.GetAdjustedPosition(newpos);
            }
        }

        private void TranslateObject(MazeObject mazeObject, int x, int y)
        {
            if (mazeObject != null)
            {
                int new_x = mazeObject.Position.X + (x * mazeObject.SnapSize.X);
                int new_y = mazeObject.Position.Y + (y * mazeObject.SnapSize.Y);

                if (new_x >= 0)
                {
                    if (new_y >= 0)
                    {
                        if (new_x <= base.Width - mazeObject.Size.Width)
                        {
                            if (new_y <= base.Height - mazeObject.Size.Height)
                            {
                                if (new_x != mazeObject.Position.X || new_y != mazeObject.Position.Y)
                                {
                                    mazeObject.Position = new Point(new_x, new_y);
                                }
                            }
                        }
                    }
                }
            }
        }

		private MazeObject ObtainObjectAt(Point location)
		{
			//Adjust select point based upon some of our Panel dimension 'hacks'.
			Point adjustedLocation = new Point( (int)( location.X - DataConverter.PADDING ), location.Y - DataConverter.PADDING );
            //Point adjustedLocation = new Point( location.X, location.Y );

            /// Get all maze objects hit..
            List<MazeObject> hitList =
				this._maze.MazeObjects.Where( mo => PointInObject( mo, adjustedLocation ) ).
                     OrderBy( o => o.GetType() == typeof( MazeWall ) ).ToList();

			/// look for an already selected object
			MazeObject selectedObject = hitList.FirstOrDefault( mo => mo.Selected );

			if ( selectedObject != null )
			{
				/// If object already selected, shift key is pressed, and multiple hit objects then
				/// cycle through and choose the "next" object in the z order.
				if ( ModifierKeys == Keys.Shift && hitList.Count > 1 )
				{
					int index = hitList.IndexOf( selectedObject );

					selectedObject = hitList[ ( index + 1 ) % hitList.Count ];
				}
			}
			else
			{
				/// Grab first in z order if no object already selected.
				selectedObject = hitList.FirstOrDefault();
			}

			return selectedObject;
		}

		private bool PointInObject(MazeObject obj, Point location)
		{
			if (location.X >= obj.RenderPosition.X && location.X <= obj.RenderPosition.X + obj.Size.Width)
			{
				if (location.Y >= obj.RenderPosition.Y && location.Y <= obj.RenderPosition.Y + obj.Size.Height)
				{
					return true;
				}
			}
			return false;
		}

		//private MazeObject GetSelectedObject()
		//{
		//	return this._maze.MazeObjects.FirstOrDefault( o => o.Selected );
		//}

		//private int GetSelectedObjectIndex()
		//{
		//	for ( int i = 0; i < _maze.MazeObjects.Count; i++)
		//	{
		//		if (_maze.MazeObjects[i].Selected) return i;
		//	}
		//	return -1;
		//}

		//private void ClearSelectedObjects()
		//{
		//	foreach ( MazeObject mazeObject in _maze.MazeObjects )
		//	{
		//		mazeObject.Selected = false;
		//	}
		//}

		//private void SetSelectedObject(int index)
		//{
		//	MazeObject mazeObject = this._maze.MazeObjects.ElementAtOrDefault( index );

		//	if ( mazeObject != null )
		//	{
		//		ClearSelectedObjects();

		//		mazeObject.Selected = true;
		//	}
		//}

		private void comboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
		{
            //ClearSelectedObjects();

			if ( _comboBoxObjects.SelectedItem is MazeObject mazeObject )
			{

                //this.SelectObjects( new List<MazeObject>() {mazeObject} );

                //mazeObject.Selected = true;
			}

            //if ( _comboBoxObjects.SelectedItem != null )
            //         {
            //             this._propertyGrid.SelectedObjects = new object[] { _comboBoxObjects.SelectedItem };
            //         }

            //Invalidate();
		}

		#endregion

		private void OnMazePropertyChanged( object sender, PropertyChangedEventArgs e )
		{
            /// Update modified time here. We do it outside the Maze Class itself because serialization
            /// operations on the Maze cause updates which corrupt the idea of this being user
            /// modification time stamp.
            if ( e.PropertyName != ChangeTrackingBase.PropertyNameString )
            {
                this._maze.Modified = new EditInfo( DateTime.Now, Containers.VersionInformation.ApplicationVersion );
            }

            /// Force redraw of maze on change..
            this.Invalidate();

			if ( this._propertyGrid != null )
			{
				this._propertyGrid.Refresh();
			}
		}

        private void mazeControllerContextMenu_Opening( object sender, CancelEventArgs e )
        {
            this.selectAllToolStripMenuItem.Enabled = this.Maze.MazeObjects.Count != 0;

            this.cutToolStripMenuItem.Enabled =
                this.copyToolStripMenuItem.Enabled =
                    this.deleteToolStripMenuItem.Enabled = this._selectedObjects.Count > 0;

            this.pasteToolStripMenuItem.Enabled = Clipboard.ContainsData(ClipboardFormatId);
        }

		private void selectAllToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            HashSet<Type> mazeObjectTypes = new HashSet<Type>();

            foreach ( MazeObject mazeObject in this._maze.MazeObjects )
            {
                mazeObjectTypes.Add( mazeObject.GetType() );
            }

            this.selectAllToolStripMenuItem.DropDownItems.Clear();

            foreach ( Type type in mazeObjectTypes )
            {
                if ( type == typeof( TripPadPyroid ) )
                {
					continue;
                }

                ToolStripMenuItem newItem =
                    new ToolStripMenuItem( type.Name )
                    {
                        Tag = type,
                        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                        ImageKey = type.Name,
                    };

                newItem.Click += this.SelectAllMazeObjectOfType_Click;

                this.selectAllToolStripMenuItem.DropDownItems.Add( newItem );
            }
        }

        private void SelectAllMazeObjectOfType_Click( object sender, EventArgs e )
        {
            ToolStripItem item = (ToolStripItem)sender;

            /// By default this operation is basically a multiselect since there are likely to be multiple
            /// objects in the passed collection. However, the multiselect now determines if we add to
            /// the currently selected or replace them.
            bool addToSelection = (ModifierKeys & Keys.Control) == Keys.Control;

            foreach ( MazeObject mazeObject in
                this._maze.MazeObjects.Where( mo => mo.GetType() == (Type) item.Tag ) )
            {
                /// if we are replacing the selected objects, addToSelection will clear on first add.
                this.SelectObject( mazeObject, addToSelection );

                /// and then multiselect the rest.
                addToSelection = true;
            }

            //this._propertyGrid.SelectedObjects =
            //    ( this._propertyGrid.SelectedObjects == null ?
            //          objectsToSelect :
            //          this._propertyGrid.SelectedObjects.Concat( objectsToSelect ) )
            //    .ToArray();

            //this.ComboBoxObjects.SelectedItem =
            //    this._propertyGrid.SelectedObjects.Length > 1 ?
            //        null : this._propertyGrid.SelectedObjects.FirstOrDefault();

            this.Invalidate();
        }

        private static string ClipboardFormatId = "MazeObjectFormat";

		private void toolStripMenuItemCut_Click(object sender, EventArgs e)
		{
   //         foreach ( MazeObject mazeObject in this.CopySelectedToClipboard() )
   //         {
			//	/// Call Delete to deal with trips/trippyroids?
   //             this._maze.MazeObjects.Remove( mazeObject );

   //             if ( mazeObject is TripPad tripPad )
   //             {
   //                 this._maze.MazeObjects.Remove(tripPad.Pyroid);
   //             }
			//}

            this.RemoveObjectsFromMaze( this.CopySelectedToClipboard() );
        }

		private void toolStripMenuItemCopy_Click(object sender, EventArgs e)
        {
            this.CopySelectedToClipboard();
        }

        private IEnumerable<MazeObject> CopySelectedToClipboard()
        {
            //         List<MazeObject> selected = this._maze.MazeObjects.Where( o => o.Selected ).ToList();

            /////TripPadPyroids can't be copied/cut by themselves!!!!
            //         if ( selected.Any( s => s.GetType() == typeof(TripPadPyroid)) )
            //         {
            //             MessageBox.Show(
            //                 "The selection includes at least one Trip Pad Pyroid. Remove to complete this action.");

            //             selected.Clear();

            //             return selected;
            //         }


            if ( this.TryRemoveTripPadPyroids( this._selectedObjects,
                out IEnumerable<MazeObject> noPyroids ) )
            {
                List<MazeObject> asList = noPyroids.ToList();

                if (asList.Count != 0 )
                {
                    /// Wrap in DataObject so that this data is cleared from clipboard on exit.
                    Clipboard.SetDataObject( new DataObject( ClipboardFormatId, asList), false );
                }

                return noPyroids;
            }

            return new List<MazeObject>();
        }

		private void toolStripMenuItemPaste_Click(object sender, EventArgs e)
        {
            if ( Clipboard.ContainsData( ClipboardFormatId ) )
            {
                IEnumerable<MazeObject> selected =
                    Clipboard.GetData( ClipboardFormatId ) as IEnumerable<MazeObject>;

				/// Always clear any selection before paste.
                this._selectedObjects.Clear();

				/// TODO: Deal with adding to many of any given object.
                //bool promptUser = true;

                //_maze.MazeObjects.OrderBy(o => o.GetType() == typeof(MazeWall)).
                //      ThenBy(o => o.GetType().Name).ToList().ConvertAll(m => (IName)m) );

				foreach ( MazeObject mazeObject in selected )
                {
                    //if (this._maze.GetObjectTypeCount(mazeObject.GetType()) >= mazeObject.MaxObjects)
                    //{
                    //    MessageBox.Show($"You can't add any more {mazeObject.GetType().Name} objects.",
                    //        "The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}

					/// Don't need to set MazeObject.Selected to True because it was when it was Cut.
					this._maze.MazeObjects.Add( mazeObject );

                    if (mazeObject is TripPad tripPad)
                    {
                        this._maze.MazeObjects.Add(tripPad.Pyroid);
                    }
                }
			}
        }

		private void toolStripMenuItemDelete_Click(object sender, EventArgs e)
        {
			this.DeleteSelectedObjects();
		}

        private void DeleteSelectedObjects()
        {
            string plural = this._selectedObjects.Count == 1 ?
                                this._selectedObjects[0].Name :
                                $"all {this._selectedObjects.Count} objects";

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete {plural}?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            //result = DialogResult.None;

            //IEnumerable<MazeObject> tripPyroids =
            //    this._selectedObjects.Where(so => so is TripPadPyroid);

            //if (tripPyroids.FirstOrDefault() != null)
            //{
            //    result = MessageBox.Show("TripPadPyroids can not be individually added or removed from " +
            //                             $"a Maze. Select the parent TripPad to perform this action.{Environment.NewLine}{Environment.NewLine}" +
            //                             "Press OK to skip the TripPadPyroids and continue.", "Selection includes TripPadPyroid",
            //        MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            //}

            //if (result == DialogResult.Cancel)
            //{
            //    return;
            //}

            //this.RemoveObjectsFromMaze( this._selectedObjects.Except( tripPyroids ) );
            if ( this.TryRemoveTripPadPyroids( this._selectedObjects,
                out IEnumerable<MazeObject> noPyroids ) )
            {
                this.RemoveObjectsFromMaze( noPyroids );
			}
        }

        private bool TryRemoveTripPadPyroids( in IEnumerable<MazeObject> mazeObjects, out IEnumerable<MazeObject> freeOfTripPadPyroids )
        {
            IEnumerable<MazeObject> tripPyroids = mazeObjects.Where( so => so is TripPadPyroid );

			DialogResult result = DialogResult.None;

            if (tripPyroids.FirstOrDefault() != null)
            {
                result = MessageBox.Show(
                    "TripPadPyroids can not be individually added to/removed from a Maze. Select the parent TripPad to perform this action." +
                    $"{Environment.NewLine}{Environment.NewLine}Press OK to skip the TripPadPyroids and continue.",
                    "Selection includes TripPadPyroid",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );
            }

            freeOfTripPadPyroids = mazeObjects.Except( tripPyroids );

			return result != DialogResult.Cancel;
        }

		private void RemoveObjectsFromMaze( IEnumerable<MazeObject> objectsToRemove )
        {
            foreach (MazeObject mazeObject in objectsToRemove)
            {
                if (mazeObject is TripPad tripPad)
                {
                    this._maze.MazeObjects.Remove(tripPad.Pyroid);
                }

                this._maze.MazeObjects.Remove(mazeObject);
            }

            this.Invalidate();
        }
	}
}
