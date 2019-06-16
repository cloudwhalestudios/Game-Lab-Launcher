using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptionsController : MonoBehaviour
{
    public PopupMenu menu;
    public InputBarButtonState gameOptionsButtonState;

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
        gameOptionsButtonState.loops = menu.Options.Count * gameOptionsButtonState.loops;
        gameOptionsButtonState.SetActive();
        gameOptionsButtonState.ChangeCurrentButtonDisplay(menu.GetText(), menu.GetIcon());

    }


    public void Close()
    {
        menu.ShowMenu(false);
        gameOptionsButtonState.SetActive(false);
    }

    public void SelectMenuOption()
    {
        menu.UseSelectedOption();
    }


    public void ToggleFavorite()
    {

    }

    public void ToggleGameAudio()
    {

    }
}
