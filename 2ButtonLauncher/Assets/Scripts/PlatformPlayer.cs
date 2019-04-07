using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using System;

public class PlatformPlayer : ActiveInputHandler
{
    public static event Action SetupPrimary;
    public static event Action SetupSecondary;

    protected override void TBPrimary_InputEvent(KeyCode primaryKey)
    {
        switch (PlatformManager.Instance.CurrentState)
        {
            case PlatformManager.PlatformState.Setup:
                SetupPrimary?.Invoke();
                break;
            case PlatformManager.PlatformState.Main:
                break;
            default:
                break;
        }
    }

    protected override void TBSecondary_InputEvent(KeyCode secondaryKey)
    {
        switch (PlatformManager.Instance.CurrentState)
        {
            case PlatformManager.PlatformState.Setup:
                SetupSecondary?.Invoke();
                break;
            case PlatformManager.PlatformState.Main:
                break;
            default:
                break;
        }
    }
}
