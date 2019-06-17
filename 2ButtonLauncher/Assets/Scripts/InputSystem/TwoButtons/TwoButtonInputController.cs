using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class TwoButtonInputController : BaseInputController
        {
            public bool overrideControls = false;
            public bool disableAutoLoad = false;

            public KeyCode primaryOverride;
            public KeyCode secondaryOverride;

            public InputKeyEvent primary = new InputKeyEvent();
            public InputKeyEvent secondary = new InputKeyEvent();

            public void Start()
            {
                if (overrideControls)
                {
                    SetControls(primaryOverride, secondaryOverride);
                } 
                else if (!disableAutoLoad)
                {
                    SetControls(PlatformPreferences.Current.Keys);
                }
            }

            public override void SetControls(params KeyCode[] keys)
            {
                inputKeyEvents = new List<InputKeyEvent>();
                if (keys == null) return;

                if (keys.Length > 0)
                {
                    primary.Key = keys[0];
                    if (keys[0] != KeyCode.None) inputKeyEvents.Add(primary);
                }

                if (keys.Length > 1)
                {
                    secondary.Key = keys[1];
                    if (keys[1] != KeyCode.None) inputKeyEvents.Add(secondary);
                }
            }
        }
    }
}