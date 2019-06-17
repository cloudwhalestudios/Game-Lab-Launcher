using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionsController : MonoBehaviour
{
    public PopupMenu menu;
    public InputBarButtonState gameOptionsButtonState;

    /*[Space]
    public Sprite pauseVideoSprite;
    public Sprite resumeVideoSprite;
    [ReadOnly] public bool isPaused = false;
    */
    [Space]
    public Image targetAudioImage;
    public Sprite mutedAudioSprite;
    public Sprite unmutedAudioSprite;
    /*
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
        UpdateMenuImages();

        baseLoopCount = gameOptionsButtonState.LoopCount;
        gameOptionsButtonState.LoopCount *= menu.Options.Count;
        gameOptionsButtonState.SetActive();

        gameOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());
    }

    void UpdateMenuImages()
    {
        targetAudioImage.sprite = PlatformPreferences.Current.GameMute ? mutedAudioSprite : unmutedAudioSprite;
    }

    public void Close()
    {
        if (baseLoopCount > 0) gameOptionsButtonState.LoopCount = baseLoopCount;
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
    */
    public void ToggleAudio()
    {
        PlatformPreferences.Current.GameMute = !PlatformPreferences.Current.GameMute;
        UpdateMenuImages();
    }
    /*
    public void ToggleFavorite()
    {

    }
    */
}
