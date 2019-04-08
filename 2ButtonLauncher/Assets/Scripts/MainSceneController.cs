using AccessibilityInputSystem.TwoButtons;
using PlayerPreferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static PlatformManager;

public class MainSceneController : MonoBehaviour
{
    

    public enum Category
    {
        Recent,
        Favorite,
        All,
        Casual,
        Arcade,
        Puzzle
    }

    [Header("Game Handling")]
    public GameName selectedGame = GameName.None;
    public bool gameMenuOpen = false;
    public BaseMenuController gameMenuController;

    [Header("State Menu Handling")]
    public BaseStateMenuController stateMenuController;

    [Space]
    public float transitionTime = 0.5f;
    public RectTransform categoryStateMenu;
    public RectTransform optionStateMenu;
    public RectTransform gameMenu;

    Coroutine showGameMenu;

    private void OnEnable()
    {
        PlatformPlayer.MainPrimary += PlatformPlayer_MainPrimary;
        PlatformPlayer.MainSecondary += PlatformPlayer_MainSecondary;
    }

    private void OnDisable()
    {
        PlatformPlayer.MainPrimary += PlatformPlayer_MainPrimary;
        PlatformPlayer.MainSecondary += PlatformPlayer_MainSecondary;
    }

    private void Start()
    {
        StateMenuManager.Instance.SetStateMenuController(stateMenuController);
        StateMenuManager.Instance.StartIndicating();
    }

    private void PlatformPlayer_MainPrimary() => Select();
    private void PlatformPlayer_MainSecondary() => Return();


    public void Select()
    {
        Debug.Log("Selecting...");
        if (gameMenuOpen)
        {
            MenuManager.Instance.SelectItem();
        }
        else
        {
            StateMenuManager.Instance.Select();
        }
    }

    public void Return()
    {
        if (gameMenuOpen)
        {
            ReturnFromGameSelection();
        }
        else
        {
            StateMenuManager.Instance.Return();
        }
    }

    [EnumAction(typeof(GameName))]
    public void SelectGame(int gameName)
    {
        selectedGame = (GameName)gameName;
        showGameMenu = StartCoroutine(GameMenuIndication());
    }

    void ReturnFromGameSelection()
    {
        if (gameMenuOpen && gameMenu != null && showGameMenu != null)
        {
            gameMenuOpen = false;

            StopCoroutine(showGameMenu);
            showGameMenu = null;

            MenuManager.Instance.StartIndicating(false);
            StartCoroutine(ShowMenu(gameMenu, false));

            StateMenuManager.Instance.StartIndicating(true);

            selectedGame = GameName.None;
        }
        else
        {
            Debug.LogWarning("Game Menu trying to be closed, when it's not open!");
        }
    }

    public void PlayGame()
    {
        if (gameMenuOpen && selectedGame != GameName.None)
        {
            PlatformManager.Instance.LaunchGame(selectedGame);
        }
    }

    public void Favorite()
    {
        // TODO add game to favorites
    }

    public void ApplyCategory()
    {
        // TODO apply category filter
    }

    public void ResetIntput()
    {
        PlatformPreferences.Current.CompletedSetup = false;
        SceneManager.LoadScene(PlatformManager.Instance.setupSceneName);
    }

    public void Exit()
    {
        PlatformManager.Instance.Exit();
    }

    public void ShowCategoryStateMenu(bool show = true)
    {
        if (categoryStateMenu != null)
        {
            StartCoroutine(ShowMenu(categoryStateMenu, show));
        }
    }
    public void ShowOptionStateMenu(bool show = true)
    {
        if (optionStateMenu != null)
        {
            StartCoroutine(ShowMenu(optionStateMenu, show));
        }
    }

    IEnumerator GameMenuIndication()
    {
        gameMenuOpen = true;
        StateMenuManager.Instance.StartIndicating(false);
        yield return StartCoroutine(ShowMenu(gameMenu, true));
        MenuManager.Instance.SetMenuController(gameMenuController);
        MenuManager.Instance.StartIndicating();
    }

    IEnumerator ShowMenu(RectTransform menu, bool show)
    {
        var elapsedTime = 0f;
        if (show)
        {
            menu.localScale = new Vector3(1, 0, 1);
            menu.gameObject.SetActive(true);
        }
        else menu.localScale = new Vector3(1, 1, 1);
        while (elapsedTime < transitionTime)
        {
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
            var percentage = Mathf.Clamp01(elapsedTime / transitionTime);
            if (!show) percentage = 1 - percentage;
            menu.localScale = new Vector3(1, percentage, 1);
        }
        menu.gameObject.SetActive(show);
    }
}
