using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using AccessibilityInputSystem;
using UnityEngine.Events;

public class InputBarController : MonoBehaviour
{
    public static InputBarController Instance { get; private set; }


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

    [Header("Alternative Control")]
    public int timerFillsBeforeAlternative = 4;
    public UnityEvent defaultAlternativeAction;
    public static event Action CurrentAlternativeAction;

    InputBarButtonState activeButtonState;
    Coroutine activeTimerRoutine;

    float currentFillTime;
    int currentTimerFills;
    private float elapsedTime;

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
                    ShowButtonState(false);
                    break;

                case BarMode.Buttons:
                    ShowButtonState(true);
                    break;

                default:
                    break;
            }
            activeMode = value;
        }
    }

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }

        activeButtonState = defaultButtonState;
    }

    void ShowButtonState(bool buttonsActive)
    {
        activeButtonState?.gameObject.SetActive(buttonsActive);
        textPrompt?.gameObject.SetActive(!buttonsActive);
    }

    public void StartTimer(bool activate, float fillTimeOverride = -1f, int timerFillsOverride = -1)
    {
        ChangeFillTime(fillTimeOverride);
        ChangeMaxTimerFills(timerFillsOverride);

        if (activeTimerRoutine != null)
        {
            StopCoroutine(activeTimerRoutine);
            activeTimerRoutine = null;

            TimerStopped?.Invoke();
        }
        if (activate)
        {
            activeTimerRoutine = StartCoroutine(TimerRoutine());
        }
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
    }

    public void ChangeFillTime(float fillTimeOverride)
    {
        ResetTimer();
        currentFillTime = fillTimeOverride > 0 ? fillTimeOverride : PlatformPreferences.Current.ReactionTime;
    }

    public void ChangeMaxTimerFills(int timerFillsOverride)
    {
        currentTimerFills = timerFillsOverride > 0 ? timerFillsOverride : timerFillsBeforeAlternative;
    }

    public void OnEnable()
    {
        PlatformPlayer.Primary += PlatformPlayer_MainPrimary;
        PlatformPlayer.Secondary += PlatformPlayer_MainSecondary;
        InputBarButtonState.ObtainButtonStateFocus += InputBarButtonState_ObtainButtonStateFocus;
        InputBarButtonState.LooseButtonStateFocus += InputBarButtonState_LooseButtonStateFocus;
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
            activeButtonState.SetActive(false);
        }

        activeButtonState = focusState;
        StartTimer(true, -1, activeButtonState.TimerIterationCount);
        ActiveMode = BarMode.Buttons;
    }

    private void InputBarButtonState_LooseButtonStateFocus(InputBarButtonState dropState)
    {
        if (activeButtonState == dropState)
        {
            activeButtonState = null;
            StartTimer(false);
            ActiveMode = BarMode.Prompt;
        }
            
    }

    private void UseAlternative()
    {
        if (activeButtonState != null && activeButtonState.isActiveAndEnabled)
        {
            if (activeButtonState.HasAlternatives)
            {
                activeButtonState.AltSelect();
                return;
            }
        }

        AudioManager.Instance?.PlaySound(AudioManager.Instance.Launch);
        if (CurrentAlternativeAction != null)
        {
            Debug.LogWarning("Invoking current alt");
            CurrentAlternativeAction?.Invoke();
            return;
        }

        if (defaultAlternativeAction == null)
            throw new Exception("No default alternative has been set!");

        Debug.LogError("Invoking default alt");
        defaultAlternativeAction?.Invoke();
        
    }

    private void UseSelection()
    {
        if (activeMode == BarMode.Buttons)
        {
            activeButtonState?.Select();
        }
    }

    private IEnumerator TimerRoutine()
    {
        TimerStarted?.Invoke();

        for (int i = 0; i < currentTimerFills; i++)
        {
            ResetTimerDisplay();

            elapsedTime = 0f;

            while (elapsedTime < currentFillTime)
            {
                yield return null;

                elapsedTime += Time.unscaledDeltaTime;

                // Update timer fill
                ResetTimerDisplay();
                UpdateTimerDisplay(elapsedTime / currentFillTime);
            }
            AudioManager.Instance?.PlaySound(AudioManager.Instance.Select);
            TimerElapsed?.Invoke();
            //Debug.LogWarning("timer fill: " + (i + 1) + " of " + maxTimerFills);
            yield return null;
        }

        UseAlternative();
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

    private void ResetTimerDisplay()
    {
        var pivot = timerBar.pivot;
        var scale = timerBar.localScale;
        scale.x = 1f;

        var color = timerBar.GetComponent<Image>().color;
        color.a = 1f;
        switch (timerFillMode)
        {
            case FillMode.LeftFill:
                pivot.x = 1f;
                scale.x = 0f;
                break;
            case FillMode.LeftCollapse:
                pivot.x = 1f;
                break;
            case FillMode.CenterFill:
                pivot.x = .5f;
                scale.x = 0f;
                break;
            case FillMode.CenterCollapse:
                pivot.x = .5f;
                break;
            case FillMode.RightFill:
                pivot.x = 0;
                scale.x = 0f;
                break;
            case FillMode.RightCollapse:
                pivot.x = 0;
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

    public void CheckPlayerExit()
    {
        // TODO give player exit prompt
    }
}
