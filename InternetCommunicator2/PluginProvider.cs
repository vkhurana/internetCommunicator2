using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using PluginSDK;
using InterComm.Helpers;

namespace InterComm
{
    /// <summary>
    /// This class is the wrapper around the plugin,
    /// it is here incase i decide to change the way plugins are used
    /// i would not have to re-compile or even make changes to plugins that are
    /// already existant, it will (should ;-> ) work seamlessly with all plugins, ever written :-)
    /// </summary>
    public class PluginWrapper
    {
        IStatelessChatInterface _plugin;
        /// <summary>
        /// constructor that records the plugin implementation
        /// </summary>
        /// <param name="plugin">the implementation of the interface</param>
        public PluginWrapper(IStatelessChatInterface plugin)
        {
            _plugin = plugin;
        }

        /// <summary>
        /// this is the trigger command that is used for the plugin
        /// </summary>
        public string CommandTrigger
        {
            get
            {
                return _plugin.GetCommandTrigger();
            }
        }

        /// <summary>
        /// this is the actual plugin implementation
        /// </summary>
        /// <param name="cmdArgs">the arguments to the command that are passed in</param>
        /// <returns>returns the reult of the command processing</returns>
        public string GetCommandResult(string cmdArgs)
        {
            return _plugin.GetCommandResult(cmdArgs);
        }
    }

    /// <summary>
    /// This is the class i will use to utilize the plugins, it will maintain
    /// a list of all the plugin wrappers, it will also load the plugins from
    /// SessionSettings.plugindirectory
    /// some funky stuff going on here... i wanted the plugins to be loaded on the fly without ANY loss in performance.
    /// polling was out of the question, and loading then at every command is just stupid! 
    /// i dont want to restart the app if a plugin was added.
    /// if a new plugin is dropped in SessionSettings.plugindirectory
    /// i want it to just start working. no extra work needed, just drop the dll and trigger the command!
    /// very cool, right? :-)
    /// </summary>
    public class PluginProvider
    {
        public PluginProvider()
        {
            FileSystemWatcher folderWatcher = new FileSystemWatcher();
            folderWatcher.Filter = "*." + SessionSettings.pluginsextensions;
            folderWatcher.Path = Path.Combine(Environment.CurrentDirectory, SessionSettings.pluginsfolder);
            folderWatcher.EnableRaisingEvents = true;

            folderWatcher.Created += new FileSystemEventHandler(folderWatcher_Created);
            folderWatcher.Deleted += new FileSystemEventHandler(folderWatcher_Deleted);
            folderWatcher.Changed += new FileSystemEventHandler(folderWatcher_Changed);
            folderWatcher.Renamed += new RenamedEventHandler(folderWatcher_Renamed);
        }

        void folderWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Logger.LogInformation("a file (" + e.Name + ") in plugin folder has been added... reload the plugins!");
            needToReloadPlugins = true;
        }
        void folderWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Logger.LogInformation("a file (" + e.Name + ") in plugin folder has been removed... reload the plugins!");
            needToReloadPlugins = true;
        }
        void folderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Logger.LogInformation("a file (" + e.Name + ") in plugin folder has been modified (updated?)... reload the plugins!");
            needToReloadPlugins = true;
        }
        void folderWatcher_Renamed(object sender, FileSystemEventArgs e)
        {
            //i dont really care man... should i? //todo(4)
        }

        private bool needToReloadPlugins = true;

        private List<PluginWrapper> _plugins;
        public List<PluginWrapper> Plugins
        {
            get
            {
                if (_plugins == null)
                {
                    Logger.LogInformation("loading plugins...");
                    LoadPlugins();
                }
                else
                {
                    if (needToReloadPlugins)
                    {
                        Logger.LogInformation("change in plugins dir detected, reloading...");
                        LoadPlugins();
                    }
                }
                return _plugins;
            }
        }

        private void LoadPlugins()
        {
            if (_plugins == null)
            {
                _plugins = new List<PluginWrapper>();
            }
            else
            {
                Logger.LogInformation("reloading plugins...");
                _plugins.Clear();
            }
            List<IStatelessChatInterface> plugins = LoadPluginAssemblies();
            foreach (IStatelessChatInterface plugin in plugins)
            {
                PluginWrapper newPlugin = new PluginWrapper(plugin);
                bool pluginConflict = false;
                if (SessionSettings.builtincommands.Contains(newPlugin.CommandTrigger)) //todo(1) no no no no...
                {
                    Logger.LogInformation("ERROR! not using plugin " + newPlugin.CommandTrigger + " built in command exists!");
                    pluginConflict = true;
                }
                //we also need to check the trigger against other plugins that have already been loaded
                foreach (PluginWrapper loadedPlugin in _plugins)
                {
                    if (loadedPlugin.CommandTrigger == newPlugin.CommandTrigger)
                    {
                        Logger.LogInformation("ERROR! not using plugin " + newPlugin.CommandTrigger + " cnflict with a plugin that is already loaded!");
                        pluginConflict = true;
                    }
                }

                if (!pluginConflict)
                    _plugins.Add(newPlugin);
            }
            //loaded the plugins, so change the refresh flag
            needToReloadPlugins = false;
        }

        private List<IStatelessChatInterface> LoadPluginAssemblies()
        {
            List<Assembly> assemblies = LoadAssemblies();
            List<Type> availableTypes = new List<Type>();

            foreach (Assembly currentAssembly in assemblies)
                availableTypes.AddRange(currentAssembly.GetTypes());

            // get a list of objects that implement the IStatelessChatInterface interface AND 
            // have the StatelessChatPlugin attribute
            List<Type> pluginList = availableTypes.FindAll(delegate(Type t)
            {
                List<Type> interfaceTypes = new List<Type>(t.GetInterfaces());
                object[] arr = t.GetCustomAttributes(typeof(StatelessChatPluginAttribute), true);
                return !(arr == null || arr.Length == 0) && interfaceTypes.Contains(typeof(IStatelessChatInterface));
            });
            Logger.LogInformation("found " + pluginList.Count + " usable plugins");
            // convert the list of Objects to an instantiated list of ICalculators
            return pluginList.ConvertAll<IStatelessChatInterface>(delegate(Type t) { return Activator.CreateInstance(t) as IStatelessChatInterface; });
        }

        private List<Assembly> LoadAssemblies()
        {
            DirectoryInfo dInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, SessionSettings.pluginsfolder));
            FileInfo[] files = dInfo.GetFiles("*." + SessionSettings.pluginsextensions);
            List<Assembly> plugInAssemblyList = new List<Assembly>();
            
            if (null != files)
            {
                foreach (FileInfo file in files)
                {
                    plugInAssemblyList.Add(Assembly.LoadFile(file.FullName));
                }
            }
            Logger.LogInformation("found " + plugInAssemblyList.Count + " assemblies in " + dInfo.FullName);
            return plugInAssemblyList;
        }
    }
}
