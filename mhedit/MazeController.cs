using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;

using Silver.UI;
using mhedit.Containers.MazeObjects;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers;
using ICSharpCode.SharpZipLib.BZip2;

namespace mhedit
{
	[DefaultPropertyAttribute("Name")]
	[Serializable]
	public class MazeController : Panel, ITreeObject, ICustomTypeDescriptor
	{
		#region Declarations

		public const int MAXWALLS = 209;

		/// This is the "padding" around the border of the entire maze.
		//private const int STAMPS_TRIM_LEFT = 3;
		//private Point objectOffset = new Point(-16, 16);

		private Maze _maze = null;
		private decimal _zoom = 1;
		private bool _repainted = false;
		private string _fileName = String.Empty;
		private PropertyGrid _propertyGrid = null;
		private ComboBox _comboBoxObjects = null;
		private bool _gridLines = false;
		private string _lastError = string.Empty;

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
			this._maze = maze;

			this._maze.PropertyChanged += this.OnMazePropertyChanged;

			DoubleBuffered = true;
			SetStyle( ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true );
			UpdateStyles();

			base.Height = ( DataConverter.CanvasGridSize * _maze.MazeStampsY ) + ( DataConverter.PADDING * 2 );
			base.Width = ( DataConverter.CanvasGridSize * _maze.MazeStampsX ) + ( DataConverter.PADDING * 2 );

			//event methods
			base.AllowDrop = true;
			base.TabStop = true;

			//DragOver += new DragEventHandler(Maze_DragOver);
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
			get
			{
				return _maze;
			}
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

		public static Maze DeserializeFromFile(string fileName)
		{
			Maze maze = null;
			using (FileStream fStream = new FileStream(fileName, FileMode.Open))
			{
				maze = DeserializeFromStream(fStream);

				maze.AcceptChanges();
			}
			return maze;
		}

		public static Maze DeserializeFromStream(Stream inputStream)
		{
			Maze maze = null;
			using (MemoryStream mStream = new MemoryStream())
			{
				BZip2.Decompress(inputStream, mStream, false);
				mStream.Position = 0;
				var serializer = new XmlSerializer(typeof(Maze));
				using (var reader = XmlReader.Create(mStream))
				{
					maze = (Maze)serializer.Deserialize(reader);

					maze.AcceptChanges();
				}
			}
			return maze;
		}

		public static bool SerializeToFile(Maze maze, string fileName)
		{
			bool result = false;
			using (FileStream fStream = new FileStream(fileName, FileMode.Create))
			{
				using (MemoryStream mStream = new MemoryStream())
				{
					var serializer = new XmlSerializer(maze.GetType());
					using (var writer = XmlWriter.Create(mStream, new XmlWriterSettings { Indent = true } ) )
					{
						serializer.Serialize(writer, maze, Constants.XmlNamespace);
					}
					//BinaryFormatter b = new BinaryFormatter();
					//b.Serialize(mStream, mazeController.Maze);
					mStream.Position = 0;
					BZip2.Compress(mStream, fStream, true, 4096);
					result = true;

					maze.AcceptChanges();
				}
			}
			return result;
		}

		public static byte[] SerializeToByteArray(Maze maze)
		{
			byte[] bytes = null;
			using (MemoryStream oStream = new MemoryStream())
			{
				using (MemoryStream mStream = new MemoryStream())
				{
					var serializer = new XmlSerializer(maze.GetType());
					using (var writer = XmlWriter.Create(mStream, new XmlWriterSettings { Indent = true } ) )
					{
						serializer.Serialize(writer, maze, Constants.XmlNamespace);
					}
					mStream.Position = 0;
					BZip2.Compress(mStream, oStream, false, 4096);
				}
				bytes = new byte[oStream.Length];
				oStream.Position = 0;
				oStream.Read(bytes, 0, (int)oStream.Length);
			}
			return bytes;
		}

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

#region Overrides

		protected override void OnPaint(PaintEventArgs e)
		{
			//Stopwatch stopwatch = new Stopwatch();
			//long time = stopwatch.ElapsedMilliseconds;
			//stopwatch.Start();
			//Console.Write("OnPaint Begin\n");

			Pen bigGridPen = new Pen(Color.DimGray, 1);
			Pen smallGridPen = new Pen(Color.DimGray, 1);
			Pen backgroundPen = new Pen(Color.Gray, 1);
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

		protected override bool IsInputKey(Keys keyData)
		{
			return true;
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Tab:
					int selectedIndex = GetSelectedObjectIndex();
					selectedIndex++;
					selectedIndex %= _maze.MazeObjects.Count;
					SetSelectedObject(selectedIndex);
					Invalidate();
					break;
				case Keys.Delete:
					MazeObject obj = GetSelectedObject();
					if (obj != null)
					{
						if (obj is TripPadPyroid )
						{
							MessageBox.Show("This Pyroid is part of a Trip Pad and cannot be deleted. Delete the related Trip Pad to remove this Pyroid.");
							return;
						}
						DialogResult dr = MessageBox.Show("Are you sure you want to remove this object from the Maze?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (dr == DialogResult.Yes)
						{
							if (obj is TripPad && ((TripPad)obj).Pyroid != null)
							{
								_maze.MazeObjects.Remove(((TripPad)obj).Pyroid); 
							}
							_maze.MazeObjects.Remove(obj);
							Invalidate();
						}
					}
					break;
				case Keys.Up:
					MoveSelectedObject(0, -1);
					Invalidate();
					break;
				case Keys.Down:
					MoveSelectedObject(0, 1);
					Invalidate();
					break;
				case Keys.Left:
					MoveSelectedObject(-1, 0);
					Invalidate();
					break;
				case Keys.Right:
					MoveSelectedObject(1, 0);
					Invalidate();
					break;

			}
			base.OnKeyDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				MazeObject obj = GetSelectedObject();
				if ( obj != null )
				{
					DragDropEffects effect = DoDragDrop( obj,
						obj is EscapePod ? DragDropEffects.None : DragDropEffects.Copy );
				}
			}
			base.OnMouseMove(e);
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			if ( e.Button == MouseButtons.Left && this.ComboBoxObjects != null )
			{
				this.Select();

				//look for objects here...
				this.ComboBoxObjects.SelectedItem = SelectObject( e.Location );

				Invalidate();
			}

			base.OnMouseDown( e );
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
						MazeObject mObject = SelectObject(this.PointToClient(new Point(drgevent.X, drgevent.Y)));
						
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
				MoveSelectedObject(panelXY);
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
					ClearSelectedObjects();

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
					ClearSelectedObjects();

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
								TripPad = tripPad
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
					_maze.MazeObjects.OrderBy( o => o.GetType().Name ).ToList().ConvertAll( m => (IName)m ) );

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

