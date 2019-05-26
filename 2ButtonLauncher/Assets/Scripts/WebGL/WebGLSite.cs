using UnityEngine;
namespace WebGLIntegration
{
    public class WebGLSite
    {
       public static void SetTitle(string title)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            JSLib.SetTitle(title);
#endif
            Debug.LogWarning("Title was set to: " + title);
        }

        public static void ActivateExitCondition()
        {
            SetTitle(Config.EXIT_TITLE_CONDITION);
        }
    }
}