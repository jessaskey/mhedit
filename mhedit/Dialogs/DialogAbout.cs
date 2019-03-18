using System;
using System.Deployment.Application;
using System.Windows.Forms;

namespace mhedit
{

    public partial class DialogAbout : Form
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
            //Show the ClickOnce assembly version if possible, if this is not a ClickOnce deployment, then show the Assembly version still 
            this.labelVersion.Text = $"{Containers.VersionInformation.ApplicationVersion} " +
                                     $"{( ApplicationDeployment.IsNetworkDeployed ? "(ClickOnce)" : "(Assembly)" )}";

            this.labelROMVersion.Text = $"{VersionInformation.RomVersion.Major.ToString( "X2" )}." +
                                        $"{VersionInformation.RomVersion.Minor.ToString( "X2" )}";

            this.labelCopyright.Text =
                $"Copyright © 2006-{DateTime.Now.Year}  Jess M. Askey/Bryan L. Roth";
        }
    }

}