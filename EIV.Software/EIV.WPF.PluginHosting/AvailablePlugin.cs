
namespace EIV.WPF.PluginHosting
{
    using EIV.WPF.PluginInterface;
    public sealed class AvailablePlugin
    {
        private IPlugin myInstance = null;
        private string myAssemblyPath = string.Empty;

        public IPlugin Instance
        {
            get { return this.myInstance; }
            set { this.myInstance = value; }
        }
        public string AssemblyPath
        {
            get { return this.myAssemblyPath; }
            set { this.myAssemblyPath = value; }
        }
    }
}