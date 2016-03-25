using System;
using System.Collections.Generic;

using UnityEngine;
using DCPMCommon.Interfaces;
using DCPMCommon;

public class PluginTemplate : MonoBehaviour, LoadablePlugin
{

    KeyCode guiButton;

    int left = 20;
    int height = 40;
    int width = 100;
    bool Active = true;

    private Vector3 currentPosition = new Vector3();
    private Vector3 indicatorSize = new Vector3(5f, 5f, 5f);
    private const int UPDATE_RATE = 60; // refresh per update
    // variables dealing with the history of the player's position
    private const int QUEUE_MAX = 200; // max element in the queue
    private Queue<Vector3> positionHistoryQueue = new Queue<Vector3>(200);
    private GameObject lastRenderedGameObject = null;



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
        LogMessage("==>Starting up Ghost<==");
        guiButton = DCPMSettings.GetKeyCodeSetting("DCPM-ToggleTimer", KeyCode.F1);
    }

 
    void Update()
    {
        //Put stuff that you would normally put in the corresponding Unity method in the following methods
        //This is called once per frame

        if (Input.GetKeyDown(guiButton))
        {
            Active = !Active;
        }
    }


    void FixedUpdate()
    {
        //This is called once per physics frame (I believe the delay between calls is always 0.02 or close to it in DeadCore)
        // check if the last rendered player history object is not null...
        if (this.lastRenderedGameObject != null) GameObject.DestroyObject(this.lastRenderedGameObject);
        // add the new position to the queue....
        this.positionHistoryQueue.Enqueue(this.currentPosition);
        // check if the queue is full. if so, make a new game object from the last object.
        if (this.positionHistoryQueue.Count == QUEUE_MAX - 1)
        {
            // create a new game object from the last element
            this.lastRenderedGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            this.lastRenderedGameObject.collider.enabled = false;
            // get the oldest coordinate from the queue
            Vector3 oldestCoordinate = this.positionHistoryQueue.Dequeue();
            // move the last rendered object to the target position
            this.lastRenderedGameObject.transform.position = oldestCoordinate;
        }
    }

    // method that will return a convenient array for the coordinates
    private String[] getLabels(Vector3 coordinates)
    {
        String[] sa = new String[3];
        sa[0] = String.Format("X ==> {0:#.00}", coordinates.x);
        sa[1] = String.Format("Y ==> {0:#.00}", coordinates.y);
        sa[2] = String.Format("Z ==> {0:#.00}", coordinates.z);
        return sa;
    }



    void OnGUI()
    {
        if (this.Active && GameManager.Instance.CurrentGameState == GameManager.GameState.InGame)
        {
            /* position tracker */
            // get the last position of the player
            this.currentPosition = Android.Instance.gameObject.transform.rigidbody.position;
            // create a box that tracks the player's position in the world
            GUI.Box(new Rect(left, (Screen.height / 2) - (height / 2) - 400, width + 10, (height * 2)), "Player Position:");
            // draw the labels on the box
            int offset = 20;
            foreach (String coordinateString in this.getLabels(this.currentPosition))
            {
                // draw each label with the corresponding string in the array
                GUI.Label(new Rect(left + 10, (Screen.height / 2) - (height / 2) - (400 - offset), 150, 30), coordinateString);
                // increment the offset to make it look good
                offset += 15;
            }
        }
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
}

 