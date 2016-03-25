using System;
using System.Collections.Generic;

using DCPMCommon;
using DCPMCommon.Interfaces;
using UnityEngine;

public class DCPMMainConsole : MonoBehaviour, LoadablePlugin
{
    /*
     ___       _          __  __           _                 
    |   \ __ _| |_ __ _  |  \/  |___ _ __ | |__  ___ _ _ ___ 
    | |) / _` |  _/ _` | | |\/| / -_) '  \| '_ \/ -_) '_(_-< 
    |___/\__,_|\__\__,_| |_|  |_\___|_|_|_|_.__/\___|_| /__/ 

    */

    //Create the pluginInfo dictionary
    Dictionary<String, String> pluginInfo = new Dictionary<String, String>()
    {
        { "Name",   "Plugin Manager Main Console" },
        { "Author", "Standalone (aka Standalone27)" },
        { "Ver",    "1.ʕ•͡ᴥ•ʔ" },
        { "Desc",   "Adds a simple scrollable UI console with text input for monitoring commands to plugins" }
    };

    private static DCPMMainConsole instance;
    public static DCPMMainConsole Instance
    {
        get
        {
            //Should never be null since the MainConsole is the first plugin to be created and attached
            //so all plugins (Including the main DCPM) should try to access the Instance property after it has been set by the Awake method
            return instance;
        }
    }

    //Event that plugins can subscribe to to scan user input to the console
    //input to the console is split with the space character as a separator
    /* eg:
        Input: 'dcpm unload VelocityMeter'
        [0] = dcpm
        [1] = unload
        [2] = VelocityMeter
    */
    public delegate void ConsoleInputCallback(String[] input);
    public event ConsoleInputCallback ConsoleInput;

    private KeyCode consoleButton;

    #region Console UI Members

    private bool drawGUI;

    //Position variables
    private Rect boxRect;
    private Rect scrollableRect;
    private Rect textInputRect;
    private Rect submitButtonRect;

    //Custom label style to remove padding
    private GUIStyle textLineStyle;

    private String consoleInput;
    private Queue<String> textQueue;

    //Tracks the scroll position
    private Vector2 scrollPos;

    #endregion

    /*
     _                 _      _    _     ___ _           _        ___     _            __             
    | |   ___  __ _ __| |__ _| |__| |___| _ \ |_  _ __ _(_)_ _   |_ _|_ _| |_ ___ _ _ / _|__ _ __ ___ 
    | |__/ _ \/ _` / _` / _` | '_ \ / -_)  _/ | || / _` | | ' \   | || ' \  _/ -_) '_|  _/ _` / _/ -_)
    |____\___/\__,_\__,_\__,_|_.__/_\___|_| |_|\_,_\__, |_|_||_| |___|_||_\__\___|_| |_| \__,_\__\___|
                                                |___/                                              
    */

    public void Unload()
    {
        LogMessage("Unload called, doing nothing since this plugin is required");
        //Do nothing on Unload since this plugin should never be unloaded
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
        LogMessage("=== Initialising Main Console ===");
        instance = this;

        consoleButton = DCPMSettings.GetKeyCodeSetting("DCPM-ConsoleButton", KeyCode.BackQuote);
        textQueue = new Queue<String>();
        drawGUI = false;
        consoleInput = "";

        boxRect =           new Rect(10, 10, 1000, 600);
        scrollableRect =    new Rect(boxRect.left + 5, boxRect.top, boxRect.width - 5, boxRect.height - 30);
        textInputRect =     new Rect(boxRect.left + 5, boxRect.top + boxRect.height - 25, boxRect.width - 100, 20);
        submitButtonRect =  new Rect(boxRect.left + boxRect.width - 95, boxRect.top + boxRect.height - 25, 85, 20);

        //Whenever a message is logged by a plugin
        try
        {
            DCPMLogger.MessageLogged += MessageLogged;
        }
        catch (Exception ex)
        {
            LogMessage(ex.ToString());
        }
        
        LogMessage("=== Main Console Initialised ===");
    }

