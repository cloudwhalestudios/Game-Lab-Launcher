using AccessibilityInputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectController : MonoBehaviour
{
    public InputBarButtonState selectControllerButtonState;
    public GameSelectScreen screen;

    [Space]
    public string selectionPrefix = "View";

    [Space]
    public GameCategory testCategory;

    [SerializeField, ReadOnly] int lastIndex;
    InputBarController barController;
    int baseLoopCount;

    public bool IsOpen => screen.gameObject.activeInHierarchy && selectControllerButtonState.isActiveAndEnabled;

    private void Start()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        
        if (testCategory != null && testCategory.games.Count > 0)
        {
            OpenGameSelectScreen(testCategory);
        }
    }

    private void OnEnable()
    {
        InputBarController.TimerElapsed += InputBarController_TimerElapsed;
    }

    
    private void OnDisable()
    {
        InputBarController.TimerElapsed -= InputBarController_TimerElapsed;
    }

    private void InputBarController_TimerElapsed()
    {
        if (IsOpen)
        {
            screen.SelectNextGame();
            selectControllerButtonState.ChangeCurrentButtonDisplay(selectionPrefix + " " + screen.GetName(), null);
        }
    }

    public void ReopenGameSelectScreen()
    {
        screen.ShowGames(lastIndex);

        baseLoopCount = selectControllerButtonState.LoopCount;
        selectControllerButtonState.LoopCount = screen.listedGames.Count * baseLoopCount;
        selectControllerButtonState.SetActive();

        selectControllerButtonState.ChangeCurrentButtonDisplay(selectionPrefix + " " + screen.GetName(), null);
    }

    public void OpenGameSelectScreen(GameCategory category)
    {
        OpenGameSelectScreen(category.games);
    }

    public void OpenGameSelectScreen(List<GameInfo> games)
    {
        lastIndex = 0;
        screen.ShowGames(games, selectControllerButtonState.LoopCount, SelectGame, lastIndex);

        baseLoopCount = selectControllerButtonState.LoopCount;
        selectControllerButtonState.LoopCount *= games.Count;
        selectControllerButtonState.SetActive();

        selectControllerButtonState.ChangeCurrentButtonDisplay(selectionPrefix + " " + screen.GetName(), null);
    }

    public void CloseGameSelectScreen(bool cleanupList = false)
    {
        if (cleanupList) lastIndex = 0;

        if (baseLoopCount > 0) selectControllerButtonState.LoopCount = baseLoopCount;

        screen.HideGames(cleanupList);
        selectControllerButtonState.SetActive(false);
    }

    public void SelectCurrentGame()
    {
        screen.UseSelectedGame();
    }

    public void SelectGame(int listingIndex, GameInfo game)
    {
        CloseGameSelectScreen();
        lastIndex = listingIndex;
        selectControllerButtonState.SetActive(false);
        LibraryController.Instance.ViewGameInfo(game);
    }
}
