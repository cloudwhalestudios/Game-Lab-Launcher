using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using System;

public class PlatformPlayer : ActiveInputHandler
{
    public static event Action Primary;
    public static event Action Secondary;

    protected override void TBPrimary_InputEvent(KeyCode primaryKey)
    {
        Primary.Invoke();
    }

    protected override void TBSecondary_InputEvent(KeyCode secondaryKey)
    {
        Secondary.Invoke();
    }
}
