using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LauncherOptionsController : MonoBehaviour
{
    public PopupMenu menu;
    public InputBarButtonState launcherOptionsButtonState;

    [Space]
    public Image targetAudioImage;
    public Sprite mutedAudioSprite;
    public Sprite unmutedAudioSprite;

    public bool IsOpen => menu.gameObject.activeInHierarchy;

    int baseLoopCount;

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
            launcherOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());
        }
    }

    public void Open()
    {
        menu.ShowMenu();

        UpdateMenuImages();

        baseLoopCount = launcherOptionsButtonState.LoopCount;
        launcherOptionsButtonState.LoopCount *= menu.Options.Count;
        launcherOptionsButtonState.SetActive();
        launcherOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());
    }

    void UpdateMenuImages()
    {
        targetAudioImage.sprite = PlatformPreferences.Current.PlatformMute ? mutedAudioSprite : unmutedAudioSprite;
    }

    public void Close()
    {
        if (baseLoopCount > 0 ) launcherOptionsButtonState.LoopCount = baseLoopCount;
        menu.ShowMenu(false);
        launcherOptionsButtonState.SetActive(false);
    }

    public void SelectMenuOption()
    {
        menu.UseSelectedOption();
    }

    public void ToggleAudio()
    {
        if (PlatformPreferences.Current.PlatformMute)
        {
            targetAudioImage.sprite = unmutedAudioSprite;
            AudioManager.Instance.UnmuteAudio();
        }
        else
        {
            targetAudioImage.sprite = mutedAudioSprite;
            AudioManager.Instance.MuteAudio();
        }
    }
}
