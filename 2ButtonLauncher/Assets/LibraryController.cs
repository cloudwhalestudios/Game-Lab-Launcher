using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    //[SerializeField] CategorySelectController categorySelectController;

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public void ViewGameInfo(GameInfo game)
    {
        gameInfoController.OpenGameInfoScreen(game);
        CurrentState = ScreenState.GameInfo;

    }

    public void ViewGameSelection(List<GameInfo> games)
    {
        gameSelectController.OpenGameSelectScreen(games);
        CurrentState = ScreenState.GameSelect;

    }

    public void ReturnToPreviousScreen()
    {
        switch (currentState)
        {
            case ScreenState.CategorySelect:

                break;
            case ScreenState.SubCategorySelect: // TODO add sub category select
                currentState = ScreenState.CategorySelect;
                ReturnToPreviousScreen();
                break;

            case ScreenState.GameSelect:
                // Open Category Select
                gameSelectController.CloseGameInfoScreen(true);
                // categorySelectController.ReopenCategoryScreen();
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

    public void QuitLauncher()
    {
        // TODO add timeout prompt asking if they want to quit
        PlatformManager.Instance.ChangeScene(PlatformManager.Instance.exitSceneName);
    }

    public void OpenHomeScreen()
    {

    }
}
