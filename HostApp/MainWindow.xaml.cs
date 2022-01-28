using HostApp.Plugin.Contract;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Windows.ApplicationModel.Resources.Core;

namespace HostApp
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            var appPackage = Windows.ApplicationModel.Package.Current;
            var pluginPackages = appPackage.Dependencies.Where(x => x.IsOptional).ToList();
            NoPluginsTextBlock.Visibility = pluginPackages.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            PluginList.ItemsSource = pluginPackages.Select(pluginPackage =>
            {
                var assemblyPath = Directory.EnumerateFiles(pluginPackage.InstalledLocation.Path, "*Plugin.dll").First();

                var context = new PluginLoadContext(assemblyPath);
                var assembly = context.LoadFromAssemblyPath(assemblyPath);

                var pluginType = assembly.ExportedTypes.FirstOrDefault(type => typeof(IPlugin).IsAssignableFrom(type));
                var plugin = (IPlugin)Activator.CreateInstance(pluginType);

                // See Trace output to confirm message is produced
                plugin.TestPackagedCode();

                // Return plugin UI to be hosted in app
                return plugin.GetView();
            });
        }

        ////Use this to inspect the resources loaded from the package graph.
        ////The plugin resources.pri is loaded however the resources are not located at runtime. Perhaps the pri structure is incorrect?
        //private void InspectResources()
        //{
        //    var rm = ResourceManager.Current;
        //    foreach (var map in rm.AllResourceMaps)
        //    {
        //        ;
        //    }
        //}
    }
}
