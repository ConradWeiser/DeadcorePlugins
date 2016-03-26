using System;
using System.Collections.Generic;

using UnityEngine;
using DCPMCommon.Interfaces;
using DCPMCommon;

public class PluginTemplate : MonoBehaviour, LoadablePlugin
{

    KeyCode resetButton;
    Dictionary<String, String> pluginInfo = new Dictionary<String, String>()
    {
        { "Name",   "Restart Level" },
        { "Author", "Conrad Weiser" },
        { "Ver",    "0.1" },
        { "Desc",   "A button to restart the game level" }
    };

    /*
     _                 _      _    _     ___ _           _        ___     _            __             
    | |   ___  __ _ __| |__ _| |__| |___| _ \ |_  _ __ _(_)_ _   |_ _|_ _| |_ ___ _ _ / _|__ _ __ ___ 
    | |__/ _ \/ _` / _` / _` | '_ \ / -_)  _/ | || / _` | | ' \   | || ' \  _/ -_) '_|  _/ _` / _/ -_)
    |____\___/\__,_\__,_\__,_|_.__/_\___|_| |_|\_,_\__, |_|_||_| |___|_||_\__\___|_| |_| \__,_\__\___|
                                                |___/                                              
    */

    public void Unload()
    {
        LogMessage("Unloading plugin");
        GameObject.Destroy(this);
    }

    public Dictionary<String, String> GetPluginInfo()
    {
        return pluginInfo;
    }



    void Awake()
    {
        //Attach to the input from the DCPMMainConsole
        LogMessage("=== Creating Level Reset Links ===");
        DCPMMainConsole.Instance.ConsoleInput += ConsoleInput;
        resetButton = DCPMSettings.GetKeyCodeSetting("DCPM-RestartLevel", KeyCode.F5);
        LogMessage("===Done. Stuff didn't break! ===");
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
        {
            if (Input.GetKeyDown(resetButton))
            {
                Application.LoadLevel(Application.loadedLevel);
                LogMessage("Loaded: {0} - Having an index of {1}", Application.loadedLevelName, Application.loadedLevel);
            }
        }
    }



    //Log a message to the default logfile without needing the namespace qualifier
    private void LogMessage(String message, params System.Object[] args)
    {
        //Using a named argument since there is also an optional logFile parameter
        DCPMLogger.LogMessage("[" + pluginInfo["Name"] + "] " + message, args: args);
    }

    private void ConsoleInput(String[] args)
    {
        if (args.Length >= 1 && args[0] == "resetbutton")
        {
            if (args.Length >= 2 && args[1] == "bind")
            {
                if (args.Length >= 3)
                {
                    try
                    {
                        resetButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), args[2]);
                        DCPMSettings.SetSetting("DCPM-RestartLevel", resetButton);
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Error: Cannot convert {0} to UnityEngine.KeyCode", args[2]);
                    }
                }
                else
                {
                    LogMessage("'DCPM-RestartLevel' = '{0}'", resetButton);
                }
            }
        }
    }
}