    void Update()
    {
        if (Input.GetKeyDown(consoleButton))
        {
            if (drawGUI)
            {
                drawGUI = false;
                //If in game then disable the cursor when we close the console
                if (GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
                {
                    Screen.showCursor = false;
                    Screen.lockCursor = true;
                }

                MouseLook[] lookArray = UnityEngine.Object.FindObjectsOfType(typeof(MouseLook)) as MouseLook[];
                foreach (MouseLook look in lookArray)
                {
                    look.enabled = true;
                }
            }
            else
            {
                //Enable the cursor when we open the console
                drawGUI = true;
                Screen.showCursor = true;
                Screen.lockCursor = false;
                
                MouseLook[] lookArray = UnityEngine.Object.FindObjectsOfType(typeof(MouseLook)) as MouseLook[];
                foreach (MouseLook look in lookArray)
                {
                    look.enabled = false;
                }
            }
        }
    }

    void OnGUI()
    {
        if (drawGUI)
        {
            if (textLineStyle == null)
            {
                textLineStyle = new GUIStyle(GUI.skin.label);
                textLineStyle.margin = new RectOffset(0, 0, 0, 0);
            }

            GUI.Box(boxRect, "");
            GUILayout.BeginArea(scrollableRect);
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(scrollableRect.width), GUILayout.Height(scrollableRect.height));

            foreach (String line in textQueue)
            {
                GUILayout.Label(line, textLineStyle);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (/*Event.current.isKey &&*/ Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return)
            {
                Submit(consoleInput);
            }

            consoleInput = GUI.TextField(textInputRect, consoleInput);


            if (GUI.Button(submitButtonRect, "Submit"))
            {
                Submit(consoleInput);
            }
        }
    }

    /*
     ___      _    _ _      __  __     _   _            _    
    | _ \_  _| |__| (_)__  |  \/  |___| |_| |_  ___  __| |___
    |  _/ || | '_ \ | / _| | |\/| / -_)  _| ' \/ _ \/ _` (_-<
    |_|  \_,_|_.__/_|_\__| |_|  |_\___|\__|_||_\___/\__,_/__/                                            

    */

    /*
     ___     _          _         __  __     _   _            _    
    | _ \_ _(_)_ ____ _| |_ ___  |  \/  |___| |_| |_  ___  __| |___
    |  _/ '_| \ V / _` |  _/ -_) | |\/| / -_)  _| ' \/ _ \/ _` (_-<
    |_| |_| |_|\_/\__,_|\__\___| |_|  |_\___|\__|_||_\___/\__,_/__/                                                      

    */

    private char[] trimParams = { ' ' };
    private void Submit(String input)
    {
        //Check if input is blank or all spaces, in which case do nothing
        if (input.TrimStart(trimParams) != "")
        {
            DCPMLogger.LogMessage(input);

            if (input == "clear")
            {
                textQueue.Clear();
            }
            else
            {
                //Fire the ConsoleInput event
                if (ConsoleInput != null)
                {
                    foreach (ConsoleInputCallback d in ConsoleInput.GetInvocationList())
                    {
                        //Encapsulated in a try block as the methods that plugin authors attach may cause an exception
                        try
                        {
                            d.Invoke(input.Split(' '));
                        }
                        catch (Exception ex)
                        {
                            //Remove the delegate call so it does not cause further exceptions
                            ConsoleInput -= d;
                            LogMessage("Delegate method " + d.Method + " has been removed as it causes an Exception");
                            LogMessage(ex.ToString());
                        }
                    }
                }
            }
        } 
    }

    //This is called whenever the MessageLogged event is fired from the DCPMLogger class
    //Never call the LogMessage method from this event, it will cause a stack overflow
    private void MessageLogged(String input)
    {
        textQueue.Enqueue(input);
    }

    private void LogMessage(String message, params System.Object[] args)
    {
        DCPMLogger.LogMessage("[MainConsole] " + message, args: args);
    }
}

