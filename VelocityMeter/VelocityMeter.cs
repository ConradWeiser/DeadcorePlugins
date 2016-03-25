using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using DCPMCommon.Interfaces;
using DCPMCommon;

public class VelocityMeter : MonoBehaviour, LoadablePlugin
{
    Vector3 velocity;
    KeyCode meterButton;
    bool drawGUI = false;

    int left = 20;
    int height = 70;
    int width = 100;

    //Create the pluginInfo dictionary
    Dictionary<String, String> pluginInfo = new Dictionary<String, String>()
    {
        { "Name",   "Velocity Meter" },
        { "Author", "Standalone (aka Standalone27)" },
        { "Ver",    "1.ʕ•͡ᴥ•ʔ" },
        { "Desc",   "Adds a simple velocity meter UI to the game (Defaults to F4)" }
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
        LogMessage("=== Initialising Velocity Meter ===");
        //Check or create settings
        //Alternatively could use the DeadCore SettingsManager however this encrypts and serializes the data to settings.save and is not easily changed
        meterButton = DCPMSettings.GetKeyCodeSetting("DCPM-ToggleVelocityMeter", KeyCode.F4);

        DCPMMainConsole.Instance.ConsoleInput += ConsoleInput;

        LogMessage("=== Velocity Meter Initialised ===");
    }

    private void ConsoleInput(String[] args)
    {
        if (args.Length >= 1 && args[0] == "velocityui")
        {
            if (args.Length >= 2 && args[1] == "bind")
            {
                if (args.Length >= 3)
                {
                    try
                    {
                        meterButton = (KeyCode) System.Enum.Parse(typeof(KeyCode), args[2]);
                        DCPMSettings.SetSetting("DCPM-ToggleVelocityMeter", meterButton);
                    }
                    catch (Exception ex)
                    {
                        LogMessage("Error: Cannot convert {0} to UnityEngine.KeyCode", args[2]);
                    }
                }
                else
                {
                    LogMessage("'DCPM-ToggleVelocityMeter' = '{0}'", meterButton);
                }
            }       
        }
    }


    //Put stuff that you would normally put in the corresponding Unity method in the following methods
    //This is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
        {
            if (Input.GetKeyDown(meterButton))
            {
                drawGUI = !drawGUI;
            }
        }
    }

    //This is called once per physics frame (I believe the delay between calls is always 0.02 or close to it in DeadCore)
    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
        {
            velocity = Android.Instance.gameObject.transform.rigidbody.velocity;
        }
    }

    //This is called once per frame for GUI elements
    //Manipulating Unity GUI elements must be done in here
    void OnGUI()
    {
        //By the by I am shit at doing UI's, so this is probably 100% shit way of doing this, so eh.
        if (this.drawGUI && GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
        {
            GUI.Box(new Rect(left, Screen.height / 2 - height / 2, width, height), "Velocity");
            GUI.Label(new Rect(left + 10, Screen.height / 2 - height / 2 + 20, 55, 20), "H Speed: ");
            GUI.Label(new Rect(left + 70, Screen.height / 2 - height / 2 + 20, 20, 20), ((int)new Vector3(velocity.x, 0, velocity.z).magnitude).ToString());

            GUI.Label(new Rect(left + 10, Screen.height / 2 - height / 2 + 40, 55, 20), "V Speed: ");
            GUI.Label(new Rect(left + 70, Screen.height / 2 - height / 2 + 40, 20, 20), ((int)new Vector3(0, velocity.y, 0).magnitude).ToString());
        }
    }

    //Log a message to the default log file
    private void LogMessage(String message, params System.Object[] args)
    {
        DCPMLogger.LogMessage("[VelocityMeter] " + message, args : args);
    }
}