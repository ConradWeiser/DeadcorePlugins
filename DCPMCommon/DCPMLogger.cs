using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DCPMCommon
{
    public static class DCPMLogger
    {
        /*
         ___       _          __  __           _                 
        |   \ __ _| |_ __ _  |  \/  |___ _ __ | |__  ___ _ _ ___ 
        | |) / _` |  _/ _` | | |\/| / -_) '  \| '_ \/ -_) '_(_-< 
        |___/\__,_|\__\__,_| |_|  |_\___|_|_|_|_.__/\___|_| /__/ 

         */

        private static String logsLocation;

        public delegate void MessageLoggedCallback(String input);
        public static event MessageLoggedCallback MessageLogged;

        /*
          ___             _               _              
         / __|___ _ _  __| |_ _ _ _  _ __| |_ ___ _ _ ___
        | (__/ _ \ ' \(_-<  _| '_| || / _|  _/ _ \ '_(_-<
         \___\___/_||_/__/\__|_|  \_,_\__|\__\___/_| /__/
                                                      
        */

        //Static constructor
        static DCPMLogger()
        {
            logsLocation = Environment.CurrentDirectory + "\\DeadCore_Data\\Managed\\logs\\";
        }

        /*
         ___ _        _   _      __  __     _   _            _    
        / __| |_ __ _| |_(_)__  |  \/  |___| |_| |_  ___  __| |___
        \__ \  _/ _` |  _| / _| | |\/| / -_)  _| ' \/ _ \/ _` (_-<
        |___/\__\__,_|\__|_\__| |_|  |_\___|\__|_||_\___/\__,_/__/
                                                           
        */
        public static void LogMessage(String message, String logFile = "Default.log", params Object[] args)
        {
            //Ensure the directory exisits
            Directory.CreateDirectory(logsLocation);

            if (MessageLogged != null)
            {
                foreach (MessageLoggedCallback d in MessageLogged.GetInvocationList())
                {
                    try
                    {
                        d.Invoke(String.Format(message, args));
                    }
                    catch (Exception ex)
                    {
                        MessageLogged -= d;
                        LogMessage("[DCPMLogger] Delegate targeting " + d.Target + " has been removed as it causes an Exception");
                        LogMessage(ex.ToString());
                    }
                }
            }  

            message = "[" + DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss") + "] " + message;
            using (StreamWriter sw = new StreamWriter(new FileStream(logsLocation + logFile, FileMode.Append, FileAccess.Write)))
            {
                sw.WriteLine(message, args);
            }
        }
    }
}
