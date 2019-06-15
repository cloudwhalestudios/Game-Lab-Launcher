using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReactionSetupController : MonoBehaviour
{
    public TextMeshProUGUI textScreenPrompt;

    [Space]
    public InputBarButtonState popupButtonState;
    public ReactionSpeedMenu popupMenu;

    [Space]
    public InputBarButtonState confirmSpeedButtonState;

    [Header("Reaction Speed Options", order = 0)]
    public bool allowManual = true;
    public char unitName = 's';
    public List<float> reactionSpeedPresets;
    public int menuLoopCount = 1;


    [Header("Dialog Configuration", order = 1)]
    [Header("Introduction", order = 2)]
    public float introTextTime = 5f;
    [TextArea]
    public List<string> introText = new List<string>
    {
        "You will now setup your reaction speed. You can either select a preset or setup your own manually by following a simple test.",
    };

    [Header("Confirm Reaction Speed", order = 3)]
    [TextArea]
    public List<string> rConfirmText = new List<string>
    {
        "You will now setup your reaction speed. You can either select a preset or setup your own manually by following a simple test.",
    };

    [Space]
    [SerializeField, ReadOnly] bool waitingForSelection;

    [SerializeField, ReadOnly] float reactionSpeed;
    [SerializeField, ReadOnly] bool confirmSpeed;

    int optionCount;
    int currentOptionIndex;

    InputBarController barController;
    Coroutine previousSetupRoutine;
    Coroutine currentSetupRoutine;
    Coroutine textDisplayRoutine;
    Action currentAlternativeAction;

    private void Start()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        StartReactionSetup();
    }

    void ChangeAlternativeAction(Action altAction)
    {
        InputBarController.CurrentAlternativeAction -= currentAlternativeAction;
        currentAlternativeAction = altAction;
        InputBarController.CurrentAlternativeAction += currentAlternativeAction;
    }

    // Intro
    public void StartReactionSetup()
    {
        // Debug.Log("Starting Reaction Setup");
        currentSetupRoutine = StartCoroutine(StartReactionSetupRoutine());
    }

    private IEnumerator StartReactionSetupRoutine()
    {
        ResetSetup();

        UpdateTextDisplay(introTextTime, introText, introTextTime * introText.Count);
        yield return new WaitForSecondsRealtime(introTextTime * introText.Count);

        StartSpeedSelection();
    }

    public void StartSpeedSelection()
    {
        // Debug.Log("Starting R1");
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(SpeedSelectionRoutine());
        StopCoroutine(previousSetupRoutine);
    }

    private IEnumerator SpeedSelectionRoutine()
    {
        ChangeAlternativeAction(StartReactionSetup);
        ResetSpeedSelection();

        // Show menu
        popupMenu.ShowMenu(reactionSpeedPresets, allowManual, unitName, SelectSpeedOption);
        popupButtonState.SetActiveState();

        // Start selection
        waitingForSelection = true;

        InputBarController.TimerElapsed += InputBarController_TimerElapsed;
        barController.ActivateTimer(true, reactionSpeedPresets[0], menuLoopCount * optionCount);

        yield return null;
    }

    // Pick Speed
    public void SelectSpeedOption(int optionIndex)
    {
        if (waitingForSelection)
        {
            InputBarController.TimerElapsed -= InputBarController_TimerElapsed;
            waitingForSelection = false;

            if (optionIndex < optionCount - 1)
            {
                reactionSpeed = reactionSpeedPresets[optionIndex];
                // Confirm speed
                StartSpeedConfirm();
            }
            else
            {
                // Start manual setup
                StartManualSetup();
            }
        }
    }
    
    // Manual setup
    public void StartManualSetup()
    {

    }

    // Test speed
    private void StartSpeedConfirm()
    {
        // Debug.Log("Starting R2");

        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(SpeedConfirmRoutine());

        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(StartSpeedSelection);
    }

    private IEnumerator SpeedConfirmRoutine()
    {
        // Confirm - Ensure the user is comfortable with the setup
        popupMenu.gameObject.SetActive(false);

        UpdateTextDisplay(reactionSpeed / rConfirmText.Count, rConfirmText, reactionSpeed);
        barController.ChangeMaxTimerFills(1);

        confirmSpeedButtonState.SetActiveState();
        while(!confirmSpeed)
        {
            yield return null;
        }

        CompleteSetup();
    }

    public void ChooseMenuOption()
    {
        SelectSpeedOption(currentOptionIndex);
    }

    public void ConfirmSpeed()
    {
        confirmSpeed = true;
    }

    public void CompleteSetup()
    {
        PlatformPreferences.Current.ReactionTime = reactionSpeed;
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.GameSelected);

        // Debug.Log("Finished reaction setup");
        SceneManager.LoadScene(PlatformManager.Instance.librarySceneName);
    }

    private void InputBarController_TimerElapsed()
    {
        currentOptionIndex = (currentOptionIndex + 1) % optionCount;
        popupMenu.UpdateSelectionDisplay(currentOptionIndex);

        var fillTime = currentOptionIndex < optionCount - 1 ? reactionSpeedPresets[currentOptionIndex] : reactionSpeedPresets[currentOptionIndex - 1];
        barController.ChangeFillTime(fillTime);
    }

    private void ResetSetup()
    {
        ResetSpeedSelection();
    }

    private void ResetSpeedSelection()
    {
        confirmSpeed = false;

        waitingForSelection = false;

        InputBarController.TimerElapsed -= InputBarController_TimerElapsed;

        popupButtonState.gameObject.SetActive(false);
        popupMenu.gameObject.SetActive(false);

        currentOptionIndex = 0;
        optionCount = reactionSpeedPresets.Count + (allowManual ? 1 : 0);
    }


    private void UpdateTextDisplay(float displayTime, List<string> textStrings, float barFillTime = -1f)
    {
        barFillTime = barFillTime <= 0 ? displayTime : barFillTime;

        StopTextDisplayUpdate();
        barController.ActivateTimer(true, barFillTime);
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(displayTime, textStrings));
    }

    void StopTextDisplayUpdate()
    {
        textScreenPrompt.text = "";
        textScreenPrompt.gameObject.SetActive(false);

        barController.ActivateTimer(false);

        if (textDisplayRoutine == null) return;
        StopCoroutine(textDisplayRoutine);
        textDisplayRoutine = null;
    }

    IEnumerator TextUpdateRoutine(float displayTime, List<string> textStrings)
    {
        TextMeshProUGUI textField;
        
        textField = textScreenPrompt;
        textScreenPrompt.gameObject.SetActive(true);

        for (int i = 0; i < textStrings.Count; i++)
        {
            textField.text = textStrings[i];
            yield return new WaitForSecondsRealtime(displayTime);
        }
    }
}
