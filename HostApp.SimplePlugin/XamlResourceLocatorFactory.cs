using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel;

namespace HostApp.SimplePlugin
{
    internal static class XamlResourceLocatorFactory
    {
        private static readonly string packageName;
        private static readonly string packagePath;

        static XamlResourceLocatorFactory()
        {
            // We can assume the assembly and package name are the same as this is required by MRT Core.
            // MRT Core throws an exception "Resource map not found..." when loading resources.pri if this is not the case.
            packageName = typeof(XamlResourceLocatorFactory).Assembly.GetName().Name;
            
            var package = Package.Current.Dependencies.First(x => x.Id.Name == packageName);
            packagePath = package.InstalledLocation.Path;

            // alternative: we might be able to get the URIs from the loaded resource map
            //var rm = Windows.ApplicationModel.Resources.Core.ResourceManager.Current;
            //var map = rm.AllResourceMaps.First(x => x.Key == packageName);
        }

        internal static Uri Create([CallerFilePath] string callerFilePath = "")
        {
            // This is not a foolproof solution but it works well enough to get started
            var i = callerFilePath.LastIndexOf(packageName);
            var componentPath = callerFilePath[i..(callerFilePath.Length-3)];

            return new Uri($"ms-appx:///{packagePath}\\{componentPath}");
        }
    }
}
