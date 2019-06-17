using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputBarButtonState : MonoBehaviour
{
    public static event Action<InputBarButtonState> ObtainButtonStateFocus;
    public static event Action<InputBarButtonState> LooseButtonStateFocus;

    [Header("Button Bar Configuration", order = 0)]
    public RectTransform buttonsParent;
    public Button defaultSelection;
    public RectTransform selectionIndicator;
    public RectTransform indicatorLocalPositon;
    public int loopCount = 1;

    [Header("Alternative Configuration", order = 1)]
    [SerializeField] private Button alternative;
    public UnityEvent alternativeEvents;
    public bool HasAlternatives =>
        (Alternative != null && Alternative.onClick != null && Alternative.onClick.GetPersistentEventCount() > 0)
        || (alternativeEvents != null && alternativeEvents.GetPersistentEventCount() > 0);
    public int ButtonCount => Buttons != null ? Buttons.Count : -1;
    public int TimerIterationCount => ButtonCount * loopCount;

    public List<Button> Buttons { get => buttons; private set => buttons = value; }
    public Button Alternative { get => alternative; set => alternative = value; }

    List<Button> buttons;
    int selectedIndex;
    bool singleButtonState = false;

    public void SetActive(bool activate = true)
    {
        if (activate)
        {
            Buttons = new List<Button>(buttonsParent.GetComponentsInChildren<Button>());
            selectedIndex = Buttons.IndexOf(defaultSelection);

            MoveIndicator();
            singleButtonState = Buttons.Count < 2;

            ObtainButtonStateFocus?.Invoke(this);
            gameObject.SetActive(true);
           
        }
        else
        {
            LooseButtonStateFocus?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    public void ChangeCurrentButtonDisplay(string displayText, Sprite icon)
    {
        var btn = Buttons[selectedIndex];
        ChangeButtonDisplay(btn, displayText, icon);
    }

    public void OverriderButton(Button button, string displayText, Sprite icon, UnityAction callback, bool removeListeners = true)
    {
        if (button == null && button != Alternative && !Buttons.Contains(button)) return;

        //print("overriding button " + button.name + " " + name);
        if (removeListeners) button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback());

        ChangeButtonDisplay(button, displayText, icon);
    }

    void ChangeButtonDisplay(Button button, string displayText, Sprite icon)
    {
        if (displayText != null && displayText != "") button.GetComponentInChildren<TextMeshProUGUI>().text = displayText;
        if (icon != null) button.GetComponentInChildren<Image>().sprite = icon;
        TextResizer.AdjustSizeDelta(button.GetComponentInChildren<TextMeshProUGUI>());
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
        selectedIndex = Buttons.IndexOf(defaultSelection);
        MoveIndicator();
    }

    private void InputBarController_TimerElapsed()
    {
        // Move to next item
        selectedIndex = (selectedIndex + 1) % Buttons.Count;
        MoveIndicator();
    }
    
    private void InputBarController_TimerStopped()
    {
        // Stop indicating
        selectionIndicator.gameObject.SetActive(false);
    }

    void MoveIndicator()
    {
        selectionIndicator.gameObject.SetActive(true);

        if (!singleButtonState)
        {

            selectionIndicator.SetParent(Buttons[selectedIndex].transform);
            selectionIndicator.localPosition = indicatorLocalPositon.localPosition;
        }
    }

    public void Select()
    {
        if (!isActiveAndEnabled) return;
        AudioManager.Instance?.PlaySound(AudioManager.Instance.Accept);
        Buttons[selectedIndex]?.onClick?.Invoke();
    }

    public void AltSelect()
    {
        AudioManager.Instance?.PlaySound(AudioManager.Instance.Abort);
        if (Alternative != null)
        {
            //Debug.Log("Invoking onCLick alt");
            Alternative?.onClick?.Invoke();
            return;
        }
        //Debug.Log("Invoking state alt " + alternativeEvents.GetPersistentEventCount());
        alternativeEvents?.Invoke();
    }
}
