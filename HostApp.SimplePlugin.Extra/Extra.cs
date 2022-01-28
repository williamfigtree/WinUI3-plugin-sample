using System.Diagnostics;

namespace HostApp.SimplePlugin.Extra
{
    /// <summary>
    /// Used to demonstrate that the plugin is able to execute code from other dlls in its package.
    /// </summary>
    public class Extra
    {
        public void CallCodeFromPackagedDll()
        {
            Trace.WriteLine("Code called from HostApp.SimplePlugin.Extra.dll which is packaged in HostApp.SimplePlugin");
        }
    }
}