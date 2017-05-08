
namespace EIV.WPF.PluginHosting
{
    public sealed class AvailablePlugins : System.Collections.CollectionBase
    {
        public void Add(AvailablePlugin pluginToAdd)
        {
            this.List.Add(pluginToAdd);
        }

        public void Remove(AvailablePlugin pluginToRemove)
        {
            this.List.Remove(pluginToRemove);
        }

        public AvailablePlugin Find(string pluginNameOrPath)
        {
            AvailablePlugin toReturn = null;

            //Loop through all the plugins
            foreach (AvailablePlugin pluginOn in this.List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                {
                    toReturn = pluginOn;
                    break;
                }
            }
            return toReturn;
        }
    }
}