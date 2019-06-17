using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ReactionSetupController : MonoBehaviour
{
    public TextMeshProUGUI textScreenPrompt;

    [Space]
    public InputBarButtonState popupButtonState;
    public ReactionSpeedMenu popupMenu;

    [Space]
    public InputBarButtonState confirmSpeedButtonState;

    [Space]
    public InputBarButtonState calibrationButtonState;
    public GameObject calibrationScreenObject;
    public GameObject calibrationScreenObjectSelected;

    [Space]
    public InputBarButtonState popupDialogState;
    public GameObject popupDialogObject;
    public float idleDialogTimer = 10f;
    [TextArea] public string timeoutText = "Returning to input setup shortly.";

    [Header("Reaction Speed Options", order = 0)]
    public bool allowManual = true;
    public char unitName = 's';
    public List<float> reactionSpeedPresets;
    public int menuLoopCount = 1;

    [Space]
    public int calibrationAttempts = 3;
    public Vector2 calibrationRandomTimeRange = new Vector2(1.5f, 3f);
    public float calibrationTimeOut = 30f;
    public float manualReactionTimeMultiplier = 2f;
    public float manualReactionSpeedFlatIncrease = 2f;


    [Header("Dialog Configuration", order = 1)]
    [Header("Introduction", order = 2)]
    public float introTimePerText = 8f;
    [TextArea]
    public List<string> introText = new List<string>
    {
        "You will now setup your reaction speed. You can either select a preset or setup your own manually by following a simple test.",
    };

    [Header("Manual Intro", order = 3)]
    public float mIntroTimePerText = 15f;
    [TextArea]
    public List<string> mIntroText = new List<string>
    {
        "You selected manual calibration. In a bit, the circle below will disappear and then reapear after some time." +
        "When it reappears, please press your main button as soon as you notice it. This process will repeat a few times." +
        "If you want to quit manual calibration early, you can simply wait for the timer to run out (or use your other button).",
    };

    [Header("Manual Calibration", order = 4)]
    [TextArea]
    public string mCalibrationAppearText = "Please press the button now.";

    public float mCalibrationTextStayTime = 1f;
    [TextArea]
    public string[] mCalibrationSelectedText = { "Great! It took you ", " seconds to react." };

    [TextArea]
    public string[] mCalibrationFinishedText = { "You have completed the calibration! Your calculated reaction time is ", " seconds." };

    [Header("Confirm Reaction Speed", order = 5)]
    [TextArea]
    public List<string> rConfirmText = new List<string>
    {
        "You will now setup your reaction speed. You can either select a preset or setup your own manually by following a simple test.",
    };

    [Space]
    [SerializeField, ReadOnly] bool waitingForSelection;

    [SerializeField, ReadOnly] float reactionSpeed;
    [SerializeField, ReadOnly] bool confirmSpeed;

    [SerializeField, ReadOnly] bool enableReact;
    [SerializeField, ReadOnly] List<float> manualReactionTimes;

    int optionCount;
    int currentOptionIndex = 0;
    InputBarController barController;
    Coroutine previousSetupRoutine;
    Coroutine currentSetupRoutine;
    Coroutine textDisplayRoutine;
    Action currentAlternativeAction;

    private void Start()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        currentOptionIndex = 0;

        StartReactionSetup();
    }

    private void OnDisable()
    {
        InputBarController.CurrentAlternativeAction -= currentAlternativeAction;
        InputBarController.TimerElapsed -= InputBarController_TimerElapsed;
    }

    void ChangeAlternativeAction(Action altAction)
    {
        InputBarController.CurrentAlternativeAction -= currentAlternativeAction;
        currentAlternativeAction = altAction;
        if (isActiveAndEnabled)
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
        barController.OverriderFillTime(idleDialogTimer);
        barController.OverrideMaxTimerFills(1);

        popupDialogObject.GetComponentInChildren<TextMeshProUGUI>().text = timeoutText;
    }

    // Intro
    public void StartReactionSetup()
    {
        StopAllCoroutines();
        // Debug.Log("Starting Reaction Setup");
        currentSetupRoutine = StartCoroutine(StartReactionSetupRoutine());
    }

    private IEnumerator StartReactionSetupRoutine()
    {
        ResetSetup();

        UpdateTextDisplay(introTimePerText, introText, introTimePerText * introText.Count);
        yield return new WaitForSecondsRealtime(introTimePerText * introText.Count);

        StartSpeedSelection();
    }

    public void StartSpeedSelection()
    {
        // Debug.Log("Starting R1");
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(SpeedSelectionRoutine());
        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(StartReactionSetup);
    }

    private IEnumerator SpeedSelectionRoutine()
    {
        ResetSpeedSelection();

        // Show menu
        popupMenu.ShowMenu(reactionSpeedPresets, allowManual, unitName, SelectSpeedOption, currentOptionIndex);
        popupButtonState.SetActive();

        // Start selection
        waitingForSelection = true;

        InputBarController.TimerElapsed += InputBarController_TimerElapsed;
        barController.StartTimer(true, reactionSpeedPresets[0], menuLoopCount * optionCount);

        yield return null;
    }

    // Pick Speed
    public void SelectSpeedOption(int optionIndex)
    {
        if (waitingForSelection)
        {
            popupMenu.gameObject.SetActive(false);
            popupButtonState.gameObject.SetActive(false);
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
        previousSetupRoutine = currentSetupRoutine;
        currentSetupRoutine = StartCoroutine(ManualCalibrationRoutine());

        StopCoroutine(previousSetupRoutine);
        ChangeAlternativeAction(StartSpeedSelection);
    }

    private IEnumerator ManualCalibrationRoutine()
    {
        ResetManualSetup();
        
        // Intro
        UpdateTextDisplay(mIntroTimePerText, mIntroText, mIntroTimePerText * mIntroText.Count);
        ShowCalibrationPrompt(true);
        yield return new WaitForSecondsRealtime(mIntroTimePerText);
        ShowCalibrationPrompt(true, true);
        yield return new WaitForSecondsRealtime(mIntroTimePerText);
        ShowCalibrationPrompt(false, false);

        yield return new WaitForSecondsRealtime(mIntroTimePerText);

        StopTextDisplayUpdate();
        calibrationButtonState.SetActive();
        calibrationButtonState.gameObject.SetActive(false);

        // Beginn calibration
        for (int i = 0; i < calibrationAttempts; i++)
        {
            ShowCalibrationPrompt(false);

            var waitTime = Random.Range(calibrationRandomTimeRange.x, calibrationRandomTimeRange.y);

            UpdateTextDisplay(calibrationTimeOut, new List<string>() { mCalibrationAppearText });
            textScreenPrompt.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(waitTime);
            textScreenPrompt.gameObject.SetActive(true);
            ShowCalibrationPrompt(true);

            enableReact = true;
            calibrationButtonState.gameObject.SetActive(true);
            var startTime = Time.realtimeSinceStartup;
            while (enableReact)
            {
                yield return null;
            }
            calibrationButtonState.gameObject.SetActive(false);

            var time = Time.realtimeSinceStartup - startTime;
            manualReactionTimes.Add(time);

            var formattedTime = Mathf.Round(time * 100f) / 100f;
            var timeText = new List<string>() { mCalibrationSelectedText[0] + formattedTime + mCalibrationSelectedText[1]};

            UpdateTextDisplay(mCalibrationTextStayTime + formattedTime, timeText);

            yield return new WaitForSecondsRealtime(mCalibrationTextStayTime + formattedTime);
        }

        calibrationButtonState.gameObject.SetActive(false);

        // Finished calibration
        CalculateReactionSpeed();

        var reactionTimeText = new List<string>() { mCalibrationFinishedText[0] + reactionSpeed + mCalibrationFinishedText[1] };
        UpdateTextDisplay(reactionSpeed, reactionTimeText);
        yield return new WaitForSecondsRealtime(reactionSpeed);

        StartSpeedConfirm();
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
        ShowCalibrationPrompt(false);

        confirmSpeedButtonState.SetActive();
        UpdateTextDisplay(reactionSpeed / rConfirmText.Count, rConfirmText, reactionSpeed);
        barController.OverrideMaxTimerFills(1);

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

    public void React()
    {
        enableReact = false;
        ShowCalibrationPrompt(true, true);
    }

    public void CompleteSetup()
    {
        PlatformPreferences.Current.ReactionTime = Mathf.Round(reactionSpeed * 100f) / 100f;

        // Debug.Log("Finished reaction setup");
        PlatformManager.Instance.ChangeScene(PlatformManager.Instance.librarySceneName);
    }

    private void InputBarController_TimerElapsed()
    {
        currentOptionIndex = (currentOptionIndex + 1) % optionCount;
        popupMenu.UpdateSelectionDisplay(currentOptionIndex);

        var fillTime = currentOptionIndex < optionCount - 1 ? reactionSpeedPresets[currentOptionIndex] : reactionSpeedPresets[currentOptionIndex - 1];
        barController.OverriderFillTime(fillTime);
    }

    private void ResetSetup()
    {
        ResetSpeedSelection();
        currentOptionIndex = 0;
        popupDialogObject.SetActive(false);
        popupDialogState.gameObject.SetActive(false);
        barController.StartTimer(false);
    }

    private void ResetSpeedSelection()
    {
        ResetManualSetup();

        confirmSpeed = false;

        waitingForSelection = false;

        InputBarController.TimerElapsed -= InputBarController_TimerElapsed;

        popupButtonState.gameObject.SetActive(false);
        popupMenu.gameObject.SetActive(false);

        optionCount = reactionSpeedPresets.Count + (allowManual ? 1 : 0);
    }

    private void ResetManualSetup()
    {
        enableReact = false;
        ShowCalibrationPrompt(false);
        StopTextDisplayUpdate();
    }

    void ShowCalibrationPrompt(bool activate, bool selected = false)
    {
        calibrationScreenObject.SetActive(activate);
        calibrationScreenObjectSelected.SetActive(selected);
    }

    private void UpdateTextDisplay(float displayTime, List<string> textStrings, float barFillTime = -1f)
    {
        barFillTime = barFillTime <= 0 ? displayTime : barFillTime;

        StopTextDisplayUpdate();
        barController.StartTimer(true, barFillTime);
        textDisplayRoutine = StartCoroutine(TextUpdateRoutine(displayTime, textStrings));
    }

    void StopTextDisplayUpdate()
    {
        textScreenPrompt.text = "";
        textScreenPrompt.gameObject.SetActive(false);

        barController.StartTimer(false);

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
            AudioManager.Instance?.PlaySound(AudioManager.Instance.Select);
            textField.text = textStrings[i];
            yield return new WaitForSecondsRealtime(displayTime);
        }
    }

    private void CalculateReactionSpeed()
    {
        var averageSpeed = CalculateAverageSpeed();
        var minSpeed = GetFastestTime();
        var maxSpeed = GetSlowestTime();

        var difference = maxSpeed - minSpeed;
        reactionSpeed = averageSpeed * manualReactionTimeMultiplier;
        reactionSpeed = Mathf.Round(reactionSpeed * 100f) / 100f;
        reactionSpeed += manualReactionSpeedFlatIncrease;
    }

    private float CalculateAverageSpeed()
    {
        var totalTime = 0f;

        foreach (var time in manualReactionTimes)
        {
            totalTime += time;
        }

        return totalTime / manualReactionTimes.Count;
    }

    private float GetFastestTime()
    {
        var fastest = Mathf.Infinity;

        foreach (var time in manualReactionTimes)
        {
            if (time < fastest) fastest = time;
        }

        return fastest;
    }

    private float GetSlowestTime()
    {
        var slowest = 0f;

        foreach (var time in manualReactionTimes)
        {
            if (time > slowest) slowest = time;
        }

        return slowest;
    }

    public void ResumeReactionSetup()
    {
        StopAllCoroutines();
        StartReactionSetup();
    }

    public void ReturnToLastScene()
    {
        PlatformManager.Instance.ReturnToLastScene();
    }
}
