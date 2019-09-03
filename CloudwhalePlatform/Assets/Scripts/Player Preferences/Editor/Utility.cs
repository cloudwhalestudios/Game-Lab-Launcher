using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PlayerPreferences
{
    public class Utility : MonoBehaviour
    {
        public static event Action PlayerPreferenceCleared;

#if UNITY_EDITOR
        [MenuItem("Tools/Clear Player Preferences")]
        private static void ClearPlayerPreferenceButton()
        {
            PlayerPrefs.DeleteAll();
            PlayerPreferenceCleared?.Invoke();
        }
        [MenuItem("Tools/Toggle Setup Flag")]
        private static void ToggleSetupCompleteFlag()
        {
            PlatformPreferences.Current.CompletedSetup = !PlatformPreferences.Current.CompletedSetup;
            Debug.Log("'CompletedSetup' flag set to " + PlatformPreferences.Current.CompletedSetup);
        }

#endif
    }
}