
namespace EIV.WPF.PluginInterface
{
    using System;
    using System.Windows;

    public interface IPlugin : IDisposable
    {
        // Testing ....
        
        string Name { get; }
        /*
        string Description { get; }
        string Author { get; }
        string Version { get; }

        IPluginHost Host { get; set; }
        */

        /// <summary>
        /// Creates plugin's visual element; called only ones in plugin's lifetime
        /// </summary>
        /// <returns>WPF framework element of the plugin</returns>
        FrameworkElement CreateControl();
    }
}