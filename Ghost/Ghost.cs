using System;
using System.Collections.Generic;

using UnityEngine;
using DCPMCommon.Interfaces;
using DCPMCommon;

public class PluginTemplate : MonoBehaviour, LoadablePlugin
{


    Dictionary<String, String> pluginInfo = new Dictionary<String, String>()
    {
        { "Name",   "" },
        { "Author", "" },
        { "Ver",    "" },
        { "Desc",   "" }
    };


    public void Unload()
    {
        //Clean up any MonoBehaviour scripts or GameObjects you may have created here
    }

    public Dictionary<String, String> GetPluginInfo()
    {
        return pluginInfo;
    }



    void Awake()
    {
        //Attach to the input from the DCPMMainConsole
        DCPMMainConsole.Instance.ConsoleInput += ConsoleInput;
    }

    void Update()
    {
        //Put stuff that you would normally put in the corresponding Unity method in the following methods
        //This is called once per frame
    }

    void FixedUpdate()
    {
        //This is called once per physics frame (I believe the delay between calls is always 0.02 or close to it in DeadCore)
    }

    void OnGUI()
    {
        //Unity's OnGUI method
        //All Unity GUI manipulation must be done here
    }
   

    private void ConsoleInput(String[] input)
    {
        throw new NotImplementedException();
    }

    //Log a message to the default logfile without needing the namespace qualifier
    private void LogMessage(String message, params System.Object[] args)
    {
        //Using a named argument since there is also an optional logFile parameter
        DCPMLogger.LogMessage("[" + pluginInfo["Name"] + "] " + message, args: args);
    }

 