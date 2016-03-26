// system imports for base functionality
using System;
using System.Collections.Generic;
// library reference imports
using UnityEngine;
using DCPMCommon.Interfaces;
using DCPMCommon;
using System.Diagnostics;
using LevelCoordinates;
using System.Text;
// class 
public class VelocityMeter : MonoBehaviour, LoadablePlugin
{
    KeyCode timerButton;

    int left = 20;
    int height = 30;
    int width = 100;

    bool timerStart = true;

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

        if (args.Length == 1 && args[0] == "coord") {
            LogMessage("=== Current Coordinates ===");
            LogMessage("= X ==> " + Math.Round(Android.Instance.gameObject.transform.rigidbody.position.x));
            LogMessage("= Y ==> " + Math.Round(Android.Instance.gameObject.transform.rigidbody.position.y));
            LogMessage("= Z ==> " + Math.Round(Android.Instance.gameObject.transform.rigidbody.position.z));
        }

    }


    //Put stuff that you would normally put in the corresponding Unity method in the following methods
    //This is called once per frame
    private int counter = 0, index = 0;
    private Vector3 currentPosition = new Vector3();
    private List<LevelCoordinate> LevelCoordinates = null;
    private LevelCoordinateIF LevelCoordinateManager = null;
    void Update() {

        if (Application.isLoadingLevel)
        {
            //This is needed to make sure the timer starts as soon as the player is able to move. The timer start is triggered based off of this temporary flag
            timerStart = true;
        }

        this.currentPosition = Android.Instance.gameObject.transform.rigidbody.position;

        // handle if the level coordinates are currently undefined
        if (this.LevelCoordinates == null && Application.loadedLevel >= 8 && Application.loadedLevel <= 12) {
            // try to define the current level coordinates all the time
            this.LevelCoordinateManager = LevelCoordinateActivator.GetInstance(Application.loadedLevel);
            this.LevelCoordinates = this.LevelCoordinateManager.GetCoordinateList();
        // ignore when the user is already going through a level
        } else if (Application.loadedLevel >= 8 && Application.loadedLevel <= 12) {
        // otherwise, handle when the level coordinates are already defined
        } else {
            // check if the user is not in a level at the moment
            if (Application.loadedLevel == 4 || Application.loadedLevel == 3) {
                // this means the user is not in the game anymore, unload the coordinates
                this.LevelCoordinateManager = null;
                this.LevelCoordinates = null;
            }
        }

        // handle the updates when the list of coordinates is already defined
        if (this.LevelCoordinates != null) {
            // check if the counter is on a checking frame, return early if not right frame
            if (((this.counter += 1) % 30) != 0) return;
            // get the player's current position
            this.currentPosition = Android.Instance.gameObject.transform.rigidbody.position;
            // check whether the user is within range of the current coordinate
            if (!this.LevelCoordinates[index].CheckCollide(this.currentPosition)) return;
            // set the visited time of the current coordinate to the current elapsed time
            if (this.LevelCoordinates[index].Visited) return;
            this.LevelCoordinates[index].VisitedTime = this.timer.Elapsed;
            this.LevelCoordinates[index].Visited = true;
            // if within range, increment the index variable
            if (this.index == this.LevelCoordinates.Count) return;
            this.index = this.index + 1;
        }
    }

    private const int BOX_OFFSET = 20;
    private const int BOX_WIDTH = 200;
    private const int LABEL_WIDTH = 120;
    private const int LINE_HEIGHT = 20;
    // method to be called every time the gui is updated
    void OnGUI()
    {
        // first check that the set of level coordinates is not null
        if (this.LevelCoordinates == null) return;
        // check if the timer has been started yet.  if not, start it.
        if (!this.timer.IsRunning) this.timer.Start();
        // construct the box that holds all of the labels in it
        int totalHeight = (this.LevelCoordinates.Count + 1) * LINE_HEIGHT;
        GUI.Box(new Rect(BOX_OFFSET, BOX_OFFSET, BOX_WIDTH, totalHeight), this.timer.Elapsed.ToString());
        // loop through all of the LevelCoordinate objects and display them in the GUI
        for (int i = 0; i < this.LevelCoordinates.Count; i ++) {
            // calculate the new height of the current label
            int labelHeight = (i + 1) * LINE_HEIGHT;
            // check if the current coordinate has already been visited or not
            String coordinateNameString = null;
            String coordinateTimeString = null;
            if (this.LevelCoordinates[i].Visited) {
                // construct a stringbuilder for the name of the current coordinate
                StringBuilder coordinateNameStringBuilder = new StringBuilder();
                // set the color as blue to indicate that it has already been visited
                coordinateNameStringBuilder.Append("<color=#0000ffff>");
                coordinateNameStringBuilder.Append(this.LevelCoordinates[i].Name);
                coordinateNameStringBuilder.Append("</color>");
                coordinateNameString = coordinateNameStringBuilder.ToString();
                // construct a stringbuilder for the time of the current coordinate
                StringBuilder coordinateTimeStringBuilder = new StringBuilder();
                // set the color as green for the time
                coordinateTimeStringBuilder.Append(this.LevelCoordinates[i].ElapsedTimeString());
                coordinateTimeString = coordinateTimeStringBuilder.ToString();
            // handle the coordinates that have not yet been visited
            } else {
                // construct a stringbuilder for the name of the current coordinate
                StringBuilder coordinateNameStringBuilder = new StringBuilder();
                // set the color as blue to indicate that it has already been visited
                coordinateNameStringBuilder.Append(this.LevelCoordinates[i].Name);
                coordinateNameString = coordinateNameStringBuilder.ToString();
                // construct a stringbuilder for the time of the current coordinate
                StringBuilder coordinateTimeStringBuilder = new StringBuilder();
                coordinateTimeStringBuilder.Append(this.LevelCoordinates[i].ElapsedTimeString());
                coordinateTimeString = coordinateTimeStringBuilder.ToString();
            }
            // write the labels onto the gui box
            GUI.Label(new Rect(BOX_OFFSET + 5, BOX_OFFSET + labelHeight, LABEL_WIDTH, LINE_HEIGHT), coordinateNameString);
            GUIStyle timeStyle = GUI.skin.GetStyle("Label"); timeStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(BOX_OFFSET + LABEL_WIDTH, BOX_OFFSET + labelHeight, LABEL_WIDTH, LINE_HEIGHT), coordinateTimeString);
        }
    }

    //Log a message to the default log file
    private void LogMessage(String message, params System.Object[] args)
    {
        DCPMLogger.LogMessage("[Timer] " + message, args: args);
    }
}



