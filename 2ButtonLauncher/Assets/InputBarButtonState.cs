using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBarButtonState : MonoBehaviour
{
    public static event Action<InputBarButtonState> ObtainButtonStateFocus;

    [Header("Button Bar Configuration", order = 0)]
    public RectTransform buttonsParent;
    public Button defaultSelection;
    public RectTransform selectionIndicator;

    [Header("Alternative Configuration", order = 1)]
    public Button alternative;

    [ReadOnly] public bool shouldIndicate = false;

    public void SetActiveState()
    {
        ObtainButtonStateFocus?.Invoke(this);
    }

    private void OnEnable()
    {
        InputBarController.TimerStarted += InputBarController_TimerStarted;
        InputBarController.TimerElapsed += InputBarController_TimerElapsed;
        InputBarController.TimerStopped += InputBarController_TimerStopped;
    }
    private void OnDisable()
    {
        InputBarController.TimerStarted -= InputBarController_TimerStarted;
        InputBarController.TimerElapsed -= InputBarController_TimerElapsed;
        InputBarController.TimerStopped -= InputBarController_TimerStopped;
    }

    private void InputBarController_TimerStarted()
    {
        // Reset selection
    }

    private void InputBarController_TimerElapsed()
    {
        // Move indicator
    }

    private void InputBarController_TimerStopped()
    {
        // Stop 
    }

    public void Select()
    {
        
    }

    public void AltSelect()
    {
        throw new NotImplementedException();
    }
}
