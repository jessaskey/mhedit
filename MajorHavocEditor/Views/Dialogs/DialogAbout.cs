using System;
using System.Windows.Forms;
using Krypton.Toolkit;
using mhedit;

namespace MajorHavocEditor.Views.Dialogs
{

    public partial class DialogAbout : KryptonForm
    {
        /// <summary>
        ///     Our default constructor for the About dialog.
        /// </summary>
        public DialogAbout()
        {
            this.InitializeComponent();
        }

        private void buttonOK_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void DialogAbout_Load( object sender, EventArgs e )
        {
            this.labelVersion.Text = this.labelROMVersion.Text = "Unavailable";

            try
            {
                //Show the ClickOnce assembly version if possible, if this is not a ClickOnce deployment, then show the Assembly version still 
                this.labelVersion.Text =
                    $"{mhedit.Containers.VersionInformation.ApplicationVersion} ";//+
                                         //$"{( ApplicationDeployment.IsNetworkDeployed ? "(ClickOnce)" : "(Assembly)" )}";

                this.labelROMVersion.Text =
                    $"{VersionInformation.RomVersion.Major.ToString( "X2" )}." +
                    $"{VersionInformation.RomVersion.Minor.ToString( "X2" )}";
            }
            catch
            {
            }

            this.labelCopyright.Text =
                $"Copyright © 2006-{DateTime.Now.Year}  Jess M. Askey/Bryan L. Roth";
        }
    }

}