// system imports for base functionality
using System;
using System.Collections.Generic;
// library reference imports
using UnityEngine;
using DCPMCommon.Interfaces;
using DCPMCommon;
using System.Diagnostics;
using LevelCoordinates;
using System.IO;
// class 
public class VelocityMeter : MonoBehaviour, LoadablePlugin
{
    KeyCode timerButton;
    bool Active = true;

    int left = 20;
    int height = 30;
    int width = 100;

    public Stopwatch timer;


    //Create the pluginInfo dictionary
    Dictionary<String, String> pluginInfo = new Dictionary<String, String>()
    {
        { "Name",   "Timer" },
        { "Author", "Conrad Weiser" },
        { "Ver",    "0.1" },
        { "Desc",   "Adds a stopwatch on the topleft." }
    };

    /*
     _                 _      _    _     ___ _           _        ___     _            __             
    | |   ___  __ _ __| |__ _| |__| |___| _ \ |_  _ __ _(_)_ _   |_ _|_ _| |_ ___ _ _ / _|__ _ __ ___ 
    | |__/ _ \/ _` / _` / _` | '_ \ / -_)  _/ | || / _` | | ' \   | || ' \  _/ -_) '_|  _/ _` / _/ -_)
    |____\___/\__,_\__,_\__,_|_.__/_\___|_| |_|\_,_\__, |_|_||_| |___|_||_\__\___|_| |_| \__,_\__\___|
                                                |___/                                              
    */

    //Destroy this script component and any other script components that this plugin may have created
    public void Unload()
    {
        LogMessage("Unloading plugin");
        GameObject.Destroy(this);
    }

    public Dictionary<String, String> GetPluginInfo()
    {
        return pluginInfo;
    }

    /*
     _   _      _ _          __  __     _   _            _    
    | | | |_ _ (_) |_ _  _  |  \/  |___| |_| |_  ___  __| |___
    | |_| | ' \| |  _| || | | |\/| / -_)  _| ' \/ _ \/ _` (_-<
     \___/|_||_|_|\__|\_, | |_|  |_\___|\__|_||_\___/\__,_/__/
                      |__/                                    
    */

    void Awake()
    {
        LogMessage("=== Creating Stopwatch links ===");
        //Check or create settings
        //Alternatively could use the DeadCore SettingsManager however this encrypts and serializes the data to settings.save and is not easily changed
        timerButton = DCPMSettings.GetKeyCodeSetting("DCPM-ToggleTimer", KeyCode.Q);


        DCPMMainConsole.Instance.ConsoleInput += ConsoleInput;
        timer = new Stopwatch();

        LogMessage("=== Stopwatch init complete ===");
    }

    private void ConsoleInput(String[] args)
    {
        if (args.Length >= 1 && args[0] == "timer")
        {
            if (args.Length >= 2 && args[1] == "bind")
            {
                if (args.Length >= 3)
                {
                    try
                    {
                        timerButton = (KeyCode)System.Enum.Parse(typeof(KeyCode), args[2]);
                        DCPMSettings.SetSetting("DCPM-ToggleTimer", timerButton);
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Error: Cannot convert {0} to UnityEngine.KeyCode", args[2]);
                    }
                }
                else
                {
                    LogMessage("'DCPM-ToggleTimer' = '{0}'", timerButton);
                }
            }
        }


    }


    //Put stuff that you would normally put in the corresponding Unity method in the following methods
    //This is called once per frame
    private List<LevelCoordinate> LevelCoordinates = null;
    private LevelCoordinateIF LevelCoordinateManager = null;
    void Update() {
        // handle if the level coordinates are currently undefined
        if (this.LevelCoordinates == null && Application.loadedLevel >= 8 && Application.loadedLevel <= 12) {
            // try to define the current level coordinates all the time
            this.LevelCoordinateManager = LevelCoordinateActivator.GetInstance(Application.loadedLevel);
            this.LevelCoordinates = this.LevelCoordinateManager.GetCoordinateList();
        } else if (Application.loadedLevel >= 8 && Application.loadedLevel <= 12) {
            // rofl idk
            // ignore
        // otherwise, handle when the level coordinates are already defined
        } else {
            // check if the user is not in a level at the moment
            LogMessage("Test4a: {0}\n", LevelCoordinateActivator.GetString());
            if (Application.loadedLevel == 4) {
                // this means the user is not in the game anymore, unload the coordinates
                this.LevelCoordinateManager = null;
            }
        }
    }


    // method to be called every time the gui is updated
    void OnGUI()
    {
        if (this.Active && GameManager.Instance.CurrentGameState == GameManager.GameState.InGame) {
            // create a box that tracks the player's time elapsed
            GUI.Box(new Rect(left, Screen.height / 2 - height / 2 - 200, width, height), "Stopwatch");
            String time = String.Format("{0}.{1}", timer.Elapsed.Seconds, timer.Elapsed.Milliseconds);
            GUI.Label(new Rect(left + 10, Screen.height / 2 - height / 2 - 180, 100, 20), time);
        }
        if (this.LevelCoordinates != null) {
            int offset = 20;
            GUI.Box(new Rect(left, Screen.height / 2 - height / 2 - 100, width * 2, height + ((this.LevelCoordinates.Count + 1) * offset)), "LevelLoading");
            foreach(LevelCoordinate coordinate in this.LevelCoordinates) {
                GUI.Label(new Rect(left + 10, Screen.height / 2 - height / 2 - 100 + offset, (width * 2) - 20, 25), coordinate.ToString());
                offset += 15;
            }
        }
    }

    //Log a message to the default log file
    private void LogMessage(String message, params System.Object[] args)
    {
        DCPMLogger.LogMessage("[Timer] " + message, args: args);
    }
}