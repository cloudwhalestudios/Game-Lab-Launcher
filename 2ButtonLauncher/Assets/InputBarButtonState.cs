using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputBarButtonState : MonoBehaviour
{
    public static event Action<InputBarButtonState> ObtainButtonStateFocus;

    [Header("Button Bar Configuration", order = 0)]
    public RectTransform buttonsParent;
    public Button defaultSelection;
    public RectTransform selectionIndicator;
    public RectTransform indicatorLocalPositon;

    [Header("Alternative Configuration", order = 1)]
    public Button alternative;
    public UnityEvent alternativeEvents;
    public bool HasAlternatives() => 
        (alternative != null && alternative.onClick != null && alternative.onClick.GetPersistentEventCount() > 0) 
        || (alternativeEvents != null && alternativeEvents.GetPersistentEventCount() > 0);

    [ReadOnly] public bool shouldIndicate = false;

    List<Button> buttons;
    int selectedIndex;
    bool singleButtonState = false;

    public void SetActiveState()
    {
        buttons = new List<Button>(buttonsParent.GetComponentsInChildren<Button>());
        selectedIndex = buttons.IndexOf(defaultSelection);
        MoveIndicator();
        ObtainButtonStateFocus?.Invoke(this);
        singleButtonState = buttons.Count < 2;
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
        selectedIndex = buttons.IndexOf(defaultSelection);
        MoveIndicator();
    }

    private void InputBarController_TimerElapsed()
    {
        // Move to next item
        selectedIndex = (selectedIndex + 1) % buttons.Count;
        MoveIndicator();
    }

    private void InputBarController_TimerStopped()
    {
        // Stop indicating
        shouldIndicate = false;
    }

    void MoveIndicator()
    {
        if (!singleButtonState)
        {
            selectionIndicator.SetParent(buttons[selectedIndex].transform);
            selectionIndicator.localPosition = indicatorLocalPositon.localPosition;
        }
    }

    public void Select()
    {
        AudioManager.Instance?.PlaySound(AudioManager.Instance.Accept);
        buttons[selectedIndex]?.onClick?.Invoke();
    }

    public void AltSelect()
    {
        AudioManager.Instance?.PlaySound(AudioManager.Instance.Abort);
        if (alternative != null)
        {
            Debug.Log("Invoking onCLick alt");
            alternative?.onClick?.Invoke();
            return;
        }
        Debug.Log("Invoking state alt " + alternativeEvents.GetPersistentEventCount());
        alternativeEvents?.Invoke();
    }
}
