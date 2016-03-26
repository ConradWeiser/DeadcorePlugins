// namespace for the level coordinates
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// namespace for hiding 
namespace LevelCoordinates { 

    // interface to define all method for the level coordinates
    public interface LevelCoordinateIF {
        // method to get the list of checkpoint coordinates
        List<LevelCoordinate> GetCoordinateList();
    }

    // abstract class to define common behaviors and non-public components
    internal abstract class AbstractLevelCoordinate : LevelCoordinateIF {
        // private field for accessing the coordinates - defined in implementing constructors
        protected List<LevelCoordinate> coordinateList;
        // implement the getCoordinateList method
        public List<LevelCoordinate> GetCoordinateList() {
            // return the coordinate list
            return this.coordinateList;
        }
        // make the ToString method abstract
        public abstract override string ToString();
    }

    // class to act as a proxy for the level coordinate objects
    public class LevelCoordinateActivator : LevelCoordinateIF {
        // private field for the instance of this class
        private static LevelCoordinateActivator Instance = null;
        // private field for the level coordinate object
        private AbstractLevelCoordinate LevelCoordinateObject;
        // make the constructor private so as to assert the singleton design pattern
        private LevelCoordinateActivator() { }
        // method to get an instance of the 
        public static LevelCoordinateIF GetInstance(int levelID) {
            // check if the instance of the LevelCoordinateActivator is null
            if (Instance == null) Instance = new LevelCoordinateActivator();
            // construct the type name
            StringBuilder typeBuilder = new StringBuilder();
            typeBuilder.Append("LevelCoordinates.Level").Append(Hash(levelID)).Append("Coordinates");
            // building the instance of the actual coordinate object
            // attempt to get a concrete type of the target class
            Type type = null;
            try { type = Type.GetType(typeBuilder.ToString());
            } catch (Exception ex) { DCPMCommon.DCPMLogger.LogMessage(ex.Message); }
            // attempt to get a concrete object of the target class
            try { Instance.LevelCoordinateObject = (AbstractLevelCoordinate) Activator.CreateInstance(type);
            } catch (Exception ex) { DCPMCommon.DCPMLogger.LogMessage(ex.Message); }
            // return the level activator object
            return Instance;
        }
        // delegate the task to the private level coordinate object
        public List<LevelCoordinate> GetCoordinateList() {
            // return the coordinate list of the instantiated object
            if (this.LevelCoordinateObject == null) { return null; }
            return this.LevelCoordinateObject.GetCoordinateList();
        }
        public static String GetString() {
            return "test string";
        }
        // method to convert the level ID into the level number
        private static int Hash(int levelID) {
            // iterate through the level values
            DCPMCommon.DCPMLogger.LogMessage("Processing hash code "+levelID+"\n");
            switch (levelID) {
                case  8: return 1; // level 1's ID is 7
                case  9: return 2; // level 2's ID is 9
                case 10: return 3; // level 3's ID is 10
                case 11: return 4; // level 4's ID is 11
                case 12: return 5; // level 5's ID is 12
                default: return 0; // any other values can return zero
            }
        }
    }

    // class for level 1 (level ID #7)
    internal class Level1Coordinates : AbstractLevelCoordinate {
        // override the default level coordinate list implementation
        public Level1Coordinates() {
            // create the list of coordinates
            this.coordinateList = new List<LevelCoordinate>() {
                new LevelCoordinate("The First Hop  ", new Vector3(-928f, -527f,    1f)), // THE FIRST HOP
                new LevelCoordinate("Leave The Spawn", new Vector3(-894f, -486f,    1f)), // OUT OF THE SPAWN
                new LevelCoordinate("Around The Wall", new Vector3(-682f, -390f,    1f)), // AROUND THE WALL
                new LevelCoordinate("Tower Approach ", new Vector3(-477f, -355f,    1f)), // TOWER APPROACHES
                new LevelCoordinate("Zig Zags       ", new Vector3(-363f, -321f,   48f)), // ZIG ZAGS
                new LevelCoordinate("Box Hops       ", new Vector3(-328f, -313f,  116f)), // BOX HOPPING
                new LevelCoordinate("More Box Hops  ", new Vector3(-221f, -301f,  113f)), // EVEN MORE BOX HOPPING
                new LevelCoordinate("Turret Dash    ", new Vector3(-100f, -288f,  113f)), // DASH PAST THE TURRETS
                new LevelCoordinate("Up And Around  ", new Vector3( 113f, -245f,   98f)), // UP AND AROUND
                new LevelCoordinate("Other Side     ", new Vector3(  80f, -190f,   -5f)), // TO THE OTHER SIDE
                new LevelCoordinate("Tumblers 1     ", new Vector3(  84f, -191f, -123f)), // TUMBLERS 1
                new LevelCoordinate("tumblers 2     ", new Vector3( -76f, -131f, -136f)), // TUMBLERS 2
                new LevelCoordinate("Tumblers 3     ", new Vector3(-161f, -131f, -209f)), // TUMBLERS 3
                new LevelCoordinate("Fans!          ", new Vector3(-247f, -125f, -124f)), // COMMON ENEMY: FANS
                new LevelCoordinate("Finish Dash 1  ", new Vector3(-241f,  -95f,   -5f)), // DASH FOR THE FINISH 1
                new LevelCoordinate("Finish Dash 2  ", new Vector3(-335f,  -82f,    1f)), // DASH FOR THE FINISH 2
                new LevelCoordinate("Ending         ", new Vector3(-238f,   19f,   -6f))  // ENDING
            };
        }
        // override the ToString method to return the level description
        public override string ToString() {
            // return what level this is
            return "Level 1 (Level ID 7)";
        }
    }

    // class for all of the coordinates
    public class LevelCoordinate {
        // private field for the coordinates
        public Vector3 Location { get; private set; }
        // private field for the name of the coordinate
        public String Name { get; private set; }
        // private boolean indicator for whether or not the coordinate has been visited
        public Boolean Visited { get; set; }
        // constructor ( name + coordinate )
        public LevelCoordinate(String name, Vector3 location) {
            // set the name of the coordinate
            this.Name = name;
            // set the location of the coordinate
            this.Location = location;
        }
        // override the ToString method
        public override string ToString() {
            // make a string builder for formatting the string
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.Name);
            // stringBuilder.Append(" (");
            // stringBuilder.Append(Math.Round(this.Location.x)).Append(", ");
            // stringBuilder.Append(Math.Round(this.Location.y)).Append(", ");
            // stringBuilder.Append(Math.Round(this.Location.z)).Append(")");
            // return the constructed string
            return stringBuilder.ToString();
        }
    }

}