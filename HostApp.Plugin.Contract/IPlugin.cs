namespace HostApp.Plugin.Contract
{
    /// <summary>
    /// The plugin contract. Shared by the host and plugin projects.
    /// </summary>
    public interface IPlugin
    {
        void TestPackagedCode();
        object GetView();
    }
}
