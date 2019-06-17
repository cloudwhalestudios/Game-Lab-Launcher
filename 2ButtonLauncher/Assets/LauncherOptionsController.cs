using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherOptionsController : MonoBehaviour
{
    public PopupMenu menu;
    public InputBarButtonState launcherOptionsButtonState;

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

        baseLoopCount = launcherOptionsButtonState.LoopCount;
        launcherOptionsButtonState.LoopCount *= menu.Options.Count;
        launcherOptionsButtonState.SetActive();
        launcherOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());
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
}
