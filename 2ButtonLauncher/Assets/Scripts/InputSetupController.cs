﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using UnityEngine.SceneManagement;

public class InputSetupController : MonoBehaviour
{
    public enum TextLocation
    {
        Screen,
        InputBar
    }

    public TextMeshProUGUI textScreenPrompt;
    public TextMeshProUGUI textInputBarPrompt;

    [Space]
    public GameObject primaryButtonHighlight;
    public GameObject primaryButtonSelected;

    [Space]
    public GameObject secondaryButtonHighlight;
    public GameObject secondaryButtonSelected;

    [Space]
    public InputBarButtonState primaryConfirmButtonState;
    public InputBarButtonState secondaryButtonConfirmState;

    [Space]
    public InputBarButtonState popupDialogState;
    public GameObject popupDialogObject;
    public float idleDialogTimer = 10f;
    [TextArea] public string timeoutReturnText = "Canceling setup shortly.";
    [TextArea] public string timeoutQuitText = "Quitting application shortly.";



    [Header("Dialog Configuration", order = 0)]
    [Header("Introduction", order = 1)]
    public float introTextTime = 3f;
    [TextArea]
    public List<string> introText = new List<string>
    {
        "Hello and welcome to the Cloudwhale Platform!",
        "Please setup your controls by following the instructions.",
    };
    [TextArea]
    public List<string> resetText = new List<string>
    {
        "Lets try that again!",
        "Please setup your controls by following the instructions.",
    };
    public TextLocation introLocation = TextLocation.Screen;
    public int introMaxFillTimes = -1;

    [Header("Primary Button", order = 2)]
    public float pSetupTime = 10f;
    [TextArea]
    public List<string> pSetupText = new List<string>
    {
        "Please now press the button you are most comfortable using"
    };
    public TextLocation pSetupLocation = TextLocation.InputBar;
    public int pSetupMaxFillTimes = -1;

    [Header("Primary Confirm", order = 3)]
    public float pConfirmTime = 10f;
    [TextArea]
    public List<string> pConfirmText = new List<string>
    {
        "Are you comfortable with your primary button?" + "\nPlease press it to continue",
    };
    public TextLocation pConfirmLocation = TextLocation.Screen;
    public int pConfirmMaxFillTimes = -1;

    [Header("Secondary Button", order = 4)]
    public float sSetupTime = 10f;
    [TextArea]
    public List<string> sSetupText = new List<string>
    {
        "Please now press your alternative button"
    };
    [TextArea]
    public List<string> sNoticeText = new List<string>
    {
        "The button you used is already your primary button",
        "Please press another to use as an alternative"
    };
    public TextLocation sSetupLocation = TextLocation.InputBar;
    public int sSetupMaxFillTimes = -1;

    [Header("Setup Confirm", order = 6)]
    public float sConfirmTime = 10f;
    [TextArea]
    public List<string> sConfirmText = new List<string>
    {
        "Are you comfortable with your button setup?",
        "\nPlease answer by selecting one of the options below",
    };
    public TextLocation sConfirmLocation = TextLocation.Screen;
    public int sConfirmMaxFillTimes = -1;

    [Space]
    [SerializeField, ReadOnly] bool waitingForNextInput;

    [SerializeField, ReadOnly] KeyCode primaryKey;
    [ReadOnly] public bool confirmPrimaryKey;

    [SerializeField, ReadOnly] KeyCode secondaryKey;
    [ReadOnly] public bool confirmSecondaryKey;

    TwoButtonInputController inputController;
    InputBarController barController;
    bool isReset = false;
    Coroutine previousSetupRoutine;
    Coroutine currentSetupRoutine;
    Coroutine textDisplayRoutine;
    Action currentAlternativeAction;

    // Start is called before the first frame update
    void Start()
    {
        inputController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TwoButtonInputController>();
        barController = GameObject.FindGameObjectWithTag("InputBar")?.GetComponent<InputBarController>();
        StartInputSetup();
    }

    private void OnDestroy()
    {
        InputBarController.CurrentAlternativeAction -= currentAlternativeAction;
    }

    void ChangeAlternativeAction(Action altAction)
    {
        InputBarController.CurrentAlternativeAction -= currentAlternativeAction;
        currentAlternativeAction = altAction;
        InputBarController.CurrentAlternativeAction += currentAlternativeAction;
    }

