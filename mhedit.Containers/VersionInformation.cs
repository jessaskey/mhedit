using System;
using System.Deployment.Application;
using System.Reflection;

namespace mhedit.Containers
{
    public static class VersionInformation
    {
        private static Version _applicationVersion;

        public static Version ApplicationVersion
        {
            get
            {
                if (_applicationVersion == null )
                {
                    Assembly assembly = Assembly.GetEntryAssembly();
                    if (assembly != null)
                    {
                        _applicationVersion = assembly.GetName().Version;

                        //if (ApplicationDeployment.IsNetworkDeployed)
                        //{
                        //    _applicationVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                        //}
                    }
                }

                return _applicationVersion;
            }
        }
    }

}
