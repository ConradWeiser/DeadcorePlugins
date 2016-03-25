using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

using DCPMCommon.Interfaces;
using DCPMCommon;
using DCPM;

namespace DCPM
{
    //Made this a static class since the Instance getter was pointless since it will never be accessed by anything more than the GameManager
    public static class DCPMPluginManager
    {
        /*
         ___       _          __  __           _                 
        |   \ __ _| |_ __ _  |  \/  |___ _ __ | |__  ___ _ _ ___ 
        | |) / _` |  _/ _` | | |\/| / -_) '  \| '_ \/ -_) '_(_-< 
        |___/\__,_|\__\__,_| |_|  |_\___|_|_|_|_.__/\___|_| /__/ 

         */

        //Plugins location
        private static String pluginsLocation;
        public static String PluginsLocation
        {
            get
            {
                return pluginsLocation;
            }
        }

        //List of validated MonoBehaviour plugin types
        private static List<Type> validPluginTypes;

        //List of plugins to load and attach to the GameManager's GameObject
        private static List<MonoBehaviour> loadedPlugins;
        //private static Dictionary<String, AppDomain> pluginAppDomains;

        /*
          ___             _               _              
         / __|___ _ _  __| |_ _ _ _  _ __| |_ ___ _ _ ___
        | (__/ _ \ ' \(_-<  _| '_| || / _|  _/ _ \ '_(_-<
         \___\___/_||_/__/\__|_|  \_,_\__|\__\___/_| /__/
                                                      
        */

        //Static Constructor, called before any static member is accessed
        static DCPMPluginManager()
        {
            LogMessage("=== Initializing Plugin Manager ===");

            pluginsLocation = Environment.CurrentDirectory + "\\DeadCore_Data\\Managed\\plugins\\";
            validPluginTypes = new List<Type>();
            loadedPlugins = new List<MonoBehaviour>();
            //pluginAppDomains = new Dictionary<String, AppDomain>();  

            LogMessage("=== Variables Initialized ===");

            LogMessage("=== Loading Main Console ===");
            loadedPlugins.Add((MonoBehaviour)GameManager.Instance.gameObject.AddComponent(typeof(DCPMMainConsole)));

            //Loads valid types from assemblies in the plugins folder
            //Does not instatiate anything
            LoadPlugins();

            //Attaches the valid plugins to the base GameObject of DeadCore's GameManager script, this GameObject persists through level loads
            AttachPlugins();

            //Subscribe to the ConsoleInput event in DCPMMainConsole
            DCPMMainConsole.Instance.ConsoleInput += ConsoleInput;
        }

        /*
         ___     _          _         __  __     _   _            _    
        | _ \_ _(_)_ ____ _| |_ ___  |  \/  |___| |_| |_  ___  __| |___
        |  _/ '_| \ V / _` |  _/ -_) | |\/| / -_)  _| ' \/ _ \/ _` (_-<
        |_| |_| |_|\_/\__,_|\__\___| |_|  |_\___|\__|_||_\___/\__,_/__/                                                      

        */

        //Called on construction of PluginManager to load the plugins in the pluginsLocation
        //Things commented out here are an experiment with AppDomains and being able to unload/load plugins on the fly
        private static void LoadPlugins()
        {
            //List<Assembly> assemblies = new List<Assembly>();
            //validPluginTypes.Clear();
            //validPluginTypes.Add(typeof(DCPMMainConsole));

            Assembly assembly;
            //AppDomain pluginDomain;
            //bool valid = false;

            foreach (String dll in Directory.GetFiles(PluginsLocation, "*.dll"))
            {
                //valid = false;
                try
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dll);
                    //pluginDomain = AppDomain.CreateDomain(an.Name);
                    assembly = Assembly.Load(an);

                    LogMessage(an.FullName);

                    foreach (Type t in assembly.GetTypes())
                    {
                        LogMessage("    " + t.FullName);

                        if (t.IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            LogMessage("    ^ Valid MonoBehaviour");
                            if (typeof(LoadablePlugin).IsAssignableFrom(t))
                            {
                                LogMessage("    ^ Implements LoadablePlugin, adding to auto load");
                                //pluginAppDomains.Add(t.Name, pluginDomain);
                                validPluginTypes.Add(t);
                                //valid = true;
                            }
                        }
                        else
                        {
                            LogMessage("    ^ Does not inherit UnityEngine.MonoBehaviour");
                        }
                    }

                    /*
                    if (!valid)
                    {
                        AppDomain.Unload(pluginDomain);
                    }
                    */
                }
                catch (Exception ex)
                {
                    LogMessage(ex.ToString());
                }
            }
        }

        //Creates and attaches classes that implemented the LoadablePlugin interface to the GameManager's base GameObject that persists through level loads
        private static void AttachPlugins()
        {
            foreach (Type t in validPluginTypes)
            {
                loadedPlugins.Add((MonoBehaviour)GameManager.Instance.gameObject.AddComponent(t));
            }
        }

        //Callback for the ConsoleInput event in DCPMMainConsole
        private static void ConsoleInput(String[] input)
        {
            //Check to see if the command entered is relevant to the DeadCore Plugin Manager
            if (input.Length >= 1 && input[0] == "dcpm")
            {
                //Plugin related commands
                if (input.Length >= 2 && input[1] == "plugins")
                {
                    //List all loaded plugins
                    if (input.Length >= 3 && input[2] == "list")
                    {
                        int c = 0;
                        foreach (LoadablePlugin plugin in loadedPlugins)
                        {
                            Dictionary<String, String> info = plugin.GetPluginInfo();
                            LogMessage(c++ + ": " + info["Name"] + " version " + info["Ver"] + " by " + info["Author"]);
                        }
                    }

                    
                    //Display the Description of a specific plugin
                    if (input.Length >= 3 && input[2] == "info")
                    {
                        if (input.Length >= 4)
                        {
                            int index;

                            if (int.TryParse(input[3], out index))
                            {
                                //Using try catch here because lazy at the moment and can't be bothered checking to see if each key exists
                                try
                                {
                                    Dictionary<String, String> info = ((LoadablePlugin)loadedPlugins[index]).GetPluginInfo();
                                    LogMessage(index + ": " + info["Name"] + " version " + info["Ver"] + " by " + info["Author"]);
                                    LogMessage("Description: " + info["Desc"]);
                                }
                                catch (Exception ex)
                                {
                                    LogMessage("The requested plugin does not implement the pluginInfo dictionary properly and caused the following exception");
                                    LogMessage(ex.ToString());
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        //Log a message to the default logfile
        private static void LogMessage(String message, params System.Object[] args)
        {
            DCPMLogger.LogMessage("[PluginManager] " + message, args : args);
        }

        /*
         ___      _    _ _      __  __     _   _            _    
        | _ \_  _| |__| (_)__  |  \/  |___| |_| |_  ___  __| |___
        |  _/ || | '_ \ | / _| | |\/| / -_)  _| ' \/ _ \/ _` (_-<
        |_|  \_,_|_.__/_|_\__| |_|  |_\___|\__|_||_\___/\__,_/__/                                            

        */

        //Called once by DeadCore's GameManager static instance property getter
        //Probably a better way to do this instead of having dummy method, but eh
        public static void Initialize()
        {
            LogMessage("=== Plugin Manager Initialized ===");
        }
    }
}
