using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootLoader : MonoBehaviour
{
    public static void LoadPlatformPlayer()
    {
        BasePlayerManager.Instance.AddPlayer(PlatformPreferences.Current.Keys);
    }
}
