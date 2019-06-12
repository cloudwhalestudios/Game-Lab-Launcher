using AccessibilityInputSystem;
using AccessibilityInputSystem.TwoButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupController : MonoBehaviour
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
            Highlighted
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
            }
        }
    }

    public float transitionTime = .3f;

    public TextMeshProUGUI dialogText;

    public InputConfigImage primaryInput;
    public InputConfigImage secondaryInput;

    public GameObject confirmationMenu;

    public string welcomeText           = "Welcome to the Cloudwhale launcher!<br>";
    public string primaryInputText      = "Please use the button that you are most comfortable<br>using in order to set it up as your primary control button!";
    public string secondaryInputText    = "Great! Now please use the button that you would like<br>as the secondary control button!";
    public string setupCompleteText     = "Perfect!";
    public string confirmText           = "Are you sure you want to use these buttons as your input?";
    public string redoExtraText         = "Lets try that again!<br>";
    public string warningText           = "The button you pressed is already your primary button. Please try using another one!";

    bool allowInput = false;
    bool isReset = false;

    public void OnEnable()
    {
        BasePlayerManager.NewPlayerBeingAdded += BasePlayerManager_NewPlayerBeingAdded;
        BasePlayerManager.NewPlayerKeyInUse += BasePlayerManager_NewPlayerKeyInUse;
        BasePlayerManager.NewPlayerAdded += BasePlayerManager_NewPlayerAdded;
        BasePlayerManager.PlayerRemoved += BasePlayerManager_PlayerRemoved;

        PlatformPlayer.Secondary += PlatformPlayer_SetupSecondary;
        PlatformPlayer.Primary += PlatformPlayer_SetupPrimary;
    }
    public void OnDisable()
    {
        BasePlayerManager.NewPlayerBeingAdded -= BasePlayerManager_NewPlayerBeingAdded;
        BasePlayerManager.NewPlayerKeyInUse -= BasePlayerManager_NewPlayerKeyInUse;
        BasePlayerManager.NewPlayerAdded -= BasePlayerManager_NewPlayerAdded;
        BasePlayerManager.PlayerRemoved -= BasePlayerManager_PlayerRemoved;

        PlatformPlayer.Secondary -= PlatformPlayer_SetupSecondary;
        PlatformPlayer.Primary -= PlatformPlayer_SetupPrimary;
    }

    public void Start()
    {
        if (BasePlayerManager.Instance.PlayerCount > 0)
        {
            isReset = true;
            BasePlayerManager.Instance.RemovePlayer();
        }

        ShowConfirmationDialog(false);

        SetButtonMappingText(primaryInput);
        SetButtonMappingText(secondaryInput);

        StartCoroutine(WelcomeRoutine(2f));
    }

    void SetButtonMappingText(InputConfigImage input, string key = "")
    {
        input.keyMapping.text = key;
    }

    IEnumerator WelcomeRoutine(float welcomeMessageTime)
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Select);
        BasePlayerManager.Instance.shouldCheckForNewPlayer = false;

        yield return StartCoroutine(SwitchElementsRoutine(welcomeText, InputConfigImage.ConfigState.Unselected, InputConfigImage.ConfigState.Unselected));
        yield return new WaitForSecondsRealtime(welcomeMessageTime);

        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Select);
        BasePlayerManager.Instance.shouldCheckForNewPlayer = true;
        yield return StartCoroutine(SwitchElementsRoutine(primaryInputText, InputConfigImage.ConfigState.Highlighted, InputConfigImage.ConfigState.Unselected));
    }

    IEnumerator SwitchElementsRoutine(string text, InputConfigImage.ConfigState primaryState, InputConfigImage.ConfigState secondaryState, float transitionTime = 0f)
    {
        //yield return new WaitForSecondsRealtime(transitionTime);
        // TODO add smooth transition effect
        dialogText.text = text;
        primaryInput.State = primaryState;
        secondaryInput.State = secondaryState;

        yield break;
    }

    public void RedoSetup()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Abort);
        SetButtonMappingText(primaryInput);
        SetButtonMappingText(secondaryInput);

        ShowConfirmationDialog(false);

        BasePlayerManager.Instance.RemovePlayer();

        BasePlayerManager.Instance.shouldCheckForNewPlayer = true;
    }

    public void ConfirmSetup()
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.GameSelected);
        Debug.Log("Finished setup");
        PlatformPreferences.Current.CompletedSetup = true;
        SceneManager.LoadScene(PlatformManager.Instance.reactionSceneName);
    }

    private void BasePlayerManager_PlayerRemoved(int newCount)
    {
        // Input was reset
        if (!isReset && newCount == 0)
        {
            StartCoroutine(SwitchElementsRoutine(redoExtraText + primaryInputText, InputConfigImage.ConfigState.Highlighted, InputConfigImage.ConfigState.Unselected));
        }
        else
        {
            isReset = false;
        }
    }


    private void PlatformPlayer_SetupPrimary()
    {
        if (allowInput) MenuManager.Instance.SelectItem();
    }

    private void PlatformPlayer_SetupSecondary()
    {
        if (allowInput) RedoSetup();
    }
    
    private void BasePlayerManager_NewPlayerBeingAdded(string name, BasePlayerManager.KeyEventSpecifier[] keySpecifiers)
    {
        // Selected Primary - Wait for Secondary
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Accept);
        SetButtonMappingText(primaryInput, keySpecifiers[0].Key.ToString());

        StartCoroutine(SwitchElementsRoutine(secondaryInputText, InputConfigImage.ConfigState.Selected, InputConfigImage.ConfigState.Highlighted));
    }

    private void BasePlayerManager_NewPlayerKeyInUse(string message, BasePlayerManager.KeyEventSpecifier keySpecifier)
    {
        // Button is already primary key
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Abort);
        StartCoroutine(SwitchElementsRoutine(warningText, InputConfigImage.ConfigState.Selected, InputConfigImage.ConfigState.Highlighted));
    }

    private void BasePlayerManager_NewPlayerAdded(BasePlayer player)
    {
        AudioManager.Instance.PlaySoundNormally(AudioManager.Instance.Accept);
        // Setup Complete
        SetButtonMappingText(secondaryInput, player.Keys[1].ToString());

        StartCoroutine(SetupCompleteRoutine(2f));
    }

    IEnumerator SetupCompleteRoutine(float completeMessageTime)
    {
        BasePlayerManager.Instance.shouldCheckForNewPlayer = false;

        StartCoroutine(SwitchElementsRoutine(setupCompleteText, InputConfigImage.ConfigState.HighlightSelected, InputConfigImage.ConfigState.HighlightSelected));
        yield return new WaitForSecondsRealtime(completeMessageTime);

        ShowConfirmationDialog();
        yield return StartCoroutine(SwitchElementsRoutine(confirmText, InputConfigImage.ConfigState.HighlightSelected, InputConfigImage.ConfigState.HighlightSelected));
    }

    private void ShowConfirmationDialog(bool show = true)
    {
        allowInput = show;
        if (!show)
        {
            MenuManager.Instance.HideMenu();
        }
        else
        {
            MenuManager.Instance.ShowMenu();
        }
    }
}
