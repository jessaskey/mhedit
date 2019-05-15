using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace mhedit.Containers.Validation
{
    public partial class ValidationWindow : UserControl
    {
        private static readonly ImageList IconList;
        private List<DataGridViewRow> _errorRows = new List<DataGridViewRow>();
        private List<DataGridViewRow> _warningRows = new List<DataGridViewRow>();
        private List<DataGridViewRow> _messageRows = new List<DataGridViewRow>();

        static ValidationWindow()
        {
            IconList = new ImageList
                             {
                                 TransparentColor = Color.Fuchsia
                             };

            IconList.Images.Add( ValidationLevel.Error.ToString(),
                ResourceFactory.GetResourceImage(
                    "mhedit.Containers.Images.Error_red_16x16.png" ) );

            IconList.Images.Add( ValidationLevel.Warning.ToString(),
                ResourceFactory.GetResourceImage(
                    "mhedit.Containers.Images.Warning_yellow_7231_16x16.png" ) );

            IconList.Images.Add( ValidationLevel.Message.ToString(),
                ResourceFactory.GetResourceImage(
                    "mhedit.Containers.Images.Information_blue_6227_16x16.png" ) );
        }

        public ValidationWindow()
            : this( new ValidationResult() )
        { }

        public ValidationWindow( IValidationResult result )
        {
            InitializeComponent();

            if ( result.Context is IName iName )
            {
                this.Text = iName.Name;
            }
            else
            {
                this.Text = result.Context.GetType().Name;
            }

            DataGridViewColumn column =
                new DataGridViewImageColumn(  )
                {
                    Name = "Level",
                    HeaderText = "",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                    MinimumWidth = 16,
                    Resizable = DataGridViewTriState.False,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };

            this.dataGridView1.Columns.Add( column );
            this.dataGridView1.Columns.Add( "Description", "Description" );
            this.dataGridView1.Columns.Add( "Maze", "Maze" );
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
                            iName.Name :
                            $".{iName.Name}";
            }

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
