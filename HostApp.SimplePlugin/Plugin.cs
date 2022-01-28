using HostApp.Plugin.Contract;

namespace HostApp.SimplePlugin
{
    public class Plugin : IPlugin
    {
        public object GetView() => new SimpleUI();
        public void TestPackagedCode() => new Extra.Extra().CallCodeFromPackagedDll();
    }
}
