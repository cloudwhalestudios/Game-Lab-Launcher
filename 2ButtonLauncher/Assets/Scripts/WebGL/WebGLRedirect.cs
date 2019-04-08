using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


namespace WebGLIntegration
{
    public class WebGLRedirect
    {
        private static void Redirect(string url)
        {
            var preferences = JsonUtility.ToJson(PlatformPreferences.Current);
#if UNITY_WEBGL
            if (preferences != "{}") 
            {
                JSLib.RedirectWithParams(Config.BASE_URL + url, preferences);
            }
            else 
            {
                JSLib.Redirect(Config.BASE_URL + url);
            }
#endif
            Debug.Log($"Should open {Config.BASE_URL + url + preferences} right now");
        }

        public static void OpenLauncher()
        {
            Redirect(Config.LAUNCHER_URL);
        }

        public static void OpenGame(string gameUrl)
        {
            Redirect(Config.GAME_BASE_URL + gameUrl);
        }
    }
}