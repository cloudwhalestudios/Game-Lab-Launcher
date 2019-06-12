using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using AccessibilityInputSystem;

public class InputBarController : MonoBehaviour
{
    public static event Action TimerStarted;
    public static event Action TimerElapsed;
    public static event Action TimerStopped;

    public enum BarMode
    {
        Prompt,
        Buttons
    }

    enum FillMode
    {
        LeftFill,
        LeftCollapse,
        CenterFill,
        CenterCollapse,
        RightFill,
        RightCollapse,
        AlphaFill,
        AlphaFade
    }

    [SerializeField, ReadOnly] private BarMode activeMode;

    [Header("Indicator Bar", order = 0)]
    public RectTransform timerBar;
    [SerializeField] FillMode timerFillMode;

    [Header("Text Prompt", order = 1)]
    public TextMeshProUGUI textPrompt;

    [Header("Default Button State", order = 2)]
    public InputBarButtonState defaultButtonState;

    InputBarButtonState activeButtonState;

    Coroutine activeTimerRoutine;

    bool controlSetup = false;

    public BarMode ActiveMode
    {
        get
        {
            return activeMode;
        }
        set
        {
            switch (value)
            {
                case BarMode.Prompt:
                    ToggleVisibility(false);
                    break;
                case BarMode.Buttons:
                    ToggleVisibility(true);
                    break;
                default:
                    break;
            }
            activeMode = value;
        }
    }

    void ToggleVisibility(bool buttonsActive)
    {
        activeButtonState?.gameObject.SetActive(buttonsActive);
        textPrompt?.gameObject.SetActive(!buttonsActive);
    }

    public void ActivateTimer(bool activate, float fillTimeOverride = -1f)
    {
        if (activeTimerRoutine != null)
        {
            StopCoroutine(activeTimerRoutine);
            activeTimerRoutine = null;

            TimerStopped?.Invoke();
        }
        if (activate)
        {
            activeTimerRoutine = StartCoroutine(TimerRoutine(fillTimeOverride));
        }
    }

    public void Awake()
    {
        activeButtonState = defaultButtonState;
    }

    public void OnEnable()
    {
        PlatformPlayer.Primary += PlatformPlayer_MainPrimary;
        PlatformPlayer.Secondary += PlatformPlayer_MainSecondary;
        InputBarButtonState.ObtainButtonStateFocus += InputBarButtonState_ObtainButtonStateFocus;
    }

    public void OnDisable()
    {
        PlatformPlayer.Primary -= PlatformPlayer_MainPrimary;
        PlatformPlayer.Secondary -= PlatformPlayer_MainSecondary;
        InputBarButtonState.ObtainButtonStateFocus -= InputBarButtonState_ObtainButtonStateFocus;
    }

    private void PlatformPlayer_MainPrimary()
    {
        UseSelection();
    }

    private void PlatformPlayer_MainSecondary()
    {
        UseAlternative();
    }
    private void InputBarButtonState_ObtainButtonStateFocus(InputBarButtonState focusState)
    {
        if (activeButtonState != null)
        {
            activeButtonState.shouldIndicate = false;
            activeButtonState.gameObject.SetActive(false);
        }


        activeButtonState = focusState;
        activeButtonState.gameObject.SetActive(true);
        activeButtonState.shouldIndicate = true;
    }

    private void UseAlternative()
    {
        if (activeButtonState != null)
        {
            activeButtonState?.AltSelect();
        }
        else
        {
            // Default exit prompt
        }
    }

    private void UseSelection()
    {
        if (activeMode == BarMode.Buttons)
        {
            activeButtonState?.Select();
        }
    }

    private IEnumerator TimerRoutine(float fillTime)
    {
        TimerStarted?.Invoke();

        fillTime = fillTime <= 0 ? PlatformPreferences.Current.ReactionTime : fillTime;

        while (true)
        {
            ResetTimer();
            var elapsedTime = 0f;

            while (elapsedTime < fillTime)
            {
                yield return null;

                elapsedTime += Time.deltaTime;

                // Update timer fill
                UpdateTimerDisplay(elapsedTime / fillTime);
            }

            TimerElapsed?.Invoke();

            yield return null;
        }
    }

    private void UpdateTimerDisplay(float percentage)
    {
        var pivot = timerBar.pivot;
        var scale = timerBar.localScale;
        var color = timerBar.GetComponent<Image>().color;
        switch (timerFillMode)
        {
            case FillMode.LeftFill:
            case FillMode.CenterFill:
            case FillMode.RightFill:
                scale.x = Mathf.Lerp(0f, 1f, percentage);
                break;

            case FillMode.LeftCollapse:
            case FillMode.CenterCollapse:
            case FillMode.RightCollapse:
                scale.x = Mathf.Lerp(1f, 0f, percentage);
                break;

            case FillMode.AlphaFill:
                color.a = Mathf.Lerp(0f, 1f, percentage);
                break;

            case FillMode.AlphaFade:
                color.a = Mathf.Lerp(1f, 0f, percentage);
                break;

            default:
                break;
        }

        timerBar.pivot = pivot;
        timerBar.localScale = scale;
        timerBar.GetComponent<Image>().color = color;
    }

    private void ResetTimer()
    {
        var pivot = timerBar.pivot;
        var scale = timerBar.localScale;
        var color = timerBar.GetComponent<Image>().color;
        switch (timerFillMode)
        {
            case FillMode.LeftFill:
                pivot.x = 1f;
                scale.x = 0f;
                break;
            case FillMode.LeftCollapse:
                pivot.x = 1f;
                scale.x = 1f;
                break;
            case FillMode.CenterFill:
                pivot.x = .5f;
                scale.x = 0f;
                break;
            case FillMode.CenterCollapse:
                pivot.x = .5f;
                scale.x = 1f;
                break;
            case FillMode.RightFill:
                pivot.x = 0;
                scale.x = 0f;
                break;
            case FillMode.RightCollapse:
                pivot.x = 0;
                scale.x = 1f;
                break;
            case FillMode.AlphaFill:
                color.a = 0;
                break;
            case FillMode.AlphaFade:
                color.a = 1f;
                break;
            default:
                break;
        }

        timerBar.pivot = pivot;
        timerBar.localScale = scale;
        timerBar.GetComponent<Image>().color = color;
    }
}