		private void MoveSelectedObject(Point newpos)
		{
			MazeObject obj = GetSelectedObject();
			if (obj != null)
			{
				obj.Position = obj.GetAdjustedPosition( newpos);
			}
		}

		private void MoveSelectedObject(int x, int y)
		{
			MazeObject obj = GetSelectedObject();
			if (obj != null)
			{
				int new_x = obj.Position.X + (x * obj.SnapSize.X);
				int new_y = obj.Position.Y + (y * obj.SnapSize.Y);

				if (new_x >= 0)
				{
					if (new_y >= 0)
					{
						if (new_x <= base.Width - obj.Size.Width)
						{
							if (new_y <= base.Height - obj.Size.Height)
							{
								if (new_x != obj.Position.X || new_y != obj.Position.Y)
								{
									obj.Position = new Point(new_x, new_y);
								}
							}
						}
					}
				}
			}
		}

		private MazeObject SelectObject(Point location)
		{
			//Adjust select point based upon some of our Panel dimension 'hacks'.
			Point adjustedLocation = new Point( (int)( location.X - DataConverter.PADDING ), location.Y - DataConverter.PADDING );
			//Point adjustedLocation = new Point( location.X, location.Y );

			/// Get all maze objects hit..
			IEnumerable<MazeObject> hitObjects =
				this._maze.MazeObjects.Where( mo => PointInObject( mo, adjustedLocation ) );

			/// look for an already selected object
			MazeObject selectedObject = hitObjects.FirstOrDefault( mo => mo.Selected );

			if ( selectedObject != null )
			{
				/// If object already selected, shift key is pressed, and multiple hit objects then
				/// cycle through and choose the "next" object in the z order.
				if ( Control.ModifierKeys == Keys.Shift && hitObjects.Count() > 1 )
				{
					selectedObject.Selected = false;

					List<MazeObject> hitList = hitObjects.ToList();

					int index = hitList.IndexOf( selectedObject );

					selectedObject = hitList[ ( index + 1 ) % hitList.Count ];

					selectedObject.Selected = true;
				}
			}
			else
			{
				/// Grab first in z order if no object already selected.
				selectedObject = hitObjects.FirstOrDefault();

				if ( selectedObject != null )
				{
					selectedObject.Selected = true;
				}
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

		private MazeObject GetSelectedObject()
		{
			return this._maze.MazeObjects.FirstOrDefault( o => o.Selected );
		}

		private int GetSelectedObjectIndex()
		{
			for ( int i = 0; i < _maze.MazeObjects.Count; i++)
			{
				if (_maze.MazeObjects[i].Selected) return i;
			}
			return -1;
		}

		private void ClearSelectedObjects()
		{
			foreach ( MazeObject mazeObject in _maze.MazeObjects )
			{
				mazeObject.Selected = false;
			}
		}

		private void SetSelectedObject(int index)
		{
			MazeObject mazeObject = this._maze.MazeObjects.ElementAtOrDefault( index );

			if ( mazeObject != null )
			{
				ClearSelectedObjects();

				mazeObject.Selected = true;
			}
		}

		private void comboBoxObjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ( _comboBoxObjects.SelectedItem is MazeObject mazeObject )
			{
				ClearSelectedObjects();

				mazeObject.Selected = true;
			}

			if ( _comboBoxObjects.SelectedItem != null )
			{
				this._propertyGrid.SelectedObject = _comboBoxObjects.SelectedItem;
			}

			Invalidate();
		}

		#endregion

		private void OnMazePropertyChanged( object sender, PropertyChangedEventArgs e )
		{
            /// Force redraw of maze on type change..
            if ( e.PropertyName.Equals( "MazeType" ) )
            {
			this.Invalidate();
			}
		}

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// MazeController
			// 
			this.BackColor = System.Drawing.Color.Black;
			this.Font = new System.Drawing.Font("Courier New", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ResumeLayout(false);

		}
	}
}
