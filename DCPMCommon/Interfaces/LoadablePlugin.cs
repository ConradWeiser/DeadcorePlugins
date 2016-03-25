using System;
using System.Collections.Generic;

//Include and implement this interface on plugins that you want to be automatically loaded by the Plugin Manager.
//If you are making a MonoBehaviour script that you will load at some point on demand later, do not implement this interface on that MonoBehavior
//However try and keep a reference to on demand MonoBehaviours and delete them when Unload is called

namespace DCPMCommon.Interfaces
{
    public interface LoadablePlugin
    {
        void Unload();
        Dictionary<String, String> GetPluginInfo();
    }
}
