using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace DCPMCommon
{
    public static class DCPMSettings
    {
        /*
         ___       _          __  __           _                 
        |   \ __ _| |_ __ _  |  \/  |___ _ __ | |__  ___ _ _ ___ 
        | |) / _` |  _/ _` | | |\/| / -_) '  \| '_ \/ -_) '_(_-< 
        |___/\__,_|\__\__,_| |_|  |_\___|_|_|_|_.__/\___|_| /__/ 

         */

        private static String settingsLocation;
        public static Dictionary<String, String> settingsDictionary;

        /*
          ___             _               _              
         / __|___ _ _  __| |_ _ _ _  _ __| |_ ___ _ _ ___
        | (__/ _ \ ' \(_-<  _| '_| || / _|  _/ _ \ '_(_-<
         \___\___/_||_/__/\__|_|  \_,_\__|\__\___/_| /__/
                                                      
        */

        //Static constructor
        static DCPMSettings()
        {
            settingsLocation = Environment.CurrentDirectory + "\\DeadCore_Data\\Managed\\settings\\";
            settingsDictionary = new Dictionary<String, String>();

            //Ensure the settings directory exis
            Directory.CreateDirectory(settingsLocation);
            settingsLocation += "settings.conf";
            LoadSettingsFromFile();
        }

        /*
         ___ _        _   _      __  __     _   _            _    
        / __| |_ __ _| |_(_)__  |  \/  |___| |_| |_  ___  __| |___
        \__ \  _/ _` |  _| / _| | |\/| / -_)  _| ' \/ _ \/ _` (_-<
        |___/\__\__,_|\__|_\__| |_|  |_\___|\__|_||_\___/\__,_/__/
                                                           
        */

        //Check if a setting exists int he Dictionary
        public static bool SettingExists(String key)
        {
            return settingsDictionary.ContainsKey(key);
        }

        //Get a setting from the settings dictionary
        //ALWAYS CHECK BEFORE USING THIS
        public static String GetSetting(String key)
        {
            return settingsDictionary[key];
        }

        //Get a KeyCode setting from the settings or create it if non existant
        //Included here for ease of use
        public static KeyCode GetKeyCodeSetting(String keySetting, KeyCode defaultKey)
        {
            KeyCode returnVal = defaultKey;

            //Check or create settings
            //Alternatively could use the DeadCore SettingsManager however this encrypts and serializes the data to settings.save and is not easily changed

            //Handle any possible exceptions here so we don't get the MainConsole delegate of the calling plugin removed by accident
            try
            {
                if (SettingExists(keySetting))
                {
                    LogMessage("Using existing '" + keySetting + "' setting");
                    return (KeyCode)System.Enum.Parse(typeof(KeyCode), DCPMSettings.GetSetting(keySetting));
                }
                else
                {
                    LogMessage("Could not find '" + keySetting + "' setting, creating");
                    SetSetting(keySetting, defaultKey);
                    return defaultKey;
                }
            }
            catch (Exception ex)
            {
                LogMessage("Error: Cannot convert setting '{0}' = '{1} to UnityEngine.KeyCode", keySetting, GetSetting(keySetting));
                LogMessage(ex.ToString());
            }

            return returnVal;
        }

        //Create a new setting in the settings dictionary
        //I'd recommend using your own naming convention for settings eg: 'DCPM-ConsoleKey'
        public static void SetSetting(String key, System.Object value)
        {
            if (settingsDictionary.ContainsKey(key))
            {
                LogMessage("Changed setting '{0}' = '{1}' to '{2}'", key, settingsDictionary[key], value);
                settingsDictionary[key] = value.ToString();
            }
            else
            {
                LogMessage("Added setting '{0}' = '{1}'", key, value);
                settingsDictionary.Add(key, value.ToString());
            }
            SaveSettingsToFile();
        }

        //Load the existing settings from the settings file
        private static void LoadSettingsFromFile()
        {
            LogMessage("Loading settings from settings.conf file");

            String line;
            String[] strings;
            String[] separators = { " = " };

            using (StreamReader sr = new StreamReader(new FileStream(settingsLocation, FileMode.OpenOrCreate, FileAccess.Read)))
            {
                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine();

                    strings = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                    settingsDictionary.Add(strings[0], strings[1]);
                    LogMessage("Added setting '{0}' = '{1}' to the settings dictionary", strings[0], strings[1]);
                }
            }

            LogMessage("Settings loaded");
        }

        //Commits the current settingsDictionary to a file
        private static void SaveSettingsToFile()
        {
            LogMessage("Saving settings to settings.conf file");

            using (StreamWriter sw = new StreamWriter(new FileStream(settingsLocation, FileMode.Create, FileAccess.Write)))
            {
                foreach (KeyValuePair<String, String> entry in settingsDictionary)
                {
                    sw.WriteLine("{0} = {1}", entry.Key, entry.Value);
                }
            }

            LogMessage("Settings saved to file");
        }

        //Log a message to the default logfile
        private static void LogMessage(String message, params System.Object[] args)
        {
            //Using a named argument since there is also an optional logFile parameter
            DCPMLogger.LogMessage("[DCPMSettings] " + message, args: args);
        }
    }
}
