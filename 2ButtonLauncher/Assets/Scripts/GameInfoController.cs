using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameInfoController : MonoBehaviour
{
    public InputBarButtonState infoControllerButtonState;
    public GameInfoScreen screen;
    public GameOptionsController gameOptionsController;

    InputBarController barController;
    [ReadOnly] public PlatformManager.GameName selectedGame;

    public bool IsOpen => screen.gameObject.activeInHierarchy && infoControllerButtonState.isActiveAndEnabled;

    private void Start()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        gameOptionsController.Close();
        //infoControllerButtonState.SetActive();
    }

    public void OpenGameInfoScreen(GameInfo game)
    {
        OpenGameInfoScreen(game.identifier, game.developer, game.title, game.cover, game.tutorial, game.tutorialUrl);
    }

    public void OpenGameInfoScreen(PlatformManager.GameName name, string developerTitle, string gameTitle, Sprite gameCover, VideoClip clip,  string clipUrl = "")
    {
        selectedGame = name;
        if (clip != null)
        {
            screen.ShowInfo(developerTitle, gameTitle, gameCover, clip, true);
        }
        else
        {
            screen.ShowInfo(developerTitle, gameTitle, gameCover, clipUrl, true);
        }
        infoControllerButtonState.SetActive();
    }

    public void CloseGameInfoScreen()
    {
        selectedGame = PlatformManager.GameName.None;
        gameOptionsController.Close();
        screen.HideInfo();
        infoControllerButtonState.SetActive(false);
    }

    public void LaunchGame()
    {
        PlatformManager.Instance.LaunchGame(selectedGame);
    }

    public void ToggleVideoPlayback()
    {
        ToggleGameOptionsPopup();

        if (screen.IsVideoPlaying) screen.PauseVideo();
        else screen.PlayVideo();
    }

    public void ToggleGameOptionsPopup()
    {
        if (gameOptionsController.IsOpen)
        {
            gameOptionsController.Close();
            infoControllerButtonState.SetActive();
            screen.PlayVideo();
        }
        else
        {
            gameOptionsController.Open();
            screen.PauseVideo();
        }
    }
}
