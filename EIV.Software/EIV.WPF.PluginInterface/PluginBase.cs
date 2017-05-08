
namespace EIV.WPF.PluginInterface
{
    using System.Windows;
    public abstract class PluginBase : IPlugin
    {
        public abstract string Name { get; }

        public abstract FrameworkElement CreateControl();

        public virtual void Dispose()
        {
        }
    }
}