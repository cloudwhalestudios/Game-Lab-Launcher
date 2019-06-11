
using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public List<InputConfigImage> inputPrompts;

    public GameObject confirmationMenu;
    public GameObject startMenu;

    public int calibrationAmount = 5;
    public float retryTimeMultiplier = 4f;
    public float currentReactionSpeed;
    public float defaultReactionSpeed = 2f;

    public string welcomeText = "Do you want to calibrate your reaction speed?";
    public string tutorialTextA = "When the process starts, after a short time an input prompt will appear.";
    public string tutorialTextB = "When it appears, please try to use your primary button as soon as you can.";
    public string tutorialTextC = "For better accuracy, this process will repeat a few times.";
    public string promptText = "Press the Primary Button!";
    public string confirmText = "Do you feel comfortable with this reaction speed?";
    public string redoExtraText = "Lets try that again!<br>";
    public string warningText = "Please wait until a prompt shows up.";

    bool enableMenuInput = false;

    List<float> reactionTimes = new List<float>();

    public void OnEnable()
    {
        PlatformPlayer.SetupSecondary += PlatformPlayer_SetupSecondary;
        PlatformPlayer.SetupPrimary += PlatformPlayer_SetupPrimary;
    }
    public void OnDisable()
    {
        PlatformPlayer.SetupSecondary -= PlatformPlayer_SetupSecondary;
        PlatformPlayer.SetupPrimary -= PlatformPlayer_SetupPrimary;
    }
    private void PlatformPlayer_SetupPrimary()
    {
        if (enableMenuInput) MenuManager.Instance.SelectItem();
        else
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
        List<float> reactionTimes = new List<float>();
        ShowConfirmationDialog(false);

        foreach (var prompt in inputPrompts)
        {
            SetButtonMappingText(prompt);
        }

        StartCoroutine(TutorialRoutine(2f));
    }


    void SetButtonMappingText(InputConfigImage input, string key = "")
    {
        input.keyMapping.text = key;
    }

    IEnumerator TutorialRoutine(float welcomeMessageTime)
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Select);
        // 1 Do you want to calibrate?

        // 2 Do part A

        // 3 Do part B

        // 4 Do part C

        // 5 Understood? Start : Back to 1


        yield break;
    }

    IEnumerator SwitchElementsRoutine(string text, int index, InputConfigImage.ConfigState primaryState, float transitionTime = 0f)
    {
        dialogText.text = text;
        inputPrompts[index].State = primaryState;
        yield break;
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
            CalculateReactionSpeed();
            MenuManager.Instance.ShowMenu();
        }
    }

    private void React()
    {
        throw new NotImplementedException();
    }

    public void RedoSetup()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Abort);
        List<float> reactionTimes = new List<float>();

        PlatformPreferences.Current.MenuProgressionTimer = defaultReactionSpeed;

        ShowConfirmationDialog(false);
    }

    public void ConfirmSetup()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.GameSelected);
        Debug.Log("Finished calibration");

        PlatformPreferences.Current.MenuProgressionTimer = currentReactionSpeed;

        SceneManager.LoadScene(PlatformManager.Instance.mainSceneName);
    }

    private void CalculateReactionSpeed()
    {
        throw new NotImplementedException();
    }
}
