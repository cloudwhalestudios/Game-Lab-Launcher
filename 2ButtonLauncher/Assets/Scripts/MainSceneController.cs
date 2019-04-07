using AccessibilityInputSystem.TwoButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [Serializable]
    public class LauncherGame
    {
        public string name;

        public LauncherGame(string name)
        {
            this.name = name;
        }
    }
    [Header("Game Handling")]
    public LauncherGame selectedGame;
    public bool gameSelected = false;
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

    private void PlatformPlayer_MainPrimary()
    {
        StateMenuManager.Instance.Select();
    }

    private void PlatformPlayer_MainSecondary()
    {
        if (gameSelected)
        {
            gameSelected = false;
            StopGameMenuIndication();
            return;
        }
        StateMenuManager.Instance.Return();
    }

    private void Start()
    {
        StateMenuManager.Instance.SetStateMenuController(stateMenuController);
        StateMenuManager.Instance.StartIndicating();
    }

    public void SelectGame(string name)
    {
        gameSelected = true;
        selectedGame = new LauncherGame(name);
        showGameMenu = StartCoroutine(GameMenuIndication());
    }

    IEnumerator GameMenuIndication()
    {
        yield return StartCoroutine(ShowMenu(gameMenu, true));
        MenuManager.Instance.SetMenuController(gameMenuController);
        MenuManager.Instance.StartIndicating();
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

    public void StopGameMenuIndication()
    {
        if (gameMenu != null && showGameMenu != null)
        {
            StopCoroutine(showGameMenu);
            showGameMenu = null;

            MenuManager.Instance.StartIndicating(false);
            StartCoroutine(ShowMenu(gameMenu, false));
        }
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
