using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptionsController : MonoBehaviour
{
    public PopupMenu menu;
    public InputBarButtonState gameOptionsButtonState;

    /*[Space]
    public Sprite pauseVideoSprite;
    public Sprite resumeVideoSprite;
    [ReadOnly] public bool isPaused = false;

    [Space]
    public Sprite muteAudioSprite;
    public Sprite unmuteAudioSprite;
    [ReadOnly] public bool isMuted = false;

    [Space]
    public Sprite favoriteSprite;
    public Sprite unfavoriteSprite;
    [ReadOnly] public bool isFavorite = false;
    */
    int baseLoopCount;


    public bool IsOpen => menu.gameObject.activeInHierarchy;

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
            menu.SelectNextOption();
            gameOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());
        }
    }

    public void Open()
    {
        menu.ShowMenu();
        baseLoopCount = gameOptionsButtonState.loopCount;
        gameOptionsButtonState.loopCount *= menu.Options.Count;
        gameOptionsButtonState.SetActive();
        gameOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());
    }


    public void Close()
    {
        if (baseLoopCount > 0) gameOptionsButtonState.loopCount = baseLoopCount;
        menu.ShowMenu(false);
        gameOptionsButtonState.SetActive(false);
    }

    public void SelectMenuOption()
    {
        InputBarController.Instance.ResetTimer();
        menu.UseSelectedOption();
    }
    /*
    public void ToggleVideoPlayback()
    {
        if (isPaused)
        {
            
        }
    }

    public void ToggleAudio()
    {

    }

    public void ToggleFavorite()
    {

    }
    */
}
