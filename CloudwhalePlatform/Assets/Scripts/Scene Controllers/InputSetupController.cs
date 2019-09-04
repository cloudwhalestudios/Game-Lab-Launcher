using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSetupController : BaseSetupController
{
    [Header("Input Steps")]
    [SerializeField] int primaryStepIndex = 0;
    [SerializeField] int secondaryStepIndex = 2;
    [SerializeField] int finalConfirmStepIndex = 4;


    [Header("User Feedback")]
    [SerializeField] List<SetupStep.TranslatedText> buttonInUseTextKeys;

    [Header("Debug")]
    [SerializeField, ReadOnly] bool waitingForNextInput;

    [SerializeField, ReadOnly] KeyCode primaryKey;
    [SerializeField, ReadOnly] bool confirmPrimaryKey;

    [SerializeField, ReadOnly] KeyCode secondaryKey;
    [SerializeField, ReadOnly] bool confirmSecondaryKey;

    [SerializeField, ReadOnly] bool confirmFinal;

    void Start ()
    {
        StartCoroutine(InputSetupCoroutine());
    }

    private IEnumerator InputSetupCoroutine()
    {
        for (int i = 0; i < setupSteps.Count; i++)
        {
            yield return StartNewCoroutine(StepCoroutine(i));
        }

        yield return null;
    }

    private IEnumerator StepCoroutine(int index)
    {
        SetupStep s = setupSteps[index];
        // Update Text display
        tMPTitle.text = s.showTitle ? s.title : "";

        StartNewTextUpdateCoroutine(s.textTranslationKeys);

        waitingForNextInput = false;

        if (index == primaryStepIndex) // wait on primary button press
        {
            while (primaryKey == KeyCode.None)
            {
                yield return null;
                CheckForNewInput();
            }
        }
        else if (index == primaryStepIndex + 1) // wait on primary button confirm
        {
            while (!confirmPrimaryKey)
            {
                yield return null;
                confirmPrimaryKey = primaryKey == GetKeyInput();
            }
            waitingForNextInput = true;
        }
        else if (index == secondaryStepIndex) // wait on secondary button press
        {
            while (secondaryKey == KeyCode.None)
            {
                yield return null;
                CheckForNewInput();
            }
        }
        else if (index == secondaryStepIndex + 1) // wait on secondary button press
        {
            while (!confirmSecondaryKey)
            {
                yield return null;
                confirmSecondaryKey = secondaryKey == GetKeyInput();
            }
        }
        else if (index == finalConfirmStepIndex) // give user choice to accept or redo button setup
        {
            while (!confirmFinal)
            {
                yield return null;
                KeyCode pressed = GetKeyInput();
                if (pressed == primaryKey) // success, go to next setup
                {
                    ScenePartsManager.Instance.ContinueSetup();
                }
                else if (pressed == secondaryKey) // abort, restart button setup
                {
                    ScenePartsManager.Instance.RedoCurrentSetup();
                }
            }
        }
        else
        {
            yield return currentUpdateTextRoutine;
        }

        yield return null;
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
                    StartNewTextUpdateCoroutine(buttonInUseTextKeys);
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