    public void DefaultIdleTimeout()
    {
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = null;

        StopAllCoroutines();

        // Show popup
        popupDialogObject.SetActive(true);
        popupDialogState.SetActive();
        barController.StartTimer(true, idleDialogTimer, 1);

        var dialogText = PlatformManager.Instance.canReturn ? timeoutReturnText : timeoutQuitText;
        popupDialogObject.GetComponentInChildren<TextMeshProUGUI>().text = dialogText;

        StartCoroutine(AnyKeyRoutine());
    }

    private IEnumerator AnyKeyRoutine()
    {
        while (GetKeyInput() == KeyCode.None) yield return null;
        ResumeInputSetup();
    }

    public void StartInputSetup()
    {
        StopAllCoroutines();
        // Debug.Log("Starting Input Setup");
        currentSetupRoutine = StartCoroutine(StartInputSetupRoutine());
    }

    private IEnumerator StartInputSetupRoutine()
    {
        // Reset
        ResetSetup();

        // Reset introduction
        float waitTime;
        if (isReset)
        {
            waitTime = introTextTime * resetText.Count;
            UpdateTextDisplay(introTextTime, introLocation, resetText, waitTime);
        }
        // Introduction
        else
        {
            waitTime = introTextTime * introText.Count;
            UpdateTextDisplay(introTextTime, introLocation, introText, waitTime);
            isReset = true;
        }
        yield return new WaitForSecondsRealtime(waitTime);

        StartPrimarySetup();
    }

    public void StartPrimarySetup()
    {
        // Debug.Log("Starting P1");
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(PrimarySetupRoutine());
        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(DefaultIdleTimeout);

    }

    IEnumerator PrimarySetupRoutine()
    {
        ResetPrimarySetup();

        // First button
        ShowPrimary(true);
        UpdateTextDisplay(pSetupTime, pSetupLocation, pSetupText, pSetupTime * pSetupText.Count, pSetupMaxFillTimes);
        while (primaryKey == KeyCode.None)
        {
            yield return null;
            CheckForNewInput();
        }

        StartPrimaryConfirm();
    }

    void StartPrimaryConfirm()
    {
        // Debug.Log("Starting P2");
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(PrimaryConfirmRoutine());
        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(StartPrimarySetup);
    }

    IEnumerator PrimaryConfirmRoutine()
    {

        // Confirm - Wait for input bar interaction using primary
        ShowPrimary(true, true);

        primaryConfirmButtonState.SetActive();
        UpdateTextDisplay(pConfirmTime, pConfirmLocation, pConfirmText, pConfirmTime * pConfirmText.Count, pConfirmMaxFillTimes);

        inputController.SetControls(primaryKey);
        while (!confirmPrimaryKey)
        {
            yield return null;
        }

        StartSecondarySetup();
    }

    public void StartSecondarySetup()
    {
        // Debug.Log("Starting S1");
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(SecondarySetupRoutine());
        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(StartSecondarySetup);
    }

    IEnumerator SecondarySetupRoutine()
    {
        ResetSecondarySetup();

        // Second Button
        ShowPrimary(false);
        ShowSecondary(true);
        waitingForNextInput = true;

        UpdateTextDisplay(sSetupTime, sSetupLocation, sSetupText, sSetupTime * sSetupText.Count, sSetupMaxFillTimes);
        while (secondaryKey == KeyCode.None)
        {
            yield return null;
            CheckForNewInput();
        }

        StartSecondaryConfirm();
    }

    void StartSecondaryConfirm()
    {
        // Debug.Log("Starting S2");
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(SecondaryConfirmRoutine());
        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(StartSecondarySetup);
    }

    IEnumerator SecondaryConfirmRoutine()
    {

        // Confirm - Wait for specific button selection
        ShowSecondary(true, true);

        secondaryButtonConfirmState.SetActive();
        UpdateTextDisplay(sConfirmTime, sConfirmLocation, sConfirmText, -1, sConfirmMaxFillTimes);

        inputController.SetControls(primaryKey, secondaryKey);
        while (!confirmSecondaryKey)
        {
            yield return null;
        }

        CompleteSetup();
    }

    public void CompleteSetup()
    {
        PlatformPreferences.Current.Keys = new KeyCode[] {primaryKey, secondaryKey};
        PlatformPreferences.Current.CompletedSetup = true;

        PlatformManager.Instance.ChangeScene(PlatformManager.Instance.reactionSceneName);
    }

