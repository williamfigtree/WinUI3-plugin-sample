using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;

namespace HostApp.SimplePlugin
{
    public sealed partial class SimpleUI : UserControl
    {
        public SimpleUI()
        {
            // Default generated code incorrectly looks for .xaml resources in the host app package
            // Override this behaviour to instead search the plugin package
            //this.InitializeComponent();
            PluginInitializeComponent();

            CounterButton.Click += (s, e) => Counter.Value++;
        }

        /// <summary>
        /// Loads the xaml component from the plugin optional package
        /// </summary>
        private void PluginInitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;

            var resourceLocator = XamlResourceLocatorFactory.Create();
            Application.LoadComponent(this, resourceLocator, ComponentResourceLocation.Application);
        }
    }
}
