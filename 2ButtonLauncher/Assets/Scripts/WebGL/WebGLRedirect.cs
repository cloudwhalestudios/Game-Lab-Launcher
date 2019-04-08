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
#if UNITY_WEBGL
            JSLib.Redirect(Config.BASE_URL + url);
#else
            Debug.Log($"Should open {Config.BASE_URL + url} right now");
#endif
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