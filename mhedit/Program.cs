using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace mhedit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new ThreadExceptionEventHandler(UIThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // Copy user settings from previous application version if necessary
            // https://stackoverflow.com/a/23924277
            if ( Properties.Settings.Default.UpdateSettings )
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            Mainform frm = new Mainform();
            if (!frm.Disposing)
            {
                if (!frm.IsDisposed)
                {
                    Application.Run(frm);
                }
            }
        }

        #region Exception Handler

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowExceptionDialog("Windows Error", t.Exception);
            }
            catch
            {
                try
                {
                    //gather some info
                    string username = "";
                    if (Properties.Settings.Default != null)
                    {
                        username = Properties.Settings.Default.MHPUsername;
                    }
                    string versionString = String.Empty;
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    {
                        Version version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                        versionString = version.ToString();
                    }
                    else
                    {
                        versionString = Application.ProductVersion;
                    }

                    //send home an exception, async, don't wait for response
                    MHEditServiceReference.MHEditClient _client = MHPController.GetClient();
                    _client.LogExceptionAsync(t.Exception.Message, t.Exception.Source, t.Exception.StackTrace, username, versionString);

                    MessageBox.Show("Fatal Windows Error",
                        "Fatal Windows Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                DialogResult result = ShowExceptionDialog("Windows Error", ex);
                Application.Exit();
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        private static DialogResult ShowExceptionDialog(string title, Exception e)
        {
            //try sending this..
            SendException(title, e);
            string errorMsg = "An application error occurred. Information will automatically be submitted to the homeworld. Sorry for an inconvienience.\n\n";
            errorMsg = errorMsg + "\n" + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            DialogResult result = MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
            return result;
        }

        public static void SendException(string title, Exception ex)
        {
            try
            {
                string versionString = "NoVersion";
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    Version version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    versionString = version.ToString();
                }

                MHEditServiceReference.MHEditClient client = MHPController.GetClient();
                client.LogException(ex.Message, ex.Source, ex.StackTrace, String.Empty, versionString);
            }
            catch {}
        }

        #endregion
    }
}