using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using mhedit.Containers.Validation;

namespace MajorHavocEditor.Views
{
    public partial class ValidationResultTab : UserControl
    {
        private static readonly ImageList IconList;
        private List<DataGridViewRow> _errorRows = new List<DataGridViewRow>();
        private List<DataGridViewRow> _warningRows = new List<DataGridViewRow>();
        private List<DataGridViewRow> _messageRows = new List<DataGridViewRow>();

        static ValidationResultTab()
        {
            IconList =
                new ImageList { TransparentColor = Color.Fuchsia }
                    .AddImages( new[]
                                {
                                    (nameof(ValidationLevel.Error), "Error_red_16x16.png"),
                                    (nameof(ValidationLevel.Warning), "Warning_yellow_7231_16x16.png"),
                                    (nameof(ValidationLevel.Message), "Information_blue_6227_16x16.png"),
                                } )
                    .WithResourcePath( "Resources/Images" )
                    .Load();
        }

        public ValidationResultTab()
            : this( new ValidationResult() )
        { }

        public ValidationResultTab( IValidationResult result, string title = "" )
        {
            this.InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            /// Avoid possible InvalidOperationException: This operation cannot be
            /// performed while an auto-filled column is being resized.
            /// 
            /// https://stackoverflow.com/a/34345439
            /// Make sure TopLeftHeaderCell is created
            var topLeftHeaderCell = this.dataGridView1.TopLeftHeaderCell;

            /// Always default to the provided title if one is passed in.
            if ( !string.IsNullOrWhiteSpace( title ) )
            {
                this.Text = title;
            }
            /// following that, choose the Name of the object.
            else if ( result.Context is IName iName && !string.IsNullOrWhiteSpace( iName.Name ) )
            {
                this.Text = iName.Name;
            }
            else
            {
                this.Text = NameFactory.Create( "Validation Results" );
            }

            DataGridViewColumn column =
                new DataGridViewImageColumn(  )
                {
                    Name = "Level",
                    HeaderText = "",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                    MinimumWidth = 16,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.Automatic,
                    DefaultCellStyle =
                    {
                        Alignment = DataGridViewContentAlignment.TopCenter,
                        Padding = new Padding(0,2,0,0)
                    }
                };

            this.dataGridView1.Columns.Add( column );

            column = new DataGridViewTextBoxColumn()
            {
                Name = "Description",
                HeaderText = "Description",
                DefaultCellStyle = { WrapMode = DataGridViewTriState.True }
            };

            this.dataGridView1.Columns.Add( column );

            this.dataGridView1.Columns.Add( "Maze", "Maze/Collection" );
            this.dataGridView1.Columns.Add( "Object", "Object" );

            this.AddResult( result, string.Empty );

            this.SetVisible( this._errorRows, this.ErrorsButton );
            this.SetVisible( this._warningRows, this.WarningsButton );
            this.SetVisible( this._messageRows, this.MessagesButton );

            this.dataGridView1.AutoResizeRowHeadersWidth(
                DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders );
        }

        private void AddResult( IValidationResult result, string name )
        {
            if ( result.Context is IName iName )
            {
                name += string.IsNullOrEmpty( name ) ?
                            string.Empty :
                            $".{iName.Name}";
            }

            /// if this layer is a collection of results than skip down into the
            /// collection without adding any rows.
            if ( result is IEnumerable<IValidationResult> collection )
            {
                foreach ( IValidationResult current in collection )
                {
                    this.AddResult( current, name );
                }

                return;
            }

            int rowId = this.dataGridView1.Rows.Add();

            DataGridViewRow row = this.dataGridView1.Rows[ rowId ];

            row.ReadOnly = true;
            row.Tag = result;

            row.Cells[ "Object" ].Value = result.Context;
            row.Cells[ "Level" ].Value = IconList.Images[ result.Level.ToString() ];
            row.Cells[ "Description" ].Value = result.ToString();
            row.Cells[ "Maze" ].Value = name;

            switch ( result.Level )
            {
                case ValidationLevel.Error:
                    this._errorRows.Add( row );
                    break;

                case ValidationLevel.Warning:
                    this._warningRows.Add( row );
                    break;

                case ValidationLevel.Message:
                    this._messageRows.Add( row );
                    break;

            }
        }

        private void ErrorsButton_CheckedChanged( object sender, EventArgs e )
        {
            this.SetVisible( this._errorRows, this.ErrorsButton );
        }

        private void WarningsButton_CheckedChanged( object sender, EventArgs e )
        {
            this.SetVisible( this._warningRows, this.WarningsButton );
        }

        private void MessagesButton_CheckedChanged( object sender, EventArgs e )
        {
            this.SetVisible( this._messageRows, this.MessagesButton );
        }

        private void SetVisible( IList<DataGridViewRow> list, ToolStripButton button  )
        {
            bool visible = button.Checked;

            foreach ( DataGridViewRow row in list )
            {
                row.Visible = visible;
            }

            button.Text = button.Checked || list.Count == 0 ?
                              $"{list.Count} {button.Tag}" :
                              $"0 of {list.Count} {button.Tag}";
        }
    }
}
