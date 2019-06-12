
using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ReactionSceneController : MonoBehaviour
{
    [Serializable]
    public class InputConfigImage
    {
        public enum ConfigState
        {
            Unselected,
            HighlightUnselected,
            Selected,
            HighlightSelected,
            Highlighted,
            Hidden
        }

        [SerializeField, ReadOnly] ConfigState state = ConfigState.Unselected;

        public Image unselected;
        public Image selected;
        public Image highlighted;
        public TextMeshProUGUI keyMapping;

        public ConfigState State
        {
            get => state;
            set
            {
                state = value;
                UpdateImageState();
            }
        }

        void UpdateImageState()
        {
            switch (State)
            {
                case ConfigState.Unselected:
                    unselected.enabled = true;
                    selected.enabled = false;
                    highlighted.enabled = false;
                    break;
                case ConfigState.HighlightUnselected:
                    unselected.enabled = true;
                    selected.enabled = false;
                    highlighted.enabled = true;
                    break;
                case ConfigState.Selected:
                    unselected.enabled = false;
                    selected.enabled = true;
                    highlighted.enabled = false;
                    break;
                case ConfigState.HighlightSelected:
                    unselected.enabled = false;
                    selected.enabled = true;
                    highlighted.enabled = true;
                    break;
                case ConfigState.Highlighted:
                    unselected.enabled = false;
                    selected.enabled = false;
                    highlighted.enabled = true;
                    break;
                case ConfigState.Hidden:
                    unselected.enabled = false;
                    selected.enabled = false;
                    highlighted.enabled = false;
                    break;
            }
        }
    }

    public float transitionTime = .3f;

    public TextMeshProUGUI dialogText;

    public InputConfigImage inputPrompt;

    public GameObject confirmationMenu;

    public int calibrationAmount = 3;
    public float retryTimeMultiplier = 4f;
    public float currentReactionSpeed;
    public float defaultReactionSpeed = 2f;
    public float minWaitTime = 2f;
    public float maxWaitTime = 4f;
    public float reactionSpeedMultiplier = 2f;

    public string introText = "Please press the primary button when you see the circle appear.";
    public string promptText = "Press the primary button!";
    public string confirmText = "Do you feel comfortable with this reaction speed?";
    public string redoExtraText = "Lets try that again!<br>";
    public string warningText = "Please wait until a prompt shows up.";

    bool enableMenuInput = false;
    bool enableReact = false;

    List<float> reactionTimes = new List<float>();
    float startTime;

    public void OnEnable()
    {
        PlatformPlayer.Secondary += PlatformPlayer_SetupSecondary;
        PlatformPlayer.Primary += PlatformPlayer_SetupPrimary;
    }
    public void OnDisable()
    {
        PlatformPlayer.Secondary -= PlatformPlayer_SetupSecondary;
        PlatformPlayer.Primary -= PlatformPlayer_SetupPrimary;
    }
    
    private void PlatformPlayer_SetupPrimary()
    {
        if (enableMenuInput) MenuManager.Instance.SelectItem();
        else if (enableReact)
        {
            React();
        }
    }

    private void PlatformPlayer_SetupSecondary()
    {
        if (enableMenuInput) RedoSetup();
    }

    public void Start()
    {
        ResetCalibration();
    }

    void SetButtonMappingText(InputConfigImage input, string key = "")
    {
        input.keyMapping.text = key;
    }

    IEnumerator CalibrationRoutine()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Select);
        // 1 Intro calibration
        SetPromptState(InputConfigImage.ConfigState.Highlighted);
        dialogText.text = introText;
        yield return new WaitForSecondsRealtime(defaultReactionSpeed);

        // 2 Reaction test
        for (int i = 0; i < retryTimeMultiplier; i++)
        {
            yield return ReactionPrompt();
        }
        CalculateReactionSpeed();

        // 3 Completed? Start : Back to 1
        ShowConfirmationDialog();

        yield break;
    }

    void SetPromptState(InputConfigImage.ConfigState state)
    {
        inputPrompt.State = state;
    }

    IEnumerator ReactionPrompt()
    {
        // hide
        SetPromptState(InputConfigImage.ConfigState.Hidden);
        dialogText.text = "";

        // wait for random time
        var waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSecondsRealtime(waitTime);

        // show prompt
        SetPromptState(InputConfigImage.ConfigState.Highlighted);
        dialogText.text = promptText;

        // wait for reaction
        enableReact = true;
        startTime = Time.time;
        while (enableReact)
        {
            yield return null;
        }

        SetPromptState(InputConfigImage.ConfigState.HighlightSelected);

        var time = Time.time - startTime;

        reactionTimes.Add(time);
        dialogText.text = "Speed: " + time;

        yield return new WaitForSeconds(defaultReactionSpeed);
    }

    IEnumerator SetupCompleteRoutine(float completeMessageTime)
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Accept);

        yield break;
    }

    private void ShowConfirmationDialog(bool show = true)
    {
        enableMenuInput = show;
        if (!show)
        {
            MenuManager.Instance.HideMenu();
        }
        else
        {
            MenuManager.Instance.ShowMenu();
        }
    }

    private void React()
    {
        enableReact = false;
    }

    public void RedoSetup()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Abort);
        List<float> reactionTimes = new List<float>();

        PlatformPreferences.Current.ReactionTime = defaultReactionSpeed;

        ResetCalibration();
    }

    public void ResetCalibration()
    {
        List<float> reactionTimes = new List<float>();
        ShowConfirmationDialog(false);

        SetButtonMappingText(inputPrompt);

        StartCoroutine(CalibrationRoutine());
    }

    public void ConfirmSetup()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.GameSelected);
        Debug.Log("Finished calibration");

        PlatformPreferences.Current.ReactionTime = currentReactionSpeed;

        SceneManager.LoadScene(PlatformManager.Instance.mainSceneName);
    }

    private void CalculateReactionSpeed()
    {
        var averageSpeed = CalculateAverageSpeed();
        var minSpeed = GetFastestTime();
        var maxSpeed = GetSlowestTime();

        Debug.Log("Average: " + averageSpeed);
        Debug.Log("Fastest: " + minSpeed);
        Debug.Log("Slowest: " + maxSpeed);

        var difference = maxSpeed - minSpeed;
        var reactionSpeed = averageSpeed * reactionSpeedMultiplier;
        reactionSpeed = Mathf.Round(reactionSpeed * 100f) / 100f;

        dialogText.text = "Comfortable Reaction Speed: " + reactionSpeed;
        currentReactionSpeed = reactionSpeed;
        PlatformPreferences.Current.ReactionTime = currentReactionSpeed;
    }

    private float CalculateAverageSpeed()
    {
        var totalTime = 0f;

        foreach (var time in reactionTimes)
        {
            totalTime += time;
        }

        return totalTime / reactionTimes.Count;
    }

    private float GetFastestTime()
    {
        var fastest = Mathf.Infinity;

        foreach (var time in reactionTimes)
        {
            if (time < fastest) fastest = time;
        }

        return fastest;
    }

    private float GetSlowestTime()
    {
        var slowest = 0f;

        foreach (var time in reactionTimes)
        {
            if (time > slowest) slowest = time;
        }

        return slowest;
    }
}
