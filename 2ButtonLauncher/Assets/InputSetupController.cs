using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;

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

    [Header("Dialog Configuration", order = 0)]
    [Header("Introduction", order = 1)]
    public float introTextTime = 3f;
    public List<string> introText = new List<string>
    {
        "Hello and welcome to the Cloudwhale Platform!",
        "Please setup your controls by following the instructions.",
    };
    public TextLocation introLocation = TextLocation.Screen;

    [Header("Primary Button", order = 2)]
    public float pSetupTime = 10f;
    public List<string> pSetupText = new List<string>
    {
        "Please now press the button you are most comfortable using"
    };
    public TextLocation pSetupLocation = TextLocation.InputBar;

    [Header("Primary Confirm", order = 3)]
    public float pConfirmTime = 10f;
    public List<string> pConfirmText = new List<string>
    {
        "Are you comfortable with your primary button?" + "\nPlease press it to continue",
    };
    public TextLocation pConfirmLocation = TextLocation.Screen;

    [Header("Secondary Button", order = 4)]
    public float sSetupTime = 10f;
    public List<string> sSetupText = new List<string>
    {
        "Please now press your alternative button"
    };
    public TextLocation sSetupLocation = TextLocation.InputBar;

    [Header("Secondary Button Notice", order = 5)]
    public float sNoticeTime = 10f;
    public List<string> sNoticeText = new List<string>
    {
        "The button you used is already your primary button",
        "Please press another to use as an alternative"
    };
    public TextLocation sNoticeLocation = TextLocation.InputBar;

    [Header("Setup Confirm", order = 6)]
    public float sConfirmTime = 10f;
    public List<string> sConfirmText = new List<string>
    {
        "Are you comfortable with your button setup?",
        "\nPlease answer by selecting one of the options below",
    };
    public TextLocation sConfirmLocation = TextLocation.Screen;

    [Header("Reset Introduction", order = 7)]
    public float resetTextTime = 3f;
    public List<string> resetText = new List<string>
    {
        "Lets try that again!",
        "Please setup your controls by following the instructions.",
    };
    public TextLocation resetLocation = TextLocation.Screen;

    [Space]
    [SerializeField, ReadOnly] bool waitingForNextInput;

    [SerializeField, ReadOnly] KeyCode primaryKey;
    [ReadOnly] public bool confirmPrimaryKey;

    [SerializeField, ReadOnly] KeyCode secondaryKey;
    [ReadOnly] public bool confirmSecondary;

    TwoButtonInputController inputController;
    InputBarController barController;
    bool isReset = false;
    Coroutine setupRoutine;
    Coroutine textDisplayRoutine;

    // Start is called before the first frame update
    void Start()
    {
        inputController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TwoButtonInputController>();
        barController = GameObject.FindGameObjectWithTag("InputBar")?.GetComponent<InputBarController>();
        setupRoutine = StartCoroutine(InputSetupRoutine());
    }

    private IEnumerator InputSetupRoutine()
    {
        // Reset
        ResetConfiguration();

        // Reset introduction
        if (isReset)
        {
            yield return textDisplayRoutine = StartCoroutine(
                TextUpdateRoutine(resetTextTime, resetLocation, resetText)
                );
        }
        // Introduction
        else
        {
            yield return textDisplayRoutine = StartCoroutine(
                TextUpdateRoutine(introTextTime, introLocation, introText)
                );
        }

        // First button
        // TODO: What to do if timer runs out?
        ShowPrimary(true);


        StopTextDisplayUpdate();
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(pSetupTime, pSetupLocation, pSetupText));
        while (primaryKey == KeyCode.None)
        {
            CheckForNewInput();
            yield return null;
        }
        
        // Confirm - Wait for input bar interaction using primary
        ShowPrimary(true, true);

        StopTextDisplayUpdate();
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(pConfirmTime, pConfirmLocation, pConfirmText));
        inputController.SetControls(primaryKey);

        // Second Button
        ShowPrimary(false);
        ShowSecondary(true);
        waitingForNextInput = true;

        StopTextDisplayUpdate();
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(sSetupTime, sSetupLocation, sSetupText));
        while (secondaryKey == KeyCode.None)
        {
            CheckForNewInput();
            yield return null;
        }

        // Confirm
        ShowSecondary(true, true);

        StopTextDisplayUpdate();
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(sConfirmTime, sConfirmLocation, sConfirmText));
        inputController.SetControls(primaryKey);

        yield break;
    }

    IEnumerator TextUpdateRoutine(float displayTime, TextLocation textLocation, List<string> textStrings)
    {
        textInputBarPrompt.text = "";
        textScreenPrompt.text = "";
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
        }

        for (int i = 0; i < textStrings.Count; i++)
        {
            textField.text = textStrings[i];
            yield return new WaitForSecondsRealtime(displayTime);
        }
    }

    private void ResetConfiguration()
    {
        primaryKey = KeyCode.None;
        secondaryKey = KeyCode.None;
        waitingForNextInput = false;

        StopTextDisplayUpdate();

        ShowPrimary(false);
        ShowSecondary(false);

        if (PlatformPreferences.Current.Keys?.Length == 0)
        {
            isReset = true;
        }
    }

    void StopTextDisplayUpdate()
    {
        if (textDisplayRoutine == null) return;
        StopCoroutine(textDisplayRoutine);
        textDisplayRoutine = null;
    }

    public void RestartSetup()
    {
        isReset = true;
        StopAllCoroutines();

        setupRoutine = null;
        textDisplayRoutine = null;
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
}
