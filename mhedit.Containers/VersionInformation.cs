using System;
using System.Deployment.Application;
using System.Reflection;

namespace mhedit.Containers
{
    public static class VersionInformation
    {
        public static readonly Version ApplicationVersion;

        static VersionInformation()
        {
            ApplicationVersion = Assembly.GetEntryAssembly().GetName().Version;

            if ( ApplicationDeployment.IsNetworkDeployed )
            {
                ApplicationVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
        }
    }
}
