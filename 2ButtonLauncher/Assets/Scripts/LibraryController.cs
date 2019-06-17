using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LibraryController : MonoBehaviour
{
    public static LibraryController Instance { get; private set; }

    public enum ScreenState
    {
        CategorySelect,
        SubCategorySelect,
        GameSelect,
        GameInfo
    }

    [SerializeField, ReadOnly] private ScreenState currentState;

    [Header("Screen Controllers")]
    [SerializeField] GameInfoController gameInfoController;
    [SerializeField] GameSelectController gameSelectController;
    //[SerializeField] SubCategorySelectController subCategorySelectController;
    [SerializeField] CategorySelectController categorySelectController;

    [Header("Confirmation Prompt Configuration")]
    [SerializeField] GameObject promptPopup;
    [SerializeField] InputBarButtonState promptButtonState;
    [SerializeField] float promptStayTimeMultiplier = 2f;
    [SerializeField, TextArea] string promptQuitMessage = "The launcher will close shortly. Press your main button to cancel.";
    [SerializeField, TextArea] string promptSetupMessage = "You will be taken back to the setup procedure shortly. Press your main button to cancel.";


    [Header("Temporary")]
    [SerializeField] List<GameCategory> launcherCategories;


    public ScreenState CurrentState
    {
        get { return currentState; }
        private set
        {
            currentState = value;
        }
    }

    protected void Awake()
    {
        HidePrompt();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        ViewCategorySelection();
    }

    public void ShowPrompt(UnityAction mainAction, UnityAction alternativeAction, string message = "")
    {
        var promptText = message == "" || message == null ? promptQuitMessage : message;
        promptPopup.GetComponentInChildren<TextMeshProUGUI>().text = promptText;

        promptPopup.SetActive(true);
        promptButtonState.SetActive();

        InputBarController.Instance.OverriderFillTime(PlatformPreferences.Current.ReactionTime * promptStayTimeMultiplier);
        InputBarController.Instance.OverrideMaxTimerFills(1);

        promptButtonState.OverriderButton(promptButtonState.Buttons[0], "Cancel", null, mainAction);
        promptButtonState.OverriderButton(promptButtonState.Alternative, "Proceed", null, alternativeAction);
    }

    public void HidePrompt()
    {
        promptButtonState.SetActive(false);
        promptPopup.SetActive(false);
    }

    public void ViewCategorySelection()
    {
        // TODO Fetch categories
        categorySelectController.OpenCategorySelectScreen(launcherCategories);
        CurrentState = ScreenState.CategorySelect;
    }

    public void ViewGameSelection(GameCategory category)
    {
        gameSelectController.OpenGameSelectScreen(category);
        CurrentState = ScreenState.GameSelect;
    }

    public void ViewGameInfo(GameInfo game)
    {
        gameInfoController.OpenGameInfoScreen(game);
        CurrentState = ScreenState.GameInfo;
    }

    public void ReturnToPreviousScreen()
    {
        switch (currentState)
        {
            case ScreenState.CategorySelect:

                break;
            /*case ScreenState.SubCategorySelect: // TODO add sub category select
                currentState = ScreenState.CategorySelect;
                ReturnToPreviousScreen();
                break;
                */
            case ScreenState.GameSelect:
                // Open Category Select
                gameSelectController.CloseGameSelectScreen(true);
                categorySelectController.ReopenCategorySelectScreen();
                currentState = ScreenState.CategorySelect;
                break;

            case ScreenState.GameInfo:
                // Open Game Select
                gameInfoController.CloseGameInfoScreen();
                gameSelectController.ReopenGameSelectScreen();
                CurrentState = ScreenState.GameSelect;
                break;

            default:
                break;
        }
    }

    public void ResumeCurrentState()
    {
        HidePrompt();
        switch (currentState)
        {
            case ScreenState.CategorySelect:
                categorySelectController.ToggleLauncherOptionsPopup();
                break;
            case ScreenState.GameInfo:
                gameInfoController.ToggleGameOptionsPopup();
                break;
            default:
                break;
        }
    }

    public void QuitLauncher()
    {
        ShowPrompt(ResumeCurrentState, () => PlatformManager.Instance.ChangeScene(PlatformManager.Instance.exitSceneName));        
    }

    public void RedoSetup()
    {
        Debug.Log("Redo setup");
        ShowPrompt(ResumeCurrentState, () => PlatformManager.Instance.ChangeScene(PlatformManager.Instance.setupSceneName), promptSetupMessage);
    }

    public void OpenHomeScreen()
    {
        PlatformManager.Instance.ChangeScene(PlatformManager.Instance.librarySceneName);
    }
}
