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

    private void Awake()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        //infoControllerButtonState.SetActive();
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
        screen.HideInfo();
        infoControllerButtonState.SetActive(false);
    }

    public void LaunchGame()
    {
        PlatformManager.Instance.LaunchGame(selectedGame);
    }

    public void ToggleVideoPlayback()
    {
        if (screen.IsVideoPlaying) screen.PauseVideo();
        else screen.PlayVideo();
    }

    public void ToggleGameOptionsPopup()
    {
        Debug.Log("I am being pressed: " + gameOptionsController.IsOpen);
        if (gameOptionsController.IsOpen)
        {
            gameOptionsController.Close();
            infoControllerButtonState.SetActive();
        }
        else
        {
            gameOptionsController.Open();
        }
    }
}