    public void ConfirmPrimaryKey()
    {
        confirmPrimaryKey = true;
    }

    public void ConfirmSecondaryKey()
    {
        confirmSecondaryKey = true;
    }

    private void UpdateTextDisplay(float displayTime, TextLocation textLocation, List<string> textStrings, float barFillTime = -1f, int barFillsOverride = -1)
    {
        barFillTime = barFillTime <= 0 ? displayTime : barFillTime;

        //Debug.Log(displayTime + "<>" + barFillTime);

        StopTextDisplayUpdate();
        barController.StartTimer(true, barFillTime, barFillsOverride);
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(displayTime, textLocation, textStrings));
    }

    IEnumerator TextUpdateRoutine(float displayTime, TextLocation textLocation, List<string> textStrings)
    {
        TextMeshProUGUI textField;
        if (textLocation == TextLocation.InputBar)
        {
            textField = textInputBarPrompt;
            textScreenPrompt.gameObject.SetActive(false);
            barController.ActiveMode = InputBarController.BarMode.Prompt;
        }
        else
        {
            textField = textScreenPrompt;
            textScreenPrompt.gameObject.SetActive(true);
            textInputBarPrompt.gameObject.SetActive(false);
        }

        for (int i = 0; i < textStrings.Count; i++)
        {
            AudioManager.Instance?.PlaySound(AudioManager.Instance.Select);
            textField.text = textStrings[i];
            yield return new WaitForSecondsRealtime(displayTime);
        }
    }

    private void ResetSetup()
    {
        ResetPrimarySetup();
        if (PlatformPreferences.Current.CompletedSetup)
        {
            isReset = true;
        }
        popupDialogObject.SetActive(false);
        popupDialogState.gameObject.SetActive(false);
        barController.StartTimer(false);
    }

    private void ResetPrimarySetup()
    {
        ResetSecondarySetup();
        primaryKey = KeyCode.None;
        inputController.SetControls(primaryKey, secondaryKey);

        confirmPrimaryKey = false;
        waitingForNextInput = false;

        primaryConfirmButtonState.gameObject.SetActive(false);

        ShowPrimary(false);
    }

    private void ResetSecondarySetup()
    {
        secondaryKey = KeyCode.None;
        inputController.SetControls(primaryKey, secondaryKey);

        confirmSecondaryKey = false;

        secondaryButtonConfirmState.gameObject.SetActive(false);

        StopTextDisplayUpdate();

        ShowSecondary(false);
    }


    void StopTextDisplayUpdate()
    {
        textInputBarPrompt.text = "";
        textScreenPrompt.text = "";

        textInputBarPrompt.gameObject.SetActive(false);
        textScreenPrompt.gameObject.SetActive(false);

        barController.StartTimer(false);

        if (textDisplayRoutine == null) return;
        StopCoroutine(textDisplayRoutine);
        textDisplayRoutine = null;
    }

    public void RestartSetup()
    {
        isReset = true;
        StopAllCoroutines();

        currentSetupRoutine = null;
        textDisplayRoutine = null;

        StartInputSetup();
    }

    void ShowPrimary(bool activate, bool selected = false)
    {
        primaryButtonHighlight.SetActive(activate);
        primaryButtonSelected.SetActive(selected);
    }

    void ShowSecondary(bool activate, bool selected = false)
    {
        secondaryButtonHighlight.SetActive(activate);
        secondaryButtonSelected.SetActive(selected);
    }

    void CheckForNewInput()
    {
        var key = GetKeyInput();

        if (key != KeyCode.None)
        {
            if (primaryKey == KeyCode.None)
            {
                primaryKey = key;
            }
            else if (waitingForNextInput && secondaryKey == KeyCode.None)
            {
                if (key == primaryKey)
                {
                    // Key already in use
                    UpdateTextDisplay(sSetupTime, sSetupLocation, sNoticeText, sSetupTime * sNoticeText.Count, sSetupMaxFillTimes);
                    return;
                }

                secondaryKey = key;
            }
        }
    }

    public KeyCode GetKeyInput()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                return kcode;
            }
        }
        return KeyCode.None;
    }

    public void ResumeInputSetup()
    {
        StopAllCoroutines();
        StartInputSetup();
    }

    public void ReturnToLastScene()
    {
        PlatformManager.Instance.ReturnToLastScene();
    }
}
